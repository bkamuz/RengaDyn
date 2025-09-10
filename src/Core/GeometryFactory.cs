using System;
using System.Collections.Generic;
using Renga;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.Core
{
    /// <summary>
    /// Фабрика для создания геометрических объектов Renga
    /// Заменяет дублирование кода в Base.cs
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(false)]
    public static class GeometryFactory
    {
        // ========== 2D ТОЧКИ И ВЕКТОРЫ ==========
        
        /// <summary>
        /// Универсальное создание 2D точек любого типа
        /// </summary>
        /// <typeparam name="T">Тип точки (Point2D или FloatPoint2D)</typeparam>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <returns>Созданная точка</returns>
        public static T CreatePoint2D<T>(double x, double y) where T : new()
        {
            var point = new T();
            
            if (point is Point2D point2D)
            {
                point2D.X = x;
                point2D.Y = y;
            }
            else if (point is FloatPoint2D floatPoint2D)
            {
                floatPoint2D.X = (float)x;
                floatPoint2D.Y = (float)y;
            }
            else
            {
                throw new ArgumentException($"Неподдерживаемый тип 2D точки: {typeof(T).Name}");
            }
            
            return point;
        }
        
        /// <summary>
        /// Универсальное создание 2D векторов любого типа
        /// </summary>
        /// <typeparam name="T">Тип вектора (Vector2D)</typeparam>
        /// <param name="x">Компонент X</param>
        /// <param name="y">Компонент Y</param>
        /// <returns>Созданный вектор</returns>
        public static T CreateVector2D<T>(double x, double y) where T : new()
        {
            var vector = new T();
            
            if (vector is Vector2D vector2D)
            {
                vector2D.X = x;
                vector2D.Y = y;
            }
            else
            {
                throw new ArgumentException($"Неподдерживаемый тип 2D вектора: {typeof(T).Name}");
            }
            
            return vector;
        }
        
        // ========== 3D ТОЧКИ И ВЕКТОРЫ ==========
        
        /// <summary>
        /// Универсальное создание 3D точек любого типа
        /// </summary>
        /// <typeparam name="T">Тип точки (Point3D или FloatPoint3D)</typeparam>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="z">Координата Z</param>
        /// <returns>Созданная точка</returns>
        public static T CreatePoint3D<T>(double x, double y, double z) where T : new()
        {
            var point = new T();
            
            if (point is Point3D point3D)
            {
                point3D.X = x;
                point3D.Y = y;
                point3D.Z = z;
            }
            else if (point is FloatPoint3D floatPoint3D)
            {
                floatPoint3D.X = (float)x;
                floatPoint3D.Y = (float)y;
                floatPoint3D.Z = (float)z;
            }
            else
            {
                throw new ArgumentException($"Неподдерживаемый тип 3D точки: {typeof(T).Name}");
            }
            
            return point;
        }
        
        /// <summary>
        /// Универсальное создание 3D векторов любого типа
        /// </summary>
        /// <typeparam name="T">Тип вектора (Vector3D или FloatVector3D)</typeparam>
        /// <param name="x">Компонент X</param>
        /// <param name="y">Компонент Y</param>
        /// <param name="z">Компонент Z</param>
        /// <returns>Созданный вектор</returns>
        public static T CreateVector3D<T>(double x, double y, double z) where T : new()
        {
            var vector = new T();
            
            if (vector is Vector3D vector3D)
            {
                vector3D.X = x;
                vector3D.Y = y;
                vector3D.Z = z;
            }
            else if (vector is FloatVector3D floatVector3D)
            {
                floatVector3D.X = (float)x;
                floatVector3D.Y = (float)y;
                floatVector3D.Z = (float)z;
            }
            else
            {
                throw new ArgumentException($"Неподдерживаемый тип 3D вектора: {typeof(T).Name}");
            }
            
            return vector;
        }
        
        // ========== ИЗВЛЕЧЕНИЕ КООРДИНАТ ==========
        
        /// <summary>
        /// Универсальное извлечение координат из 2D геометрических объектов
        /// </summary>
        /// <param name="geometry2D">2D геометрический объект</param>
        /// <returns>Словарь с координатами X и Y</returns>
        [dr.MultiReturn(new[] { "X", "Y" })]
        public static Dictionary<string, object> ExtractCoordinates2D(object geometry2D)
        {
            return geometry2D switch
            {
                Point2D p => new Dictionary<string, object> { { "X", p.X }, { "Y", p.Y } },
                FloatPoint2D fp => new Dictionary<string, object> { { "X", (double)fp.X }, { "Y", (double)fp.Y } },
                Vector2D v => new Dictionary<string, object> { { "X", v.X }, { "Y", v.Y } },
                _ => throw new ArgumentException($"Неподдерживаемый тип 2D геометрии: {geometry2D?.GetType().Name ?? "null"}")
            };
        }
        
        /// <summary>
        /// Универсальное извлечение координат из 3D геометрических объектов
        /// </summary>
        /// <param name="geometry3D">3D геометрический объект</param>
        /// <returns>Словарь с координатами X, Y и Z</returns>
        [dr.MultiReturn(new[] { "X", "Y", "Z" })]
        public static Dictionary<string, object> ExtractCoordinates3D(object geometry3D)
        {
            return geometry3D switch
            {
                Point3D p => new Dictionary<string, object> { { "X", p.X }, { "Y", p.Y }, { "Z", p.Z } },
                FloatPoint3D fp => new Dictionary<string, object> { { "X", (double)fp.X }, { "Y", (double)fp.Y }, { "Z", (double)fp.Z } },
                Vector3D v => new Dictionary<string, object> { { "X", v.X }, { "Y", v.Y }, { "Z", v.Z } },
                FloatVector3D fv => new Dictionary<string, object> { { "X", (double)fv.X }, { "Y", (double)fv.Y }, { "Z", (double)fv.Z } },
                _ => throw new ArgumentException($"Неподдерживаемый тип 3D геометрии: {geometry3D?.GetType().Name ?? "null"}")
            };
        }
        
        // ========== СПЕЦИАЛЬНЫЕ ОБЪЕКТЫ ==========
        
        /// <summary>
        /// Создание треугольника триангуляции
        /// </summary>
        /// <param name="v0">Индекс первой вершины</param>
        /// <param name="v1">Индекс второй вершины</param>
        /// <param name="v2">Индекс третьей вершины</param>
        /// <returns>Треугольник триангуляции</returns>
        public static Triangle CreateTriangle(int v0, int v1, int v2)
        {
            return new Triangle
            {
                V0 = Convert.ToUInt32(v0),
                V1 = Convert.ToUInt32(v1),
                V2 = Convert.ToUInt32(v2)
            };
        }
        
        /// <summary>
        /// Извлечение индексов из треугольника
        /// </summary>
        /// <param name="triangle">Треугольник триангуляции</param>
        /// <returns>Словарь с индексами вершин</returns>
        [dr.MultiReturn(new[] { "V0", "V1", "V2" })]
        public static Dictionary<string, object> ExtractTriangleIndices(Triangle triangle)
        {
            return new Dictionary<string, object>
            {
                { "V0", Convert.ToInt32(triangle.V0) },
                { "V1", Convert.ToInt32(triangle.V1) },
                { "V2", Convert.ToInt32(triangle.V2) }
            };
        }
    }
}