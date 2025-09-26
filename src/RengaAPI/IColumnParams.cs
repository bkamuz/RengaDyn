using System;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using DynRenga.DynGeometry;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IColumnParams interface
    /// </summary>
    public class IColumnParams : IDisposable
    {
        private Renga.IColumnParams _i;
        private bool _disposed = false;

        public Renga.IColumnParams _i_Internal => _i;

        public IColumnParams(object columnParams)
        {
            if (columnParams == null)
                throw new ArgumentNullException(nameof(columnParams), "ColumnParams cannot be null");
            _i = columnParams as Renga.IColumnParams;
            if (_i == null)
                throw new ArgumentException("Object does not implement IColumnParams interface", nameof(columnParams));
        }

        public IColumnParams(Renga.IColumnParams columnParams)
        {
            _i = columnParams ?? throw new ArgumentNullException(nameof(columnParams));
        }

        ~IColumnParams()
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
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                _disposed = true;
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public Placement2D GetProfilePlacement()
        {
            if (_i == null) throw new InvalidOperationException("IColumnParams interface is not initialized.");
            try { return new Placement2D(_i.GetProfilePlacement()); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get profile placement: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public double Height
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnParams interface is not initialized.");
                try { return _i.Height; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Height: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point3D Position
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnParams interface is not initialized.");
                try { return _i.Position; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Position: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public int StyleId
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnParams interface is not initialized.");
                try { return _i.StyleId; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get StyleId: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public double VerticalOffset
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnParams interface is not initialized.");
                try { return _i.VerticalOffset; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get VerticalOffset: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}


