using System;
using System.Collections.Generic;
using System.Linq;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class StartupProjectsEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.StartupProjectsEventArgs" />
        ///     class.
        /// </summary>
        /// <param name="startupProjects">The startup projects.</param>
        public StartupProjectsEventArgs(IEnumerable<string> startupProjects)
        {
            StartupProjects = new HashSet<string>(startupProjects);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.StartupProjectsEventArgs" /> class.
        /// </summary>
        public StartupProjectsEventArgs()
            : this(Enumerable.Empty<string>())
        {
        }

        /// <summary>
        ///     Gets the startup projects.
        /// </summary>
        /// <value>
        ///     The startup projects.
        /// </value>
        public ICollection<string> StartupProjects { get; }
    }
}