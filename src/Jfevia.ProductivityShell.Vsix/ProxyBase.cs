namespace Jfevia.ProductivityShell.Vsix
{
    internal class ProxyBase : IProxy
    {
    }

    internal class ProxyBase<TSource, TTarget> : ProxyBase, IProxy<TSource, TTarget>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Gets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        public TSource Source { get; protected set; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        public TTarget Target { get; protected set; }
    }
}