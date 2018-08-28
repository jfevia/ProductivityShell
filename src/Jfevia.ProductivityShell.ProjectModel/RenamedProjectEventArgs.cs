using System;

namespace Jfevia.ProductivityShell.ProjectModel
{
    public class RenamedProjectEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.ProjectModel.RenamedProjectEventArgs" />
        ///     class.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public RenamedProjectEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        /// <summary>
        ///     Gets the old name.
        /// </summary>
        /// <value>
        ///     The old name.
        /// </value>
        public string OldName { get; }

        /// <summary>
        ///     Gets the new name.
        /// </summary>
        /// <value>
        ///     The new name.
        /// </value>
        public string NewName { get; }
    }
}