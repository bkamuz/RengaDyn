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
                // Get project wrapper from application (IProject)
                var projectWrapper = application.Project;
                if (projectWrapper == null)
                    throw new InvalidOperationException("Project wrapper is null. Ensure a project is loaded in Renga.");

                // Access underlying COM project and obtain its Model
                this._i = projectWrapper._i?.Model as Renga.IModel;

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
        /// Constructor - Creates IModel from Drawing (drawing model).
        /// Use this to create drawing objects (DrawingText, DrawingImage, DrawingReferenceDrawing) on the sheet.
        /// The drawing entity exposes IModel as an additional interface per Renga API.
        /// </summary>
        /// <param name="drawing">Drawing instance (e.g. from Project.Drawings2)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModel(Drawing drawing)
        {
            if (drawing == null)
                throw new ArgumentNullException(nameof(drawing), "Drawing cannot be null.");
            
            if (drawing._i == null)
                throw new InvalidOperationException("Drawing interface is not initialized.");
            
            // IModel can be obtained as additional interface from IEntity when entity is a Drawing (Drawings2)
            var drawingModel = drawing._i as Renga.IModel;
            if (drawingModel == null)
            {
                var entity = drawing._i as Renga.IEntity;
                if (entity != null)
                {
                    var modelObj = entity.GetInterfaceByName("IModel");
                    drawingModel = modelObj as Renga.IModel;
                }
            }
            
            if (drawingModel == null)
                throw new InvalidOperationException("Drawing does not support IModel. Ensure the entity is from Project.Drawings2.");
            
            this._i = drawingModel;
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
        /// Creates a new model object using direct COM interface manipulation
        /// This method bypasses wrapper layers to ensure proper TypeId synchronization.
        /// For 2D placement based objects (e.g. DrawingText) use placement2D or placement2DStruct.
        /// For 3D/level based objects (e.g. ModelText) use placement3D and hostObjectId.
        /// </summary>
        /// <param name="typeId">The object type ID (required)</param>
        /// <param name="hostObjectId">The host object ID (optional, -1 to skip; for ModelText use level ID)</param>
        /// <param name="categoryId">The category ID (optional, -1 to skip)</param>
        /// <param name="placement3D">The 3D placement (optional, null to skip; for building model objects)</param>
        /// <param name="placement2D">The 2D placement wrapper (optional; for drawing model objects e.g. DrawingText)</param>
        /// <param name="placement2DStruct">The 2D placement struct from Placement2D.CreateStruct (optional; use when no wrapper)</param>
        /// <returns>Dictionary with ModelObject, DebugInfo, and Success status</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo", "Success" })]
        public Dictionary<string, object> CreateObject(Guid typeId, int hostObjectId = -1, int categoryId = -1, DynRenga.DynGeometry.Placement3D placement3D = null, DynRenga.DynGeometry.Placement2D placement2D = null, Renga.Placement2D? placement2DStruct = null)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Model interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            var debugInfo = new System.Text.StringBuilder();
            debugInfo.AppendLine("🔧 IModel.CreateObject Debug Info:");
            debugInfo.AppendLine($"TypeId: {typeId}");
            debugInfo.AppendLine($"HostObjectId: {hostObjectId}");
            debugInfo.AppendLine($"CategoryId: {categoryId}");
            debugInfo.AppendLine($"Placement3D provided: {placement3D != null}");
            debugInfo.AppendLine($"Placement2D provided: {placement2D != null || placement2DStruct.HasValue}");
            
            try
            {
                // Create INewEntityArgs directly on COM interface
                var rengaNewEntityArgs = this._i.CreateNewEntityArgs();
                debugInfo.AppendLine($"COM INewEntityArgs created: {rengaNewEntityArgs != null}");
                
                // Set type on COM: TypeId setter may not persist (returns 00000000), so set TypeIdS explicitly so CreateObject sees the type
                rengaNewEntityArgs.TypeId = typeId;
                rengaNewEntityArgs.TypeIdS = typeId.ToString("B");
                debugInfo.AppendLine($"TypeId set directly on COM: {typeId}");
                debugInfo.AppendLine($"TypeIdS set: {typeId.ToString("B")}");
                debugInfo.AppendLine($"TypeId after setting: {rengaNewEntityArgs.TypeId}");
                debugInfo.AppendLine($"TypeIdS after setting: {rengaNewEntityArgs.TypeIdS}");
                
                // Set other properties if provided
                if (hostObjectId != -1)
                {
                    rengaNewEntityArgs.HostObjectId = hostObjectId;
                    debugInfo.AppendLine($"HostObjectId set: {hostObjectId}");
                }
                
                if (categoryId != -1)
                {
                    rengaNewEntityArgs.CategoryId = categoryId;
                    debugInfo.AppendLine($"CategoryId set: {categoryId}");
                }
                
                // Set 2D placement if provided (for drawing model objects: DrawingText, DrawingImage, etc.)
                if (placement2D != null)
                {
                    try
                    {
                        rengaNewEntityArgs.Placement2D = placement2D.ToRengaPlacement2D();
                        debugInfo.AppendLine("Placement2D set from wrapper successfully");
                    }
                    catch (Exception placementEx)
                    {
                        debugInfo.AppendLine($"⚠️ Warning: Could not set Placement2D: {placementEx.Message}");
                    }
                }
                else if (placement2DStruct.HasValue)
                {
                    try
                    {
                        rengaNewEntityArgs.Placement2D = placement2DStruct.Value;
                        debugInfo.AppendLine("Placement2D set from struct successfully");
                    }
                    catch (Exception placementEx)
                    {
                        debugInfo.AppendLine($"⚠️ Warning: Could not set Placement2D: {placementEx.Message}");
                    }
                }
                
                // Set 3D placement if provided (for building model objects)
                if (placement3D != null)
                {
                    try
                    {
                        // Use the struct if available, otherwise try the COM interface
                        if (!placement3D._placement.Equals(default(Renga.Placement3D)))
                        {
                            rengaNewEntityArgs.Placement3D = placement3D._placement;
                            debugInfo.AppendLine($"Placement3D set from struct successfully");
                        }
                        else if (placement3D._i != null)
                        {
                            rengaNewEntityArgs.Placement3D = placement3D._i.Placement;
                            debugInfo.AppendLine($"Placement3D set from COM interface successfully");
                        }
                        else
                        {
                            debugInfo.AppendLine($"⚠️ Warning: Placement3D wrapper has neither COM interface nor struct data");
                        }
                        
                        debugInfo.AppendLine($"Placement3D Origin: ({placement3D.Origin().X}, {placement3D.Origin().Y}, {placement3D.Origin().Z})");
                    }
                    catch (Exception placementEx)
                    {
                        debugInfo.AppendLine($"⚠️ Warning: Could not set Placement3D: {placementEx.Message}");
                    }
                }
                
                // Create within an operation (Renga API: StartOperation -> CreateObject -> Apply)
                debugInfo.AppendLine("🚀 Starting operation and creating object...");
                Renga.IOperation rengaOperation = null;
                try
                {
                    rengaOperation = this._i.CreateOperation();
                    if (rengaOperation != null)
                    {
                        rengaOperation.Start();
                        debugInfo.AppendLine("Operation started.");
                    }
                }
                catch (Exception opEx)
                {
                    debugInfo.AppendLine($"⚠️ Could not start operation: {opEx.Message}");
                }

                var rengaModelObject = this._i.CreateObject(rengaNewEntityArgs);
                debugInfo.AppendLine($"✅ Direct COM object creation successful: {rengaModelObject != null}");

                if (rengaOperation != null)
                {
                    try
                    {
                        rengaOperation.Apply();
                        debugInfo.AppendLine("Operation applied.");
                    }
                    catch (Exception applyEx)
                    {
                        debugInfo.AppendLine($"⚠️ Operation.Apply failed: {applyEx.Message}");
                    }
                }
                
                if (rengaModelObject == null)
                {
                    debugInfo.AppendLine("❌ CreateObject returned null (check TypeId/TypeIdS and that model is drawing model for DrawingText).");
                    return new Dictionary<string, object>
                    {
                        ["ModelObject"] = null,
                        ["DebugInfo"] = debugInfo.ToString(),
                        ["Success"] = false
                    };
                }
                
                // Wrap the result
                var modelObject = new IModelObject(rengaModelObject);
                
                return new Dictionary<string, object>
                {
                    ["ModelObject"] = modelObject,
                    ["DebugInfo"] = debugInfo.ToString(),
                    ["Success"] = true
                };
            }
            catch (Exception ex)
            {
                debugInfo.AppendLine($"❌ Direct COM object creation failed: {ex.Message}");
                debugInfo.AppendLine($"🔍 Error Type: {ex.GetType().Name}");
                debugInfo.AppendLine($"📋 Stack Trace: {ex.StackTrace}");
                
                return new Dictionary<string, object>
                {
                    ["ModelObject"] = null,
                    ["DebugInfo"] = debugInfo.ToString(),
                    ["Success"] = false
                };
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
        /// Creates a ModelText object in the building model (use with IModel from Application/Project).
        /// Text content is set after creation via ITextObject on the returned ModelObject.
        /// </summary>
        /// <param name="placement3D">3D placement of the text</param>
        /// <param name="levelId">Level ID (host object); use -1 for current level</param>
        /// <returns>Dictionary with ModelObject, DebugInfo, Success</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo", "Success" })]
        public Dictionary<string, object> CreateModelText(DynRenga.DynGeometry.Placement3D placement3D, int levelId = -1)
        {
            return CreateObject(Renga.EntityTypes.ModelText, levelId, -1, placement3D, null, null);
        }

        /// <summary>
        /// Creates a DrawingText object in the drawing model (use with IModel from Drawing.GetModel() or IModel(drawing)).
        /// Text content is set after creation via ITextObject on the returned ModelObject.
        /// </summary>
        /// <param name="placement2DStruct">2D placement from Placement2D.CreateStruct(originX, originY) in meters</param>
        /// <returns>Dictionary with ModelObject, DebugInfo, Success</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo", "Success" })]
        public Dictionary<string, object> CreateDrawingText(Renga.Placement2D placement2DStruct)
        {
            return CreateObject(Renga.EntityTypes.DrawingText, -1, -1, null, null, placement2DStruct);
        }

        /// <summary>
        /// Creates a DrawingText object in the drawing model using a Placement2D wrapper.
        /// </summary>
        /// <param name="placement2D">2D placement wrapper (e.g. from GetProfilePlacement)</param>
        /// <returns>Dictionary with ModelObject, DebugInfo, Success</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo", "Success" })]
        public Dictionary<string, object> CreateDrawingText(DynRenga.DynGeometry.Placement2D placement2D)
        {
            return CreateObject(Renga.EntityTypes.DrawingText, -1, -1, null, placement2D, null);
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
        /// Returns the drawing model (IModel) for a drawing from the project's Drawings2.
        /// Use for CreateDrawingText, GetObjects, etc. Appears in IModel category.
        /// </summary>
        /// <param name="project">IProject (e.g. from Application.Project)</param>
        /// <param name="drawingIndex">Zero-based index in Drawings2 (0 = first drawing)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static IModel GetDrawingModel(IProject project, int drawingIndex = 0)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            return project.GetDrawingModel(drawingIndex);
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
                "DrawingText",
                "Element",
                "Floor",
                "Hatch",
                "Hole",
                "IsolatedFoundation",
                "Level",
                "ModelText",
                "Opening",
                "Plate",
                "Railing",
                "Ramp",
                "Room",
                "Stairway",
                "Wall",
                "WallFoundation",
                "Window"
            };
        }

        /// <summary>
        /// Returns model object type GUIDs by name (for use with CreateObject).
        /// ModelText: building model. DrawingText: drawing model (use IModel from Drawing).
        /// </summary>
        /// <returns>Dictionary of type name to Guid</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelText", "DrawingText", "Wall", "Floor", "Level", "Opening", "Window", "Door" })]
        public static Dictionary<string, Guid> GetModelObjectTypeIds()
        {
            return new Dictionary<string, Guid>
            {
                ["ModelText"] = Renga.EntityTypes.ModelText,
                ["DrawingText"] = Renga.EntityTypes.DrawingText,
                ["Wall"] = Renga.EntityTypes.Wall,
                ["Floor"] = Renga.EntityTypes.Floor,
                ["Level"] = Renga.EntityTypes.Level,
                ["Opening"] = Renga.EntityTypes.Opening,
                ["Window"] = Renga.EntityTypes.Window,
                ["Door"] = Renga.EntityTypes.Door
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
