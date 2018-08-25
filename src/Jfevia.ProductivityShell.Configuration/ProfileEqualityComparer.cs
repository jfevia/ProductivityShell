using System.Collections.Generic;
using System.Linq;

namespace Jfevia.ProductivityShell.Configuration
{
    public class ProfileEqualityComparer : IEqualityComparer<Profile>
    {
        private static ProfileEqualityComparer _instance;

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static ProfileEqualityComparer Instance => _instance ?? (_instance = new ProfileEqualityComparer());

        /// <inheritdoc />
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(Profile x, Profile y)
        {
            if (x == null || y == null)
                return false;

            if (x.DisplayName != y.DisplayName)
                return false;

            foreach (var project in x.Projects)
                if (!y.Projects.Any(projectY => ProjectEqualityComparer.Instance.Equals(project, projectY)))
                    return false;

            return true;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(Profile obj)
        {
            var hashCode = -738036739;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.DisplayName);

            foreach (var project in obj.Projects)
                hashCode = hashCode * -1521134295 + ProjectEqualityComparer.Instance.GetHashCode(project);

            return hashCode;
        }
    }
}