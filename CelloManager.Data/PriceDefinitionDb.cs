using System.ComponentModel.DataAnnotations;

namespace CelloManager.Data;

public record PriceDefinitionDb
{
    [Key, MaxLength(36)]
    public string Id { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
    
    [Range(1, 1000)]
    public double Price { get; init; }
    
    [Range(1, 10000)]
    public double Lenght { get; init; }
}