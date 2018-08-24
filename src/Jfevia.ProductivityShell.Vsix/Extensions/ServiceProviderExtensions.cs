using System;

namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    internal static class ServiceProviderExtensions
    {
        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>The service.</returns>
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T) serviceProvider.GetService(typeof(T));
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The service.</returns>
        public static TValue GetService<TService, TValue>(this IServiceProvider serviceProvider)
        {
            return (TValue) serviceProvider.GetService(typeof(TService));
        }
    }
}