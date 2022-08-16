namespace Game.Engine;

/// <summary>
///     Provides extension methods for the <see cref="Int32" /> type.
/// </summary>
public static class Int32Utils
{
    /// <summary>
    ///     Gets an integer from a single hex character
    /// </summary>
    /// <param name="digit">The single hex character</param>
    /// <returns>The integer</returns>
    public static int FromHex(this char digit)
    {
        if (digit is (< '0' or > '9') and (< 'A' or > 'F') and (< 'a' or > 'f'))
            throw new ArgumentException("digit");


        return FromHexNoCheck(digit);
    }

    /// <summary>
    ///     Gets an integer from a single hex character without checking whether the character is
    ///     a valid hex character.
    /// </summary>
    /// <param name="digit">The single hex character</param>
    /// <returns>The integer</returns>
    internal static int FromHexNoCheck(this char digit)
    {
        if (digit > '9')
            return (digit <= 'F' ? digit - 'A' : digit - 'a') + '\n';


        return digit - '0';
    }
}