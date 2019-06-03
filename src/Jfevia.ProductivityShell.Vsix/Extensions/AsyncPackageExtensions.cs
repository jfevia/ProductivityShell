using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    internal static class AsyncPackageExtensions
    {
        /// <summary>
        ///     Gets type-based services from the VSPackage service container
        /// </summary>
        /// <typeparam name="TSource">The type of the source service.</typeparam>
        /// <typeparam name="TTarget">The type of the target service.</typeparam>
        /// <returns>An instance of the requested service, or null if the service could not be found.</returns>
        public static TTarget GetGlobalService<TSource, TTarget>(this AsyncPackage package)
            where TTarget : class
        {
            return Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(TSource)) as TTarget;
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="package">The package.</param>
        /// <returns>The service.</returns>
        public static async Task<T> GetServiceAsync<T>(this AsyncPackage package)
        {
            return (T) await package.GetServiceAsync(typeof(T));
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="package">The package.</param>
        /// <returns>The service.</returns>
        public static async Task<TValue> GetServiceAsync<TService, TValue>(this AsyncPackage package)
        {
            return (TValue) await package.GetServiceAsync(typeof(TService));
        }
    }
}