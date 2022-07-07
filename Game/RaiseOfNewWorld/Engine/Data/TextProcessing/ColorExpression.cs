using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class ColorExpression
{
    public static ColorScheme MergeColorScheme(this ColorScheme original, ColorScheme? toMerge)
    {
        if (toMerge is null) return original;
        
        if (original.Normal == default)
            original.Normal = toMerge.Normal;
        
        if (original.Disabled == default)
            original.Disabled = toMerge.Disabled;
        
        if (original.Focus == default)
            original.Focus = toMerge.Focus;
        
        if (original.HotFocus == default)
            original.HotFocus = toMerge.HotFocus;
        
        if (original.HotNormal == default)
            original.HotNormal = toMerge.HotNormal;

        return original;
    }
    
    public static ColorScheme Parse(string input)
    {
        var data = new ParameterParser(input);

        var scheme = CheckDefault(data.MethodName);
        return scheme ?? ReadScheme(data);
    }

    private static ColorScheme? CheckDefault(string toCheck)
        => toCheck switch
        {
            "toplevel" => Colors.TopLevel,
            "base" => Colors.Base,
            "menu" => Colors.Menu,
            "dialog" => Colors.Dialog,
            "error" => Colors.Error,
            _ => null
        };

    private static ColorScheme ReadScheme(ParameterParser data)
    {
        var scheme = new ColorScheme();
        var pos = -1;
        
        do
        {
            var proptry = data.ResolveParameter(pos);
            switch (proptry)
            {
                case "normal":
                    scheme.Normal = ReadAttributte();
                    break;
                case "focus":
                    scheme.Focus = ReadAttributte();
                    break;
                case  "hotnormal":
                    scheme.HotNormal = ReadAttributte();
                    break;
                case "hotfocus":
                    scheme.HotFocus = ReadAttributte();
                    break;
                case "disabled":
                    scheme.Disabled = ReadAttributte();
                    break;
            }
            
        } while (data.CanResolve(pos));

        return scheme;

        Attribute ReadAttributte()
            => Attribute.Make(
                Enum.Parse<Color>(data.ResolveParameter(pos++)),
                Enum.Parse<Color>(data.ResolveParameter(pos++)));
    }
}
