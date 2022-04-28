using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.SpoolDisplay;

namespace CelloManager.Avalonia.Views.SpoolDisplay;

public partial class SpoolDisplayView : ReactiveUserControl<SpoolDisplayViewModel>
{
    public SpoolDisplayView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}