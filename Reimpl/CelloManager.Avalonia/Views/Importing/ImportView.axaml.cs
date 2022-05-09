using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.Importing;

namespace CelloManager.Avalonia.Views.Importing;

public sealed partial class ImportView : ReactiveUserControl<ImportViewModel>
{
    public ImportView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}