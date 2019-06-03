using System;
using System.Configuration;
using System.Threading.Tasks;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Design.Serialization;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.AppConfig
{
    internal static class AppConfigFileGenerator
    {
        /// <summary>
        ///     Writes the specified settings container.
        /// </summary>
        /// <param name="settingsContainer">The settings container.</param>
        public static async Task WriteAsync(SettingsContainer settingsContainer)
        {
            var sectionName = $"{settingsContainer.Namespace}.{settingsContainer.Name}";
            var solution = Package.Instance.GetGlobalService<SVsSolution, IVsSolution>();
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            solution.GetProjectOfUniqueName(Package.Instance.Dte.ActiveDocument.ProjectItem.ContainingProject.UniqueName,
                out var vsHierarchy);

            var appConfigDocData = await GetAppConfigDocDataAsync(Package.Instance, vsHierarchy, false);
            if (appConfigDocData == null)
                return;

            var exeConfigurationFileMap = new ExeConfigurationFileMap();
            exeConfigurationFileMap.ExeConfigFilename = appConfigDocData.Name;

            var configHelperService = new ConfigurationHelperService();

            var appScopedSettingsPropertyCollection = new SettingsPropertyCollection();
            var appScopedSettingsPropertyValueCollection = configHelperService.ReadSettings(exeConfigurationFileMap,
                ConfigurationUserLevel.None, appConfigDocData, sectionName, false,
                appScopedSettingsPropertyCollection);

            var userScopedSettingsPropertyCollection = new SettingsPropertyCollection();
            var userScopedSettingsPropertyValueCollection = configHelperService.ReadSettings(exeConfigurationFileMap,
                ConfigurationUserLevel.None, appConfigDocData, sectionName, true,
                userScopedSettingsPropertyCollection);

            foreach (var setting in settingsContainer.Settings)
            {
                var newSettingPropertyValue = new SettingsPropertyValue(new SettingsProperty(setting.Name));
                var exists = false;
                var settingsPropertyValueCollection = setting.Scope == SettingScope.Application ? appScopedSettingsPropertyValueCollection : userScopedSettingsPropertyValueCollection;

                foreach (SettingsPropertyValue settingPropertyValue in settingsPropertyValueCollection)
                {
                    if (!string.Equals(settingPropertyValue.Property.Name, newSettingPropertyValue.Property.Name, StringComparison.OrdinalIgnoreCase))
                        continue;

                    newSettingPropertyValue = settingPropertyValue;
                    exists = true;
                    break;
                }

                if (!exists)
                    settingsPropertyValueCollection.Add(newSettingPropertyValue);

                // Update
                newSettingPropertyValue.Property.DefaultValue = setting.DefaultValue;
                newSettingPropertyValue.PropertyValue = setting.Value;
                newSettingPropertyValue.SerializedValue = setting.Value;
                newSettingPropertyValue.Property.PropertyType = setting.Type;
                newSettingPropertyValue.Property.SerializeAs = SettingsSerializeAs.String;
            }

            configHelperService.WriteSettings(exeConfigurationFileMap, ConfigurationUserLevel.None, appConfigDocData,
                sectionName,
                false, appScopedSettingsPropertyValueCollection);

            configHelperService.WriteSettings(exeConfigurationFileMap, ConfigurationUserLevel.None, appConfigDocData,
                sectionName,
                true, userScopedSettingsPropertyValueCollection);
            appConfigDocData.CheckoutFile(Package.Instance);
        }

        /// <summary>
        ///     Gets the application configuration document data.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="createIfNotExists">if set to <see langword="true" /> [create if not exists].</param>
        /// <returns>The application configuration document data.</returns>
        /// <exception cref="NotSupportedException">Incompatible buffer</exception>
        public static async Task<DocData> GetAppConfigDocDataAsync(IServiceProvider serviceProvider, IVsHierarchy hierarchy,
            bool createIfNotExists)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            var projSpecialFiles = hierarchy as IVsProjectSpecialFiles;
            DocData appConfigDocData = null;

            if (projSpecialFiles != null)
            {
                var flags = createIfNotExists
                    ? Convert.ToUInt32(__PSFFLAGS.PSFF_CreateIfNotExist | __PSFFLAGS.PSFF_FullPath)
                    : Convert.ToUInt32(__PSFFLAGS.PSFF_FullPath);
                projSpecialFiles.GetFile((int) __PSFFILEID.PSFFILEID_AppConfig, flags, out var appConfigItemId,
                    out var appConfigFileName);

                if (appConfigItemId != (uint) VSConstants.VSITEMID.Nil) appConfigDocData = new DocData(serviceProvider, appConfigFileName);
            }

            if (appConfigDocData == null || appConfigDocData.Buffer != null)
                return appConfigDocData;

            // The native DocData needs to implement the IVsTextBuffer so DocDataTextReaders/Writers can be used.
            // If this is not possible, inform the user that things may be broken
            appConfigDocData.Dispose();
            throw new NotSupportedException("Incompatible buffer");
        }
    }
}