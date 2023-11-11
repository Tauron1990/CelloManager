using System.ComponentModel.DataAnnotations;

namespace CelloManager.Data;

public sealed class SpoolDataDb
{
    [Key]
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int Amount { get; init; } 
    public int NeedAmount { get; init; } 
}