using System;
using System.ComponentModel;
using System.Reflection;

namespace ProductivityShell.Core
{
    /// <summary>
    ///     Manages the decisions on if a type is valid as a setting type and/or is obsolete
    /// </summary>
    internal class SettingTypeValidator
    {
        /// <summary>
        ///     Indicate if the type has changed from the version that is currently loaded in this app domain.
        /// </summary>
        /// <param name="type"></param>
        /// <returns><c>true</c> if the specified type is obsolete; otherwise, <c>false</c>.</returns>
        public static bool IsTypeObsolete(Type type)
        {
            var typeInLoadedAssembly = Type.GetType(type.AssemblyQualifiedName);

            // If the type we get from System.Type.GetType(<assembly qualified type name>) is not the same
            // as the type provided, something has changed in the defining assembly
            return ReferenceEquals(type, typeInLoadedAssembly) == false;
        }

        /// <summary>
        ///     Determines whether [is valid setting type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if [is valid setting type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidSettingType(Type type)
        {
            if (!type.IsPublic)
                return false;

            if (type.IsPointer)
                return false;

            if (type.IsGenericType)
                return false;

            if (type == typeof(void))
                return false;

            if (!(type.IsClass || type.IsValueType))
                return false;

            return CanSerializeType(type);
        }

        /// <summary>
        ///     Determines whether this instance [can serialize type] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if this instance [can serialize type] the specified type; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanSerializeType(Type type)
        {
            try
            {
                var tc = TypeDescriptor.GetConverter(type);
                if (tc.CanConvertFrom(typeof(string)) && tc.CanConvertTo(typeof(string)))
                    return true;
                if (type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis,
                        Type.EmptyTypes, null) != null)
                    return true;
            }
            catch
            {
                // Since we have no idea what the custom type descriptors may do, we catch everything that we can
                // and pretend that nothing too bad happened...
            }

            return false;
        }
    }
}