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
        /// <param name="profiles">The profiles.</param>
        /// <param name="selectedProfile">The selected profile.</param>
        public ProjectEventArgs(string projectName, Configuration.Configuration configuration, ICollection<Profile> profiles, Profile selectedProfile)
            : base(configuration, profiles, selectedProfile)
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