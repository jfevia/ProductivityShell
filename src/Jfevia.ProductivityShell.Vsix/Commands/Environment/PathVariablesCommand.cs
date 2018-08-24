using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;

namespace Jfevia.ProductivityShell.Vsix.Commands.Environment
{
    internal sealed class PathVariablesCommand : CommandBase<PathVariablesCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Environment.PathVariablesCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private PathVariablesCommand(PackageBase package)
            : base(package, PackageCommands.EnvironmentPathVariablesCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static void Initialize(PackageBase package)
        {
            Instance = new PathVariablesCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            var pane = Package?.PackageOutputPane;
            if (pane == null || Package?.ActivateOutputWindow() == false)
                return;

            const string semiColon = ";";
            var expanded = System.Environment.ExpandEnvironmentVariables("%path%");
            var text = expanded.Replace(semiColon, System.Environment.NewLine);

            pane.Activate();
            pane.Clear();
            pane.OutputString("Path Variables" + System.Environment.NewLine);
            pane.OutputString("==============" + System.Environment.NewLine);
            pane.OutputString(text);
        }
    }
}