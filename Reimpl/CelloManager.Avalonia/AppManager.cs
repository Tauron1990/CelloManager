using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using CelloManager.Data;
using Material.Colors;
using Material.Styles.Themes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReactiveUI;

namespace CelloManager;

public partial class AppManager : ReactiveObject
{
    private static JsonSerializerSettings _serializerSettings =
        new()
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>{ new ColorPairSerializer()},
        };

    private string _themeFile = Path.Combine(DataOperationManager.DatabaseDic, "theme.json");
    
    public static AppManager Instance { get; } = new();

    private readonly ILogger<AppManager> _logger = App.ServiceProvider.GetService<ILogger<AppManager>>();
    
    private Theme _currentTheme;

    public Theme CurrentTheme
    {
        get => _currentTheme;
        set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
    }

    [LoggerMessage(Level = LogLevel.Warning, EventId = 1, Message = "Error on Deserialize Theme")]
    private partial void ThemeDeserialitionError(Exception e);
    
    private AppManager()
    {
        try
        {

        }
        catch (Exception e)
        {
            ThemeDeserialitionError(e);
        }
    }

    private Theme CreateDefaultTheme()
        => Theme.Create(
            Theme.Dark,
            SwatchHelper.Lookup[MaterialColor.BlueGrey],
            SwatchHelper.Lookup[MaterialColor.Amber]);
    
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