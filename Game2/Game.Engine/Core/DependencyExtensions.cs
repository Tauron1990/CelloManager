using System.Diagnostics;
using JetBrains.Annotations;

namespace Game.Engine.Core;

[PublicAPI]
public static class DependencyExtensions
{
    private enum VisitState
    {
        NotVisited,
        Visiting,
        Visited
    };

    private static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) => dictionary.TryGetValue(key, out var value) ? value : defaultValue;

    private static void TryPush<T>(T node, Func<T, IEnumerable<T>> lookup, Stack<KeyValuePair<T, IEnumerator<T>>> stack, IDictionary<T, VisitState> visited, ICollection<List<T>> cycles)
        where T : notnull
    {
        var state = visited.ValueOrDefault(node, VisitState.NotVisited);
        switch (state)
        {
            case VisitState.Visited:
                return;
            case VisitState.Visiting:
            {
                Debug.Assert(stack.Count > 0);
                var list = stack.Select(pair => pair.Key).TakeWhile(parent => !EqualityComparer<T>.Default.Equals(parent, node)).ToList();
                list.Add(node);
                list.Reverse();
                list.Add(node);
                cycles.Add(list);
                break;
            }
            case VisitState.NotVisited:
            default:
                visited[node] = VisitState.Visiting;
                stack.Push(new KeyValuePair<T, IEnumerator<T>>(node, lookup(node).GetEnumerator()));
                break;
        }
    }

    private static List<List<T>> FindCycles<T>(T root, Func<T, IEnumerable<T>> lookup, Dictionary<T, VisitState> visited) where T : notnull
    {
        var stack = new Stack<KeyValuePair<T, IEnumerator<T>>>();
        var cycles = new List<List<T>>();

        TryPush(root, lookup, stack, visited, cycles);
        while (stack.Count > 0)
        {
            var pair = stack.Peek();
            if (!pair.Value.MoveNext())
            {
                stack.Pop();                    
                visited[pair.Key] = VisitState.Visited;
                pair.Value.Dispose();
            }
            else
                TryPush(pair.Value.Current, lookup, stack, visited, cycles);
        }
        return cycles;
    }

    public static List<List<T>> FindCycles<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> edges) 
        where T : notnull
    {
        var cycles = new List<List<T>>();
        var visited = new Dictionary<T, VisitState>();
        foreach (var node in nodes)
            cycles.AddRange(FindCycles(node, edges, visited));
        return cycles;
    }

    public static List<List<T>> FindCycles<T, TValueList>(this IDictionary<T, TValueList> listDictionary)
        where TValueList : class, IEnumerable<T> where T : notnull =>
        listDictionary.Keys.FindCycles(key => listDictionary!.ValueOrDefault(key, null) ?? Enumerable.Empty<T>());
}