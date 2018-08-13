using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;

namespace ProductivityShell.Helpers
{
    internal class FocusHelpers
    {
        /// <summary>
        ///     The ensure focus property.
        /// </summary>
        public static readonly DependencyProperty EnsureFocusProperty = DependencyProperty.RegisterAttached("EnsureFocus", typeof(bool), typeof(FocusHelpers), new PropertyMetadata(OnEnsureFocusChanged));

        /// <summary>
        ///     Gets the ensure focus.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static bool GetEnsureFocus(DependencyObject obj)
        {
            return (bool) obj.GetValue(EnsureFocusProperty);
        }

        /// <summary>
        ///     Sets the ensure focus.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetEnsureFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(EnsureFocusProperty, value);
        }

        /// <summary>
        ///     Called when [ensure focus changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static async void OnEnsureFocusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
                return;

            if (!(dependencyObject is Control control))
                return;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            control.Focus();
        }
    }
}