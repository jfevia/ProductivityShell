using System;
using System.Collections.Generic;
using System.Linq;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class StartupProjectsEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.StartupProjectsEventArgs" />
        ///     class.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="startupProjects">The startup projects.</param>
        public StartupProjectsEventArgs(Profile profile, IEnumerable<string> startupProjects)
        {
            Profile = profile;
            StartupProjects = new HashSet<string>(startupProjects);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.StartupProjectsEventArgs" />
        ///     class.
        /// </summary>
        public StartupProjectsEventArgs()
            : this(null, Enumerable.Empty<string>())
        {
        }

        /// <summary>
        ///     Gets the profile.
        /// </summary>
        /// <value>
        ///     The profile.
        /// </value>
        public Profile Profile { get; }

        /// <summary>
        ///     Gets the startup projects.
        /// </summary>
        /// <value>
        ///     The startup projects.
        /// </value>
        public ICollection<string> StartupProjects { get; }
    }
}