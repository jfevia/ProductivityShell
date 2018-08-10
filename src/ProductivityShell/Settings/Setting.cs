using System;

namespace ProductivityShell.Settings
{
    internal class Setting
    {
        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        /// <value>
        ///     The scope.
        /// </value>
        public SettingScope Scope { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public Type Type { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        ///     Gets or sets the profile.
        /// </summary>
        /// <value>
        ///     The profile.
        /// </value>
        public string Profile { get; set; } = "(Default)";

        /// <summary>
        ///     Gets or sets the default value.
        /// </summary>
        /// <value>
        ///     The default value.
        /// </value>
        public object DefaultValue { get; set; }
    }
}