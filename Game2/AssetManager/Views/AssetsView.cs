using AssetManager.ViewModels;
using Avalonia.ReactiveUI;

namespace AssetManager.Views;

public sealed partial class AssetsView : ReactiveUserControl<AssetsViewModel>
{
    public AssetsView()
    {
        InitializeComponent();
    }
}