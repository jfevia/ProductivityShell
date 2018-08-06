using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace ProductivityShell.Core.Settings
{
    public class GenericTypeDescriptionProvider : TypeDescriptionProvider
    {
    }

    public class SettingsTypeCache : IDisposable
    {
        private readonly bool _caseSensitive;
        private readonly Type[] _wellKnownTypes;
        private ITypeResolutionService _typeResolutionService;
        private readonly TypeDescriptionProvider typeDescriptionProvider;

        public SettingsTypeCache(IVsHierarchy vsHierarchy, uint itemId, ITypeResolutionService typeResolutionService,
            bool caseSensitive)
        {
            typeDescriptionProvider = new GenericTypeDescriptionProvider();

            _wellKnownTypes = new[]
            {
                typeof(bool),
                typeof(byte),
                typeof(char),
                typeof(DateTime),
                typeof(decimal),
                typeof(double),
                typeof(Guid),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(sbyte),
                typeof(float),
                typeof(TimeSpan),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(Color),
                typeof(Font),
                typeof(Point),
                typeof(Size),
                typeof(string),
                typeof(StringCollection)
            };

            if (typeResolutionService == null || vsHierarchy == null)
                throw new ArgumentNullException();
            _typeResolutionService = typeResolutionService;
            _caseSensitive = caseSensitive;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _typeResolutionService = null;
        }

        public Type GetSettingType(string typeName)
        {
            var wellKnownTypes = GetWellKnownTypes();
            var index = 0;
            Type type1;
            while (index < wellKnownTypes.Length)
            {
                var type2 = wellKnownTypes[index];
                if (string.Equals(type2.FullName, typeName))
                {
                    type1 = type2;
                    goto label_6;
                }

                checked
                {
                    ++index;
                }
            }

            type1 = ResolveType(typeName, _caseSensitive);
            label_6:
            return type1;
        }

        public Type[] GetSupportedTypes(Type[] sourceTypes)
        {
            if (sourceTypes == null)
                throw new ArgumentNullException(nameof(sourceTypes));
            var typeList = new List<Type>();
            foreach (var sourceType in sourceTypes)
                if (IsSupportedType(sourceType))
                    typeList.Add(sourceType);
            return typeList.ToArray();
        }

        public Type[] GetWellKnownTypes()
        {
            return GetSupportedTypes(_wellKnownTypes);
        }

        public bool IsSupportedType(Type sourceType, Func<Type, bool> customFilter = null)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            var flag = false;
            try
            {
                flag = typeDescriptionProvider.IsSupportedType(sourceType);
                if (flag)
                    if (customFilter != null)
                    {
                        var reflectionType = typeDescriptionProvider.GetReflectionType(sourceType);
                        if (reflectionType != null)
                            flag = customFilter(reflectionType);
                    }
            }
            catch
            {
                // Nothing
            }

            return flag;
        }

        public bool IsWellKnownType(Type type)
        {
            var wellKnownTypes = GetWellKnownTypes();
            var index = 0;
            while (index < wellKnownTypes.Length)
            {
                if (wellKnownTypes[index] == (object) type) return true;

                checked
                {
                    ++index;
                }
            }

            return false;
        }

        private Type ResolveType(string persistedSettingTypeName, bool caseSensitive)
        {
            var typeFromResolutionService =
                _typeResolutionService.GetType(persistedSettingTypeName, false, !caseSensitive);
            var supportedType = (object) typeFromResolutionService == null
                ? null
                : GetSupportedType(typeFromResolutionService, false);
            return supportedType;
        }


        public Type GetSupportedType(Type sourceType, bool needReflectionType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (!IsSupportedType(sourceType, null))
                return null;
            if (needReflectionType)
                return typeDescriptionProvider.GetReflectionType(sourceType);
            return typeDescriptionProvider.GetRuntimeType(sourceType);
        }

        public string TypeNameConverter(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            var assemblyQualifiedName = sourceType.AssemblyQualifiedName;
            var supportedType = GetSupportedType(sourceType, true);
            if (supportedType != null)
                assemblyQualifiedName = supportedType.AssemblyQualifiedName;
            return assemblyQualifiedName;
        }

        public string TypeTransformer(string sourceTypeName)
        {
            var str = (string) null;
            if (!string.IsNullOrEmpty(sourceTypeName))
            {
                var type = _typeResolutionService.GetType(sourceTypeName, false, !_caseSensitive);
                if ((object) type != null)
                    str = TypeNameConverter(type);
            }

            return str;
        }
    }
}