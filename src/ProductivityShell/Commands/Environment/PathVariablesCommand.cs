using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Environment
{
    internal sealed class PathVariablesCommand : CommandBase<PathVariablesCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PathVariablesCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private PathVariablesCommand(PackageBase package)
            : base(package, PackageIds.EnvironmentPathVariablesCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new PathVariablesCommand(package);
        }

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