using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using QuestPDF.Fluent;

namespace CelloManager.Core.Printing.Impl;

public sealed class PdfDocument : FileSelectingDocument<PdfDocument>
{
    public override DocumentType Type => DocumentType.Pdf;
    protected override async ValueTask RenderTo(Dispatcher dispatcher, IStorageFile file)
    {
        var stream = await file.OpenWriteAsync().ConfigureAwait(false);
        await using (stream.ConfigureAwait(false))
            PrintView.GeneratePdf(stream);
    }

    protected override async ValueTask<FilePickerSaveOptions> ConfigurateDialog() =>
        new()
        {
            Title = "PDF Datei",
            SuggestedFileName = "print.pdf",
            DefaultExtension = ".pdf",
            FileTypeChoices = new []
            {
                new FilePickerFileType("PDF")
                {
                    Patterns = new []{"pdf"},
                },
            },
            SuggestedStartLocation = await GetFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).ConfigureAwait(false),
        };

}