﻿using System.Collections.Generic;
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
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Commands.ProjectItem.ShowInExplorerCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ShowInExplorerCommand(PackageBase package)
            : base(package, PackageIds.ProjectItemShowInExplorerCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static void Initialize(PackageBase package)
        {
            Instance = new ShowInExplorerCommand(package);
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

            foreach (var selectedItem in UIHierarchyHelper.GetSelectedUIHierarchyItems(Package))
            {
                if (selectedItem.Object is EnvDTE.ProjectItem projectItem)
                    paths.Add(projectItem.Document?.FullName ??
                              projectItem.Properties?.Item("FullPath").Value.ToString());

                if (selectedItem.Object is EnvDTE.Project project)
                    paths.Add(project.FullName);

                if (selectedItem.Object is Solution solution)
                    paths.Add(solution.FullName);
            }

            var groupedPaths = paths.GroupBy(Path.GetDirectoryName);
            foreach (var path in groupedPaths)
                WindowsExplorerHelper.FilesOrFolders(path);
        }
    }
}