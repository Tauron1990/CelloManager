﻿using System.Collections.Immutable;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public interface ITextData
{
    
}

public sealed record AttributeData(string Name, string Value);

public sealed record SimpleText(string Text) : ITextData;

public sealed record TextData(string? Name, string? Type, ImmutableArray<AttributeData> Attributes, ImmutableArray<ITextData> Content) : ITextData;