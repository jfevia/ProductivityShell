using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Jfevia.ProductivityShell.Vsix.Shell;

namespace Jfevia.ProductivityShell.Vsix.Helpers
{
    /// <summary>
    ///     A static helper class for working with the UI hierarchies.
    /// </summary>
    internal static class UIHierarchyHelper
    {
        /// <summary>
        ///     Causes the given item and all of its expanded children to be collapsed. This may cause
        ///     selections to change.
        /// </summary>
        /// <param name="parentItem">The parent item to collapse from.</param>
        internal static async Task CollapseRecursivelyAsync(UIHierarchyItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (!parentItem.UIHierarchyItems.Expanded)
                return;

            // Recurse to all children first.
            foreach (UIHierarchyItem childItem in parentItem.UIHierarchyItems)
                await CollapseRecursivelyAsync(childItem);

            if (await ShouldCollapseItemAsync(parentItem))
            {
                // Attempt the direct collapse first.
                parentItem.UIHierarchyItems.Expanded = false;

                // If failed, solution folder oddity may be at play. Try an alternate path.
                if (parentItem.UIHierarchyItems.Expanded)
                {
                    parentItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
                    ((DTE2) parentItem.DTE).ToolWindows.SolutionExplorer.DoDefaultAction();
                }
            }
        }

        /// <summary>
        ///     Gets an enumerable set of the selected UI hierarchy items.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <returns>The enumerable set of selected UI hierarchy items.</returns>
        internal static async Task<IEnumerable<UIHierarchyItem>> GetSelectedUIHierarchyItemsAsync(PackageBase package)
        {
            var solutionExplorer = GetSolutionExplorer(package);

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            return ((object[]) solutionExplorer.SelectedItems).Cast<UIHierarchyItem>().ToList();
        }

        /// <summary>
        ///     Gets the solution explorer for the specified hosting package.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <returns>The solution explorer.</returns>
        internal static UIHierarchy GetSolutionExplorer(PackageBase package)
        {
            return package.Dte.ToolWindows.SolutionExplorer;
        }

        /// <summary>
        ///     Gets the top level (solution) UI hierarchy item.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        /// <returns>The top level (solution) UI hierarchy item, otherwise null.</returns>
        internal static async Task<UIHierarchyItem> GetTopUIHierarchyItemAsync(PackageBase package)
        {
            var solutionExplorer = GetSolutionExplorer(package);

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            return solutionExplorer.UIHierarchyItems.Count > 0 ? solutionExplorer.UIHierarchyItems.Item(1) : null;
        }

        /// <summary>
        ///     Determines whether the specified item has any expanded children.
        /// </summary>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>True if there are expanded children, false otherwise.</returns>
        internal static async Task<bool> HasExpandedChildrenAsync(UIHierarchyItem parentItem)
        {
            if (parentItem == null)
                throw new ArgumentNullException(nameof(parentItem));

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            foreach (UIHierarchyItem childItem in parentItem.UIHierarchyItems)
            {
                if (childItem.UIHierarchyItems.Expanded || await HasExpandedChildrenAsync(childItem))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines if the specified parent item should be collapsed.
        /// </summary>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>True if the item should be collapsed, otherwise false.</returns>
        private static async Task<bool> ShouldCollapseItemAsync(UIHierarchyItem parentItem)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Make sure not to collapse the solution, causes odd behavior.
            if (parentItem.Object is Solution)
                return false;

            // Conditionally skip collapsing the only project in a solution.
            // Note: Visual Studio automatically creates a second invisible project called
            //       "Miscellaneous files".
            if (!(parentItem.Object is Project))
                return true;

            var solution = parentItem.DTE.Solution;
            var hasCollapsibleItem = false;
            foreach (var project in solution.Projects)
            {
                if (!(project is Project s))
                    continue;

                if (s == parentItem.Object || s.Name == "Miscellaneous Files")
                    continue;

                hasCollapsibleItem = true;
                break;
            }

            return hasCollapsibleItem;
        }
    }
}