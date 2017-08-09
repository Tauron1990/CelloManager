using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow
{
    [ExportViewModel(AppConststands.OptionsWindow)]
    public sealed class OptionsViewModel : ViewModelBase
    {
        private readonly List<IOptionsPanel> _optionsPanels = new List<IOptionsPanel>();
        private ModelBase _currentSelection;
        private bool _isHandeled;
        private IWindow _window;

        public ModelBase CurrentSelection
        {
            get => _currentSelection;
            set
            {
                _currentSelection = value;
                OnPropertyChanged();
            }
        }

        private void ResolveOptionsModel(string name)
        {
            SetPanel(ResolveViewModel(name));
        }

        private void SetPanel(ModelBase model)
        {
            var panel = model as IOptionsPanel;
            if (panel != null)
            {
                panel.Reset();
                _optionsPanels.Add(panel);
            }

            CurrentSelection = model;
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
                panel.Rollback();
        }

        [CommandTarget]
        public void Cancel()
        {
            _isHandeled = true;

            Rollback();

            _window.Close();
        }

        [CommandTarget]
        public void Commit()
        {
            _isHandeled = true;

            foreach (var panel in _optionsPanels)
                panel.Commit();

            _window.Close();
        }

        [CommandTarget]
        public bool CanCommit()
        {
            return !_optionsPanels.Any(p => p.HasErrors);
        }

        [CommandTarget]
        public void LayoutOptions()
        {
            ResolveOptionsModel(AppConststands.LayoutOptionsView);
        }
    }
}