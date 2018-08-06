using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProductivityShell.Core
{
    public static class HierarchyExtensions
    {
        public static ServiceProvider GetServiceProvider(this IVsHierarchy hierarchy)
        {
            ErrorHandler.ThrowOnFailure(hierarchy.GetSite(out var serviceProviderInterop));
            return new ServiceProvider(serviceProviderInterop);
        }

        public static Project GetProject(this IVsHierarchy hierarchy)
        {
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty((uint) VSConstants.VSITEMID.Root,
                (int) __VSHPROPID.VSHPROPID_ExtObject, out var pvar));
            return (Project) pvar;
        }
    }
}