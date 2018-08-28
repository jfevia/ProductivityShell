using System;
using Jfevia.ProductivityShell.ProjectModel;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Solutions;
using Microsoft.VisualStudio.Shell.Interop;
using Project = EnvDTE.Project;

namespace Jfevia.ProductivityShell.Vsix.Projects
{
    internal class ProjectProxyBase : ProxyBase<Project, ProjectModel.Project>
    {
    }

    internal class ProjectProxy : ProjectProxyBase, IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectProxy" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="source">The source.</param>
        public ProjectProxy(string name, Project source)
        {
            Name = name;
            Source = source;

            Target = new ProjectModel.Project(name);
            Target.Renamed += Target_Renamed;
        }

        /// <summary>
        ///     Occurs when [renamed].
        /// </summary>
        public event EventHandler<RenamedProjectEventArgs> Renamed;

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        ///     Gets or sets the hierarchy.
        /// </summary>
        /// <value>
        ///     The hierarchy.
        /// </value>
        public IVsHierarchy Hierarchy { get; set; }

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
            if (Target == null)
                return;

            Target.Renamed -= Target_Renamed;
        }

        /// <summary>
        ///     Handles the Renamed event of the Target control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenamedProjectEventArgs" /> instance containing the event data.</param>
        private void Target_Renamed(object sender, RenamedProjectEventArgs e)
        {
            Renamed?.Invoke(this, e);
        }

        /// <summary>
        ///     Renames the project with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Rename(string name)
        {
            Name = name;
            RelativePath = Source.GetRelativePath(SolutionProxy.Source);

            Target.Rename(name);
        }
    }
}