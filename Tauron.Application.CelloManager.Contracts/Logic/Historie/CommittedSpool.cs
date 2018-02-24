using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Logic.Historie
{
    [PublicAPI]
    public sealed class CommittedSpool
    {
        public CommittedSpool([NotNull] string name, int orderedCount, string type, int spoolId)
        {
            Name         = name;
            OrderedCount = orderedCount;
            Type         = type;
            SpoolId      = spoolId;
        }

        public string Name         { get; }
        public string Type         { get; }
        public int    OrderedCount { get; set; }
        public int    SpoolId      { get; }
    }
}