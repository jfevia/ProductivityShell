using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix.Solutions
{
    internal class ConfigurationFileTracker : IVsFileChangeEvents
    {
        private readonly string _configFilePath;
        private readonly IVsFileChangeEx _fileChangeService;
        private uint _fileChangeCookie;
        private bool _isRunning;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationFileTracker" /> class.
        /// </summary>
        /// <param name="configFilePath">The configuration file path.</param>
        /// <param name="fileChangeService">The file change service.</param>
        public ConfigurationFileTracker(string configFilePath, IVsFileChangeEx fileChangeService)
        {
            _configFilePath = configFilePath;
            _fileChangeService = fileChangeService;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies clients of changes made to a directory.
        /// </summary>
        /// <param name="pszDirectory">[in] Name of the directory that had a change.</param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int DirectoryChanged(string pszDirectory)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies clients of changes made to one or more files.
        /// </summary>
        /// <param name="cChanges">[in] Number of files changed.</param>
        /// <param name="rgpszFile">[in, size_is(cChanges)] Array of file names.</param>
        /// <param name="rggrfChange">
        ///     [in, size_is(cChanges)] Array of flags indicating the type of changes. See
        ///     <see cref="T:Microsoft.VisualStudio.Shell.Interop._VSFILECHANGEFLAGS" />.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
        {
            FileChanged?.Invoke(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Occurs when [file changed].
        /// </summary>
        public event EventHandler FileChanged;

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning)
                throw new InvalidOperationException("The file tracker is not running");

            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            _fileChangeService.UnadviseFileChange(_fileChangeCookie);
            _isRunning = false;
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public async Task StartAsync()
        {
            await Package.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            _fileChangeService.AdviseFileChange(_configFilePath, (uint) (_VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out _fileChangeCookie);
            _isRunning = true;
        }
    }
}