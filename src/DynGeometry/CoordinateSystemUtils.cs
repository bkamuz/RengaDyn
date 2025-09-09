using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynGeometry
{
    /// <summary>
    /// Утилиты для работы с системами координат Renga в Dynamo
    /// </summary>
    public class CoordinateSystemUtils
    {
        /// <summary>
        /// Создание стандартной системы координат (мировая СК)
        /// </summary>
        /// <returns>Dynamo система координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem CreateWorldCoordinateSystem()
        {
            return dg.CoordinateSystem.Identity();
        }

        /// <summary>
        /// Создание системы координат по точке и двум векторам
        /// </summary>
        /// <param name="origin">Точка начала</param>
        /// <param name="xAxis">Ось X</param>
        /// <param name="yAxis">Ось Y</param>
        /// <returns>Dynamo система координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem CreateCoordinateSystem(dg.Point origin, dg.Vector xAxis, dg.Vector yAxis)
        {
            if (origin == null || xAxis == null || yAxis == null) return null;

            try
            {
                return dg.CoordinateSystem.ByOriginVectors(origin, xAxis, yAxis);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создание системы координат по точке и двум векторам (3D)
        /// </summary>
        /// <param name="origin">Точка начала</param>
        /// <param name="xAxis">Ось X</param>
        /// <param name="yAxis">Ось Y</param>
        /// <param name="zAxis">Ось Z</param>
        /// <returns>Dynamo система координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem CreateCoordinateSystem3D(dg.Point origin, dg.Vector xAxis, dg.Vector yAxis, dg.Vector zAxis)
        {
            if (origin == null || xAxis == null || yAxis == null || zAxis == null) return null;

            try
            {
                return dg.CoordinateSystem.ByOriginVectors(origin, xAxis, yAxis, zAxis);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создание системы координат по трем точкам
        /// </summary>
        /// <param name="origin">Точка начала</param>
        /// <param name="xPoint">Точка на оси X</param>
        /// <param name="xyPoint">Точка в плоскости XY</param>
        /// <returns>Dynamo система координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem CreateCoordinateSystemByPoints(dg.Point origin, dg.Point xPoint, dg.Point xyPoint)
        {
            if (origin == null || xPoint == null || xyPoint == null) return null;

            try
            {
                dg.Vector xAxis = dg.Vector.ByTwoPoints(origin, xPoint);
                dg.Vector tempYAxis = dg.Vector.ByTwoPoints(origin, xyPoint);
                
                // Create Z-axis as cross product of X and temp Y
                dg.Vector zAxis = xAxis.Cross(tempYAxis);
                zAxis = zAxis.Normalized();
                
                // Create proper Y-axis as cross product of Z and X
                dg.Vector yAxis = zAxis.Cross(xAxis);
                yAxis = yAxis.Normalized();
                
                return dg.CoordinateSystem.ByOriginVectors(origin, xAxis, yAxis, zAxis);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Получение информации о системе координат
        /// </summary>
        /// <param name="coordinateSystem">Система координат</param>
        /// <returns>Строка с информацией о СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetCoordinateSystemInfo(dg.CoordinateSystem coordinateSystem)
        {
            if (coordinateSystem == null) return "Coordinate System is null";

            try
            {
                var info = new System.Text.StringBuilder();
                
                dg.Point origin = coordinateSystem.Origin;
                dg.Vector xAxis = coordinateSystem.XAxis;
                dg.Vector yAxis = coordinateSystem.YAxis;
                dg.Vector zAxis = coordinateSystem.ZAxis;

                info.AppendLine("Coordinate System Information:");
                info.AppendLine($"Origin: ({origin.X:F3}, {origin.Y:F3}, {origin.Z:F3})");
                info.AppendLine($"X-Axis: ({xAxis.X:F3}, {xAxis.Y:F3}, {xAxis.Z:F3})");
                info.AppendLine($"Y-Axis: ({yAxis.X:F3}, {yAxis.Y:F3}, {yAxis.Z:F3})");
                info.AppendLine($"Z-Axis: ({zAxis.X:F3}, {zAxis.Y:F3}, {zAxis.Z:F3})");
                
                // Check if it's orthogonal
                double dotXY = xAxis.Dot(yAxis);
                double dotXZ = xAxis.Dot(zAxis);
                double dotYZ = yAxis.Dot(zAxis);
                
                info.AppendLine($"Is Orthogonal: {Math.Abs(dotXY) < 1e-6 && Math.Abs(dotXZ) < 1e-6 && Math.Abs(dotYZ) < 1e-6}");
                
                return info.ToString();
            }
            catch (System.Exception ex)
            {
                return $"Error getting coordinate system info: {ex.Message}";
            }
        }

        /// <summary>
        /// Создание системы координат по двум точкам (2D)
        /// </summary>
        /// <param name="origin">Точка начала</param>
        /// <param name="xPoint">Точка на оси X</param>
        /// <returns>Dynamo система координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem CreateCoordinateSystem2D(dg.Point origin, dg.Point xPoint)
        {
            if (origin == null || xPoint == null) return null;

            try
            {
                dg.Vector xAxis = dg.Vector.ByTwoPoints(origin, xPoint);
                dg.Vector yAxis = dg.Vector.ByCoordinates(-xAxis.Y, xAxis.X, 0); // Perpendicular to X
                yAxis = yAxis.Normalized();
                
                return dg.CoordinateSystem.ByOriginVectors(origin, xAxis, yAxis);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создание системы координат по углу поворота (2D)
        /// </summary>
        /// <param name="origin">Точка начала</param>
        /// <param name="angleDegrees">Угол поворота в градусах</param>
        /// <returns>Dynamo система координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem CreateCoordinateSystemByAngle(dg.Point origin, double angleDegrees)
        {
            if (origin == null) return null;

            try
            {
                double angleRadians = angleDegrees * Math.PI / 180.0;
                
                dg.Vector xAxis = dg.Vector.ByCoordinates(Math.Cos(angleRadians), Math.Sin(angleRadians), 0);
                dg.Vector yAxis = dg.Vector.ByCoordinates(-Math.Sin(angleRadians), Math.Cos(angleRadians), 0);
                
                return dg.CoordinateSystem.ByOriginVectors(origin, xAxis, yAxis);
            }
            catch
            {
                return null;
            }
        }
    }
}