using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Game.Engine;

/// <summary>
/// A helper class for creating predicate based on a regular expression.
/// Use the <see cref="PredicateUtils"/> to create actual regular expression predicate. 
/// </summary>
[PublicAPI]
public class RegexBasedPredicateHelper
{
    /// <summary>
    /// The regular expression
    /// </summary>
    public virtual Regex Regex { get; }

    /// <summary>
    /// Creates a new <see cref="RegexBasedPredicateHelper"/> from the given <see cref="regex"/>.
    /// </summary>
    /// <param name="regex">The <see cref="regex"/></param>
    protected internal RegexBasedPredicateHelper(string regex)
        : this(new Regex(regex))
    {
    }

    /// <summary>
    /// Creates a new <see cref="RegexBasedPredicateHelper"/> from the given <see cref="regex"/>.
    /// </summary>
    /// <param name="regex">The <see cref="regex"/></param>
    protected internal RegexBasedPredicateHelper(Regex regex) => Regex = regex;

    public static bool IsRegexBasedPredicate(Predicate<string> regexBasedPredicate) => regexBasedPredicate.Target is RegexBasedPredicateHelper;

    /// <summary>
    /// Gets the regular expression from the given predicate.
    /// </summary>
    /// <param name="regexBasedPredicate"></param>
    /// <exception cref="ArgumentException">The given predicate was not created by<see cref="RegexBasedPredicateHelper"/></exception>
    /// <returns>The <see cref="Regex"/> associated with the <see cref="Predicate{T}"/></returns>
    public static Regex GetRegexFromPredicate(Predicate<string> regexBasedPredicate)
    {
        if (regexBasedPredicate.Target is not RegexBasedPredicateHelper predicateHelper)
            throw new ArgumentException($"Must be a predicateHelper created from {nameof(RegexBasedPredicateHelper)}");

        return predicateHelper.Regex;
    }

    public virtual bool Accept(string value)
    {
        return Regex.IsMatch(value);
    }

    public virtual Predicate<string> ToPredicate() => this;

    public static implicit operator Predicate<string>(RegexBasedPredicateHelper predicateHelper) => predicateHelper.Accept;
}