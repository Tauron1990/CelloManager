using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CelloManager.Core.Comp.OldData
{
    [UsedImplicitly]
    public sealed class CommittedSpoolEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int OrderedCount { get; set; }

        public int SpoolId { get; set; }
    }
}