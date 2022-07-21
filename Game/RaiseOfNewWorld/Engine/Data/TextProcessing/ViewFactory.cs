using System.Collections.Immutable;
using System.Linq.Expressions;
using FastExpressionCompiler;
using Figgle;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class ViewFactory : FragmentNodeVisitor<ImmutableDictionary<string, View>>
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
    
    private bool _root = true;
    
    public override ImmutableDictionary<string, View> VisitTextFragment(TextFragmentNode textFragmentNode)
    {
        try
        {
            var root = _root;
            _root = false;

            var views = ImmutableDictionary<string, View>.Empty;
            var name = textFragmentNode.Name;
            var type = textFragmentNode.Type;
            var text = textFragmentNode.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = Guid.NewGuid().ToString("N");
                textFragmentNode.Name = name;
            }
            if (string.IsNullOrWhiteSpace(type.Type))
                type.Type = "label";

            var view = CreateView(
                type,
                name,
                text);

            if (root)
            {
                var last = views.LastOrDefault().Value;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (last is not null)
                    view.Y = Pos.Top(last) + 1;
            }

            return views.Add(name, view).AddRange(textFragmentNode.FragmentNodes.SelectMany(Accept));
        }
        finally
        {
            _root = true;
        }
    }

    public override ImmutableDictionary<string, View> VisitTextData(TextDataNode textDataNode)
        => ImmutableDictionary<string, View>.Empty.AddRange(textDataNode.FragmentNodes.SelectMany(Accept));
    
    private static View CreateView(TypeRepesentation typeName, string id, string text)
    {
        if (ViewTypes.TryGetValue(typeName.Type, out var type))
        {
            if (Activator.CreateInstance(type) is not View inst) throw new InvalidOperationException("View Creation Failed");
            inst.Id = id;
            inst.Text = text;
            
            return inst;
        }

        if (typeName.Type == "figgle" && FiggleFonts.TryGetValue(
                typeName.Parameter,
                out var font))
            return new Label { Id = id, Text = font().Render(text) };
        return new View { Id = id, Text = text};
    }
}