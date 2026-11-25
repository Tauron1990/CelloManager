using System;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

namespace CelloManager.Views;

public sealed class MainWindowProvider
{
    private Window? _window;
    
    public Window MainWindow => _window ?? throw new InvalidOperationException("MainWindow is not initialized");
    
    public ILauncher Launcher => MainWindow.Launcher;

    public IStorageProvider StorageProvider => MainWindow.StorageProvider;

    public IClipboard? Clipboard => MainWindow.Clipboard;
    
    public void SetMainWindow(Window window)
    {
        _window = window;
    }
}