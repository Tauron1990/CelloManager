﻿using System.Collections.Immutable;
using System.Text;
using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public sealed class TextFragmentNode : FragmentContainerNode
{
    public string Name { get; }

    public TypeRepesentation Type { get; }

    public string Text { get; }

    public ImmutableList<AttributeNode> Attributes { get; set; }

    public TextFragmentNode(string name, TypeRepesentation type, string text, ImmutableList<AttributeNode> attributes,
        ImmutableList<TextFragmentNode> fragmentNodes)
        : base(fragmentNodes)
    {
        Name = name;
        Type = type;
        Text = text;
        Attributes = attributes;
    }

    public void Intigrate(IEnumerable<AttributeNode> attributes)
    {
        foreach (var attributeNode in attributes)
        {
            var element = Attributes.Find(n => n.Name == attributeNode.Name);
            if (element is null)
                Attributes = Attributes.Add(attributeNode);
            else
                element.Value = element.Value.Merge(attributeNode.Value);
        }
    }

    public override void Validate()
    {
        Attributes.ForEach(a => a.Validate());
    }

    protected override string Format()
    {
        var builder = new StringBuilder();

        builder.Append('{').Append(Type);

        if (!string.IsNullOrWhiteSpace(Name))
            builder.Append($":{Name}");

        builder.Append('(')
            .AppendJoin(
                ',',
                Attributes)
            .AppendLine(")");

        if (!string.IsNullOrWhiteSpace(Text))
            builder.AppendLine(Text);

        FormatFragments(builder);

        builder.Append('}');

        return builder.ToString();
    }

    public override TReturn Visit<TReturn>(FragmentNodeVisitor<TReturn> fragmentNodeVisitor)
        => fragmentNodeVisitor.VisitTextFragment(this);
}