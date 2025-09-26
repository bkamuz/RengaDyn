using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;


namespace DynRenga.DynObjects.Geometry
{
    /// <summary>
    /// Класс для работы с интерфейсом Renga.IMesh
    /// </summary>
    public class Mesh
    {
        public Renga.IMesh _i;
        /// <summary>
        /// Инициализация класса через интерфейс Renga.IMesh
        /// </summary>
        /// <param name="mesh_obj"></param>
        internal Mesh(object mesh_obj)
        {
            this._i = mesh_obj as Renga.IMesh;
        }
        /// <summary>
        /// Получение количества Grid
        /// </summary>
        /// <returns></returns>
        public int GridCount => this._i.GridCount;
        /// <summary>
        /// Получение типа Мэша
        /// </summary>
        /// <returns></returns>
        public Guid MeshType => this._i.MeshType;
        /// <summary>
        /// Получение отдельной Grid по её порядковому номеру в составе Мэша
        /// </summary>
        /// <param name="grid_index"></param>
        /// <returns></returns>
        public Grid GetGrid(int grid_index)
        {
            if (grid_index < 0 | grid_index > this._i.GridCount) return null;
            else return new Grid(this._i.GetGrid(grid_index));
        }
        /// <summary>
        /// Получение всех Grids
        /// </summary>
        /// <returns></returns>
        public List<Grid> GetGrids()
        {
            List<Grid> grids = new List<Grid>();
            for (int i = 0; i < this._i.GridCount; i++)
            {
                grids.Add(new Grid(this._i.GetGrid(i)));
            }
            return grids;
        }
        
        /// <summary>
        /// Типы MeshType
        /// </summary>
        /// <returns>Словарь с типами MeshType</returns>
        [dr.MultiReturn(new[] { "DoorReveal", "DoorPanel", "DoorTransom",
        "DoorLining","DoorThreshold","DoorPlatband","WindowReveal", "WindowPanel",
            "WindowSill", "WindowOutwardSill","Undefined" })]
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> MeshTypes()
        {
            return new Dictionary<string, object>
            {
                {"DoorReveal",Renga.MeshTypes.DoorReveal},
                {"DoorPanel",Renga.MeshTypes.DoorPanel},
                {"DoorTransom",Renga.MeshTypes.DoorTransom},
                {"DoorLining",Renga.MeshTypes.DoorLining},
                {"DoorThreshold",Renga.MeshTypes.DoorThreshold},
                {"DoorPlatband",Renga.MeshTypes.DoorPlatband},
                {"WindowReveal", Renga.MeshTypes.WindowReveal },
                {"WindowPanel", Renga.MeshTypes.WindowPanel },
                {"WindowSill", Renga.MeshTypes.WindowSill },
                {"WindowOutwardSill", Renga.MeshTypes.WindowOutwardSill},
                {"Undefined",Renga.MeshTypes.Undefined }
            };
        }

        /// <summary>
        /// Convert this Renga.IMesh to a Dynamo Mesh (Autodesk.DesignScript.Geometry.Mesh)
        /// </summary>
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

    }
}
