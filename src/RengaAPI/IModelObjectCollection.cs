using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IModelObjectCollection interface - Complete API Reference
    /// This class provides comprehensive access to all IModelObjectCollection interface members
    /// </summary>
    public class IModelObjectCollection : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IModelObjectCollection
        /// </summary>
        public Renga.IModelObjectCollection _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IModelObjectCollection from existing IModelObjectCollection interface
        /// </summary>
        /// <param name="collection">Existing IModelObjectCollection interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObjectCollection(Renga.IModelObjectCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "ModelObjectCollection interface cannot be null.");
            
            this._i = collection;
        }

        /// <summary>
        /// Constructor - Creates IModelObjectCollection from existing ModelObjectCollection
        /// </summary>
        /// <param name="collection">Existing ModelObjectCollection instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObjectCollection(DynDocument.ModelObjectCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "ModelObjectCollection cannot be null.");
            
            if (collection._i == null)
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            this._i = collection._i;
        }

        /// <summary>
        /// Check if model object collection interface is valid
        /// </summary>
        /// <returns>True if collection interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IModelObjectCollection Properties

        /// <summary>
        /// Gets the number of objects in the collection
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
                
                return this._i.Count;
            }
        }

        #endregion

        #region IModelObjectCollection Methods

        /// <summary>
        /// Gets an object by its identifier
        /// </summary>
        /// <param name="id">Object identifier</param>
        /// <returns>IModelObject instance or null if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject GetById(int id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                var modelObject = this._i.GetById(id);
                return modelObject != null ? new IModelObject(modelObject) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object by ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an object by the given index
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <returns>IModelObject instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject GetByIndex(int index)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Collection has {Count} items.");
            
            try
            {
                var modelObject = this._i.GetByIndex(index);
                return new IModelObject(modelObject);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object by index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an object by its unique identifier
        /// </summary>
        /// <param name="uniqueId">Unique identifier (GUID)</param>
        /// <returns>IModelObject instance or null if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject GetByUniqueId(Guid uniqueId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                var modelObject = this._i.GetByUniqueId(uniqueId);
                return modelObject != null ? new IModelObject(modelObject) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object by unique ID {uniqueId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the identifiers of all objects in the collection
        /// </summary>
        /// <returns>Array of object identifiers</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int[] GetIds()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                return (int[])this._i.GetIds();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object IDs: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Collection Methods

        /// <summary>
        /// Gets all objects in the collection as a list
        /// </summary>
        /// <returns>List of IModelObject instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IModelObject> GetAllObjects()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                var objects = new List<IModelObject>();
                for (int i = 0; i < Count; i++)
                {
                    objects.Add(GetByIndex(i));
                }
                return objects;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets objects by their type
        /// </summary>
        /// <param name="objectType">Type of objects to retrieve</param>
        /// <returns>List of IModelObject instances of the specified type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IModelObject> GetObjectsByType(Guid objectType)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.Where(obj => obj.ObjectType == objectType).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get objects by type {objectType}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets objects by their type string
        /// </summary>
        /// <param name="objectTypeString">Type string of objects to retrieve</param>
        /// <returns>List of IModelObject instances of the specified type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IModelObject> GetObjectsByTypeS(string objectTypeString)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            if (string.IsNullOrEmpty(objectTypeString))
                throw new ArgumentException("Object type string cannot be null or empty", nameof(objectTypeString));
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.Where(obj => obj.ObjectTypeS.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get objects by type string '{objectTypeString}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets objects by their names (partial match)
        /// </summary>
        /// <param name="namePattern">Name pattern to search for</param>
        /// <param name="exactMatch">Whether to use exact match or partial match</param>
        /// <returns>List of IModelObject instances matching the name pattern</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IModelObject> GetObjectsByName(string namePattern, bool exactMatch = false)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            if (string.IsNullOrEmpty(namePattern))
                throw new ArgumentException("Name pattern cannot be null or empty", nameof(namePattern));
            
            try
            {
                var allObjects = GetAllObjects();
                if (exactMatch)
                {
                    return allObjects.Where(obj => obj.Name.Equals(namePattern, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    return allObjects.Where(obj => obj.Name.IndexOf(namePattern, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get objects by name '{namePattern}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if an object with the specified ID exists in the collection
        /// </summary>
        /// <param name="id">Object ID to check</param>
        /// <returns>True if object exists, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool ContainsId(int id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                return GetById(id) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an object with the specified unique ID exists in the collection
        /// </summary>
        /// <param name="uniqueId">Unique ID to check</param>
        /// <returns>True if object exists, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool ContainsUniqueId(Guid uniqueId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                return GetByUniqueId(uniqueId) != null;
            }
            catch
            {
                return false;
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
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                var objectIds = GetIds();
                var objectTypes = GetAllObjects().Select(obj => obj.ObjectTypeS).Distinct().ToList();
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Count", Count },
                    { "HasObjects", Count > 0 },
                    { "ObjectIds", objectIds },
                    { "ObjectTypes", objectTypes },
                    { "UniqueObjectTypes", objectTypes.Count }
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
                    { "ObjectIds", new int[0] },
                    { "ObjectTypes", new List<string>() },
                    { "UniqueObjectTypes", 0 }
                };
            }
        }

        /// <summary>
        /// Gets a summary of object types and their counts
        /// </summary>
        /// <returns>Dictionary with object type counts</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, int> GetObjectTypeCounts()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObjectCollection interface is not initialized.");
            
            try
            {
                var allObjects = GetAllObjects();
                return allObjects.GroupBy(obj => obj.ObjectTypeS)
                                .ToDictionary(group => group.Key, group => group.Count());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object type counts: {ex.Message}", ex);
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IModelObjectCollection COM object: {ex.Message}");
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
        ~IModelObjectCollection()
        {
            Dispose(false);
        }

        #endregion
    }
}
