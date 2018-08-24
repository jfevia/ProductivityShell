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
        /// <param name="projectConfigurations">The project configurations.</param>
        /// <param name="selectedProjectConfiguration">The selected project configuration.</param>
        public SolutionEventArgs(Configuration.Configuration configuration, ICollection<ProjectConfiguration> projectConfigurations, ProjectConfiguration selectedProjectConfiguration)
        {
            Configuration = configuration;
            ProjectConfigurations = projectConfigurations;
            SelectedProjectConfiguration = selectedProjectConfiguration;
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        public Configuration.Configuration Configuration { get; }

        /// <summary>
        ///     Gets the project configurations.
        /// </summary>
        /// <value>
        ///     The project configurations.
        /// </value>
        public ICollection<ProjectConfiguration> ProjectConfigurations { get; }

        /// <summary>
        ///     Gets the selected project configuration.
        /// </summary>
        /// <value>
        ///     The selected project configuration.
        /// </value>
        public ProjectConfiguration SelectedProjectConfiguration { get; }
    }
}