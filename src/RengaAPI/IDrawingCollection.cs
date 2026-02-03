using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IDrawingCollection interface.
    /// Represents a collection of IDrawing objects.
    /// API: https://help.rengabim.com/api/interface_i_drawing_collection.html
    /// </summary>
    public class IDrawingCollection : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IDrawingCollection
        /// </summary>
        public Renga.IDrawingCollection _i;

        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IDrawingCollection from COM object
        /// </summary>
        /// <param name="rengaDrawingCollectionObject">The Renga.IDrawingCollection COM object (e.g. from IProject.Drawings or IProject.Drawings2)</param>
        internal IDrawingCollection(object rengaDrawingCollectionObject)
        {
            if (rengaDrawingCollectionObject == null)
            {
                _i = null;
                return;
            }
            _i = rengaDrawingCollectionObject as Renga.IDrawingCollection;
            if (_i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IDrawingCollection.");
        }

        /// <summary>
        /// Gets the number of drawings in the collection. Returns 0 when collection is not initialized (e.g. no Drawings in project).
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (_i == null)
                    return 0;
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
        /// Returns a drawing by index in the collection (0-based)
        /// </summary>
        /// <param name="index">Zero-based index</param>
        /// <returns>IDrawing wrapper</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IDrawing Get(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("DrawingCollection interface is not initialized (e.g. no Drawings in project).");
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Collection has {Count} items.");
            try
            {
                var rengaDrawing = _i.Get(index);
                return new IDrawing(rengaDrawing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get drawing at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all drawings in the collection as a list. Returns empty list when collection is not initialized.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IDrawing> GetAllDrawings()
        {
            var list = new List<IDrawing>();
            if (_i == null)
                return list;
            for (int i = 0; i < Count; i++)
                list.Add(Get(i));
            return list;
        }

        /// <summary>
        /// Disposes the Renga.IDrawingCollection COM object
        /// </summary>
        public void Dispose()
        {
            if (_i != null && !_disposed)
            {
                Marshal.ReleaseComObject(_i);
                _i = null;
                _disposed = true;
            }
        }

        ~IDrawingCollection()
        {
            Dispose();
        }
    }
}
