using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Helpers;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Project
{
    internal sealed class ReloadCommand : CommandBase<ReloadCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Project.ReloadCommand" />
        ///     class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ReloadCommand(PackageBase package)
            : base(package, PackageCommands.ProjectReloadCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new ReloadCommand(package);
            await Instance.InitializeAsync();
        }

        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
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
            if (Package.Dte.SelectedItems.Count == 0)
                return;

            var solution = Package.GetGlobalService<SVsSolution, IVsSolution>();

            foreach (SelectedItem selectedItem in Package.Dte.SelectedItems)
            {
                var project = selectedItem.Project;
                if (project != null)
                {
                    await OutputWindowHelper.DiagnosticWriteLineAsync($"Reloading project: {project.Name}");
                    await SolutionHelper.ReloadProjectAsync(solution, project);
                }
                else
                {
                    var projectItem = selectedItem.ProjectItem;
                    if (projectItem?.ContainingProject == null)
                        continue;

                    await OutputWindowHelper.DiagnosticWriteLineAsync($"Reloading project: {projectItem.ContainingProject.Name}");
                    await SolutionHelper.ReloadProjectAsync(solution, projectItem.ContainingProject);
                }
            }
        }

        /// <summary>
        ///     Called when [change].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnChangeAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }
    }
}