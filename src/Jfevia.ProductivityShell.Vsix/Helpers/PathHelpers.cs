using System;
using System.IO;

namespace Jfevia.ProductivityShell.Vsix.Helpers
{
    internal class PathHelpers
    {
        /// <summary>
        ///     Gets the destination path relative to the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The destination path relative to the source.</returns>
        public static string GetPathRelativeTo(string source, string destination)
        {
            var pathUri = new Uri(source);

            // Folders must end in a slash
            if (!destination.EndsWith(Path.DirectorySeparatorChar.ToString()))
                destination += Path.DirectorySeparatorChar;

            var folderUri = new Uri(destination);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}