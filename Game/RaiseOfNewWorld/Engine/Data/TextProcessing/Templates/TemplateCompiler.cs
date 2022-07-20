using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using NStack;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Ast;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;
using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing.Templates;

public sealed class TemplateCompilerOld : TemplateMatcherVisitor, IEnumerable<CompiledTemplate>
{
    
    public abstract class ProcessElement
    {
        
    }
    
    private sealed class TemplateElement : MatcherElement, IDisposable
    {
        public List<ProcessElement> Expressions { get; } = Pools.TemplateCompilerPool.Get();

        public void Dispose() => Pools.TemplateCompilerPool.Return(Expressions);
        
        public override void SetMatcher(MatcherElement expression) => Expressions.Add((ProcessElement)expression);
    }
    
    private abstract class MatcherElement : ProcessElement
    {
        public abstract void SetMatcher(MatcherElement expression);

        public virtual Expression BuildExpressions(ParameterExpression parameterExpression)
            => Expression.Throw(
                Expression.New(
                    typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) })!,
                    Expression.Constant("No Matcher Expression Builder is Overriden")));

        protected void ThrowInvalidmatcher()
            => throw new InvalidOperationException("To manay MatcherElements");
        
        protected void ThrowInvalidmatcherToFew()
            => throw new InvalidOperationException("To few MatcherElements"); 
    }
    
    private sealed class AndMatcherElement : MatcherElement
    {
        private MatcherElement? _left;
        private MatcherElement? _right;
        
        public override void SetMatcher(MatcherElement expression)
        {
            if (_left is null)
                _left = expression;
            else if (_right is null)
                _right = expression;
            else
                ThrowInvalidmatcher();
        }

        public override Expression BuildExpressions(ParameterExpression parameterExpression)
        {
            if (_left is not null && _right is not null)
                return Expression.And(
                    _left.BuildExpressions(parameterExpression),
                    _right.BuildExpressions(parameterExpression));
            
            ThrowInvalidmatcherToFew();
            return base.BuildExpressions(parameterExpression);

        }
    }
    
    private sealed class NameMatcherElement : MatcherElement
    {
        private static readonly PropertyInfo IdProperty = typeof(View).GetProperty("Id") ?? throw new InvalidOperationException("No Id property Found");
        
        private readonly string _targetName;

        public NameMatcherElement(string targetName) => _targetName = targetName;

        public override void SetMatcher(MatcherElement expression) => ThrowInvalidmatcher();

        public override Expression BuildExpressions(ParameterExpression parameterExpression)
        {
            return Expression.Equal(
                Expression.Property(parameterExpression, IdProperty), 
                Expression.Constant(ustring.Make(_targetName)));
        }
    }
    
    private sealed class NotMatcher : MatcherElement
    {
        private readonly 
        
        public override void SetMatcher(MatcherElement expression)
        {
            
        }
    }
    
    private readonly Stack<ProcessElement> _expressionStack = new();
    private readonly List<CompiledTemplate> _templates = new();

    private TType GetLastElement<TType>()
        where TType : ProcessElement
    {
        if (_expressionStack.Count == 0)
            throw new InvalidOperationException("No Expression in Expression Stack");

        if (_expressionStack.Last() is not TType element)
            throw new InvalidOperationException($"The last Element has not the Right Type: {typeof(TType)}");

        return element;
    }

    private TMatcher SetLast<TMatcher>(TMatcher matcher)
        where TMatcher : MatcherElement
    {
        var last = GetLastElement<MatcherElement>();
        last.SetMatcher(matcher);
        return matcher;
    }
    
    public override void VisitAndMatcher(AndMatcherNode andMatcherNode)
    {
        var newElement = SetLast(new AndMatcherElement());
        
        _expressionStack.Push(newElement);
        Accept(andMatcherNode.Left);
        Accept(andMatcherNode.Right);
        _expressionStack.Pop();
    }

    public override void VisitNameMatch(NameMatchNode nameMatchNode) => SetLast(new NameMatcherElement(nameMatchNode.Name));

    public override void VisitNot(NotMatcherNode notMatcherNode)
    {
        throw new NotImplementedException();
    }

    public override void VisitOrMatcher(OrMatcherNode orMatcherNode)
    {
        throw new NotImplementedException();
    }

    public override void VisitRegexMatcher(RegexMatcherNode regexMatcherNode)
    {
        throw new NotImplementedException();
    }

    public override void VisitTemplateEntry(TemplateEntryNode templateEntryNode)
    {
        throw new NotImplementedException();
    }

    public override void VisitTemplateReference(TemplateReferenceNode templateReferenceNode)
    {
        throw new NotImplementedException();
    }

    public override void VisitTypeMatcher(TypeMatcherNode typeMatcherNode)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<CompiledTemplate> GetEnumerator()
        => _templates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}