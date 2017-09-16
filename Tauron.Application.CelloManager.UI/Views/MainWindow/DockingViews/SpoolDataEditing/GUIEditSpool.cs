using System;
using System.Collections;
using System.ComponentModel;
using Tauron.Application.CelloManager.Logic.Manager;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    public interface ISpoolChangeNotifer
    {
        void SpoolValueChanged(CelloSpoolBase spool);
    }

    public class GuiEditSpool : ObservableObject, INotifyDataErrorInfo
    {
        private readonly ISpoolChangeNotifer _model;

        public GuiEditSpool(ISpoolChangeNotifer model, CelloSpoolBase spool)
        {
            _model = model;
            CelloSpoolBase = spool;
            CelloSpoolBase.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(Name):
                    case nameof(Type):
                    case nameof(Amount):
                    case nameof(Neededamount):
                        SpoolChanged();
                        break;
                }
            };
        }

        public string Name
        {
            get => CelloSpoolBase.Name;
            set => CelloSpoolBase.Name = value;
        }

        public string Type
        {
            get => CelloSpoolBase.Type;
            set => CelloSpoolBase.Type = value;
        }

        public int Amount
        {
            get => CelloSpoolBase.Amount;
            set => CelloSpoolBase.Amount = value;
        }

        public int Neededamount
        {
            get => CelloSpoolBase.Neededamount;
            set => CelloSpoolBase.Neededamount = value;
        }

        public CelloSpoolBase CelloSpoolBase { get; }

        public IEnumerable GetErrors(string propertyName)
        {
            return CelloSpoolBase.GetErrors(propertyName);
        }

        public bool HasErrors => CelloSpoolBase.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => CelloSpoolBase.ErrorsChanged += value;
            remove => CelloSpoolBase.ErrorsChanged -= value;
        }

        private void SpoolChanged()
        {
            _model.SpoolValueChanged(CelloSpoolBase);
        }
    }
}