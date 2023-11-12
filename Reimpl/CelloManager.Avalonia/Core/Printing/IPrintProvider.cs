using System.Threading.Tasks;
using Avalonia.Threading;
using QuestPDF.Infrastructure;

namespace CelloManager.Core.Printing;

public interface IPrintProvider
{
    ValueTask RunPinting(IDocument document);

    ValueTask Shutdown();
}