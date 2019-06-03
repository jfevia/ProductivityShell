using System;

namespace Jfevia.ProductivityShell.Vsix.Helpers
{
    public class ProjectGuidResult
    {
        /// <summary>
        ///     Gets or sets the handler result.
        /// </summary>
        /// <value>
        ///     The handler result.
        /// </value>
        public int HandlerResult { get; set; }

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        /// <value>
        ///     The result.
        /// </value>
        public Guid Guid { get; set; }
    }
}