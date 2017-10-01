#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

#endregion

namespace AutoReleaser
{
    /// <summary>
    ///     The ui sync observable collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [DebuggerNonUserCode]
    [PublicAPI]
    [Serializable]
    public class UISyncObservableCollection<TType> : ObservableCollection<TType>
    {
        private class DispoableBlocker : IDisposable
        {
            private readonly UISyncObservableCollection<TType> _collection;

            public DispoableBlocker(UISyncObservableCollection<TType> collection)
            {
                _collection = collection;
                _collection._isBlocked = true;
            }

            public void Dispose()
            {
                _collection._isBlocked = false;
                _collection.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private bool _isBlocked;

        [NotNull]
        private Dispatcher InternalUISynchronize => Application.Current.Dispatcher;

        public void AddRange([NotNull] IEnumerable<TType> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            foreach (var item in enumerable) Add(item);
        }

        public IDisposable BlockChangedMessages()
        {
            return new DispoableBlocker(this);
        }

        #region Methods

        /// <summary>
        ///     The on collection changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_isBlocked) return;
            InternalUISynchronize.Invoke(() => base.OnCollectionChanged(e));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_isBlocked) return;
            InternalUISynchronize.Invoke(() => base.OnPropertyChanged(e));
        }

        #endregion
    }
}