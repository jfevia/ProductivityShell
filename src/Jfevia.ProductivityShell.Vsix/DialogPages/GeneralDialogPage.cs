using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Jfevia.ProductivityShell.Vsix.DialogPages
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Guid(PackageGuids.GeneralDialogPageString)]
    public class GeneralDialogPage : DialogPage
    {
        [Category(PackageConstants.GeneralCategory)]
        [DisplayName("Diagnostics Mode")]
        [Description("Enables or disables the diagnostics mode.")]
        public bool DiagnosticsMode { get; set; } = false;
    }
}