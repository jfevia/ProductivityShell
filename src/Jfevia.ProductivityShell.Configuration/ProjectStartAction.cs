namespace Jfevia.ProductivityShell.Configuration
{
    public enum ProjectStartAction
    {
        /// <summary>
        ///     The project does not have a start action.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The project is started.
        /// </summary>
        Start = 1,

        /// <summary>
        ///     The project is started without debugging.
        /// </summary>
        StartWithoutDebugging = 2
    }
}