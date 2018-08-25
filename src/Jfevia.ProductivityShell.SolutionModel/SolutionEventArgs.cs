using System;
using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class SolutionEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.SolutionEventArgs" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="profiles">The profiles.</param>
        /// <param name="selectedProfile">The selected profile.</param>
        public SolutionEventArgs(Configuration.Configuration configuration, ICollection<Profile> profiles, Profile selectedProfile)
        {
            Configuration = configuration;
            Profiles = profiles;
            SelectedProfile = selectedProfile;
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        public Configuration.Configuration Configuration { get; }

        /// <summary>
        ///     Gets the profiles.
        /// </summary>
        /// <value>
        ///     The profiles.
        /// </value>
        public ICollection<Profile> Profiles { get; }

        /// <summary>
        ///     Gets the selected profile.
        /// </summary>
        /// <value>
        ///     The selected profile.
        /// </value>
        public Profile SelectedProfile { get; }
    }
}