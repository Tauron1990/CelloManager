using System.ComponentModel.DataAnnotations;

namespace CelloManager.Data;

public sealed class PendingOrderDb
{
    [Key, MaxLength(36)] 
    public string Id { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Spools { get; init; } = string.Empty;
    
    public DateTimeOffset Time { get; init; } = DateTimeOffset.MinValue;
}