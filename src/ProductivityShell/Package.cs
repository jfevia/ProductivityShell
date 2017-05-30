using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProductivityShell.Commands.Environment;
using ProductivityShell.Commands.Package;
using ProductivityShell.Commands.Project;
using ProductivityShell.Commands.ProjectItem;
using ProductivityShell.Commands.Shell;
using ProductivityShell.Commands.Tools;
using ProductivityShell.DialogPages;
using ProductivityShell.Shell;

namespace ProductivityShell
{
    [InstalledProductRegistration(PackageConstants.ProductName, PackageConstants.ProductDetails, PackageConstants.ProductVersion, IconResourceID = 400)]
    [ProvideOptionPage(typeof(GeneralDialogPage), PackageConstants.ProductName, PackageConstants.GeneralPage, 0, 0, false)]
    [ProvideOptionPage(typeof(ToolsDialogPage), PackageConstants.ProductName, PackageConstants.ToolsPage, 0, 0, false)]
    [Guid(PackageGuids.PackageString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class Package : PackageBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Package" /> class.
        /// </summary>
        public Package() : base(PackageGuids.PackageCommandSet)
        {
            Instance = this;
        }

        public static Package Instance { get; internal set; }

        protected override void Initialize()
        {
            base.Initialize();

            // Shell
            RestartNormalCommand.Initialize(this);
            RestartElevatedCommand.Initialize(this);

            // Environment
            PathVariablesCommand.Initialize(this);

            // Project
            OpenOutputFolderCommand.Initialize(this);
            ReloadCommand.Initialize(this);

            // Project Item
            ShowInExplorerCommand.Initialize(this);

            // Tools
            ReplaceGuidPlaceholdersCommand.Initialize(this);

            // Package
            ShowOptionPageCommand.Initialize(this);
        }
    }
}