using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IMath interface - Complete API Reference
    /// This class provides comprehensive access to all IMath interface members
    /// </summary>
    public class IMath : IDisposable
    {
        private Renga.IMath _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.IMath _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="math">COM object implementing IMath</param>
        public IMath(object math)
        {
            if (math == null)
                throw new ArgumentNullException(nameof(math), "Math cannot be null");
            
            _i = math as Renga.IMath;
            if (_i == null)
                throw new ArgumentException("Object does not implement IMath interface", nameof(math));
        }

        /// <summary>
        /// Constructor from existing IMath
        /// </summary>
        /// <param name="math">Existing IMath instance</param>
        public IMath(Renga.IMath math)
        {
            if (math == null)
                throw new ArgumentNullException(nameof(math), "Math cannot be null");
            
            _i = math;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~IMath()
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

        #region 2D Curve Creation

        /// <summary>
        /// Creates a 2D line segment between two specified points
        /// </summary>
        /// <param name="startPoint">Starting point of the line segment</param>
        /// <param name="endPoint">Ending point of the line segment</param>
        /// <returns>ICurve2D representing the line segment</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D CreateLineSegment2D(Renga.Point2D startPoint, Renga.Point2D endPoint)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var curve2D = this._i.CreateLineSegment2D(startPoint, endPoint);
                return new ICurve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 2D line segment: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a 2D circle defined by a center point and a radius
        /// </summary>
        /// <param name="centerPoint">Center point of the circle</param>
        /// <param name="radius">Radius of the circle</param>
        /// <returns>ICurve2D representing the circle</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D CreateCircle2D(Renga.Point2D centerPoint, double radius)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (radius <= 0)
                throw new ArgumentException("Radius must be greater than zero", nameof(radius));
            
            try
            {
                var curve2D = this._i.CreateCircle2D(centerPoint, radius);
                return new ICurve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 2D circle with radius {radius}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a 2D arc defined by a center point, a start point, an end point, and a direction
        /// </summary>
        /// <param name="centerPoint">Center point of the arc</param>
        /// <param name="startPoint">Starting point of the arc</param>
        /// <param name="endPoint">Ending point of the arc</param>
        /// <param name="clockwise">Direction of the arc (true for clockwise, false for counterclockwise)</param>
        /// <returns>ICurve2D representing the arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D CreateArc2DByCenterStartEndPoints(Renga.Point2D centerPoint, Renga.Point2D startPoint, Renga.Point2D endPoint, bool clockwise)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var curve2D = this._i.CreateArc2DByCenterStartEndPoints(centerPoint, startPoint, endPoint, clockwise);
                return new ICurve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 2D arc: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a 2D arc defined by three points: a start point, an intermediate point, and an end point
        /// </summary>
        /// <param name="startPoint">Starting point of the arc</param>
        /// <param name="intermediatePoint">Intermediate point on the arc</param>
        /// <param name="endPoint">Ending point of the arc</param>
        /// <returns>ICurve2D representing the arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D CreateArc2DByThreePoints(Renga.Point2D startPoint, Renga.Point2D intermediatePoint, Renga.Point2D endPoint)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var curve2D = this._i.CreateArc2DByThreePoints(startPoint, intermediatePoint, endPoint);
                return new ICurve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 2D arc from three points: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a composite 2D curve from an array of existing 2D curves
        /// </summary>
        /// <param name="curves">Array of ICurve2D objects to combine</param>
        /// <returns>ICurve2D representing the composite curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D CreateCompositeCurve2D(ICurve2D[] curves)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (curves == null || curves.Length == 0)
                throw new ArgumentException("Curves array cannot be null or empty", nameof(curves));
            
            try
            {
                // Convert ICurve2D array to COM object array
                var comCurves = new object[curves.Length];
                for (int i = 0; i < curves.Length; i++)
                {
                    if (curves[i] == null)
                        throw new ArgumentException($"Curve at index {i} cannot be null", nameof(curves));
                    
                    comCurves[i] = curves[i]._i_Internal;
                }
                
                var curve2D = this._i.CreateCompositeCurve2D(comCurves);
                return new ICurve2D(curve2D);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create composite 2D curve from {curves.Length} curves: {ex.Message}", ex);
            }
        }

        #endregion

        #region 3D Curve Creation

        /// <summary>
        /// Creates a 3D line segment between two specified points
        /// </summary>
        /// <param name="startPoint">Starting point of the line segment</param>
        /// <param name="endPoint">Ending point of the line segment</param>
        /// <returns>Renga.ICurve3D representing the line segment</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ICurve3D CreateLineSegment3D(Renga.Point3D startPoint, Renga.Point3D endPoint)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.CreateLineSegment3D(startPoint, endPoint);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 3D line segment: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a 3D circle defined by a center point, a normal vector, and a radius
        /// </summary>
        /// <param name="centerPoint">Center point of the circle</param>
        /// <param name="normalVector">Normal vector of the circle's plane</param>
        /// <param name="radius">Radius of the circle</param>
        /// <returns>Renga.ICurve3D representing the circle</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ICurve3D CreateCircle3D(Renga.Point3D centerPoint, Renga.Vector3D normalVector, double radius)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (radius <= 0)
                throw new ArgumentException("Radius must be greater than zero", nameof(radius));
            
            try
            {
                return this._i.CreateCircle3D(centerPoint, normalVector, radius);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 3D circle with radius {radius}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a 3D arc defined by a center point, a start point, an end point, and a direction
        /// </summary>
        /// <param name="centerPoint">Center point of the arc</param>
        /// <param name="startPoint">Starting point of the arc</param>
        /// <param name="endPoint">Ending point of the arc</param>
        /// <param name="clockwise">Direction of the arc (true for clockwise, false for counterclockwise)</param>
        /// <returns>Renga.ICurve3D representing the arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ICurve3D CreateArc3DByCenterStartEndPoints(Renga.Point3D centerPoint, Renga.Point3D startPoint, Renga.Point3D endPoint, bool clockwise)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.CreateArc3DByCenterStartEndPoints(centerPoint, startPoint, endPoint, clockwise);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 3D arc: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a 3D arc defined by three points: a start point, an intermediate point, and an end point
        /// </summary>
        /// <param name="startPoint">Starting point of the arc</param>
        /// <param name="intermediatePoint">Intermediate point on the arc</param>
        /// <param name="endPoint">Ending point of the arc</param>
        /// <returns>Renga.ICurve3D representing the arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ICurve3D CreateArc3DByThreePoints(Renga.Point3D startPoint, Renga.Point3D intermediatePoint, Renga.Point3D endPoint)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                return this._i.CreateArc3DByThreePoints(startPoint, intermediatePoint, endPoint);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create 3D arc from three points: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a composite 3D curve from an array of existing 3D curves
        /// </summary>
        /// <param name="curves">Array of Renga.ICurve3D objects to combine</param>
        /// <returns>Renga.ICurve3D representing the composite curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ICurve3D CreateCompositeCurve3D(Renga.ICurve3D[] curves)
        {
            if (this._i == null) 
                throw new InvalidOperationException("IMath interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (curves == null || curves.Length == 0)
                throw new ArgumentException("Curves array cannot be null or empty", nameof(curves));
            
            try
            {
                // Convert ICurve3D array to COM object array
                var comCurves = new object[curves.Length];
                for (int i = 0; i < curves.Length; i++)
                {
                    if (curves[i] == null)
                        throw new ArgumentException($"Curve at index {i} cannot be null", nameof(curves));
                    
                    comCurves[i] = curves[i];
                }
                
                return this._i.CreateCompositeCurve3D(comCurves);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create composite 3D curve from {curves.Length} curves: {ex.Message}", ex);
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
            return "IMath";
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
            var info = "🔧 IMath Debug Information:\n";
            info += $"✅ Interface initialized: {IsInitialized()}\n";
            info += $"✅ COM object valid: {_i != null}\n";
            info += $"✅ Disposed: {_disposed}\n";
            
            if (IsInitialized())
            {
                info += "✅ IMath interface ready for geometric operations\n";
                info += "📐 Available operations:\n";
                info += "  • CreateLineSegment2D/3D\n";
                info += "  • CreateCircle2D/3D\n";
                info += "  • CreateArc2D/3D (by center/points or three points)\n";
                info += "  • CreateCompositeCurve2D/3D\n";
            }
            
            return info;
        }

        #endregion
    }
}