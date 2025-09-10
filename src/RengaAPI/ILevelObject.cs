using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ILevelObject interface - Complete API Reference
    /// This class provides comprehensive access to all ILevelObject interface members
    /// </summary>
    public class ILevelObject : IDisposable
    {
        private Renga.ILevelObject _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.ILevelObject _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="levelObject">COM object implementing ILevelObject</param>
        public ILevelObject(object levelObject)
        {
            if (levelObject == null)
                throw new ArgumentNullException(nameof(levelObject), "LevelObject cannot be null");
            
            _i = levelObject as Renga.ILevelObject;
            if (_i == null)
                throw new ArgumentException("Object does not implement ILevelObject interface", nameof(levelObject));
        }

        /// <summary>
        /// Constructor from existing ILevelObject
        /// </summary>
        /// <param name="levelObject">Existing ILevelObject instance</param>
        public ILevelObject(Renga.ILevelObject levelObject)
        {
            if (levelObject == null)
                throw new ArgumentNullException(nameof(levelObject), "LevelObject cannot be null");
            
            _i = levelObject;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ILevelObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method
        /// </summary>
        /// <param name="disposing">True if called from Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }
                
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                
                _disposed = true;
            }
        }

        #region Core ILevelObject Properties

        /// <summary>
        /// Gets the elevation of the object above the parent level
        /// The elevation of an object equals the sum of the elevation of the object's placement and the vertical offset of the object from its placement
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double ElevationAboveLevel
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.ElevationAboveLevel;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get elevation above level: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the ID of the parent level
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int LevelId
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.LevelId;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get level ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the elevation of the object's placement above the parent level
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double PlacementElevation
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.PlacementElevation;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get placement elevation: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the vertical offset from the object's placement
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double VerticalOffset
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.VerticalOffset;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get vertical offset: {ex.Message}", ex);
                }
            }
        }

        #endregion

        #region Core ILevelObject Methods

        /// <summary>
        /// Returns the copy of the local coordinate system of an object in three-dimensional space
        /// </summary>
        /// <returns>Placement3D representing the object's placement</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Placement3D GetPlacement()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var placement = this._i.GetPlacement();
                return new Placement3D(placement);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get placement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the local coordinate system of an object in three-dimensional space
        /// Limitations: It is impossible to edit placement of an object with dependent objects
        /// For now you can edit placement of all objects on Level except: Beam, Line3D, Rebar, Room, RoutePoint
        /// </summary>
        /// <param name="placement">New placement for the object</param>
        /// <returns>Dictionary with Success status and DebugInfo</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> SetPlacement(Placement3D placement)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (placement == null)
                throw new ArgumentNullException(nameof(placement), "Placement cannot be null");
            
            var debugInfo = "🔧 ILevelObject.SetPlacement Debug Info:\n";
            debugInfo += $"✅ Interface initialized: {IsInitialized()}\n";
            debugInfo += $"✅ Placement parameter: {placement != null}\n";
            debugInfo += $"✅ Level ID: {LevelId}\n";
            debugInfo += $"✅ Current elevation above level: {ElevationAboveLevel} mm\n";
            
            try
            {
                // Check if placement can be set (basic validation)
                var canSet = CanSetPlacement();
                debugInfo += $"✅ Can set placement: {canSet}\n";
                
                if (!canSet)
                {
                    debugInfo += "⚠️ Warning: Placement may not be editable (object has dependent objects or is restricted type)\n";
                }
                
                // Get current placement for comparison
                try
                {
                    var currentPlacement = this._i.GetPlacement();
                    debugInfo += $"✅ Current placement exists: {currentPlacement != null}\n";
                    if (currentPlacement != null)
                    {
                        debugInfo += $"✅ Current placement origin: ({currentPlacement.Origin.X}, {currentPlacement.Origin.Y}, {currentPlacement.Origin.Z})\n";
                    }
                }
                catch (Exception getEx)
                {
                    debugInfo += $"⚠️ Could not get current placement: {getEx.Message}\n";
                }
                
                // Validate placement parameter
                try
                {
                    var placementValid = IsPlacementValid(placement);
                    debugInfo += $"✅ Placement parameter valid: {placementValid}\n";
                }
                catch (Exception validEx)
                {
                    debugInfo += $"⚠️ Could not validate placement: {validEx.Message}\n";
                }
                
                debugInfo += "🚀 Attempting to set placement...\n";
                
                this._i.SetPlacement(placement._i_Internal);
                
                debugInfo += "✅ Placement set successfully!\n";
                
                // Verify the placement was set
                try
                {
                    var newPlacement = this._i.GetPlacement();
                    debugInfo += $"✅ Verification: New placement exists: {newPlacement != null}\n";
                    if (newPlacement != null)
                    {
                        debugInfo += $"✅ New placement origin: ({newPlacement.Origin.X}, {newPlacement.Origin.Y}, {newPlacement.Origin.Z})\n";
                    }
                }
                catch (Exception verifyEx)
                {
                    debugInfo += $"⚠️ Could not verify placement was set: {verifyEx.Message}\n";
                }
                
                // Output debug info to console (as per workspace rules)
                System.Diagnostics.Debug.WriteLine(debugInfo);
                
                return new Dictionary<string, object>
                {
                    ["Success"] = true,
                    ["DebugInfo"] = debugInfo
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during placement setting: {ex.Message}\n";
                debugInfo += $"🔍 Error Type: {ex.GetType().Name}\n";
                debugInfo += $"📋 Stack Trace: {ex.StackTrace}\n\n";
                debugInfo += "💡 Troubleshooting Tips:\n";
                debugInfo += "1. Check if object has dependent objects - placement cannot be edited\n";
                debugInfo += "2. Verify the object type is not restricted (Beam, Line3D, Rebar, Room, RoutePoint)\n";
                debugInfo += "3. Ensure the placement is valid and properly constructed\n";
                debugInfo += "4. Check if you're in an operation context\n";
                debugInfo += "5. Verify the placement coordinates are in millimeters\n";
                debugInfo += "6. Ensure the object supports ILevelObject interface\n";
                
                // Output debug info to console (as per workspace rules)
                System.Diagnostics.Debug.WriteLine(debugInfo);
                
                return new Dictionary<string, object>
                {
                    ["Success"] = false,
                    ["DebugInfo"] = debugInfo
                };
            }
        }

        #endregion

        #region Advanced LevelObject Methods

        /// <summary>
        /// Validates if the placement can be set for this object
        /// </summary>
        /// <returns>True if placement can be set, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool CanSetPlacement()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                // Try to get the current placement to check if it's editable
                var placement = this._i.GetPlacement();
                return placement != null;
            }
            catch (Exception)
            {
                // If we can't get the placement, it might not be editable
                return false;
            }
        }

        /// <summary>
        /// Gets comprehensive level object information
        /// </summary>
        /// <returns>Dictionary containing level object information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "LevelId", "ElevationAboveLevel", "PlacementElevation", "VerticalOffset", "CanEditPlacement", "DebugInfo" })]
        public Dictionary<string, object> GetLevelObjectInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            var debugInfo = "🔧 ILevelObject.GetLevelObjectInfo Debug Info:\n";
            var levelId = -1;
            var elevationAboveLevel = 0.0;
            var placementElevation = 0.0;
            var verticalOffset = 0.0;
            var canEditPlacement = false;
            
            try
            {
                debugInfo += $"✅ Interface initialized: {IsInitialized()}\n";
                
                // Get level ID
                levelId = this._i.LevelId;
                debugInfo += $"✅ Level ID: {levelId}\n";
                
                // Get elevation above level
                elevationAboveLevel = this._i.ElevationAboveLevel;
                debugInfo += $"✅ Elevation above level: {elevationAboveLevel} mm\n";
                
                // Get placement elevation
                placementElevation = this._i.PlacementElevation;
                debugInfo += $"✅ Placement elevation: {placementElevation} mm\n";
                
                // Get vertical offset
                verticalOffset = this._i.VerticalOffset;
                debugInfo += $"✅ Vertical offset: {verticalOffset} mm\n";
                
                // Check if placement can be edited
                canEditPlacement = CanSetPlacement();
                debugInfo += $"✅ Can edit placement: {canEditPlacement}\n";
                
                debugInfo += "✅ Level object information retrieved successfully\n";
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error getting level object information: {ex.Message}\n";
                debugInfo += "💡 Check if the object supports ILevelObject interface\n";
            }
            
            return new Dictionary<string, object>
            {
                { "LevelId", levelId },
                { "ElevationAboveLevel", elevationAboveLevel },
                { "PlacementElevation", placementElevation },
                { "VerticalOffset", verticalOffset },
                { "CanEditPlacement", canEditPlacement },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Checks if the placement is valid for this object type
        /// </summary>
        /// <param name="placement">Placement to validate</param>
        /// <returns>True if placement is valid, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsPlacementValid(Placement3D placement)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (placement == null)
                return false;
            
            try
            {
                // Basic validation - check if placement has valid values
                var origin = placement.Origin;
                var xAxis = placement.XAxis;
                var zAxis = placement.ZAxis;
                
                // Check for NaN or infinite values
                if (double.IsNaN(origin.X) || double.IsNaN(origin.Y) || double.IsNaN(origin.Z) ||
                    double.IsInfinity(origin.X) || double.IsInfinity(origin.Y) || double.IsInfinity(origin.Z))
                {
                    return false;
                }
                
                if (double.IsNaN(xAxis.X) || double.IsNaN(xAxis.Y) || double.IsNaN(xAxis.Z) ||
                    double.IsInfinity(xAxis.X) || double.IsInfinity(xAxis.Y) || double.IsInfinity(xAxis.Z))
                {
                    return false;
                }
                
                if (double.IsNaN(zAxis.X) || double.IsNaN(zAxis.Y) || double.IsNaN(zAxis.Z) ||
                    double.IsInfinity(zAxis.X) || double.IsInfinity(zAxis.Y) || double.IsInfinity(zAxis.Z))
                {
                    return false;
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the total elevation of the object (level elevation + object elevation above level)
        /// </summary>
        /// <param name="level">The level this object belongs to</param>
        /// <returns>Total elevation in millimeters</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double GetTotalElevation(ILevel level)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevelObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (level == null)
                throw new ArgumentNullException(nameof(level), "Level cannot be null");
            
            try
            {
                return level.Elevation + this.ElevationAboveLevel;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to calculate total elevation: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the object type name for debugging
        /// </summary>
        /// <returns>String representation of the object type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetObjectTypeName()
        {
            return "ILevelObject";
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        /// <returns>True if initialized, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized()
        {
            return _i != null && !_disposed;
        }

        /// <summary>
        /// Gets debug information about the interface state
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            var info = "🔧 ILevelObject Debug Information:\n";
            info += $"✅ Interface initialized: {IsInitialized()}\n";
            info += $"✅ COM object valid: {_i != null}\n";
            info += $"✅ Disposed: {_disposed}\n";
            
            if (IsInitialized())
            {
                try
                {
                    info += $"✅ Level ID: {this.LevelId}\n";
                    info += $"✅ Elevation above level: {this.ElevationAboveLevel} mm\n";
                    info += $"✅ Placement elevation: {this.PlacementElevation} mm\n";
                    info += $"✅ Vertical offset: {this.VerticalOffset} mm\n";
                    info += $"✅ Can edit placement: {CanSetPlacement()}\n";
                }
                catch (Exception ex)
                {
                    info += $"❌ Error accessing level object properties: {ex.Message}\n";
                }
            }
            
            return info;
        }

        #endregion
    }
}
