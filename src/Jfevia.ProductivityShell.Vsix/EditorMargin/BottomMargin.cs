using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Jfevia.ProductivityShell.Vsix.Helpers;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

namespace Jfevia.ProductivityShell.Vsix.EditorMargin
{
    internal class BottomMargin : DockPanel, IWpfTextViewMargin
    {
        public const string MarginName = "Productivity Shell Margin";
        private readonly IClassifier _classifier;
        private readonly ITextDocument _doc;
        private readonly InteractiveTextControl _labelFilePath;
        private bool _isDisposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BottomMargin" /> class.
        /// </summary>
        /// <param name="textView">The text view.</param>
        /// <param name="classifier">The classifier.</param>
        /// <param name="documentService">The document service.</param>
        public BottomMargin(IWpfTextView textView, IClassifierAggregatorService classifier, ITextDocumentFactoryService documentService)
        {
            _classifier = classifier.GetClassifier(textView.TextBuffer);

            SetResourceReference(BackgroundProperty, EnvironmentColors.ScrollBarBackgroundBrushKey);

            ClipToBounds = true;

            _labelFilePath = new InteractiveTextControl("Path");
            _labelFilePath.ValueMouseDown += OnFilePathValueMouseDown;
            _labelFilePath.SetTooltip($"Left Click to copy file path{Environment.NewLine}Right Click to show file in File Explorer");

            Children.Add(_labelFilePath);

            if (documentService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _doc))
            {
                _doc.FileActionOccurred += OnFileChangedOnDiskAsync;
                Package.Instance.JoinableTaskFactory.Run(() => UpdateFilePathAsync(_doc));
            }
        }

        /// <summary>
        ///     Gets the <see cref="T:System.Windows.FrameworkElement" /> that renders the margin.
        /// </summary>
        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        /// <summary>
        ///     Gets the size of the margin.
        /// </summary>
        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ActualHeight;
            }
        }

        /// <summary>
        ///     Determines whether the margin is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        ///     Gets the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> with the specified margin name.
        /// </summary>
        /// <param name="marginName">The name of the <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" />.</param>
        /// <returns>
        ///     The <see cref="T:Microsoft.VisualStudio.Text.Editor.ITextViewMargin" /> named <paramref name="marginName" />, or
        ///     null if no match is found.
        /// </returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return marginName == MarginName ? this : null;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;

                _doc.FileActionOccurred -= OnFileChangedOnDiskAsync;

                (_classifier as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        ///     Called when [file path value mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void OnFilePathValueMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var groupedPaths = new[] {_labelFilePath.Value}.GroupBy(Path.GetDirectoryName);
                foreach (var path in groupedPaths)
                    WindowsExplorerHelper.FilesOrFolders(path);
                return;
            }

            if (e.ChangedButton == MouseButton.Left) Clipboard.SetData(DataFormats.Text, _labelFilePath.Value);
        }

        /// <summary>
        ///     Called when [file changed on disk asynchronous].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextDocumentFileActionEventArgs" /> instance containing the event data.</param>
        private async void OnFileChangedOnDiskAsync(object sender, TextDocumentFileActionEventArgs e)
        {
            await UpdateFilePathAsync(_doc);
        }

        /// <summary>
        ///     Updates the file path.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <returns>The task.</returns>
        private async Task UpdateFilePathAsync(ITextDocument doc)
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            await Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    _labelFilePath.Value = doc.FilePath;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }
            }), DispatcherPriority.ApplicationIdle, null);
        }

        /// <summary>
        ///     Throws if disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }
    }
}