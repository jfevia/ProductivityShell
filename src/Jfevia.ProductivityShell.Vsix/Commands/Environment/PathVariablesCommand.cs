using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Environment
{
    internal sealed class PathVariablesCommand : CommandBase<PathVariablesCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Environment.PathVariablesCommand" /> class.
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
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new PathVariablesCommand(package);
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
        protected override async Task OnExecuteAsync(OleMenuCommand command)
        {
            var pane = Package?.PackageOutputPane;
            if (pane == null || Package?.ActivateOutputWindow() == false)
                return;

            const string semiColon = ";";
            var expanded = await Task.Run(() => System.Environment.ExpandEnvironmentVariables("%path%"));
            var text = expanded.Replace(semiColon, System.Environment.NewLine);

            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            pane.Activate();
            pane.Clear();
            pane.OutputString("Path Variables" + System.Environment.NewLine);
            pane.OutputString("==============" + System.Environment.NewLine);
            pane.OutputString(text);
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