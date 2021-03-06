﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Jfevia.ProductivityShell.Configuration;
using Jfevia.ProductivityShell.SolutionModel;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Solutions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Solution = EnvDTE.Solution;

namespace Jfevia.ProductivityShell.Vsix.Shell
{
    internal abstract class ShellProxyBase : ProxyBase
    {
        private DTE _dte;
        private DTE2 _dte2;
        private IVsFileChangeEx _fileChange;
        private IMenuCommandService _menuCommandService;
        private IVsMonitorSelection _monitorSelection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShellProxyBase" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        protected ShellProxyBase(Package package)
        {
            Package = package;
        }

        /// <summary>
        ///     Gets the DTE.
        /// </summary>
        /// <value>
        ///     The DTE.
        /// </value>
        public DTE Dte => _dte ?? (_dte = Package.GetGlobalService<_DTE, DTE>());

        /// <summary>
        ///     Gets the DTE2.
        /// </summary>
        /// <value>
        ///     The DTE2.
        /// </value>
        public DTE2 Dte2 => _dte2 ?? (_dte2 = Package.GetGlobalService<_DTE, DTE2>());

        /// <summary>
        ///     Gets the package.
        /// </summary>
        /// <value>
        ///     The package.
        /// </value>
        public Package Package { get; }

        /// <summary>
        ///     Gets the monitor selection.
        /// </summary>
        /// <value>
        ///     The monitor selection.
        /// </value>
        public async Task<IVsMonitorSelection> GetMonitorSelectionAsync()
        {
            return _monitorSelection ?? (_monitorSelection = await Package.GetServiceAsync<SVsShellMonitorSelection, IVsMonitorSelection>());
        }

        /// <summary>
        ///     Gets the file change.
        /// </summary>
        /// <value>
        ///     The file change.
        /// </value>
        public async Task<IVsFileChangeEx> GetFileChangeAsync()
        {
            return _fileChange ?? (_fileChange = await Package.GetServiceAsync<SVsFileChangeEx, IVsFileChangeEx>());
        }

        /// <summary>
        ///     Gets the menu command service.
        /// </summary>
        /// <value>
        ///     The menu command service.
        /// </value>
        public async Task<IMenuCommandService> GetMenuCommandServiceAsync()
        {
            return _menuCommandService ?? (_menuCommandService = await Package.GetServiceAsync<IMenuCommandService>());
        }
    }

    internal class ShellProxy : ShellProxyBase, IDisposable
    {
        private uint _debuggingCookie;
        private uint _selectionEventsCookie;
        private SolutionProxy _solutionProxy;
        private StartupProfilesService _startupProfileService;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Shell.ShellProxy" />
        ///     class.
        /// </summary>
        /// <param name="package">The package.</param>
        public ShellProxy(Package package)
            : base(package)
        {
        }

        /// <summary>
        ///     Gets the solution proxy.
        /// </summary>
        /// <value>
        ///     The solution proxy.
        /// </value>
        public SolutionProxy SolutionProxy
        {
            get
            {
                if (_solutionProxy == null)
                {
                    _solutionProxy = new SolutionProxy(this, new SolutionModel.Solution());
                    _solutionProxy.Opened += SolutionProxy_Opened;
                    _solutionProxy.ClosingProject += SolutionProxy_ClosingProject;
                    _solutionProxy.OpenedProject += SolutionProxy_OpenedProject;
                    _solutionProxy.ConfigurationChanged += SolutionProxy_ConfigurationChanged;
                    _solutionProxy.CurrentStartupProjectsChanged += SolutionProxy_CurrentStartupProjectsChanged;
                }

                return _solutionProxy;
            }
        }

        /// <summary>
        ///     Gets the debugging cookie.
        /// </summary>
        /// <value>
        ///     The debugging cookie.
        /// </value>
        public uint DebuggingCookie => _debuggingCookie;

