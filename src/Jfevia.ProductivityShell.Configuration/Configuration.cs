using System;
using System.Collections.Generic;
using System.Linq;

namespace Jfevia.ProductivityShell.Configuration
{
    public class Configuration
    {
        /// <summary>
        ///     Gets or sets the startup.
        /// </summary>
        /// <value>
        ///     The startup.
        /// </value>
        public StartupConfiguration Startup { get; set; } = new StartupConfiguration();

        /// <summary>
        ///     Gets the configuration layer.
        /// </summary>
        /// <param name="machineConfig">The machine configuration.</param>
        /// <param name="sharedConfig">The shared configuration.</param>
        /// <param name="userConfig">The user configuration.</param>
        /// <returns>The configuration layer.</returns>
        public static Configuration ComputeConfig(Configuration machineConfig, Configuration sharedConfig, Configuration userConfig)
        {
            var configuration = new Configuration();

            var configs = new[] {machineConfig, sharedConfig, userConfig};
            foreach (var config in configs.Where(s => s != null))
            {
                foreach (var item in config.Startup.Profiles)
                {
                    var existingProfile = configuration.Startup.Profiles.FirstOrDefault(s => string.Equals(s.DisplayName, item.DisplayName, StringComparison.OrdinalIgnoreCase));
                    if (existingProfile == null)
                    {
                        configuration.Startup.Profiles.Add(item);
                        continue;
                    }

                    existingProfile.Projects = new List<Project>(item.Projects);
                }
            }

            return configuration;
        }
    }
}