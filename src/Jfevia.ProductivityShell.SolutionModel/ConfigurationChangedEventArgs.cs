using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ConfigurationChangedEventArgs : SolutionEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationChangedEventArgs" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="projectConfigurations">The project configurations.</param>
        /// <param name="selectedProjectConfiguration">The selected project configuration.</param>
        public ConfigurationChangedEventArgs(Configuration.Configuration configuration, ICollection<ProjectConfiguration> projectConfigurations, ProjectConfiguration selectedProjectConfiguration)
            : base(configuration, projectConfigurations, selectedProjectConfiguration)
        {
        }
    }
}