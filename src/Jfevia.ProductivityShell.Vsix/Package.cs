using System.Runtime.InteropServices;
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

namespace Jfevia.ProductivityShell.Vsix
{
    [InstalledProductRegistration(PackageConstants.ProductName, PackageConstants.ProductDetails, PackageConstants.ProductVersion, IconResourceID = 400)]
    [ProvideOptionPage(typeof(GeneralDialogPage), PackageConstants.ProductName, PackageConstants.GeneralPage, 0, 0, false)]
    [ProvideOptionPage(typeof(ToolsDialogPage), PackageConstants.ProductName, PackageConstants.ToolsPage, 0, 0, false)]
    [Guid(PackageGuids.PackageString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _shellProxy.SolutionProxy.UnadviseSolutionEvents();
            _shellProxy.UnadviseSelectionEvents();
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Package Instance { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _shellProxy = new ShellProxy(this);
            _shellProxy.AdviseDebuggingEvents(this);
            _shellProxy.AdviseSelectionEvents(this);
            _shellProxy.SolutionProxy.AdviseSolutionEvents(this);

            // Shell
            RestartNormalCommand.Initialize(this);
            RestartElevatedCommand.Initialize(this);

            // Environment
            PathVariablesCommand.Initialize(this);

            // Project
            OpenOutputFolderCommand.Initialize(this);
            ShowOutputFileInExplorerCommand.Initialize(this);
            ReloadCommand.Initialize(this);

            // Project Item
            ShowInExplorerCommand.Initialize(this);

            // Refactoring
            MoveToSettingsCommand.Initialize(this);

            // Tools
            ReplaceGuidPlaceholdersCommand.Initialize(this);

            // Package
            ShowOptionPageCommand.Initialize(this);
        }
    }
}