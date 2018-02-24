
using System;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public sealed class CelloSpool : ObservableObject, IEquatable<CelloSpool>
    {
        private int _amount;
        private int _neededamount;
        private string _type;
        private string _name;

        public CelloSpool(string name, string type, int amount, int neededamount, int id)
        {
            Name = name;
            Type = type;
            Amount = amount;
            Neededamount = neededamount;
            Id = id;
        }

        public CelloSpool()
        {
            
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public int Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public int Neededamount
        {
            get => _neededamount;
            set => SetProperty(ref _neededamount, value);
        }

        public int Id { get; }

        public bool Equals(CelloSpool other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
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