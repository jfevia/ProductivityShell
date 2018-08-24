using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Jfevia.ProductivityShell.Configuration
{
    internal static class XmlSerializerExtensions
    {
        /// <summary>
        ///     Deserializes the content of the specified reader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(this XmlSerializer serializer, XmlReader reader)
        {
            return (T) serializer.Deserialize(reader);
        }
    }
}