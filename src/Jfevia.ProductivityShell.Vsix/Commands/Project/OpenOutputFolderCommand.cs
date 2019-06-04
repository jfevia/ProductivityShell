using System.Collections.Generic;
using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Project
{
    internal sealed class OpenOutputFolderCommand : CommandBase<OpenOutputFolderCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Project.OpenOutputFolderCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private OpenOutputFolderCommand(PackageBase package)
            : base(package, PackageCommands.ProjectOpenOutputFolderCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new OpenOutputFolderCommand(package);
            await Instance.InitializeAsync();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnExecuteAsync(OleMenuCommand command)
        {
            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (Package.Dte.SelectedItems.Count == 0)
                return;

            var paths = new HashSet<string>();

            foreach (SelectedItem selectedItem in Package.Dte.SelectedItems)
            {
                var project = selectedItem.Project;
                if (project != null)
                {
                    paths.Add(project.GetOutputFolder());
                }
                else
                {
                    var projectItem = selectedItem.ProjectItem;
                    if (projectItem?.ContainingProject == null)
                        continue;

                    paths.Add(projectItem.ContainingProject.GetOutputFolder());
                }
            }

            foreach (var path in paths)
                await Task.Run(() => Process.Start(path));
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