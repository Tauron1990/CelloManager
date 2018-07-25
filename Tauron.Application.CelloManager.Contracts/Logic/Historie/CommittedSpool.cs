using JetBrains.Annotations;
using Tauron.Application.CelloManager.Data.Historie;

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

        public bool Skip { get; set; }
        public string Name         { get; }
        public string Type         { get; }
        public int    OrderedCount { get; set; }
        public int    SpoolId      { get; }

        public CommittedSpoolEntity CreateEntity()
        {
            return new CommittedSpoolEntity
            {
                Id = 0,
                Name = Name,
                OrderedCount = OrderedCount,
                SpoolId = SpoolId,
                Type = Type
            };
        }
    }
}