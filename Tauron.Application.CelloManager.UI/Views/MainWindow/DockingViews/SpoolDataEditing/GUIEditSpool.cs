using System;
using System.ComponentModel;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.CelloManager.UI.Models;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Views.MainWindow.DockingViews
{

    public class GuiEditSpool : ModelBase
    {
        public GuiEditSpool(EditSpool spool)
            : this()
        {
            Initialize(spool);
        }

        public GuiEditSpool()
        {
            ConvertPropertyIssuesToString = true;
        }

        public void Initialize(EditSpool spool)
        {
            spool.UIViewSpool = this;
            CelloSpool = spool.Spool;
            EditSpool = spool;

            SetEmptyStringErrors(Name, nameof(Name));
            SetEmptyStringErrors(Type, nameof(Type));
            SetNegativeNumberErrors(Amount, nameof(Amount));
            SetNegativeNumberErrors(Neededamount, nameof(Neededamount));
        }

        public int Id
        {
            get => CelloSpool?.Id ?? 0;
            set => throw new InvalidOperationException();
        }


        public string Name
        {
            get => CelloSpool?.Name;
            set { CelloSpool.Name = value; OnPropertyChanged();}
        }

        public string Type
        {
            get => CelloSpool?.Type;
            set { CelloSpool.Type = value; OnPropertyChanged();}
        }

        public int Amount
        {
            get => CelloSpool?.Amount ?? 0;
            set { CelloSpool.Amount = value; OnPropertyChanged();}
        }

        public int Neededamount
        {
            get => CelloSpool?.Neededamount ?? 0;
            set { CelloSpool.Neededamount = value; OnPropertyChanged();}
        }

        public CelloSpool CelloSpool { get; private set; }

        public EditSpool EditSpool { get; private set; }

        public override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case nameof(Name):
                    SetEmptyStringErrors(Name, eventArgs.PropertyName);
                    break;
                case nameof(Type):
                    SetEmptyStringErrors(Type, eventArgs.PropertyName);
                    break;
                case nameof(Amount):
                    SetNegativeNumberErrors(Amount, eventArgs.PropertyName);
                    break;
                case nameof(Neededamount):
                    SetNegativeNumberErrors(Neededamount, eventArgs.PropertyName);
                    break;
            }

            base.OnPropertyChanged(eventArgs);
        }

        private void SetEmptyStringErrors(string text, string name) => 
            SetIssues(name, string.IsNullOrWhiteSpace(text) ? new[] {new PropertyIssue(name, text, UIResources.LabelErrorNonEmptyString)} : Array.Empty<PropertyIssue>());

        private void SetNegativeNumberErrors(int number, string name) => 
            SetIssues(name, number < 0 ? new[] {new PropertyIssue(name, number, UIResources.LabelErrorNonEmptyString)} : Array.Empty<PropertyIssue>());
    }
}