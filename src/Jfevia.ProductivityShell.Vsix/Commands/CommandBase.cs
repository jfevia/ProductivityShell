using System;
using System.ComponentModel.Design;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands
{
    public abstract class CommandBase<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandBase{T}" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">package</exception>
        protected CommandBase(PackageBase package, int id)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            Id = id;
        }

        /// <summary>
        ///     Gets the package.
        /// </summary>
        /// <value>
        ///     The package.
        /// </value>
        protected PackageBase Package { get; }

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        protected int Id { get; }

        /// <summary>
        ///     Gets or sets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        protected static T Instance { get; set; }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public async Task InitializeAsync()
        {
            var menuCommandId = new CommandID(Package.CommandSet, Id);
            var menuItem = new OleMenuCommand((s, e) => ExecuteHandler(s), (s, e) => ChangeHandler(s), menuCommandId);
            menuItem.BeforeQueryStatus += (s, e) => BeforeQueryStatusHandler(s);

            var commandService = await Package.GetCommandServiceAsync();
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        ///     Before the query status handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private async void BeforeQueryStatusHandler(object sender)
        {
            if (!(sender is OleMenuCommand command))
                return;

            await OnBeforeQueryStatusAsync(command);
        }

        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The task.</returns>
        protected abstract Task OnBeforeQueryStatusAsync(OleMenuCommand command);

        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The task.</returns>
        protected abstract Task OnExecuteAsync(OleMenuCommand command);

        /// <summary>
        ///     Called when [change].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The task.</returns>
        protected abstract Task OnChangeAsync(OleMenuCommand command);

        /// <summary>
        ///     Executes the handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private async void ExecuteHandler(object sender)
        {
            if (!(sender is OleMenuCommand command))
                return;

            await OnExecuteAsync(command);
        }

        /// <summary>
        ///     Changes the handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private async void ChangeHandler(object sender)
        {
            if (!(sender is OleMenuCommand command))
                return;

            await OnChangeAsync(command);
        }
    }
}