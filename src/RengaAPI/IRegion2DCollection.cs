using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IRegion2DCollection interface
    /// </summary>
    public class IRegion2DCollection : IDisposable
    {
        private Renga.IRegion2DCollection _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object reference
        /// </summary>
        public Renga.IRegion2DCollection _i_Internal => _i;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="collection">COM object implementing IRegion2DCollection</param>
        public IRegion2DCollection(object collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Region2DCollection cannot be null");

            _i = collection as Renga.IRegion2DCollection;
            if (_i == null)
                throw new ArgumentException("Object does not implement IRegion2DCollection interface", nameof(collection));
        }

        /// <summary>
        /// Constructor from existing IRegion2DCollection
        /// </summary>
        /// <param name="collection">Existing IRegion2DCollection instance</param>
        public IRegion2DCollection(Renga.IRegion2DCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Region2DCollection cannot be null");

            _i = collection;
        }

        ~IRegion2DCollection()
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
        /// Number of regions in the collection
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (_i == null)
                    throw new InvalidOperationException("IRegion2DCollection interface is not initialized.");
                try
                {
                    return _i.Count;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get Count: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Get region by index
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRegion2D GetByIndex(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("IRegion2DCollection interface is not initialized.");
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            try
            {
                var region = _i.Get(index);
                return new IRegion2D(region);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get region at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all regions as a list
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IRegion2D> GetAll()
        {
            if (_i == null)
                throw new InvalidOperationException("IRegion2DCollection interface is not initialized.");
            var list = new List<IRegion2D>();
            for (int i = 0; i < Count; i++)
            {
                list.Add(GetByIndex(i));
            }
            return list;
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


