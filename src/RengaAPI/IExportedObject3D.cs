using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynObjects;
using DynRenga.DynObjects.Geometry;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IExportedObject3D interface
    /// </summary>
    public class IExportedObject3D : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IExportedObject3D
        /// </summary>
        public Renga.IExportedObject3D _i;

        private bool _disposed = false;

        /// <summary>
        /// Constructor from COM object
        /// </summary>
        /// <param name="exportedObject3D">Renga.IExportedObject3D or object</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IExportedObject3D(object exportedObject3D)
        {
            if (exportedObject3D == null)
                throw new ArgumentNullException(nameof(exportedObject3D));

            this._i = exportedObject3D as Renga.IExportedObject3D;
            if (this._i == null)
                throw new InvalidCastException("Provided object cannot be cast to Renga.IExportedObject3D");
        }

        /// <summary>
        /// Check validity
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public int ModelObjectId
        {
            get
            {
                if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
                return this._i.ModelObjectId;
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid ModelObjectType
        {
            get
            {
                if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
                return this._i.ModelObjectType;
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public string ModelObjectTypeS
        {
            get
            {
                if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
                return this._i.ModelObjectTypeS;
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public int MeshCount
        {
            get
            {
                if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
                return this._i.MeshCount;
            }
        }

        /// <summary>
        /// Get mesh by index as DynRenga.RengaAPI.IMesh wrapper
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IMesh GetMesh(int index)
        {
            if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
            if (index < 0 || index >= MeshCount) throw new ArgumentOutOfRangeException(nameof(index));

            var mesh = this._i.GetMesh(index);
            return new IMesh(mesh);
        }

        /// <summary>
        /// Get all meshes
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IMesh> GetMeshes()
        {
            if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
            var list = new List<IMesh>();
            for (int i = 0; i < MeshCount; i++) list.Add(GetMesh(i));
            return list;
        }

        /// <summary>
        /// Convert to Dynamo geometry (Meshes and BoundingBox)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Meshes_group", "BoundingBox" })]
        public Dictionary<string, object> GetDynamoGeometry()
        {
            if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");

            var meshes = new List<dg.Mesh>();
            dg.Point min = dg.Point.ByCoordinates(double.MaxValue, double.MaxValue, double.MaxValue);
            dg.Point max = dg.Point.ByCoordinates(double.MinValue, double.MinValue, double.MinValue);

            for (int m = 0; m < MeshCount; m++)
            {
                var meshObj = this._i.GetMesh(m);
                // Use typed wrapper
                var meshWrapper = new IMesh(meshObj);
                var dynMesh = meshWrapper.ToDynamoMesh();
                if (dynMesh != null) meshes.Add(dynMesh);

                // compute bbox from mesh vertices via grids
                for (int gi = 0; gi < meshWrapper.GridCount; gi++)
                {
                    var grid = meshWrapper.GetGrid(gi);
                    for (int vi = 0; vi < grid.VertexCount; vi++)
                    {
                        var pt = grid.GetVertexAsPoint(vi);
                        if (pt.X < min.X) min = dg.Point.ByCoordinates(pt.X, min.Y, min.Z);
                        if (pt.Y < min.Y) min = dg.Point.ByCoordinates(min.X, pt.Y, min.Z);
                        if (pt.Z < min.Z) min = dg.Point.ByCoordinates(min.X, min.Y, pt.Z);
                        if (pt.X > max.X) max = dg.Point.ByCoordinates(pt.X, max.Y, max.Z);
                        if (pt.Y > max.Y) max = dg.Point.ByCoordinates(max.X, pt.Y, max.Z);
                        if (pt.Z > max.Z) max = dg.Point.ByCoordinates(max.X, max.Y, pt.Z);
                    }
                    grid.Dispose();
                }

                meshWrapper.Dispose();
            }

            return new Dictionary<string, object>
            {
                { "Meshes_group", meshes },
                { "BoundingBox", dg.BoundingBox.ByGeometry(new List<dg.Point> { min, max }) }
            };
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetTrianglesCount()
        {
            if (this._i == null) throw new InvalidOperationException("ExportedObject3D interface is not initialized.");
            int count = 0;
            for (int mi = 0; mi < MeshCount; mi++)
            {
                var mesh = this._i.GetMesh(mi);
                for (int gi = 0; gi < mesh.GridCount; gi++)
                {
                    var grid = mesh.GetGrid(gi);
                    count += grid.TriangleCount;
                }
            }
            return count;
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

        ~IExportedObject3D()
        {
            Dispose();
        }
    }
}
