using System;
using System.Linq;
using DynamicData;

namespace AssetManager.Models;

public abstract class NewEntrySelector
{
    public abstract IObservable<IChangeSet<NewEntrySelectorButton>> Buttons { get; }

    public abstract void Select(NewEntrySelectorButton button);
}

public abstract class NewEntrySelectorButton
{
    public abstract string ButtonContent { get; }
}

public sealed class NewEntryButton<TValue> : NewEntrySelectorButton
{
    public TValue Value { get; }

    public NewEntryButton(string name, TValue value)
    {
        Value = value;
        ButtonContent = name;
    }

    public override string ButtonContent { get; }
}

public sealed class NewEntryModelData<TValue> : NewEntrySelector
{
    private readonly Action<TValue> _result;
    private readonly SourceList<NewEntryButton<TValue>> _buttons = new();

    public override IObservable<IChangeSet<NewEntrySelectorButton>> Buttons
        => _buttons.Connect().Cast(b => (NewEntrySelectorButton)b);
    
    public NewEntryModelData(Action<TValue> result, params (string Name, TValue Value)[] buttons)
    {
        _result = result;
        _buttons.AddRange(buttons.Select(t => new NewEntryButton<TValue>(t.Name, t.Value)));
    }
    
    public override void Select(NewEntrySelectorButton button) => _result(((NewEntryButton<TValue>)button).Value);
}