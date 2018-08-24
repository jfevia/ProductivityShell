using Jfevia.ProductivityShell.Vsix.DialogPages;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;

namespace Jfevia.ProductivityShell.Vsix.Commands.Package
{
    internal sealed class ShowOptionPageCommand : CommandBase<ShowOptionPageCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Package.ShowOptionPageCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ShowOptionPageCommand(PackageBase package)
            : base(package, PackageCommands.PackageShowOptionPageCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static void Initialize(PackageBase package)
        {
            Instance = new ShowOptionPageCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            Package.ShowOptionPage<GeneralDialogPage>();
        }
    }
}