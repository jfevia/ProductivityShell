using System;
using System.Linq;
using System.Xml.Linq;

namespace ProductivityShell.Commands.TextEditor
{
    internal class SettingsSerializer : IDisposable
    {
        private const string Namespace = "http://schemas.microsoft.com/VisualStudio/2004/01/settings";
        private readonly XDocument _document;
        private readonly string _filePath;
        private readonly XElement _parent;

        private readonly XName _settingsName = XName.Get("Settings", Namespace);
        private readonly XName _singleSettingName = XName.Get("Setting", Namespace);
        private readonly XName _valueName = XName.Get("Value", Namespace);

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsSerializer" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public SettingsSerializer(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _document = XDocument.Load(filePath);
            _parent = _document.Root?.Element(_settingsName);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public void Save()
        {
            _document.Save(_filePath);
        }

        /// <summary>
        ///     Adds a new or updates an existing settings with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeFullName">The full name of the type.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="value">The value.</param>
        /// <param name="profile">The profile.</param>
        public void AddOrUpdate(string name, string typeFullName, SettingScope scope, string value, string profile = "(Default)")
        {
            var entry = GetExistingEntry(name);
            if (entry == null)
            {
                WriteEntry(name, typeFullName, scope, value, profile);
                return;
            }

            var valueElement = entry.Element(_valueName);
            if (valueElement == null)
            {
                WriteEntryValue(entry, value, profile);
                return;
            }

            valueElement.Value = value;
        }

        /// <summary>
        ///     Gets the existing entry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The existing entry.</returns>
        private XElement GetExistingEntry(string name)
        {
            return _parent
                .Elements(_singleSettingName)
                .FirstOrDefault(s => s.Attribute("Name")?.Value == name);
        }

        /// <summary>
        ///     Writes the entry.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeFullName">The full name of the type.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="value">The value.</param>
        /// <param name="profile">The profile.</param>
        private void WriteEntry(string name, string typeFullName, SettingScope scope, string value, string profile)
        {
            var entry = new XElement(_singleSettingName);
            entry.Add(new XAttribute("Name", name));
            entry.Add(new XAttribute("Type", typeFullName));
            entry.Add(new XAttribute("Scope", scope));

            WriteEntryValue(entry, value, profile);

            _parent.Add(entry);
        }

        /// <summary>
        ///     Writes the entry value.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="value">The value.</param>
        /// <param name="profile">The profile.</param>
        private void WriteEntryValue(XContainer entry, string value, string profile)
        {
            var valueElement = new XElement(_valueName);
            valueElement.Add(new XAttribute("Profile", profile));
            valueElement.Value = value;
            entry.Add(valueElement);
        }
    }
}