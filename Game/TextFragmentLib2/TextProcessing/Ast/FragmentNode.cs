﻿using TextFragmentLib2.TextProcessing.Parsing;

namespace TextFragmentLib2.TextProcessing.Ast;

public abstract class FragmentNode : AstNode
{
    public abstract TReturn Visit<TReturn>(FragmentNodeVisitor<TReturn> fragmentNodeVisitor);
}