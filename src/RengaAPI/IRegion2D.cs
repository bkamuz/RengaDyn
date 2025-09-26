using System;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IRegion2D interface
    /// </summary>
    public class IRegion2D : IDisposable
    {
        private Renga.IRegion2D _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.IRegion2D _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="region2D">COM object implementing IRegion2D</param>
        public IRegion2D(object region2D)
        {
            if (region2D == null)
                throw new ArgumentNullException(nameof(region2D), "Region2D cannot be null");

            _i = region2D as Renga.IRegion2D;
            if (_i == null)
                throw new ArgumentException("Object does not implement IRegion2D interface", nameof(region2D));
        }

        /// <summary>
        /// Constructor from existing IRegion2D
        /// </summary>
        /// <param name="region2D">Existing IRegion2D instance</param>
        public IRegion2D(Renga.IRegion2D region2D)
        {
            if (region2D == null)
                throw new ArgumentNullException(nameof(region2D), "Region2D cannot be null");

            _i = region2D;
        }

        ~IRegion2D()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // managed
                }
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Get the number of contours including the outer contour.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetContourCount()
        {
            if (_i == null)
                throw new InvalidOperationException("IRegion2D interface is not initialized.");
            try
            {
                return _i.GetContourCount();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get contour count: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get outer contour of the region.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetOuterContour()
        {
            if (_i == null)
                throw new InvalidOperationException("IRegion2D interface is not initialized.");
            try
            {
                var contour = _i.GetOuterContour();
                return new ICurve2D(contour);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get outer contour: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get contour by index.
        /// </summary>
        /// <param name="index">Zero-based index of contour</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve2D GetContour(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("IRegion2D interface is not initialized.");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            try
            {
                var contour = _i.GetContour(index);
                return new ICurve2D(contour);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get contour at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper: returns whether interface is initialized.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized()
        {
            return _i != null && !_disposed;
        }
    }
}


