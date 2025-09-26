using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IEntity interface - Complete API Reference
    /// This class provides comprehensive access to all IEntity interface members
    /// </summary>
    public class IEntity : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IEntity
        /// </summary>
        public Renga.IEntity _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IEntity from COM object
        /// </summary>
        /// <param name="rengaEntityObject">The Renga.IEntity COM object</param>
        internal IEntity(object rengaEntityObject)
        {
            if (rengaEntityObject == null)
                throw new ArgumentNullException(nameof(rengaEntityObject), "Renga IEntity cannot be null.");
            
            this._i = rengaEntityObject as Renga.IEntity;
            if (this._i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IEntity.");
        }

        /// <summary>
        /// Gets the unique identifier of the entity
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Entity interface is not initialized.");
                
                try
                {
                    return this._i.Id;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the name of the entity
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Entity interface is not initialized.");
                
                try
                {
                    return this._i.Name;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity name: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the type identifier of the entity
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid TypeId
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Entity interface is not initialized.");
                
                try
                {
                    return this._i.TypeId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity type ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the type identifier of the entity as a string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string TypeIdS
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Entity interface is not initialized.");
                
                try
                {
                    return this._i.TypeIdS;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity type ID string: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the unique identifier of the entity
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid UniqueId
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Entity interface is not initialized.");
                
                try
                {
                    return this._i.UniqueId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity unique ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the unique identifier of the entity as a string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string UniqueIdS
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Entity interface is not initialized.");
                
                try
                {
                    return this._i.UniqueIdS;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity unique ID string: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Returns an interface by its name
        /// </summary>
        /// <param name="interfaceName">The name of the requested interface</param>
        /// <returns>Requested interface as object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetInterfaceByName(string interfaceName)
        {
            if (this._i == null)
                throw new InvalidOperationException("Entity interface is not initialized.");
            
            if (string.IsNullOrEmpty(interfaceName))
                throw new ArgumentException("Interface name cannot be null or empty.", nameof(interfaceName));
            
            try
            {
                return this._i.GetInterfaceByName(interfaceName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get interface by name '{interfaceName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns an interface by its name with type safety
        /// </summary>
        /// <param name="interfaceName">The name of the requested interface</param>
        /// <returns>Requested interface as specific RengaAPI wrapper type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetInterfaceByNameTyped(string interfaceName)
        {
            if (this._i == null)
                throw new InvalidOperationException("Entity interface is not initialized.");
            
            if (string.IsNullOrEmpty(interfaceName))
                throw new ArgumentException("Interface name cannot be null or empty.", nameof(interfaceName));
            
            try
            {
                var comInterface = this._i.GetInterfaceByName(interfaceName);
                if (comInterface == null)
                    return null;

                // Return specific wrapper types for known interfaces
                switch (interfaceName)
                {
                    case "IPropertyContainer":
                        return new DynRenga.DynProperties.Properties.PropertyContainer(comInterface);
                    case "IParameterContainer":
                        return new DynRenga.DynProperties.Parameters.ParameterContainer(comInterface);
                    case "IModel":
                        return new IModel(new IApplication(comInterface));
                    case "IModelObject":
                        return new IModelObject(comInterface);
                    case "IModelObjectCollection":
                        return new IModelObjectCollection(comInterface);
                    case "IModelView":
                        return new IModelView(comInterface);
                    case "IBaseline2DObject":
                        return new IBaseline2DObject(comInterface);
                    case "ICurve2D":
                        return new ICurve2D(comInterface);
                    case "IRegion2D":
                        return new IRegion2D(comInterface);
                    case "IRegion2DCollection":
                        return new IRegion2DCollection(comInterface);
                    case "IMath":
                        return new IMath(comInterface);
                    case "IOperation":
                        return new IOperation(comInterface);
                    case "IProfile":
                        return new IProfile(comInterface);
                    case "IProfileDescription":
                        return new IProfileDescription(comInterface);
                    case "IProfileDescriptionManager":
                        return new IProfileDescriptionManager(comInterface);
                    case "IColumnParams":
                        return new IColumnParams(comInterface);
                    case "IColumnStyle":
                        return new IColumnStyle(comInterface);
                    case "IColumnStyleManager":
                        return new IColumnStyleManager(comInterface);
                    default:
                        // For unknown interfaces, return the raw COM object
                        return comInterface;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get typed interface by name '{interfaceName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets comprehensive entity information
        /// </summary>
        /// <returns>Dictionary containing entity properties</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Id", "Name", "TypeId", "TypeIdS", "UniqueId", "UniqueIdS", "DebugInfo" })]
        public Dictionary<string, object> GetEntityInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("Entity interface is not initialized.");
            
            try
            {
                var info = new Dictionary<string, object>();
                info["Id"] = this.Id;
                info["Name"] = this.Name;
                info["TypeId"] = this.TypeId;
                info["TypeIdS"] = this.TypeIdS;
                info["UniqueId"] = this.UniqueId;
                info["UniqueIdS"] = this.UniqueIdS;
                
                var debugInfo = $"--- Entity Debug Info ---\n";
                debugInfo += $"ID: {this.Id}\n";
                debugInfo += $"Name: {this.Name}\n";
                debugInfo += $"Type ID: {this.TypeId}\n";
                debugInfo += $"Type ID (String): {this.TypeIdS}\n";
                debugInfo += $"Unique ID: {this.UniqueId}\n";
                debugInfo += $"Unique ID (String): {this.UniqueIdS}\n";
                debugInfo += $"Interface Status: {(this._i != null ? "✅ Initialized" : "❌ Not Initialized")}\n";
                
                info["DebugInfo"] = debugInfo;
                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get entity information: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets entity information with debug details
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ Entity interface is not initialized.";
            
            try
            {
                var debugInfo = $"--- IEntity Debug Info ---\n";
                debugInfo += $"ID: {this.Id}\n";
                debugInfo += $"Name: {this.Name}\n";
                debugInfo += $"Type ID: {this.TypeId}\n";
                debugInfo += $"Type ID (String): {this.TypeIdS}\n";
                debugInfo += $"Unique ID: {this.UniqueId}\n";
                debugInfo += $"Unique ID (String): {this.UniqueIdS}\n";
                debugInfo += $"Interface Status: ✅ Initialized\n";
                
                // Test GetInterfaceByName for common interfaces
                var commonInterfaces = new[] { "IPropertyContainer", "IParameterContainer", "IModel", "IModelObject" };
                debugInfo += "\n--- Available Interfaces ---\n";
                
                foreach (var interfaceName in commonInterfaces)
                {
                    try
                    {
                        var interfaceObj = this._i.GetInterfaceByName(interfaceName);
                        debugInfo += $"✅ {interfaceName}: {(interfaceObj != null ? "Available" : "Not Available")}\n";
                    }
                    catch
                    {
                        debugInfo += $"❌ {interfaceName}: Not Available\n";
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
        /// Disposes the Renga.IEntity COM object
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

        ~IEntity()
        {
            Dispose();
        }
    }
}
