using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Shell
{
    internal sealed class RestartNormalCommand : CommandBase<RestartNormalCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Commands.Shell.RestartNormalCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private RestartNormalCommand(PackageBase package)
            : base(package, PackageIds.ShellRestartNormalCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static void Initialize(PackageBase package)
        {
            Instance = new RestartNormalCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            Package.Restart(RestartMode.Normal);
        }
    }
}