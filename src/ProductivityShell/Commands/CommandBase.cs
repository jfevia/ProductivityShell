using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands
{
    public class CommandBase<T>
    {
        protected CommandBase(PackageBase package, int id)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            Id = id;

            Initialize();
        }

        protected PackageBase Package { get; }

        protected int Id { get; }

        protected static T Instance { get; set; }

        private void Initialize()
        {
            var menuCommandId = new CommandID(Package.CommandSet, Id);
            var menuItem = new OleMenuCommand((s, e) => { ExecuteHandler(s); }, (s, e) => { ChangeHandler(s); }, menuCommandId);
            menuItem.BeforeQueryStatus += (s, e) => { BeforeQueryStatusHandler(s); };

            Package.CommandService.AddCommand(menuItem);
        }

        private void BeforeQueryStatusHandler(object sender)
        {
            if (!(sender is OleMenuCommand command))
                return;

            OnBeforeQueryStatus(command);
        }

        protected virtual void OnBeforeQueryStatus(OleMenuCommand command)
        {
        }

        protected virtual void OnExecute(OleMenuCommand command)
        {
        }

        protected virtual void OnChange(OleMenuCommand command)
        {
        }

        private void ExecuteHandler(object sender)
        {
            if (!(sender is OleMenuCommand command))
                return;

            OnExecute(command);
        }

        private void ChangeHandler(object sender)
        {
            if (!(sender is OleMenuCommand command))
                return;

            OnChange(command);
        }
    }
}