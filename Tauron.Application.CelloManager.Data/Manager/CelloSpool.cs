using System;
using System.ComponentModel;
using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.Models;
using Tauron.Application.Models.Rules;

namespace Tauron.Application.CelloManager.Data.Manager
{
    public sealed class CelloSpool : CelloSpoolBase, IEquatable<CelloSpool>
    {
        public static ObservableProperty NameProperty = RegisterProperty("Name", typeof(CelloSpool), typeof(string), new ObservablePropertyMetadata()
            .SetValidationRules(new RequiredRule{ FieldName = UIResources.LabelOptionsLayoutName }));

        public static ObservableProperty TypeProperty = RegisterProperty("Type", typeof(CelloSpool), typeof(string), new ObservablePropertyMetadata()
            .SetValidationRules(new RequiredRule { FieldName = UIResources.LabelOptionsLayoutType}));

        public static ObservableProperty AmountProperty = RegisterProperty("Amount", typeof(CelloSpool), typeof(int),
            new ObservablePropertyMetadata().SetValidationRules(new ModelRule(IntValidator) {Message = () => UIResources.LabelErrorNonNegativeInt}));

        public static ObservableProperty NeededamountProperty = RegisterProperty("Neededamount", typeof(CelloSpool),
            typeof(int), new ObservablePropertyMetadata().SetValidationRules(new ModelRule(IntValidator) {Message = () => UIResources.LabelErrorNonNegativeInt}));

        public CelloSpool(CelloSpoolEntry entry, CelloRepository repository, bool isNew = false)
        {
            Name = entry.Name;
            Type = entry.Type;
            Amount = entry.Amount;
            Neededamount = entry.Neededamount;

            CoreEntry = entry;
            IsNew = isNew;
            _repository = repository;
            entry.IdChangedEvent += () => OnPropertyChangedExplicit(nameof(Id));
        }

        private readonly CelloRepository _repository;

        public CelloSpoolEntry CoreEntry { get; private set; }
        public bool IsNew { get; }

        public override string Name
        {
            get => GetValue<string>(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public override string Type
        {
            get => GetValue<string>(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public override int Amount
        {
            get => GetValue<int>(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        public override int Neededamount
        {
            get => GetValue<int>(NeededamountProperty);
            set => SetValue(NeededamountProperty, value);
        }

        public override int Id => CoreEntry.Id;

        public override void UpdateSpool()
        {
            CoreEntry = _repository.GetEntry(Name, Type);

            Name = CoreEntry.Name;
            Type = CoreEntry.Type;
            Amount = CoreEntry.Amount;
            Neededamount = CoreEntry.Neededamount;
        }

        public override string UniquieId => BuildUinqueId(this);

        public override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if (CoreEntry != null)
                switch (eventArgs.PropertyName)
                {
                    case nameof(Name):
                        CoreEntry = UpdateCoreEntry(CoreEntry.Id, entry => entry.Name = Name);
                        break;
                    case nameof(Type):
                        CoreEntry = UpdateCoreEntry(CoreEntry.Id, entry => entry.Type = Type);
                        break;
                    case nameof(Amount):
                        CoreEntry = UpdateCoreEntry(CoreEntry.Id, entry => entry.Amount = Amount);
                        break;
                    case nameof(Neededamount):
                        CoreEntry = UpdateCoreEntry(CoreEntry.Id, entry => entry.Neededamount = Neededamount);
                        break;
                }

            base.OnPropertyChanged(eventArgs);
        }

        private CelloSpoolEntry UpdateCoreEntry(int id, Action<CelloSpoolEntry> entryUpdate)
        {
            entryUpdate(CoreEntry);
            if(!IsNew)
                _repository.UpdateEntry(CoreEntry);
            return CoreEntry;
        }

        public bool Equals(CelloSpool other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(CoreEntry, other.CoreEntry);
        }

        private static bool IntValidator(object o, ValidatorContext validatorContext) => o as int? >= 0;

        private static void SpoolSetValueCommon(CelloSpool spool, ObservableProperty property, object o)
        {
            spool.BlockOnPropertyChanged = true;
            spool.SetValue(property, o);
        }

        public static string BuildUinqueId(CelloSpoolEntry entry)
        {
            return BuildUinqueId(entry.Name, entry.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CelloSpool && Equals((CelloSpool) obj);
        }

        public override int GetHashCode()
        {
            return CoreEntry?.GetHashCode() ?? 0;
        }

        public static bool operator ==(CelloSpool left, CelloSpool right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CelloSpool left, CelloSpool right)
        {
            return !Equals(left, right);
        }
    }
}