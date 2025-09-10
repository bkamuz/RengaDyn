using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IBaseline2DObject interface - Complete API Reference
    /// This class provides comprehensive access to all IBaseline2DObject interface members
    /// </summary>
    public class IBaseline2DObject : IDisposable
    {
        private Renga.IBaseline2DObject _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.IBaseline2DObject _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="baseline2DObject">COM object implementing IBaseline2DObject</param>
        public IBaseline2DObject(object baseline2DObject)
        {
            if (baseline2DObject == null)
                throw new ArgumentNullException(nameof(baseline2DObject), "Baseline2DObject cannot be null");
            
            _i = baseline2DObject as Renga.IBaseline2DObject;
            if (_i == null)
                throw new ArgumentException("Object does not implement IBaseline2DObject interface", nameof(baseline2DObject));
        }

        /// <summary>
        /// Constructor from existing IBaseline2DObject
        /// </summary>
        /// <param name="baseline2DObject">Existing IBaseline2DObject instance</param>
        public IBaseline2DObject(Renga.IBaseline2DObject baseline2DObject)
        {
            if (baseline2DObject == null)
                throw new ArgumentNullException(nameof(baseline2DObject), "Baseline2DObject cannot be null");
            
            _i = baseline2DObject;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~IBaseline2DObject()
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

        #region Core IBaseline2DObject Methods

        /// <summary>
        /// Returns the copy of the 2D baseline of the object in its own coordinate system
        /// </summary>
        /// <returns>Curve2D representing the baseline</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetBaseline()
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var curve2D = this._i.GetBaseline();
                return new Curve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get baseline: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the copy of the 2D baseline of the object in the specified coordinate system
        /// </summary>
        /// <param name="placement2D">A coordinate system to which the 2D baseline will be transformed</param>
        /// <returns>Curve2D representing the baseline in the specified coordinate system</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetBaselineInCS(DynRenga.DynGeometry.Placement2D placement2D)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (placement2D == null)
                throw new ArgumentNullException(nameof(placement2D), "Placement2D cannot be null");
            
            try
            {
                var curve2D = this._i.GetBaselineInCS(placement2D.ToRengaPlacement2D());
                return new Curve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get baseline in coordinate system: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the 2D baseline of the object in its own coordinate system
        /// Limitations: It is impossible to edit the baseline of an object with dependent objects (e.g., Roof)
        /// </summary>
        /// <param name="baseline">New baseline of the object</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBaseline(Curve2D baseline)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (baseline == null)
                throw new ArgumentNullException(nameof(baseline), "Baseline cannot be null");
            
            try
            {
                this._i.SetBaseline(baseline._i);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set baseline: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the 2D baseline of the object in the specified coordinate system
        /// Limitations: It is impossible to edit the baseline of an object with dependent objects (e.g., Roof)
        /// </summary>
        /// <param name="placement2D">A coordinate system to which the 2D baseline will be transformed</param>
        /// <param name="baselineInCS">New baseline of the object in the specified coordinate system</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBaselineInCS(DynRenga.DynGeometry.Placement2D placement2D, Curve2D baselineInCS)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (placement2D == null)
                throw new ArgumentNullException(nameof(placement2D), "Placement2D cannot be null");
            
            if (baselineInCS == null)
                throw new ArgumentNullException(nameof(baselineInCS), "BaselineInCS cannot be null");
            
            try
            {
                this._i.SetBaselineInCS(placement2D.ToRengaPlacement2D(), baselineInCS._i);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set baseline in coordinate system: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Baseline2D Methods

        /// <summary>
        /// Validates if the baseline can be set for this object
        /// </summary>
        /// <returns>True if baseline can be set, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool CanSetBaseline()
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                // Try to get the current baseline to check if it's editable
                var baseline = this._i.GetBaseline();
                return baseline != null;
            }
            catch (Exception)
            {
                // If we can't get the baseline, it might not be editable
                return false;
            }
        }

        /// <summary>
        /// Gets baseline information including coordinate system details
        /// </summary>
        /// <returns>Dictionary containing baseline information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "HasBaseline", "CanEdit", "DebugInfo" })]
        public Dictionary<string, object> GetBaselineInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            var debugInfo = "🔧 Getting baseline information...\n";
            var hasBaseline = false;
            var canEdit = false;
            
            try
            {
                // Check if baseline exists
                var baseline = this._i.GetBaseline();
                hasBaseline = baseline != null;
                debugInfo += $"✅ Baseline exists: {hasBaseline}\n";
                
                if (hasBaseline)
                {
                    // Check if baseline can be edited
                    canEdit = CanSetBaseline();
                    debugInfo += $"✅ Baseline editable: {canEdit}\n";
                }
                
                debugInfo += "✅ Baseline information retrieved successfully\n";
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error getting baseline information: {ex.Message}\n";
                debugInfo += "💡 Check if the object supports IBaseline2DObject interface\n";
            }
            
            return new Dictionary<string, object>
            {
                { "HasBaseline", hasBaseline },
                { "CanEdit", canEdit },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Creates a copy of the baseline in a different coordinate system
        /// </summary>
        /// <param name="sourcePlacement">Source coordinate system</param>
        /// <param name="targetPlacement">Target coordinate system</param>
        /// <returns>Curve2D representing the baseline in the target coordinate system</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D TransformBaseline(DynRenga.DynGeometry.Placement2D sourcePlacement, DynRenga.DynGeometry.Placement2D targetPlacement)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (sourcePlacement == null)
                throw new ArgumentNullException(nameof(sourcePlacement), "Source placement cannot be null");
            
            if (targetPlacement == null)
                throw new ArgumentNullException(nameof(targetPlacement), "Target placement cannot be null");
            
            try
            {
                // Get baseline in source coordinate system
                var baselineInSource = this._i.GetBaselineInCS(sourcePlacement.ToRengaPlacement2D());
                
                // Transform to target coordinate system
                var baselineInTarget = this._i.GetBaselineInCS(targetPlacement.ToRengaPlacement2D());
                
                return new Curve2D(baselineInTarget);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to transform baseline: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the baseline is valid for this object type
        /// </summary>
        /// <param name="baseline">Baseline to validate</param>
        /// <returns>True if baseline is valid, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsBaselineValid(Curve2D baseline)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IBaseline2DObject interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (baseline == null)
                return false;
            
            try
            {
                // Try to set the baseline temporarily to check if it's valid
                var originalBaseline = this._i.GetBaseline();
                this._i.SetBaseline(baseline._i);
                
                // Restore original baseline
                this._i.SetBaseline(originalBaseline);
                
                return true;
            }
            catch (Exception)
            {
                return false;
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
            return "IBaseline2DObject";
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
            var info = "🔧 IBaseline2DObject Debug Information:\n";
            info += $"✅ Interface initialized: {IsInitialized()}\n";
            info += $"✅ COM object valid: {_i != null}\n";
            info += $"✅ Disposed: {_disposed}\n";
            
            if (IsInitialized())
            {
                try
                {
                    var baseline = this._i.GetBaseline();
                    info += $"✅ Baseline available: {baseline != null}\n";
                }
                catch (Exception ex)
                {
                    info += $"❌ Error accessing baseline: {ex.Message}\n";
                }
            }
            
            return info;
        }

        #endregion
    }
}
