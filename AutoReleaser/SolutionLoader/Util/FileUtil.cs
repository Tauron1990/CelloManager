/*
 * Filename:    FileUtil.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: File utility methods.
 * Copyright:   Julijan Šribar, 2004-2013
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the author(s) be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoReleaser.SolutionLoader.Util
{
    /// <summary>
    ///     A set of file utilities.
    /// </summary>
    public struct FileUtil
    {
        /// <summary>
        ///     Reads text from a file.
        /// </summary>
        /// <param name="fileName">
        ///     Filename.
        /// </param>
        /// <param name="encoding">
        ///     <c>out</c> parameter with the encoding detected.
        /// </param>
        /// <returns>
        ///     Content of the file.
        /// </returns>
        public static string ReadTextFile(string fileName, out Encoding encoding)
        {
            Debug.Assert(fileName != null);
            Debug.Assert(File.Exists(fileName));
            encoding = Encoding.Default;
            using (var sr = new StreamReader(File.OpenRead(fileName), encoding, true))
            {
                var content = sr.ReadToEnd();
                encoding = sr.CurrentEncoding;
                return content;
            }
        }

        /// <summary>
        ///     Reads text from a file.
        /// </summary>
        /// <param name="fileName">
        ///     Filename.
        /// </param>
        /// <returns>
        ///     Content of the file.
        /// </returns>
        public static string ReadTextFile(string fileName)
        {
            Debug.Assert(fileName != null);
            Debug.Assert(File.Exists(fileName));
            using (var sr = new StreamReader(File.OpenRead(fileName), true))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        ///     Saves the text to a file.
        /// </summary>
        /// <param name="fileName">
        ///     Name of the file to save text to.
        /// </param>
        /// <param name="content">
        ///     Text to save.
        /// </param>
        /// <param name="encoding">
        ///     Encoding used to save the file.
        /// </param>
        public static void SaveTextFile(string fileName, string content, Encoding encoding)
        {
            Debug.Assert(fileName != null);
            var fileAttribs = File.GetAttributes(fileName);
            if ((fileAttribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                fileAttribs ^= FileAttributes.ReadOnly;
                File.SetAttributes(fileName, fileAttribs);
            }
            using (var sw = new StreamWriter(fileName, false, encoding))
            {
                sw.Write(content);
            }
        }

        /// <summary>
        ///     Gets last write time for a file.
        /// </summary>
        /// <param name="fileName">
        ///     Filename for which operation is requested.
        /// </param>
        /// <returns>
        ///     <c>DateTime</c> structure.
        /// </returns>
        public static DateTime GetLastWriteTime(string fileName)
        {
            Debug.Assert(fileName != null);
            Debug.Assert(File.Exists(fileName));
            var fileInfo = new FileInfo(fileName);
            return fileInfo.LastWriteTime;
        }

        /// <summary>
        ///     Checks if name matches pattern with '?' and '*' wildcards.
        /// </summary>
        /// <param name="filename">
        ///     Name to match.
        /// </param>
        /// <param name="pattern">
        ///     Pattern to match to.
        /// </param>
        /// <returns>
        ///     <c>true</c> if name matches pattern, otherwise <c>false</c>.
        /// </returns>
        public static bool FilenameMatchesPattern(string filename, string pattern)
        {
            // prepare the pattern to the form appropriate for Regex class
            var sb = new StringBuilder(pattern);
            // remove superflous occurences of  "?*" and "*?"
            while (sb.ToString().IndexOf("?*", StringComparison.Ordinal) != -1) sb.Replace("?*", "*");
            while (sb.ToString().IndexOf("*?", StringComparison.Ordinal) != -1) sb.Replace("*?", "*");
            // remove superflous occurences of asterisk '*'
            while (sb.ToString().IndexOf("**", StringComparison.Ordinal) != -1) sb.Replace("**", "*");
            // if only asterisk '*' is left, the mask is ".*"
            if (sb.ToString().Equals("*"))
            {
                pattern = ".*";
            }
            else
            {
                // replace '.' with "\."
                sb.Replace(".", "\\.");
                // replaces all occurrences of '*' with ".*" 
                sb.Replace("*", ".*");
                // replaces all occurrences of '?' with '.*' 
                sb.Replace("?", ".");
                // add "\b" to the beginning and end of the pattern
                sb.Insert(0, "\\b");
                sb.Append("\\b");
                pattern = sb.ToString();
            }
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(filename);
        }

        /// <summary>
        ///     Compares two rooted paths for equality. Any of the paths provided
        ///     may end with path separator - it will be ignored.
        /// </summary>
        /// <param name="absolutePath1">
        ///     First path.
        /// </param>
        /// <param name="absolutePath2">
        ///     Second path.
        /// </param>
        /// <returns>
        ///     <c>true</c> if both paths correspond to the same folder.
        /// </returns>
        public static bool PathsAreEqual(string absolutePath1, string absolutePath2)
        {
            Debug.Assert(absolutePath1 != null && Path.IsPathRooted(absolutePath1));
            Debug.Assert(absolutePath2 != null && Path.IsPathRooted(absolutePath2));
            // remove any trailing separators
            var endingChar = absolutePath1[absolutePath1.Length - 1];
            if (endingChar == Path.DirectorySeparatorChar || endingChar == Path.AltDirectorySeparatorChar)
                absolutePath1 = Path.GetDirectoryName(absolutePath1);
            else
                absolutePath1 = Path.GetFullPath(absolutePath1);
            endingChar = absolutePath2[absolutePath2.Length - 1];
            if (endingChar == Path.DirectorySeparatorChar || endingChar == Path.AltDirectorySeparatorChar)
                absolutePath2 = Path.GetDirectoryName(absolutePath2);
            else
                absolutePath2 = Path.GetFullPath(absolutePath2);
            return string.Compare(absolutePath1, absolutePath2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        ///     Combines two paths, removing root-folder combinations.
        /// </summary>
        /// <param name="rootPath">
        /// </param>
        /// <param name="relativePath">
        /// </param>
        /// <returns></returns>
        public static string CombinePaths(string rootPath, string relativePath)
        {
            // remove any trailing path separators
            if (Path.GetPathRoot(rootPath) != rootPath)
                rootPath = rootPath.Trim(DirectorySeparators);
            if (rootPath == null) return null;
            var lastSeparator = rootPath.Length;
            var relativePathComponents = relativePath.Split(DirectorySeparators);
            int i;
            for (i = 0; i < relativePathComponents.Length; i++)
                if (relativePathComponents[i].Equals(".."))
                {
                    lastSeparator = rootPath.LastIndexOfAny(DirectorySeparators, lastSeparator - 1);
                    Debug.Assert(lastSeparator > 0);
                    rootPath = rootPath.Substring(0, lastSeparator + 1);
                }
                else if (!relativePathComponents[i].Equals("."))
                {
                    break;
                }
            Debug.Assert(i < relativePathComponents.Length);
            for (var j = i; j < relativePathComponents.Length; j++) rootPath = Path.Combine(rootPath, relativePathComponents[j]);
            return rootPath;
        }

        /// <summary>
        ///     Creates foldername (in user's ApplicationData) where configuration
        ///     is stored.
        /// </summary>
        /// <returns>
        ///     Path to the folder with configuration file.
        /// </returns>
        public static string GetConfigurationFolder()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appName = "BuildAutoIncrement";
            return Path.Combine(appDataPath, appName);
        }

        /// <summary>
        ///     Directory separator characters.
        /// </summary>
        private static readonly char[] DirectorySeparators = {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
    }
}