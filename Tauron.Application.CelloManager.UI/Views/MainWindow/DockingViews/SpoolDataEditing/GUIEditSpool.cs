using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{
    public interface ISpoolChangeNotifer
    {
        void SpoolValueChanged(CelloSpoolBase spool);
    }

    public class GuiEditSpool : ObservableObject, INotifyDataErrorInfo
    {
        private ISpoolChangeNotifer _model;

        public GuiEditSpool(ISpoolChangeNotifer model, CelloSpoolBase spool)
        {
            Initialize(model, spool);
        }

        public GuiEditSpool()
        {
            
        }

        public void Initialize(ISpoolChangeNotifer model, CelloSpoolBase spool)
        {
            _model = model;
            CelloSpoolBase = spool;
            CelloSpoolBase.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(Id):
                        OnPropertyChangedExplicit(nameof(Id));
                        break;
                    case nameof(Name):
                    case nameof(Type):
                    case nameof(Amount):
                    case nameof(Neededamount):
                        SpoolChanged();
                        if(args.PropertyName == nameof(Type))
                            OnPropertyChangedExplicit(nameof(Type));
                        break;
                }
            };
            CelloSpoolBase.ErrorsChanged += (sender, args) => ErrorsChanged?.Invoke(this, args);
        }

        public int Id
        {
            get => CelloSpoolBase?.Id ?? 0;
            set => throw new InvalidOperationException();
        }


        public string Name
        {
            get => CelloSpoolBase?.Name;
            set => CelloSpoolBase.Name = value;
        }

        public string Type
        {
            get => CelloSpoolBase?.Type;
            set => CelloSpoolBase.Type = value;
        }

        public int Amount
        {
            get => CelloSpoolBase?.Amount ?? 0;
            set => CelloSpoolBase.Amount = value;
        }

        public int Neededamount
        {
            get => CelloSpoolBase?.Neededamount ?? 0;
            set => CelloSpoolBase.Neededamount = value;
        }

        public CelloSpoolBase CelloSpoolBase { get; private set; }

        public IEnumerable GetErrors(string propertyName)
        {
            foreach (var error in CelloSpoolBase?.GetErrors(propertyName)?.OfType<PropertyIssue>() ?? Enumerable.Empty<PropertyIssue>())
            {
                yield return error.Message;
            }
        }

        public bool HasErrors => CelloSpoolBase?.HasErrors == true;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void SpoolChanged()
        {
            _model.SpoolValueChanged(CelloSpoolBase);
        }
    }
}