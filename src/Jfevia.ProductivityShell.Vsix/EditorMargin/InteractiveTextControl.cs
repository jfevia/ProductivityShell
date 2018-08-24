using System;
using System.Windows;
using System.Windows.Input;

namespace Jfevia.ProductivityShell.Vsix.EditorMargin
{
    internal class InteractiveTextControl : TextControl
    {
        public EventHandler<MouseButtonEventArgs> ValueMouseDown;

        public InteractiveTextControl(string name, string value = "Loading...")
            : base(name, value)
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBlockValue.PreviewMouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ValueMouseDown?.Invoke(this, e);
        }
    }
}