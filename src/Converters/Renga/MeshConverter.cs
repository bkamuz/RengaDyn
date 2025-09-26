using System;
using System.Collections.Generic;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;

namespace DynRenga.Converters.Renga
{
    /// <summary>
    /// Utilities to convert Renga API mesh objects to Dynamo geometry
    /// </summary>
    public static class MeshConverter
    {
        /// <summary>
        /// Convert a single IMesh to a Dynamo Mesh. Returns null if mesh has no vertices.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Mesh ToDynamoMesh(DynRenga.RengaAPI.IMesh mesh)
        {
            if (mesh == null) throw new ArgumentNullException(nameof(mesh));
            return mesh.ToDynamoMesh();
        }

        /// <summary>
        /// Convert all meshes of an exported object into Dynamo meshes and return
        /// a list plus an overall bounding box.
        /// </summary>
        [dr.MultiReturn(new[] { "Meshes", "BoundingBox" })]
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> ExportedObjectToDynamoGeometry(DynRenga.RengaAPI.IExportedObject3D exported)
        {
            if (exported == null) throw new ArgumentNullException(nameof(exported));

            var meshes = new List<dg.Mesh>();
            // use primitive doubles for min/max to avoid creating dg.Point for every vertex
            double minX = double.PositiveInfinity, minY = double.PositiveInfinity, minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity, maxZ = double.NegativeInfinity;
            bool foundValidVertex = false;

            var rengaMeshes = exported.GetMeshes();
            foreach (var rm in rengaMeshes)
            {
                var dm = rm.ToDynamoMesh();
                if (dm != null) meshes.Add(dm);

                // update bounding box using mesh vertices (cached verts + primitive mins/maxs)
                if (dm != null)
                {
                    try
                    {
                        var verts = dm.VertexPositions;
                        foreach (var v in verts)
                        {
                            // ignore NaN/Infinity points which can inflate the bounding box
                            if (double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z)) continue;
                            if (double.IsInfinity(v.X) || double.IsInfinity(v.Y) || double.IsInfinity(v.Z)) continue;

                            foundValidVertex = true;

                            if (v.X < minX) minX = v.X;
                            if (v.Y < minY) minY = v.Y;
                            if (v.Z < minZ) minZ = v.Z;

                            if (v.X > maxX) maxX = v.X;
                            if (v.Y > maxY) maxY = v.Y;
                            if (v.Z > maxZ) maxZ = v.Z;
                        }
                    }
                    catch
                    {
                        // ignore; vertex enumeration may fail for null/invalid mesh
                    }
                }
            }

            dg.BoundingBox bb = null;
            if (meshes.Count > 0 && foundValidVertex)
            {
                // only create bounding box when at least one finite vertex was found
                var min = dg.Point.ByCoordinates(minX, minY, minZ);
                var max = dg.Point.ByCoordinates(maxX, maxY, maxZ);
                bb = dg.BoundingBox.ByGeometry(new List<dg.Point> { min, max });
            }

            return new Dictionary<string, object>
            {
                { "Meshes", meshes },
                { "BoundingBox", bb }
            };
        }

        /// <summary>
        /// Compute the overall bounding box for an exported object using the same
        /// finite-vertex filtering logic as ExportedObjectToDynamoGeometry.
        /// Returns null if no finite vertices found.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.BoundingBox ExportedObjectToBoundingBox(DynRenga.RengaAPI.IExportedObject3D exported)
        {
            if (exported == null) throw new ArgumentNullException(nameof(exported));

            double minX = double.PositiveInfinity, minY = double.PositiveInfinity, minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity, maxY = double.NegativeInfinity, maxZ = double.NegativeInfinity;
            bool foundValidVertex = false;

            var rengaMeshes = exported.GetMeshes();
            foreach (var rm in rengaMeshes)
            {
                var dm = rm.ToDynamoMesh();
                if (dm == null) continue;

                try
                {
                    var verts = dm.VertexPositions;
                    foreach (var v in verts)
                    {
                        if (double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z)) continue;
                        if (double.IsInfinity(v.X) || double.IsInfinity(v.Y) || double.IsInfinity(v.Z)) continue;

                        foundValidVertex = true;

                        if (v.X < minX) minX = v.X;
                        if (v.Y < minY) minY = v.Y;
                        if (v.Z < minZ) minZ = v.Z;

                        if (v.X > maxX) maxX = v.X;
                        if (v.Y > maxY) maxY = v.Y;
                        if (v.Z > maxZ) maxZ = v.Z;
                    }
                }
                catch
                {
                    // ignore
                }
            }

            if (foundValidVertex)
            {
                var min = dg.Point.ByCoordinates(minX, minY, minZ);
                var max = dg.Point.ByCoordinates(maxX, maxY, maxZ);
                return dg.BoundingBox.ByGeometry(new List<dg.Point> { min, max });
            }

            return null;
        }
    }
}
