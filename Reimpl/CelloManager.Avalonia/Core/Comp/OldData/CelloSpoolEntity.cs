using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CelloManager.Core.Comp.OldData
{
    [UsedImplicitly]
    public sealed class CelloSpoolEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Timestamp { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Amount { get; set; }

        public int Neededamount { get; set; }
    }
}