using System;
using System.IO;
using System.Text;
using System.Xml;
using ProductivityShell.Core.Properties;

namespace ProductivityShell.Core.Settings
{
    public sealed class SettingsSerializer
    {
        public const string SchemaUri = "http://schemas.microsoft.com/VisualStudio/2004/01/settings";
        public const string CultureInvariantDefaultProfileName = "(Default)";

        /// <summary>
        ///     Deserialize XML stream of settings
        /// </summary>
        /// <param name="settingsContainer">The settings container.</param>
        /// <param name="reader">The reader.</param>
        public static void Deserialize(SettingsContainer settingsContainer, TextReader reader)
        {
            var xmlDoc = new XmlDocument();
            var xmlReader = new XmlTextReader(reader)
            {
                Normalization = false
            };

            xmlDoc.Load(xmlReader);

            var xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            xmlNamespaceManager.AddNamespace("Settings", SchemaUri);

            var rootNode = xmlDoc.SelectSingleNode("Settings:SettingsFile", xmlNamespaceManager);

            // Deserialize persisted namespace
            settingsContainer.Namespace = rootNode?.Attributes?["GeneratedClassNamespace"]?.Value;

            // Deserialize settings
            var settingNodes = xmlDoc.SelectNodes("Settings:SettingsFile/Settings:Settings/Settings:Setting",
                xmlNamespaceManager);

            if (settingNodes == null)
                return;

            foreach (XmlNode settingNode in settingNodes)
            {
                if (settingNode.Attributes == null)
                    continue;

                var typeAttr = settingNode.Attributes["Type"];
                var scopeAttr = settingNode.Attributes["Scope"];
                var nameAttr = settingNode.Attributes["Name"];
                var generateDefaultValueAttribute = settingNode.Attributes["GenerateDefaultValueInCode"];
                var descriptionAttr = settingNode.Attributes["Description"];
                var providerAttr = settingNode.Attributes["Provider"];
                var roamingAttr = settingNode.Attributes["Roaming"];

                if (typeAttr == null || scopeAttr == null || nameAttr == null)
                    throw new SettingsSerializerException(Resources.SettingsSerializer_CannotLoadSettingsFile);

                var newSettingName = settingsContainer.CreateUniqueName(nameAttr.Value);
                if (!settingsContainer.IsValidName(newSettingName))
                    throw new SettingsSerializerException(string.Format(
                        Resources.SettingsSerializer_InvalidIdentifier_1Arg,
                        nameAttr.Value));

                var setting = new Setting();
                setting.TypeName = typeAttr.Value;
                setting.Name = newSettingName;
                setting.Scope = Enum.TryParse(scopeAttr.Value, out SettingScope scope) ? scope : default(SettingScope);

                if (!string.IsNullOrWhiteSpace(descriptionAttr?.Value))
                    setting.Description = descriptionAttr.Value;

                if (!string.IsNullOrWhiteSpace(providerAttr?.Value))
                    setting.Provider = providerAttr.Value;

                if (!string.IsNullOrWhiteSpace(roamingAttr?.Value))
                    setting.Roaming = XmlConvert.ToBoolean(roamingAttr.Value);

                if (!string.IsNullOrWhiteSpace(generateDefaultValueAttribute?.Value) &&
                    !XmlConvert.ToBoolean(generateDefaultValueAttribute.Value))
                    setting.GenerateDefaultValueInCode = false;
                else
                    setting.GenerateDefaultValueInCode = true;

                // Deserialize the value
                XmlNode valueNode = null;

                // Let's check the "normal" value element
                valueNode = settingNode.SelectSingleNode("./Settings:Value[@Profile=\"(Default)\"]",
                    xmlNamespaceManager);

                if (valueNode != null)
                    setting.Value = valueNode.InnerText;

                settingsContainer.AddOrUpdate(setting);
            }
        }

        /// <summary>
        ///     Serializes the specified settings container.
        /// </summary>
        /// <param name="settingsContainer">The settings container.</param>
        /// <param name="generatedClassNamespace">The generated class namespace.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="writer">Text writer on stream to serialize settings to.</param>
        /// <param name="encoding">The encoding.</param>
        public static void Serialize(SettingsContainer settingsContainer, string generatedClassNamespace,
            string className,
            TextWriter writer, Encoding encoding)
        {
            // We have to store the namespace here in case it changes during serialization
            settingsContainer.Namespace = generatedClassNamespace;

            var settingsWriter = new XmlTextWriter(writer)
            {
                Formatting = Formatting.Indented,
                Indentation = 2
            };

            if (encoding == null)
                encoding = Encoding.Unicode;

            settingsWriter.WriteProcessingInstruction("xml", $@"version='1.0' encoding='{encoding.BodyName}'");
            settingsWriter.WriteStartElement("SettingsFile");
            settingsWriter.WriteAttributeString("xmlns", string.Empty, SchemaUri);
            settingsWriter.WriteAttributeString("CurrentProfile", CultureInvariantDefaultProfileName);

            if (settingsContainer.ItemsCount > 0)
            {
                // We only want to scribble this into the file if we actually have some settings to generate.
                // The main purpose for this is to be able to clean up any default values that we may have persisted
                // in the app.config file (which includes the generated namespace) If we don't save anything, we
                // know we don't have anything to clean up!
                settingsWriter.WriteAttributeString("GeneratedClassNamespace", generatedClassNamespace);
                settingsWriter.WriteAttributeString("GeneratedClassName", className);
            }

            // Write (empty) Profiles element
            settingsWriter.WriteStartElement("Profiles");
            settingsWriter.WriteEndElement();

            settingsWriter.WriteStartElement("Settings");
            foreach (var setting in settingsContainer.Items)
            {
                settingsWriter.WriteStartElement("Setting");
                settingsWriter.WriteAttributeString("Name", setting.Name);

                if (!string.IsNullOrWhiteSpace(setting.Description))
                    settingsWriter.WriteAttributeString("Description", setting.Description);

                if (!string.IsNullOrWhiteSpace(setting.Provider))
                    settingsWriter.WriteAttributeString("Provider", setting.Provider);

                if (setting.Roaming)
                    settingsWriter.WriteAttributeString("Roaming", XmlConvert.ToString(setting.Roaming));

                if (!setting.GenerateDefaultValueInCode)
                    settingsWriter.WriteAttributeString("GenerateDefaultValueInCode", XmlConvert.ToString(false));

                settingsWriter.WriteAttributeString("Type", setting.TypeName);
                settingsWriter.WriteAttributeString("Scope", setting.Scope.ToString());

                // We should always have a "normal" value, regardless of whether there is a design time value
                settingsWriter.WriteStartElement("Value");
                settingsWriter.WriteAttributeString("Profile", CultureInvariantDefaultProfileName);
                settingsWriter.WriteString(setting.Value);
                settingsWriter.WriteEndElement(); // End of Value element

                settingsWriter.WriteEndElement(); // End of Setting element
            }

            settingsWriter.WriteEndElement(); // End of Settings element
            settingsWriter.WriteEndElement(); // End of SettingsFile element
            settingsWriter.Flush();
            settingsWriter.Close();
        }
    }
}