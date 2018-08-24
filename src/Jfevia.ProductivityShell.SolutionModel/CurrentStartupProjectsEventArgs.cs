using System;
using System.Collections.Generic;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class CurrentStartupProjectsEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.CurrentStartupProjectsEventArgs" /> class.
        /// </summary>
        public CurrentStartupProjectsEventArgs()
        {
            StartupProjects = new HashSet<string>();
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