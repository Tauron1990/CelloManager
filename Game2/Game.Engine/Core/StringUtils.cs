using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace Game.Engine.Core;

	/// <summary>
	/// Provides extension methods and static utility methods for strings.
	/// </summary>
	[PublicAPI]
	public static class StringUtils
	{
		public static KeyValuePair<TKey, TValue> ToPair<TKey, TValue>((TKey, TValue) pair)
			=> KeyValuePair.Create(pair.Item1, pair.Item2);

		public static Func<string, Func<string>> NewAdder(char appendChar) => NewAdder(appendChar, true);

		public static Func<string, Func<string>> NewAdder(char appendChar, bool appendAtEnd)
		{
			Func<string> retval;
			var builder = new StringBuilder();

			if (appendAtEnd)
				retval = builder.ToString;
			else
			{
				retval = () =>
				{
					if (builder.Length > 0) builder.Length--;

					return builder.ToString();
				};
			}

			return value =>
			{
				builder.Append(value).Append(appendChar);

				return retval;
			};
		}

		/// <summary>
		/// Returns a new string that is made up of part of the current string.
		/// This method will return an empty or smaller than expected string if 
		/// the index or lengths overflow.
		/// </summary>
		/// <param name="s">The current string</param>
		/// <param name="index">The index of the first character to return</param>
		/// <param name="length">The length of the string to return</param>
		/// <returns>
		/// A new string
		/// </returns>
		public static string MidString(this string s, int index, int length)
		{
			if (length < 0) length = 0;

			if (index < 0)
				index = 0;
			else if (index >= s.Length)
				return string.Empty;

			var y = index + length;

			if (y >= s.Length) length = Math.Max(length - (y - s.Length), 0);

			return s.Substring(index, length);
		}

		public static (string Left, string Right) SplitOnFirst(this string s, string searchString)
		{
			var x = s.IndexOf(searchString, StringComparison.Ordinal);

			return x < 0 ? (s, string.Empty) : (s[..x], s[(x + searchString.Length)..]);
		}

		public static (string Left, string Right) SplitOnLast(this string s, string searchString)
		{
			var x = s.LastIndexOf(searchString, StringComparison.Ordinal);

			return x < 0 ? (string.Empty, s) : (s[..x], s[(x + searchString.Length)..]);
		}

		public static (string Left, string Right) SplitAroundFirstCharFromLeft(this string s, char c) => s.SplitAroundCharFromLeft(PredicateUtils.ObjectEquals(c));

		public static (string Left, string Right) SplitAroundFirstCharFromRight(this string s, char c) => s.SplitAroundCharFromRight(PredicateUtils.ObjectEquals(c));

		public static (string Left, string Right) SplitAroundCharFromLeft(this string s, char c) => s.SplitAroundCharFromLeft(PredicateUtils.ObjectEquals(c));

		public static (string Left, string Right) SplitAroundCharFromLeft(this string s, Predicate<char> isSplitChar) => s.SplitAroundCharFromLeft(0, isSplitChar);

		public static (string Left, string Right) SplitAroundCharFromLeft(this string s, int startIndex, Predicate<char> isSplitChar)
		{
			for (var i = startIndex; i < s.Length; i++)
				if (isSplitChar(s[i]))
					return (s.Left(i), Right(s, s.Length - i - 1));

			return (s, string.Empty);
		}

		public static (string Left, string Right) SplitAroundCharFromRight(this string s, char c) => s.SplitAroundCharFromRight(PredicateUtils.ObjectEquals(c));

		public static (string Left, string Right) SplitAroundCharFromRight(this string s, Predicate<char> isSplitChar)
		{
			for (var i = s.Length - 1; i >= 0; i--)
				if (isSplitChar(s[i]))
					return (s.Left(i), Right(s, s.Length - i - 1));

			return (string.Empty, s);
		}

		public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
		
		public static string Capitalize(this string s)
		{
			if (s.Length == 0)
			{
				return s;
			}

			if (char.IsUpper(s[0]))
				return s;
			
			var builder = new StringBuilder(s.Length);

			builder.Append(char.ToUpper(s[0]));
			builder.Append(s, 1, s.Length - 1);

			return builder.ToString();
		}

		public static string Uncapitalize(this string s)
		{
			if (s.Length == 0)
				return s;

			if (char.IsLower(s[0]))
				return s;
			
			var builder = new StringBuilder(s.Length);

			builder.Append(char.ToLower(s[0]));
			builder.Append(s, 1, s.Length - 1);

			return builder.ToString();
		}

		public static bool EqualsIgnoreCase(this string s1, string s2) => s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);

		public static bool EqualsIgnoreCaseInvariant(this string s1, string s2) => s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);

		/// <summary>
		/// Gets the string made of all the characters on the left of the string while the
		/// predicate is satisfied.
		/// </summary>
		/// <param name="s">The string to operate on</param>
		/// <param name="acceptChar">A predicate that takes a char and returns false when left should return</param>
		/// <returns>A new string</returns>
		public static string Left(this string s, Predicate<char> acceptChar)
		{
			int i;

			for (i = 0; i < s.Length; i++)
			{
				if (acceptChar(s[i]))
					continue;

				break;
			}

			return i >= s.Length ? s : s[..i];
		}

		/// <summary>
		/// Gets the string made up of all the characters on the right of all the characters
		/// on the left that match the predicate
		/// </summary>
		public static string RightFromLeft(this string s, Predicate<char> predicate)
		{
			int i;

			for (i = 0; i < s.Length; i++)
			{
				if (predicate(s[i]))
					continue;
				break;
			}

			return i >= s.Length ? string.Empty : s[(i + 1)..];
		}

		/// <summary>
		/// Gets the string that is made up of the right most characters of <c>s</c>
		/// that satisfy <c>predicate</c>.
		/// </summary>
		/// <remarks>
		/// Gets the string that is made up of the right most characters of <c>s</c>
		/// that satisfy <c>predicate</c>.  The method terminates and returns as soon
		/// as a character that doesn't satisfy <c>predicate</c> is found.
		/// <p>
		/// If every character satisfies the predicate then <c>s</c> is returned.
		/// </p>
		/// </remarks>
		/// <param name="s"></param>
		/// <param name="acceptChar"></param>
		/// <returns></returns>
		public static string Right(this string s, Predicate<char> acceptChar)
		{
			int i;

			for (i = s.Length - 1; i >= 0; i--)
			{
				if (acceptChar(s[i]))
					continue;

				break;
			}

			return i  < 0 ? s : s[(i + 1)..];
		}

		public static string LeftFromRight(this string s, Predicate<char> accept)
		{
			int i;

			for (i = s.Length - 1; i >= 0; i--)
			{
				if (accept(s[i]))
					continue;

				break;
			}

			return i  < 0 ? string.Empty : s[..(i + 1)];
		}

		public static string Left(this string s, int count) => count >= s.Length ? s : s[..count];

		public static string Right(this string s, int count)
		{
			if (count >= s.Length)
				return s;

			if (count < 0)
				return string.Empty;

			return s.Length - count <= 0 ? string.Empty : s.Substring(s.Length - count, count);
		}

		public static int CountChars(this string s, Predicate<char> predicate) => CountChars(s, predicate, 0, s.Length);

		public static int CountChars(this string s, Predicate<char> acceptChar, int startIndex, int count)
		{
			var retval = 0;

			for (var i = startIndex; i < count; i++)
			{
				if (acceptChar(s[i])) retval++;
			}

			return retval;
		}

		public static string TrimLeft(this string s) => s.TrimLeft(' ');

		public static string TrimRight(this string s) => s.TrimRight(' ');

		public static string Trim(this string s) => Trim(s, ' ');

		public static string TrimLeft(this string s, char c)
		{
			int i;
			
			for (i = 0; i < s.Length; i++)
			{
				if (s[i] != c)
					break;
			}

			return s[i..];
		}

		public static string TrimRight(this string s, char c)
		{
			int i;
			
			for (i = s.Length - 1; i >= 0; i--)
			{
				if (s[i] != c)
					break;
			}

			return s.Substring(0, i + 1);
		}

		public static string Trim(this string s, char c)
		{
			int x, y;
			
			for (x = 0; x < s.Length; x++)
			{
				if (s[x] != c)
					break;
			}

			for (y = s.Length - 1; y > x; y--)
			{
				if (s[y] != c)
					break;
			}

			return s.Substring(x, y + 1 - x);
		}

		public static string TrimLeft(this string s, Predicate<char> trimChar)
		{
			int i;
			
			for (i = 0; i < s.Length; i++)
			{
				if (!trimChar(s[i]))
					break;
			}

			return s.Substring(i);
		}

		public static string TrimRight(this string s, Predicate<char> trimChar)
		{
			int i;
			
			for (i = s.Length - 1; i >= 0; i--)
			{
				if (!trimChar(s[i]))
					break;
			}

			return s[..(i + 1)];
		}

		public static string Trim(this string s, Predicate<char> trimChar)
		{
			int x, y;
			
			for (x = 0; x < s.Length; x++)
			{
				if (!trimChar(s[x]))
					break;
			}

			for (y = s.Length - 1; y > x; y--)
			{
				if (!trimChar(s[y]))
					break;
			}

			return s.Substring(x, y + 1 - x);
		}

		public static string TrimLeft(this string s, string match) => s.StartsWith(match) ? s.Substring(match.Length) : s;

		public static string TrimRight(this string s, string match) => s.EndsWith(match) ? s.Substring(0, s.Length - match.Length) : s;

		/// <summary>
		/// Returns the index of the first character that satifies the predicate.
		/// </summary>
		/// <param name="s">The string to search</param>
		/// <param name="acceptChar">The predicate</param>
		/// <returns>The index of the first character if found otherwise -1</returns>
		public static int IndexOf(this string s, Predicate<char> acceptChar) => s.IndexOf(0, acceptChar);

		/// <summary>
		/// Returns the index of the first character that satisfies the given predicate.
		/// </summary>
		/// <param name="s">The string to search.</param>
		/// <param name="startIndex">The index to start search at</param>
		/// <param name="acceptChar">
		/// The predicate that every character is asserted against.
		/// </param>
		/// <returns>
		/// The index of the first character that satisfies <c>predicate</c> or -1 if no
		/// characters satisfy <c>predicate</c>.
		/// </returns>
		public static int IndexOf(this string s, int startIndex, Predicate<char> acceptChar)
		{
			for (var i = startIndex; i < s.Length; i++)
			{
				if (acceptChar(s[i]))
					return i;
			}

			return -1;
		}

		/// <summary>
		/// Returns the index of the last character that satisfies the given predicate.
		/// </summary>
		/// <param name="s">The string to search.</param>
		/// <param name="acceptChar">
		/// The predicate that every character is asserted against.
		/// </param>
		/// <returns>
		/// The index of the last character that satisfies <c>predicate</c> or -1 if no
		/// characters satisfy <c>predicate</c>.
		/// </returns>
		public static int LastIndexOf(this string s, Predicate<char> acceptChar)
		{
			for (var i = s.Length - 1; i >= 0; i--)
			{
				if (acceptChar(s[i]))
					return i;
			}

			return -1;
		}

		public static string LongHead(this string s) => s.Length <= 1 ? s : s[..^1];

		public static string ShortTail(this string s) => s.Length == 0 ? s : s[^1].ToString(CultureInfo.InvariantCulture);

		public static string Head(this string s) => s.Length == 0 ? s : s[0].ToString(CultureInfo.InvariantCulture);

		public static string Tail(this string s) => s.Length <= 1 ? s : s[1..];

		public static bool IsNumeric(this string s)
		{
			var i = 0;

			if (s[i] == '-') i++;

			for (; i < s.Length; i++)
			{
				if (!char.IsNumber(s[i]))
					return false;
			}

			return true;
		}

		public static bool EndsWith(this string s, char value)
		{
			if (s.Length == 0)
				return false;

			return s[^1] == value;
		}

		public static bool StartsWith(this string s, char value)
		{
			if (s.Length == 0)
				return false;

			return s[0] == value;
		}
	}