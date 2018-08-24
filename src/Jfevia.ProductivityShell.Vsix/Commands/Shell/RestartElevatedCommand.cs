﻿using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;

namespace Jfevia.ProductivityShell.Vsix.Commands.Shell
{
    internal sealed class RestartElevatedCommand : CommandBase<RestartElevatedCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Shell.RestartElevatedCommand" /> class.
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
        public static void Initialize(PackageBase package)
        {
            Instance = new RestartElevatedCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnBeforeQueryStatus(OleMenuCommand command)
        {
            command.Visible = !Package.IsRunningElevated;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            Package.Restart(RestartMode.Elevated);
        }
    }
}