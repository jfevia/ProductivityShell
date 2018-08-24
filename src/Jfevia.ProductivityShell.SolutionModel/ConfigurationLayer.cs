namespace Jfevia.ProductivityShell.SolutionModel
{
    public class ConfigurationLayer
    {
        private static ConfigurationLayer _default;

        /// <summary>
        ///     Gets or sets the machine file path.
        /// </summary>
        /// <value>
        ///     The machine file path.
        /// </value>
        public string MachineFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the shared file path.
        /// </summary>
        /// <value>
        ///     The shared file path.
        /// </value>
        public string SharedFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the user file path.
        /// </summary>
        /// <value>
        ///     The user file path.
        /// </value>
        public string UserFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the computed configuration.
        /// </summary>
        /// <value>
        ///     The computed configuration.
        /// </value>
        public Configuration.Configuration ComputedConfig { get; set; }

        /// <summary>
        ///     Gets or sets the machine configuration.
        /// </summary>
        /// <value>
        ///     The machine configuration.
        /// </value>
        public Configuration.Configuration MachineConfig { get; set; }

        /// <summary>
        ///     Gets or sets the shared configuration.
        /// </summary>
        /// <value>
        ///     The shared configuration.
        /// </value>
        public Configuration.Configuration SharedConfig { get; set; }

        /// <summary>
        ///     Gets or sets the user configuration.
        /// </summary>
        /// <value>
        ///     The user configuration.
        /// </value>
        public Configuration.Configuration UserConfig { get; set; }

        /// <summary>
        ///     Gets the default.
        /// </summary>
        /// <value>
        ///     The default.
        /// </value>
        public static ConfigurationLayer Default => _default ?? (_default = new ConfigurationLayer
        {
            MachineConfig = new Configuration.Configuration(),
            SharedConfig = new Configuration.Configuration(),
            UserConfig = new Configuration.Configuration()
        });
    }
}