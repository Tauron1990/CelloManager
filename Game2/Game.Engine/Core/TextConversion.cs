using System.Text;
using JetBrains.Annotations;

// ReSharper disable CognitiveComplexity

namespace Game.Engine.Core;

/// <summary>
///     Provides useful methods for converting text.
/// </summary>
[PublicAPI]
public static class TextConversion
{
    public static readonly char[] Base32Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    public static readonly char[] Base32AlphabetLowercase = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();

    public static readonly char[] HexValues = "0123456789ABCDEF".ToCharArray();
    public static readonly char[] HexValuesLowercase = "0123456789abcdef".ToCharArray();

    public static readonly char[] SoundexValues =
    {
        //A  B   C   D   E   F   G   H   I   J   K   L   M
        '0', '1', '2', '3', '0', '1', '2', '0', '0', '2', '2', '4', '5',
        //N  O   P   W   R   S   T   U   V   W   X   Y   Z
        '5', '0', '1', '2', '6', '2', '3', '0', '1', '0', '2', '0', '2'
    };

    private static readonly Predicate<char> PredicateIsAsciiLetterOrDigit = PredicateUtils.Not<char>(IsAsciiLetterOrDigit);

    /// <summary>
    ///     Returns True if the char is an ascii letter (A-Z, a-z) or a digit (0-9).
    /// </summary>
    /// <param name="c">The character to check</param>
    /// <returns>True if the char is an ascii letter or digit</returns>
    public static bool IsAsciiLetterOrDigit(this char c) =>
        (c >= 48 && c <= 57) // 0-9
        || (c >= 65 && c <= 90) // A-Z
        || (c >= 97 && c <= 122); // a-z

    /// <summary>
    ///     Returns true if the char is a hexadecimal digit (0-9, A-F, a-f).
    /// </summary>
    /// <param name="c">The character to check</param>
    /// <returns>True if the char is a hexadecimal digit</returns>
    public static bool IsHexDigit(this char c) => Uri.IsHexDigit(c);

    /// <summary>
    ///     Decodes a string from an escaped hex string (URL encoded).
    /// </summary>
    /// <param name="s">The escaped hex encoded string</param>
    /// <returns>The deocded string</returns>
    public static string FromEscapedHexString(string s) => Uri.UnescapeDataString(s);

    public static void WriteSoundex(TextWriter writer, string value)
    {
        var length = 0;
        var previousChar = '?';

        if (value.Length > 0)
        {
            writer.Write(value[0]);

            for (var i = 1; i < value.Length && length < 4; i++)
            {
                var currentChar = char.ToUpper(value[i]);

                if (currentChar is >= 'A' and <= 'Z' && currentChar != previousChar)
                {
                    var soundexChar = SoundexValues[currentChar - 'A'];

                    if (soundexChar != '0')
                    {
                        length++;
                        writer.Write(soundexChar);
                    }


                    previousChar = currentChar;
                }
            }
        }


        while (length < 4)
        {
            length++;
            writer.Write('0');
        }
    }

    public static string ToSoundex(string value)
    {
        var previousChar = '?';
        var retval = new StringBuilder(4);

        if (value.Length > 0)
        {
            retval.Append(value[0]);

            for (var i = 1; i < value.Length && retval.Length < 4; i++)
            {
                var currentChar = char.ToUpper(value[i]);

                if (currentChar is >= 'A' and <= 'Z' && currentChar != previousChar)
                {
                    var soundexChar = SoundexValues[currentChar - 'A'];

                    if (soundexChar != '0') retval.Append(soundexChar);

                    previousChar = currentChar;
                }
            }
        }


        while (retval.Length < 4) retval.Append('0');

        return retval.ToString();
    }

    public static TextWriter WriteUnescapedHexString(StringReader reader, StringWriter writer)
    {
        var bytes = new byte[1];
        var chars = new char[1];

        var decoder = Encoding.UTF8.GetDecoder();

        while (true)
        {
            var x = reader.Read();

            if (x == -1) break;

            if (x != '%')
            {
                writer.Write((char)x);

                continue;
            }


            x = reader.Read();

            if (x == -1) break;

            if (!((char)x).IsHexDigit())
            {
                writer.Write('%');
                writer.Write((char)x);

                continue;
            }


            var y = reader.Read();

            if (y == -1) break;

            if (!((char)y).IsHexDigit())
            {
                writer.Write('%');
                writer.Write((char)x);
                writer.Write((char)y);
            }


            bytes[0] = (byte)(((char)x).FromHexNoCheck() * 0x10 + ((char)y).FromHexNoCheck());

            var charcount = decoder.GetChars(bytes, 0, 1, chars, 0);

            if (charcount > 0) writer.Write(chars[0]);
        }


        return writer;
    }

