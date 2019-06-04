using System;
using System.Threading.Tasks;
using Jfevia.ProductivityShell.Vsix.DialogPages;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Helpers
{
    /// <summary>
    ///     A helper class for writing messages to a ProductivityShell output window pane.
    /// </summary>
    internal static class OutputWindowHelper
    {
        private static IVsOutputWindowPane _packageOutputWindowPane;

        private static async Task<IVsOutputWindowPane> GetPackageOutputWindowPaneAsync() => _packageOutputWindowPane ?? (_packageOutputWindowPane = await GetInternalPackageOutputWindowPaneAsync());

        /// <summary>
        ///     Writes the specified diagnostic line to the ProductivityShell output pane, but only if diagnostics are enabled.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">An optional exception that was handled.</param>
        internal static async Task DiagnosticWriteLineAsync(string message, Exception ex = null)
        {
            var generalOptionsPage = Package.Instance.GetDialogPage<GeneralDialogPage>();
            if (!generalOptionsPage.DiagnosticsMode)
                return;

            if (ex != null)
                message += $": {ex}";

            await WriteLineAsync("Diagnostic", message);
        }

        /// <summary>
        ///     Writes the specified exception line to the ProductivityShell output pane.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception that was handled.</param>
        internal static async Task ExceptionWriteLineAsync(string message, Exception ex)
        {
            var exceptionMessage = $"{message}: {ex}";

            await WriteLineAsync("Handled Exception", exceptionMessage);
        }

        /// <summary>
        ///     Writes the specified warning line to the ProductivityShell output pane.
        /// </summary>
        /// <param name="message">The message.</param>
        internal static async Task WarningWriteLineAsync(string message)
        {
            await WriteLineAsync("Warning", message);
        }

        /// <summary>
        ///     Attempts to create and retrieve the ProductivityShell output window pane.
        /// </summary>
        /// <returns>The ProductivityShell output window pane, otherwise null.</returns>
        private static async Task<IVsOutputWindowPane> GetInternalPackageOutputWindowPaneAsync()
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (!(Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsOutputWindow)) is IVsOutputWindow outputWindow))
                return null;

            var outputPaneGuid = new Guid(PackageGuids.OutputPaneString);

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            outputWindow.CreatePane(ref outputPaneGuid, PackageConstants.ProductName, 1, 1);
            outputWindow.GetPane(ref outputPaneGuid, out var windowPane);

            return windowPane;
        }

        /// <summary>
        ///     Writes the specified line to the ProductivityShell output pane.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        private static async Task WriteLineAsync(string category, string message)
        {
            var outputWindowPane = await GetPackageOutputWindowPaneAsync();
            if (outputWindowPane != null)
            {
                var outputMessage = $"[{PackageConstants.ProductName} {category} {DateTime.Now:hh:mm:ss tt}] {message}{Environment.NewLine}";

                await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
                outputWindowPane.OutputString(outputMessage);
            }
        }
    }
}