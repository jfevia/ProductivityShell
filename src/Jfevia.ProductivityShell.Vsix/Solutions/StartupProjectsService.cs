using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Jfevia.ProductivityShell.Configuration;
using Microsoft.VisualStudio.Shell;

namespace Jfevia.ProductivityShell.Vsix.Solutions
{
    internal class StartupProjectsService
    {
        private readonly IMenuCommandService _menuCommandService;
        private Profile _editConfigurationItem;
        private IList<Profile> _items;
        private CommandID _itemsSourceCommand;
        private OleMenuCommand _queryItemsSourceCommand;
        private OleMenuCommand _querySelectedItemCommand;
        private CommandID _selectedItemCommand;
        private Profile _selectedItem;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartupProjectsService" /> class.
        /// </summary>
        /// <param name="menuCommandService">The menu command service.</param>
        public StartupProjectsService(IMenuCommandService menuCommandService)
        {
            _menuCommandService = menuCommandService;
            Initialize();
        }

        /// <summary>
        ///     Occurs when [requested show configuration].
        /// </summary>
        public event EventHandler<EventArgs> RequestedShowConfiguration;

        /// <summary>
        ///     Occurs when [selected startup project changed].
        /// </summary>
        public event EventHandler<StartupProjectsChangedEventArgs> SelectedStartupProjectsChanged;

        /// <summary>
        ///     Gets or sets the selected item.
        /// </summary>
        /// <value>
        ///     The selected item.
        /// </value>
        public Profile SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (ProfileEqualityComparer.Instance.Equals(_selectedItem, value))
                    return;

                _selectedItem = value;
            }
        }

        /// <summary>
        ///     Gets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public IEnumerable<Profile> Items
        {
            get => _items ?? (_items = new List<Profile>());
            set
            {
                _items = new List<Profile>(value);
                _items.Add(_editConfigurationItem);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get => _querySelectedItemCommand.Enabled;
            set => _querySelectedItemCommand.Enabled = value;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _editConfigurationItem = new Profile
            {
                DisplayName = "Configure..."
            };

            RegisterCommands();
        }

        /// <summary>
        ///     Registers the commands.
        /// </summary>
        private void RegisterCommands()
        {
            // Commands for the selected item
            _selectedItemCommand = new CommandID(PackageGuids.PackageCommandSet, PackageCommands.StartupProjectsComboBoxCommand);
            _querySelectedItemCommand = new OleMenuCommand(OnQuerySelectedItemCommand, _selectedItemCommand);

            // Accept any argument string
            _querySelectedItemCommand.ParametersDescription = "$";
            _querySelectedItemCommand.Enabled = false;

            // Commands for the items source
            _itemsSourceCommand = new CommandID(PackageGuids.PackageCommandSet, PackageCommands.StartupProjectsComboBoxItemsCommand);
            _queryItemsSourceCommand = new OleMenuCommand(OnQueryItemsSourceCommand, _itemsSourceCommand);

            _menuCommandService.AddCommand(_querySelectedItemCommand);
            _menuCommandService.AddCommand(_queryItemsSourceCommand);

            _querySelectedItemCommand.Enabled = true;
        }

        /// <summary>
        ///     Called when [query selected item command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnQuerySelectedItemCommand(object sender, EventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e == EventArgs.Empty)
                throw new ArgumentException("EventArgs cannot be empty");

            if (!(e is OleMenuCmdEventArgs eventArgs))
                throw new InvalidCastException("Could not cast EventArgs to OleMenuCmdEventArgs type");

            var oldSelectedIndex = eventArgs.OutValue;
            var newSelectedIndex = eventArgs.InValue as int?;

            if (oldSelectedIndex == IntPtr.Zero && newSelectedIndex == null)
                throw new ArgumentException("Neither in nor out parameters can be null");

            if (oldSelectedIndex != IntPtr.Zero && newSelectedIndex != null)
                throw new ArgumentException("Both in and out parameters should be specified");

            if (oldSelectedIndex != IntPtr.Zero && SelectedItem != null)
            {
                // If this value is not null, the IDE is requesting the current value for the combo
                Marshal.GetNativeVariantForObject(SelectedItem.DisplayName, oldSelectedIndex);
            }
            else if (newSelectedIndex != null)
            {
                // If this value is not null, the IDE is sending the new value selected in the combo
                var selectedItem = _items[newSelectedIndex.Value];
                if (selectedItem == _editConfigurationItem)
                {
                    RequestedShowConfiguration?.Invoke(this, EventArgs.Empty);
                    return;
                }

                SelectedItem = selectedItem;
                SelectedStartupProjectsChanged?.Invoke(this, new StartupProjectsChangedEventArgs(SelectedItem));
            }
        }

        /// <summary>
        ///     Called when [query items source command].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnQueryItemsSourceCommand(object sender, EventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e == EventArgs.Empty)
                throw new ArgumentException("EventArgs cannot be empty");

            if (!(e is OleMenuCmdEventArgs eventArgs))
                throw new InvalidCastException("Could not cast EventArgs to OleMenuCmdEventArgs type");

            var comboBox = eventArgs.OutValue;
            if (comboBox == IntPtr.Zero)
                throw new ArgumentException("Combo box can not be null");

            Marshal.GetNativeVariantForObject(Items.Select(s => s.DisplayName).ToArray(), comboBox);
        }

        /// <summary>
        ///     Called when [debugging state changed].
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void OnDebuggingStateChanged(bool isEnabled)
        {
            IsEnabled = !isEnabled;
        }

        /// <summary>
        ///     Called when [closed solution].
        /// </summary>
        public void OnClosedSolution()
        {
            SelectedItem = null;
            _items = null;
        }
    }
}