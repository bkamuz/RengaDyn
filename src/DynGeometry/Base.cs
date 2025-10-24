using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.Core;

namespace DynRenga.DynGeometry
{
    /// <summary>
    /// Рефакторенный класс Base - теперь использует GeometryFactory для устранения дублирования кода
    /// Сохраняет обратную совместимость со старыми методами
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(false)]
    public class Base
    {
        private Base() { }
        
        // ========== НОВЫЕ РЕКОМЕНДУЕМЫЕ МЕТОДЫ (используют GeometryFactory) ==========
        
        /// <summary>
        /// Создание 2D точки с координатами double (рекомендуемый метод)
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <returns>Объект Point2D</returns>
        public static Point2D CreatePoint2D(double x, double y)
        {
            return GeometryFactory.CreatePoint2D<Point2D>(x, y);
        }
        
        /// <summary>
        /// Создание 2D точки с координатами float (рекомендуемый метод)
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <returns>Объект FloatPoint2D</returns>
        public static FloatPoint2D CreateFloatPoint2D(float x, float y)
        {
            return GeometryFactory.CreatePoint2D<FloatPoint2D>(x, y);
        }
        
        /// <summary>
        /// Создание 3D точки с координатами double (рекомендуемый метод)
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="z">Координата Z</param>
        /// <returns>Объект Point3D</returns>
        public static Point3D CreatePoint3D(double x, double y, double z)
        {
            return GeometryFactory.CreatePoint3D<Point3D>(x, y, z);
        }
        
        /// <summary>
        /// Создание 3D точки с координатами float (рекомендуемый метод)
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="z">Координата Z</param>
        /// <returns>Объект FloatPoint3D</returns>
        public static FloatPoint3D CreateFloatPoint3D(float x, float y, float z)
        {
            return GeometryFactory.CreatePoint3D<FloatPoint3D>(x, y, z);
        }
        
        /// <summary>
        /// Универсальное извлечение координат из 2D геометрических объектов (рекомендуемый метод)
        /// </summary>
        /// <param name="geometry2D">2D геометрический объект</param>
        /// <returns>Словарь с координатами X и Y</returns>
        [dr.MultiReturn(new[] { "X", "Y" })]
        public static Dictionary<string, object> ExtractCoords2D(object geometry2D)
        {
            return GeometryFactory.ExtractCoordinates2D(geometry2D);
        }
        
        /// <summary>
        /// Универсальное извлечение координат из 3D геометрических объектов (рекомендуемый метод)
        /// </summary>
        /// <param name="geometry3D">3D геометрический объект</param>
        /// <returns>Словарь с координатами X, Y и Z</returns>
        [dr.MultiReturn(new[] { "X", "Y", "Z" })]
        public static Dictionary<string, object> ExtractCoords3D(object geometry3D)
        {
            return GeometryFactory.ExtractCoordinates3D(geometry3D);
        }
        
        /// <summary>
        /// Создание треугольника триангуляции (рекомендуемый метод)
        /// </summary>
        /// <param name="v0">Индекс первой вершины</param>
        /// <param name="v1">Индекс второй вершины</param>
        /// <param name="v2">Индекс третьей вершины</param>
        /// <returns>Объект Triangle</returns>
        public static Triangle CreateTriangle(int v0, int v1, int v2)
        {
            return GeometryFactory.CreateTriangle(v0, v1, v2);
        }
        
        /// <summary>
        /// Извлечение индексов из треугольника (рекомендуемый метод)
        /// </summary>
        /// <param name="triangle">Треугольник триангуляции</param>
        /// <returns>Словарь с индексами вершин</returns>
        [dr.MultiReturn(new[] { "V0", "V1", "V2" })]
        public static Dictionary<string, object> ExtractTriangleInfo(Triangle triangle)
        {
            return GeometryFactory.ExtractTriangleIndices(triangle);
        }
        
        // ========== УСТАРЕВШИЕ МЕТОДЫ (для обратной совместимости) ==========
        
        /// <summary>
        /// (Устаревший) Точка в 3D с координатами float. Используйте CreateFloatPoint3D
        /// </summary>
        [Obsolete("Используйте CreateFloatPoint3D или GeometryFactory.CreatePoint3D<FloatPoint3D>")]
        public static object SetFloatPoint3D(float X, float Y, float Z)
        {
            return CreateFloatPoint3D(X, Y, Z);
        }
        
        /// <summary>
        /// (Устаревший) Получение координат из 2D объекта по типу. Используйте ExtractCoords2D
        /// </summary>
        [Obsolete("Используйте ExtractCoords2D или GeometryFactory.ExtractCoordinates2D")]
        [dr.MultiReturn(new[] { "X", "Y" })]
        public static Dictionary<string, object> GetCoords_2D(object com_Base2DGeometry, int ObjType)
        {
            try
            {
                return GeometryFactory.ExtractCoordinates2D(com_Base2DGeometry);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// (Устаревший) Получение координат из 3D объекта по типу. Используйте ExtractCoords3D
        /// </summary>
        [Obsolete("Используйте ExtractCoords3D или GeometryFactory.ExtractCoordinates3D")]
        [dr.MultiReturn(new[] { "X", "Y", "Z" })]
        public static Dictionary<string, object> GetCoords_3D(object com_Base3DGeometry, int ObjType)
        {
            try
            {
                return GeometryFactory.ExtractCoordinates3D(com_Base3DGeometry);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// (Устаревший) Точка в 3D с координатами double. Используйте CreatePoint3D
        /// </summary>
        [Obsolete("Используйте CreatePoint3D или GeometryFactory.CreatePoint3D<Point3D>")]
        public static object SetPoint3D(double X, double Y, double Z)
        {
            return CreatePoint3D(X, Y, Z);
        }
        
        /// <summary>
        /// (Устаревший) Точка в 2D с координатами float. Используйте CreateFloatPoint2D
        /// </summary>
        [Obsolete("Используйте CreateFloatPoint2D или GeometryFactory.CreatePoint2D<FloatPoint2D>")]
        public static object SetFloatPoint2D(float X, float Y)
        {
            return CreateFloatPoint2D(X, Y);
        }
        
        /// <summary>
        /// (Устаревший) Точка в 2D с координатами double. Используйте CreatePoint2D
        /// </summary>
        [Obsolete("Используйте CreatePoint2D или GeometryFactory.CreatePoint2D<Point2D>")]
        public static object SetPoint2D(double X, double Y)
        {
            return CreatePoint2D(X, Y);
        }
        
        /// <summary>
        /// (Устаревший) Вектор в 3D с координатами float
        /// </summary>
        [Obsolete("Используйте GeometryFactory.CreateVector3D<FloatVector3D>")]
        public static object SetFloatVector3D(float X, float Y, float Z)
        {
            return GeometryFactory.CreateVector3D<FloatVector3D>(X, Y, Z);
        }
        
        /// <summary>
        /// (Устаревший) Вектор в 2D с координатами double
        /// </summary>
        [Obsolete("Используйте GeometryFactory.CreateVector2D<Vector2D>")]
        public static object SetVector2D(double X, double Y)
        {
            return GeometryFactory.CreateVector2D<Vector2D>(X, Y);
        }
        
        /// <summary>
        /// (Устаревший) Вектор в 3D с координатами double
        /// </summary>
        [Obsolete("Используйте GeometryFactory.CreateVector3D<Vector3D>")]
        public static object SetVector3D(double X, double Y, double Z)
        {
            return GeometryFactory.CreateVector3D<Vector3D>(X, Y, Z);
        }
        
        /// <summary>
        /// (Устаревший) Треугольник триангуляции. Используйте CreateTriangle
        /// </summary>
        [Obsolete("Используйте CreateTriangle или GeometryFactory.CreateTriangle")]
        public static object SetTriangle(int V0, int V1, int V2)
        {
            return CreateTriangle(V0, V1, V2);
        }
        
        /// <summary>
        /// (Устаревший) Получение индексов треугольника. Используйте ExtractTriangleInfo
        /// </summary>
        [Obsolete("Используйте ExtractTriangleInfo или GeometryFactory.ExtractTriangleIndices")]
        [dr.MultiReturn(new[] { "V0", "V1", "V2" })]
        public static Dictionary<string, object> GetTriangleInfo(object com_Triangle)
        {
            if (com_Triangle is Triangle triangle)
            {
                return GeometryFactory.ExtractTriangleIndices(triangle);
            }
            
            return new Dictionary<string, object>
            {
                { "V0", -1 },
                { "V1", -1 },
                { "V2", -1 }
            };
        }
    }
}
