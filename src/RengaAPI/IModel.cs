using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynDocument;
using DynRenga.DynDocument.Project;
using DynRenga.DynObjects;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IModel interface - Complete API Reference
    /// This class provides comprehensive access to all IModel interface members
    /// </summary>
    public class IModel : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IModel
        /// </summary>
        public Renga.IModel _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IModel from IApplication
        /// </summary>
        /// <param name="application">Renga application instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModel(IApplication application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application), "Application cannot be null. Make sure you have a valid Renga application loaded.");
            
            if (application._i == null)
                throw new InvalidOperationException("Application interface is not initialized. This usually means:\n" +
                    "1. Renga application is not running - please start Renga first\n" +
                    "2. No project is loaded in Renga - please open a project in Renga");
            
            if (application.Project == null)
                throw new InvalidOperationException("Project is not available. This usually means:\n" +
                    "1. No project is loaded in Renga - please open a project in Renga\n" +
                    "2. The project failed to load properly - check the project file");
            
            try
            {
                // Get project from application
                var project = application.Project as dynamic;
                this._i = project.Model as Renga.IModel;
                
                if (this._i == null)
                    throw new InvalidOperationException("Failed to get Model from Project. This usually means:\n" +
                        "1. The project is not properly loaded in Renga\n" +
                        "2. The project file is corrupted or invalid\n" +
                        "3. Renga version compatibility issues\n" +
                        "Please check that you have a valid project open in Renga.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to access model from application: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Constructor - Creates IModel from existing IModel interface
        /// </summary>
        /// <param name="model">Existing IModel interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModel(Renga.IModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Model interface cannot be null.");
            
            this._i = model;
        }

        /// <summary>
        /// Constructor - Creates IModel from Project
        /// </summary>
        /// <param name="project">Project instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModel(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project), "Project cannot be null. Make sure you have a valid Renga project loaded.");
            
            if (project._i == null)
                throw new InvalidOperationException("Project interface is not initialized. This usually means:\n" +
                    "1. Renga application is not running - please start Renga first\n" +
                    "2. No project is loaded in Renga - please open a project in Renga\n" +
                    "3. The project failed to load properly - check the project file");
            
            this._i = project._i.Model;
            
            if (this._i == null)
                throw new InvalidOperationException("Failed to get Model from Project. This usually means:\n" +
                    "1. The project is not properly loaded in Renga\n" +
                    "2. The project file is corrupted or invalid\n" +
                    "3. Renga version compatibility issues\n" +
                    "Please check that you have a valid project open in Renga.");
        }

        /// <summary>
        /// Check if model interface is valid
        /// </summary>
        /// <returns>True if model interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IModel Properties

        /// <summary>
        /// Gets the identifier of the model
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid Id
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                return this._i.Id;
            }
        }

        #endregion

        #region IModel Methods

        /// <summary>
        /// Creates arguments for creating a new entity
        /// </summary>
        /// <returns>INewEntityArgs object for creating new entities</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public INewEntityArgs CreateNewEntityArgs()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var rengaNewEntityArgs = this._i.CreateNewEntityArgs();
                return new INewEntityArgs(rengaNewEntityArgs);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create new entity args: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a new model object
        /// </summary>
        /// <param name="args">Arguments for creating the model object</param>
        /// <returns>Created IModelObject instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateObject(object args)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (args == null)
                throw new ArgumentNullException(nameof(args), "Entity args cannot be null.");
            
            try
            {
                var modelObject = this._i.CreateObject(args as Renga.INewEntityArgs);
                return new ModelObject(modelObject);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create model object: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates an operation (deprecated - use IProject::CreateOperation instead)
        /// </summary>
        /// <returns>IOperation object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IOperation CreateOperation()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
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
        /// Returns the local identifier of the object by its unique identifier
        /// </summary>
        /// <param name="uniqueId">Unique identifier (GUID)</param>
        /// <returns>Local identifier (int)</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetIdFromUniqueId(Guid uniqueId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetIdFromUniqueId(uniqueId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get ID from unique ID {uniqueId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the local identifier of the object by its unique identifier (string version)
        /// </summary>
        /// <param name="uniqueId">Unique identifier as string</param>
        /// <returns>Local identifier (int)</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetIdFromUniqueIdS(string uniqueId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (string.IsNullOrEmpty(uniqueId))
                throw new ArgumentException("Unique ID string cannot be null or empty", nameof(uniqueId));
            
            try
            {
                return this._i.GetIdFromUniqueIdS(uniqueId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get ID from unique ID string '{uniqueId}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns a collection of all objects present in the model
        /// </summary>
        /// <returns>IModelObjectCollection containing all model objects</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObjectCollection GetObjects()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = this._i.GetObjects();
                return new IModelObjectCollection(collection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get model objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the unique identifier of the object by its local identifier
        /// </summary>
        /// <param name="id">Local identifier (int)</param>
        /// <returns>Unique identifier (GUID)</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid GetUniqueIdFromId(int id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetUniqueIdFromId(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get unique ID from ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the unique identifier of the object by its local identifier (string version)
        /// </summary>
        /// <param name="id">Local identifier (int)</param>
        /// <returns>Unique identifier as string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetUniqueIdFromIdS(int id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetUniqueIdFromIdS(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get unique ID string from ID {id}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Model Methods

        /// <summary>
        /// Gets all model objects as a list
        /// </summary>
        /// <returns>List of IModelObject instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IModelObject> GetAllObjects()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetObjects();
                var objects = new List<IModelObject>();
                
                for (int i = 0; i < collection.Count; i++)
                {
                    objects.Add(collection.GetByIndex(i));
                }
                
                return objects;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all model objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets model objects by type
        /// </summary>
        /// <param name="objectType">Type of objects to retrieve</param>
        /// <returns>List of IModelObject instances of the specified type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IModelObject> GetObjectsByType(Guid objectType)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
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
        /// Gets a model object by its ID
        /// </summary>
        /// <param name="id">Object ID</param>
        /// <returns>IModelObject instance or null if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject GetObjectById(int id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetObjects();
                return collection.GetById(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object by ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a model object by its unique ID
        /// </summary>
        /// <param name="uniqueId">Unique ID (GUID)</param>
        /// <returns>IModelObject instance or null if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject GetObjectByUniqueId(Guid uniqueId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var id = GetIdFromUniqueId(uniqueId);
                return GetObjectById(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object by unique ID {uniqueId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a model object by its unique ID string
        /// </summary>
        /// <param name="uniqueIdString">Unique ID as string</param>
        /// <returns>IModelObject instance or null if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject GetObjectByUniqueIdS(string uniqueIdString)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var id = GetIdFromUniqueIdS(uniqueIdString);
                return GetObjectById(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object by unique ID string '{uniqueIdString}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the count of objects in the model
        /// </summary>
        /// <returns>Number of objects in the model</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetObjectCount()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var collection = GetObjects();
                return collection.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get object count: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets model information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with model information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetModelInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var objectCount = GetObjectCount();
                var modelId = Id;
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "ModelId", modelId },
                    { "ObjectCount", objectCount },
                    { "HasObjects", objectCount > 0 }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "ModelId", Guid.Empty },
                    { "ObjectCount", 0 },
                    { "HasObjects", false }
                };
            }
        }

        /// <summary>
        /// Gets supported object types that can be created
        /// </summary>
        /// <returns>List of supported object type names</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<string> GetSupportedObjectTypes()
        {
            return new List<string>
            {
                "AssemblyInstance",
                "Column",
                "Door",
                "Element",
                "Floor",
                "Hatch",
                "Hole",
                "IsolatedFoundation",
                "Level",
                "Opening",
                "Plate",
                "Railing",
                "Ramp",
                "Room",
                "Stairway",
                "TextObject",
                "Wall",
                "WallFoundation",
                "Window"
            };
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IModel COM object: {ex.Message}");
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
        ~IModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
