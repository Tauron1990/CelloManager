using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CelloManager.Avalonia.ViewModels.Editing;

namespace CelloManager.Avalonia.Views.Editing;

public partial class SpoolEditorView : ReactiveUserControl<ModifySpoolEditorViewModel>
{
    public SpoolEditorView()
    {
        InitializeComponent();
    }
}