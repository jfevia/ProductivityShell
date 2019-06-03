﻿using System.Threading.Tasks;
using EnvDTE;

namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    /// <summary>
    ///     A set of extension methods for <see cref="Document" />.
    /// </summary>
    internal static class DocumentExtensions
    {
        /// <summary>
        ///     Attempts to get the TextDocument associated with the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The associated text document, otherwise null.</returns>
        internal static TextDocument GetTextDocument(this Document document)
        {
            return document.Object("TextDocument") as TextDocument;
        }

        /// <summary>
        ///     Determines if the specified document is external to the solution.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>True if the document is external, otherwise false.</returns>
        internal static async Task<bool> IsExternalAsync(this Document document)
        {
            var projectItem = document.ProjectItem;

            return projectItem == null || await projectItem.IsExternalAsync();
        }
    }
}