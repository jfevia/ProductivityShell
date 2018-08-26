using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Jfevia.ProductivityShell.Vsix.Settings
{
    internal static class SettingsSerializer
    {
        private const string Namespace = "http://schemas.microsoft.com/VisualStudio/2004/01/settings";

        private static readonly XName ProfilesName = XName.Get("Profiles", Namespace);
        private static readonly XName SettingsName = XName.Get("Settings", Namespace);
        private static readonly XName SingleSettingName = XName.Get("Setting", Namespace);
        private static readonly XName ValueName = XName.Get("Value", Namespace);

        /// <summary>
        ///     Deserializes the object from the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The settings container.</returns>
        public static SettingsContainer Deserialize(string text)
        {
            var document = XDocument.Parse(text);
            if (document.Root == null)
                throw new ArgumentOutOfRangeException(nameof(document.Root));

            var root = document.Root;
            if (root == null)
                throw new ArgumentOutOfRangeException(nameof(root));

            var settingsContainer = new SettingsContainer();
            settingsContainer.CurrentProfile = root.Attribute("CurrentProfile")?.Value;
            settingsContainer.Namespace = root.Attribute("GeneratedClassNamespace")?.Value;
            settingsContainer.Name = root.Attribute("GeneratedClassName")?.Value;

            XElement profilesParentElement;
            if ((profilesParentElement = root.Element(ProfilesName)) != null)
                foreach (var profileElement in profilesParentElement.Elements("Profile"))
                    settingsContainer.AddOrUpdate(profileElement?.Value);

            XElement settingsParentElement;
            if ((settingsParentElement = root.Element(SettingsName)) == null)
                return settingsContainer;

            foreach (var settingElement in settingsParentElement.Elements(SingleSettingName))
            {
                var setting = new Setting();

                var typeName = settingElement.Attribute("Type")?.Value;
                if (!string.IsNullOrWhiteSpace(typeName))
                    setting.Type = Type.GetType(typeName);

                var scopeName = settingElement.Attribute("Scope")?.Value;
                if (!string.IsNullOrWhiteSpace(scopeName))
                    setting.Scope = (SettingScope) Enum.Parse(typeof(SettingScope), scopeName);

                setting.Name = settingElement.Attribute("Name")?.Value;

                var valueElement = settingElement.Element(ValueName);
                if (valueElement == null)
                    throw new ArgumentOutOfRangeException(nameof(valueElement));

                setting.Profile = valueElement.Attribute("Profile")?.Value;
                setting.Value = valueElement.Value;
                setting.DefaultValue = valueElement.Value;

                settingsContainer.AddOrUpdate(setting);
            }

            return settingsContainer;
        }

        /// <summary>
        ///     Serializes the specified settings container.
        /// </summary>
        /// <param name="settingsContainer">The settings container.</param>
        /// <returns>The serialized settings container.</returns>
        public static string Serialize(SettingsContainer settingsContainer)
        {
            var stringBuilder = new StringBuilder();
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.Encoding = Encoding.UTF8;

            var document = XmlWriter.Create(stringBuilder, xmlSettings);
            document.WriteStartElement("SettingsFile", Namespace);
            document.WriteAttributeString("CurrentProfile", settingsContainer.CurrentProfile);
            document.WriteAttributeString("GeneratedClassNamespace", settingsContainer.Namespace);
            document.WriteAttributeString("GeneratedClassName", settingsContainer.Name);

            // Profiles
            document.WriteStartElement("Profiles");
            foreach (var profile in settingsContainer.Profiles)
            {
                document.WriteStartElement("Profile");
                document.WriteValue(profile);
                document.WriteEndElement();
            }

            document.WriteEndElement();

            // Settings
            document.WriteStartElement("Settings");
            foreach (var setting in settingsContainer.Settings)
            {
                document.WriteStartElement("Setting");
                document.WriteAttributeString("Name", setting.Name);
                document.WriteAttributeString("Type", setting.Type.FullName ?? string.Empty);
                document.WriteAttributeString("Scope", setting.Scope.ToString());
                document.WriteStartElement("Value");
                document.WriteAttributeString("Profile", setting.Profile);
                document.WriteValue(setting.Value);
                document.WriteEndElement();
                document.WriteEndElement();
            }

            document.WriteEndElement();

            // SettingsFile
            document.WriteEndElement();

            document.Flush();
            return stringBuilder.ToString();
        }
    }
}