using System;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Jfevia.ProductivityShell.Vsix.Commands.Project;
using Jfevia.ProductivityShell.Vsix.DialogPages;
using Jfevia.ProductivityShell.Vsix.Extensions;
using Jfevia.ProductivityShell.Vsix.Helpers;
using Jfevia.ProductivityShell.Vsix.Shell;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Jfevia.ProductivityShell.Vsix.Commands.Tools
{
    internal sealed class ReplaceGuidPlaceholdersCommand : CommandBase<ReplaceGuidPlaceholdersCommand>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="T:Jfevia.ProductivityShell.Vsix.Commands.Tools.ReplaceGuidPlaceholdersCommand" />
        ///     class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ReplaceGuidPlaceholdersCommand(PackageBase package)
            : base(package, PackageCommands.ToolsReplaceGuidPlaceholdersCommand)
        {
        }

        /// <summary>
        ///     Initializes the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        public static async Task InitializeAsync(PackageBase package)
        {
            Instance = new ReplaceGuidPlaceholdersCommand(package);
            await Instance.InitializeAsync();
        }

        /// <summary>
        ///     Called when [before query status].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///     The task.
        /// </returns>
        protected override async Task OnBeforeQueryStatusAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Called when [execute].
        /// </summary>
        /// <param name="command">The command.</param>
        protected override async Task OnExecuteAsync(OleMenuCommand command)
        {
            await ReplacePlaceholdersInSelectedItemsAsync();
        }

        /// <summary>
        ///     Called when [change].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///     The task.
        /// </returns>
        protected override async Task OnChangeAsync(OleMenuCommand command)
        {
            await Task.Yield();
        }

        /// <summary>
        ///     Replaces the placeholders in selected items.
        /// </summary>
        private async Task ReplacePlaceholdersInSelectedItemsAsync()
        {
            var projectItems = (await SolutionHelper.GetSelectedProjectItemsRecursivelyAsync(Package)).ToList();
            projectItems = projectItems.Distinct(new ProjectItemEqualityComparer()).ToList();
            if (projectItems.Count <= 0)
            {
                MessageBox.Show("No items were found", "GUID Placeholders");
                return;
            }

            if (MessageBox.Show($"Do you wish to search and replace GUID placeholders in {projectItems.Count} items?",
                    "GUID Placeholders", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            foreach (var projectItem in projectItems)
                await ReplaceGuidPlaceholdersAsync(projectItem);
        }

        /// <summary>
        ///     Replaces GUID placeholders in a project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        private async Task ReplaceGuidPlaceholdersAsync(EnvDTE.ProjectItem projectItem)
        {
            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Attempt to open the document if not already opened.
            var wasOpen = projectItem.IsOpen[Constants.vsViewKindTextView] ||
                          projectItem.IsOpen[Constants.vsViewKindCode];
            if (!wasOpen)
                try
                {
                    projectItem.Open(Constants.vsViewKindTextView);
                }
                catch (Exception)
                {
                    // File cannot be opened (e.g.: deleted from disk, non-text based type)
                }

            try
            {
                if (projectItem.Document != null)
                {
                    await ReplaceGuidPlaceholdersAsync(projectItem.Document);

                    // Close the document if it was opened for cleanup.
                    if (!wasOpen)
                        projectItem.Document.Close(vsSaveChanges.vsSaveChangesYes);
                }
            }
            catch (Exception)
            {
                // File cannot be opened (e.g.: deleted from disk, non-text based type)
            }
        }

        /// <summary>
        ///     Replaces GUID placeholders in a document.
        /// </summary>
        /// <param name="document">The document.</param>
        private async Task ReplaceGuidPlaceholdersAsync(Document document)
        {
            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Make sure the document to be cleaned up is active, required for some commands like format document.
            document.Activate();

            // Check for designer windows being active, which should not proceed with cleanup as the code isn't truly active.
            if (document.ActiveWindow.Caption.EndsWith(" [Design]"))
                return;

            if (Package.Dte.ActiveDocument != document)
                await OutputWindowHelper.WarningWriteLineAsync(
                    $"Activation was not completed before replacing began for '{document.Name}'");

            await OutputWindowHelper.DiagnosticWriteLineAsync($"ReplaceGuidPlaceholdersAsync started for '{document.FullName}'");
            Package.Dte.StatusBar.Text = $"Replacing GUID placeholders in '{document.Name}'...";

            var textDocument = document.GetTextDocument();
            await ReplaceGuidPlaceholdersAsync(textDocument);

            Package.Dte.StatusBar.Text = $"Replaced GUID placeholders in '{document.Name}'.";
            await OutputWindowHelper.DiagnosticWriteLineAsync($"ReplaceGuidPlaceholdersAsync completed for '{document.FullName}'");
        }

        /// <summary>
        ///     Replaces GUID placeholders in a text document.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        private async Task ReplaceGuidPlaceholdersAsync(TextDocument textDocument)
        {
            var toolsDialogPage = Package.GetDialogPage<ToolsDialogPage>();
            if (string.IsNullOrWhiteSpace(toolsDialogPage.GuidPlaceholders))
                return;

            await ProductivityShell.Vsix.Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();

            foreach (var placeholder in toolsDialogPage.GuidPlaceholders.Split(
                Properties.Settings.Default.GuidPlaceholderSplitChar))
            {
                if (string.IsNullOrWhiteSpace(placeholder))
                    continue;

                var startEditPoint = textDocument.StartPoint.CreateEditPoint();
                var endEditPoint = textDocument.EndPoint.CreateEditPoint();
                var content = startEditPoint.GetText(endEditPoint);
                var isMatch = IsMatch(content, placeholder);

                if (!isMatch)
                    continue;

                while (isMatch)
                {
                    var comparisonType = GetComparisonType(toolsDialogPage.GuidMatchCase);
                    var guidStringBuilder = GetGuidStringBuilder(toolsDialogPage);

                    content = content.Replace(placeholder, guidStringBuilder, comparisonType, 1);
                    isMatch = IsMatch(content, placeholder);
                }

                startEditPoint.Delete(endEditPoint);
                startEditPoint.Insert(content);
            }
        }

        /// <summary>
        ///     Gets the unique identifier.
        /// </summary>
        /// <param name="toolsDialogPage">The tools dialog page.</param>
        /// <returns>The unique identifier.</returns>
        private static string GetGuidStringBuilder(ToolsDialogPage toolsDialogPage)
        {
            var guidStringBuilder = new GuidStringBuilder();
            guidStringBuilder.Guid = Guid.NewGuid();
            guidStringBuilder.Format = toolsDialogPage.GuidFormattingOption;
            guidStringBuilder.Case = toolsDialogPage.GuidLetterCase;

            return guidStringBuilder.ToString();
        }

        /// <summary>
        ///     Gets a comparison type based on case matching.
        /// </summary>
        /// <param name="matchCase">Whether case matching is used.</param>
        /// <returns>The comparison type.</returns>
        private static StringComparison GetComparisonType(bool matchCase)
        {
            return matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        /// <summary>
        ///     Returns whether a content contains a specific placeholder.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="placeholder">The placeholder.</param>
        /// <returns>Whether the content contains a placeholder.</returns>
        private static bool IsMatch(string content, string placeholder)
        {
            return !string.IsNullOrWhiteSpace(content) && content.Contains(placeholder);
        }
    }
}