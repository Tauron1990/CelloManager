using System.Collections;
using System.Collections.Immutable;

namespace Game.Engine.Core;

public class WeakReferenceDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    where TValue : class where TKey : notnull
{
    private readonly IDictionary<TKey, WeakReference<TValue>> _weakReferences;
    private readonly Timer _purgeTimer;
    
    public WeakReferenceDictionary(IDictionary<TKey, WeakReference<TValue>>? dictionary = null)
    {
        _weakReferences = dictionary ?? new Dictionary<TKey, WeakReference<TValue>>();
        _purgeTimer = new Timer(Purge, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
    }

    public WeakReferenceDictionary(int capacity)
        : this(new Dictionary<TKey, WeakReference<TValue>>(capacity))
    { }

    private void Purge(object? state)
    {
        if(_weakReferences.IsReadOnly) return;
        if(_weakReferences.Count == 0) return;

        foreach (var pair in _weakReferences.ToArray())
        {
            if(pair.Value.TryGetTarget(out _))
                continue;
            _weakReferences.Remove(pair.Key);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        TValue? GetData(WeakReference<TValue> reference) => reference.TryGetTarget(out var value) ? value : default;

        return
        (
            from value in _weakReferences
            let inst = GetData(value.Value)
            where inst is not null
            select KeyValuePair.Create(value.Key, inst)
        ).GetEnumerator()!;

    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<TKey, TValue> item)
        => _weakReferences.Add(KeyValuePair.Create(item.Key, new WeakReference<TValue>(item.Value)));

    public void Clear()
        => _weakReferences.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item)
        => _weakReferences.ContainsKey(item.Key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        var index = 0;
        foreach (var weakReference in _weakReferences)
        {
            weakReference.Value.TryGetTarget(out var item);
            array[arrayIndex + index] = KeyValuePair.Create(weakReference.Key, item!);
            index++;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
        => _weakReferences.Remove(item.Key);

    public int Count => _weakReferences.Count;

    public bool IsReadOnly => _weakReferences.IsReadOnly;

    public void Add(TKey key, TValue value)
        => _weakReferences.Add(key, new WeakReference<TValue>(value));

    public bool ContainsKey(TKey key)
        => _weakReferences.ContainsKey(key);

    public bool Remove(TKey key)
        => _weakReferences.Remove(key);

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_weakReferences.TryGetValue(key, out var reference) && reference.TryGetTarget(out value!))
            return true;

        value = default!;
        return false;
    }

    public TValue this[TKey key]
    {
        get => _weakReferences[key].TryGetTarget(out var item) ? item : default!;
        set
        {
            if(_weakReferences.TryGetValue(key, out var reference))
                reference.SetTarget(value);
            else
                _weakReferences.Add(key, new WeakReference<TValue>(value));
        }
    }

    public ICollection<TKey> Keys => _weakReferences.Keys;

    public ICollection<TValue> Values =>
    (
        from value in this
        select value.Value
    ).ToImmutableList();

    ~WeakReferenceDictionary() => _purgeTimer.Dispose();
}