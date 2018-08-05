using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Extensions;
using ProductivityShell.Helpers;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Project
{
    internal sealed class ShowOutputFileInExplorerCommand : CommandBase<ShowOutputFileInExplorerCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Commands.Project.ShowOutputFileInExplorerCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ShowOutputFileInExplorerCommand(PackageBase package)
            : base(package, PackageIds.ProjectShowOutputFileInExplorerCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static void Initialize(PackageBase package)
        {
            Instance = new ShowOutputFileInExplorerCommand(package);
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

            var paths = new HashSet<string>();

            foreach (SelectedItem selectedItem in Package.Dte.SelectedItems)
            {
                var project = selectedItem.Project;
                if (project != null)
                {
                    paths.Add(project.GetOutputFilePath());
                }
                else
                {
                    var projectItem = selectedItem.ProjectItem;
                    if (projectItem?.ContainingProject == null)
                        continue;

                    paths.Add(projectItem.ContainingProject.GetOutputFilePath());
                }
            }

            var groupedPaths = paths.GroupBy(Path.GetDirectoryName);
            foreach (var path in groupedPaths)
                WindowsExplorerHelper.FilesOrFolders(path);
        }
    }
}