using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly ITextDocument _doc;
        private readonly IClassifier _classifier;
        private bool _isDisposed;
        private readonly InteractiveTextControl _labelFilePath;

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
                _doc.FileActionOccurred += FileChangedOnDisk;
                UpdateFilePath(_doc);
            }
        }

        private void OnFilePathValueMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var groupedPaths = new[] {_labelFilePath.Value}.GroupBy(Path.GetDirectoryName);
                foreach (var path in groupedPaths)
                    WindowsExplorerHelper.FilesOrFolders(path);
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                Clipboard.SetData(DataFormats.Text, _labelFilePath.Value);
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ActualHeight;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return marginName == MarginName ? this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;

                _doc.FileActionOccurred -= FileChangedOnDisk;

                (_classifier as IDisposable)?.Dispose();
            }
        }

        private void FileChangedOnDisk(object sender, TextDocumentFileActionEventArgs e)
        {
            UpdateFilePath(_doc);
        }

        private void UpdateFilePath(ITextDocument doc)
        {
            Dispatcher.BeginInvoke(new Action(() =>
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

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }
    }
}