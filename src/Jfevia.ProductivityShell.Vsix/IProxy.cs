namespace Jfevia.ProductivityShell.Vsix
{
    internal interface IProxy
    {
    }

    internal interface IProxy<out TSource, out TTarget> : IProxy
    {
        /// <summary>
        ///     Gets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        TSource Source { get; }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        TTarget Target { get; }
    }
}