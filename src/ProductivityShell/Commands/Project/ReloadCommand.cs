using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProductivityShell.Helpers;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Project
{
    internal sealed class ReloadCommand : CommandBase<ReloadCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReloadCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ReloadCommand(PackageBase package) : base(package, PackageIds.ProjectReloadCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new ReloadCommand(package);
        }

        protected override void OnExecute(OleMenuCommand command)
        {
            if (Package.Dte.SelectedItems.Count == 0)
            {
                return;
            }

            var solution = PackageBase.GetGlobalService<SVsSolution, IVsSolution>();

            foreach (SelectedItem selectedItem in Package.Dte.SelectedItems)
            {
                var project = selectedItem.Project;
                if (project != null)
                {
                    OutputWindowHelper.DiagnosticWriteLine($"Reloading project: {project.Name}");
                    SolutionHelper.ReloadProject(solution, project);
                }
                else
                {
                    var projectItem = selectedItem.ProjectItem;
                    if (projectItem?.ContainingProject == null)
                    {
                        // Project may be already unloaded
                        continue;
                    }

                    OutputWindowHelper.DiagnosticWriteLine($"Reloading project: {projectItem.ContainingProject.Name}");
                    SolutionHelper.ReloadProject(solution, projectItem.ContainingProject);
                }
            }
        }
    }
}