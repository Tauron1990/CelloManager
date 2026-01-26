using System;
using System.IO;
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
        Stream stream = await file.OpenWriteAsync().ConfigureAwait(false);
        await using (stream.ConfigureAwait(false))
        {
            PrintView.GeneratePdf(stream);
        }
    }

    protected override async ValueTask<FilePickerSaveOptions> ConfigurateDialog() =>
        new()
        {
            Title = "PDF Datei",
            SuggestedFileName = "print.pdf",
            DefaultExtension = ".pdf",
            FileTypeChoices =
            [
                new FilePickerFileType("PDF")
                                  {
                                      Patterns = ["pdf"],
                                  },
            ],
            SuggestedStartLocation = await GetFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)).ConfigureAwait(false)
        };
}