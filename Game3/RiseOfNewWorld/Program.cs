// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using Raylib_CsLo;

namespace RiseOfNewWorld;

static class Program
{
    // [DllImport("raylib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    // [return: MarshalAs(UnmanagedType.U1)]
    // public static extern bool GuiSpinner(
    //     Rectangle bounds,
    //     [MarshalAs(UnmanagedType.LPStr)]
    //     string text,
    //     ref int value,
    //     int minValue,
    //     int maxValue,
    //     [MarshalAs(UnmanagedType.U1)] 
    //     bool editMode);
    
    public static int Main(string[] args)
    {
        Raylib.InitWindow(0, 0, "Hallo Welt");
        Raylib.MaximizeWindow();
        
        new GameManager().StartApplication();
        
        Raylib.CloseWindow();

        return 0;
    }
}