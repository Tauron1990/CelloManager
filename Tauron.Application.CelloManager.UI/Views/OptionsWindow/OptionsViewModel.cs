using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow
{
    [ExportViewModel(AppConststands.OptionsWindow)]
    public sealed class OptionsViewModel : ViewModelBase
    {
        private readonly Dictionary<string, IOptionsPanel> _optionsPanels = new Dictionary<string, IOptionsPanel>();
        private ModelBase _currentSelection;
        private bool _isHandeled;
        private IWindow _window;
        private bool _isUnlock = true;

        public ModelBase CurrentSelection
        {
            get => _currentSelection;
            set
            {
                _currentSelection = value;
                OnPropertyChanged();
            }
        }

        private void SetPanel(string name)
        {
            if (!_optionsPanels.TryGetValue(name, out var panel))
            {
                panel = (IOptionsPanel)ResolveViewModel(name);
                panel.Reset();
                panel.LockUIEvent += (sender, args) => _isUnlock = false;
                _optionsPanels[name] = panel;
            }
            
            CurrentSelection = (ModelBase)panel;
        }

        public override void OnShow(IWindow window)
        {
            _isHandeled = false;
            _window = window;
            _window.Closed += WindowOnClosed;
            _optionsPanels.Clear();
        }

        private void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            _window.Closed -= WindowOnClosed;
            if (_isHandeled) return;

            CommonApplication.Scheduler.QueueTask(new UserTask(Rollback, false));
        }

        private void Rollback()
        {
            foreach (var panel in _optionsPanels)
                panel.Value.Rollback();
        }

        [CommandTarget]
        public void Cancel()
        {
            _isHandeled = true;

            Rollback();

            _window.Close();
        }

        [CommandTarget]
        public bool CanCancel() => _isUnlock;

        [CommandTarget]
        public void Commit()
        {
            _isHandeled = true;

            foreach (var panel in _optionsPanels)
                panel.Value.Commit();

            _window.Close();
        }

        [CommandTarget]
        public bool CanCommit()
        {
            return !_optionsPanels.Select(e => e.Value).Any(p => p.HasErrors) && _isUnlock;
        }

        [CommandTarget]
        public void LayoutOptions() => SetPanel(AppConststands.LayoutOptionsView);
        [CommandTarget]
        public bool CanLayoutOptions() => _isUnlock;

        [CommandTarget]
        public void AutoUpdateOptions() => SetPanel(AppConststands.UpdateOptionsView);
        [CommandTarget]
        public bool CanAutoUpdateOptions() => _isUnlock;
    }
}