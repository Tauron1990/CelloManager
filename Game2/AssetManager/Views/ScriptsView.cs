using AssetManager.ViewModels;
using Avalonia.ReactiveUI;

namespace AssetManager.Views;

public sealed partial class ScriptsView : ReactiveUserControl<ScriptsViewModel>
{
    public ScriptsView()
    {
        InitializeComponent();
    }
}