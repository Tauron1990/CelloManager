using System.Drawing.Printing;
using System.Threading.Tasks;

namespace CelloManager.Avalonia.Core.Movere.Services
{
    public interface IPrintDialogService
    {
        Task<bool> ShowDialogAsync(PrintDocument document);
    }
}