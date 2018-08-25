using System.Collections.Generic;

namespace Jfevia.ProductivityShell.Configuration
{
    public class ProjectEqualityComparer : IEqualityComparer<Project>
    {
        private static ProjectEqualityComparer _instance;

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static ProjectEqualityComparer Instance => _instance ?? (_instance = new ProjectEqualityComparer());

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(Project x, Project y)
        {
            if (x == null || y == null)
                return false;

            return x.Name == y.Name &&
                   x.CommandLineArgs == y.CommandLineArgs &&
                   x.WorkingDirectory == y.WorkingDirectory &&
                   x.StartExternalProgram == y.StartExternalProgram &&
                   x.StartBrowserUrl == y.StartBrowserUrl &&
                   x.IsRemoteDebuggingEnabled == y.IsRemoteDebuggingEnabled &&
                   x.RemoteDebuggingMachine == y.RemoteDebuggingMachine;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(Project obj)
        {
            var hashCode = -1570574728;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.CommandLineArgs);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.WorkingDirectory);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.StartExternalProgram);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.StartBrowserUrl);
            hashCode = hashCode * -1521134295 + obj.IsRemoteDebuggingEnabled.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.RemoteDebuggingMachine);
            return hashCode;
        }
    }
}