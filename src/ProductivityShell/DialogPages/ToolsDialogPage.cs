using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Shell;

namespace ProductivityShell.DialogPages
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Guid(PackageGuids.ToolsDialogPageString)]
    public class ToolsDialogPage : DialogPage
    {
        [Category(PackageConstants.GuidCategory)]
        [DisplayName("Placeholders")]
        [Description("Defines placeholders to be replaced with GUID values. Multiple values are supported by separating them with a semicolon.")]
        public string GuidPlaceholders { get; set; } = "INSERT-GUID-HERE;PUT-GUID-HERE;";

        [Category(PackageConstants.GuidCategory)]
        [DisplayName("Match Case")]
        [Description("Defines whether placeholders should only match case-sensitive ocurrences.")]
        public bool GuidMatchCase { get; set; } = false;

        [Category(PackageConstants.GuidCategory)]
        [DisplayName("Format")]
        [Description("Defines the format used to generate GUID values.")]
        public GuidFormattingOption GuidFormattingOption { get; set; } = GuidFormattingOption.HyphenSeparation;

        [Category(PackageConstants.GuidCategory)]
        [DisplayName("Letter Case")]
        [Description("Defines the letter case used to generated GUID values.")]
        public LetterCase GuidLetterCase { get; set; } = LetterCase.UpperCase;

        [Category(PackageConstants.TextEditorCategory)]
        [DisplayName("Show Margin")]
        [Description("Defines whether the margin in the text editor should be displayed.")]
        public bool ShowTextEditorMargin { get; set; } = true;
    }
}