    public static string ToEscapedHexString(string s) => ToEscapedHexString(s, PredicateIsAsciiLetterOrDigit);

    public static string ToEscapedHexString(string s, string charsToEscape)
    {
        var builder = new StringBuilder((int)(s.Length * 1.5));

        foreach (var c in s)
            if (c == '%' || charsToEscape.Contains(c))
            {
                builder.Append('%');
                builder.Append(HexValues[(c & '\x00f0') >> 4]);
                builder.Append(HexValues[c & '\x000f']);
            }
            else
            {
                builder.Append(c);
            }


        return builder.ToString();
    }

    public static bool IsStandardUrlEscapedChar(char c) => !IsStandardUrlUnEscapedChar(c);

    public static bool IsStandardUrlUnEscapedChar(char c)
    {
        if (c is ' ' or '|' or '{' or '}' or '^' or '<' or '>' or '"' or '\\' or '%' or ';')
            return false;


        return c >= 21 && c <= 126;
    }

    public static string ToReEscapedHexString(string s, Predicate<char> shouldEscape)
    {
        var skip = 0;

        return ToEscapedHexString(
            s,
            context =>
            {
                if (skip > 0)
                {
                    skip--;

                    return false;
                }


                var c = context.Text[context.TextIndex];

                if (Uri.IsHexEncoding(context.Text, context.TextIndex))
                {
                    skip = 2;

                    return false;
                }


                return c == '%' || shouldEscape(c);
            }
        );
    }

    public static string ToEscapedHexString(string s, Predicate<(string Text, int TextIndex)> shouldEscapeChar)
    {
        var chars = new char[1];
        var encoding = Encoding.UTF8;
        var builder = new StringBuilder((int)(s.Length * 1.5));

        var buffer = new byte[encoding.GetMaxByteCount(1)];

        for (var i = 0; i < s.Length; i++)
            if (shouldEscapeChar((s, i)))
            {
                chars[0] = s[i];

                var bytecount = encoding.GetBytes(chars, 0, 1, buffer, 0);

                for (var j = 0; j < bytecount; j++)
                {
                    builder.Append('%');
                    builder.Append(HexValues[(buffer[j] & '\x00f0') >> 4]);
                    builder.Append(HexValues[buffer[j] & '\x000f']);
                }
            }
            else
            {
                builder.Append(s[i]);
            }


        return builder.ToString();
    }

    public static string ToEscapedHexString(string s, Predicate<char> shouldEscapeChar)
    {
        return ToEscapedHexString(
            s,
            stringInt => shouldEscapeChar(stringInt.Text[stringInt.TextIndex])
        );
    }

    // public static TextWriter WriteEscapedHexString(TextReader reader, TextWriter writer)
    // {
    // 	return WriteEscapedHexString
    // 	(
    // 		reader,
    // 		writer,
    // 		PredicateIsAsciiLetterOrDigit
    // 	);
    // }

    /*public static TextWriter WriteEscapedHexString(TextReader reader, TextWriter writer, Predicate<char> shouldEscapeChar)
    {
        var chars = new char[1];
        var encoding = Encoding.UTF8;
        
        var buffer = new byte[encoding.GetMaxByteCount(1)];

        return reader.ConvertAndDump(writer, (c, w) =>
        {
            if (c == '%' || shouldEscapeChar(c))
            {
                chars[0] = c;

                var bytecount = encoding.GetBytes(chars, 0, 1, buffer, 0);

                for (int j = 0; j < bytecount; j++)
                {
                    w.Write('%');
                    w.Write(HexValues[(buffer[j] & '\x00f0') >> 4]);
                    w.Write(HexValues[(buffer[j] & '\x00f0') >> 4]);
                }
            }
            else
            {
                w.Write(c);
            }
        });
    }*/

    public static bool IsHexChar(char c) => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9';

    public static int FromHex(char c)
    {
        return c switch
        {
            >= '0' and <= '9' => c - '0',
            >= 'A' and <= 'Z' => c - 'A' + 10,
            >= 'a' and <= 'z' => c - 'a' + 10,
            _ => throw new ArgumentException("Not a hex char", nameof(c))
        };
    }

