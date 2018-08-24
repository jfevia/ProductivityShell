namespace Jfevia.ProductivityShell.Vsix.Settings
{
    public class SettingsFile
    {
        /// <summary>
        ///     Gets or sets the full path.
        /// </summary>
        /// <value>
        ///     The full path.
        /// </value>
        public string FullPath { get; set; }

        /// <summary>
        ///     Gets or sets the relative path.
        /// </summary>
        /// <value>
        ///     The relative path.
        /// </value>
        public string RelativePath { get; set; }

        /// <summary>
        ///     Gets or sets the name of the directory.
        /// </summary>
        /// <value>
        ///     The name of the directory.
        /// </value>
        public string DirectoryName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the file.
        /// </summary>
        /// <value>
        ///     The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the designer file path.
        /// </summary>
        /// <value>
        ///     The designer file path.
        /// </value>
        public string DesignerFilePath { get; set; }

        /// <summary>
        ///     Gets or sets the name of the designer file.
        /// </summary>
        /// <value>
        ///     The name of the designer file.
        /// </value>
        public string DesignerFileName { get; set; }
    }
}