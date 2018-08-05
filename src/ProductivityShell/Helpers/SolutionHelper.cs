using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ProductivityShell.Shell;

namespace ProductivityShell.Helpers
{
    /// <summary>
    ///     A static helper class for working with the solution.
    /// </summary>
    internal static class SolutionHelper
    {
        /// <summary>
        ///     Gets an enumerable set of all items of the specified type within the solution.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="solution">The solution.</param>
        /// <returns>The enumerable set of all items.</returns>
        internal static IEnumerable<T> GetAllItemsInSolution<T>(Solution solution)
            where T : class
        {
            var allProjects = new List<T>();

            if (solution != null)
                allProjects.AddRange(GetItemsRecursively<T>(solution));

            return allProjects;
        }

        /// <summary>
        ///     Gets items of the specified type recursively from the specified parent item. Includes
        ///     the parent item if it matches the specified type as well.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>The enumerable set of items within the parent item, may be empty.</returns>
        internal static IEnumerable<T> GetItemsRecursively<T>(object parentItem)
            where T : class
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));

            // Create a collection.
            var projectItems = new List<T>();

            // Include the parent item if it is of the desired type.
            if (parentItem is T desiredType)
                projectItems.Add(desiredType);

            // Get all children based on the type of parent item.
            var children = GetChildren(parentItem);

            // Then recurse through all children.
            foreach (var childItem in children)
                projectItems.AddRange(GetItemsRecursively<T>(childItem));

            return projectItems;
        }

        /// <summary>
        ///     Reloads the project.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code. If the project was not previously unloaded, then this method does nothing and returns
        ///     <see cref="F:Microsoft.VisualStudio.VSConstants.S_FALSE" />.
        /// </returns>
        internal static int ReloadProject(IVsSolution solution, string projectName)
        {
            int hr;
            if ((hr = solution.GetProjectOfUniqueName(projectName, out var projectHierarchy)) != VSConstants.S_OK)
                return hr;

            if ((hr = solution.GetGuidOfProject(projectHierarchy, out var projectGuid)) != VSConstants.S_OK)
                return hr;

            return ReloadProject(solution, projectGuid);
        }

        /// <summary>
        ///     Gets an enumerable set of the selected project items.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <returns>The enumerable set of selected project items.</returns>
        internal static IEnumerable<ProjectItem> GetSelectedProjectItemsRecursively(PackageBase package)
        {
            var selectedProjectItems = new List<ProjectItem>();
            var selectedUiHierarchyItems = UIHierarchyHelper.GetSelectedUIHierarchyItems(package);

            foreach (var item in selectedUiHierarchyItems.Select(uiHierarchyItem => uiHierarchyItem.Object))
                selectedProjectItems.AddRange(GetItemsRecursively<ProjectItem>(item));

            return selectedProjectItems;
        }

        /// <summary>
        ///     Gets an enumerable set of similar project items compared by file name, useful for shared projects.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <param name="projectItem">The project item to match.</param>
        /// <returns>The enumerable set of similar project items.</returns>
        internal static IEnumerable<ProjectItem> GetSimilarProjectItems(PackageBase package, ProjectItem projectItem)
        {
            var allItems = GetAllItemsInSolution<ProjectItem>(package.Dte.Solution);

            return allItems.Where(x =>
                x.Name == projectItem.Name && x.Kind == projectItem.Kind &&
                x.Document.FullName == projectItem.Document.FullName);
        }

        /// <summary>
        ///     Gets the children of the specified parent item if applicable.
        /// </summary>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>An enumerable set of children, may be empty.</returns>
        private static IEnumerable<object> GetChildren(object parentItem)
        {
            // First check if the item is a solution.
            var solution = parentItem as Solution;
            if (solution?.Projects != null)
                return solution.Projects.Cast<Project>().Cast<object>().ToList();

            // Next check if the item is a project.
            var project = parentItem as Project;
            if (project?.ProjectItems != null)
                return project.ProjectItems.Cast<ProjectItem>().Cast<object>().ToList();

            // Next check if the item is a project item.
            if (parentItem is ProjectItem projectItem)
            {
                // Standard projects.
                if (projectItem.ProjectItems != null)
                    return projectItem.ProjectItems.Cast<ProjectItem>().Cast<object>().ToList();

                // Projects within a solution folder.
                if (projectItem.SubProject != null)
                    return new[] {projectItem.SubProject};
            }

            // Otherwise return an empty array.
            return new object[0];
        }

        public static int GetGuidOfProject(IVsSolution solution, Project project, out Guid projectGuid)
        {
            projectGuid = default(Guid);

            int hr;
            if ((hr = solution.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy)) !=
                VSConstants.S_OK)
                return hr;

            return solution.GetGuidOfProject(projectHierarchy, out projectGuid);
        }

        public static int UnloadProject(IVsSolution solution, Project project)
        {
            int hr;
            if ((hr = GetGuidOfProject(solution, project, out var projectGuid)) != VSConstants.S_OK)
                return hr;

            return UnloadProject((IVsSolution4) solution, projectGuid);
        }

        public static int ReloadProject(IVsSolution solution, Guid projectGuid)
        {
            var vsSolution4 = (IVsSolution4) solution;
            UnloadProject(vsSolution4, projectGuid);

            return LoadProject(vsSolution4, projectGuid);
        }

        public static int ReloadProject(IVsSolution solution, Project project)
        {
            int hr;
            if ((hr = GetGuidOfProject(solution, project, out var projectGuid)) != VSConstants.S_OK)
                return hr;

            return ReloadProject(solution, projectGuid);
        }

        public static int LoadProject(IVsSolution solution, Project project)
        {
            int hr;
            if ((hr = GetGuidOfProject(solution, project, out var projectGuid)) != VSConstants.S_OK)
                return hr;

            return LoadProject((IVsSolution4) solution, projectGuid);
        }

        public static int UnloadProject(IVsSolution4 solution, Guid projectGuid)
        {
            return solution.UnloadProject(ref projectGuid, (uint) _VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);
        }

        public static int LoadProject(IVsSolution4 solution, Guid projectGuid)
        {
            return solution.ReloadProject(ref projectGuid);
        }
    }
}