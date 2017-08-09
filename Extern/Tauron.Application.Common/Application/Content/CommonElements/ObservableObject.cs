#region

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Aop.Model;

#endregion

namespace Tauron.Application
{
    /// <summary>The property helper.</summary>
    public static class PropertyHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The extract property name.
        /// </summary>
        /// <param name="propertyExpression">
        ///     The property expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ExtractPropertyName<T>([NotNull] Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null) throw new ArgumentNullException(nameof(propertyExpression));
            var memberExpression = (MemberExpression) propertyExpression.Body;

            return memberExpression.Member.Name;
        }

        #endregion
    }

    /// <summary>The observable object.</summary>
    [Serializable]
    //[DebuggerNonUserCode]
    [PublicAPI]
    public abstract class ObservableObject : BaseObject, INotifyPropertyChangedMethod
    {
        [PublicAPI]
        protected class LogHelper
        {
            private readonly ObservableObject _target;
            private readonly Type _type;
            private readonly string _name;
            private Logger _logger;

            public LogHelper([NotNull] ObservableObject target)
            {
                _target = target;
                _type = target.GetType();
                _name = _type.Name;
            }

            public Logger Logger => _logger ?? (_logger = LogManager.GetLogger(_name, _type));

            public void Write([CanBeNull] object message, LogLevel type)
            {
                Logger.Log(type, message);
            }

            [StringFormatMethod("format")]
            public void Write(LogLevel type, [NotNull] string format, [NotNull] params object[] parms)
            {
                Logger.Log(type, format, parms);
            }
        }

        protected ObservableObject()
        {
            Category = GetType().ToString();
        }

        #region Public Events

        /// <summary>The property changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
//        {
//            add => AddEvent("PropertyChangedEventHandler", value);
//
//            remove => RemoveEvent("PropertyChangedEventHandler", value);
//        }

        #endregion

        #region Methods

        /// <summary>
        ///     The queue workitem.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <param name="withDispatcher">
        ///     The with dispatcher.
        /// </param>
        protected static void QueueWorkitem([NotNull] Action action, bool withDispatcher)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            CommonApplication.Scheduler.QueueTask(new UserTask(action, withDispatcher));
        }

        #endregion

        #region Public Properties

        private LogHelper _logHelper;

        /// <summary>Gets the current dispatcher.</summary>
        /// <value>The current dispatcher.</value>
        [NotNull]
        public static IUISynchronize CurrentDispatcher => UiSynchronize.Synchronize;

        [NotNull]
        protected LogHelper Log => _logHelper ?? (_logHelper = new LogHelper(this));

        [CanBeNull]
        protected string Category { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        public virtual void OnPropertyChanged([CallerMemberName] string eventArgs = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(eventArgs));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        public virtual void OnPropertyChanged([NotNull] PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs == null) throw new ArgumentNullException(nameof(eventArgs));
            PropertyChanged?.Invoke(this, eventArgs);
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual void OnPropertyChanged<T>([NotNull] Expression<Func<T>> eventArgs)
        {
            if (eventArgs == null) throw new ArgumentNullException(nameof(eventArgs));
            OnPropertyChanged(new PropertyChangedEventArgs(PropertyHelper.ExtractPropertyName(eventArgs)));
        }


        public virtual void OnPropertyChangedExplicit([NotNull] string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}