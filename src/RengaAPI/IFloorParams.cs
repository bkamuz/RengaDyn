using System;
using System.Linq;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IFloorParams interface
    /// Provides methods and properties to access floor geometry and parameters
    /// </summary>
    public class IFloorParams : IDisposable
    {
        private Renga.IFloorParams _i;
        private bool _disposed = false;

        public Renga.IFloorParams _i_Internal => _i;

        /// <summary>
        /// Constructor - Creates IFloorParams from COM object
        /// </summary>
        /// <param name="floorParams">The Renga.IFloorParams COM object</param>
        public IFloorParams(object floorParams)
        {
            if (floorParams == null)
                throw new ArgumentNullException(nameof(floorParams), "FloorParams cannot be null");
            _i = floorParams as Renga.IFloorParams;
            if (_i == null)
                throw new ArgumentException("Object does not implement IFloorParams interface", nameof(floorParams));
        }

        /// <summary>
        /// Constructor - Creates IFloorParams from typed COM object
        /// </summary>
        /// <param name="floorParams">The typed Renga.IFloorParams object</param>
        public IFloorParams(Renga.IFloorParams floorParams)
        {
            _i = floorParams ?? throw new ArgumentNullException(nameof(floorParams));
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~IFloorParams()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose implementation
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Gets the thickness of the floor
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double Thickness
        {
            get
            {
                if (_i == null) 
                    throw new InvalidOperationException("IFloorParams interface is not initialized.");
                try 
                { 
                    return _i.Thickness; 
                } 
                catch (Exception ex) 
                { 
                    throw new InvalidOperationException($"Failed to get floor thickness: {ex.Message}", ex); 
                }
            }
        }

        /// <summary>
        /// Gets the vertical offset of the floor
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double VerticalOffset
        {
            get
            {
                if (_i == null) 
                    throw new InvalidOperationException("IFloorParams interface is not initialized.");
                try 
                { 
                    return _i.VerticalOffset; 
                } 
                catch (Exception ex) 
                { 
                    throw new InvalidOperationException($"Failed to get floor vertical offset: {ex.Message}", ex); 
                }
            }
        }

        /// <summary>
        /// Retrieves the contour of the floor
        /// </summary>
        /// <returns>ICurve2D wrapper object representing the floor contour</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetContour()
        {
            if (_i == null) 
                throw new InvalidOperationException("IFloorParams interface is not initialized.");
            try 
            { 
                var contour = _i.GetContour();
                return new ICurve2D(contour);
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get floor contour: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Retrieves the IDs of objects that are dependent on this floor
        /// </summary>
        /// <returns>Array of dependent object IDs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int[] GetDependentObjectIds()
        {
            if (_i == null) 
                throw new InvalidOperationException("IFloorParams interface is not initialized.");
            try 
            { 
                var ids = _i.GetDependentObjectIds();
                return ids.Cast<int>().ToArray();
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get dependent object IDs: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        /// <returns>True if initialized and not disposed</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;

        /// <summary>
        /// Gets debug information about the floor parameters
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        public string GetDebugInfo()
        {
            if (!IsInitialized())
                return "IFloorParams - Not initialized or disposed";
            
            try
            {
                return $"IFloorParams - Thickness: {Thickness}, VerticalOffset: {VerticalOffset}, Initialized: {IsInitialized()}";
            }
            catch (Exception ex)
            {
                return $"IFloorParams - Error getting debug info: {ex.Message}";
            }
        }

        /// <summary>
        /// Returns a string representation of the floor parameters
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            if (!IsInitialized())
                return "IFloorParams (Not initialized)";
            
            try
            {
                return $"IFloorParams: Thickness={Thickness}, VerticalOffset={VerticalOffset}";
            }
            catch
            {
                return "IFloorParams (Error accessing properties)";
            }
        }
    }
}