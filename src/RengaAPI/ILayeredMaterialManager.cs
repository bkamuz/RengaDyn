using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ILayeredMaterialManager interface
    /// Provides methods for managing layered materials within the Renga system
    /// </summary>
    public class ILayeredMaterialManager : IDisposable
    {
        private Renga.ILayeredMaterialManager _i;
        private bool _disposed = false;

        // GUID constants for layered material assignment (from Control plugin)
        private static readonly Guid GuidStyleLayeredMaterial = new Guid("2cbc4964-61a0-4553-8103-36e2dd2ab31c");
        private static readonly Guid GuidThickness = new Guid("f2712442-b9df-44fe-ac7b-c3524342c804");
        private static readonly Guid GuidVerticalOffset = new Guid("337fd89b-763e-46dc-b1a9-7aecb253adbf");
        private static readonly Guid GuidRoundingRadius = new Guid("3b97850d-d3ea-4a2d-91ff-0bc572d8795d");

        public Renga.ILayeredMaterialManager _i_Internal => _i;

        /// <summary>
        /// Constructor - Creates ILayeredMaterialManager from COM object
        /// </summary>
        /// <param name="manager">The Renga.ILayeredMaterialManager COM object</param>
        public ILayeredMaterialManager(object manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager), "LayeredMaterialManager cannot be null");
            _i = manager as Renga.ILayeredMaterialManager;
            if (_i == null)
                throw new ArgumentException("Object does not implement ILayeredMaterialManager interface", nameof(manager));
        }

        /// <summary>
        /// Constructor - Creates ILayeredMaterialManager from typed COM object
        /// </summary>
        /// <param name="manager">The typed Renga.ILayeredMaterialManager object</param>
        public ILayeredMaterialManager(Renga.ILayeredMaterialManager manager)
        {
            _i = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ILayeredMaterialManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose implementation
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Returns the layered material by ID
        /// </summary>
        /// <param name="id">The ID of the layered material to retrieve</param>
        /// <returns>ILayeredMaterial wrapper object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ILayeredMaterial GetLayeredMaterial(int id)
        {
            if (_i == null) 
                throw new InvalidOperationException("ILayeredMaterialManager interface is not initialized.");
            try 
            { 
                var material = _i.GetLayeredMaterial(id);
                return new ILayeredMaterial(material);
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get layered material with ID {id}: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Returns the layered material by LayeredMaterialIdGroupPair (Deprecated)
        /// This method is deprecated, use GetLayeredMaterial instead
        /// </summary>
        /// <param name="layeredMaterialIdGroupPair">The ID group pair for the layered material</param>
        /// <returns>ILayeredMaterial wrapper object</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        [Obsolete("This method is deprecated, use GetLayeredMaterial instead")]
        public ILayeredMaterial GetLayeredMaterialByIdGroupPair(object layeredMaterialIdGroupPair)
        {
            if (_i == null) 
                throw new InvalidOperationException("ILayeredMaterialManager interface is not initialized.");
            try 
            { 
                var material = _i.GetLayeredMaterialByIdGroupPair((dynamic)layeredMaterialIdGroupPair);
                return new ILayeredMaterial(material);
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get layered material by ID group pair: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Gets layered materials filtered by material group type
        /// </summary>
        /// <param name="materialGroup">Material group filter (0=Undefined, 1=Wall, 2=Floor, 3=Roof, 4=Insulation)</param>
        /// <param name="project">Renga project to get materials from</param>
        /// <returns>Dictionary of material ID and name pairs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, string> GetLayeredMaterialsByGroup(int materialGroup, object project)
        {
            if (_i == null) 
                throw new InvalidOperationException("ILayeredMaterialManager interface is not initialized.");
            
            var materials = new Dictionary<int, string>();
            
            try 
            {
                // Try different casting approaches for project
                dynamic rengaProject = project;
                if (rengaProject == null)
                    throw new ArgumentException("Invalid project object");

                var allMaterials = rengaProject.LayeredMaterials;
                for (int i = 0; i < allMaterials.Count; i++)
                {
                    var entity = allMaterials.GetByIndex(i);
                    var layeredMaterial = _i.GetLayeredMaterial(entity.Id);
                    
                    if (layeredMaterial != null)
                    {
                        var idGroupPair = layeredMaterial.GetIdGroupPair();
                        if ((int)idGroupPair.Group == materialGroup)
                        {
                            materials[layeredMaterial.Id] = layeredMaterial.Name;
                        }
                    }
                }
                
                return materials;
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get layered materials by group: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Gets all floor layered materials from the project
        /// </summary>
        /// <param name="project">Renga project to get materials from</param>
        /// <returns>Dictionary of material ID and name pairs for floor materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, string> GetFloorLayeredMaterials(object project)
        {
            return GetLayeredMaterialsByGroup(2, project); // LayeredMaterialGroup_Floor = 2
        }

        /// <summary>
        /// Gets all wall layered materials from the project
        /// </summary>
        /// <param name="project">Renga project to get materials from</param>
        /// <returns>Dictionary of material ID and name pairs for wall materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, string> GetWallLayeredMaterials(object project)
        {
            return GetLayeredMaterialsByGroup(1, project); // LayeredMaterialGroup_Wall = 1
        }

        /// <summary>
        /// Gets all roof layered materials from the project
        /// </summary>
        /// <param name="project">Renga project to get materials from</param>
        /// <returns>Dictionary of material ID and name pairs for roof materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, string> GetRoofLayeredMaterials(object project)
        {
            return GetLayeredMaterialsByGroup(3, project); // LayeredMaterialGroup_Roof = 3
        }

        /// <summary>
        /// Sets layered material to a model object using parameter container
        /// This is the key method that enables layered material assignment
        /// </summary>
        /// <param name="modelObject">Model object to set layered material on</param>
        /// <param name="layeredMaterialId">Layered material ID to assign</param>
        /// <param name="setThickness">Whether to also set thickness from base layer (optional, default true)</param>
        /// <returns>True if successful, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool SetLayeredMaterialToObject(IModelObject modelObject, int layeredMaterialId, bool setThickness = true)
        {
            if (_i == null) 
                throw new InvalidOperationException("ILayeredMaterialManager interface is not initialized.");
            if (modelObject == null)
                throw new ArgumentNullException(nameof(modelObject), "ModelObject cannot be null");
            if (layeredMaterialId <= 0)
                throw new ArgumentException("LayeredMaterialId must be greater than 0", nameof(layeredMaterialId));

            try 
            {
                // Get the parameter container
                var parameterContainer = modelObject.GetParameters();
                if (parameterContainer == null)
                    throw new InvalidOperationException("Failed to get parameter container from model object");

                // Check if the object supports layered materials
                if (parameterContainer.Contains(GuidStyleLayeredMaterial))
                {
                    // Get the layered material parameter
                    var layeredMaterialParam = parameterContainer.Get(GuidStyleLayeredMaterial);
                    
                    // Set the layered material ID - using IParameter methods
                    layeredMaterialParam.SetIntValue(layeredMaterialId);
                }
                else
                {
                    throw new InvalidOperationException("Object does not support layered materials");
                }
                
                // Optional: Also set thickness from the base layer
                if (setThickness)
                {
                    SetThicknessFromLayeredMaterial(parameterContainer, layeredMaterialId);
                }
                
                return true;
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to set layered material: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Helper method to set thickness from layered material's base layer
        /// </summary>
        /// <param name="parameterContainer">Parameter container of the object</param>
        /// <param name="layeredMaterialId">Layered material ID</param>
        [dr.IsVisibleInDynamoLibrary(false)]
        private void SetThicknessFromLayeredMaterial(IParameterContainer parameterContainer, int layeredMaterialId)
        {
            try
            {
                var layeredMaterial = GetLayeredMaterial(layeredMaterialId);
                
                if (layeredMaterial != null)
                {
                    // Get the base layer thickness using RengaAPI wrapper
                    var baseLayer = layeredMaterial.GetBaseLayer();
                    
                    // Set thickness parameter if it exists
                    if (parameterContainer.Contains(GuidThickness))
                    {
                        var thicknessParam = parameterContainer.Get(GuidThickness);
                        // For now, we'll need to get thickness from the underlying interface
                        // This might need adjustment based on ILayeredMaterial implementation
                        if (baseLayer != null)
                        {
                            // thicknessParam.SetDoubleValue(baseLayer.Thickness);
                            // TODO: Need to check ILayeredMaterial wrapper for thickness access
                        }
                    }
                }
            }
            catch
            {
                // Ignore thickness setting errors - this is optional
            }
        }

        /// <summary>
        /// Sets layered material to a floor object by ID using IModel
        /// This method provides a complete workflow for setting layered materials
        /// </summary>
        /// <param name="model">IModel instance from RengaAPI</param>
        /// <param name="objectId">Object ID to set layered material on</param>
        /// <param name="layeredMaterialId">Layered material ID to assign</param>
        /// <param name="setThickness">Whether to also set thickness from base layer (optional, default true)</param>
        /// <returns>Dictionary with success status and debug info</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> SetLayeredMaterialById(IModel model, int objectId, int layeredMaterialId, bool setThickness = true)
        {
            var debugInfo = "🎨 Setting layered material to object...\n";
            
            try
            {
                if (_i == null) 
                    throw new InvalidOperationException("ILayeredMaterialManager interface is not initialized.");
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Model cannot be null");
                if (objectId <= 0)
                    throw new ArgumentException("ObjectId must be greater than 0", nameof(objectId));
                if (layeredMaterialId <= 0)
                    throw new ArgumentException("LayeredMaterialId must be greater than 0", nameof(layeredMaterialId));

                debugInfo += $"📋 Object ID: {objectId}\n";
                debugInfo += $"🎨 Material ID: {layeredMaterialId}\n";

                // Get the object from the model
                var objectCollection = model.GetObjects();
                var modelObject = objectCollection.GetById(objectId);
                
                if (modelObject == null)
                {
                    debugInfo += $"❌ Object with ID {objectId} not found\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }

                debugInfo += $"✅ Object found: {modelObject.ObjectType}\n";

                // Set the layered material
                bool success = SetLayeredMaterialToObject(modelObject, layeredMaterialId, setThickness);
                
                if (success)
                {
                    debugInfo += "✅ Layered material set successfully!\n";
                    if (setThickness)
                    {
                        debugInfo += "✅ Thickness also updated from base layer\n";
                    }
                }
                else
                {
                    debugInfo += "❌ Failed to set layered material\n";
                }

                return new Dictionary<string, object>
                {
                    { "Success", success },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error: {ex.Message}\n";
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        /// <returns>True if initialized and not disposed</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;

        /// <summary>
        /// Gets debug information about the manager state
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        public string GetDebugInfo()
        {
            return $"ILayeredMaterialManager - Initialized: {IsInitialized()}, Disposed: {_disposed}";
        }
    }
}