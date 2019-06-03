using System;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Helpers;

namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    /// <summary>
    ///     A set of extension methods for <see cref="ProjectItem" />.
    /// </summary>
    internal static class ProjectItemExtensions
    {
        /// <summary>
        ///     Attempts to retrieve the file name for the specified project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>The filename of the project item if available, otherwise null.</returns>
        internal static async Task<string> GetFileNameAsync(this ProjectItem projectItem)
        {
            try
            {
                return projectItem.FileNames[1];
            }
            catch (Exception ex)
            {
                await OutputWindowHelper.DiagnosticWriteLineAsync("Unable to retrieve ProjectItem file name", ex);
                return null;
            }
        }

        /// <summary>
        ///     Attempts to retrieve the parent project item for the specified project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>The parent project item, otherwise null.</returns>
        internal static async Task<ProjectItem> GetParentProjectItemAsync(this ProjectItem projectItem)
        {
            try
            {
                var parentProjectItem = projectItem.Collection?.Parent as ProjectItem;
                return parentProjectItem;
            }
            catch (Exception ex)
            {
                await OutputWindowHelper.DiagnosticWriteLineAsync("Unable to retrieve parent ProjectItem", ex);
                return null;
            }
        }

        /// <summary>
        ///     Determines if the specified project item is external to the solution.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>True if the project item is external, otherwise false.</returns>
        internal static async Task<bool> IsExternalAsync(this ProjectItem projectItem)
        {
            try
            {
                if (projectItem.Collection == null || !await projectItem.IsPhysicalFileAsync())
                    return true;

                return projectItem.Collection.OfType<ProjectItem>().All(x => x.Object != projectItem.Object);
            }
            catch (Exception ex)
            {
                await OutputWindowHelper.DiagnosticWriteLineAsync("Unable to determine if ProjectItem is external", ex);
                return false;
            }
        }

        /// <summary>
        ///     Determines if the specified project item is a physical file.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>True if the project item is a physical file, otherwise false.</returns>
        internal static async Task<bool> IsPhysicalFileAsync(this ProjectItem projectItem)
        {
            try
            {
                return string.Equals(projectItem.Kind, Constants.vsProjectItemKindPhysicalFile, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                // Some ProjectItem types (e.g. WiX) may throw an error when accessing the Kind member.
                await OutputWindowHelper.DiagnosticWriteLineAsync("Unable to determine if ProjectItem is a physical file", ex);
                return false;
            }
        }
    }
}