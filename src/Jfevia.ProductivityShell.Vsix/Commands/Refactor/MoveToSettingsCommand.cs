﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Jfevia.ProductivityShell.Vsix.AppConfig;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Helpers;
using Jfevia.ProductivityShell.Vsix.Settings;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using SettingScope = Jfevia.ProductivityShell.Vsix.Settings.SettingScope;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Refactor
{
    internal sealed class MoveToSettingsCommand : CommandBase<MoveToSettingsCommand>
    {
        private const string SettingsFileExtension = ".settings";
        private readonly Dictionary<Type, int> _defaultTypes;

        private readonly Dictionary<string, Func<string, string>> _replacementActionByExtension;
        private readonly List<SettingScope> _scopes;
        private string _extension;
        private string _projectPath;
        private List<EnvDTE.ProjectItem> _settingsFiles;
        private TextSelection _textSelection;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Refactor.MoveToSettingsCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private MoveToSettingsCommand(PackageBase package)
            : base(package, PackageCommands.RefactorMoveToSettingsCommand)
        {
            _settingsFiles = new List<EnvDTE.ProjectItem>();
            _defaultTypes = new Dictionary<Type, int>
            {
                {typeof(bool), 10},
                {typeof(char), 0},
                {typeof(decimal), 0},
                {typeof(double), 0},
                {typeof(float), 0},
                {typeof(int), 10},
                {typeof(long), 0},
                {typeof(sbyte), 0},
                {typeof(short), 0},
                {typeof(string), 10},
                {typeof(StringCollection), 0},
                {typeof(DateTime), 0},
                {typeof(Color), 0},
                {typeof(Font), 0},
                {typeof(Point), 0},
                {typeof(Size), 0},
                {typeof(Guid), 0},
                {typeof(TimeSpan), 0},
                {typeof(uint), 0},
                {typeof(ulong), 0},
                {typeof(ushort), 0}
            };
            _scopes = new List<SettingScope>
            {
                SettingScope.Application,
                SettingScope.User
            };
            _replacementActionByExtension = new Dictionary<string, Func<string, string>>
            {
                {".xaml", settingName => $"{{Binding Path={settingName}, Source={{x:Static properties:Settings.Default}}}}"},
                {".cs", settingName => $"Settings.Default.{settingName}"}
            };
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new MoveToSettingsCommand(package);
            await Instance.InitializeAsync();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnBeforeQueryStatusAsync(OleMenuCommand command)
        {
            command.Enabled = false;

            _extension = null;
            _projectPath = null;
            _settingsFiles.Clear();

            if (Package.Dte.ActiveDocument == null)
                return;

            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            var extension = Path.GetExtension(Package.Dte.ActiveDocument.FullName);
            if (!CanMove(extension))
                return;

            if (!(Package.Dte.ActiveDocument.Selection is TextSelection textSelection))
                return;

            var project = Package.Dte.ActiveDocument.ProjectItem.ContainingProject;
            if (project == null)
                return;

            if (string.IsNullOrWhiteSpace(project.FullName))
                return;

            var projectPath = Path.GetDirectoryName(project.FullName);
            if (string.IsNullOrWhiteSpace(projectPath))
                return;

            _textSelection = textSelection;
            _projectPath = projectPath;
            _settingsFiles = new List<EnvDTE.ProjectItem>(await GetSettingsProjectItemsAsync(project));
            _extension = extension;

            command.Enabled = true;

            await Task.Yield();
        }

        /// <summary>
        ///     Gets the settings project items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The project items for settings.</returns>
        private static async Task<IEnumerable<EnvDTE.ProjectItem>> GetSettingsProjectItemsAsync(EnvDTE.Project project)
        {
            var projectItems = await SolutionHelper.GetItemsRecursivelyAsync<EnvDTE.ProjectItem>(project);
            var items = new List<EnvDTE.ProjectItem>();
            foreach (var projectItem in projectItems)
            {
                await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (projectItem.Name.EndsWith(SettingsFileExtension))
                    items.Add(projectItem);
            }

            return items;
        }

        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnExecuteAsync(OleMenuCommand command)
        {
            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            var window = new MoveToSettingsWindow();
            window.SettingsName = "NewSettings";
            window.Value = _textSelection.Text;

            window.Types = new ObservableCollection<Type>(_defaultTypes.Keys);
            window.SelectedType = GetDetectedType(_textSelection.Text);

            window.Scopes = new ObservableCollection<SettingScope>(_scopes);
            window.SelectedScope = _scopes.FirstOrDefault();

            var settingsFiles = new List<SettingsFile>(await GetDetectedSettingsFilesAsync());
            window.Settings = new ObservableCollection<SettingsFile>(settingsFiles);
            window.SelectedSettings = settingsFiles.FirstOrDefault();

            if (window.ShowModal() != true)
                return;

            var setting = new Setting();
            setting.Name = window.SettingsName;
            setting.Scope = window.SelectedScope;
            setting.Type = window.SelectedType;
            setting.Value = window.Value;
            setting.DefaultValue = window.Value;
            //setting.Profile = window.Profile;

            await AddOrUpdateAsync(window.SelectedSettings, setting);
        }

        /// <summary>
        ///     Called when [change].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnChangeAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }

        /// <summary>
        ///     Adds or updates the specified setting.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="setting">The setting.</param>
        private async Task AddOrUpdateAsync(SettingsFile file, Setting setting)
        {
            var replacementContent = GetReplacementContent(_extension, setting.Name);
            if (replacementContent == null)
                return;

            // Read the settings
            var serializedContainer = File.ReadAllText(file.FullPath);
            var settingsContainer = SettingsSerializer.Deserialize(serializedContainer);

            // Let's add the new entry
            settingsContainer.AddOrUpdate(setting);

            // ... and write the settings again
            serializedContainer = SettingsSerializer.Serialize(settingsContainer);
            File.WriteAllText(file.FullPath, serializedContainer);

            SettingsFileGenerator.Write(file, settingsContainer);
            await AppConfigFileGenerator.WriteAsync(settingsContainer);

            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            var startEditPoint = _textSelection.TopPoint.CreateEditPoint();
            var endEditPoint = _textSelection.BottomPoint.CreateEditPoint();

            startEditPoint.Delete(endEditPoint);
            startEditPoint.Insert(replacementContent);
        }

        /// <summary>
        ///     Gets the detected settings files.
        /// </summary>
        /// <returns>The detected settings files.</returns>
        private async Task<IEnumerable<SettingsFile>> GetDetectedSettingsFilesAsync()
        {
            var items = new List<SettingsFile>();
            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            foreach (var settingsFile in _settingsFiles)
            {
                var language = settingsFile.ContainingProject.CodeModel.Language.ToLanguage();
                var defaultExtension = language.ToDefaultExtension();
                for (short index = 0; index < settingsFile.FileCount; ++index)
                {
                    var filePath = settingsFile.FileNames[index];
                    var relativePath = filePath.Replace(_projectPath, $"<{settingsFile.ContainingProject.Name}>");
                    var directoryName = Path.GetDirectoryName(filePath);

                    Debug.Assert(directoryName != null);

                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var designerFileName = $"{fileName}.Designer{defaultExtension}";
                    var designerFilePath = Path.Combine(directoryName, designerFileName);

                    items.Add(new SettingsFile
                    {
                        FullPath = filePath,
                        RelativePath = relativePath,
                        DirectoryName = directoryName,
                        FileName = fileName,
                        DesignerFileName = designerFileName,
                        DesignerFilePath = designerFilePath,
                        Language = language
                    });
                }
            }

            return items;
        }

        /// <summary>
        ///     Gets the type of the detected.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The type.</returns>
        private Type GetDetectedType(string value)
        {
            foreach (var kvp in _defaultTypes.OrderByDescending(s => s.Value))
            {
                var converter = TypeDescriptor.GetConverter(kvp.Key);
                if (!converter.CanConvertFrom(typeof(string)))
                    continue;

                try
                {
                    var newValue = converter.ConvertFromInvariantString(value);
                    if (newValue != null)
                        return kvp.Key;
                }
                catch
                {
                    // Nothing
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the content of the replacement.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="entryName">Name of the entry.</param>
        /// <returns>The content of the replacement.</returns>
        /// <exception cref="ArgumentOutOfRangeException">extension</exception>
        private string GetReplacementContent(string extension, string entryName)
        {
            extension = extension.ToLower();
            return _replacementActionByExtension[extension](entryName);
        }

        /// <summary>
        ///     Determines whether this instance can move the selection.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        ///     <c>true</c> if this instance can move the selection; otherwise, <c>false</c>.
        /// </returns>
        private bool CanMove(string extension)
        {
            extension = extension.ToLower();
            return _replacementActionByExtension.ContainsKey(extension);
        }
    }
}