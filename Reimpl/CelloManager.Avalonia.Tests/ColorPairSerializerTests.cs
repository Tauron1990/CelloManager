using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;
using CelloManager.Core;
using Material.Colors;
using Xunit;

namespace CelloManager.Avalonia.Tests;

public sealed class ColorPairSerializerTests
{
    [Fact]
    public void SerializeAndDeserializePreservesBothColors()
    {
        var color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var foregroundColor = Color.FromArgb(0x98, 0xBA, 0xDC, 0xFE);
        var pair = new ColorPair(color, foregroundColor);

        var json = JsonSerializer.Serialize(pair, CreateOptions());
        var restored = JsonSerializer.Deserialize<ColorPair>(json, CreateOptions());

        Assert.Equal(color, restored.Color);
        Assert.Equal(foregroundColor, restored.ForegroundColor);
    }

    [Fact]
    public void DeserializeReadsPropertiesInAnyOrderAndSkipsUnknownValues()
    {
        var color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var foregroundColor = Color.FromArgb(0x98, 0xBA, 0xDC, 0xFE);
        var json = $"{{\"metadata\":{{\"nested\":[1,{{\"ignored\":true}}]}},\"ForegroundColor\":{JsonSerializer.Serialize(foregroundColor.ToString())},\"Color\":{JsonSerializer.Serialize(color.ToString())}}}";

        var restored = JsonSerializer.Deserialize<ColorPair>(json, CreateOptions());

        Assert.Equal(color, restored.Color);
        Assert.Equal(foregroundColor, restored.ForegroundColor);
    }

    [Fact]
    public void SerializeAndDeserializeUsesCustomColorConverter()
    {
        var color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var foregroundColor = Color.FromArgb(0x98, 0xBA, 0xDC, 0xFE);
        var options = CreateOptions();
        options.Converters.Add(new ChannelColorConverter());

        var json = JsonSerializer.Serialize(new ColorPair(color, foregroundColor), options);
        var restored = JsonSerializer.Deserialize<ColorPair>(json, options);

        Assert.Contains("\"18,52,86,120\"", json);
        Assert.Equal(color, restored.Color);
        Assert.Equal(foregroundColor, restored.ForegroundColor);
    }

    [Fact]
    public void HonorsPropertyNamingAndCaseInsensitiveOptions()
    {
        var color = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
        var foregroundColor = Color.FromArgb(0x98, 0xBA, 0xDC, 0xFE);
        var options = CreateOptions();
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.PropertyNameCaseInsensitive = true;

        var json = JsonSerializer.Serialize(new ColorPair(color, foregroundColor), options)
            .Replace("\"color\"", "\"COLOR\"")
            .Replace("\"foregroundColor\"", "\"FOREGROUNDCOLOR\"");
        var restored = JsonSerializer.Deserialize<ColorPair>(json, options);

        Assert.Equal(color, restored.Color);
        Assert.Equal(foregroundColor, restored.ForegroundColor);
    }

    [Fact]
    public void ReadRejectsNonObjectsAndMissingOrNullColors()
    {
        var options = CreateOptions();

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ColorPair>("null", options));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ColorPair>("[]", options));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ColorPair>("{}", options));
        var colorJson = JsonSerializer.Serialize(Color.FromArgb(0x12, 0x34, 0x56, 0x78).ToString());
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ColorPair>($"{{\"Color\":{colorJson}}}", options));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ColorPair>($"{{\"ForegroundColor\":{colorJson}}}", options));
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ColorPair>("{\"Color\":null,\"ForegroundColor\":null}", options));
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ColorPairSerializer());
        return options;
    }

    private sealed class ChannelColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var channels = reader.GetString()?.Split(',');
            if (channels is not { Length: 4 }
                || !byte.TryParse(channels[0], CultureInfo.InvariantCulture, out var alpha)
                || !byte.TryParse(channels[1], CultureInfo.InvariantCulture, out var red)
                || !byte.TryParse(channels[2], CultureInfo.InvariantCulture, out var green)
                || !byte.TryParse(channels[3], CultureInfo.InvariantCulture, out var blue))
            {
                throw new JsonException();
            }

            return Color.FromArgb(alpha, red, green, blue);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{value.A},{value.R},{value.G},{value.B}");
        }
    }
}