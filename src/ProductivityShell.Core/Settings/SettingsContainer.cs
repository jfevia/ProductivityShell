using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Designer.Interfaces;
using ProductivityShell.Core.Properties;

namespace ProductivityShell.Core.Settings
{
    public class SettingsContainer
    {
        private readonly IVSMDCodeDomProvider _codeDomProvider;
        private readonly HashSet<Setting> _settings;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Core.Settings.SettingsContainer" /> class.
        /// </summary>
        public SettingsContainer(IVSMDCodeDomProvider codeDomProvider)
        {
            _codeDomProvider = codeDomProvider;
            _settings = new HashSet<Setting>();
        }

        /// <summary>
        ///     Gets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public IEnumerable<Setting> Items => _settings;

        /// <summary>
        ///     Gets the items count.
        /// </summary>
        /// <value>
        ///     The items count.
        /// </value>
        public int ItemsCount => _settings.Count;

        /// <summary>
        ///     Gets the code provider.
        /// </summary>
        /// <value>
        ///     The code provider.
        /// </value>
        private CodeDomProvider CodeProvider
        {
            get
            {
                try
                {
                    return _codeDomProvider?.CodeDomProvider as CodeDomProvider;
                }
                catch (COMException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the namespace.
        /// </summary>
        /// <value>
        ///     The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        ///     Adds or updates a specified setting.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <exception cref="ArgumentNullException">setting</exception>
        public void AddOrUpdate(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            var existingItem = _settings.FirstOrDefault(s => EqualIdentifiers(s.Name, setting.Name));
            if (existingItem != null)
            {
                existingItem.Scope = setting.Scope;
                existingItem.TypeName = setting.TypeName;
                existingItem.Value = setting.Value;
            }

            _settings.Add(setting);
        }

        /// <summary>
        ///     Creates a unique name.
        /// </summary>
        /// <param name="baseName">Name of the base.</param>
        /// <returns>The unique name.</returns>
        internal string CreateUniqueName(string baseName = null)
        {
            if (string.IsNullOrEmpty(baseName))
                baseName = Resources.SettingsContainer_DefaultSettingName;

            var existingNames = new HashSet<string>();
            foreach (var setting in _settings)
                existingNames.Add(setting.Name);

            string suggestedName = MakeValidIdentifier(baseName);
            if (!existingNames.Contains(suggestedName))
                return suggestedName;

            for (var settingIndex = 0; settingIndex < _settings.Count; settingIndex++)
            {
                suggestedName = MakeValidIdentifier($"{baseName}{settingIndex}");
                if (!existingNames.Contains(suggestedName))
                    return suggestedName;
            }

            Debug.Fail("You should never reach this line of code!");
            return "";
        }

        private string MakeValidIdentifier(string name)
        {
            if (CodeProvider != null && !IsValidIdentifier(name))
                return CodeProvider.CreateValidIdentifier(name);
            return name;
        }

        /// <summary>
        ///     Determines whether [is valid name] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="checkForUniqueness">if set to <c>true</c> [check for uniqueness].</param>
        /// <param name="instanceToIgnore">The instance to ignore.</param>
        /// <returns>
        ///     <c>true</c> if [is valid name] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsValidName(string name, bool checkForUniqueness = false, Setting instanceToIgnore = null)
        {
            return IsValidIdentifier(name) && (!checkForUniqueness || IsUniqueName(name, instanceToIgnore));
        }

        /// <summary>
        ///     Determines whether [is unique name] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="instanceToIgnore">The instance to ignore.</param>
        /// <returns>
        ///     <c>true</c> if [is unique name] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsUniqueName(string name, Setting instanceToIgnore = null)
        {
            // An empty name is not considered unique
            if (string.IsNullOrWhiteSpace(name))
                return false;

            foreach (var setting in _settings)
            {
                if (!EqualIdentifiers(name, setting.Name))
                    continue;

                if (setting != instanceToIgnore)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether two identifiers are equal. Identifiers are case insensitive.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns><c>true</c> if the two identifiers are equal; otherwise, <c>false</c>.</returns>
        internal static bool EqualIdentifiers(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Determines whether [is valid identifier] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///     <c>true</c> if [is valid identifier] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidIdentifier(string name)
        {
            if (name == null)
                return false;

            if (CodeGenerator.IsValidLanguageIndependentIdentifier(name))
                return CodeProvider == null || CodeProvider.IsValidIdentifier(name);

            return false;
        }
    }
}