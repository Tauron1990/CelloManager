using System.Collections.Immutable;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class ViewBuilderContstructor
{
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
        => textData.Type switch
        {
            _ => new View{ Id = textData.Name },
        };


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
            }
        }
        
        view.EndInit();
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