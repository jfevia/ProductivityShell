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

        private void Initialize()
        {
            var menuCommandId = new CommandID(Package.CommandSet, Id);
            var menuItem = new OleMenuCommand((s, e) => { ExecuteHandler(s); }, (s, e) => { ChangeHandler(s); }, menuCommandId);
            Package.CommandService.AddCommand(menuItem);
        }

        protected PackageBase Package { get; }

        protected int Id { get; }

        protected static T Instance { get; set; }

        protected virtual void OnExecute(OleMenuCommand command)
        {
        }

        protected virtual void OnChange(OleMenuCommand command)
        {
        }

        private void ExecuteHandler(object sender)
        {
            var command = sender as OleMenuCommand;
            if (command == null)
            {
                return;
            }

            OnExecute(command);
        }

        private void ChangeHandler(object sender)
        {
            var command = sender as OleMenuCommand;
            if (command == null)
            {
                return;
            }

            OnChange(command);
        }
    }
}