namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

public enum TemplateTokentype
{
    Eof,
    Text,
    OpenTemplate,
    CloseTemplate,
    TemplateMatchSeperator,
}

public record struct TemplateToken(string Text, TemplateTokentype Type, int Position);