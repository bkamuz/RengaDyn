using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ILevel interface - Complete API Reference
    /// This class provides comprehensive access to all ILevel interface members
    /// </summary>
    public class ILevel : IDisposable
    {
        private Renga.ILevel _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.ILevel _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="level">COM object implementing ILevel</param>
        public ILevel(object level)
        {
            if (level == null)
                throw new ArgumentNullException(nameof(level), "Level cannot be null");
            
            _i = level as Renga.ILevel;
            if (_i == null)
                throw new ArgumentException("Object does not implement ILevel interface", nameof(level));
        }

        /// <summary>
        /// Constructor from existing ILevel
        /// </summary>
        /// <param name="level">Existing ILevel instance</param>
        public ILevel(Renga.ILevel level)
        {
            if (level == null)
                throw new ArgumentNullException(nameof(level), "Level cannot be null");
            
            _i = level;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ILevel()
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

        #region Core ILevel Properties

        /// <summary>
        /// Gets the elevation of the level relative to the XOY plane of the project coordinate system in millimeters
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double Elevation
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevel interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.Elevation;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get level elevation: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the name of the level
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string LevelName
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevel interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.LevelName;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get level name: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the level placement in the project coordinate system
        /// The placement origin Z coordinate is equal to the Elevation property value
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.DynGeometry.Placement3D Placement
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ILevel interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                var placement = this._i.Placement;
                return new DynRenga.DynGeometry.Placement3D(placement);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get level placement: {ex.Message}", ex);
                }
            }
        }

        #endregion

        #region Advanced Level Methods

        /// <summary>
        /// Gets comprehensive level information
        /// </summary>
        /// <returns>Dictionary containing level information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Elevation", "LevelName", "Placement", "DebugInfo" })]
        public Dictionary<string, object> GetLevelInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevel interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            var debugInfo = "🔧 ILevel.GetLevelInfo Debug Info:\n";
            var elevation = 0.0;
            var levelName = "";
            DynRenga.DynGeometry.Placement3D placement = null;
            
            try
            {
                debugInfo += $"✅ Interface initialized: {IsInitialized()}\n";
                
                // Get elevation
                elevation = this._i.Elevation;
                debugInfo += $"✅ Elevation: {elevation} mm\n";
                
                // Get level name
                levelName = this._i.LevelName;
                debugInfo += $"✅ Level Name: {levelName}\n";
                
                // Get placement
                var rengaPlacement = this._i.Placement;
                placement = new DynRenga.DynGeometry.Placement3D(rengaPlacement);
                debugInfo += $"✅ Placement: Origin=({placement.Origin().X}, {placement.Origin().Y}, {placement.Origin().Z})\n";
                
                debugInfo += "✅ Level information retrieved successfully\n";
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error getting level information: {ex.Message}\n";
                debugInfo += "💡 Check if the object supports ILevel interface\n";
            }
            
            return new Dictionary<string, object>
            {
                { "Elevation", elevation },
                { "LevelName", levelName },
                { "Placement", placement },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Checks if this level is above another level
        /// </summary>
        /// <param name="otherLevel">The level to compare with</param>
        /// <returns>True if this level is above the other level</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsAbove(ILevel otherLevel)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevel interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (otherLevel == null)
                throw new ArgumentNullException(nameof(otherLevel), "Other level cannot be null");
            
            try
            {
                return this.Elevation > otherLevel.Elevation;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to compare levels: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the vertical distance between this level and another level
        /// </summary>
        /// <param name="otherLevel">The level to measure distance to</param>
        /// <returns>Vertical distance in millimeters (positive if this level is above)</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double GetVerticalDistance(ILevel otherLevel)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ILevel interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (otherLevel == null)
                throw new ArgumentNullException(nameof(otherLevel), "Other level cannot be null");
            
            try
            {
                return this.Elevation - otherLevel.Elevation;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to calculate vertical distance: {ex.Message}", ex);
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
            return "ILevel";
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
            var info = "🔧 ILevel Debug Information:\n";
            info += $"✅ Interface initialized: {IsInitialized()}\n";
            info += $"✅ COM object valid: {_i != null}\n";
            info += $"✅ Disposed: {_disposed}\n";
            
            if (IsInitialized())
            {
                try
                {
                    info += $"✅ Level Name: {this.LevelName}\n";
                    info += $"✅ Elevation: {this.Elevation} mm\n";
                    info += $"✅ Placement: Origin=({this.Placement.Origin().X}, {this.Placement.Origin().Y}, {this.Placement.Origin().Z})\n";
                }
                catch (Exception ex)
                {
                    info += $"❌ Error accessing level properties: {ex.Message}\n";
                }
            }
            
            return info;
        }

        #endregion
    }
}
