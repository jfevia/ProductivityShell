using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ParsedConfiguration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParsedConfiguration" /> class.
        /// </summary>
        public ParsedConfiguration()
        {
            ProjectConfigurations = new HashSet<ProjectConfiguration>();
        }

        /// <summary>
        ///     Gets the resulting startup projects.
        /// </summary>
        /// <value>
        ///     The resulting startup projects.
        /// </value>
        public ICollection<ProjectConfiguration> ProjectConfigurations { get; }

        /// <summary>
        ///     Gets or sets the selected project configuration.
        /// </summary>
        /// <value>
        ///     The selected project configuration.
        /// </value>
        public ProjectConfiguration CurrentProjectConfiguration { get; set; }
    }
}