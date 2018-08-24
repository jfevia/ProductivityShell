using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Jfevia.ProductivityShell.SolutionModel;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Solutions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Solution = EnvDTE.Solution;

namespace Jfevia.ProductivityShell.Vsix.VisualStudio
{
    public abstract class VisualStudioProxyBase
    {
        private DTE _dte;
        private DTE2 _dte2;
        private IVsFileChangeEx _fileChange;
        private IMenuCommandService _menuCommandService;
        private IVsMonitorSelection _monitorSelection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VisualStudioProxyBase" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        protected VisualStudioProxyBase(Package package)
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
        ///     Gets the monitor selection.
        /// </summary>
        /// <value>
        ///     The monitor selection.
        /// </value>
        public IVsMonitorSelection MonitorSelection => _monitorSelection ?? (_monitorSelection = Package.GetService<SVsShellMonitorSelection, IVsMonitorSelection>());

        /// <summary>
        ///     Gets the file change.
        /// </summary>
        /// <value>
        ///     The file change.
        /// </value>
        public IVsFileChangeEx FileChange => _fileChange ?? (_fileChange = Package.GetService<SVsFileChangeEx, IVsFileChangeEx>());

        /// <summary>
        ///     Gets the menu command service.
        /// </summary>
        /// <value>
        ///     The menu command service.
        /// </value>
        public IMenuCommandService MenuCommandService => _menuCommandService ?? (_menuCommandService = Package.GetService<IMenuCommandService>());

        /// <summary>
        ///     Gets the package.
        /// </summary>
        /// <value>
        ///     The package.
        /// </value>
        public Package Package { get; }
    }

    public class VisualStudioProxy : VisualStudioProxyBase, IDisposable
    {
        private readonly StartupProjectsService _startupProjectsService;
        private uint _debuggingCookie;
        private uint _selectionEventsCookie;
        private SolutionProxy _solutionProxy;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.VisualStudio.VisualStudioProxy" />
        ///     class.
        /// </summary>
        /// <param name="package">The package.</param>
        public VisualStudioProxy(Package package)
            : base(package)
        {
            _startupProjectsService = new StartupProjectsService(MenuCommandService);
            _startupProjectsService.SelectedStartupProjectsChanged += StartupProjectsService_SelectedStartupProjectChanged;
            _startupProjectsService.RequestedShowConfiguration += StartupProjectsService_RequestedShowConfiguration;
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
                _solutionProxy.Dispose();
            }

            if (_startupProjectsService != null)
            {
                _startupProjectsService.SelectedStartupProjectsChanged -= StartupProjectsService_SelectedStartupProjectChanged;
                _startupProjectsService.RequestedShowConfiguration -= StartupProjectsService_RequestedShowConfiguration;
            }
        }

        /// <summary>
        ///     Handles the ConfigurationChanged event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            // TODO: Needs code reuse, this is exactly the same in SolutionProxy_Opened
            var itemMap = e.ProjectConfigurations.OrderBy(s => s.DisplayName).ToDictionary(s => s);
            _startupProjectsService.Items = itemMap.Values;

            if (e.SelectedProjectConfiguration == null)
                return;

