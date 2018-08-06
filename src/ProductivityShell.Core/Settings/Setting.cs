namespace ProductivityShell.Core.Settings
{
    public class Setting
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the scope.
        /// </summary>
        /// <value>
        ///     The scope.
        /// </value>
        public SettingScope Scope { get; set; }

        /// <summary>
        ///     Gets or sets the name of the type.
        /// </summary>
        /// <value>
        ///     The name of the type.
        /// </value>
        public string TypeName { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>
        ///     The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the provider.
        /// </summary>
        /// <value>
        ///     The provider.
        /// </value>
        public string Provider { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Setting" /> is roaming.
        /// </summary>
        /// <value>
        ///     <c>true</c> if roaming; otherwise, <c>false</c>.
        /// </value>
        public bool Roaming { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [generate default value in code].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [generate default value in code]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateDefaultValueInCode { get; set; }
    }
}