// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using Raylib_CsLo;

namespace RiseOfNewWorld;

static class Program
{
    [DllImport("raylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool GuiSpinner(
        Rectangle bounds,
        [MarshalAs(UnmanagedType.LPStr)]
        string text,
        ref int value,
        int minValue,
        int maxValue,
        [MarshalAs(UnmanagedType.U1)] 
        bool editMode);
    
    static int _test = 0;
    
    public static unsafe int Main(string[] args)
    {
        const int screenWidth = 800;
        const int screenHeight = 450;

        Raylib.InitWindow(screenWidth, screenHeight, "Hallo Welt");

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Raylib.DARKBLUE);
            Raylib.DrawText("Hallo Welt", 190, 200, 20, Raylib.BLACK);

            var spinnerText = "Test Spinner";
            GuiSpinner(new Rectangle(100, 10, 100, 20), spinnerText , ref _test, 0, 20, true);
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();

        return 0;
    }
}