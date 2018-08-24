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
        /// <param name="projectConfiguration">The project configuration.</param>
        public StartupProjectsChangedEventArgs(ProjectConfiguration projectConfiguration)
        {
            ProjectConfiguration = projectConfiguration;
        }

        /// <summary>
        ///     Gets the project configuration.
        /// </summary>
        /// <value>
        ///     The project configuration.
        /// </value>
        public ProjectConfiguration ProjectConfiguration { get; }
    }
}