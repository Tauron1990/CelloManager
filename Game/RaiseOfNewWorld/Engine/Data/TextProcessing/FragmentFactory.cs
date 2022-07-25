using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class FragmentFactory
{
    public static TextFragment Create(string input)
    {
        var ast = new TextParser(input).Parse(GameManager.ContentManager);
        var views = new ViewFactory().VisitTextData(ast);
        new ViewAttributeApplayer(views).Accept(ast);

        return new ComplexFragment(views.Values.ToArray());
    }

    private sealed class ComplexFragment : TextFragment
    {
        private readonly View[] _views;

        public ComplexFragment(View[] views)
        {
            _views = views;
        }

        public override View Render(View view)
        {
            view.Add(_views);
            return view;
        }
    }
}