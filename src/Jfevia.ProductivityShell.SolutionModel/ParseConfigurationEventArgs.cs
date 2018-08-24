using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ParseConfigurationEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParseConfigurationEventArgs" /> class.
        /// </summary>
        /// <param name="projectConfigurations">The project configurations.</param>
        /// <param name="startupProjects">The startup projects.</param>
        /// <param name="selectedStartupProjects">The selected startup projects.</param>
        public ParseConfigurationEventArgs(ICollection<ProjectConfiguration> projectConfigurations, ICollection<string> startupProjects, ICollection<string> selectedStartupProjects)
        {
            ProjectConfigurations = projectConfigurations;
            StartupProjects = startupProjects;
            SelectedStartupProjects = selectedStartupProjects;
        }

        /// <summary>
        ///     Gets the project configurations.
        /// </summary>
        /// <value>
        ///     The project configurations.
        /// </value>
        public ICollection<ProjectConfiguration> ProjectConfigurations { get; }

        /// <summary>
        ///     Gets the startup projects.
        /// </summary>
        /// <value>
        ///     The startup projects.
        /// </value>
        public ICollection<string> StartupProjects { get; }

        /// <summary>
        ///     Gets the selected startup projects.
        /// </summary>
        /// <value>
        ///     The selected startup projects.
        /// </value>
        public ICollection<string> SelectedStartupProjects { get; }

        /// <summary>
        ///     Gets or sets the parsed configuration.
        /// </summary>
        /// <value>
        ///     The parsed configuration.
        /// </value>
        public ParsedConfiguration ParsedConfiguration { get; set; }
    }
}