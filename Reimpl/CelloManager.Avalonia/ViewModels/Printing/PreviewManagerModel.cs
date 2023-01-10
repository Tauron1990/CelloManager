using System;
using System.Drawing.Printing;

namespace CelloManager.ViewModels.Printing;

public sealed class PreviewManagerModel : ViewModelBase, IDisposable
{
    private readonly PreviewPrintController _previewPrintController = new();
    
    
    public void MakePreView(PrintDocument document)
    {
        document.PrintController = _previewPrintController;
    }

    public void Dispose()
    {
    }
}