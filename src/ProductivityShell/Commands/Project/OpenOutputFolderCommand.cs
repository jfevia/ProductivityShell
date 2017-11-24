using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Extensions;
using ProductivityShell.Shell;
using Process = System.Diagnostics.Process;

namespace ProductivityShell.Commands.Project
{
    internal sealed class OpenOutputFolderCommand : CommandBase<OpenOutputFolderCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OpenOutputFolderCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private OpenOutputFolderCommand(PackageBase package)
            : base(package, PackageIds.ProjectOpenOutputFolderCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new OpenOutputFolderCommand(package);
        }

        protected override void OnExecute(OleMenuCommand command)
        {
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
                Process.Start(path);
        }
    }
}