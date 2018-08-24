namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    internal static class PackageExtensions
    {
        /// <summary>
        ///     Gets type-based services from the VSPackage service container
        /// </summary>
        /// <typeparam name="TSource">The type of the source service.</typeparam>
        /// <typeparam name="TTarget">The type of the target service.</typeparam>
        /// <returns>An instance of the requested service, or null if the service could not be found.</returns>
        public static TTarget GetGlobalService<TSource, TTarget>(this Microsoft.VisualStudio.Shell.Package package)
            where TTarget : class
        {
            return Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(TSource)) as TTarget;
        }
    }
}