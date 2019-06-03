using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Shell
{
    internal sealed class RestartElevatedCommand : CommandBase<RestartElevatedCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Shell.RestartElevatedCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private RestartElevatedCommand(PackageBase package)
            : base(package, PackageCommands.ShellRestartElevatedCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new RestartElevatedCommand(package);
            await Instance.InitializeAsync();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnBeforeQueryStatusAsync(OleMenuCommand command)
        {
            command.Visible = !await Package.GetIsRunningElevatedAsync();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnExecuteAsync(OleMenuCommand command)
        {
            await Package.RestartAsync(RestartMode.Elevated);
        }

        /// <summary>
        ///     Called when [change].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The task.</returns>
        protected override async Task OnChangeAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }
    }
}