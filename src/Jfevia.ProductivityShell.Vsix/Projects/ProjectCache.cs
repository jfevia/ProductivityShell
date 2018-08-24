using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Projects
{
    internal class ProjectCache
    {
        private readonly IDictionary<IVsHierarchy, ProjectProxy> _projectByHierarchy;
        private readonly IDictionary<string, ProjectProxy> _projectByName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectCache" /> class.
        /// </summary>
        public ProjectCache()
        {
            _projectByHierarchy = new Dictionary<IVsHierarchy, ProjectProxy>();
            _projectByName = new Dictionary<string, ProjectProxy>();
        }

        /// <summary>
        ///     Gets the projects.
        /// </summary>
        /// <returns>The projects.</returns>
        public IEnumerable<ProjectProxy> GetProjects()
        {
            return _projectByHierarchy.Values;
        }

        /// <summary>
        ///     Tries to find a project with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="project">The project.</param>
        /// <returns><c>true</c> if the project with the specified name is cached; otherwise, <c>false</c>.</returns>
        public bool TryGetProjectByName(string name, out ProjectProxy project)
        {
            return _projectByName.TryGetValue(name, out project);
        }

        /// <summary>
        ///     Tries to find a project with the specified hierarchy.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="project">The project.</param>
        /// <returns><c>true</c> if the project with the specified hierarchy is cached; otherwise, <c>false</c>.</returns>
        public bool TryGetProjectByHierarchy(IVsHierarchy hierarchy, out ProjectProxy project)
        {
            return _projectByHierarchy.TryGetValue(hierarchy, out project);
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        public void Clear()
        {
            _projectByHierarchy.Clear();
            _projectByName.Clear();
        }

        /// <summary>
        ///     Removes the specified project proxy.
        /// </summary>
        /// <param name="projectProxy">The project proxy.</param>
        /// <exception cref="ArgumentNullException">projectProxy</exception>
        public void Remove(ProjectProxy projectProxy)
        {
            if (projectProxy == null)
                throw new ArgumentNullException(nameof(projectProxy));

            _projectByHierarchy.Remove(projectProxy.Hierarchy);
            _projectByName.Remove(projectProxy.Name);
        }

        /// <summary>
        ///     Adds the specified project proxy.
        /// </summary>
        /// <param name="projectProxy">The project proxy.</param>
        public void Add(ProjectProxy projectProxy)
        {
            if (projectProxy == null)
                throw new ArgumentNullException(nameof(projectProxy));

            _projectByHierarchy.Add(projectProxy.Hierarchy, projectProxy);
            _projectByName.Add(projectProxy.Name, projectProxy);
        }

        /// <summary>
        ///     Renames the specified project proxy.
        /// </summary>
        /// <param name="projectProxy">The project proxy.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public void Rename(ProjectProxy projectProxy, string oldName, string newName)
        {
            _projectByName.Remove(oldName);
            _projectByName.Add(newName, projectProxy);
        }
    }
}