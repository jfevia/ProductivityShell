using System;

namespace Jfevia.ProductivityShell.Vsix
{
    internal static class PackageGuids
    {
        public const string PackageString = "F80CCF45-FA6A-4636-9361-DFCA27E2313E";
        public const string OutputPaneString = "CD930D2C-758A-4520-AC29-9B395EBA9054";
        public const string PackageCommandSetString = "A2D0A697-8809-450C-9F66-88A95C9B09F4";
        public const string GeneralDialogPageString = "4C7CFC9D-AD90-4703-814A-2899528D7D65";
        public const string ToolsDialogPageString = "843FB0B7-39DA-4524-9836-4C5A6CDAE0B5";
        public static Guid Package = new Guid(PackageString);
        public static Guid OutputPane = new Guid(OutputPaneString);
        public static Guid PackageCommandSet = new Guid(PackageCommandSetString);
        public static Guid GeneralDialogPage = new Guid(GeneralDialogPageString);
        public static Guid ToolsDialogPage = new Guid(ToolsDialogPageString);
    }
}