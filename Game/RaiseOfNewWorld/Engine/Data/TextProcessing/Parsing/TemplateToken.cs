namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public enum TemplateTokentype
{
    Eof,
    Text,
    OpenTemplate,
    CloseTemplate,
    TemplateMatchSeperator,
    MatchNot,
    OpenExpression,
    CloseExpression
}

public record struct TemplateToken(string Text, TemplateTokentype Type, int Position);