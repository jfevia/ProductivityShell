using Microsoft.VisualStudio.Shell;
using ProductivityShell.DialogPages;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Package
{
    internal sealed class ShowOptionPageCommand : CommandBase<ShowOptionPageCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ShowOptionPageCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ShowOptionPageCommand(PackageBase package) : base(package, PackageIds.PackageShowOptionPageCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new ShowOptionPageCommand(package);
        }

        protected override void OnExecute(OleMenuCommand command)
        {
            Package.ShowOptionPage<GeneralDialogPage>();
        }
    }
}