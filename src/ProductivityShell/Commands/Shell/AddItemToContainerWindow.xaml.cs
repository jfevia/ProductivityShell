using System.Windows;
using System.Windows.Controls;
using EnvDTE;
using Microsoft.VisualStudio.PlatformUI;

namespace ProductivityShell.Commands.Shell
{
    public partial class AddItemToCommandBarWindow : DialogWindow
    {
        /// <summary>
        ///     The command name dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandNameDependencyProperty = DependencyProperty.Register(
            nameof(CommandName), typeof(string), typeof(AddItemToCommandBarWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The display name dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayNameDependencyProperty = DependencyProperty.Register(
            nameof(DisplayName), typeof(string), typeof(AddItemToCommandBarWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The command bar name dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandBarNameDependencyProperty = DependencyProperty.Register(
            nameof(CommandBarName), typeof(string), typeof(AddItemToCommandBarWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The command bar type dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandBarTypeDependencyProperty = DependencyProperty.Register(
            nameof(CommandBarType), typeof(vsCommandBarType), typeof(AddItemToCommandBarWindow), new PropertyMetadata(vsCommandBarType.vsCommandBarTypeMenu));

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ProductivityShell.Commands.Shell.AddItemToCommandBarWindow" />
        ///     class.
        /// </summary>
        public AddItemToCommandBarWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the name of the command.
        /// </summary>
        /// <value>
        ///     The name of the command.
        /// </value>
        public string CommandName
        {
            get => (string) GetValue(CommandNameDependencyProperty);
            set => SetValue(CommandNameDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName
        {
            get => (string) GetValue(DisplayNameDependencyProperty);
            set => SetValue(DisplayNameDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the name of the command bar.
        /// </summary>
        /// <value>
        ///     The name of the command bar.
        /// </value>
        public string CommandBarName
        {
            get => (string) GetValue(CommandBarNameDependencyProperty);
            set => SetValue(CommandBarNameDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the type of the command bar.
        /// </summary>
        /// <value>
        ///     The type of the command bar.
        /// </value>
        public vsCommandBarType CommandBarType
        {
            get => (vsCommandBarType) GetValue(CommandBarTypeDependencyProperty);
            set => SetValue(CommandBarTypeDependencyProperty, value);
        }

        /// <summary>
        ///     Handles the Click event of the Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        ///     Handles the CheckedChanged event of the RadioButtonMenuBarCommandBarType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButtonMenuBarCommandBarType_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radioButton))
                return;

            if (radioButton.IsChecked != true)
                return;

            CommandBarType = vsCommandBarType.vsCommandBarTypeMenu;
        }

        /// <summary>
        ///     Handles the CheckedChanged event of the RadioButtonToolbarCommandBarType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButtonToolbarCommandBarType_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radioButton))
                return;

            if (radioButton.IsChecked != true)
                return;

            CommandBarType = vsCommandBarType.vsCommandBarTypeToolbar;
        }

        /// <summary>
        ///     Handles the CheckedChanged event of the RadioButtonContextMenuCommandBarType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RadioButtonContextMenuCommandBarType_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radioButton))
                return;

            if (radioButton.IsChecked != true)
                return;

            CommandBarType = vsCommandBarType.vsCommandBarTypePopup;
        }
    }
}