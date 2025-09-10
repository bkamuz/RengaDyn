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

        #endregion

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
        /// <returns>ParameterContainer instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ParameterContainer GetParameters()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            try
            {
                var parameterContainer = this._i.GetParameters();
                return new ParameterContainer(parameterContainer);
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
                    { "UniqueIdS", UniqueIdS }
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
                    { "UniqueIdS", "Error" }
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
