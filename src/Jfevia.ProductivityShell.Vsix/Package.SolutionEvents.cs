using Jfevia.ProductivityShell.Vsix.VisualStudio;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jfevia.ProductivityShell.Vsix
{
    public partial class Package : IVsSolutionEvents, IVsSolutionEvents4, IVsSolutionLoadEvents, IVsSelectionEvents
    {
        private VisualStudioProxy _visualStudioProxy;
        private bool _projectsLoadInBatches;

        /// <inheritdoc />
        /// <summary>
        ///     Reports that the command UI context has changed.
        /// </summary>
        /// <param name="dwCmdUICookie">
        ///     [in] DWORD representation of the GUID identifying the command UI context.
        /// </param>
        /// <param name="fActive">
        ///     [in] Flag that is set to <see langword="true" /> if the command UI context identified by
        ///     <paramref name="dwCmdUICookie" /> has become active and <see langword="false" /> if it has become inactive.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            if (dwCmdUICookie == _visualStudioProxy.DebuggingCookie)
                _visualStudioProxy.OnDebuggingStateChanged(fActive != 0);

            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Reports that an element value has changed.
        /// </summary>
        /// <param name="elementid">
        ///     [in] DWORD value representing a particular entry in the array of element values associated with
        ///     the selection context. For valid <paramref name="elementid" /> values, see
        ///     <see cref="T:Microsoft.VisualStudio.VSConstants.VSSELELEMID" />.
        /// </param>
        /// <param name="varValueOld">
        ///     [in] VARIANT that contains the previous element value. This parameter contains
        ///     element-specific data, such as a pointer to the
        ///     <see cref="T:Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget" /> interface if <paramref name="elementid" /> is
        ///     set to <see langword="SEID_ResultsList" /> or a pointer to the
        ///     <see cref="T:Microsoft.VisualStudio.OLE.Interop.IOleUndoManager" /> interface if <paramref name="elementid" /> is
        ///     set to <see langword="SEID_UndoManager" />.
        /// </param>
        /// <param name="varValueNew">
        ///     [in] VARIANT that contains the new element value. This parameter contains element-specific
        ///     data, such as a pointer to the <see langword="IOleCommandTarget" /> interface if <paramref name="elementid" /> is
        ///     set to <see langword="SEID_ResultsList" /> or a pointer to the <see langword="IOleUndoManager" /> interface if
        ///     <paramref name="elementid" /> is set to <see langword="SEID_UndoManager" />.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            if (elementid == (uint) VSConstants.VSSELELEMID.SEID_StartupProject)
                _visualStudioProxy.OnStartupProjectChanged();

            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Reports that the project hierarchy, item and/or selection container has changed.
        /// </summary>
        /// <param name="pHierOld">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface
        ///     of the project hierarchy for the previous selection.
        /// </param>
        /// <param name="itemidOld">
        ///     [in] Identifier of the project item for previous selection. For valid
        ///     <paramref name="itemidOld" /> values, see <see langword="VSITEMID" />.
        /// </param>
        /// <param name="pMisOld">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsMultiItemSelect" />
        ///     interface to access a previous multiple selection.
        /// </param>
        /// <param name="pScOld">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.ISelectionContainer" />
        ///     interface to access Properties window data for the previous selection.
        /// </param>
        /// <param name="pHierNew">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface
        ///     of the project hierarchy for the current selection.
        /// </param>
        /// <param name="itemidNew">
        ///     [in] Identifier of the project item for the current selection. For valid
        ///     <paramref name="itemidNew" /> values, see <see langword="VSITEMID" />.
        /// </param>
        /// <param name="pMisNew">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsMultiItemSelect" />
        ///     interface for the current selection.
        /// </param>
        /// <param name="pScNew">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.ISelectionContainer" />
        ///     interface for the current selection.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMisOld, ISelectionContainer pScOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMisNew, ISelectionContainer pScNew)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that a solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            _visualStudioProxy.OnClosedSolution();
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that the project has been loaded.
        /// </summary>
        /// <param name="pStubHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the placeholder hierarchy for the unloaded project.
        /// </param>
        /// <param name="pRealHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project that was loaded.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            _visualStudioProxy.OnProjectLoaded();
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that the project has been opened.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project being loaded.
        /// </param>
        /// <param name="fAdded">
        ///     [in] <see langword="true" /> if the project is added to the solution after the solution is opened.
        ///     <see langword="false" /> if the project is added to the solution while the solution is being opened.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            _visualStudioProxy.OnOpenedProject(pHierarchy, fAdded == 1);
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that the solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <param name="fNewSolution">
        ///     [in] <see langword="true" /> if the solution is being created. <see langword="false" /> if
        ///     the solution was created previously or is being loaded.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            if (!_projectsLoadInBatches)
                _visualStudioProxy.OnOpenedSolution();

            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that the project is about to be closed.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project being closed.
        /// </param>
        /// <param name="fRemoved">
        ///     [in] <see langword="true" /> if the project was removed from the solution before the solution
        ///     was closed. <see langword="false" /> if the project was removed from the solution while the solution was being
        ///     closed.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            _visualStudioProxy.OnClosingProject(pHierarchy);
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that the solution is about to be closed.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            _visualStudioProxy.OnClosingSolution();
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that the project is about to be unloaded.
        /// </summary>
        /// <param name="pRealHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project that will be unloaded.
        /// </param>
        /// <param name="pStubHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the placeholder hierarchy for the project being unloaded.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Queries listening clients as to whether the project can be closed.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project to be closed.
        /// </param>
        /// <param name="fRemoving">
        ///     [in] <see langword="true" /> if the project is being removed from the solution before the
        ///     solution is closed. <see langword="false" /> if the project is being removed from the solution while the solution
        ///     is being closed.
        /// </param>
        /// <param name="pfCancel">
        ///     [out] <see langword="true" /> if the client vetoed the closing of the project.
        ///     <see langword="false" /> if the client approved the closing of the project.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Queries listening clients as to whether the solution can be closed.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <param name="pfCancel">
        ///     [out] <see langword="true" /> if the client vetoed closing the solution.
        ///     <see langword="false" /> if the client approved closing the solution.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Queries listening clients as to whether the project can be unloaded.
        /// </summary>
        /// <param name="pRealHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project to be unloaded.
        /// </param>
        /// <param name="pfCancel">
        ///     [out] <see langword="true" /> if the client vetoed unloading the project.
        ///     <see langword="false" /> if the client approved unloading the project.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that a project has been opened asynchronously.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project being loaded.
        /// </param>
        /// <param name="fAdded">
        ///     [in] <see langword="true" /> if the project is added to the solution after the solution is opened.
        ///     <see langword="false" /> if the project is added to the solution while the solution is being opened.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that a project parent has changed.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the changed project parent.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Notifies listening clients that a project has been renamed.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the renamed project.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            _visualStudioProxy.OnRenamedProject(pHierarchy);
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Queries listening clients as to whether a parent project has changed.
        /// </summary>
        /// <param name="pHierarchy">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the project parent.
        /// </param>
        /// <param name="pNewParentHier">
        ///     [in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
        ///     interface of the changed project parent.
        /// </param>
        /// <param name="pfCancel">
        ///     [in, out] <see langword="true" /> if the client vetoed the closing of the project.
        ///     <see langword="false" /> if the client approved the closing of the project.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Fired when the solution load process is fully complete, including all background loading of projects.
        /// </summary>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterBackgroundSolutionLoadComplete()
        {
            _visualStudioProxy.OnOpenedSolution();
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Fired when the loading of a batch of dependent projects is complete.
        /// </summary>
        /// <param name="fIsBackgroundIdleBatch">
        ///     <see langword="true" /> if the batch is loaded in the background, otherwise
        ///     <see langword="false" />.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Fired when background loading of projects is beginning again after the initial solution open operation has
        ///     completed.
        /// </summary>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnBeforeBackgroundSolutionLoadBegins()
        {
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Fired when loading a batch of dependent projects as part of loading a solution in the background.
        /// </summary>
        /// <param name="fIsBackgroundIdleBatch">
        ///     <see langword="true" /> if the batch is loaded in the background, otherwise
        ///     <see langword="false" />.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
        {
            _projectsLoadInBatches = true;
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Fired before a solution open begins. Extenders can activate a solution load manager by setting
        ///     <see cref="F:Microsoft.VisualStudio.Shell.Interop.__VSPROPID4.VSPROPID_ActiveSolutionLoadManager" />.
        /// </summary>
        /// <param name="pszSolutionFilename">The name of the solution file.</param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnBeforeOpenSolution(string pszSolutionFilename)
        {
            _projectsLoadInBatches = false;
            _visualStudioProxy.OnOpeningSolution();
            return VSConstants.S_OK;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Fired before background loading a batch of projects. Normally a background batch loads a single pending project.
        ///     This is a cancelable event.
        /// </summary>
        /// <param name="pfShouldDelayLoadToNextIdle">
        ///     [out] <see langword="true" /> if other background operations should complete
        ///     before starting to load the project, otherwise <see langword="false" />.
        /// </param>
        /// <returns>
        ///     If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it
        ///     returns an error code.
        /// </returns>
        public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
        {
            pfShouldDelayLoadToNextIdle = false;
            return VSConstants.S_OK;
        }
    }
}