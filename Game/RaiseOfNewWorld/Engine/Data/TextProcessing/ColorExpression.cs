using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public class ColorVisitor : AttributeValueVisitor<ColorScheme>
{
    public override ColorScheme VisitCall(CallAttributeValue callAttributeValue)
        => callAttributeValue.MethodName switch
        {
            "color" => CreateColorScheme(callAttributeValue),
            _ => throw new InvalidOperationException("Attribute Value for Color Should be color")
        };

    private ColorScheme CreateColorScheme(CallAttributeValue attributeValue)
    {
        var scheme = new ColorScheme();
        string? paramName = null;
        Color? color1 = null;
        Color? color2 = null;
        
        for (int i = 0; i < attributeValue.Parameters.Count; i++)
        {
            CheckIsReady();
            if (attributeValue.Parameters[i] is not TextAttributeValue paramAttribute)
                throw new InvalidOperationException("Only Textparameters are Supportet for ColorScheme Method");

            var value = paramAttribute.Value;

            if(string.IsNullOrEmpty(paramName))
                paramName = 
        }

        CheckIsReady();
        
        void CheckIsReady()
        {
            if (paramName is null || color1 is null || color2 is null) return;
            var colorAttr = Attribute.Make(color1.Value, color2.Value);
            switch (paramName)
            {
                case "normal":
                    scheme.Normal = colorAttr;
                    break;
                case "focus":
                    scheme.Focus = colorAttr;
                    break;
                case  "hotnormal":
                    scheme.HotNormal = colorAttr;
                    break;
                case "hotfocus":
                    scheme.HotFocus = colorAttr;
                    break;
                case "disabled":
                    scheme.Disabled = colorAttr;
                    break;
            }
        }
        
        return scheme;
    }
    
    public override ColorScheme VisitExpression(ExpressionAttributeValue expressionAttributeValue) 
        => expressionAttributeValue.Left.Visit(this).MergeColorScheme(expressionAttributeValue.Right.Visit(this));

    public override ColorScheme VisitText(TextAttributeValue textAttributeValue)
        => textAttributeValue.Value switch
        {
            "toplevel" => Colors.TopLevel,
            "base" => Colors.Base,
            "menu" => Colors.Menu,
            "dialog" => Colors.Dialog,
            "error" => Colors.Error,
            _ => Colors.Base
        };
}

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
    
    public static ColorScheme Parse(CallAttributeValue input)
    {
        var scheme = CheckDefault(input.MethodName);
        return scheme ?? ReadScheme(input);
    }
    
    private static ColorScheme ReadScheme(CallAttributeValue data)
    {
        var scheme = new ColorScheme();

        for (int i = 0; i < data.Parameters.Count; i++)
        {
            var proptry = data.Parameters[i]
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
        }
        
        var pos = -1;
        
        do
        {
            var proptry = data.ResolveParameter(pos);

            
        } while (data.CanResolve(pos));

        return scheme;

        Attribute ReadAttributte()
            => Attribute.Make(
                Enum.Parse<Color>(data.ResolveParameter(pos++)),
                Enum.Parse<Color>(data.ResolveParameter(pos++)));
    }
}
