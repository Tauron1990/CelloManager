using System.Text;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Data.Historie
{
    [PublicAPI]
    public sealed class CommittedSpool
    {
        public CommittedSpool([NotNull] string name, int orderedCount, string type, int spoolId)
        {
            Name = name;
            OrderedCount = orderedCount;
            Type = type;
            SpoolId = spoolId;
        }

        public CommittedSpool()
        {
            
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int OrderedCount { get; set; }
        public int SpoolId { get; set; }

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