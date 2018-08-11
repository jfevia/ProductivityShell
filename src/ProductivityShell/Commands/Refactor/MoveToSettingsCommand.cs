using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.AppConfig;
using ProductivityShell.Helpers;
using ProductivityShell.Settings;
using ProductivityShell.Shell;
using SettingScope = ProductivityShell.Settings.SettingScope;

namespace ProductivityShell.Commands.Refactor
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
        ///     <see cref="T:ProductivityShell.Commands.Refactor.MoveToSettingsCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private MoveToSettingsCommand(PackageBase package)
            : base(package, PackageIds.RefactorMoveToSettingsCommand)
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
        public static void Initialize(PackageBase package)
        {
            Instance = new MoveToSettingsCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnBeforeQueryStatus(OleMenuCommand command)
        {
            command.Enabled = false;

            _extension = null;
            _projectPath = null;
            _settingsFiles.Clear();

            if (Package.Dte.ActiveDocument == null)
                return;

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
            _settingsFiles = new List<EnvDTE.ProjectItem>(GetSettingsProjectItems(project));
            _extension = extension;

            command.Enabled = true;
        }

        /// <summary>
        ///     Gets the settings project items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>The project items for settings.</returns>
        private static IEnumerable<EnvDTE.ProjectItem> GetSettingsProjectItems(EnvDTE.Project project)
        {
            return SolutionHelper.GetItemsRecursively<EnvDTE.ProjectItem>(project)
                .Where(s => s.Name.EndsWith(SettingsFileExtension));
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            var window = new MoveToSettingsWindow();
            window.SettingsName = "NewSettings";
            window.Value = _textSelection.Text;

            window.Types = new ObservableCollection<Type>(_defaultTypes.Keys);
            window.SelectedType = GetDetectedType(_textSelection.Text);

            window.Scopes = new ObservableCollection<SettingScope>(_scopes);
            window.SelectedScope = _scopes.FirstOrDefault();

            var settingsFiles = new List<SettingsFile>(GetDetectedSettingsFiles());
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

            AddOrUpdate(window.SelectedSettings, setting);
        }

        /// <summary>
        ///     Adds or updates the specified setting.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="setting">The setting.</param>
        private void AddOrUpdate(SettingsFile file, Setting setting)
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

            SettingsFileGenerator.Write(file.DesignerFilePath, settingsContainer);
            AppConfigFileGenerator.Write(settingsContainer);

            var startEditPoint = _textSelection.TopPoint.CreateEditPoint();
            var endEditPoint = _textSelection.BottomPoint.CreateEditPoint();

            startEditPoint.Delete(endEditPoint);
            startEditPoint.Insert(replacementContent);
        }

        /// <summary>
        ///     Gets the detected settings files.
        /// </summary>
        /// <returns>The detected settings files.</returns>
        private IEnumerable<SettingsFile> GetDetectedSettingsFiles()
        {
            foreach (var settingsFile in _settingsFiles)
                for (short index = 0; index < settingsFile.FileCount; ++index)
                {
                    var filePath = settingsFile.FileNames[index];
                    var relativePath = filePath.Replace(_projectPath, $"<{settingsFile.ContainingProject.Name}>");
                    var directoryName = Path.GetDirectoryName(filePath);
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var designerFileName = $"{fileName}.Designer.cs";
                    var designerFilePath = Path.Combine(directoryName, designerFileName);

                    yield return new SettingsFile
                    {
                        FullPath = filePath,
                        RelativePath = relativePath,
                        DirectoryName = directoryName,
                        FileName = fileName,
                        DesignerFileName = designerFileName,
                        DesignerFilePath = designerFilePath
                    };
                }
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
        /// <returns></returns>
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