using System;

namespace Jfevia.ProductivityShell.ProjectModel
{
    public class Project
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Project" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Project(string name)
        {
            Name = name;
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
        ///     Renames the project with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Rename(string name)
        {
            var oldName = Name;
            var newName = name;

            Renamed?.Invoke(this, new RenamedProjectEventArgs(oldName, newName));
        }
    }
}