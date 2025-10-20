using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynProperties.Parameters;
using DynRenga.DynProperties.Properties;
using DynRenga.DynProperties.Quantities;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IModelObject interface - Complete API Reference
    /// This class provides comprehensive access to all IModelObject interface members
    /// </summary>
    public class IModelObject : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IModelObject
        /// </summary>
        public Renga.IModelObject _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IModelObject from existing IModelObject interface
        /// </summary>
        /// <param name="modelObject">Existing IModelObject interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject(Renga.IModelObject modelObject)
        {
            if (modelObject == null)
                throw new ArgumentNullException(nameof(modelObject), "ModelObject interface cannot be null.");
            
            this._i = modelObject;
        }

        /// <summary>
        /// Constructor - Creates IModelObject from existing ModelObject
        /// </summary>
        /// <param name="modelObject">Existing ModelObject instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelObject(DynObjects.ModelObject modelObject)
        {
            if (modelObject == null)
                throw new ArgumentNullException(nameof(modelObject), "ModelObject cannot be null.");
            
            if (modelObject._i == null)
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            this._i = modelObject._i;
        }

        /// <summary>
        /// Check if model object interface is valid
        /// </summary>
        /// <returns>True if model object interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IModelObject Properties

        /// <summary>
        /// Gets the identifier of the object instance (unique in current project)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                return this._i.Id;
            }
        }

        /// <summary>
        /// Gets the localized object name
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                return this._i.Name;
            }
        }

        /// <summary>
        /// Gets the type of the object as GUID
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid ObjectType
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                return this._i.ObjectType;
            }
        }

        /// <summary>
        /// Gets the type of the object as string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string ObjectTypeS
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                return this._i.ObjectTypeS;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the object instance (universal unique identifier)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid UniqueId
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                return this._i.UniqueId;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the object instance as string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string UniqueIdS
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                return this._i.UniqueIdS;
            }
        }

        /// <summary>
        /// Gets the host object ID (level ID for level-based objects)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int HostObjectId
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelObject interface is not initialized.");
                
                try
                {
                    // Try to get ILevelObject interface to access LevelId
                    var levelObject = this._i.GetInterfaceByName("ILevelObject");
                    if (levelObject != null)
                    {
                        var levelInterface = levelObject as Renga.ILevelObject;
                        if (levelInterface != null)
                        {
                            return levelInterface.LevelId;
                        }
                    }
                    
                    // If not a level-based object, return -1
                    return -1;
                }
                catch (Exception ex)
                {
                    // If there's an error accessing the level interface, return -1
                    System.Diagnostics.Debug.WriteLine($"Failed to get host object ID: {ex.Message}");
                    return -1;
                }
            }
        }

        #endregion

        /// <summary>
        /// Attempts to retrieve the 3D placement (local coordinate system) for this model object.
        /// Tries several strategies: ILevelObject.GetPlacement(), direct GetPlacement() on the COM object,
        /// or properties named Placement / Placement3D. Returns a DynRenga.DynGeometry.Placement3D wrapper.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.DynGeometry.Placement3D GetPlacement()
        {
            if (this._i == null)
                throw new InvalidOperationException("ModelObject interface is not initialized.");

            try
            {
                // If object supports ILevelObject, prefer that (it has GetPlacement())
                var levelObj = this._i.GetInterfaceByName("ILevelObject");
                if (levelObj != null)
                {
                    try
                    {
                        var lvl = levelObj as Renga.ILevelObject;
                        if (lvl != null)
                        {
                            var placement = lvl.GetPlacement();
                            return new DynRenga.DynGeometry.Placement3D(placement);
                        }
                    }
                    catch
                    {
                        // ignore and fallback
                    }
                }

                // Try direct method GetPlacement() on the COM object
                var comType = this._i.GetType();
                var getPlacementMethod = comType.GetMethod("GetPlacement");
                if (getPlacementMethod != null)
                {
                    var placementObj = getPlacementMethod.Invoke(this._i, null);
                    if (placementObj != null)
                    {
                        return new DynRenga.DynGeometry.Placement3D(placementObj);
                    }
                }

                // Try properties named Placement or Placement3D
                var prop = comType.GetProperty("Placement3D") ?? comType.GetProperty("Placement");
                if (prop != null)
                {
                    var placementVal = prop.GetValue(this._i);
                    if (placementVal != null)
                    {
                        // If it's the Renga.Placement3D struct
                        if (placementVal is Renga.Placement3D placementStruct)
                        {
                            return new DynRenga.DynGeometry.Placement3D(placementStruct);
                        }

                        // If it's a COM interface wrapper
                        return new DynRenga.DynGeometry.Placement3D(placementVal);
                    }
                }

                throw new InvalidOperationException("Placement not available for this model object.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get placement: {ex.Message}", ex);
            }
        }

        #region IModelObject Methods

        /// <summary>
        /// Returns an interface by its name (alternative to COM QueryInterface)
        /// Returns proper RengaAPI wrapper types for known interfaces
        /// </summary>
        /// <param name="interfaceName">The name of the requested interface</param>
        /// <returns>Requested interface as proper RengaAPI wrapper type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetInterfaceByName(string interfaceName)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            if (string.IsNullOrEmpty(interfaceName))
                throw new ArgumentException("Interface name cannot be null or empty", nameof(interfaceName));
            
            try
            {
                var comObject = this._i.GetInterfaceByName(interfaceName);
                
                if (comObject == null)
                    return null;
                
                // Return proper RengaAPI wrapper types for known interfaces
                switch (interfaceName)
                {
                    case "IBaseline2DObject":
                        return new IBaseline2DObject(comObject);
                    
                    case "ILevelObject":
                        return new ILevelObject(comObject);
                    
                    case "ILevel":
                        return new ILevel(comObject);
                    
                    case "IParameterContainer":
                        return new IParameterContainer(comObject as Renga.IParameterContainer);
                    
                    case "IParameter":
                        return new IParameter(comObject as Renga.IParameter);
                    
                    case "IParameterDefinition":
                        return new IParameterDefinition(comObject as Renga.IParameterDefinition);
                    
                    case "ILine3DParams":
                        return new ILine3DParams(comObject as Renga.ILine3DParams);
                    
                    case "ICurve3D":
                        return new ICurve3D(comObject as Renga.ICurve3D);
                    case "IRegion2D":
                        return new IRegion2D(comObject as Renga.IRegion2D);
                    case "IRegion2DCollection":
                        return new IRegion2DCollection(comObject as Renga.IRegion2DCollection);
                    case "IProfile":
                        return new IProfile(comObject as Renga.IProfile);
                    case "IProfileDescription":
                        return new IProfileDescription(comObject as Renga.IProfileDescription);
                    case "IProfileDescriptionManager":
                        return new IProfileDescriptionManager(comObject as Renga.IProfileDescriptionManager);
                    case "IColumnParams":
                        return new IColumnParams(comObject as Renga.IColumnParams);
                    case "IColumnStyle":
                        return new IColumnStyle(comObject as Renga.IColumnStyle);
                    case "IColumnStyleManager":
                        return new IColumnStyleManager(comObject as Renga.IColumnStyleManager);
                    
                    case "IObjectWithPorts":
                        return new IObjectWithPorts(comObject);
                    
                    case "IFloorParams":
                        return new IFloorParams(comObject);
                    
                    // Add more interface mappings as needed
                    // case "IObjectWithMaterial":
                    //     return new IObjectWithMaterial(comObject);
                    // case "IObjectWithLayeredMaterial":
                    //     return new IObjectWithLayeredMaterial(comObject);
                    // case "IObjectWithLink":
                    //     return new IObjectWithLink(comObject);
                    // case "ITextObject":
                    //     return new ITextObject(comObject);
                    
                    default:
                        // For unknown interfaces, return the raw COM object
                        return comObject;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get interface by name '{interfaceName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the parameter container interface for the object
        /// </summary>
        /// <returns>IParameterContainer instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterContainer GetParameters()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                var parameterContainer = this._i.GetParameters();
                return new IParameterContainer(parameterContainer);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameters: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the property container interface for the object
        /// </summary>
        /// <returns>PropertyContainer instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public PropertyContainer GetProperties()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                var propertyContainer = this._i.GetProperties();
                return new PropertyContainer(propertyContainer);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get properties: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the quantity container interface for the object
        /// </summary>
        /// <returns>QuantityContainer instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public QuantityContainer GetQuantities()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                var quantityContainer = this._i.GetQuantities();
                return new QuantityContainer(quantityContainer);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get quantities: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the IObjectWithPorts interface if the object supports ports
        /// </summary>
        /// <returns>IObjectWithPorts instance or null if not supported</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IObjectWithPorts GetObjectWithPorts()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                var objectWithPorts = this._i.GetInterfaceByName("IObjectWithPorts");
                if (objectWithPorts == null)
                    return null;
                
                return new IObjectWithPorts(objectWithPorts);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get IObjectWithPorts interface: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced ModelObject Methods

        /// <summary>
        /// Gets object information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with object information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetObjectInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Id", Id },
                    { "Name", Name },
                    { "ObjectType", ObjectType },
                    { "ObjectTypeS", ObjectTypeS },
                    { "UniqueId", UniqueId },
                    { "UniqueIdS", UniqueIdS },
                    { "HostObjectId", HostObjectId }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "Id", -1 },
                    { "Name", "Error" },
                    { "ObjectType", Guid.Empty },
                    { "ObjectTypeS", "Error" },
                    { "UniqueId", Guid.Empty },
                    { "UniqueIdS", "Error" },
                    { "HostObjectId", -1 }
                };
            }
        }

        /// <summary>
        /// Checks if the object is of a specific type
        /// </summary>
        /// <param name="objectType">Object type to check against</param>
        /// <returns>True if object is of the specified type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsOfType(Guid objectType)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            return ObjectType == objectType;
        }

        /// <summary>
        /// Checks if the object is of a specific type by string
        /// </summary>
        /// <param name="objectTypeString">Object type string to check against</param>
        /// <returns>True if object is of the specified type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsOfTypeS(string objectTypeString)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            if (string.IsNullOrEmpty(objectTypeString))
                return false;
            
            return ObjectTypeS.Equals(objectTypeString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets detailed host object information including level details
        /// </summary>
        /// <returns>Dictionary with host object information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetHostObjectInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                var hostObjectInfo = new Dictionary<string, object>
                {
                    { "HostObjectId", HostObjectId },
                    { "HasHostObject", HostObjectId != -1 },
                    { "IsLevelBased", false },
                    { "LevelId", -1 },
                    { "ElevationAboveLevel", 0.0 },
                    { "PlacementElevation", 0.0 },
                    { "VerticalOffset", 0.0 }
                };
                
                // Try to get detailed level information
                if (HostObjectId != -1)
                {
                    try
                    {
                        var levelObject = this._i.GetInterfaceByName("ILevelObject");
                        if (levelObject != null)
                        {
                            var levelInterface = levelObject as Renga.ILevelObject;
                            if (levelInterface != null)
                            {
                                hostObjectInfo["IsLevelBased"] = true;
                                hostObjectInfo["LevelId"] = levelInterface.LevelId;
                                hostObjectInfo["ElevationAboveLevel"] = levelInterface.ElevationAboveLevel;
                                hostObjectInfo["PlacementElevation"] = levelInterface.PlacementElevation;
                                hostObjectInfo["VerticalOffset"] = levelInterface.VerticalOffset;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        hostObjectInfo["LevelError"] = ex.Message;
                    }
                }
                
                return hostObjectInfo;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "HostObjectId", -1 },
                    { "HasHostObject", false },
                    { "IsLevelBased", false },
                    { "Error", ex.Message }
                };
            }
        }

        /// <summary>
        /// Gets a list of common object type names for reference
        /// </summary>
        /// <returns>List of common object type names</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<string> GetCommonObjectTypes()
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

        #region Layered Material Methods

        /// <summary>
        /// Sets layered material to this model object
        /// This is a convenience method that uses ILayeredMaterialManager internally
        /// </summary>
        /// <param name="layeredMaterialManager">Layered material manager instance</param>
        /// <param name="layeredMaterialId">Layered material ID to assign</param>
        /// <param name="setThickness">Whether to also set thickness from base layer (optional, default true)</param>
        /// <returns>True if successful, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool SetLayeredMaterial(ILayeredMaterialManager layeredMaterialManager, int layeredMaterialId, bool setThickness = true)
        {
            if (layeredMaterialManager == null)
                throw new ArgumentNullException(nameof(layeredMaterialManager), "LayeredMaterialManager cannot be null");
            
            return layeredMaterialManager.SetLayeredMaterialToObject(this, layeredMaterialId, setThickness);
        }

        /// <summary>
        /// Checks if this object supports layered materials
        /// </summary>
        /// <returns>True if the object supports layered materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool SupportsLayeredMaterials()
        {
            // GUID for layered material parameter (from Control plugin)
            var guidStyleLayeredMaterial = new Guid("2cbc4964-61a0-4553-8103-36e2dd2ab31c");
            
            try
            {
                var parameterContainer = GetParameters();
                return parameterContainer.Contains(guidStyleLayeredMaterial);
            }
            catch
            {
                return false;
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IModelObject COM object: {ex.Message}");
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
        ~IModelObject()
        {
            Dispose(false);
        }

        #endregion
    }
}
