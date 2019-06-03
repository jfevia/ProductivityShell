using System;
using System.Runtime.InteropServices;
using System.Threading;
using Jfevia.ProductivityShell.Vsix.Commands.Environment;
using Jfevia.ProductivityShell.Vsix.Commands.Package;
using Jfevia.ProductivityShell.Vsix.Commands.Project;
using Jfevia.ProductivityShell.Vsix.Commands.ProjectItem;
using Jfevia.ProductivityShell.Vsix.Commands.Refactor;
using Jfevia.ProductivityShell.Vsix.Commands.Shell;
using Jfevia.ProductivityShell.Vsix.Commands.Tools;
using Jfevia.ProductivityShell.Vsix.DialogPages;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix
{
    [InstalledProductRegistration(PackageConstants.ProductName, PackageConstants.ProductDetails, PackageConstants.ProductVersion, IconResourceID = 400)]
    [ProvideOptionPage(typeof(GeneralDialogPage), PackageConstants.ProductName, PackageConstants.GeneralPage, 0, 0, false)]
    [ProvideOptionPage(typeof(ToolsDialogPage), PackageConstants.ProductName, PackageConstants.ToolsPage, 0, 0, false)]
    [Guid(PackageGuids.PackageString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed partial class Package : PackageBase
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Package" /> class.
        /// </summary>
        public Package()
            : base(PackageGuids.PackageCommandSet)
        {
            Instance = this;
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Package Instance { get; private set; }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Instance.JoinableTaskFactory.Run(() => _shellProxy.SolutionProxy.UnadviseSolutionEventsAsync(this));
            Instance.JoinableTaskFactory.Run(() => _shellProxy.UnadviseSelectionEventsAsync(this));
        }

        /// <summary>
        ///     The async initialization portion of the package initialization process. This method is invoked from a background
        ///     thread.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token to monitor for initialization cancellation, which can occur when
        ///     VS is shutting down.
        /// </param>
        /// <param name="progress"></param>
        /// <returns>
        ///     A task representing the async work of package initialization, or an already completed task if there is none. Do not
        ///     return null from this method.
        /// </returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            _shellProxy = new ShellProxy(this);
            await _shellProxy.AdviseDebuggingEventsAsync(this);
            await _shellProxy.AdviseSelectionEventsAsync(this);
            await _shellProxy.SolutionProxy.AdviseSolutionEventsAsync(this);

            // Shell
            await RestartNormalCommand.InitializeAsync(this);
            await RestartElevatedCommand.InitializeAsync(this);

            // Environment
            await PathVariablesCommand.InitializeAsync(this);

            // Project
            await OpenOutputFolderCommand.InitializeAsync(this);
            await ShowOutputFileInExplorerCommand.InitializeAsync(this);
            await ReloadCommand.InitializeAsync(this);

            // Project Item
            await ShowInExplorerCommand.InitializeAsync(this);

            // Refactoring
            await MoveToSettingsCommand.InitializeAsync(this);

            // Tools
            await ReplaceGuidPlaceholdersCommand.InitializeAsync(this);

            // Package
            await ShowOptionPageCommand.InitializeAsync(this);
        }
    }
}