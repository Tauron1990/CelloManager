using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
    
    private ITheme _currentTheme;

    public ITheme CurrentTheme
    {
        get => _currentTheme;
        set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
    }

    [LoggerMessage(Level = LogLevel.Warning, EventId = 1, Message = "Error on Deserialize Theme")]
    private partial void ThemeDeserialitionError(Exception e);

    [LoggerMessage(Level = LogLevel.Warning, EventId = 2, Message = "Error on Serialize Theme")]
    private partial void ThemeSerializationError(Exception e);
    
    private AppManager()
    {
        try
        {
            if (File.Exists(_themeFile))
                _currentTheme = JsonConvert.DeserializeObject<Theme>(File.ReadAllText(_themeFile), _serializerSettings)!;

            _currentTheme ??= CreateDefaultTheme();
        }
        catch (Exception e)
        {
            ThemeDeserialitionError(e);
            _currentTheme = CreateDefaultTheme();
        }

        this.WhenAny(m => m.CurrentTheme, c => c.Value)
            .SelectMany(async d => await SaveTheme(d).ConfigureAwait(false))
            .Subscribe();
    }

    public ColorPair PrimaryLight
    {
        get => _currentTheme.PrimaryLight;
        set => SetColor(t => t.PrimaryLight = value);
    }

    public ColorPair PrimaryMid
    {
        get => _currentTheme.PrimaryMid;
        set => SetColor(t => t.PrimaryMid = value);
    }

    public ColorPair PrimaryDark
    {
        get => _currentTheme.PrimaryDark;
        set => SetColor(t => t.PrimaryDark = value);
    }

    public ColorPair SecondaryLight { get; }

    public ColorPair SecondaryMid { get; }

    public ColorPair SecondaryDark { get; }

    public Color ValidationError { get; }

    public Color Background { get; }

    public Color Paper { get; }

    public Color CardBackground { get; }

    public Color ToolBarBackground { get; }

    public Color Body { get; }

    public Color BodyLight { get; }

    public Color ColumnHeader { get; }

    public Color CheckBoxOff { get; }

    public Color CheckBoxDisabled { get; }

    public Color Divider { get; }

    public Color Selection { get; }

    public Color ToolForeground { get; }

    public Color ToolBackground { get; }

    public Color FlatButtonClick { get; }

    public Color FlatButtonRipple { get; }

    public Color ToolTipBackground { get; }

    public Color ChipBackground { get; }

    public Color SnackbarBackground { get; }

    public Color SnackbarMouseOver { get; }

    public Color SnackbarRipple { get; }

    public Color TextBoxBorder { get; }

    public Color TextFieldBoxBackground { get; }

    public Color TextFieldBoxHoverBackground { get; }

    public Color TextFieldBoxDisabledBackground { get; }

    public Color TextAreaBorder { get; }

    public Color TextAreaInactiveBorder { get; }

    public Color DataGridRowHoverBackground { get; }

    private void SetColor(Action<ITheme> setter, [CallerMemberName] string? propertyName = null)
    {
        setter(_currentTheme);
        this.RaisePropertyChanged(propertyName);
        this.RaisePropertyChanged(nameof(CurrentTheme));
    }
    
    private async ValueTask<bool> SaveTheme(ITheme theme)
    {
        try
        {
            await File.WriteAllTextAsync(_themeFile, JsonConvert.SerializeObject(theme, _serializerSettings)).ConfigureAwait(false);
            return true;
        }
        catch (Exception e)
        {
            ThemeSerializationError(e);
            return false;
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