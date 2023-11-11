using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CelloManager.Core.Comp.OldData
{
    [PublicAPI]
    public sealed class CommittedRefillEntity
    {
        [Key]
        public int Id { get; set; }
        
        #pragma warning disable MA0016
        public List<CommittedSpoolEntity> CommitedSpools { get; } = new();
        #pragma warning restore MA0016

        public DateTime SentTime { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CompledTime { get; set; }
    }
}