using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynObjects;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IExportedObject3DCollection interface - Complete API Reference
    /// This class provides comprehensive access to all IExportedObject3DCollection interface members
    /// </summary>
    public class IExportedObject3DCollection : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IExportedObject3DCollection
        /// </summary>
        public Renga.IExportedObject3DCollection _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IExportedObject3DCollection from existing IExportedObject3DCollection interface
        /// </summary>
        /// <param name="collection">Existing IExportedObject3DCollection interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IExportedObject3DCollection(Renga.IExportedObject3DCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "ExportedObject3DCollection interface cannot be null.");
            
            this._i = collection;
        }

        /// <summary>
        /// Check if exported object 3D collection interface is valid
        /// </summary>
        /// <returns>True if collection interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IExportedObject3DCollection Properties

        /// <summary>
        /// Gets the number of exported 3D objects in the collection
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
                
                return this._i.Count;
            }
        }

        #endregion

        #region IExportedObject3DCollection Methods

        /// <summary>
        /// Gets an exported 3D object by the given index
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <returns>IExportedObject3D instance
        /// </returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IExportedObject3D GetByIndex(int index)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Collection has {Count} items.");
            
            try
            {
                var exportedObject3D = this._i.Get(index);
                return new IExportedObject3D(exportedObject3D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get exported 3D object by index {index}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Collection Methods

        /// <summary>
        /// Gets all exported 3D objects in the collection as a list
        /// </summary>
        /// <returns>List of IExportedObject3D instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IExportedObject3D> GetAllObjects()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var objects = new List<IExportedObject3D>();
                for (int i = 0; i < Count; i++)
                {
                    objects.Add(GetByIndex(i));
                }
                return objects;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all exported 3D objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets exported 3D objects by model object ID
        /// </summary>
        /// <param name="modelObjectId">Model object ID to filter by</param>
    /// <returns>List of IExportedObject3D instances with the specified model object ID</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
    public List<IExportedObject3D> GetObjectsByModelObjectId(int modelObjectId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.Where(obj => obj.ModelObjectId == modelObjectId).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get objects by model object ID {modelObjectId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets unique model object IDs from all exported objects
        /// </summary>
        /// <returns>List of unique model object IDs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<int> GetUniqueModelObjectIds()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.Select(obj => obj.ModelObjectId).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get unique model object IDs: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the count of exported objects for a specific model object
        /// </summary>
        /// <param name="modelObjectId">Model object ID to count</param>
        /// <returns>Number of exported objects for the specified model object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetObjectCountByModelObjectId(int modelObjectId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var objectsWithModelId = GetObjectsByModelObjectId(modelObjectId);
                return objectsWithModelId.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object count by model object ID {modelObjectId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets exported objects by model object type
        /// </summary>
        /// <param name="objectType">Model object type to filter by</param>
    /// <returns>List of IExportedObject3D instances with the specified object type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
    public List<IExportedObject3D> GetObjectsByType(Guid objectType)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.Where(obj => obj.ModelObjectType == objectType).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get objects by type {objectType}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets unique model object types from all exported objects
        /// </summary>
        /// <returns>List of unique model object types</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<Guid> GetUniqueObjectTypes()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.Select(obj => obj.ModelObjectType).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get unique object types: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets collection information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with collection information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetCollectionInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var uniqueModelObjectIds = GetUniqueModelObjectIds();
                var uniqueObjectTypes = GetUniqueObjectTypes();
                var modelObjectCounts = uniqueModelObjectIds.ToDictionary(
                    id => id.ToString(), 
                    id => GetObjectCountByModelObjectId(id)
                );
                var objectTypeCounts = uniqueObjectTypes.ToDictionary(
                    type => type.ToString(), 
                    type => GetObjectsByType(type).Count
                );
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Count", Count },
                    { "HasObjects", Count > 0 },
                    { "UniqueModelObjectIds", uniqueModelObjectIds },
                    { "UniqueObjectTypes", uniqueObjectTypes },
                    { "ModelObjectCounts", modelObjectCounts },
                    { "ObjectTypeCounts", objectTypeCounts },
                    { "UniqueModelObjectCount", uniqueModelObjectIds.Count },
                    { "UniqueObjectTypeCount", uniqueObjectTypes.Count }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "Count", 0 },
                    { "HasObjects", false },
                    { "UniqueModelObjectIds", new List<int>() },
                    { "UniqueObjectTypes", new List<Guid>() },
                    { "ModelObjectCounts", new Dictionary<string, int>() },
                    { "ObjectTypeCounts", new Dictionary<string, int>() },
                    { "UniqueModelObjectCount", 0 },
                    { "UniqueObjectTypeCount", 0 }
                };
            }
        }

        /// <summary>
        /// Gets a summary of model object usage across all exported objects
        /// </summary>
        /// <returns>Dictionary with model object ID and exported object count pairs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, int> GetModelObjectUsageSummary()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.GroupBy(obj => obj.ModelObjectId)
                              .ToDictionary(group => group.Key, group => group.Count());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get model object usage summary: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a summary of object type usage across all exported objects
        /// </summary>
        /// <returns>Dictionary with object type and exported object count pairs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<Guid, int> GetObjectTypeUsageSummary()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ExportedObject3DCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.GroupBy(obj => obj.ModelObjectType)
                              .ToDictionary(group => group.Key, group => group.Count());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object type usage summary: {ex.Message}", ex);
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IExportedObject3DCollection COM object: {ex.Message}");
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
        ~IExportedObject3DCollection()
        {
            Dispose(false);
        }

        #endregion
    }
}
