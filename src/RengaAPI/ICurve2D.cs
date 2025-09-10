using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ICurve2D interface - Complete API Reference
    /// This class provides comprehensive access to all ICurve2D interface members
    /// </summary>
    public class ICurve2D : IDisposable
    {
        private Renga.ICurve2D _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.ICurve2D _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="curve2D">COM object implementing ICurve2D</param>
        public ICurve2D(object curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");
            
            _i = curve2D as Renga.ICurve2D;
            if (_i == null)
                throw new ArgumentException("Object does not implement ICurve2D interface", nameof(curve2D));
        }

        /// <summary>
        /// Constructor from existing ICurve2D
        /// </summary>
        /// <param name="curve2D">Existing ICurve2D instance</param>
        public ICurve2D(Renga.ICurve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");
            
            _i = curve2D;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ICurve2D()
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

        #region Core ICurve2D Methods

        /// <summary>
        /// Calculates the start point of the curve
        /// </summary>
        /// <returns>Renga.Point2D representing the start point</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point2D GetBeginPoint()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetBeginPoint();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get begin point: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates the end point of the curve
        /// </summary>
        /// <returns>Renga.Point2D representing the end point</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point2D GetEndPoint()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetEndPoint();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get end point: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates the metric length of the curve
        /// </summary>
        /// <returns>Length of the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double GetLength()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetLength();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get length: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the curve is closed
        /// </summary>
        /// <returns>True if the curve is closed, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsClosed()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.IsClosed();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if curve is closed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates a point on the curve by the given parameter value
        /// </summary>
        /// <param name="param">Parameter value</param>
        /// <returns>Renga.Point2D on the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point2D GetPointOn(double param)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetPointOn(param);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get point on curve at parameter {param}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates the nearest projection of a point onto the curve
        /// </summary>
        /// <param name="point">Point to project</param>
        /// <returns>Parameter value of the projection</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double PointProjection(Renga.Point2D point)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.PointProjection(point);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to project point onto curve: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates the bounding rectangle of the curve
        /// </summary>
        /// <returns>Dictionary containing two Renga.Point2D objects representing the bounding box</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "MinPoint", "MaxPoint" })]
        public Dictionary<string, object> GetGabarit()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                Renga.Point2D p1, p2;
                this._i.GetGabarit(out p1, out p2);
                return new Dictionary<string, object>
                {
                    { "MinPoint", p1 },
                    { "MaxPoint", p2 }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get bounding box: {ex.Message}", ex);
            }
        }

        #endregion

        #region Curve Operations

        /// <summary>
        /// Returns a copy of the curve
        /// </summary>
        /// <returns>ICurve2D copy of the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetCopy()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var copy = this._i.GetCopy();
                return new ICurve2D(copy);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get copy: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns an equidistant curve which is shifted by the given value
        /// </summary>
        /// <param name="offset">Offset distance</param>
        /// <returns>ICurve2D representing the offset curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetOffseted(double offset)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var offseted = this._i.GetOffseted(offset);
                return new ICurve2D(offseted);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create offset curve with offset {offset}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns curve copy transformed according to a given transform
        /// </summary>
        /// <param name="transform">Transform to apply</param>
        /// <returns>ICurve2D representing the transformed curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetTransformed(Renga.ITransform2D transform)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "Transform cannot be null");
            
            try
            {
                var transformed = this._i.GetTransformed(transform);
                return new ICurve2D(transformed);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to transform curve: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Constructs a trimmed curve
        /// </summary>
        /// <param name="t1">Start parameter</param>
        /// <param name="t2">End parameter</param>
        /// <param name="sense">Direction sense (1 for forward, -1 for reverse)</param>
        /// <returns>ICurve2D representing the trimmed curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetTrimmed(double t1, double t2, int sense)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var trimmed = this._i.GetTrimmed(t1, t2, sense);
                return new ICurve2D(trimmed);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to trim curve from {t1} to {t2}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates a point on the curve which is offset from a given reference point on the curve
        /// </summary>
        /// <param name="startT">Starting parameter</param>
        /// <param name="distance">Distance to offset</param>
        /// <param name="direction">Direction (1 for forward, -1 for reverse)</param>
        /// <returns>Parameter value of the offset point</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double GetParameterAtDistance(double startT, double distance, int direction)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.GetParameterAtDistance(startT, distance, direction);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter at distance {distance} from {startT}: {ex.Message}", ex);
            }
        }

        #endregion

        #region 3D Curve Creation

        /// <summary>
        /// Constructs a 3D curve based on the provided placement
        /// </summary>
        /// <param name="placement">3D placement for the curve</param>
        /// <returns>Renga.ICurve3D representing the 3D curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ICurve3D CreateCurve3D(Renga.IPlacement3D placement)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (placement == null)
                throw new ArgumentNullException(nameof(placement), "Placement cannot be null");
            
            try
            {
                return this._i.CreateCurve3D(placement);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 3D curve: {ex.Message}", ex);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the type of the curve
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Curve2DType Curve2DType
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.Curve2DType;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get curve type: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// The upper bound of the parameter interval the curve is defined on
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double MaxParameter
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.MaxParameter;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get max parameter: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// The lower bound of the parameter interval the curve is defined on
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double MinParameter
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ICurve2D interface is not initialized. Please check that Renga is running and a project is loaded.");
                
                try
                {
                    return this._i.MinParameter;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get min parameter: {ex.Message}", ex);
                }
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
            return "ICurve2D";
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
            var info = "🔧 ICurve2D Debug Information:\n";
            info += $"✅ Interface initialized: {IsInitialized()}\n";
            info += $"✅ COM object valid: {_i != null}\n";
            info += $"✅ Disposed: {_disposed}\n";
            
            if (IsInitialized())
            {
                try
                {
                    info += $"✅ Curve type: {Curve2DType}\n";
                    info += $"✅ Parameter range: [{MinParameter}, {MaxParameter}]\n";
                    info += $"✅ Length: {GetLength()}\n";
                    info += $"✅ Is closed: {IsClosed()}\n";
                }
                catch (Exception ex)
                {
                    info += $"❌ Error accessing curve properties: {ex.Message}\n";
                }
            }
            
            return info;
        }

        #endregion
    }
}
