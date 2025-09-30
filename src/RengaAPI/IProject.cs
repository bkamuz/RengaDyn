using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IProject interface - Complete API Reference
    /// This class provides comprehensive access to all IProject interface members
    /// </summary>
    public class IProject : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IProject
        /// </summary>
        public Renga.IProject _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IProject from COM object
        /// </summary>
        /// <param name="rengaProjectObject">The Renga.IProject COM object</param>
        internal IProject(object rengaProjectObject)
        {
            if (rengaProjectObject == null)
                throw new ArgumentNullException(nameof(rengaProjectObject), "Renga IProject cannot be null.");
            
            this._i = rengaProjectObject as Renga.IProject;
            if (this._i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IProject.");
        }

        /// <summary>
        /// Gets the file path of the project
        /// </summary>
        public string FilePath
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Project interface is not initialized.");
                
                try
                {
                    return this._i.FilePath;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get project file path: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the journal file path
        /// </summary>
        public string JournalPath
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Project interface is not initialized.");
                
                try
                {
                    return this._i.JournalPath;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get journal path: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Indicates whether the project has ever been saved
        /// </summary>
        public bool HasFile
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Project interface is not initialized.");
                
                try
                {
                    return this._i.HasFile();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to check if project has file: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Indicates whether the project has unsaved changes
        /// </summary>
        public bool HasUnsavedChanges
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Project interface is not initialized.");
                
                try
                {
                    return this._i.HasUnsavedChanges();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to check if project has unsaved changes: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Indicates whether the project has an active operation
        /// </summary>
        public bool HasActiveOperation
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Project interface is not initialized.");
                
                try
                {
                    return this._i.HasActiveOperation();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to check if project has active operation: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the column style manager for this project
        /// </summary>
        /// <returns>IColumnStyleManager wrapper</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IColumnStyleManager GetColumnStyleManager()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            try
            {
                var mgr = this._i.ColumnStyleManager;
                return new IColumnStyleManager(mgr);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get ColumnStyleManager: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the layered material manager for this project
        /// </summary>
        /// <returns>ILayeredMaterialManager wrapper</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ILayeredMaterialManager GetLayeredMaterialManager()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            try
            {
                var mgr = this._i.LayeredMaterialManager;
                return new ILayeredMaterialManager(mgr);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get LayeredMaterialManager: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates an operation for editing the project
        /// </summary>
        /// <returns>IOperation object for managing changes</returns>
        public IOperation CreateOperation()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            
            try
            {
                var rengaOperation = this._i.CreateOperation();
                return new IOperation(rengaOperation);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create operation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates an operation with undo history for the specified model
        /// </summary>
        /// <param name="modelId">The model ID for the operation</param>
        /// <returns>IOperation object with undo support</returns>
        public IOperation CreateOperationWithUndo(Guid modelId)
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            
            try
            {
                var rengaOperation = this._i.CreateOperationWithUndo(modelId);
                return new IOperation(rengaOperation);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create operation with undo for model {modelId}: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Disposes the Renga.IProject COM object
        /// </summary>
        public void Dispose()
        {
            if (_i != null && !_disposed)
            {
                Marshal.ReleaseComObject(_i);
                _i = null;
                _disposed = true;
            }
        }

        ~IProject()
        {
            Dispose();
        }
    }
}
