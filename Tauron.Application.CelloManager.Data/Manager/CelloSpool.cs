using System;
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

        public CelloSpool(CelloSpoolEntry entry, bool isNew = false)
        {
            Name = entry.Name;
            Type = entry.Type;
            Amount = entry.Amount;
            Neededamount = entry.Neededamount;
            Id = entry.Id;

            IsNew = isNew;
            entry.IdChangedEvent += () => OnPropertyChangedExplicit(nameof(Id));
        }
        
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

        public override int Id { get; }

        public override void UpdateSpool(IUnitOfWork work)
        {
            work.SpoolRepository.UpdateEntry(this);
        }

        public override string UniquieId => BuildUinqueId(this);


        public bool Equals(CelloSpool other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
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
            return Id;
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