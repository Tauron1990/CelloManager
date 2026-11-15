using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Avalonia.Media;
using CelloManager.Core.Data;
using Material.Colors;
using Material.Styles.Themes;

namespace CelloManager.Core;

[JsonSerializable(typeof(ColorPair))]
[JsonSerializable(typeof(Color))]
[JsonSerializable(typeof(Theme))]
[JsonSerializable(typeof(SpoolData[]))]
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(OrderedSpoolList))]
[JsonSerializable(typeof(ImmutableList<OrderedSpoolList>))]
public sealed partial class SerializerContext : JsonSerializerContext
{
    
}