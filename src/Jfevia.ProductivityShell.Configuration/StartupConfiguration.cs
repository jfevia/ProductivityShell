using System.Collections.Generic;

namespace Jfevia.ProductivityShell.Configuration
{
    public class StartupConfiguration
    {
        /// <summary>
        ///     Gets or sets the profiles.
        /// </summary>
        /// <value>
        ///     The profiles.
        /// </value>
        public List<Profile> Profiles { get; set; } = new List<Profile>();
    }
}