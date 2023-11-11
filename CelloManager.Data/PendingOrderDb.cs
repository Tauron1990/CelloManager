using System.ComponentModel.DataAnnotations;

namespace CelloManager.Data;

public sealed class PendingOrderDb
{
    [Key]
    public string Id { get; init; } = string.Empty;
    public string Spools { get; init; } = string.Empty;
    public DateTimeOffset Time { get; init; } = DateTimeOffset.MinValue;
}