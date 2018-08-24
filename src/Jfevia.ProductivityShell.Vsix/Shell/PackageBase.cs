using System;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Shell
{
    public class PackageBase : Microsoft.VisualStudio.Shell.Package
    {
        private readonly IVsShell3 _shell3;
        private readonly IVsShell4 _shell4;
        private IMenuCommandService _commandService;
        private DTE2 _dte;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Jfevia.ProductivityShell.Vsix.Shell.PackageBase" /> class.
        /// </summary>
        /// <param name="commandSet">The command set.</param>
        public PackageBase(Guid commandSet)
        {
            CommandSet = commandSet;

            _shell3 = this.GetGlobalService<SVsShell, IVsShell3>();
            _shell4 = this.GetGlobalService<SVsShell, IVsShell4>();
        }

        /// <summary>
        ///     Gets the command service.
        /// </summary>
        /// <value>
        ///     The command service.
        /// </value>
        internal IMenuCommandService CommandService => _commandService ?? (_commandService = this.GetService<IMenuCommandService>());

        /// <summary>
        ///     Gets the command set.
        /// </summary>
        /// <value>
        ///     The command set.
        /// </value>
        internal Guid CommandSet { get; }

        /// <summary>
        ///     Gets the top-level object in the Visual Studio object model.
        /// </summary>
        public DTE2 Dte => _dte ?? (_dte = this.GetGlobalService<_DTE, DTE2>());

        public OutputWindowPane PackageOutputPane
        {
            get
            {
                var outputPanes = Dte?.ToolWindows.OutputWindow.OutputWindowPanes;

                try
                {
                    return outputPanes.Item(PackageConstants.ProductName);
                }
                catch (Exception)
                {
                    return outputPanes.Add(PackageConstants.ProductName);
                }
            }
        }

        public bool IsRunningElevated
        {
            get
            {
                _shell3.IsRunningElevated(out var isElevated);
                return isElevated;
            }
        }

        public bool ActivateOutputWindow()
        {
            Dte?.ToolWindows.OutputWindow.Parent.Activate();
            return true;
        }

        public void ShowOptionPage<T>()
            where T : DialogPage
        {
            ShowOptionPage(typeof(T));
        }

        public T GetDialogPage<T>()
            where T : DialogPage
        {
            return (T) GetDialogPage(typeof(T));
        }

        /// <summary>
        ///     Adds the service.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serviceInstance">The service instance.</param>
        public void AddService(Type type, object serviceInstance)
        {
            ((IServiceContainer) this).AddService(type, serviceInstance);
        }

        /// <summary>
        ///     Restarts the shell with a specified mode.
        /// </summary>
        /// <param name="mode">The restart mode.</param>
        /// <returns>Whether the restart is successful.</returns>
        public bool Restart(RestartMode mode)
        {
            var vsRestartMode = __VSRESTARTTYPE.RESTART_Normal;
            if (mode == RestartMode.Elevated)
                vsRestartMode = __VSRESTARTTYPE.RESTART_Elevated;

            return !ErrorHandler.Failed(Restart(vsRestartMode));
        }

        private int Restart(__VSRESTARTTYPE vsRestartMode)
        {
            return _shell4.Restart((uint) vsRestartMode);
        }
    }
}