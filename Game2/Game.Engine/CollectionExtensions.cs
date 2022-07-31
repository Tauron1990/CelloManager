namespace Game.Engine;

public static class CollectionExtensions
{
    public static bool ElementsAreEqual<TType>(this IEnumerable<TType> first, IEnumerable<TType> second)
    {
        using var secondEnumerator = second.GetEnumerator();
        foreach (var element1 in first)
        {
            if (!secondEnumerator.MoveNext())
                return false;
            if (Equals(secondEnumerator.Current, element1))
                continue;
            return false;
        }

        return true;
    }
}