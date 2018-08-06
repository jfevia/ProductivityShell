using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Core.AppConfig;
using ProductivityShell.Core.Settings;
using ProductivityShell.Helpers;
using ProductivityShell.Shell;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ProductivityShell.Commands.TextEditor
{
    internal sealed class MoveToSettingsCommand : CommandBase<MoveToSettingsCommand>
    {
        private readonly Dictionary<Type, int> _defaultTypes;
        private readonly List<string> _scopes;
        private string _extension;
        private string _projectPath;
        private List<EnvDTE.ProjectItem> _settingsFiles;
        private TextSelection _textSelection;
        private const string SettingsFileExtension = ".settings";

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:ProductivityShell.Commands.TextEditor.MoveToSettingsCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private MoveToSettingsCommand(PackageBase package)
            : base(package, PackageIds.TextEditorMoveToSettingsCommand)
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
            _scopes = new List<string>
            {
                "Application",
                "User"
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
            command.Visible = false;

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

            command.Visible = true;
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

            window.Scopes = new ObservableCollection<string>(_scopes);
            window.SelectedScope = _scopes.FirstOrDefault();

            var settingsFiles = new List<SettingsFile>(GetDetectedSettingsFiles());
            window.Settings = new ObservableCollection<SettingsFile>(settingsFiles);
            window.SelectedSettings = settingsFiles.FirstOrDefault();

            if (window.ShowModal() != true)
                return;

            ApplyChanges(window.SettingsName, window.Value, window.SelectedSettings, window.SelectedType,
                window.SelectedScope);
        }

        /// <summary>
        ///     Applies the changes.
        /// </summary>
        /// <param name="settingsName">Name of the settings.</param>
        /// <param name="value">The value.</param>
        /// <param name="settingsFile">The settings file.</param>
        /// <param name="type">The type.</param>
        /// <param name="scope">The scope.</param>
        private void ApplyChanges(string settingsName, string value, SettingsFile settingsFile, Type type, string scope)
        {
            var replacementContent = GetReplacementContent(_extension, settingsName);
            if (replacementContent == null)
                return;

            using (var settingsSerializer = new SettingsSerializer(settingsFile.FullPath))
            {
                settingsSerializer.AddOrUpdate(settingsName, type.FullName, scope, value);
                settingsSerializer.Save();
                FlushAppConfig();
            }

            var startEditPoint = _textSelection.TopPoint.CreateEditPoint();
            var endEditPoint = _textSelection.BottomPoint.CreateEditPoint();

            startEditPoint.Delete(endEditPoint);
            startEditPoint.Insert(replacementContent);
        }

        /// <summary>
        /// Flushes the application configuration.
        /// </summary>
        private void FlushAppConfig(SettingsContainer  settingsContainer)
        {
            if (!AttachAppConfigDocData(true))
                return;

            try
            {
                var service = Package.QueryService<IVSMDCodeDomProvider>();
                AppConfigSerializer.Serialize(settingsContainer,
                    Package.GetService< SettingsTypeCache>(),
                    Package.GetService< SettingsValueCache>(), GeneratedClassName,
                    GeneratedClassNamespace(true), _appConfigDocData, VsHierarchy, true);
            }
            catch
            {
                // Failed to flush values to the app.config document
                MessageBox.Show("An error occurred while saving values to the app.config file. The file might be corrupted or contain invalid XML.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AttachAppConfigDocData(bool createIfNotExist)
        {
            var appConfigDocData = AppConfigSerializer.GetAppConfigDocData(VBPackage.Instance, VsHierarchy, createIfNotExist, false, DocDataService);
            if (appConfigDocData != null)
                appConfigDocData.DataChanged += ExternalChange;
            return appConfigDocData != null;
        }

        /// <summary>
        ///     Gets the detected settings files.
        /// </summary>
        /// <returns>The detected settings files.</returns>
        private IEnumerable<SettingsFile> GetDetectedSettingsFiles()
        {
            foreach (var settingsFile in _settingsFiles)
            {
                for (short index = 0; index < settingsFile.FileCount; ++index)
                {
                    var filePath = settingsFile.FileNames[index];
                    var relativePath = filePath.Replace(_projectPath, $"<{settingsFile.ContainingProject.Name}>");
                    yield return new SettingsFile
                    {
                        FullPath = filePath,
                        RelativePath = relativePath
                    };
                }
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
        private static string GetReplacementContent(string extension, string entryName)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".xaml":
                    return $"{{Binding Path={entryName}, Source={{x:Static properties:Settings.Default}}}}";
                case ".cs":
                case ".vb":
                    return $"Settings.Default.{entryName}";
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Determines whether this instance can move the selection.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        ///     <c>true</c> if this instance can move the selection; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanMove(string extension)
        {
            switch (extension)
            {
                case ".xaml":
                case ".cs":
                case ".vb":
                    return true;
                default:
                    return false;
            }
        }
    }
}