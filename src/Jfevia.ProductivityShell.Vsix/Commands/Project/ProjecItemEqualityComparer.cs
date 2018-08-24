using System;
using System.Collections.Generic;

namespace Jfevia.ProductivityShell.Vsix.Commands.Project
{
    internal class ProjecItemEqualityComparer : IEqualityComparer<EnvDTE.ProjectItem>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(EnvDTE.ProjectItem x, EnvDTE.ProjectItem y)
        {
            return string.Equals(x.Properties.Item("FullPath")?.ToString(), y.Properties.Item("FullPath")?.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(EnvDTE.ProjectItem projectItem)
        {
            return projectItem.Properties.Item("FullPath").GetHashCode();
        }
    }
}