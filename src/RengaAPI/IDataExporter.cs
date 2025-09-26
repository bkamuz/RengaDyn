using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynObjects;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IDataExporter interface - Complete API Reference
    /// This class provides comprehensive access to all IDataExporter interface members
    /// Allows the user to export different types of data from the application
    /// </summary>
    public class IDataExporter : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IDataExporter
        /// </summary>
        public Renga.IDataExporter _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IDataExporter from COM object
        /// </summary>
        /// <param name="rengaDataExporterObject">The Renga.IDataExporter COM object</param>
        internal IDataExporter(object rengaDataExporterObject)
        {
            if (rengaDataExporterObject == null)
                throw new ArgumentNullException(nameof(rengaDataExporterObject), "Renga IDataExporter cannot be null.");
            
            this._i = rengaDataExporterObject as Renga.IDataExporter;
            if (this._i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IDataExporter.");
        }

        /// <summary>
        /// Constructor - Creates IDataExporter from IProject
        /// </summary>
        /// <param name="project">Project instance containing the DataExporter</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IDataExporter(IProject project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project), "Project cannot be null. Make sure you have a valid Renga project loaded.");
            
            if (project._i == null)
                throw new InvalidOperationException("Project interface is not initialized. This usually means:\n" +
                    "1. Renga application is not running - please start Renga first\n" +
                    "2. No project is loaded in Renga - please open a project in Renga\n" +
                    "3. The project failed to load properly - check the project file");
            
            try
            {
                this._i = project._i.DataExporter;
                
                if (this._i == null)
                    throw new InvalidOperationException("Failed to get DataExporter from Project. This usually means:\n" +
                        "1. The project is not properly loaded in Renga\n" +
                        "2. The project file is corrupted or invalid\n" +
                        "3. Renga version compatibility issues\n" +
                        "Please check that you have a valid project open in Renga.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to access DataExporter from project: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if DataExporter interface is valid
        /// </summary>
        /// <returns>True if DataExporter interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IDataExporter Methods

        /// <summary>
        /// Exports grids for all objects as a collection of separate grids with their materials
        /// Note that the grids will share the materials (i.e. have equivalent material IDs) 
        /// exactly the same way as the corresponding objects did in the Renga model
        /// </summary>
        /// <returns>IGridWithMaterialCollection containing all grids with their materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IGridWithMaterialCollection GetGrids()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var gridsCollection = this._i.GetGrids();
                return new IGridWithMaterialCollection(gridsCollection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get grids: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exports scene objects as a collection of separate ExportedObject3D items
        /// Returns a collection of ExportedObject3D items representing objects in the scene, 
        /// containing the geometry data
        /// </summary>
        /// <returns>IExportedObject3DCollection containing all exported 3D objects</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IExportedObject3DCollection GetObjects3D()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var objectsCollection = this._i.GetObjects3D();
                return new IExportedObject3DCollection(objectsCollection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get 3D objects: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced DataExporter Methods

        /// <summary>
        /// Gets all grids as a list
        /// </summary>
        /// <returns>List of GridWithMaterial instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<GridWithMaterial> GetAllGrids()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetGrids();
                var grids = new List<GridWithMaterial>();
                
                for (int i = 0; i < collection.Count; i++)
                {
                    grids.Add(collection.GetByIndex(i));
                }
                
                return grids;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all grids: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all 3D objects as a list
        /// </summary>
        /// <returns>List of IExportedObject3D instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IExportedObject3D> GetAllObjects3D()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetObjects3D();
                var objects = new List<IExportedObject3D>();

                for (int i = 0; i < collection.Count; i++)
                {
                    objects.Add(collection.GetByIndex(i));
                }

                return objects;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all 3D objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the count of grids available for export
        /// </summary>
        /// <returns>Number of grids in the collection</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetGridCount()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetGrids();
                return collection.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get grid count: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the count of 3D objects available for export
        /// </summary>
        /// <returns>Number of 3D objects in the collection</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetObject3DCount()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetObjects3D();
                return collection.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get 3D object count: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets DataExporter information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with DataExporter information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetDataExporterInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("DataExporter interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var gridCount = GetGridCount();
                var object3DCount = GetObject3DCount();
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "GridCount", gridCount },
                    { "Object3DCount", object3DCount },
                    { "HasGrids", gridCount > 0 },
                    { "HasObjects3D", object3DCount > 0 }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "GridCount", 0 },
                    { "Object3DCount", 0 },
                    { "HasGrids", false },
                    { "HasObjects3D", false }
                };
            }
        }

        /// <summary>
        /// Gets debug information about the DataExporter
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ DataExporter is null";

            try
            {
                var info = "🔧 DataExporter Debug Information:\n";
                info += $"✅ Is Valid: {IsValid()}\n";
                
                try
                {
                    var gridCount = GetGridCount();
                    info += $"✅ Grid Count: {gridCount}\n";
                }
                catch (Exception ex)
                {
                    info += $"❌ Grid Count Error: {ex.Message}\n";
                }
                
                try
                {
                    var object3DCount = GetObject3DCount();
                    info += $"✅ 3D Object Count: {object3DCount}\n";
                }
                catch (Exception ex)
                {
                    info += $"❌ 3D Object Count Error: {ex.Message}\n";
                }

                return info;
            }
            catch (Exception ex)
            {
                return $"❌ Debug info failed: {ex.Message}";
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases COM object resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Protected method for releasing resources
        /// </summary>
        /// <param name="disposing">true if called from Dispose(), false if from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    if (_i != null)
                    {
                        try
                        {
                            // Release COM object
                            if (Marshal.IsComObject(_i))
                            {
                                Marshal.ReleaseComObject(_i);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ignore errors when releasing, as object may already be released
                            System.Diagnostics.Debug.WriteLine($"Error releasing IDataExporter COM object: {ex.Message}");
                        }
                        finally
                        {
                            _i = null;
                        }
                    }
                }
                
                _disposed = true;
            }
        }
        
        /// <summary>
        /// Finalizer for releasing unmanaged resources
        /// </summary>
        ~IDataExporter()
        {
            Dispose(false);
        }

        #endregion
    }
}
