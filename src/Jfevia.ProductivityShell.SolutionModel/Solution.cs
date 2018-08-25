using System;
using System.Collections.Generic;
using Jfevia.ProductivityShell.Configuration;

namespace Jfevia.ProductivityShell.SolutionModel
{
    public class Solution
    {
        private Profile _currentProfile;
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
        ///     Occurs when [current startup projects changed].
        /// </summary>
        public event EventHandler<StartupProjectsEventArgs> CurrentStartupProjectsChanged;

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
        ///     Called when [startup projects changed].
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="startupProjects">The startup projects.</param>
        public void OnStartupProjectsChanged(Profile profile, string[] startupProjects)
        {
            // Skip update if explicitly requested (e.g.: startup projects being set through our combo box)
            if (_startupProjectChangeNotificationsSuspended)
                return;

            if (IsOpening)
                return;

            if (startupProjects == null)
                return;

            if (ProfileEqualityComparer.Instance.Equals(_currentProfile, profile))
                return;

            SuspendStartupProjectChangeNotifications();
            _currentProfile = profile;
            CurrentStartupProjectsChanged?.Invoke(this, new StartupProjectsEventArgs(profile, startupProjects));
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

            var configurationLayer = GetConfigurationLayer();
            var startupProjects = GetStartupProjects();
            var currentStartupProjects = GetCurrentStartupProjects();
            var parsedConfiguration = GetParsedConfiguration(configurationLayer.ComputedConfig.Startup.Profiles, startupProjects, currentStartupProjects);

            _currentProfile = parsedConfiguration.CurrentProfile;

            Opened?.Invoke(this, new SolutionEventArgs(configurationLayer.ComputedConfig, parsedConfiguration.Profiles, parsedConfiguration.CurrentProfile));
        }

        /// <summary>
        ///     Gets the parsed configuration.
        /// </summary>
        /// <param name="profiles">The profiles.</param>
        /// <param name="startupProjects">The startup projects.</param>
        /// <param name="currentStartupProjects">The current startup projects.</param>
        /// <returns>The parsed configuration.</returns>
        private ParsedConfiguration GetParsedConfiguration(ICollection<Profile> profiles, ICollection<string> startupProjects, ICollection<string> currentStartupProjects)
        {
            var parseConfigurationEventArgs = new ParseConfigurationEventArgs(profiles, startupProjects, currentStartupProjects);
            ParseConfiguration?.Invoke(this, parseConfigurationEventArgs);
            return parseConfigurationEventArgs.ParsedConfiguration;
        }

        /// <summary>
        ///     Gets the current startup projects.
        /// </summary>
        /// <returns>The current startup projects.</returns>
        private ICollection<string> GetCurrentStartupProjects()
        {
            var currentStartupProjectsEventArgs = new CurrentStartupProjectsEventArgs();
            QueryCurrentStartupProjects?.Invoke(this, currentStartupProjectsEventArgs);
            return currentStartupProjectsEventArgs.StartupProjects;
        }

        /// <summary>
        ///     Gets the startup projects.
        /// </summary>
        /// <returns>The startup projects.</returns>
        private ICollection<string> GetStartupProjects()
        {
            var startupProjectsEventArgs = new StartupProjectsEventArgs();
            QueryStartupProjects?.Invoke(this, startupProjectsEventArgs);
            return startupProjectsEventArgs.StartupProjects;
        }

        /// <summary>
        ///     Gets the configuration layer.
        /// </summary>
        /// <returns>The configuration layer.</returns>
        private ConfigurationLayer GetConfigurationLayer()
        {
            var configurationEventArgs = new ConfigurationEventArgs();
            QueryConfiguration?.Invoke(this, configurationEventArgs);
            return configurationEventArgs.ConfigurationLayer;
        }

        /// <summary>
        ///     Called when [configuration changed].
        /// </summary>
        public void OnConfigurationChanged()
        {
            var configurationLayer = GetConfigurationLayer();
            var startupProjects = GetStartupProjects();
            var currentStartupProjects = GetCurrentStartupProjects();
            var parsedConfiguration = GetParsedConfiguration(configurationLayer.ComputedConfig.Startup.Profiles, startupProjects, currentStartupProjects);

            _currentProfile = parsedConfiguration.CurrentProfile;

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(configurationLayer.ComputedConfig, parsedConfiguration.Profiles, parsedConfiguration.CurrentProfile));
        }

        /// <summary>
        ///     Called when [closing project].
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public void OnClosingProject(string projectName)
        {
            var configurationLayer = GetConfigurationLayer();
            var startupProjects = GetStartupProjects();
            var currentStartupProjects = GetCurrentStartupProjects();
            var parsedConfiguration = GetParsedConfiguration(configurationLayer.ComputedConfig.Startup.Profiles, startupProjects, currentStartupProjects);

            _currentProfile = parsedConfiguration.CurrentProfile;

            ClosingProject?.Invoke(this, new ProjectEventArgs(projectName, configurationLayer.ComputedConfig, parsedConfiguration.Profiles, parsedConfiguration.CurrentProfile));
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
            var configurationLayer = GetConfigurationLayer();
            var startupProjects = GetStartupProjects();
            var currentStartupProjects = GetCurrentStartupProjects();
            var parsedConfiguration = GetParsedConfiguration(configurationLayer.ComputedConfig.Startup.Profiles, startupProjects, currentStartupProjects);

            _currentProfile = parsedConfiguration.CurrentProfile;

            OpenedProject?.Invoke(this, new ProjectEventArgs(projectName, configurationLayer.ComputedConfig, parsedConfiguration.Profiles, parsedConfiguration.CurrentProfile));
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