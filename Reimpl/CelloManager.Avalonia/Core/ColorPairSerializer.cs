using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;
using Material.Colors;

namespace CelloManager.Core;

public sealed class ColorPairSerializer : JsonConverter<ColorPair>
{
    // ReSharper disable once CognitiveComplexity
    public override ColorPair Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var colorPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(ColorPair.Color)) ?? nameof(ColorPair.Color);
        var foregroundColorPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(ColorPair.ForegroundColor)) ?? nameof(ColorPair.ForegroundColor);
        var comparison = options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var hasColor = false;
        var hasForegroundColor = false;
        var color = default(Color);
        var foregroundColor = default(Color);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (!hasColor || !hasForegroundColor)
                {
                    throw new JsonException();
                }

                return new ColorPair(color, foregroundColor);
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            if (!reader.Read())
            {
                throw new JsonException();
            }

            if (string.Equals(propertyName, colorPropertyName, comparison))
            {
                color = ReadColor(ref reader, options);
                hasColor = true;
            }
            else if (string.Equals(propertyName, foregroundColorPropertyName, comparison))
            {
                foregroundColor = ReadColor(ref reader, options);
                hasForegroundColor = true;
            }
            else
            {
                reader.Skip();
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ColorPair value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var colorPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(ColorPair.Color)) ?? nameof(ColorPair.Color);
        writer.WritePropertyName(colorPropertyName);
        WriteColor(writer, value.Color, options);

        var foregroundColorPropertyName = options.PropertyNamingPolicy?.ConvertName(nameof(ColorPair.ForegroundColor)) ?? nameof(ColorPair.ForegroundColor);
        writer.WritePropertyName(foregroundColorPropertyName);
        WriteColor(writer, value.ForegroundColor, options);

        writer.WriteEndObject();
    }

    private static Color ReadColor(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var converter = GetColorConverter(options);
        if (converter is not null)
        {
            return converter.Read(ref reader, typeof(Color), options);
        }

        if (reader.TokenType != JsonTokenType.String || !Color.TryParse(reader.GetString(), out var color))
        {
            throw new JsonException();
        }

        return color;
    }

    private static void WriteColor(Utf8JsonWriter writer, Color color, JsonSerializerOptions options)
    {
        var converter = GetColorConverter(options);
        if (converter is not null)
        {
            converter.Write(writer, color, options);
            return;
        }

        writer.WriteStringValue(color.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
    }

    private static JsonConverter<Color>? GetColorConverter(JsonSerializerOptions options)
    {
        foreach (var registeredConverter in options.Converters)
        {
            if (registeredConverter is JsonConverter<Color> colorConverter)
            {
                return colorConverter;
            }
        }

        return null;
    }
}