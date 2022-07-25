using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data;

public abstract class TextFragment
{
    /*public static TextFragment Create(IEnumerable<TextFragment> fragments)
        => new CompountFragment(fragments);

    public static TextFragment Create(string text, Action<Label>? attributeBuilder)
        => new SimpleText(
            text,
            attributeBuilder);

    private TView DefaultLayout<TView>(TView view, View? next)
        where TView : View
    {
        view.X = 1;
        view.Y = next == null ? 1 : Pos.Top(next) + 1;

        return view;
    }*/


    public abstract View Render(View view);

    /*private sealed class SimpleText : TextFragment
    {
        private readonly Action<Label>? _changeLabel;
        private readonly string _text;

        public SimpleText(string text, Action<Label>? changeLabel = null)
        {
            _text = text;
            _changeLabel = changeLabel;
        }

        protected override View Render(View view, View? next)
        {
            var label = DefaultLayout(
                new Label
                {
                    Text = _text
                },
                next);

            _changeLabel?.Invoke(label);
            view.Add(label);

            return label;
        }
    }

    private sealed class CompountFragment : TextFragment
    {
        private readonly IReadOnlyCollection<TextFragment> _fragments;

        public CompountFragment(IEnumerable<TextFragment> fragments)
        {
            _fragments = fragments.ToArray();
        }

        protected override View Render(View view, View? next)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var fragment in _fragments)
                next = fragment.Render(
                    view,
                    next);

            return view;
        }
    }*/
}