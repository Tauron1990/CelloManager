/*
 * Filename:    UnknownProjectTypeException.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     CommandLine
 * Description: Exception thrown in the case of unknown project type.
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

namespace AutoReleaser.SolutionLoader.SolutionFileReaders
{
    /// <summary>
    ///     Exception thrown when not supported project type is encountered.
    /// </summary>
    public class UnknownProjectTypeException : Exception
    {
        public readonly string Clsid;

        public readonly string ProjectName;

        /// <summary>
        ///     Creates <c>NotSupportedProjectTypeException</c> object.
        /// </summary>
        /// <param name="projectName">
        ///     Name of the project.
        /// </param>
        /// <param name="clsid">
        ///     clsid for the project type.
        /// </param>
        public UnknownProjectTypeException(string projectName, string clsid)
        {
            ProjectName = projectName;
            Clsid = clsid;
        }
    }
}