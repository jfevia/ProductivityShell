using System.Collections.Generic;
using Jfevia.ProductivityShell.ProjectModel;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class SolutionBuildConfiguration
    {
        /// <summary>
        ///     Gets or sets the startup projects.
        /// </summary>
        /// <value>
        ///     The startup projects.
        /// </value>
        public IEnumerable<Project> StartupProjects { get; set; }
    }
}