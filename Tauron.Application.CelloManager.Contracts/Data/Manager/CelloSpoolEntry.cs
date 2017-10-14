using System;

namespace Tauron.Application.CelloManager.Data.Manager
{
    public sealed class CelloSpoolEntry : IEquatable<CelloSpoolEntry>
    {
        private int _id;
        public event Action IdChangedEvent;

        public int Id
        {
            get => _id;
            set { _id = value; IdChangedEvent?.Invoke();}
        }

        public DateTime Timestamp { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int Amount { get; set; }

        public int Neededamount { get; set; }

        public bool Equals(CelloSpoolEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CelloSpoolEntry && Equals((CelloSpoolEntry) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(CelloSpoolEntry left, CelloSpoolEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CelloSpoolEntry left, CelloSpoolEntry right)
        {
            return !Equals(left, right);
        }
    }
}