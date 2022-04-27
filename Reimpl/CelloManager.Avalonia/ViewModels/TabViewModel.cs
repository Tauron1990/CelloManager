using System;
using System.Reactive;
using DynamicData;
using ReactiveUI;

namespace CelloManager.Avalonia.ViewModels;

public sealed class TabViewModel : ViewModelBase, IDisposable
{
    private bool _canClose;
    private string _title = string.Empty;
    private ViewModelBase? _content;

    public bool CanClose
    {
        get => _canClose;
        set => this.RaiseAndSetIfChanged(ref _canClose, value);
    }

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public ViewModelBase? Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public ReactiveCommand<Unit, Unit> Close { get; }

    public TabViewModel(Action close) => Close = ReactiveCommand.Create(close);

    public void Dispose() => Close.Dispose();

    public static TabViewModel Create(ViewModelBase viewModelBase, ISourceList<ViewModelBase> tabs)
    {
        
    }
}

public interface 

