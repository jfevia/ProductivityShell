using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ProjectEventArgs : SolutionEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.SolutionModel.ProjectEventArgs" /> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="projectConfigurations">The project configurations.</param>
        /// <param name="selectedProjectConfiguration">The selected project configuration.</param>
        public ProjectEventArgs(string projectName, Configuration.Configuration configuration, ICollection<ProjectConfiguration> projectConfigurations, ProjectConfiguration selectedProjectConfiguration)
            : base(configuration, projectConfigurations, selectedProjectConfiguration)
        {
            ProjectName = projectName;
        }

        /// <summary>
        ///     Gets the name of the project.
        /// </summary>
        /// <value>
        ///     The name of the project.
        /// </value>
        public string ProjectName { get; }
    }
}