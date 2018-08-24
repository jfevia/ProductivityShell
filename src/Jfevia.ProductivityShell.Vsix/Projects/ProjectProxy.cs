using System;
using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Solutions;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Projects
{
    internal class ProjectProxyBase
    {
    }

    internal class ProjectProxy : ProjectProxyBase, IDisposable
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the hierarchy.
        /// </summary>
        /// <value>
        ///     The hierarchy.
        /// </value>
        public IVsHierarchy Hierarchy { get; set; }

        /// <summary>
        ///     Gets or sets the vs project.
        /// </summary>
        /// <value>
        ///     The vs project.
        /// </value>
        public Project VsProject { get; set; }

        /// <summary>
        ///     Gets or sets the solution proxy.
        /// </summary>
        /// <value>
        ///     The solution proxy.
        /// </value>
        public SolutionProxy SolutionProxy { get; set; }

        /// <summary>
        ///     Gets or sets the relative path.
        /// </summary>
        /// <value>
        ///     The relative path.
        /// </value>
        public string RelativePath { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}