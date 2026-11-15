using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace CelloManager.Core.Comp.OldData;

[UsedImplicitly]
[RequiresUnreferencedCode("EF Core requires unreferenced code to perform reflection operations.")]
public sealed class CelloSpoolEntity
{
    [Key] 
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    public int Amount { get; set; }

    public int Neededamount { get; set; }
}