using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
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