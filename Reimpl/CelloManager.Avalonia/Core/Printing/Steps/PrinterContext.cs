using System;
using System.ComponentModel.Design;
using Avalonia.Threading;
using CelloManager.Core.Data;
using CelloManager.Core.Printing.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace CelloManager.Core.Printing.Steps;

public sealed class PrinterContext
{
    private Dispatcher? _dispatcher;
    private TempFiles? _tempFiles;
    
    public PendingOrder? Order { get; set; }

    public IServiceProvider ServiceProvider { get; set; } = new ServiceContainer();

    public TempFiles TempFiles => GetOrCreate(ref _tempFiles);
    
    public Dispatcher Dispatcher
    {
        get => _dispatcher ??= Dispatcher.UIThread;
        set => _dispatcher = value;
    }

    public bool NeedSpliting { get; set; }

    private TService GetOrCreate<TService>(ref TService? service) 
        where TService : notnull
    {
        if(service is null)
            service = ServiceProvider.GetRequiredService<TService>();
        return service;
    }
}