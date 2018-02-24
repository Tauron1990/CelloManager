using System.Text;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Historie
{
    [PublicAPI]
    public sealed class CommittedSpoolEntity : GenericBaseEntity<int>
    {
        private string _name;
        private string _type;
        private int _spoolId;
        private int _orderedCount;

        public CommittedSpoolEntity([NotNull] string name, int orderedCount, string type, int spoolId)
        {
            Name = name;
            OrderedCount = orderedCount;
            Type = type;
            SpoolId = spoolId;
        }

        public CommittedSpoolEntity()
        {
            
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

        public int OrderedCount
        {
            get => _orderedCount;
            set => SetWithNotify(ref _orderedCount, value);
        }

        public int SpoolId
        {
            get => _spoolId;
            set => SetWithNotify(ref _spoolId, value);
        }

        public void BuildString(StringBuilder builder)
        {
            builder
                .Append("ID  :     ").Append(Id).AppendLine()
                .Append("Name:     ").Append(Name).AppendLine()
                .Append("Type:     ").Append(Type).AppendLine()
                .Append("Bestellt: ").Append(OrderedCount).AppendLine();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            BuildString(builder);

            return builder.ToString();
        }
    }
}