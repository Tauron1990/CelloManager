using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;
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

    private static ColorScheme CreateColorScheme(CallAttributeValue attributeValue)
    {
        var scheme = new ColorScheme();
        string? paramName = null;
        Color? color1 = null;
        Color? color2 = null;

        foreach (var node in attributeValue.Parameters)
        {
            CheckIsReady();
            var value = StringVisitor.Instance.ToString(node);

            if (string.IsNullOrEmpty(paramName))
                paramName = value;
            else if (color1 is null)
                color1 = Enum.Parse<Color>(value);
            else
                color2 = Enum.Parse<Color>(value);
        }

        CheckIsReady();

        void CheckIsReady()
        {
            if (paramName is null || color1 is null || color2 is null) return;

            var colorAttr = Attribute.Make(
                color1.Value,
                color2.Value);
            switch (paramName)
            {
                case "normal":
                    scheme.Normal = colorAttr;
                    break;
                case "focus":
                    scheme.Focus = colorAttr;
                    break;
                case "hotnormal":
                    scheme.HotNormal = colorAttr;
                    break;
                case "hotfocus":
                    scheme.HotFocus = colorAttr;
                    break;
                case "disabled":
                    scheme.Disabled = colorAttr;
                    break;
            }

            paramName = null;
            color1 = null;
            color2 = null;
        }

        return scheme;
    }

    public override ColorScheme VisitExpression(ExpressionAttributeValue expressionAttributeValue)
    {
        if (expressionAttributeValue.OperatorType == OperatorType.Add)
            return expressionAttributeValue.Left.Visit(this)
                .MergeColorScheme(expressionAttributeValue.Right.Visit(this));
        throw new InvalidOperationException("Only Adding Operation for Color Expresson are Supported");
    }

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