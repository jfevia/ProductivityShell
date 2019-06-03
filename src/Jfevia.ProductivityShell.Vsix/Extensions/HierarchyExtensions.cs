using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    internal static class HierarchyExtensions
    {
        /// <summary>
        ///     Converts this hierarchy into its project equivalent.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns>The project.</returns>
        public static async Task<Project> ToProjectAsync(this IVsHierarchy hierarchy)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) __VSHPROPID.VSHPROPID_ExtObject, out var output));
            return (Project) output;
        }
    }
}