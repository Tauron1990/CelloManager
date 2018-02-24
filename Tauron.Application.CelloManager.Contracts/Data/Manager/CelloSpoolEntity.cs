using System;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Manager
{
    public sealed class CelloSpoolEntity : GenericBaseEntity<int>, IEquatable<CelloSpoolEntity>
    {
        private DateTime _timestamp;
        private string _name;
        private string _type;
        private int _amount;
        private int _neededamount;

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetWithNotify(ref _timestamp, value);
        }

        public string Name
        {
            get => _name;
            set => SetWithNotify(ref _name, value);
        }

        public string Type
        {
            get => _type;
            set => SetWithNotify(ref _type, value);
        }

        public int Amount
        {
            get => _amount;
            set => SetWithNotify(ref _amount, value);
        }

        public int Neededamount
        {
            get => _neededamount;
            set => SetWithNotify(ref _neededamount, value);
        }

        public bool Equals(CelloSpoolEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CelloSpoolEntity entity && Equals(entity);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(CelloSpoolEntity left, CelloSpoolEntity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CelloSpoolEntity left, CelloSpoolEntity right)
        {
            return !Equals(left, right);
        }
    }
}