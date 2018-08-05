using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace ProductivityShell.Commands.TextEditor
{
    public partial class MoveToSettingsWindow : DialogWindow
    {
        /// <summary>
        ///     The settings name dependency property.
        /// </summary>
        public static readonly DependencyProperty SettingsNameDependencyProperty = DependencyProperty.Register(
            nameof(SettingsName), typeof(string), typeof(MoveToSettingsWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueDependencyProperty = DependencyProperty.Register(
            nameof(Value), typeof(string), typeof(MoveToSettingsWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The selected type dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTypeDependencyProperty = DependencyProperty.Register(
            nameof(SelectedType), typeof(Type), typeof(MoveToSettingsWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The selected string dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSettingsDependencyProperty = DependencyProperty.Register(
            nameof(SelectedSettings), typeof(SettingsFile), typeof(MoveToSettingsWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The selected scope dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedScopeDependencyProperty = DependencyProperty.Register(
            nameof(SelectedScope), typeof(string), typeof(MoveToSettingsWindow), new PropertyMetadata(null));

        /// <summary>
        ///     The types dependency property.
        /// </summary>
        public static readonly DependencyProperty TypesDependencyProperty = DependencyProperty.Register(
            nameof(Types), typeof(ObservableCollection<Type>), typeof(MoveToSettingsWindow),
            new PropertyMetadata(new ObservableCollection<Type>()));

        /// <summary>
        ///     The settings dependency property.
        /// </summary>
        public static readonly DependencyProperty SettingsDependencyProperty = DependencyProperty.Register(
            nameof(Settings), typeof(ObservableCollection<SettingsFile>), typeof(MoveToSettingsWindow),
            new PropertyMetadata(new ObservableCollection<SettingsFile>()));

        /// <summary>
        ///     The scopes dependency property.
        /// </summary>
        public static readonly DependencyProperty ScopesDependencyProperty = DependencyProperty.Register(
            nameof(Scopes), typeof(ObservableCollection<string>), typeof(MoveToSettingsWindow),
            new PropertyMetadata(new ObservableCollection<string>()));


        /// <summary>
        ///     Initializes a new instance of the <see cref="MoveToSettingsWindow" /> class.
        /// </summary>
        public MoveToSettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the name of the settings.
        /// </summary>
        /// <value>
        ///     The name of the settings.
        /// </value>
        public string SettingsName
        {
            get => (string) GetValue(SettingsNameDependencyProperty);
            set => SetValue(SettingsNameDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public string Value
        {
            get => (string) GetValue(ValueDependencyProperty);
            set => SetValue(ValueDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the types.
        /// </summary>
        /// <value>
        ///     The types.
        /// </value>
        public ObservableCollection<Type> Types
        {
            get => (ObservableCollection<Type>) GetValue(TypesDependencyProperty);
            set => SetValue(TypesDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        /// <value>
        ///     The settings.
        /// </value>
        public ObservableCollection<SettingsFile> Settings
        {
            get => (ObservableCollection<SettingsFile>) GetValue(SettingsDependencyProperty);
            set => SetValue(SettingsDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the scopes.
        /// </summary>
        /// <value>
        ///     The scopes.
        /// </value>
        public ObservableCollection<string> Scopes
        {
            get => (ObservableCollection<string>) GetValue(ScopesDependencyProperty);
            set => SetValue(ScopesDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected scope.
        /// </summary>
        /// <value>
        ///     The selected scope.
        /// </value>
        public string SelectedScope
        {
            get => (string) GetValue(SelectedScopeDependencyProperty);
            set => SetValue(SelectedScopeDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected type.
        /// </summary>
        /// <value>
        ///     The selected type.
        /// </value>
        public Type SelectedType
        {
            get => (Type) GetValue(SelectedTypeDependencyProperty);
            set => SetValue(SelectedTypeDependencyProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected settings.
        /// </summary>
        /// <value>
        ///     The selected settings.
        /// </value>
        public SettingsFile SelectedSettings
        {
            get => (SettingsFile) GetValue(SelectedSettingsDependencyProperty);
            set => SetValue(SelectedSettingsDependencyProperty, value);
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
    }
}