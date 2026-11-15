using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace CelloManager.Core.Comp.OldData;

[UsedImplicitly]
public class OptionEntity
{
    [Key] public int Id { get; init; }

    public string Key { get; init; } = string.Empty;

    public string Value { get; init; } = string.Empty;
}