using System.Xml.Serialization;

namespace Jfevia.ProductivityShell.Configuration
{
    public class Project
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the command line arguments.
        /// </summary>
        /// <value>
        ///     The command line arguments.
        /// </value>
        public string CommandLineArgs { get; set; }

        /// <summary>
        ///     Gets or sets the working directory.
        /// </summary>
        /// <value>
        ///     The working directory.
        /// </value>
        public string WorkingDirectory { get; set; }

        /// <summary>
        ///     Gets or sets the start external program.
        /// </summary>
        /// <value>
        ///     The start external program.
        /// </value>
        public string StartExternalProgram { get; set; }

        /// <summary>
        ///     Gets or sets the start browser URL.
        /// </summary>
        /// <value>
        ///     The start browser URL.
        /// </value>
        public string StartBrowserUrl { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is remote debugging enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is remote debugging enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsRemoteDebuggingEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the remote debugging machine.
        /// </summary>
        /// <value>
        ///     The remote debugging machine.
        /// </value>
        public string RemoteDebuggingMachine { get; set; }
    }
}