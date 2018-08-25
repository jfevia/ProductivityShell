using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Jfevia.ProductivityShell.Configuration;
using Jfevia.ProductivityShell.SolutionModel;
using Jfevia.ProductivityShell.Vsix.Configuration;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Projects;
using Jfevia.ProductivityShell.Vsix.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Constants = EnvDTE.Constants;
using Project = Jfevia.ProductivityShell.Configuration.Project;
using Solution = Jfevia.ProductivityShell.SolutionModel.Solution;

namespace Jfevia.ProductivityShell.Vsix.Solutions
{
    public abstract class SolutionProxyBase
    {
        private IVsSolution2 _solution2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SolutionProxyBase" /> class.
        /// </summary>
        /// <param name="vsProxy">The Visual Studio proxy.</param>
        protected SolutionProxyBase(VisualStudioProxy vsProxy)
        {
            VsProxy = vsProxy;
        }

        /// <summary>
        ///     Gets the Visual Studio proxy.
        /// </summary>
        /// <value>
        ///     The Visual Studio proxy.
        /// </value>
        public VisualStudioProxy VsProxy { get; }

        /// <summary>
        ///     Gets the Visual Studio solution.
        /// </summary>
        /// <value>
        ///     The Visual Studio solution.
        /// </value>
        public IVsSolution2 Solution2 => _solution2 ?? (_solution2 = VsProxy.Package.GetService<SVsSolution, IVsSolution2>());
    }

    public class SolutionProxy : SolutionProxyBase, IDisposable
    {
        private const string LocalExtension = ".ProductivityShell";
        private const string UserExtension = ".ProductivityShell.user";
        private readonly List<ConfigurationFileTracker> _configurationFileTrackers;
        private readonly ProjectCache _projectCache;
        private readonly Solution _psSolution;
        private readonly EnvDTE.Solution _vsSolution;
        private uint _solutionEventsCookie;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Solutions.SolutionProxy" /> class.
        /// </summary>
        /// <param name="vsProxy">The Visual Studio proxy.</param>
        /// <param name="psSolution">The Productivity Shell solution.</param>
        public SolutionProxy(VisualStudioProxy vsProxy, Solution psSolution)
            : base(vsProxy)
        {
            _configurationFileTrackers = new List<ConfigurationFileTracker>();
            _projectCache = new ProjectCache();
            _vsSolution = VsProxy.VsSolution;
            _psSolution = psSolution;

            Initialize();
        }

        /// <summary>
        ///     Occurs when [closing project].
        /// </summary>
        public event EventHandler<ProjectEventArgs> ClosingProject;

        /// <summary>
        ///     Occurs when [configuration changed].
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        /// <summary>
        ///     Occurs when [current startup projects changed].
        /// </summary>
        public event EventHandler<StartupProjectsEventArgs> CurrentStartupProjectsChanged;

        /// <summary>
        ///     Occurs when [opened].
        /// </summary>
        public event EventHandler<SolutionEventArgs> Opened;

        /// <summary>
        ///     Occurs when [opened project].
        /// </summary>
        public event EventHandler<ProjectEventArgs> OpenedProject;

        /// <summary>
        ///     Gets the machine configuration file path.
        /// </summary>
        /// <value>
        ///     The machine configuration file path.
        /// </value>
        public string MachineConfigFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jfevia", "ProductivityShell", "Shared", "vAny", $"{Path.GetFileName(_vsSolution.FullName)}{LocalExtension}");

        /// <summary>
        ///     Gets the shared configuration file path.
        /// </summary>
        /// <value>
        ///     The shared configuration file path.
        /// </value>
        public string SharedConfigFilePath => $"{_vsSolution.FullName}{LocalExtension}";

        /// <summary>
        ///     Gets the user configuration file path.
        /// </summary>
        /// <value>
        ///     The user configuration file path.
        /// </value>
        public string UserConfigFilePath => $"{_vsSolution.FullName}{UserExtension}";

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_psSolution == null)
                return;

