using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Jfevia.ProductivityShell.Configuration
{
    public static class ConfigurationXmlSerializer
    {
        /// <summary>
        ///     Serializes the specified configuration and writes the result to the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Serialize(Stream stream, Configuration configuration, XmlWriterSettings settings = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (settings == null)
            {
                settings = new XmlWriterSettings();
                settings.Indent = true;
            }

            var serializer = new XmlSerializer(typeof(Configuration));
            using (var xmlWriter = XmlWriter.Create(stream, settings))
                serializer.Serialize(xmlWriter, configuration);
        }

        /// <summary>
        ///     Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static Configuration Deserialize(Stream stream, XmlReaderSettings settings = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (settings == null)
                settings = new XmlReaderSettings();

            var serializer = new XmlSerializer(typeof(Configuration));
            using (var xmlReader = XmlReader.Create(stream, settings))
                return serializer.Deserialize<Configuration>(xmlReader);
        }
    }
}