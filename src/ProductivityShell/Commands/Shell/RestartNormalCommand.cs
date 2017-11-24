using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Shell
{
    internal sealed class RestartNormalCommand : CommandBase<RestartNormalCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RestartNormalCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private RestartNormalCommand(PackageBase package)
            : base(package, PackageIds.ShellRestartNormalCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new RestartNormalCommand(package);
        }

        protected override void OnExecute(OleMenuCommand command)
        {
            Package.Restart(RestartMode.Normal);
        }
    }
}