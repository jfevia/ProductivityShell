using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Shell
{
    internal sealed class RestartElevatedCommand : CommandBase<RestartElevatedCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RestartElevatedCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private RestartElevatedCommand(PackageBase package) : base(package, PackageIds.ShellRestartElevatedCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new RestartElevatedCommand(package);
        }

        protected override void OnBeforeQueryStatus(OleMenuCommand command)
        {
            command.Visible = !Package.IsRunningElevated;
        }

        protected override void OnExecute(OleMenuCommand command)
        {
            Package.Restart(RestartMode.Elevated);
        }
    }
}