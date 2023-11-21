using System;
using System.Collections.Generic;
using Avalonia.Media;
using Material.Colors;
using Material.Styles.Themes;
using Newtonsoft.Json;
using ReactiveUI;

namespace CelloManager;

public class AppManager : ReactiveObject
{
    private static JsonSerializerSettings _serializerSettings =
        new()
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>{ new ColorPairSerializer()},
        };
    
    public static AppManager Instance { get; } = new();
    
    private ITheme _currentTheme;

    public ITheme CurrentTheme
    {
        get => _currentTheme;
        set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
    }

    private AppManager()
    {
        _currentTheme = Theme.Create(
            Theme.Dark,
            SwatchHelper.Lookup[MaterialColor.BlueGrey],
            SwatchHelper.Lookup[MaterialColor.Amber]);
    }

    private sealed class ColorPairSerializer : JsonConverter<ColorPair>
    {
        public override void WriteJson(JsonWriter writer, ColorPair value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(ColorPair.Color));
            serializer.Serialize(writer, value.Color);
            writer.WritePropertyName(nameof(ColorPair.ForegroundColor));
            serializer.Serialize(writer, value.ForegroundColor);
            writer.WriteEndObject();
        }

        public override ColorPair ReadJson(JsonReader reader, Type objectType, ColorPair existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Color color = default;
            Color foregroundColor = default;

            reader.Read();
            SetColor();
            reader.Read();
            SetColor();
            reader.Read();
            
            return new ColorPair(color, foregroundColor);

            void SetColor()
            {
                switch ((reader.Value as string) ?? string.Empty)
                {
                    case nameof(ColorPair.Color):
                        reader.Read();
                        color = serializer.Deserialize<Color>(reader);
                        break;
                    case nameof(ColorPair.ForegroundColor):
                        reader.Read();
                        foregroundColor = serializer.Deserialize<Color>(reader);
                        break;
                    default:
                        throw new InvalidOperationException($"Unkowen property Name: {reader.Value}");
                }
            }
        }
    }
}