            _psSolution.QueryCurrentStartupProjects -= Solution_QueryCurrentStartupProjects;
            _psSolution.QueryStartupProjects -= Solution_QueryStartupProjects;
            _psSolution.QueryConfiguration -= Solution_QueryConfiguration;
            _psSolution.ParseConfiguration -= Solution_ParseConfiguration;
            _psSolution.CurrentStartupProjectsChanged -= Solution_CurrentStartupProjectsChanged;
            _psSolution.Opened -= Solution_Opened;
            _psSolution.ClosingProject -= Solution_ClosingProject;
            _psSolution.OpenedProject -= Solution_OpenedProject;
            _psSolution.ConfigurationChanged -= Solution_ConfigurationChanged;
        }

        /// <summary>
        ///     Handles the ConfigurationChanged event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        private void Solution_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            ConfigurationChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Handles the OpenedProject event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProjectEventArgs" /> instance containing the event data.</param>
        private void Solution_OpenedProject(object sender, ProjectEventArgs e)
        {
            OpenedProject?.Invoke(this, e);
        }

        /// <summary>
        ///     Handles the QueryStartupProjects event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupProjectsEventArgs" /> instance containing the event data.</param>
        private void Solution_QueryStartupProjects(object sender, StartupProjectsEventArgs e)
        {
            foreach (var startupProject in GetStartupProjects())
                e.StartupProjects.Add(startupProject);
        }

        /// <summary>
        ///     Gets the startup projects.
        /// </summary>
        /// <returns>The startup projects.</returns>
        private IEnumerable<string> GetStartupProjects()
        {
            return _projectCache.GetProjects().Select(s => s.Name);
        }

        /// <summary>
        ///     Handles the Opened event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SolutionEventArgs" /> instance containing the event data.</param>
        private void Solution_Opened(object sender, SolutionEventArgs e)
        {
            Opened?.Invoke(this, e);
        }

        /// <summary>
        ///     Handles the QueryCurrentStartupProjects event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CurrentStartupProjectsEventArgs" /> instance containing the event data.</param>
        private void Solution_QueryCurrentStartupProjects(object sender, CurrentStartupProjectsEventArgs e)
        {
            e.StartupProjects.Clear();

            foreach (var startupProject in GetCurrentStartupProjects())
                e.StartupProjects.Add(startupProject);
        }

        /// <summary>
        ///     Called when [startup project changed].
        /// </summary>
        public void OnStartupProjectChanged()
        {
            var startupProjects = GetStartupProjects();
            var currentStartupProjects = GetCurrentStartupProjects();
            var configurationLayer = LoadConfigurationLayer();
            var parsedConfiguration = ParseConfiguration(configurationLayer.ComputedConfig.Startup.Profiles, startupProjects, currentStartupProjects.ToArray());
            OnStartupProjectChanged(parsedConfiguration.CurrentProfile);
        }

        /// <summary>
        ///     Gets the current startup projects.
        /// </summary>
        /// <returns>The current startup projects.</returns>
        private IEnumerable<string> GetCurrentStartupProjects()
        {
            if (_vsSolution.SolutionBuild.StartupProjects == null)
                return Enumerable.Empty<string>();

            return ((object[]) _vsSolution.SolutionBuild.StartupProjects).Cast<string>();
        }

        /// <summary>
        ///     Called when [startup project changed].
        /// </summary>
        /// <param name="profile">The profile.</param>
        public void OnStartupProjectChanged(Profile profile)
        {
            var startupProjects = profile.Projects.Select(s => _projectCache.TryGetProjectByName(s.Name, out var project) ? project : null).ToList();
            foreach (var startupProject in startupProjects.Where(s => s != null))
            {
                var startupProjectConfig = profile.Projects.FirstOrDefault(s => string.Equals(s.Name, startupProject.Name, StringComparison.OrdinalIgnoreCase));
                if (startupProjectConfig == null)
                    continue;

                foreach (EnvDTE.Configuration configuration in startupProject.VsProject.ConfigurationManager)
                {
                    if (configuration?.Properties == null)
                        continue;

                    foreach (var property in configuration.Properties.Cast<Property>())
                    {
                        if (property == null)
                            continue;

                        switch (property.Name)
                        {
                            case "StartArguments":
                                property.Value = startupProjectConfig.CommandLineArgs;
                                break;
                            case "StartWorkingDirectory":
                                property.Value = startupProjectConfig.WorkingDirectory;
                                break;
                            case "StartProgram":
                                property.Value = startupProjectConfig.StartExternalProgram;
                                break;
                            case "StartURL":
                                property.Value = startupProjectConfig.StartBrowserUrl;
                                break;
                            case "RemoteDebugEnabled":
                                property.Value = startupProjectConfig.IsRemoteDebuggingEnabled;
                                break;
                            case "RemoteDebugMachine":
                                property.Value = startupProjectConfig.RemoteDebuggingMachine;
                                break;
                            case "StartAction":
                                property.Value = (int) startupProjectConfig.GetStartAction();
                                break;
                        }
                    }
                }
            }

            _psSolution.OnStartupProjectsChanged(profile, startupProjects.Select(s => s.RelativePath).ToArray());
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _psSolution.QueryCurrentStartupProjects += Solution_QueryCurrentStartupProjects;
            _psSolution.QueryStartupProjects += Solution_QueryStartupProjects;
            _psSolution.QueryConfiguration += Solution_QueryConfiguration;
            _psSolution.ParseConfiguration += Solution_ParseConfiguration;
            _psSolution.CurrentStartupProjectsChanged += Solution_CurrentStartupProjectsChanged;
            _psSolution.Opened += Solution_Opened;
            _psSolution.ClosingProject += Solution_ClosingProject;
            _psSolution.OpenedProject += Solution_OpenedProject;
            _psSolution.ConfigurationChanged += Solution_ConfigurationChanged;
        }

        /// <summary>
        ///     Handles the ClosingProject event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SolutionEventArgs" /> instance containing the event data.</param>
        private void Solution_ClosingProject(object sender, ProjectEventArgs e)
        {
            ClosingProject?.Invoke(this, e);
        }

        /// <summary>
        ///     Handles the CurrentStartupProjectsChanged event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupProjectsEventArgs" /> instance containing the event data.</param>
        private void Solution_CurrentStartupProjectsChanged(object sender, StartupProjectsEventArgs e)
        {
            var projects = e.StartupProjects.Cast<object>().ToArray();
            _vsSolution.SolutionBuild.StartupProjects = projects.Length == 1 ? projects[0] : projects;
            CurrentStartupProjectsChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Handles the ParseConfiguration event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ParseConfigurationEventArgs" /> instance containing the event data.</param>
        private void Solution_ParseConfiguration(object sender, ParseConfigurationEventArgs e)
        {
            e.ParsedConfiguration = ParseConfiguration(e.Profiles, e.StartupProjects, e.SelectedStartupProjects);
        }

        /// <summary>
        ///     Parses the configuration.
        /// </summary>
        /// <param name="profiles">The profiles.</param>
        /// <param name="startupProjects">The startup projects.</param>
        /// <param name="currentStartupProjects">The current startup projects.</param>
        /// <returns>The parsed configuration.</returns>
        private ParsedConfiguration ParseConfiguration(IEnumerable<Profile> profiles, IEnumerable<string> startupProjects, ICollection<string> currentStartupProjects)
        {
            var parsedConfiguration = new ParsedConfiguration();

            // List all projects
            foreach (var startupProject in startupProjects)
            {
                var profile = new Profile();
                profile.DisplayName = startupProject;

                var projectItem = new Project();
                projectItem.Name = startupProject;

                profile.Projects.Add(projectItem);
                parsedConfiguration.Profiles.Add(profile);
            }

            var currentProfile = new Profile();
            foreach (var startupProject in currentStartupProjects)
            {
                var projectName = Path.GetFileNameWithoutExtension(startupProject);
                if (!_projectCache.TryGetProjectByName(projectName, out var projectProxy))
                    continue;

                var projectItem = new Project();
                projectItem.Name = projectProxy.Name;

                foreach (EnvDTE.Configuration configuration in projectProxy.VsProject.ConfigurationManager)
                {
                    if (configuration?.Properties == null)
                        continue;

                    foreach (var property in configuration.Properties.Cast<Property>())
                    {
                        if (property == null)
                            continue;

                        switch (property.Name)
                        {
                            case "StartArguments":
                                projectItem.CommandLineArgs = Convert.ToString(property.Value);
                                break;
                            case "StartWorkingDirectory":
                                projectItem.WorkingDirectory = Convert.ToString(property.Value);
                                break;
                            case "StartProgram":
                                projectItem.StartExternalProgram = Convert.ToString(property.Value);
                                break;
                            case "StartURL":
                                projectItem.StartBrowserUrl = Convert.ToString(property.Value);
                                break;
                            case "RemoteDebugEnabled":
                                projectItem.IsRemoteDebuggingEnabled = Convert.ToBoolean(property.Value);
                                break;
                            case "RemoteDebugMachine":
                                projectItem.RemoteDebuggingMachine = Convert.ToString(property.Value);
                                break;
                        }
                    }
                }

                currentProfile.Projects.Add(projectItem);
            }

            // List custom configuration
            foreach (var profile in profiles)
                parsedConfiguration.Profiles.Add(profile);

            // Identify current configuration
            var profileScores = ProfileScore.Generate(parsedConfiguration.Profiles, currentProfile);
            var bestMatch = profileScores.OrderByDescending(s => s.Score).FirstOrDefault();

            parsedConfiguration.CurrentProfile = bestMatch?.Configuration;
            return parsedConfiguration;
        }

        /// <summary>
        ///     Handles the QueryConfiguration event of the Solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ConfigurationEventArgs" /> instance containing the event data.</param>
        private void Solution_QueryConfiguration(object sender, ConfigurationEventArgs e)
        {
            e.ConfigurationLayer = LoadConfigurationLayer();
        }

        /// <summary>
        ///     Loads the configuration layer.
        /// </summary>
        /// <returns>The configuration layer.</returns>
        private ConfigurationLayer LoadConfigurationLayer()
        {
            var configurationLayer = new ConfigurationLayer();
            configurationLayer.MachineFilePath = MachineConfigFilePath;
            configurationLayer.SharedFilePath = SharedConfigFilePath;
            configurationLayer.UserFilePath = UserConfigFilePath;

            if (File.Exists(configurationLayer.MachineFilePath))
                configurationLayer.MachineConfig = ConfigurationXmlSerializer.Deserialize(File.OpenRead(configurationLayer.MachineFilePath));

            if (File.Exists(configurationLayer.SharedFilePath))
                configurationLayer.SharedConfig = ConfigurationXmlSerializer.Deserialize(File.OpenRead(configurationLayer.SharedFilePath));

            if (File.Exists(configurationLayer.UserFilePath))
                configurationLayer.UserConfig = ConfigurationXmlSerializer.Deserialize(File.OpenRead(configurationLayer.UserFilePath));

            configurationLayer.ComputedConfig = ProductivityShell.Configuration.Configuration.ComputeConfig(configurationLayer.MachineConfig, configurationLayer.SharedConfig, configurationLayer.UserConfig);
            return configurationLayer;
        }

        /// <summary>
        ///     Establishes client notification of solution events.
        /// </summary>
        /// <param name="package">
        ///     Pointer to the Microsoft.VisualStudio.Shell.Interop.IVsSolutionEvents interface on the object
        ///     requesting notification of solution events.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int AdviseSolutionEvents(Package package)
        {
            return Solution2.AdviseSolutionEvents(package, out _solutionEventsCookie);
        }

        /// <summary>
        ///     Called when [closed solution].
        /// </summary>
        public void OnClosedSolution()
        {
            _projectCache.Clear();
            _psSolution.OnClosed();
        }

        /// <summary>
        ///     Called when [opened solution].
        /// </summary>
        public void OnOpenedSolution()
        {
            _configurationFileTrackers.Add(new ConfigurationFileTracker(MachineConfigFilePath, VsProxy.FileChange));
            _configurationFileTrackers.Add(new ConfigurationFileTracker(SharedConfigFilePath, VsProxy.FileChange));
            _configurationFileTrackers.Add(new ConfigurationFileTracker(UserConfigFilePath, VsProxy.FileChange));
            _configurationFileTrackers.ForEach(s =>
            {
                s.FileChanged += ConfigurationFileTracker_FileChanged;
                s.Start();
            });

            _psSolution.OnOpened();
        }

        /// <summary>
        ///     Handles the FileChanged event of the ConfigurationFileTracker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ConfigurationFileTracker_FileChanged(object sender, EventArgs e)
        {
            _psSolution.OnConfigurationChanged();
        }

        /// <summary>
        ///     Unadvises the solution events.
        /// </summary>
        public void UnadviseSolutionEvents()
        {
            if (Solution2 != null && _solutionEventsCookie != 0)
                Solution2.UnadviseSolutionEvents(_solutionEventsCookie);
        }

        /// <summary>
        ///     Called when [opened project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        public void OnOpenedProject(IVsHierarchy hierarchy, bool isNew)
        {
            // Visual Studio reports folders as opened projects but without a name
            var project = hierarchy.ToProject();
            if (string.IsNullOrWhiteSpace(project.FullName))
                return;

            var projectProxy = new ProjectProxy();
            projectProxy.Hierarchy = hierarchy;
            projectProxy.SolutionProxy = this;
            projectProxy.VsProject = project;
            projectProxy.Name = project.Name;
            projectProxy.RelativePath = project.GetRelativePath(_vsSolution);

            _projectCache.Add(projectProxy);
            _psSolution.OnOpenedProject(projectProxy.Name);
        }

        /// <summary>
        ///     Opens the configuration file.
        /// </summary>
        public void OpenConfigurationFile()
        {
            SaveConfigurationLayer(ConfigurationLayer.Default, false);
            VsProxy.Dte.ItemOperations.OpenFile(SharedConfigFilePath, Constants.vsViewKindCode);
        }

        /// <summary>
        ///     Saves the configuration layer.
        /// </summary>
        /// <param name="configurationLayer">The configuration layer.</param>
        /// <param name="override">if set to <c>true</c> [@override].</param>
        public void SaveConfigurationLayer(ConfigurationLayer configurationLayer, bool @override)
        {
            if (configurationLayer == null)
                throw new ArgumentNullException(nameof(configurationLayer));

            var configMap = new[]
            {
                new {FilePath = MachineConfigFilePath, Configuration = configurationLayer.MachineConfig},
                new {FilePath = SharedConfigFilePath, Configuration = configurationLayer.SharedConfig},
                new {FilePath = UserConfigFilePath, Configuration = configurationLayer.UserConfig}
            };

            foreach (var config in configMap)
            {
                if (string.IsNullOrWhiteSpace(config.FilePath) || config.Configuration == null)
                    continue;

                if (!@override && File.Exists(config.FilePath))
                    continue;

                var directoryName = Path.GetDirectoryName(config.FilePath);
                if (string.IsNullOrWhiteSpace(directoryName))
                    continue;

                Directory.CreateDirectory(directoryName);
                using (var fileStream = File.OpenWrite(config.FilePath))
                {
                    ConfigurationXmlSerializer.Serialize(fileStream, config.Configuration);
                }
            }
        }

        /// <summary>
        ///     Called when [closing project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        public void OnClosingProject(IVsHierarchy hierarchy)
        {
            if (!_projectCache.TryGetProjectByHierarchy(hierarchy, out var project))
                return;

            _projectCache.Remove(project);
            _psSolution.OnClosingProject(project.Name);
        }

        /// <summary>
        ///     Called when [closing solution].
        /// </summary>
        public void OnClosingSolution()
        {
            if (_configurationFileTrackers == null)
                return;

            _configurationFileTrackers.ForEach(s =>
            {
                s.FileChanged -= ConfigurationFileTracker_FileChanged;
                s.Stop();
            });
            _configurationFileTrackers.Clear();
        }

        /// <summary>
        ///     Called when [opening solution].
        /// </summary>
        public void OnOpeningSolution()
        {
            _psSolution.OnOpening();
        }

        /// <summary>
        ///     Called when [project loaded].
        /// </summary>
        public void OnProjectLoaded()
        {
            OnStartupProjectChanged();
        }

        /// <summary>
        ///     Called when [renamed project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        public void OnRenamedProject(IVsHierarchy hierarchy)
        {
            if (!_projectCache.TryGetProjectByHierarchy(hierarchy, out var project))
                return;

            var newProject = hierarchy.ToProject();
            var oldName = project.Name;

            project.Hierarchy = hierarchy;
            project.SolutionProxy = this;
            project.VsProject = newProject;
            project.Name = newProject.Name;
            project.RelativePath = newProject.GetRelativePath(_vsSolution);

            _projectCache.Rename(project, oldName, project.Name);
            _psSolution.OnRenamedProject(oldName, project.Name);
        }
    }
}