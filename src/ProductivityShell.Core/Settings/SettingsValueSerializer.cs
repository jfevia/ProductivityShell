using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Shell.Design.Serialization;

namespace ProductivityShell.Core.Settings
{
    /// <summary>
    ///     Serialize object in the same way that the runtime will serialize them, with the additional twist that you can pass
    ///     a culture info.
    /// </summary>
    public class SettingsValueSerializer
    {
        /// <summary>
        ///     Deserializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The deserialized value.</returns>
        public object Deserialize(string value, Type type, CultureInfo culture)
        {
            // Strings require special handling, since the ConfigHelper API assumes that all serialized representations
            // of types that it will use anything but XML serialization to serialize and deserialize are correctly escaped.
            if (type == typeof(string))
                return value;

            // We don't care about the name of the setting
            var prop = new SettingsProperty(string.Empty)
            {
                ThrowOnErrorDeserializing = true,
                PropertyType = type
            };

            var propVal = new SettingsPropertyValue(prop);
            if (type == null)
                return null;

            try
            {
                var configurationHelperService = new ConfigurationHelperService();
                prop.SerializeAs = configurationHelperService.GetSerializeAs(prop.PropertyType);
                object val;
                if (prop.SerializeAs == SettingsSerializeAs.String)
                {
                    // We have to take care of this, because we want to use the CultureInfo passed in to us when doing the deserialization
                    // The runtime will always use InvariantCulture, but we sometimes have to show the default value in the UI, which
                    // means that we should use the current thread's culture
                    var typeConverter = TypeDescriptor.GetConverter(type);
                    val = typeConverter.ConvertFromString(null, culture, value);
                }
                else
                {
                    propVal.SerializedValue = value;
                    val = propVal.PropertyValue;
                }

                // If the type converter was broken and returned an unknown type, we better stop it here
                if (val != null && !type.IsInstanceOfType(val))
                    return null;

                // For some reason, enumerations' deserialization works even when the integer value is out of range
                // We know better, let's check for this funky case and return null
                if (val == null || !typeof(Enum).IsAssignableFrom(type) || Enum.IsDefined(type, val))
                    return val;

                // If this is a flags attribute, we can't assume that the enum is defined
                return type.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0 ? val : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The serialized value.</returns>
        public string Serialize(object value, CultureInfo culture)
        {
            string serializedValue = null;
            try
            {
                serializedValue = GetSerializedValue(value, culture);
            }
            catch
            {
                // Nothing
            }

            // Make sure we always return a valid string
            return serializedValue ?? string.Empty;
        }

        /// <summary>
        ///     Gets the serialized value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The serialized value.</returns>
        private static string GetSerializedValue(object value, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return string.Empty;
                case string s:
                    return s;
            }

            // We don't care about the name of the setting
            var prop = new SettingsProperty(string.Empty);
            var configurationHelperService = new ConfigurationHelperService();
            prop.ThrowOnErrorSerializing = true;
            prop.PropertyType = value.GetType();
            prop.SerializeAs = configurationHelperService.GetSerializeAs(prop.PropertyType);
            if (prop.SerializeAs == SettingsSerializeAs.String)
            {
                // We have to take care of this, cause we want to use the CultureInfo passed in to us when doing the serialization
                // The runtime will always use InvariantCulture, but we sometimes have to show the default value in the UI, which
                // means that we should use the current thread's culture
                // We intentionally pass in the type instead of the actual object, because that is what the runtime does
                var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                return typeConverter.ConvertToString(null, culture, value);
            }

            var propVal = new SettingsPropertyValue(prop);
            try
            {
                propVal.PropertyValue = value;
                return propVal.SerializedValue as string;
            }
            catch
            {
                // Failed to serialize, let's pretend nothing happened
                return value.ToString();
            }
        }

        /// <summary>
        ///     Normalizes the specified serialized value.
        /// </summary>
        /// <param name="serializedValue">The serialized value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The normalized serialized value.</returns>
        public string Normalize(string serializedValue, Type type)
        {
            if (!SettingTypeValidator.IsTypeObsolete(type))
                return Serialize(
                    RuntimeHelpers.GetObjectValue(Deserialize(serializedValue, type,
                        CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);

            return serializedValue;
        }
    }
}