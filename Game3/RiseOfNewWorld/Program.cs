// See https://aka.ms/new-console-template for more information

using EcsRx.Plugins.Views.Components;
using EcsRx.Plugins.Views.Systems;
using EcsRx.Plugins.Views.ViewHandlers;
using Raylib_CsLo;

const int screenWidth = 800;
const int screenHeight = 450;

Raylib.InitWindow(screenWidth, screenHeight, "Hallo Welt");


while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    
    Raylib.ClearBackground(Raylib.DARKBLUE);
    Raylib.DrawText("Hallo Welt", 190, 200, 20, Raylib.BLACK);
    
    Raylib.EndDrawing();
}

Raylib.CloseWindow();

return 0;