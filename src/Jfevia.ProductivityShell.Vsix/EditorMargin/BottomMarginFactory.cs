using System.ComponentModel.Composition;
using Jfevia.ProductivityShell.Vsix.DialogPages;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Jfevia.ProductivityShell.Vsix.EditorMargin
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(BottomMargin.MarginName)]
    [Order(After = PredefinedMarginNames.BottomControl)]
    [MarginContainer(PredefinedMarginNames.Top)]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class BottomMarginFactory : IWpfTextViewMarginProvider
    {
        [Import]
        private IClassifierAggregatorService _classifierService;

        [Import]
        private ITextDocumentFactoryService _documentService;

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            if (wpfTextViewHost == null || _classifierService == null || _documentService == null)
                return null;

            var generalOptionsPage = Package.Instance.GetDialogPage<ToolsDialogPage>();
            if (!generalOptionsPage.ShowTextEditorMargin)
                return null;

            return new BottomMargin(wpfTextViewHost.TextView, _classifierService, _documentService);
        }
    }
}