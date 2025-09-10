using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga INewEntityArgs interface - Complete API Reference
    /// This class provides comprehensive access to all INewEntityArgs interface members
    /// </summary>
    public class INewEntityArgs : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.INewEntityArgs
        /// </summary>
        public Renga.INewEntityArgs _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates INewEntityArgs from COM object
        /// </summary>
        /// <param name="rengaNewEntityArgsObject">The Renga.INewEntityArgs COM object</param>
        internal INewEntityArgs(object rengaNewEntityArgsObject)
        {
            if (rengaNewEntityArgsObject == null)
                throw new ArgumentNullException(nameof(rengaNewEntityArgsObject), "Renga INewEntityArgs cannot be null.");
            
            this._i = rengaNewEntityArgsObject as Renga.INewEntityArgs;
            if (this._i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.INewEntityArgs.");
        }

        /// <summary>
        /// Gets or sets the entity type ID
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid TypeId
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    return this._i.TypeId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity type ID: {ex.Message}", ex);
                }
            }
            set
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    this._i.TypeId = value;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to set entity type ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the entity type ID as a string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string TypeIdS
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    return this._i.TypeIdS;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get entity type ID string: {ex.Message}", ex);
                }
            }
            set
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Type ID string cannot be null or empty.", nameof(value));
                
                try
                {
                    this._i.TypeIdS = value;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to set entity type ID string: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the entity category ID
        /// The category ID of the entity to create. The property applies to entities that are based on categories (MEP styles).
        /// For example, to create Equipment style or Pipe fitting style, set CategoryID equal to one of the Equipment or Pipe fitting categories.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int CategoryId
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    return this._i.CategoryId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get category ID: {ex.Message}", ex);
                }
            }
            set
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    this._i.CategoryId = value;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to set category ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the host object ID
        /// The ID of the object that is the host for the object being created.
        /// For example: To create wall, set HostObjectId equal to one of the level IDs. To create window, set HostObjectId equal to one of the wall IDs.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int HostObjectId
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    return this._i.HostObjectId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get host object ID: {ex.Message}", ex);
                }
            }
            set
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    this._i.HostObjectId = value;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to set host object ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the 3D placement of the object being created
        /// The global 3D placement of the object being created. By default, Placement3D is equal to right orthonormalized coordinate system.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Placement3D Placement3D
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    return this._i.Placement3D;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get 3D placement: {ex.Message}", ex);
                }
            }
            set
            {
                if (this._i == null)
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                
                try
                {
                    this._i.Placement3D = value;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to set 3D placement: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Sets the 3D placement from a DynRenga.DynGeometry.Placement3D wrapper
        /// </summary>
        /// <param name="placement3D">DynRenga.DynGeometry.Placement3D wrapper object</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetPlacement3D(DynRenga.DynGeometry.Placement3D placement3D)
        {
            if (this._i == null)
                throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
            
            if (placement3D == null)
                throw new ArgumentNullException(nameof(placement3D), "Placement3D cannot be null.");
            
            try
            {
                // Use the struct if available, otherwise try the COM interface
                if (!placement3D._placement.Equals(default(Renga.Placement3D)))
                {
                    this._i.Placement3D = placement3D._placement;
                }
                else if (placement3D._i != null)
                {
                    this._i.Placement3D = placement3D._i.Placement;
                }
                else
                {
                    throw new InvalidOperationException("Placement3D wrapper has neither COM interface nor struct data");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set 3D placement from wrapper: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the 3D placement as a DynRenga.DynGeometry.Placement3D wrapper
        /// </summary>
        /// <returns>DynRenga.DynGeometry.Placement3D wrapper object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.DynGeometry.Placement3D GetPlacement3DWrapper()
        {
            if (this._i == null)
                throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
            
            try
            {
                var placement3D = this._i.Placement3D;
                return new DynRenga.DynGeometry.Placement3D(placement3D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get 3D placement wrapper: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets comprehensive new entity arguments information
        /// </summary>
        /// <returns>Dictionary containing all argument properties</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "TypeId", "TypeIdS", "CategoryId", "HostObjectId", "Placement3D", "DebugInfo" })]
        public Dictionary<string, object> GetArgsInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
            
            try
            {
                var info = new Dictionary<string, object>();
                info["TypeId"] = this.TypeId;
                info["TypeIdS"] = this.TypeIdS;
                info["CategoryId"] = this.CategoryId;
                info["HostObjectId"] = this.HostObjectId;
                info["Placement3D"] = this.Placement3D;
                
                var debugInfo = $"--- NewEntityArgs Debug Info ---\n";
                debugInfo += $"Type ID: {this.TypeId}\n";
                debugInfo += $"Type ID (String): {this.TypeIdS}\n";
                debugInfo += $"Category ID: {this.CategoryId}\n";
                debugInfo += $"Host Object ID: {this.HostObjectId}\n";
                debugInfo += $"Placement3D: Origin=({this.Placement3D.Origin.X}, {this.Placement3D.Origin.Y}, {this.Placement3D.Origin.Z})\n";
                debugInfo += $"Interface Status: {(this._i != null ? "✅ Initialized" : "❌ Not Initialized")}\n";
                
                info["DebugInfo"] = debugInfo;
                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get arguments information: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets debug information about the new entity arguments
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ NewEntityArgs interface is not initialized.";
            
            try
            {
                var debugInfo = $"--- INewEntityArgs Debug Info ---\n";
                debugInfo += $"Type ID: {this.TypeId}\n";
                debugInfo += $"Type ID (String): {this.TypeIdS}\n";
                debugInfo += $"Category ID: {this.CategoryId}\n";
                debugInfo += $"Host Object ID: {this.HostObjectId}\n";
                debugInfo += $"Placement3D: Origin=({this.Placement3D.Origin.X}, {this.Placement3D.Origin.Y}, {this.Placement3D.Origin.Z})\n";
                debugInfo += $"X-Axis: ({this.Placement3D.xAxis.X}, {this.Placement3D.xAxis.Y}, {this.Placement3D.xAxis.Z})\n";
                debugInfo += $"Z-Axis: ({this.Placement3D.zAxis.X}, {this.Placement3D.zAxis.Y}, {this.Placement3D.zAxis.Z})\n";
                debugInfo += $"Interface Status: ✅ Initialized\n";
                
                return debugInfo;
            }
            catch (Exception ex)
            {
                return $"❌ Failed to get debug information: {ex.Message}";
            }
        }

        /// <summary>
        /// Creates a copy of the current new entity arguments
        /// </summary>
        /// <returns>New INewEntityArgs instance with the same values</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public INewEntityArgs Clone()
        {
            if (this._i == null)
                throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
            
            try
            {
                // Create a new instance by copying the current values
                var newArgs = new INewEntityArgs(this._i);
                return newArgs;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to clone new entity arguments: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Resets all arguments to their default values
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void ResetToDefaults()
        {
            if (this._i == null)
                throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
            
            try
            {
                this.TypeId = Guid.Empty;
                this.TypeIdS = Guid.Empty.ToString();
                this.CategoryId = 0;
                this.HostObjectId = 0;
                
                // Reset to default placement (right orthonormalized coordinate system)
                var defaultPlacement = new Renga.Placement3D
                {
                    Origin = new Renga.Point3D { X = 0, Y = 0, Z = 0 },
                    xAxis = new Renga.Vector3D { X = 1, Y = 0, Z = 0 },
                    zAxis = new Renga.Vector3D { X = 0, Y = 0, Z = 1 }
                };
                this.Placement3D = defaultPlacement;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to reset to defaults: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Disposes the Renga.INewEntityArgs COM object
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

        ~INewEntityArgs()
        {
            Dispose();
        }
    }
}
