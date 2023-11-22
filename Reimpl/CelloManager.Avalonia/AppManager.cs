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
    
    public static AppManager Instance { get; set; } = new();

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

    public ColorPair SecondaryLight
    {
        get => _currentTheme.SecondaryLight;
        set => SetColor(t => t.SecondaryLight = value);
    }

    public ColorPair SecondaryMid
    {
        get => _currentTheme.SecondaryMid;
        set => SetColor(t => t.SecondaryMid = value);
    }

    public ColorPair SecondaryDark
    {
        get => _currentTheme.SecondaryMid;
        set => SetColor(t => t.SecondaryDark = value);
    }

    public Color ValidationError
    {
        get => _currentTheme.ValidationError;
        set => SetColor(t => t.ValidationError = value);
    }

    public Color Background
    {
        get => _currentTheme.Background;
        set => SetColor(t => t.Background = value);
    }

    public Color Paper
    {
        get => _currentTheme.Paper;
        set => SetColor(t => t.Paper = value);
    }

    public Color CardBackground
    {
        get => _currentTheme.CardBackground;
        set => SetColor(t => t.CardBackground = value);
    }

    public Color ToolBarBackground { get; set; }

    public Color Body { get; set; }

    public Color BodyLight { get; set; }

    public Color ColumnHeader { get; set; }

    public Color CheckBoxOff { get; set; }

    public Color CheckBoxDisabled { get; set; }

    public Color Divider { get; set; }

    public Color Selection { get; set; }

    public Color ToolForeground { get; set; }

    public Color ToolBackground { get; set; }

    public Color FlatButtonClick { get; set; }

    public Color FlatButtonRipple { get; set; }

    public Color ToolTipBackground { get; set; }

    public Color ChipBackground { get; set; }

    public Color SnackbarBackground { get; set; }

    public Color SnackbarMouseOver { get; set; }

    public Color SnackbarRipple { get; set; }

    public Color TextBoxBorder { get; set; }

    public Color TextFieldBoxBackground { get; set; }

    public Color TextFieldBoxHoverBackground { get; set; }

    public Color TextFieldBoxDisabledBackground { get; set; }

    public Color TextAreaBorder { get; set; }

    public Color TextAreaInactiveBorder { get; set; }

    public Color DataGridRowHoverBackground { get; set; }

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