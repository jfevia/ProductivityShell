using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Helpers;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.ProjectItem
{
    internal sealed class ShowInExplorerCommand : CommandBase<ShowInExplorerCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ShowInExplorerCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ShowInExplorerCommand(PackageBase package) : base(package, PackageIds.ProjectItemShowInExplorerCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new ShowInExplorerCommand(package);
        }

        protected override void OnExecute(OleMenuCommand command)
        {
            if (Package.Dte.SelectedItems.Count == 0)
            {
                return;
            }

            var paths = new HashSet<string>();

            foreach (var selectedItem in UIHierarchyHelper.GetSelectedUIHierarchyItems(Package))
            {
                var projectItem = selectedItem.Object as EnvDTE.ProjectItem;
                if (projectItem != null)
                {
                    paths.Add(projectItem.Document?.FullName ?? projectItem.Properties?.Item("FullPath")
                                                                           .Value.ToString());
                }

                var project = selectedItem.Object as EnvDTE.Project;
                if (project != null)
                {
                    paths.Add(project.FullName);
                }

                var solution = selectedItem.Object as Solution;
                if (solution != null)
                {
                    paths.Add(solution.FullName);
                }
            }

            var groupedPaths = paths.GroupBy(Path.GetDirectoryName);
            foreach (var path in groupedPaths)
            {
                WindowsExplorerHelper.FilesOrFolders(path);
            }
        }
    }
}