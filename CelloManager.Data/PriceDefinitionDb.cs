using System.ComponentModel.DataAnnotations;

namespace CelloManager.Data;

public record PriceDefinitionDb
{
    [Key]
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public double Price { get; init; }
    public double Lenght { get; init; }
}