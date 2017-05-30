using System;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using ProductivityShell.Commands.Project;
using ProductivityShell.DialogPages;
using ProductivityShell.Extensions;
using ProductivityShell.Helpers;
using ProductivityShell.Properties;
using ProductivityShell.Shell;

namespace ProductivityShell.Commands.Tools
{
    internal sealed class ReplaceGuidPlaceholdersCommand : CommandBase<ReplaceGuidPlaceholdersCommand>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReplaceGuidPlaceholdersCommand" /> class.
        /// </summary>
        /// <param name="package">The package.</param>
        private ReplaceGuidPlaceholdersCommand(PackageBase package) : base(package, PackageIds.ToolsReplaceGuidReplaceholdersCommand)
        {
        }

        public static void Initialize(PackageBase package)
        {
            Instance = new ReplaceGuidPlaceholdersCommand(package);
        }

        protected override void OnExecute(OleMenuCommand command)
        {
            ReplacePlaceholdersInSelectedItems();
        }

        private void ReplacePlaceholdersInSelectedItems()
        {
            var projectItems = SolutionHelper.GetSelectedProjectItemsRecursively(Package).Distinct(new ProjecItemEqualityComparer()).ToList();
            if (projectItems.Count <= 0)
            {
                MessageBox.Show("No items were found", "GUID Placeholders");
                return;
            }

            if (MessageBox.Show($"Do you wish to search and replace GUID placeholders in {projectItems.Count} items?", "GUID Placeholders", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            foreach (var projectItem in projectItems)
            {
                ReplaceGuidPlaceholders(projectItem);
            }
        }

        /// <summary>
        ///     Replaces GUID placeholders in a project item.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        private void ReplaceGuidPlaceholders(EnvDTE.ProjectItem projectItem)
        {
            // Attempt to open the document if not already opened.
            var wasOpen = projectItem.IsOpen[Constants.vsViewKindTextView] || projectItem.IsOpen[Constants.vsViewKindCode];
            if (!wasOpen)
            {
                try
                {
                    projectItem.Open(Constants.vsViewKindTextView);
                }
                catch (Exception)
                {
                    // OK if file cannot be opened (ex: deleted from disk, non-text based type.)
                }
            }

            try
            {
                if (projectItem.Document != null)
                {
                    ReplaceGuidPlaceholders(projectItem.Document);

                    // Close the document if it was opened for cleanup.
                    if (!wasOpen)
                    {
                        projectItem.Document.Close(vsSaveChanges.vsSaveChangesYes);
                    }
                }
            }
            catch (Exception)
            {
                // OK if file cannot be opened (ex: deleted from disk, non-text based type.)
            }
        }

        /// <summary>
        ///     Replaces GUID placeholders in a document.
        /// </summary>
        /// <param name="document">The document.</param>
        private void ReplaceGuidPlaceholders(Document document)
        {
            // Make sure the document to be cleaned up is active, required for some commands like format document.
            document.Activate();

            // Check for designer windows being active, which should not proceed with cleanup as the code isn't truly active.
            if (document.ActiveWindow.Caption.EndsWith(" [Design]"))
            {
                return;
            }

            if (Package.Dte.ActiveDocument != document)
            {
                OutputWindowHelper.WarningWriteLine($"Activation was not completed before replacing began for '{document.Name}'");
            }

            OutputWindowHelper.DiagnosticWriteLine($"ReplaceGuidPlaceholders started for '{document.FullName}'");
            Package.Dte.StatusBar.Text = $"Replacing GUID placeholders in '{document.Name}'...";

            var textDocument = document.GetTextDocument();
            ReplaceGuidPlaceholders(textDocument);

            Package.Dte.StatusBar.Text = $"Replaced GUID placeholders in '{document.Name}'.";
            OutputWindowHelper.DiagnosticWriteLine($"ReplaceGuidPlaceholders completed for '{document.FullName}'");
        }

        /// <summary>
        ///     Replaces GUID placeholders in a text document.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        private void ReplaceGuidPlaceholders(TextDocument textDocument)
        {
            var toolsDialogPage = Package.GetDialogPage<ToolsDialogPage>();
            if (string.IsNullOrWhiteSpace(toolsDialogPage.GuidPlaceholders))
            {
                return;
            }

            foreach (var placeholder in toolsDialogPage.GuidPlaceholders.Split(Settings.Default.GuidPlaceholderSplitChar))
            {
                if (string.IsNullOrWhiteSpace(placeholder))
                {
                    continue;
                }

                var startEditPoint = textDocument.StartPoint.CreateEditPoint();
                var endEditPoint = textDocument.EndPoint.CreateEditPoint();
                var content = startEditPoint.GetText(endEditPoint);
                var isMatch = IsMatch(content, placeholder);

                if (!isMatch)
                {
                    continue;
                }

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