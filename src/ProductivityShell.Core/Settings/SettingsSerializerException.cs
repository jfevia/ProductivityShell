using System;

namespace ProductivityShell.Core
{
    public class SettingsSerializerException : ApplicationException
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Core.SettingsSerializerException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public SettingsSerializerException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Core.SettingsSerializerException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public SettingsSerializerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}