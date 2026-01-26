using System.ComponentModel.DataAnnotations;

namespace CelloManager.Data;

public sealed class SpoolDataDb
{
    [Key, MaxLength(36)]
    public string Id { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
    [MaxLength(100)]
    public string Category { get; init; } = string.Empty;
    public int Amount { get; init; }
    public int NeedAmount { get; init; }
}