            _startupProjectsService.SelectedItem = itemMap[e.SelectedProjectConfiguration];
        }

        /// <summary>
        ///     Handles the OpenedProject event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ProjectEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_OpenedProject(object sender, ProjectEventArgs e)
        {
            // TODO: Needs code reuse, this is exactly the same in SolutionProxy_Opened
            var itemMap = e.ProjectConfigurations.OrderBy(s => s.DisplayName).ToDictionary(s => s);
            _startupProjectsService.Items = itemMap.Values;

            if (e.SelectedProjectConfiguration == null)
                return;

            _startupProjectsService.SelectedItem = itemMap[e.SelectedProjectConfiguration];
        }

        /// <summary>
        ///     Handles the RequestedShowConfiguration event of the StartupProjectsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StartupProjectsService_RequestedShowConfiguration(object sender, EventArgs e)
        {
            SolutionProxy.OpenConfigurationFile();
        }

        /// <summary>
        ///     Handles the SelectedStartupProjectChanged event of the StartupProjectsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupProjectsChangedEventArgs" /> instance containing the event data.</param>
        private void StartupProjectsService_SelectedStartupProjectChanged(object sender, StartupProjectsChangedEventArgs e)
        {
            SolutionProxy.OnStartupProjectChanged(e.ProjectConfiguration);
        }

        /// <summary>
        ///     Handles the Opened event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SolutionEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_Opened(object sender, SolutionEventArgs e)
        {
            var itemMap = e.ProjectConfigurations.OrderBy(s => s.DisplayName).ToDictionary(s => s);
            _startupProjectsService.Items = itemMap.Values;

            if (e.SelectedProjectConfiguration == null)
                return;

            _startupProjectsService.SelectedItem = itemMap[e.SelectedProjectConfiguration];
        }

        /// <summary>
        ///     Advises the debugging events.
        /// </summary>
        /// <param name="package">The package.</param>
        public void AdviseDebuggingEvents(Package package)
        {
            MonitorSelection?.GetCmdUIContextCookie(VSConstants.UICONTEXT.Debugging_guid, out _debuggingCookie);
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
        public int? AdviseSelectionEvents(Package package)
        {
            return MonitorSelection?.AdviseSelectionEvents(package, out _selectionEventsCookie);
        }

        /// <summary>
        ///     Called when [debugging state changed].
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void OnDebuggingStateChanged(bool isEnabled)
        {
            _startupProjectsService.OnDebuggingStateChanged(isEnabled);
        }

        /// <summary>
        ///     Called when [closed solution].
        /// </summary>
        public void OnClosedSolution()
        {
            _startupProjectsService.OnClosedSolution();
            SolutionProxy.OnClosedSolution();
        }

        /// <summary>
        ///     Unadvises the selection events.
        /// </summary>
        public void UnadviseSelectionEvents()
        {
            if (MonitorSelection != null && _selectionEventsCookie != 0)
                MonitorSelection.UnadviseSelectionEvents(_selectionEventsCookie);
        }

        /// <summary>
        ///     Handles the ClosingProject event of the SolutionProxy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SolutionEventArgs" /> instance containing the event data.</param>
        private void SolutionProxy_ClosingProject(object sender, ProjectEventArgs e)
        {
            // TODO: Needs code reuse, this is exactly the same in SolutionProxy_Opened
            var itemMap = e.ProjectConfigurations.OrderBy(s => s.DisplayName).ToDictionary(s => s);
            _startupProjectsService.Items = itemMap.Values;

            if (e.SelectedProjectConfiguration == null)
                return;

            _startupProjectsService.SelectedItem = itemMap[e.SelectedProjectConfiguration];
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
        public void OnClosingSolution()
        {
            _solutionProxy.OnClosingSolution();
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
        public void OnOpenedProject(IVsHierarchy hierarchy, bool isNew)
        {
            SolutionProxy.OnOpenedProject(hierarchy, isNew);
        }

        /// <summary>
        ///     Called when [opened solution].
        /// </summary>
        public void OnOpenedSolution()
        {
            SolutionProxy.OnOpenedSolution();
        }

        /// <summary>
        ///     Called when [startup project changed].
        /// </summary>
        public void OnStartupProjectChanged()
        {
            SolutionProxy.OnStartupProjectChanged();
        }

        /// <summary>
        ///     Called when [project loaded].
        /// </summary>
        public void OnProjectLoaded()
        {
            SolutionProxy.OnProjectLoaded();
        }

        /// <summary>
        ///     Called when [renamed project].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        public void OnRenamedProject(IVsHierarchy hierarchy)
        {
            SolutionProxy.OnRenamedProject(hierarchy);
        }
    }
}