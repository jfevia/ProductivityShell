using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProductivityShell.Shell
{
    public class PackageBase : Microsoft.VisualStudio.Shell.Package
    {
        private readonly IVsShell3 _shell3 = GetGlobalService<SVsShell, IVsShell3>();
        private readonly IVsShell4 _shell4 = GetGlobalService<SVsShell, IVsShell4>();
        private IMenuCommandService _commandService;
        private DTE2 _dte;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PackageBase" /> class.
        /// </summary>
        /// <param name="commandSet">The command set.</param>
        public PackageBase(Guid commandSet)
        {
            CommandSet = commandSet;
        }

        /// <summary>
        ///     Gets the command service.
        /// </summary>
        /// <value>
        ///     The command service.
        /// </value>
        internal IMenuCommandService CommandService =>
            _commandService ?? (_commandService = GetService<IMenuCommandService>());

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
        public DTE2 Dte => _dte ?? (_dte = GetGlobalService<_DTE, DTE2>());

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

        /// <summary>
        ///     Adds the or update command bar.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="barName">Name of the bar.</param>
        /// <param name="type">The type.</param>
        public void AddOrUpdateCommandBar(string name, string displayName, string barName, vsCommandBarType type)
        {
            if (!(Dte.Application.CommandBars is CommandBars commandBars))
                return;

            CommandBar commandBar = null;
            for (var commandBarIndex = 0; commandBarIndex < commandBars.Count; commandBarIndex++)
            {
                var tempCommandBar = commandBars[commandBarIndex + 1];
                Debug.WriteLine($"Name (Local): {tempCommandBar.NameLocal}, Name: {tempCommandBar.Name}");
                Debug.WriteLine("\tBegin Controls");
                var controls = tempCommandBar.Controls;

                foreach (CommandBarControl item in controls)
                    Debug.WriteLine($"\t\tDesc: {item.DescriptionText}, {item.Type}");

                Debug.WriteLine("\tEnd Controls");

                if (!string.Equals(barName, tempCommandBar.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                commandBar = tempCommandBar;
                break;
            }

            if (commandBar == null)
                commandBar = (CommandBar)Dte.Commands.AddCommandBar(barName, type);

            Debug.WriteLine(commandBar);

            //var commandItem = commandBar.AddControl(barName);
            //commandItem.Caption = displayName;
            //Debug.WriteLine(commandItem);
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
        ///     Gets type-based services from the VSPackage service container
        /// </summary>
        /// <typeparam name="TSource">The type of the source service.</typeparam>
        /// <typeparam name="TTarget">The type of the target service.</typeparam>
        /// <returns>An instance of the requested service, or null if the service could not be found.</returns>
        public static TTarget GetGlobalService<TSource, TTarget>()
            where TTarget : class
        {
            return GetGlobalService(typeof(TSource)) as TTarget;
        }

        /// <summary>
        ///     Gets the service.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>The service.</returns>
        public T GetService<T>()
        {
            return (T) GetService(typeof(T));
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