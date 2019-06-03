using Jfevia.ProductivityShell.Vsix.DialogPages;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Package
{
    internal sealed class ShowOptionPageCommand : CommandBase<ShowOptionPageCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Package.ShowOptionPageCommand" /> class.
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
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new ShowOptionPageCommand(package);
            await Instance.InitializeAsync();
        }

        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///     The task.
        /// </returns>
        protected override async Task OnBeforeQueryStatusAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///     The task.
        /// </returns>
        protected override async Task OnExecuteAsync(OleMenuCommand command)
        {
            Package.ShowOptionPage<GeneralDialogPage>();
            await Task.Yield();
        }

        /// <summary>
        ///     Called when [change].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///     The task.
        /// </returns>
        protected override async Task OnChangeAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }
    }
}