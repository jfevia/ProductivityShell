using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ConfigurationChangedEventArgs : SolutionEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="profiles">The profiles.</param>
        /// <param name="selectedProfile">The selected profile.</param>
        public ConfigurationChangedEventArgs(Configuration.Configuration configuration, ICollection<Profile> profiles, Profile selectedProfile)
            : base(configuration, profiles, selectedProfile)
        {
        }
    }
}