        /// <summary>
        ///     Gets or sets the Visual Studio solution.
        /// </summary>
        /// <value>
        ///     The Visual Studio solution.
        /// </value>
        public Solution VsSolution => Dte2.Solution;

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_solutionProxy != null)
            {
                _solutionProxy.Opened -= SolutionProxy_Opened;
                _solutionProxy.ClosingProject -= SolutionProxy_ClosingProject;
                _solutionProxy.OpenedProject -= SolutionProxy_OpenedProject;
                _solutionProxy.ConfigurationChanged -= SolutionProxy_ConfigurationChanged;
                _solutionProxy.CurrentStartupProjectsChanged -= SolutionProxy_CurrentStartupProjectsChanged;
                _solutionProxy.Dispose();
            }

            if (_startupProfileService != null)
            {
                _startupProfileService.SelectedStartupProfileChanged -= StartupProfileService_SelectedStartupProfileChanged;
                _startupProfileService.RequestedShowConfiguration -= StartupProfileService_RequestedShowConfiguration;
            }
        }

        /// <summary>
        ///     Gets the startup profile service.
        /// </summary>
        /// <returns>The startup profile service.</returns>
        public async Task<StartupProfilesService> GetStartupProfileServiceAsync()
        {
            if (_startupProfileService != null)
                return _startupProfileService;

            _startupProfileService = new StartupProfilesService(await GetMenuCommandServiceAsync());
            _startupProfileService.SelectedStartupProfileChanged += StartupProfileService_SelectedStartupProfileChanged;
            _startupProfileService.RequestedShowConfiguration += StartupProfileService_RequestedShowConfiguration;
            return _startupProfileService;
        }

        /// <summary>
        ///     Handles the CurrentStartupProjectsChanged event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProfileEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_CurrentStartupProjectsChanged(object sender, ProfileEventArgs e)
        {
            UpdateCurrentStartupProjectService(e.Profile);
        }

        /// <summary>
        ///     Updates the current startup project service.
        /// </summary>
        /// <param name="profile">The profile.</param>
        private void UpdateCurrentStartupProjectService(Profile profile)
        {
            if (profile == null)
                return;

            _startupProfileService.SelectedItem = profile;
        }

        /// <summary>
        ///     Handles the ConfigurationChanged event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            UpdateStartupProjects(e.Profiles, e.SelectedProfile);
        }

        /// <summary>
        ///     Handles the OpenedProject event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProjectEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_OpenedProject(object sender, ProjectEventArgs e)
        {
            UpdateStartupProjects(e.Profiles, e.SelectedProfile);
        }

        /// <summary>
        ///     Handles the RequestedShowConfiguration event of the StartupProfileService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void StartupProfileService_RequestedShowConfiguration(object sender, EventArgs e)
        {
            await SolutionProxy.OpenConfigurationFileAsync();
        }

        /// <summary>
        ///     Handles the SelectedStartupProfileChanged event of the StartupProfileService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupProfileChangedEventArgs" /> instance containing the event data.</param>
        private async void StartupProfileService_SelectedStartupProfileChanged(object sender, StartupProfileChangedEventArgs e)
        {
            await SolutionProxy.OnStartupProfileChangedAsync(e.Profile);
        }

        /// <summary>
        ///     Handles the Opened event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SolutionEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_Opened(object sender, SolutionEventArgs e)
        {
            _startupProfileService.Items = e.Profiles.OrderBy(s => s.DisplayName);

            if (e.SelectedProfile == null)
                return;

            _startupProfileService.SelectedItem = e.SelectedProfile;
        }

        /// <summary>
        ///     Advises the debugging events.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>The task.</returns>
        public async Task AdviseDebuggingEventsAsync(Package package)
        {
            var monitorSelection = await GetMonitorSelectionAsync();
            if (monitorSelection != null)
            {
                await package.JoinableTaskFactory.SwitchToMainThreadAsync();
                monitorSelection.GetCmdUIContextCookie(VSConstants.UICONTEXT.Debugging_guid, out _debuggingCookie);
            }
        }

        /// <summary>
        ///     Registers a VSPackage for selection event notification.
        /// </summary>
        /// <param name="package">
        ///     Pointer to the Microsoft.VisualStudio.Shell.Interop.IVsSelectionEvents interface of the VSPackage
        ///     registering for selection event notification.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public async Task<int?> AdviseSelectionEventsAsync(Package package)
        {
            var monitorSelection = await GetMonitorSelectionAsync();
            if (monitorSelection == null)
                return null;

            await package.JoinableTaskFactory.SwitchToMainThreadAsync();
            return monitorSelection.AdviseSelectionEvents(package, out _selectionEventsCookie);
        }

        /// <summary>
        ///     Called when [debugging state changed].
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void OnDebuggingStateChanged(bool isEnabled)
        {
            _startupProfileService.OnDebuggingStateChanged(isEnabled);
        }

        /// <summary>
        ///     Called when [closed solution].
        /// </summary>
        public void OnClosedSolution()
        {
            _startupProfileService.OnClosedSolution();
            SolutionProxy.OnClosedSolution();
        }

        /// <summary>
        ///     Unadvises the selection events.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>The task.</returns>
        public async Task UnadviseSelectionEventsAsync(Package package)
        {
            var monitorSelection = await GetMonitorSelectionAsync();
            if (monitorSelection != null && _selectionEventsCookie != 0)
            {
                await package.JoinableTaskFactory.SwitchToMainThreadAsync();
                monitorSelection.UnadviseSelectionEvents(_selectionEventsCookie);
            }
        }

        /// <summary>
        ///     Handles the ClosingProject event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SolutionEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_ClosingProject(object sender, ProjectEventArgs e)
        {
            UpdateStartupProjects(e.Profiles, e.SelectedProfile);
        }

        /// <summary>
        ///     Updates the startup projects.
        /// </summary>
        /// <param name="profiles">The profiles.</param>
        /// <param name="currentProfile">The current profile.</param>
        private void UpdateStartupProjects(IEnumerable<Profile> profiles, Profile currentProfile)
        {
            var itemMap = profiles.OrderBy(s => s.DisplayName).ToDictionary(s => s);
            _startupProfileService.Items = itemMap.Values;

            UpdateCurrentStartupProjectService(currentProfile);
        }

        /// <summary>
        ///     Called when [closing project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        public void OnClosingProject(IVsHierarchy hierarchy)
        {
            _solutionProxy.OnClosingProject(hierarchy);
        }


        /// <summary>
        ///     Called when [closing solution].
        /// </summary>
        /// <returns>The task.</returns>
        public async Task OnClosingSolutionAsync()
        {
            await _solutionProxy.OnClosingSolutionAsync();
        }

        /// <summary>
        ///     Called when [opening solution].
        /// </summary>
        public void OnOpeningSolution()
        {
            SolutionProxy.OnOpeningSolution();
        }

        /// <summary>
        ///     Called when [opened project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        /// <returns>The task.</returns>
        public async Task OnOpenedProjectAsync(IVsHierarchy hierarchy, bool isNew)
        {
            await SolutionProxy.OnOpenedProjectAsync(hierarchy, isNew);
        }

        /// <summary>
        ///     Called when [opened solution].
        /// </summary>
        public async Task OnOpenedSolutionAsync()
        {
            await SolutionProxy.OnOpenedSolutionAsync();
        }

        /// <summary>
        ///     Called when [startup project changed].
        /// </summary>
        public async Task OnStartupProjectChangedAsync()
        {
            await SolutionProxy.OnStartupProjectChangedAsync();
        }

        /// <summary>
        ///     Called when [project loaded].
        /// </summary>
        public async Task OnProjectLoadedAsync()
        {
            await SolutionProxy.OnProjectLoadedAsync();
        }

        /// <summary>
        ///     Called when [renamed project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        public async Task OnRenamedProjectAsync(IVsHierarchy hierarchy)
        {
            await SolutionProxy.OnRenamedProjectAsync(hierarchy);
        }
    }
}