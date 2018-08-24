namespace Jfevia.ProductivityShell.Configuration
{
    public static class ProjectExtensions
    {
        /// <summary>
        ///     Gets the start action.
        /// </summary>
        /// <value>
        ///     The start action.
        /// </value>
        public static ProjectStartAction GetStartAction(this Project project)
        {
            if (!string.IsNullOrWhiteSpace(project.StartExternalProgram))
                return ProjectStartAction.Start;

            if (!string.IsNullOrWhiteSpace(project.StartBrowserUrl))
                return ProjectStartAction.StartWithoutDebugging;

            return ProjectStartAction.None;
        }
    }
}