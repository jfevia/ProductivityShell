using System;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.Vsix.Solutions
{
    internal class StartupProjectsChangedEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Solutions.StartupProjectsChangedEventArgs" /> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        public StartupProjectsChangedEventArgs(Profile profile)
        {
            Profile = profile;
        }

        /// <summary>
        ///     Gets the profile.
        /// </summary>
        /// <value>
        ///     The profile.
        /// </value>
        public Profile Profile { get; }
    }
}