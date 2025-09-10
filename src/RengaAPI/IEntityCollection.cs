using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IEntityCollection interface - Complete API Reference
    /// This class provides comprehensive access to all IEntityCollection interface members
    /// </summary>
    public class IEntityCollection : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IEntityCollection
        /// </summary>
        public Renga.IEntityCollection _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IEntityCollection from COM object
        /// </summary>
        /// <param name="rengaEntityCollectionObject">The Renga.IEntityCollection COM object</param>
        internal IEntityCollection(object rengaEntityCollectionObject)
        {
            if (rengaEntityCollectionObject == null)
                throw new ArgumentNullException(nameof(rengaEntityCollectionObject), "Renga IEntityCollection cannot be null.");
            
            this._i = rengaEntityCollectionObject as Renga.IEntityCollection;
            if (this._i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IEntityCollection.");
        }

        /// <summary>
        /// Gets the number of entities in the collection
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("EntityCollection interface is not initialized.");
                
                try
                {
                    return this._i.Count;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get collection count: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Removes all entities currently present in the collection
        /// Warning: If any project entity is referencing the entity being removed, calling this method may result in program termination
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Clear()
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                this._i.Clear();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to clear collection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Indicates whether the collection contains the entity with the specified ID
        /// </summary>
        /// <param name="id">The entity identifier</param>
        /// <returns>True if the collection contains the entity, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool Contains(int id)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                return this._i.Contains(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if collection contains entity with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Indicates whether the collection contains the entity with the specified unique ID
        /// </summary>
        /// <param name="uniqueId">The entity unique identifier</param>
        /// <returns>True if the collection contains the entity, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool ContainsUniqueId(Guid uniqueId)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                return this._i.ContainsUniqueId(uniqueId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if collection contains entity with unique ID {uniqueId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Indicates whether the collection contains the entity with the specified unique ID (string version)
        /// </summary>
        /// <param name="uniqueIdS">The entity unique identifier as string</param>
        /// <returns>True if the collection contains the entity, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool ContainsUniqueIdS(string uniqueIdS)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            if (string.IsNullOrEmpty(uniqueIdS))
                throw new ArgumentException("Unique ID string cannot be null or empty.", nameof(uniqueIdS));
            
            try
            {
                return this._i.ContainsUniqueIdS(uniqueIdS);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if collection contains entity with unique ID string '{uniqueIdS}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new entity using the provided arguments
        /// </summary>
        /// <param name="args">Arguments for creating the new entity</param>
        /// <returns>Newly created entity</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IEntity Create(DynRenga.DynDocument.NewEntityArgs args)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            if (args == null)
                throw new ArgumentNullException(nameof(args), "NewEntityArgs cannot be null.");
            
            try
            {
                var rengaEntity = this._i.Create(args._i);
                return new IEntity(rengaEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create entity: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates arguments for adding a new entity
        /// </summary>
        /// <returns>New entity arguments object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.DynDocument.NewEntityArgs CreateNewEntityArgs()
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                var rengaArgs = this._i.CreateNewEntityArgs();
                return new DynRenga.DynDocument.NewEntityArgs(rengaArgs);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create new entity args: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an entity by its identifier
        /// </summary>
        /// <param name="id">The entity identifier</param>
        /// <returns>Entity with the specified ID</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IEntity GetById(int id)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                var rengaEntity = this._i.GetById(id);
                return new IEntity(rengaEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity by ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an entity by its index in the collection
        /// </summary>
        /// <param name="index">The entity index</param>
        /// <returns>Entity at the specified index</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IEntity GetByIndex(int index)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Collection has {this.Count} items.");
            
            try
            {
                var rengaEntity = this._i.GetByIndex(index);
                return new IEntity(rengaEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity by index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an entity by its unique identifier
        /// </summary>
        /// <param name="uniqueId">The entity unique identifier</param>
        /// <returns>Entity with the specified unique ID</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IEntity GetByUniqueId(Guid uniqueId)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                var rengaEntity = this._i.GetByUniqueId(uniqueId);
                return new IEntity(rengaEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity by unique ID {uniqueId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets an entity by its unique identifier (string version)
        /// </summary>
        /// <param name="uniqueIdS">The entity unique identifier as string</param>
        /// <returns>Entity with the specified unique ID</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IEntity GetByUniqueIdS(string uniqueIdS)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            if (string.IsNullOrEmpty(uniqueIdS))
                throw new ArgumentException("Unique ID string cannot be null or empty.", nameof(uniqueIdS));
            
            try
            {
                var rengaEntity = this._i.GetByUniqueIdS(uniqueIdS);
                return new IEntity(rengaEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity by unique ID string '{uniqueIdS}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all entity identifiers in the collection
        /// </summary>
        /// <returns>Array of entity identifiers</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int[] GetIds()
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                var ids = this._i.GetIds();
                return ids.Cast<int>().ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity IDs: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all entity unique identifiers in the collection
        /// </summary>
        /// <returns>Collection of unique identifiers</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetUniqueIds()
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                return this._i.GetUniqueIds();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity unique IDs: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Removes an entity from the collection by its identifier
        /// </summary>
        /// <param name="id">The entity identifier to remove</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Remove(int id)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                this._i.Remove(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove entity with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Removes an entity from the collection by its unique identifier
        /// </summary>
        /// <param name="uniqueId">The entity unique identifier to remove</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void RemoveByUniqueId(Guid uniqueId)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                this._i.RemoveByUniqueId(uniqueId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove entity with unique ID {uniqueId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Removes an entity from the collection by its unique identifier (string version)
        /// </summary>
        /// <param name="uniqueIdS">The entity unique identifier as string to remove</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void RemoveByUniqueIdS(string uniqueIdS)
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            if (string.IsNullOrEmpty(uniqueIdS))
                throw new ArgumentException("Unique ID string cannot be null or empty.", nameof(uniqueIdS));
            
            try
            {
                this._i.RemoveByUniqueIdS(uniqueIdS);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove entity with unique ID string '{uniqueIdS}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all entities in the collection as a list
        /// </summary>
        /// <returns>List of all entities in the collection</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IEntity> GetAllEntities()
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                var entities = new List<IEntity>();
                var ids = this.GetIds();
                
                foreach (int id in ids)
                {
                    try
                    {
                        var entity = this.GetById(id);
                        entities.Add(entity);
                    }
                    catch
                    {
                        // Skip entities that can't be retrieved
                        continue;
                    }
                }
                
                return entities;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all entities: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets comprehensive collection information
        /// </summary>
        /// <returns>Dictionary containing collection properties</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Count", "EntityIds", "EntityCount", "DebugInfo" })]
        public Dictionary<string, object> GetCollectionInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("EntityCollection interface is not initialized.");
            
            try
            {
                var info = new Dictionary<string, object>();
                info["Count"] = this.Count;
                info["EntityIds"] = this.GetIds();
                info["EntityCount"] = this.Count;
                
                var debugInfo = $"--- EntityCollection Debug Info ---\n";
                debugInfo += $"Count: {this.Count}\n";
                debugInfo += $"Interface Status: {(this._i != null ? "✅ Initialized" : "❌ Not Initialized")}\n";
                
                var ids = this.GetIds();
                debugInfo += $"Entity IDs: [{string.Join(", ", ids)}]\n";
                
                info["DebugInfo"] = debugInfo;
                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get collection information: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets debug information about the collection
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ EntityCollection interface is not initialized.";
            
            try
            {
                var debugInfo = $"--- IEntityCollection Debug Info ---\n";
                debugInfo += $"Count: {this.Count}\n";
                debugInfo += $"Interface Status: ✅ Initialized\n";
                
                var ids = this.GetIds();
                debugInfo += $"Entity IDs: [{string.Join(", ", ids)}]\n";
                debugInfo += $"Entity Count: {ids.Length}\n";
                
                // Test some basic operations
                if (ids.Length > 0)
                {
                    try
                    {
                        var firstEntity = this.GetById(ids[0]);
                        debugInfo += $"✅ First entity retrieval successful (ID: {ids[0]})\n";
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ First entity retrieval failed: {ex.Message}\n";
                    }
                }
                
                return debugInfo;
            }
            catch (Exception ex)
            {
                return $"❌ Failed to get debug information: {ex.Message}";
            }
        }

        /// <summary>
        /// Disposes the Renga.IEntityCollection COM object
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

        ~IEntityCollection()
        {
            Dispose();
        }
    }
}
