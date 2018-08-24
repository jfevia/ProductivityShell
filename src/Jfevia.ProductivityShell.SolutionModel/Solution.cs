using System;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class Solution
    {
        private bool _startupProjectChangeNotificationsSuspended;

        /// <summary>
        ///     Occurs when [closing project].
        /// </summary>
        public event EventHandler<ProjectEventArgs> ClosingProject;

        /// <summary>
        ///     Occurs when [configuration changed].
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        /// <summary>
        ///     Occurs when [opened].
        /// </summary>
        public event EventHandler<SolutionEventArgs> Opened;

        /// <summary>
        ///     Occurs when [opened project].
        /// </summary>
        public event EventHandler<ProjectEventArgs> OpenedProject;

        /// <summary>
        ///     Occurs when [parse configuration].
        /// </summary>
        public event EventHandler<ParseConfigurationEventArgs> ParseConfiguration;

        /// <summary>
        ///     Occurs when [query configuration].
        /// </summary>
        public event EventHandler<ConfigurationEventArgs> QueryConfiguration;

        /// <summary>
        ///     Occurs when [query current startup projects].
        /// </summary>
        public event EventHandler<CurrentStartupProjectsEventArgs> QueryCurrentStartupProjects;

        /// <summary>
        ///     Occurs when [query startup projects].
        /// </summary>
        public event EventHandler<StartupProjectsEventArgs> QueryStartupProjects;

        /// <summary>
        ///     Occurs when [renamed project].
        /// </summary>
        public event EventHandler<RenameProjectEventArgs> RenamedProject;

        /// <summary>
        ///     Occurs when [startup projects changed].
        /// </summary>
        public event EventHandler<StartupProjectsEventArgs> StartupProjectsChanged;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is opening.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is opening; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpening { get; protected set; }

        /// <summary>
        ///     Suspends the startup project change notifications.
        /// </summary>
        private void SuspendStartupProjectChangeNotifications()
        {
            _startupProjectChangeNotificationsSuspended = true;
        }

        /// <summary>
        ///     Resumes the startup project change notifications.
        /// </summary>
        private void ResumeStartupProjectChangeNotifications()
        {
            _startupProjectChangeNotificationsSuspended = false;
        }

        /// <summary>
        ///     Sets the startup projects.
        /// </summary>
        /// <param name="startupProjects">The startup projects.</param>
        public void SetStartupProjects(string[] startupProjects)
        {
            // Skip update if explicitly requested (e.g.: startup projects being set through our combo box)
            if (_startupProjectChangeNotificationsSuspended)
                return;

            if (IsOpening)
                return;

            if (startupProjects == null)
                return;

            SuspendStartupProjectChangeNotifications();
            StartupProjectsChanged?.Invoke(this, new StartupProjectsEventArgs(startupProjects));
            ResumeStartupProjectChangeNotifications();
        }

        /// <summary>
        ///     Called when [closed].
        /// </summary>
        public void OnClosed()
        {
        }

        /// <summary>
        ///     Called when [opened].
        /// </summary>
        public void OnOpened()
        {
            IsOpening = false;

            var configurationEventArgs = new ConfigurationEventArgs();
            QueryConfiguration?.Invoke(this, configurationEventArgs);

            var startupProjectsEventArgs = new StartupProjectsEventArgs();
            QueryStartupProjects?.Invoke(this, startupProjectsEventArgs);

            var currentStartupProjectsEventArgs = new CurrentStartupProjectsEventArgs();
            QueryCurrentStartupProjects?.Invoke(this, currentStartupProjectsEventArgs);

            var parseConfigurationEventArgs = new ParseConfigurationEventArgs(configurationEventArgs.ConfigurationLayer.ComputedConfig.Startup.ProjectConfigurations, startupProjectsEventArgs.StartupProjects, currentStartupProjectsEventArgs.StartupProjects);
            ParseConfiguration?.Invoke(this, parseConfigurationEventArgs);

            Opened?.Invoke(this, new SolutionEventArgs(configurationEventArgs.ConfigurationLayer.ComputedConfig, parseConfigurationEventArgs.ParsedConfiguration.ProjectConfigurations, parseConfigurationEventArgs.ParsedConfiguration.CurrentProjectConfiguration));
        }

        /// <summary>
        ///     Called when [configuration changed].
        /// </summary>
        public void OnConfigurationChanged()
        {
            // TODO: Code reuse, this is the same as OnOpened
            var configurationEventArgs = new ConfigurationEventArgs();
            QueryConfiguration?.Invoke(this, configurationEventArgs);

            var startupProjectsEventArgs = new StartupProjectsEventArgs();
            QueryStartupProjects?.Invoke(this, startupProjectsEventArgs);

            var currentStartupProjectsEventArgs = new CurrentStartupProjectsEventArgs();
            QueryCurrentStartupProjects?.Invoke(this, currentStartupProjectsEventArgs);

            var parseConfigurationEventArgs = new ParseConfigurationEventArgs(configurationEventArgs.ConfigurationLayer.ComputedConfig.Startup.ProjectConfigurations, startupProjectsEventArgs.StartupProjects, currentStartupProjectsEventArgs.StartupProjects);
            ParseConfiguration?.Invoke(this, parseConfigurationEventArgs);

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(configurationEventArgs.ConfigurationLayer.ComputedConfig, parseConfigurationEventArgs.ParsedConfiguration.ProjectConfigurations, parseConfigurationEventArgs.ParsedConfiguration.CurrentProjectConfiguration));
        }

        /// <summary>
        ///     Called when [closing project].
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public void OnClosingProject(string projectName)
        {
            // TODO: Code reuse, this is the same as OnOpened
            var configurationEventArgs = new ConfigurationEventArgs();
            QueryConfiguration?.Invoke(this, configurationEventArgs);

            var startupProjectsEventArgs = new StartupProjectsEventArgs();
            QueryStartupProjects?.Invoke(this, startupProjectsEventArgs);

            var currentStartupProjectsEventArgs = new CurrentStartupProjectsEventArgs();
            QueryCurrentStartupProjects?.Invoke(this, currentStartupProjectsEventArgs);

            var parseConfigurationEventArgs = new ParseConfigurationEventArgs(configurationEventArgs.ConfigurationLayer.ComputedConfig.Startup.ProjectConfigurations, startupProjectsEventArgs.StartupProjects, currentStartupProjectsEventArgs.StartupProjects);
            ParseConfiguration?.Invoke(this, parseConfigurationEventArgs);

            ClosingProject?.Invoke(this, new ProjectEventArgs(projectName, configurationEventArgs.ConfigurationLayer.ComputedConfig, parseConfigurationEventArgs.ParsedConfiguration.ProjectConfigurations, parseConfigurationEventArgs.ParsedConfiguration.CurrentProjectConfiguration));
        }

        /// <summary>
        ///     Called when [opening].
        /// </summary>
        public void OnOpening()
        {
            IsOpening = true;
        }

        /// <summary>
        ///     Called when [opened project].
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public void OnOpenedProject(string projectName)
        {
            // TODO: Code reuse, this is the same as OnOpened
            var configurationEventArgs = new ConfigurationEventArgs();
            QueryConfiguration?.Invoke(this, configurationEventArgs);

            var startupProjectsEventArgs = new StartupProjectsEventArgs();
            QueryStartupProjects?.Invoke(this, startupProjectsEventArgs);

            var currentStartupProjectsEventArgs = new CurrentStartupProjectsEventArgs();
            QueryCurrentStartupProjects?.Invoke(this, currentStartupProjectsEventArgs);

            var parseConfigurationEventArgs = new ParseConfigurationEventArgs(configurationEventArgs.ConfigurationLayer.ComputedConfig.Startup.ProjectConfigurations, startupProjectsEventArgs.StartupProjects, currentStartupProjectsEventArgs.StartupProjects);
            ParseConfiguration?.Invoke(this, parseConfigurationEventArgs);

            OpenedProject?.Invoke(this, new ProjectEventArgs(projectName, configurationEventArgs.ConfigurationLayer.ComputedConfig, parseConfigurationEventArgs.ParsedConfiguration.ProjectConfigurations, parseConfigurationEventArgs.ParsedConfiguration.CurrentProjectConfiguration));
        }

        /// <summary>
        ///     Called when [renamed project].
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public void OnRenamedProject(string oldName, string newName)
        {
            RenamedProject?.Invoke(this, new RenameProjectEventArgs(oldName, newName));
        }
    }
}