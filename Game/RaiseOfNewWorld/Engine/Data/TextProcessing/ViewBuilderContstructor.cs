using System.Collections.Immutable;
using System.Linq.Expressions;
using FastExpressionCompiler;
using Figgle;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class ViewBuilderContstructor
{
    private static readonly ImmutableDictionary<string, Func<FiggleFont>> FiggleFonts = CreateFonts();

    private static ImmutableDictionary<string,Func<FiggleFont>> CreateFonts()
    {
        var props = typeof(FiggleFonts).GetProperties();

        return props.ToImmutableDictionary(
            p => p.Name.ToLower(),
            pi => Expression.Lambda<Func<FiggleFont>>(
                    Expression.Property(null, pi))
                .CompileFast());
    }

    private static readonly ImmutableDictionary<string, Type> ViewTypes = ImmutableDictionary<string, Type>.Empty
        .Add("button", typeof(Button))
        .Add("checkbox", typeof(CheckBox))
        .Add("colorpicker", typeof(ColorPicker))
        .Add("combobox", typeof(ComboBox))
        .Add("frameview", typeof(FrameView))
        .Add("lineview", typeof(LineView))
        .Add("label", typeof(Label));

    public static View CreateView(ITextData textData)
    {
        var result = textData switch
        {
            SimpleText simpleText => CreateSimpleView(simpleText),
            TextData complexText => CreateComplexView(complexText),
            _ => throw new InvalidOperationException($"Unkowen ITextData Types {textData}")
        };
        
        result.BeginInit();

        return result;
    }

    private static View CreateSimpleView(SimpleText textData)
        => new Label { Text = textData.Text };

    private static View CreateComplexView(TextData textData)
    {
        if (ViewTypes.TryGetValue(textData.Type ?? string.Empty, out var type))
        {
            if (Activator.CreateInstance(type) is not View inst) throw new InvalidOperationException("View Creation Failed");
            inst.Id = textData.Name;
            
            return inst;
        }

        if (textData.Type == "figgle")
        {
            FiggleFont font;
            
            FiggleFonts.Double
        }
        return new View { Id = textData.Name };
    }


    // ReSharper disable once CognitiveComplexity
    public static void ApplyAttributes(ViewContext context, ImmutableArray<AttributeData> datas)
    {
        var view = context.Target;
        
        foreach (var (name, value) in datas)
        {
            ApplyViewAttributes(view, context, name, value);
            switch (view)
            {
                case Button button:
                    ApplyButtonAttributes(button, name, value);
                    break;
                case CheckBox checkBox:
                    ApplyChekBoxAttribute(checkBox, name, value);
                    break;
                case ColorPicker colorPicker:
                    ApplyColorPickerAttribute(colorPicker, name, value);
                    break;
                case FrameView frameView:
                    ApplyFrameviewAttributes(frameView, name, value);
                    break;
                case LineView lineView:
                    ApplyLineViewAttributes(lineView, name, value);
                    break;
                // case GraphView graphView:
                //     ApplyGraphViewAttribute(graphView, name, value);
                //     break;
            }
        }
        
        view.EndInit();
    }

    private static void ApplyLineViewAttributes(LineView lineView, string name, string value)
    {
        switch (name)
        {
            case "startinganchor":
                lineView.StartingAnchor = value[0];
                break;
            case "endinganchor":
                lineView.EndingAnchor = value[0];
                break;
            case "linerune":
                lineView.LineRune = value[0];
                break;
            case "orientation":
                lineView.Orientation = Enum.Parse<Orientation>(value);
                break;
        }
    }

    // private static void ApplyGraphViewAttribute(GraphView graphView, string name, string value)
    // {
    //     
    // }
    
    private static void ApplyFrameviewAttributes(FrameView frameView, string name, string value)
    {
        if (name == "title")
            frameView.Title = value;
    }
    
    private static void ApplyColorPickerAttribute(ColorPicker colorPicker, string name, string value)
    {
        if (name == "selectedcolor") colorPicker.SelectedColor = Enum.Parse<Color>(value);
    }
    private static void ApplyChekBoxAttribute(CheckBox checkBox, string name, string value)
    {
        if (name == "checked") checkBox.Checked = bool.Parse(value);
    }
    private static void ApplyButtonAttributes(Button button, string name, string value)
    {
        if (name == "isdefault") button.IsDefault = bool.Parse(value);
    }
    // ReSharper disable once CognitiveComplexity
    private static void ApplyViewAttributes(View view, ViewContext context, string name, string value)
    {
        switch (name.ToLower())
        {
            case "canfocus":
                view.CanFocus = bool.Parse(value);
                break;
            case "enabled":
                view.Enabled = bool.Parse(value);
                break;
            case "visible":
                view.Enabled = bool.Parse(value);
                break;
            case "hotkey":
                view.HotKey = ParseKey(value);
                break;
            case "hotkeyspecifier":
                view.HotKeySpecifier = value[0];
                break;
            case "shortcut":
                view.Shortcut = ParseKey(value);
                break;
            case "tabindex":
                view.TabIndex = int.Parse(value);
                break;
            case "tabstop":
                view.TabStop = bool.Parse(value);
                break;
            case "id":
                view.Id = value;
                break;
            case "x":
                view.X = PosFabricatorExpression.Parse(value).Evaluate(context);
                break;
            case "y":
                view.Y = PosFabricatorExpression.Parse(value).Evaluate(context);
                break;
            case "width":
                view.Width = DimFabricatorExpression.Parse(value).Evaluate(context);
                break;
            case "height":
                view.Height = DimFabricatorExpression.Parse(value).Evaluate(context);
                break;
            case "colorscheme":
                view.ColorScheme = ColorExpression.Parse(value).MergeColorScheme(view.ColorScheme);
                break;
            case "text":
                view.Text = value;
                break;
            case "autosize":
                view.AutoSize = bool.Parse(value);
                break;
            case "textalignment":
                view.TextAlignment = Enum.Parse<TextAlignment>(value);
                break;
            case "verticaltextalignment":
                view.VerticalTextAlignment = Enum.Parse<VerticalTextAlignment>(value);
                break;
            case "textdirection":
                view.TextDirection = Enum.Parse<TextDirection>(value);
                break;
        }
    }

    private static Key ParseKey(string input)
        => Enum.Parse<Key>(input.Replace('@', ','));
}