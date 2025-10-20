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
        /// Gets the ProjectInfo as a DynDocument wrapper
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.DynDocument.Project.ProjectInfo GetProjectInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            try
            {
                return new DynRenga.DynDocument.Project.ProjectInfo(this._i.ProjectInfo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get ProjectInfo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets BuildingInfo wrapper
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.DynDocument.Project.BuildingInfo GetBuildingInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            try
            {
                return new DynRenga.DynDocument.Project.BuildingInfo(this._i.BuildingInfo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get BuildingInfo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the DataExporter for the project
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IDataExporter GetDataExporter()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            try
            {
                return new IDataExporter(this);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get DataExporter: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an IEntityCollection wrapper for BeamStyles
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IEntityCollection GetBeamStyles()
        {
            if (this._i == null)
                throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.BeamStyles); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get BeamStyles: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for ColumnStyles
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetColumnStyles()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.ColumnStyles); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get ColumnStyles: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for DoorStyles
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDoorStyles()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.DoorStyles); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DoorStyles: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for Drawings
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDrawings()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.Drawings); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Drawings: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for Drawings2 (alternative drawings collection)
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDrawings2()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.Drawings2); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Drawings2: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for DuctAccessoryCategories
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDuctAccessoryCategories()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.DuctAccessoryCategories); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DuctAccessoryCategories: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for DuctAccessoryStyles
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDuctAccessoryStyles()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.DuctAccessoryStyles); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DuctAccessoryStyles: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for DuctFittingCategories
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDuctFittingCategories()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.DuctFittingCategories); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DuctFittingCategories: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for DuctFittingStyles
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDuctFittingStyles()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.DuctFittingStyles); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DuctFittingStyles: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for DuctStyles
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetDuctStyles()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.DuctStyles); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DuctStyles: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets the profile description manager
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IProfileDescriptionManager GetProfileDescriptionManager()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IProfileDescriptionManager(this._i.ProfileDescriptionManager); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get ProfileDescriptionManager: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for LayeredMaterials
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetLayeredMaterials()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.LayeredMaterials); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get LayeredMaterials: {ex.Message}", ex); }
        }

    /// <summary>
    /// Gets an IEntityCollection wrapper for Materials
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(true)]
    public IEntityCollection GetMaterials()
        {
            if (this._i == null) throw new InvalidOperationException("Project interface is not initialized.");
            try { return new IEntityCollection(this._i.Materials); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Materials: {ex.Message}", ex); }
        }

        /// <summary>
        /// Indicates whether the IProject wrapper is initialized and not disposed
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized()
        {
            return this._i != null && !_disposed;
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

        /// <summary>
        /// Finalizer: release COM resources if Dispose wasn't called
        /// </summary>
        ~IProject()
        {
            Dispose();
        }
    }
}