    public static char ToHexChar(long x)
    {
        if (x is > int.MaxValue or < int.MinValue)
            throw new ArgumentException("Not a hex char", nameof(x));


        return ToHexChar((int)x, false);
    }

    public static char ToHexChar(short x) => ToHexChar(x, false);

    public static char ToHexChar(byte x) => ToHexChar(x, false);

    public static char ToHexChar(int x) => ToHexChar(x, false);

    public static char ToHexChar(int x, bool lowercase) => ToHexChar(x, lowercase ? HexValuesLowercase : HexValues);

    private static char ToHexChar(int x, char[] hexValues)
    {
        if (x is < 0 or > 16)
            throw new ArgumentException("Not a hex char", nameof(x));


        return hexValues[x];
    }

    public static byte[] FromHexString(string s)
    {
        var bytes = new byte[s.Length / 2];

        for (var i = 0; i < s.Length; i += 2)
        {
            int x, y;

            if (!IsHexChar(s[i]) || !IsHexChar(s[i + 1])) throw new ArgumentException("Contains invlaid hexadecimal values", nameof(s));

            try
            {
                x = FromHex(s[i]);
                y = FromHex(s[i + 1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                throw;
            }


            bytes[i / 2] = (byte)((x << 4) | y);
        }


        return bytes;
    }

    public static string ToHexString(long value)
    {
        var bytes = new byte[8];

        bytes[7] = (byte)((value >> 0) & 0xf);
        bytes[6] = (byte)((value >> 8) & 0xf);
        bytes[5] = (byte)((value >> 16) & 0xf);
        bytes[4] = (byte)((value >> 24) & 0xf);
        bytes[3] = (byte)((value >> 32) & 0xf);
        bytes[2] = (byte)((value >> 40) & 0xf);
        bytes[1] = (byte)((value >> 48) & 0xf);
        bytes[0] = (byte)((value >> 56) & 0xf);

        return ToHexString(bytes);
    }

    public static string ToHexString(int value)
    {
        var bytes = new byte[4];

        bytes[2] = (byte)((value >> 0) & 0xf);
        bytes[3] = (byte)((value >> 8) & 0xf);
        bytes[1] = (byte)((value >> 16) & 0xf);
        bytes[0] = (byte)((value >> 24) & 0xf);

        return ToHexString(bytes);
    }

    public static string ToHexString(byte[] bytes) => ToHexString(bytes, false);

    public static string ToHexString(byte[] bytes, bool lowercase) => ToHexString(bytes, lowercase ? HexValuesLowercase : HexValues);

    private static string ToHexString(byte[] bytes, char[] hexValues)
    {
        var builder = new StringBuilder(bytes.Length * 2);

        foreach (var b in bytes)
        {
            builder.Append(hexValues[b >> 4]);
            builder.Append(hexValues[b & 15]);
        }


        return builder.ToString();
    }

    public static string ToBase32String(long x)
    {
        var data = new byte[8];

        data[0] = (byte)(x & 0xff);
        data[1] = (byte)((x >> 8) & 0xff);
        data[2] = (byte)((x >> 16) & 0xff);
        data[3] = (byte)((x >> 24) & 0xff);
        data[4] = (byte)((x >> 32) & 0xff);
        data[5] = (byte)((x >> 40) & 0xff);
        data[6] = (byte)((x >> 48) & 0xff);
        data[7] = (byte)((x >> 56) & 0xff);

        return ToBase32String(data);
    }

    public static string ToBase32String(int x)
    {
        var outBlock = new char[8];

        var b0 = (byte)(x & 0xff);
        var b1 = (byte)((x & 0xff00) >> 8);
        var b2 = (byte)((x & 0xff0000) >> 16);
        var b3 = (byte)((x & 0xff000000) >> 24);
        var b4 = 0;

        outBlock[0] = Base32Alphabet[(b0 & 0xF8) >> 3];
        outBlock[1] = Base32Alphabet[((b0 & 0x7) << 2) | ((b1 & 0xc0) >> 6)];
        outBlock[2] = Base32Alphabet[(b1 & 0x3e) >> 1];
        outBlock[3] = Base32Alphabet[((b1 & 0x1) << 4) | ((b2 & 0xf0) >> 4)];
        outBlock[4] = Base32Alphabet[((b2 & 0xf) << 1) | ((b3 & 0x80) >> 7)];
        outBlock[5] = Base32Alphabet[(b3 & 0x7c) >> 2];
        outBlock[6] = Base32Alphabet[((b3 & 0x3) << 3) | ((b4 & 0xe0) >> 5)];
        outBlock[7] = Base32Alphabet[b4 & 0x1f];

        return new string(outBlock);
    }

    public static byte[] FromBase64CharArray(char[] array, int offset, int length) => Convert.FromBase64CharArray(array, offset, length);

    public static byte[] FromBase64String(string s) => Convert.FromBase64String(s);

    public static string ToBase64String(byte[] input) => Convert.ToBase64String(input, 0, input.Length);

    public static string ToBase64String(byte[] input, int offset, int length) => Convert.ToBase64String(input, offset, length);

    public static int ToBase64CharArray(byte[] input, int offsetIn, int length, char[] outArray, int offsetOut) =>
        Convert.ToBase64CharArray(input, offsetIn, length, outArray, offsetOut);

    public static int ToBase32CharArray(byte[] input, int offsetIn, int length, char[] outArray, int offsetOut)
    {
        var value = ToBase32String(input, offsetIn, length);

        value.CopyTo(0, outArray, offsetOut, value.Length);

        return value.Length;
    }

    public static string ToBase32String(byte[] input) => ToBase32String(input, 0, input.Length);

    public static string ToBase32String(byte[] input, int offset, int length)
    {
        var ex = -1;
        var outBlock = new char[8];
        var buffer = new StringBuilder(input.Length * 2);

        for (var i = offset; i < offset + input.Length; i += 5)
        {
            byte b4;
            byte b2;
            byte b0;
            byte b1;
            byte b3;

            if (i + 4 < input.Length)
            {
                b0 = input[i];
                b1 = input[i + 1];
                b2 = input[i + 2];
                b3 = input[i + 3];
                b4 = input[i + 4];

                outBlock[0] = Base32Alphabet[(b0 & 0xF8) >> 3];
                outBlock[1] = Base32Alphabet[((b0 & 0x7) << 2) | ((b1 & 0xc0) >> 6)];
                outBlock[2] = Base32Alphabet[(b1 & 0x3e) >> 1];
                outBlock[3] = Base32Alphabet[((b1 & 0x1) << 4) | ((b2 & 0xf0) >> 4)];
                outBlock[4] = Base32Alphabet[((b2 & 0xf) << 1) | ((b3 & 0x80) >> 7)];
                outBlock[5] = Base32Alphabet[(b3 & 0x7c) >> 2];
                outBlock[6] = Base32Alphabet[((b3 & 0x3) << 3) | ((b4 & 0xe0) >> 5)];
                outBlock[7] = Base32Alphabet[b4 & 0x1f];
            }
            else
            {
                b0 = b1 = b2 = b3 = b4 = 0;

                if (i < input.Length) b0 = input[i];

                if (i + 1 < input.Length) b1 = input[i + 1];

                if (i + 2 < input.Length) b2 = input[i + 2];

                if (i + 3 < input.Length) b3 = input[i + 3];

                if (i + 4 < input.Length) b4 = input[i + 4];

                outBlock[0] = Base32Alphabet[(b0 & 0xF8) >> 3];
                outBlock[1] = Base32Alphabet[((b0 & 0x7) << 2) | ((b1 & 0xc0) >> 6)];

                if (i + 1 >= input.Length)
                {
                    ex = 2;
                    goto end;
                }


                outBlock[2] = Base32Alphabet[(b1 & 0x3e) >> 1];
                outBlock[3] = Base32Alphabet[((b1 & 0x1) << 4) | ((b2 & 0xf0) >> 4)];

                if (i + 2 >= input.Length)
                {
                    ex = 4;
                    goto end;
                }


                outBlock[4] = Base32Alphabet[((b2 & 0xf) << 1) | ((b3 & 0x80) >> 7)];

                if (i + 3 >= input.Length)
                {
                    ex = 5;
                    goto end;
                }


                outBlock[5] = Base32Alphabet[(b3 & 0x7c) >> 2];
                outBlock[6] = Base32Alphabet[((b3 & 0x3) << 3) | ((b4 & 0xe0) >> 5)];

                if (i + 4 >= input.Length)
                {
                    ex = 7;
                    goto end;
                }


                outBlock[7] = Base32Alphabet[b4 & 0x1f];
            }


            end:

            if (ex > -1)
                for (var j = ex; j < outBlock.Length; j++)
                    outBlock[j] = '=';


            ex = -1;

            buffer.Append(outBlock);
        }


        return buffer.ToString();
    }
}