using System;
using System.Collections.Generic;
using System.Linq;

namespace Jfevia.ProductivityShell.Vsix.Settings
{
    internal class SettingsContainer
    {
        private readonly List<string> _profiles;
        private readonly List<Setting> _settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsContainer" /> class.
        /// </summary>
        public SettingsContainer()
        {
            _settings = new List<Setting>();
            _profiles = new List<string>();
        }

        /// <summary>
        ///     Gets or sets the namespace.
        /// </summary>
        /// <value>
        ///     The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        ///     Gets or sets the access modifier.
        /// </summary>
        /// <value>
        ///     The access modifier.
        /// </value>
        public AccessModifier AccessModifier { get; set; }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>
        ///     The settings.
        /// </value>
        public IEnumerable<Setting> Settings => _settings;

        /// <summary>
        ///     Gets the profiles.
        /// </summary>
        /// <value>
        ///     The profiles.
        /// </value>
        public IEnumerable<string> Profiles => _profiles;

        /// <summary>
        ///     Gets the setting count.
        /// </summary>
        /// <value>
        ///     The setting count.
        /// </value>
        public int SettingCount => _settings.Count;

        /// <summary>
        ///     Gets the profile count.
        /// </summary>
        /// <value>
        ///     The profile count.
        /// </value>
        public int ProfileCount => _profiles.Count;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the current profile.
        /// </summary>
        /// <value>
        ///     The current profile.
        /// </value>
        public string CurrentProfile { get; set; }

        /// <summary>
        ///     Adds a new or updates an existing setting with the specified name.
        /// </summary>
        /// <param name="setting">The setting.</param>
        public void AddOrUpdate(Setting setting)
        {
            var existingSetting = _settings.FirstOrDefault(s => Equals(s, setting));
            if (existingSetting == null)
            {
                _settings.Add(setting);
                return;
            }

            // Update
            existingSetting.Scope = setting.Scope;
            existingSetting.Type = setting.Type;
            existingSetting.Value = setting.Value;
            existingSetting.DefaultValue = setting.DefaultValue;
            existingSetting.Profile = setting.Profile;
        }

        /// <summary>
        ///     Adds a new or updates an existing profile.
        /// </summary>
        /// <param name="profile">The profile.</param>
        public void AddOrUpdate(string profile)
        {
            var exists = _profiles.Any(s => string.Equals(s, profile, StringComparison.OrdinalIgnoreCase));
            if (exists)
                return;

            _profiles.Add(profile);
        }

        private static bool Equals(Setting a, Setting b)
        {
            return string.Equals(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}