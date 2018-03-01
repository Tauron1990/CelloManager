namespace Tauron.Application.CelloManager.Logic.RefillPrinter.DTO
{
    public class RefillPrinterResult
    {
        public bool Result { get; }

        public RefillPrinterResult(bool result)
        {
            Result = result;
        }
    }
}