using System.Collections.Specialized;
using System.Text;
using JetBrains.Annotations;
// ReSharper disable CognitiveComplexity

namespace Game.Engine.Core;

	/// <summary>
	/// Provides useful methods for dealing with string based URIs.
	/// </summary>
	[PublicAPI]
	public sealed class StringUriUtils
	{
		/// <summary>
		/// Array of standard <c>acceptable</c> seperator chars for use when normalizing a path.
		/// </summary>
		/// <remarks>
		/// This array holds the chars <c>'/'</c> and <c>'\'</c>.
		/// </remarks>
		public static readonly char[] AcceptableSeperatorChars = new char[] { '/', '\\' };

		// /// <summary>
		// /// Returns the name part of a URL
		// /// </summary>
		// /// <param name="uri"></param>
		// /// <returns></returns>
		// public static string GetName(string uri)
		// {
		// 	return uri.SplitAroundCharFromRight(PredicateUtils.ObjectEqualsAny<char>('/', '\\')).Right;
		// }

		public static (string Scheme, string Uri) GetSchemeAndPath(string uri)
		{
			var x = uri.IndexOf("://", StringComparison.Ordinal);

			return x < 0 ? ("", uri) : (uri[..x], uri[(x + 1)..]);
		}

		public static string GetPath(string uri)
		{
			var x = uri.IndexOf("://", StringComparison.Ordinal);

			return x < 0 ? uri : uri[(x + 3)..];
		}

		public static string GetScheme(string uri)
		{
			var x = uri.IndexOf("://", StringComparison.Ordinal);

			return x < 0 ? "" : uri[..x];
		}

		/// <summary>
		/// <see cref="NormalizePath(string, int, int)"/>
		/// </summary>
		/// <remarks>
		/// Calls <see cref="NormalizePath(string, int, int)"/> with the array
		/// <see cref="AcceptableSeperatorChars"/> which contains <c>'/'</c> and <c>'\'</c>.
		/// </remarks>
		public static string NormalizePath(string path) => NormalizePath(path, 0, path.Length, AcceptableSeperatorChars, false);

		public static string NormalizePath(string path, int startIndex, int count) => NormalizePath(path, startIndex, count, AcceptableSeperatorChars, false);

		private static bool CharArrayContains(char[] array, char c) => array.Any(t => t == c);

		[ThreadStatic]
		private static int[]? _backtrackStack;

		/// <summary>
		/// Normalises a given path and returns the new normalised version.
		/// </summary>
		/// <remarks>
		/// <p>
		/// The normalization process consists of the following:
		/// 
		/// Path elements consisting of '.' are removed.
		/// Path elements before path elements consisting of '..' are removed.
		/// The given chars are replaced with the standard URI seperator char '/'.
		/// </p>
		/// <p>
		/// Paths will always be returned without the trailing seperator char '/'.
		/// </p>
		/// </remarks>
		/// <param name="path">
		/// The path to normalise.
		/// </param>
		/// <returns>
		/// A normalised version of the path.
		/// </returns>
		public static string NormalizePath(string path, int startIndex, int count, char[] seperatorChars, bool preserveEndingSeperator)
		{
			if (count == 0)
				return "";

			var xi = 0;
			var x = startIndex;

			if (_backtrackStack == null)
				lock (typeof(StringUriUtils))
					_backtrackStack ??= new int[100];

			var localBackTrackStack = _backtrackStack;

			localBackTrackStack[xi] = startIndex;

			var startsWithSeperator = CharArrayContains(seperatorChars, path[startIndex]);
			var endsWithSeperator = CharArrayContains(seperatorChars, path[^1]);

			if (startsWithSeperator)
			{
				if (count == 1)
					return "/";
				x = startIndex + 1;

				localBackTrackStack[xi] = startIndex;
			}

			xi++;
			
			var builder = new StringBuilder(path.Length);
			
			while (x < (count + startIndex))
			{
				var y = x;

				while (y < (count + startIndex))
				{
					if (CharArrayContains(seperatorChars, path[y]))
						break;

					y++;
				}

				var z = y - x;

				switch (z)
				{
					case 0:
						// Ignore '//'.
						break;
					case 1 when path[x] == '.':
						// Ignore '/./'
						break;
					case 2 when path[x] == '.' && path[x + 1] == '.':
					{
						if (x < 2)
							throw new ArgumentException("Root (/) has no parent.", path);

						if (builder.Length == 0)
							throw new InvalidOperationException("Impossible attempt to go above path root (/).");

						xi--;
						builder.Remove(_backtrackStack[xi], builder.Length - _backtrackStack[xi]);
						break;
					}
					default:
					{
						localBackTrackStack[xi++] = builder.Length;

						if (x == 0)
						{
							if (startsWithSeperator) builder.Append('/');
						}
						else
							builder.Append('/');

						builder.Append(path, x, z);
						break;
					}
				}

				x = y + 1;
			}
			
			if (builder.Length == 0)
				return startsWithSeperator ? "/" : ".";

			if (endsWithSeperator && preserveEndingSeperator) builder.Append('/');

			return builder.ToString();
		}

		public static string Combine(string left, string right)
		{
			if (left.EndsWith("/"))
			{
				if (right.StartsWith("/"))
					return left[..^1] + right;

				return left + right;
			}

			return right.StartsWith("/") ? left + right : left + "/" + right;
		}

		public static string Unescape(string s)
		{
			var builder = new StringBuilder(s.Length + 10);

			for (var i = 0; i < s.Length; ) builder.Append(Uri.HexUnescape(s, ref i));

			return builder.ToString();
		}

		public static string Escape(string s, Predicate<char> includeChar)
		{
		    var writer = new StringWriter();
			var reader = new StringReader(s);

			var charValue = reader.Read();

			while (charValue != -1)
			{
				if (charValue is >= 48 and <= 57 or >= 65 and <= 90 or >= 97 and <= 122) // a-z
					writer.Write((char)charValue);
				else
					writer.Write("%{0:x2}", charValue);

				charValue = reader.Read();
			}

			return writer.ToString();
		}

		public static string UrlEncode(string instring) => TextConversion.ToEscapedHexString(instring, TextConversion.IsStandardUrlEscapedChar);

		public static string BuildQuery(NameValueCollection nameValueCollection) 
			=> BuildQuery(nameValueCollection.EnumerateEntrys(), PredicateUtils<KeyValuePair<string, string>>.AlwaysTrue);

		public static string BuildQuery(NameValueCollection nameValueCollection, Predicate<KeyValuePair<string, string>> acceptPair) 
			=> BuildQuery(nameValueCollection.EnumerateEntrys(), acceptPair);

		public static string BuildQuery(IEnumerable<KeyValuePair<string, string>> pairs) => BuildQuery(pairs, PredicateUtils<KeyValuePair<string, string>>.AlwaysTrue);

		public static string BuildQuery(IEnumerable<(string, string)> pairs) => BuildQuery(pairs.Select(t => KeyValuePair.Create(t.Item1, t.Item2)), PredicateUtils<KeyValuePair<string, string>>.AlwaysTrue);

		public static string BuildQuery(IEnumerable<KeyValuePair<string, string>> pairs, Predicate<KeyValuePair<string, string>> acceptPair)
		{
		    var builder = new StringBuilder();

			foreach (var pair in pairs)
			{
				if (acceptPair(pair))
				{
					builder.Append(UrlEncode(pair.Key));
					builder.Append('=');
					builder.Append(UrlEncode(pair.Value));
				}
			}

			return builder.ToString();
		}

		public static IEnumerable<KeyValuePair<string, string>> ParseQuery(string query)
		{
			if (query == "")
				yield break;

			var ss = query.Split('&');

			foreach (var t in ss)
			{
				string key, value;

				var j = t.IndexOf('=');

				if (j < 0)
				{
					key = t;
					value="";
				}
				else
				{
					key = t[..j];
					value = t[(j + 1)..];
				}

				yield return KeyValuePair.Create(key, value);
			}
		}

		public static bool ContainsQuery(string uri) => uri.LastIndexOf('?') >= 0;

		public static string RemoveQuery(string uri)
		{
			return uri.Left(PredicateUtils.ObjectEquals<char>('?').Not());
		}

		public static string AppendQueryPart(string uri, string key, string value)
		{
			if (uri.LastIndexOf('?') > 0)
			{
				uri += "&";
			}
			else
			{
				uri += '?';
			}
			
			uri += UrlEncode(key) + "=" + UrlEncode(value);

			return uri;
		}
	}