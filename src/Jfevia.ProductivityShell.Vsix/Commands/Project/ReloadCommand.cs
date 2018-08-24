﻿using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Helpers;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
        public static void Initialize(PackageBase package)
        {
            Instance = new ReloadCommand(package);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void OnExecute(OleMenuCommand command)
        {
            if (Package.Dte.SelectedItems.Count == 0)
                return;

            var solution = Package.GetGlobalService<SVsSolution, IVsSolution>();

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
                        continue;

                    OutputWindowHelper.DiagnosticWriteLine($"Reloading project: {projectItem.ContainingProject.Name}");
                    SolutionHelper.ReloadProject(solution, projectItem.ContainingProject);
                }
            }
        }
    }
}