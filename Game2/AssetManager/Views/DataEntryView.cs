using AssetManager.ViewModels;
using Avalonia.ReactiveUI;

namespace AssetManager.Views;

public partial class DataEntryView : ReactiveUserControl<DataEntryViewModel>
{
    public DataEntryView()
    {
        InitializeComponent();
    }
}