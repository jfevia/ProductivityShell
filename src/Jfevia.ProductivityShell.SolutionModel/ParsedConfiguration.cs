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
            Profiles = new HashSet<Profile>();
        }

        /// <summary>
        ///     Gets the resulting startup projects.
        /// </summary>
        /// <value>
        ///     The resulting startup projects.
        /// </value>
        public ICollection<Profile> Profiles { get; }

        /// <summary>
        ///     Gets or sets the selected profile.
        /// </summary>
        /// <value>
        ///     The selected profile.
        /// </value>
        public Profile CurrentProfile { get; set; }
    }
}