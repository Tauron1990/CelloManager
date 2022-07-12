using System.Collections.Immutable;
using System.Text;
using RaiseOfNewWorld.Engine.Data.TextProcessing.Parsing;

namespace RaiseOfNewWorld.Engine.Data.TextProcessingOld;

public abstract class ExpressionNode<TResult>
{   
    protected abstract class ExpressionParser
    {
        private readonly string _input;

        protected ExpressionParser(string input) => _input = input;

        protected abstract SubstractExpression<TResult> CreateSubstract(ExpressionNode<TResult> first, StringTokenizer input);

        protected abstract AddExpression<TResult> CreateAdd(ExpressionNode<TResult> first, StringTokenizer input);

        protected abstract ExpressionNode<TResult> CreateFabricator(ParameterParser token);

        protected ExpressionNode<TResult> Create(StringTokenizer tokenizer)
        {
            ValidateNext(tokenizer);

            var token = tokenizer.GetAndIncement();
            var node = CreateFabricator(new ParameterParser(token));
            if (tokenizer.CanNext)
            {
                token = tokenizer.GetAndIncement();
                return token switch
                {
                    "+" => CreateAdd(node, tokenizer),
                    "-" => CreateSubstract(node, tokenizer),
                    _ => node
                };
            }

            return node;
        }

        public ExpressionNode<TResult> Create()
        {
            var tokenizer = new StringTokenizer(GetTokens().ToImmutableArray());

            return Create(tokenizer);
        }

        protected void ValidateNext(StringTokenizer tokenizer)
        {
            if(tokenizer.CanNext) return;

            throw new InvalidOperationException("No next Token in Expression Parser");
        }
        
        // ReSharper disable once CognitiveComplexity
        private IEnumerable<string> GetTokens()
        {
            var builder = new StringBuilder();
            
            foreach (var c in _input)
            {
                switch (c)
                {
                    case '+':
                        if (builder.Length != 0)
                            yield return builder.ToString();
                        builder.Clear();

                        yield return "+";
                        break;
                    case '-':
                        if (builder.Length != 0)
                            yield return builder.ToString();
                        builder.Clear();

                        yield return "-";
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            if (builder.Length != 0)
                yield return builder.ToString();
        }
    }

    public abstract TResult Evaluate(ViewContext context);
}