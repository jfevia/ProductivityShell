using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ProductivityShell.Core.Settings
{
    public class SettingsValueCache
    {
        private readonly Dictionary<Type, Dictionary<string, object>> _cachedSettingValues;
        private readonly CultureInfo _culture;

        public SettingsValueCache(CultureInfo culture)
        {
            _cachedSettingValues = new Dictionary<Type, Dictionary<string, object>>();
            _culture = culture;
        }

        public object GetValue(Type type, string value)
        {
            if (!_cachedSettingValues.TryGetValue(type, out var dictionary))
            {
                dictionary = new Dictionary<string, object>();
                _cachedSettingValues[type] = dictionary;
            }

            if (dictionary.TryGetValue(value, out var obj))
                return obj;

            var settingsValueSerializer = new SettingsValueSerializer();
            obj = RuntimeHelpers.GetObjectValue(settingsValueSerializer.Deserialize(value, type, _culture));
            dictionary[value] = RuntimeHelpers.GetObjectValue(obj);

            return obj;
        }
    }
}