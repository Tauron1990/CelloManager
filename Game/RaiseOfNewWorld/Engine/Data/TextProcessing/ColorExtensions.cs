using Terminal.Gui;

namespace RaiseOfNewWorld.Engine.Data.TextProcessing;

public static class ColorExtensions
{
    public static ColorScheme MergeColorScheme(this ColorScheme original, ColorScheme? toMerge)
    {
        if (toMerge is null) return original;
        
        if (original.Normal == default)
            original.Normal = toMerge.Normal;
        
        if (original.Disabled == default)
            original.Disabled = toMerge.Disabled;
        
        if (original.Focus == default)
            original.Focus = toMerge.Focus;
        
        if (original.HotFocus == default)
            original.HotFocus = toMerge.HotFocus;
        
        if (original.HotNormal == default)
            original.HotNormal = toMerge.HotNormal;

        return original;
    }
}
