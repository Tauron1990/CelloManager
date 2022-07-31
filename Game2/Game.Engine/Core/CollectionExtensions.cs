using System.Collections.Specialized;

namespace Game.Engine.Core;

public static class CollectionExtensions
{
    public static IDictionary<string, object> ToDictionary(this NameValueCollection? @this)
    {
        var dict = new Dictionary<string, object>();

        if (@this is not null)
        {
            foreach (var key in @this.AllKeys)
            {
                dict.Add(key ?? string.Empty, @this[key] ?? string.Empty);
            }
        }

        return dict;
    }
    
    public static IEnumerable<KeyValuePair<string, string>> EnumerateEntrys(this NameValueCollection? @this)
    {
        if (@this is not null)
            foreach (var key in @this.AllKeys)
                yield return KeyValuePair.Create(key ?? string.Empty, @this[key] ?? string.Empty);
    }
}