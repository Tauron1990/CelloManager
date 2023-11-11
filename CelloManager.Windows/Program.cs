namespace CelloManager.Windows;

public static class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        Bootstrapper.PrintProvider = new WindowsPrintingProvider();
        await Bootstrapper.StartApp(args).ConfigureAwait(true);
    }
}