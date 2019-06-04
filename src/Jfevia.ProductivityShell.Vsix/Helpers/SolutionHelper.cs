using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Helpers
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
        internal static async Task<IEnumerable<T>> GetAllItemsInSolutionAsync<T>(Solution solution)
            where T : class
        {
            var allProjects = new List<T>();

            if (solution != null)
                allProjects.AddRange(await GetItemsRecursivelyAsync<T>(solution));

            return allProjects;
        }

        /// <summary>
        ///     Gets items of the specified type recursively from the specified parent item. Includes
        ///     the parent item if it matches the specified type as well.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>The enumerable set of items within the parent item, may be empty.</returns>
        internal static async Task<IEnumerable<T>> GetItemsRecursivelyAsync<T>(object parentItem)
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
            var children = await GetChildrenAsync(parentItem);

            // Then recurse through all children.
            foreach (var childItem in children)
                projectItems.AddRange(await GetItemsRecursivelyAsync<T>(childItem));

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
        internal static async Task<int> ReloadProjectAsync(IVsSolution solution, string projectName)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            int hr;
            if ((hr = solution.GetProjectOfUniqueName(projectName, out var projectHierarchy)) != VSConstants.S_OK)
                return hr;

            if ((hr = solution.GetGuidOfProject(projectHierarchy, out var projectGuid)) != VSConstants.S_OK)
                return hr;

            return await ReloadProjectAsync(solution, projectGuid);
        }

        /// <summary>
        ///     Gets an enumerable set of the selected project items.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <returns>The enumerable set of selected project items.</returns>
        internal static async Task<IEnumerable<ProjectItem>> GetSelectedProjectItemsRecursivelyAsync(PackageBase package)
        {
            var selectedProjectItems = new List<ProjectItem>();
            var selectedUiHierarchyItems = await UIHierarchyHelper.GetSelectedUIHierarchyItemsAsync(package);

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            foreach (var item in selectedUiHierarchyItems)
            {
                var items = await GetItemsRecursivelyAsync<ProjectItem>(item.Object);
                selectedProjectItems.AddRange(items);
            }

            return selectedProjectItems;
        }

        /// <summary>
        ///     Gets an enumerable set of similar project items compared by file name, useful for shared projects.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <param name="projectItem">The project item to match.</param>
        /// <returns>The enumerable set of similar project items.</returns>
        internal static async Task<IEnumerable<ProjectItem>> GetSimilarProjectItemsAsync(PackageBase package, ProjectItem projectItem)
        {
            var allItems = await GetAllItemsInSolutionAsync<ProjectItem>(package.Dte.Solution);
            var items = new List<ProjectItem>();

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            foreach (var item in allItems)
            {
                if (item.Name == projectItem.Name && item.Kind == projectItem.Kind && item.Document.FullName == projectItem.Document.FullName)
                    items.Add(item);
            }

            return items;
        }

        /// <summary>
        ///     Gets the children of the specified parent item if applicable.
        /// </summary>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>An enumerable set of children, may be empty.</returns>
        private static async Task<IEnumerable<object>> GetChildrenAsync(object parentItem)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

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

        public static async Task<ProjectGuidResult> GetGuidOfProjectAsync(IVsSolution solution, Project project)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            int hr;
            if ((hr = solution.GetProjectOfUniqueName(project.UniqueName, out var projectHierarchy)) !=
                VSConstants.S_OK)
                return new ProjectGuidResult {HandlerResult = hr};

            hr = solution.GetGuidOfProject(projectHierarchy, out var projectGuid);
            return new ProjectGuidResult {HandlerResult = hr, Guid = projectGuid};
        }

        public static async Task<int> UnloadProjectAsync(IVsSolution solution, Project project)
        {
            var projectGuidResult = await GetGuidOfProjectAsync(solution, project);
            if (projectGuidResult.HandlerResult != VSConstants.S_OK)
                return projectGuidResult.HandlerResult;

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            return await UnloadProjectAsync((IVsSolution4) solution, projectGuidResult.Guid);
        }

        public static async Task<int> ReloadProjectAsync(IVsSolution solution, Guid projectGuid)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            var vsSolution4 = (IVsSolution4) solution;
            await UnloadProjectAsync(vsSolution4, projectGuid);

            return await LoadProjectAsync(vsSolution4, projectGuid);
        }

        public static async Task<int> ReloadProjectAsync(IVsSolution solution, Project project)
        {
            var projectGuidResult = await GetGuidOfProjectAsync(solution, project);
            if (projectGuidResult.HandlerResult != VSConstants.S_OK)
                return projectGuidResult.HandlerResult;

            return await ReloadProjectAsync(solution, projectGuidResult.Guid);
        }

        public static async Task<int> LoadProjectAsync(IVsSolution solution, Project project)
        {
            var projectGuidResult = await GetGuidOfProjectAsync(solution, project);
            if (projectGuidResult.HandlerResult != VSConstants.S_OK)
                return projectGuidResult.HandlerResult;

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            return await LoadProjectAsync((IVsSolution4) solution, projectGuidResult.Guid);
        }

        public static async Task<int> UnloadProjectAsync(IVsSolution4 solution, Guid projectGuid)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            return solution.UnloadProject(ref projectGuid, (uint) _VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);
        }

        public static async Task<int> LoadProjectAsync(IVsSolution4 solution, Guid projectGuid)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            return solution.ReloadProject(ref projectGuid);
        }
    }
}