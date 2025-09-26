using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Typed wrapper for Renga.IGrid
    /// </summary>
    public class IGrid : IDisposable
    {
        public Renga.IGrid _i;
        private bool _disposed = false;

        internal IGrid(object gridObj)
        {
            if (gridObj == null) throw new ArgumentNullException(nameof(gridObj));
            this._i = gridObj as Renga.IGrid;
            if (this._i == null) throw new InvalidCastException("Provided object cannot be cast to Renga.IGrid");
        }

        public int TriangleCount => this._i.TriangleCount;
        public int VertexCount => this._i.VertexCount;

    // Additional properties exposed by Renga.IGrid (Context7 API)
    public int GridType => this._i.GridType;
    public bool DoubleSided => this._i.DoubleSided;
    public int NormalCount => this._i.NormalCount;
    public int TextureCoordinateCount => this._i.TextureCoordinateCount;

        public dg.Point GetVertexAsPoint(int index)
        {
            if (index < 0 || index >= VertexCount) throw new ArgumentOutOfRangeException(nameof(index));
            var p = this._i.GetVertex(index);
            return dg.Point.ByCoordinates(p.X / 1000.0, p.Y / 1000.0, p.Z / 1000.0);
        }

        /// <summary>
        /// Returns vertex components (x, y, z) as primitive doubles (meters)
        /// </summary>
        public (double X, double Y, double Z) GetVertexComponents(int index)
        {
            if (index < 0 || index >= VertexCount) throw new ArgumentOutOfRangeException(nameof(index));
            float x = 0, y = 0, z = 0;
            try
            {
                this._i.GetVertexComponents(index, out x, out y, out z);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"GetVertexComponents failed: {ex.Message}", ex);
            }
            return (x / 1000.0, y / 1000.0, z / 1000.0);
        }

        public dg.IndexGroup GetTriangleAsIndexGroup(int index)
        {
            if (index < 0 || index >= TriangleCount) throw new ArgumentOutOfRangeException(nameof(index));
            var tr = this._i.GetTriangle(index);
            return dg.IndexGroup.ByIndices((uint)tr.V0, (uint)tr.V1, (uint)tr.V2);
        }

        /// <summary>
        /// Returns triangle components (v0, v1, v2) as unsigned ints
        /// </summary>
        public (uint V0, uint V1, uint V2) GetTriangleComponents(int index)
        {
            if (index < 0 || index >= TriangleCount) throw new ArgumentOutOfRangeException(nameof(index));
            uint v0 = 0, v1 = 0, v2 = 0;
            try
            {
                this._i.GetTriangleComponents(index, out v0, out v1, out v2);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"GetTriangleComponents failed: {ex.Message}", ex);
            }
            return (v0, v1, v2);
        }

        /// <summary>
        /// Returns normal vector as DesignScript point / vector helper
        /// </summary>
        public dg.Vector GetNormalAsVector(int index)
        {
            if (index < 0 || index >= NormalCount) throw new ArgumentOutOfRangeException(nameof(index));
            var vec = this._i.GetNormal(index);
            return dg.Vector.ByCoordinates(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Returns normal components (x,y,z)
        /// </summary>
        public (double X, double Y, double Z) GetNormalComponents(int index)
        {
            if (index < 0 || index >= NormalCount) throw new ArgumentOutOfRangeException(nameof(index));
            float x = 0, y = 0, z = 0;
            try
            {
                this._i.GetNormalComponents(index, out x, out y, out z);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"GetNormalComponents failed: {ex.Message}", ex);
            }
            return (x, y, z);
        }

        /// <summary>
        /// Returns texture coordinate as DesignScript Point2D-like by using Point.ByCoordinates (x,y)
        /// </summary>
        public dg.Point GetTextureCoordinateAsPoint(int index)
        {
            if (index < 0 || index >= TextureCoordinateCount) throw new ArgumentOutOfRangeException(nameof(index));
            var t = this._i.GetTextureCoordinate(index);
            return dg.Point.ByCoordinates(t.X, t.Y, 0.0);
        }

        /// <summary>
        /// Returns texture coordinate components (u,v)
        /// </summary>
        public (double U, double V) GetTextureCoordinateComponents(int index)
        {
            if (index < 0 || index >= TextureCoordinateCount) throw new ArgumentOutOfRangeException(nameof(index));
            float x = 0, y = 0;
            try
            {
                this._i.GetTextureCoordinateComponents(index, out x, out y);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"GetTextureCoordinateComponents failed: {ex.Message}", ex);
            }
            return (x, y);
        }

        public void Dispose()
        {
            if (_i != null && !_disposed)
            {
                try { Marshal.ReleaseComObject(_i); } catch { }
                _i = null;
                _disposed = true;
            }
        }

        ~IGrid()
        {
            Dispose();
        }
    }
}
