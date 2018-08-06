using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Design.Serialization;
using Microsoft.VisualStudio.Shell.Interop;
using ProductivityShell.Core.Properties;
using ProductivityShell.Core.Settings;
using Configuration = EnvDTE.Configuration;

namespace ProductivityShell.Core.AppConfig
{
    /// <summary>
    ///     Helper class for serializing and deserializing the contents of app.config files.
    /// </summary>
    public class AppConfigSerializer
    {
        public static void Deserialize(SettingsContainer settings, SettingsTypeCache typeCache,
            SettingsValueCache valueCache, string sectionName, DocData appConfigDocData)
        {
            var existingSettings = new Dictionary<string, Setting>();
            foreach (var instance in settings.Items)
                existingSettings[instance.Name] = instance;

            var configHelperService = new ConfigurationHelperService();
            var userScopedSettingProps = new SettingsPropertyCollection();
            var appScopedSettingProps = new SettingsPropertyCollection();

            foreach (var instance in existingSettings.Values)
            {
                var settingType = typeCache.GetSettingType(instance.TypeName);

                // We need a name and a type to be able to serialize
                if (string.IsNullOrEmpty(instance.Name) || settingType == null)
                    continue;

                var newSettingProperty = new SettingsProperty(instance.Name)
                {
                    PropertyType = settingType,
                    SerializeAs = configHelperService.GetSerializeAs(settingType)
                };

                if (instance.Scope == SettingScope.Application)
                    appScopedSettingProps.Add(newSettingProperty);
                else
                    userScopedSettingProps.Add(newSettingProperty);
            }

            // Deserialize "normal" app scoped and user scoped settings
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = appConfigDocData.Name
            };

            configHelperService.ReadSettings(configFileMap,
                ConfigurationUserLevel.None, appConfigDocData, sectionName, false, appScopedSettingProps);
            configHelperService.ReadSettings(configFileMap,
                ConfigurationUserLevel.None, appConfigDocData, sectionName, true, userScopedSettingProps);
        }

        internal static DocData GetAppConfigDocData(IServiceProvider serviceProvider, IVsHierarchy hierarchy,
            bool createIfNotExists, bool writeable,
            DesignerDocDataService docDataService = null)
        {
            var projSpecialFiles = hierarchy as IVsProjectSpecialFiles;
            DocData appConfigDocData = null;

            if (projSpecialFiles != null)
            {
                var flags = createIfNotExists
                    ? Convert.ToUInt32(__PSFFLAGS.PSFF_CreateIfNotExist | __PSFFLAGS.PSFF_FullPath)
                    : Convert.ToUInt32(__PSFFLAGS.PSFF_FullPath);
                projSpecialFiles.GetFile((int) __PSFFILEID.PSFFILEID_AppConfig, flags, out var appConfigItemId,
                    out var appConfigFileName);

                if (appConfigItemId != (uint) VSConstants.VSITEMID.Nil)
                {
                    if (docDataService != null)
                    {
                        var access = writeable ? FileAccess.ReadWrite : FileAccess.Read;
                        appConfigDocData = docDataService.GetFileDocData(appConfigFileName, access, null);
                    }
                    else
                    {
                        appConfigDocData = new DocData(serviceProvider, appConfigFileName);
                    }
                }
            }

            if (appConfigDocData == null || appConfigDocData.Buffer != null)
                return appConfigDocData;

            // The native DocData needs to implement the IVsTextBuffer so DocDataTextReaders/Writers can be used.
            // If this is not possible, inform the user that things may be broken
            appConfigDocData.Dispose();
            throw new NotSupportedException(Resources.AppConfigSerializer_IncompatibleBuffer);
        }

        internal static void Serialize(SettingsContainer settings, SettingsTypeCache typeCache,
            SettingsValueCache valueCache, string className, string ns, DocData appConfigDocData,
            IVsHierarchy hierarchy, bool synchronizeUserConfig)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentNullException(nameof(className));

            if (ns == null)
                throw new ArgumentNullException(nameof(ns));

            if (appConfigDocData == null)
                throw new ArgumentNullException(nameof(appConfigDocData));

