using System.Collections.Generic;

namespace Jfevia.ProductivityShell.Configuration
{
    public class StartupConfiguration
    {
        /// <summary>
        ///     Gets or sets the project configurations.
        /// </summary>
        /// <value>
        ///     The project configurations.
        /// </value>
        public List<ProjectConfiguration> ProjectConfigurations { get; set; } = new List<ProjectConfiguration>();
    }
}