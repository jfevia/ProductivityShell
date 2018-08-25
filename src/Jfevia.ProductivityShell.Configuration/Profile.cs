using System.Collections.Generic;
using System.Xml.Serialization;

namespace Jfevia.ProductivityShell.Configuration
{
    public class Profile
    {
        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        [XmlAttribute]
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the projects.
        /// </summary>
        /// <value>
        ///     The projects.
        /// </value>
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}