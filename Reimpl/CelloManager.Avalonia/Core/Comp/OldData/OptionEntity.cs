using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CelloManager.Core.Comp.OldData
{
    [UsedImplicitly]
    public class OptionEntity
    {
        [Key]
        public int Id { get; set; }

        public string key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}