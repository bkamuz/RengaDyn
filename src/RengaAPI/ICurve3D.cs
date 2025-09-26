using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ICurve3D interface - Complete API Reference
    /// This class provides comprehensive access to all ICurve3D interface members
    /// </summary>
    public class ICurve3D : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.ICurve3D
        /// </summary>
        public Renga.ICurve3D _i;

        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates ICurve3D from existing ICurve3D interface
        /// </summary>
        /// <param name="curve3D">Existing ICurve3D interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D(Renga.ICurve3D curve3D)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D interface cannot be null.");
            
            this._i = curve3D;
        }

        /// <summary>
        /// Constructor - Creates ICurve3D from existing Curve3D
        /// </summary>
        /// <param name="curve3D">Existing Curve3D instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D(DynGeometry.Curve3D curve3D)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null.");
            
            if (curve3D._i == null)
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            this._i = curve3D._i;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the curve
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Curve3DType
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Curve3D interface is not initialized.");
                
                return this._i.Curve3DType;
            }
        }

        /// <summary>
        /// Gets the upper bound of the parameter interval the curve is defined on
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double MaxParameter
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Curve3D interface is not initialized.");
                
                return this._i.MaxParameter;
            }
        }

        /// <summary>
        /// Gets the lower bound of the parameter interval the curve is defined on
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double MinParameter
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Curve3D interface is not initialized.");
                
                return this._i.MinParameter;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the start point of the curve
        /// </summary>
        /// <returns>Point3D representing the start point</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point3D GetBeginPoint()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
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
        /// <returns>Point3D representing the end point</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point3D GetEndPoint()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
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
        /// Returns a copy of the curve
        /// </summary>
        /// <returns>ICurve3D copy of the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D GetCopy()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                var copy = this._i.GetCopy();
                return new ICurve3D(copy);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get copy: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates the bounding box of a curve
        /// </summary>
        /// <returns>Cube representing the bounding box</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Cube GetGabarit()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                return this._i.GetGabarit();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get gabarit: {ex.Message}", ex);
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
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
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
        /// Calculates a point on the curve by the given parameter value
        /// </summary>
        /// <param name="param">Parameter value</param>
        /// <returns>Point3D on the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point3D GetPointOn(double param)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                return this._i.GetPointOn(param);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get point on curve: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns a curve offset on the given vector
        /// </summary>
        /// <param name="offset">Offset vector</param>
        /// <returns>ICurve3D offset curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D GetOffseted(Renga.Vector3D offset)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                var offsetCurve = this._i.GetOffseted(offset);
                return new ICurve3D(offsetCurve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get offset curve: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns curve copy transformed according to a given transform
        /// </summary>
        /// <param name="transform">Transform to apply</param>
        /// <returns>ICurve3D transformed curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D GetTransformed(DynGeometry.Transform3D transform)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            if (transform == null)
                throw new ArgumentNullException(nameof(transform), "Transform cannot be null.");
            
            try
            {
                var transformedCurve = this._i.GetTransformed(transform._i);
                return new ICurve3D(transformedCurve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get transformed curve: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Constructs a trimmed curve
        /// </summary>
        /// <param name="t1">First parameter value</param>
        /// <param name="t2">Second parameter value</param>
        /// <param name="sense">Direction sense (1 for forward, -1 for backward)</param>
        /// <returns>ICurve3D trimmed curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D GetTrimmed(double t1, double t2, int sense)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                var trimmedCurve = this._i.GetTrimmed(t1, t2, sense);
                return new ICurve3D(trimmedCurve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get trimmed curve: {ex.Message}", ex);
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
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
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
        /// Calculates the nearest projection of a point onto the curve
        /// </summary>
        /// <param name="point">Point to project</param>
        /// <returns>Parameter value of the projection</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double PointProjection(Renga.Point3D point)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                return this._i.PointProjection(point);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to project point: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculates a point on the curve which is offset from a given reference point on the curve
        /// </summary>
        /// <param name="startT">Starting parameter value</param>
        /// <param name="distance">Distance to offset</param>
        /// <param name="direction">Direction (1 for forward, -1 for backward)</param>
        /// <returns>Parameter value of the offset point</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double GetParameterAtDistance(double startT, double distance, int direction)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                return this._i.GetParameterAtDistance(startT, distance, direction);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter at distance: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the curve type as a string representation
        /// </summary>
        /// <returns>String representation of the curve type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetCurve3DTypeAsString()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                var curveType = (Renga.Curve3DType)this._i.Curve3DType;
                return GetCurve3DTypeAsString(curveType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get curve type as string: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts Curve3DType enum to string representation
        /// </summary>
        /// <param name="curveType">Curve3DType enum value</param>
        /// <returns>String representation of the curve type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetCurve3DTypeAsString(Renga.Curve3DType curveType)
        {
            switch (curveType)
            {
                case Renga.Curve3DType.Curve3DType_Undefined:
                    return "Undefined";
                case Renga.Curve3DType.Curve3DType_LineSegment:
                    return "LineSegment";
                case Renga.Curve3DType.Curve3DType_Arc:
                    return "Arc";
                case Renga.Curve3DType.Curve3DType_PolyCurve:
                    return "PolyCurve";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Gets all available Curve3DType values as a dictionary
        /// </summary>
        /// <returns>Dictionary of Curve3DType values</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, Renga.Curve3DType> GetCurve3DTypes()
        {
            return new Dictionary<string, Renga.Curve3DType>
            {
                { "Undefined", Renga.Curve3DType.Curve3DType_Undefined },
                { "LineSegment", Renga.Curve3DType.Curve3DType_LineSegment },
                { "Arc", Renga.Curve3DType.Curve3DType_Arc },
                { "PolyCurve", Renga.Curve3DType.Curve3DType_PolyCurve }
            };
        }

        /// <summary>
        /// Gets comprehensive information about the curve
        /// </summary>
        /// <returns>Dictionary containing all curve information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Curve3D interface is not initialized.");
            
            try
            {
                var info = new Dictionary<string, object>
                {
                    { "Curve3DType", this.Curve3DType },
                    { "Curve3DTypeAsString", GetCurve3DTypeAsString() },
                    { "MinParameter", this.MinParameter },
                    { "MaxParameter", this.MaxParameter },
                    { "Length", GetLength() },
                    { "IsClosed", IsClosed() },
                    { "BeginPoint", GetBeginPoint() },
                    { "EndPoint", GetEndPoint() }
                };

                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get curve info: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets debug information about the object
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null) 
                return "Curve3D interface is not initialized.";
            
            try
            {
                var info = GetInfo();
                var debugInfo = new System.Text.StringBuilder();
                debugInfo.AppendLine("=== ICurve3D Debug Info ===");
                debugInfo.AppendLine($"Type: {info["Curve3DTypeAsString"]}");
                debugInfo.AppendLine($"Parameter Range: [{info["MinParameter"]}, {info["MaxParameter"]}]");
                debugInfo.AppendLine($"Length: {info["Length"]}");
                debugInfo.AppendLine($"Is Closed: {info["IsClosed"]}");
                debugInfo.AppendLine($"Begin Point: {info["BeginPoint"]}");
                debugInfo.AppendLine($"End Point: {info["EndPoint"]}");
                
                return debugInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting debug info: {ex.Message}";
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of the resources used by this object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(false)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
        [dr.IsVisibleInDynamoLibrary(false)]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here if any
                }
                
                // Release COM object
                if (this._i != null)
                {
                    Marshal.ReleaseComObject(this._i);
                    this._i = null;
                }
                
                _disposed = true;
            }
        }
        
        /// <summary>
        /// Finalizer for releasing unmanaged resources
        /// </summary>
        ~ICurve3D()
        {
            Dispose(false);
        }

        #endregion
    }
}
