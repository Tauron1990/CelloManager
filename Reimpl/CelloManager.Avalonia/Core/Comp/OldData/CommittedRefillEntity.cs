using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CelloManager.Avalonia.Core.Comp.OldData
{
    [PublicAPI]
    public sealed class CommittedRefillEntity
    {
        [Key]
        public int Id { get; set; }
        
        public List<CommittedSpoolEntity> CommitedSpools { get; } = new();

        public DateTime SentTime { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CompledTime { get; set; }
    }
}