            var configHelperService = new ConfigurationHelperService(typeCache.TypeTransformer);
            var fullyQualifiedClassName = TypeHelpers.FullyQualifiedClassName(ns, className);

            Serialize(settings, typeCache, valueCache,
                configHelperService.GetSectionName(fullyQualifiedClassName, string.Empty), appConfigDocData, hierarchy,
                synchronizeUserConfig);
        }

        private static void Serialize(SettingsContainer settings, SettingsTypeCache typeCache,
            SettingsValueCache valueCache, string sectionName, DocData appConfigDocData, IVsHierarchy hierarchy,
            bool shouldSynchronizeUserConfig)
        {
            // Populate user and application scoped settings respectively
            var existingUserScopedSettings = new SettingsPropertyValueCollection();
            var existingApplicationScopedSettings = new SettingsPropertyValueCollection();
            var existingConnectionStringSettings = new ConnectionStringSettingsCollection();
            var configHelperService = new ConfigurationHelperService(typeCache.TypeTransformer);

            foreach (var instance in settings.Items)
            {
                var settingType = typeCache.GetSettingType(instance.TypeName);

                object settingValue = null;
                if (settingType != null)
                    settingValue = valueCache.GetValue(settingType, instance.Value);

                if (string.IsNullOrWhiteSpace(instance.Name) || settingType == null || settingValue == null)
                    continue;

                var settingsProperty = new SettingsProperty(instance.Name);
                settingsProperty.PropertyType = settingType;
                settingsProperty.SerializeAs = configHelperService.GetSerializeAs(settingsProperty.PropertyType);

                var propertyValue = new SettingsPropertyValue(settingsProperty);
                propertyValue.SerializedValue = instance.Value;

                if (instance.Scope == SettingScope.Application)
                    existingApplicationScopedSettings.Add(propertyValue);
                else
                    existingUserScopedSettings.Add(propertyValue);
            }

            // Let us populate the settings with values from app.config file
            var configFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = appConfigDocData.Name
            };
            configHelperService.WriteConnectionStrings(appConfigDocData.Name, appConfigDocData, sectionName,
                existingConnectionStringSettings);
            configHelperService.WriteSettings(configFileMap, ConfigurationUserLevel.None, appConfigDocData, sectionName,
                true, existingUserScopedSettings);
            configHelperService.WriteSettings(configFileMap, ConfigurationUserLevel.None, appConfigDocData, sectionName,
                false, existingApplicationScopedSettings);

            if (!shouldSynchronizeUserConfig)
                return;

            try
            {
                SynchronizeUserConfig(sectionName, hierarchy, configHelperService, settings, appConfigDocData);
            }
            catch
            {
                // Nothing
            }
        }

        private static void SynchronizeUserConfig(string sectionName, IVsHierarchy hierarchy,
            ConfigurationHelperService configHelperService, SettingsContainer settings, DocData appConfigDocData)
        {
            // List all the user-scoped settings that we know about and set the value to true or false depending
            // on if the setting is roaming
            var settingsTheDesignerKnownsAbout = new Dictionary<string, bool>();

            foreach (var setting in settings.Items)
                if (setting.Scope == SettingScope.User)
                    settingsTheDesignerKnownsAbout[setting.Name] = setting.Roaming;

            SynchronizeUserConfig(sectionName, hierarchy, configHelperService, settingsTheDesignerKnownsAbout,
                appConfigDocData);
        }

        /// <summary>
        ///     Synchronize (remove) any entries in user.config that the settings designer doesn't consider to be part of this
        ///     settings class's user scoped settings collection. Checks either the set of files used when running under VSHost or
        ///     when running "stand-alone"
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="hierarchy"></param>
        /// <param name="configHelperService"></param>
        /// <param name="settingsTheDesignerKnownsAbout"></param>
        /// <param name="appConfigDocData"></param>
        /// <remarks></remarks>
        private static void SynchronizeUserConfig(string sectionName, IVsHierarchy hierarchy,
            ConfigurationHelperService configHelperService, Dictionary<string, bool> settingsTheDesignerKnownsAbout,
            DocData appConfigDocData)
        {
            var serviceProvider = hierarchy?.GetServiceProvider();
            var project = hierarchy.GetProject();

            foreach (Configuration buildConfig in project.ConfigurationManager)
            {
                // Grab the relevant file locations
                var map = new ExeConfigurationFileMap();
                try
                {
                    map.ExeConfigFilename = appConfigDocData.Name;
                    map.LocalUserConfigFilename = configHelperService.GetUserConfigurationPath(serviceProvider, project,
                        ConfigurationUserLevel.PerUserRoamingAndLocal, underHostingProcess: false,
                        buildConfiguration: buildConfig);
                    map.RoamingUserConfigFilename = configHelperService.GetUserConfigurationPath(serviceProvider,
                        project, ConfigurationUserLevel.PerUserRoaming, underHostingProcess: false,
                        buildConfiguration: buildConfig);
                }
                catch
                {
                    // Can't really do anything, synchronize will fail...
                    return;
                }

                // We can't scrub the directories if we can't get the directory names
                if (map.LocalUserConfigFilename == null || map.RoamingUserConfigFilename == null)
                    return;

                // If we find a setting in the merged view that the designer doesn't know anything about,
                // we need to "scrub" the user.config files...
                var scrubNeeded = false;

                // While we are going through this, add the existing settings to a local and roaming user
                // collection depending on if the roaming flag is set...
                var localUserSettings = new SettingsPropertyValueCollection();
                var roamingUserSettings = new SettingsPropertyValueCollection();

                // If the settings designer doesn't know anything about any settings, then there is a very high risk that there
                // isn't a section handler declared for the user scoped settings for this class. That means that the configuration
                // system won't find any settings in local/roaming user config files, even if they are in there (it will only find
                // settings if the section handler is declared)
                // We get around this by "scrubbing" the local/roaming user user.config files by serializing an empty
                // SettingsPropertyValueCollection, since that effectively removes any old garbage that may be in there...
                if (settingsTheDesignerKnownsAbout.Count == 0)
                {
                    scrubNeeded = true;
                }
                else
                {
                    // If we have one or more settings, the section handler should have been added in app.config, so we should
                    // be able to find any old settings hanging around in the user.config file
                    var mergedViewSettings = configHelperService.ReadSettings(map,
                        ConfigurationUserLevel.PerUserRoamingAndLocal, appConfigDocData, sectionName, true,
                        new SettingsPropertyCollection());

                    // Strip out any and all settings that are not included in our "known" set of settings...
                    foreach (SettingsPropertyValue prop in mergedViewSettings)
                        // Unknown setting, need to scrub
                        if (!settingsTheDesignerKnownsAbout.TryGetValue(prop.Name, out var roaming))
                            scrubNeeded = true;
                        else
                            // Known setting - put it in the appropriate bucket of settings in case we need to
                            // scrub later on...
                        if (roaming)
                            roamingUserSettings.Add(prop);
                        else
                            localUserSettings.Add(prop);
                }

                if (scrubNeeded)
                {
                    // The "scrub" is really only us writing out the appropriate set of settingpropertyvalues. Anything that
                    // we didn't know about should already have been remove by now...
                    configHelperService.WriteSettings(map, ConfigurationUserLevel.PerUserRoaming, appConfigDocData,
                        sectionName, true, roamingUserSettings);
                    configHelperService.WriteSettings(map, ConfigurationUserLevel.PerUserRoamingAndLocal,
                        appConfigDocData, sectionName, true, localUserSettings);
                }
            }
        }

        private static XmlDocument LoadAppConfigDocument(TextReader reader)
        {
            var appConfigXmlDoc = new XmlDocument
            {
                PreserveWhitespace = false
            };

            // Load App.Config into XML Doc
            var xmlAppConfigReader = new XmlTextReader(reader)
            {
                DtdProcessing = DtdProcessing.Prohibit,
                WhitespaceHandling = WhitespaceHandling.All
            };
            appConfigXmlDoc.Load(xmlAppConfigReader);
            return appConfigXmlDoc;
        }
    }
}