using System;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ConfigurationEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets or sets the configuration layer.
        /// </summary>
        /// <value>
        ///     The configuration layer.
        /// </value>
        public ConfigurationLayer ConfigurationLayer { get; set; }
    }
}