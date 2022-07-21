﻿using System.Collections.Immutable;
using System.Reactive;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.PrimitiveVisitor;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Templates;
using SystemsRx.Extensions;
using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public sealed class ViewAttributeApplayer : FragmentNodeVisitor<Unit>
{
    private ImmutableDictionary<string, View> _views;
    private ImmutableList<CompiledTemplate> _templates = ImmutableList<CompiledTemplate>.Empty;

    public ViewAttributeApplayer(ImmutableDictionary<string, View> views)
        => _views = views;

    public override Unit VisitTextFragment(TextFragmentNode textFragmentNode)
    {
        if(!_views.TryGetValue(textFragmentNode.Name, out var view))
            return Unit.Default;

        _templates
            .SelectMany(t => t.Entrys)
            .Where(t => t.IsMatch(view))
            .ForEachRun(e => textFragmentNode.Intigrate(e.Attributes));
        
        
    }

    public override Unit VisitTextData(TextDataNode textDataNode)
    {
        _templates = textDataNode.Templates.Select(TemplateCompiler.GetTemplate).ToImmutableList();
        try
        {
            return textDataNode.FragmentNodes.Select(Accept).LastOrDefault();
        }
        finally
        {
            _templates = ImmutableList<CompiledTemplate>.Empty;
        }
    }
    
      // ReSharper disable once CognitiveComplexity
    public void ApplyAttributes(View view, ImmutableArray<AttributeNode> datas)
    {
        foreach (var value in datas)
        {
            ApplyViewAttributes(view, value.Name, value.Value);
            switch (view)
            {
                case Button button:
                    ApplyButtonAttributes(button, value.Name, value.Value);
                    break;
                case CheckBox checkBox:
                    ApplyChekBoxAttribute(checkBox, value.Name, value.Value);
                    break;
                case ColorPicker colorPicker:
                    ApplyColorPickerAttribute(colorPicker, value.Name, value.Value);
                    break;
                case FrameView frameView:
                    ApplyFrameviewAttributes(frameView, value.Name, value.Value);
                    break;
                case LineView lineView:
                    ApplyLineViewAttributes(lineView, value.Name, value.Value);
                    break;
            }
        }
        
        view.EndInit();
    }

    private void ApplyLineViewAttributes(LineView lineView, string name, AttributeValueNode value)
    {
        switch (name)
        {
            case "startinganchor":
                lineView.StartingAnchor = StringVisitor.Evaluate(value)[0];
                break;
            case "endinganchor":
                lineView.EndingAnchor = StringVisitor.Evaluate(value)[0];
                break;
            case "linerune":
                lineView.LineRune = StringVisitor.Evaluate(value)[0];
                break;
            case "orientation":
                lineView.Orientation = Enum.Parse<Orientation>(StringVisitor.Evaluate(value));
                break;
        }
    }

    
    private void ApplyFrameviewAttributes(FrameView frameView, string name, AttributeValueNode value)
    {
        if (name == "title")
            frameView.Title = StringVisitor.Evaluate(value);
    }
    
    private void ApplyColorPickerAttribute(ColorPicker colorPicker, string name, AttributeValueNode value)
    {
        if (name == "selectedcolor") colorPicker.SelectedColor = Enum.Parse<Color>(StringVisitor.Evaluate(value));
    }
    private void ApplyChekBoxAttribute(CheckBox checkBox, string name, AttributeValueNode value)
    {
        if (name == "checked") checkBox.Checked = BoolVisitor.Evaluate(value);
    }
    private void ApplyButtonAttributes(Button button, string name, AttributeValueNode value)
    {
        if (name == "isdefault") button.IsDefault = BoolVisitor.Evaluate(value);
    }
    
    // ReSharper disable once CognitiveComplexity
    private void ApplyViewAttributes(View view, string name, AttributeValueNode value)
    {
        switch (name.ToLower())
        {
            case "canfocus":
                view.CanFocus = BoolVisitor.Evaluate(value);
                break;
            case "enabled":
                view.Enabled = BoolVisitor.Evaluate(value);
                break;
            case "visible":
                view.Enabled = BoolVisitor.Evaluate(value);
                break;
            case "hotkey":
                view.HotKey = ParseKey(StringVisitor.Evaluate(value));
                break;
            case "hotkeyspecifier":
                view.HotKeySpecifier = StringVisitor.Evaluate(value)[0];
                break;
            case "shortcut":
                view.Shortcut = ParseKey(StringVisitor.Evaluate(value));
                break;
            case "tabindex":
                view.TabIndex = DoubleVisitor.EvaluateInt(value);
                break;
            case "tabstop":
                view.TabStop = BoolVisitor.Evaluate(value);
                break;
            case "id":
                view.Id = value;
                break;
            case "x":
                view.X =  PosVisitor.Evaluate() e(value).Evaluate(context);
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
                view.ColorScheme = ColorExtensions.Parse(value).MergeColorScheme(view.ColorScheme);
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