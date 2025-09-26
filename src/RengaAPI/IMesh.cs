using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Typed wrapper for Renga.IMesh
    /// </summary>
    public class IMesh : IDisposable
    {
        public Renga.IMesh _i;
        private bool _disposed = false;

        internal IMesh(object meshObj)
        {
            if (meshObj == null) throw new ArgumentNullException(nameof(meshObj));
            this._i = meshObj as Renga.IMesh;
            if (this._i == null) throw new InvalidCastException("Provided object cannot be cast to Renga.IMesh");
        }

        public int GridCount => this._i.GridCount;

        public Guid MeshType => this._i.MeshType;

        public IGrid GetGrid(int index)
        {
            if (index < 0 || index >= GridCount) throw new ArgumentOutOfRangeException(nameof(index));
            return new IGrid(this._i.GetGrid(index));
        }

        public List<IGrid> GetGrids()
        {
            var list = new List<IGrid>();
            for (int i = 0; i < GridCount; i++) list.Add(GetGrid(i));
            return list;
        }

        [dr.IsVisibleInDynamoLibrary(false)]
        public dg.Mesh ToDynamoMesh()
        {
            var points = new List<dg.Point>();
            var indexes = new List<dg.IndexGroup>();

            for (int g = 0; g < this._i.GridCount; g++)
            {
                var grid = this._i.GetGrid(g);
                int baseIndex = points.Count;
                for (int v = 0; v < grid.VertexCount; v++)
                {
                    var p = grid.GetVertex(v);
                    points.Add(dg.Point.ByCoordinates(p.X / 1000.0, p.Y / 1000.0, p.Z / 1000.0));
                }

                for (int t = 0; t < grid.TriangleCount; t++)
                {
                    var tr = grid.GetTriangle(t);
                    indexes.Add(dg.IndexGroup.ByIndices((uint)(baseIndex + tr.V0), (uint)(baseIndex + tr.V1), (uint)(baseIndex + tr.V2)));
                }
            }

            if (points.Count == 0)
                return null;

            return dg.Mesh.ByPointsFaceIndices(points, indexes);
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

        ~IMesh()
        {
            Dispose();
        }
    }
}
