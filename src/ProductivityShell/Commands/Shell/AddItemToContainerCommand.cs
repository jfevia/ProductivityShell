using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Shell
{
    internal sealed class AddItemToCommandBarCommand : CommandBase<AddItemToCommandBarCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Commands.Shell.AddItemToCommandBarCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private AddItemToCommandBarCommand(PackageBase package)
            : base(package, PackageIds.ShellAddItemToContainerCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static void Initialize(PackageBase package)
        {
            Instance = new AddItemToCommandBarCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            var window = new AddItemToCommandBarWindow();
            if (window.ShowModal() != true)
                return;

            Package.AddOrUpdateCommandBar(window.CommandName, window.DisplayName, window.CommandBarName, window.CommandBarType);
        }
    }
}