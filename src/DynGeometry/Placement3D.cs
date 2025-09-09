using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynGeometry
{
    /// <summary>
    /// Класс для работы с интерфейсом Renga.IPlacement3D - локальной системой координат 
    /// в трехмерном пространстве
    /// </summary>
    public class Placement3D
    {
        public Renga.IPlacement3D _i;
        public Renga.Placement3D _placement; // Store the struct directly
        
        internal Placement3D (object Placement3D_object)
        {
            this._i = Placement3D_object as Renga.IPlacement3D;
        }
        
        // Constructor for Renga.Placement3D struct
        internal Placement3D (Renga.Placement3D placement)
        {
            this._placement = placement;
            this._i = null; // No COM interface for struct
        }
        //properties
        /// <summary>
        /// Получение точки начала СК
        /// </summary>
        /// <returns></returns>
        public dg.Point Origin()
        {
            Renga.Point3D p;
            if (this._i != null)
            {
                p = this._i.Origin;
            }
            else
            {
                p = this._placement.Origin;
            }
            return dg.Point.ByCoordinates(p.X / 1000.0, p.Y / 1000.0, p.Z / 1000.0);
        }
        /// <summary>
        /// Получение вектора X
        /// </summary>
        /// <returns></returns>
        public dg.Vector AxisX()
        {
            Renga.Vector3D vX;
            if (this._i != null)
            {
                vX = this._i.AxisX;
            }
            else
            {
                vX = this._placement.xAxis;
            }
            return dg.Vector.ByCoordinates(vX.X / 1000.0, vX.Y / 1000.0, vX.Z / 1000.0);
        }
        /// <summary>
        /// Получение вектора Y
        /// </summary>
        /// <returns></returns>
        public dg.Vector AxisY()
        {
            Renga.Vector3D vY;
            if (this._i != null)
            {
                vY = this._i.AxisY;
            }
            else
            {
                // Calculate Y-axis as cross product of Z and X axes
                var zAxis = this._placement.zAxis;
                var xAxis = this._placement.xAxis;
                vY = new Renga.Vector3D
                {
                    X = zAxis.Y * xAxis.Z - zAxis.Z * xAxis.Y,
                    Y = zAxis.Z * xAxis.X - zAxis.X * xAxis.Z,
                    Z = zAxis.X * xAxis.Y - zAxis.Y * xAxis.X
                };
            }
            return dg.Vector.ByCoordinates(vY.X / 1000.0, vY.Y / 1000.0, vY.Z / 1000.0);
        }
        /// <summary>
        /// Получение вектора Z
        /// </summary>
        /// <returns></returns>
        public dg.Vector AxisZ()
        {
            Renga.Vector3D vZ;
            if (this._i != null)
            {
                vZ = this._i.AxisZ;
            }
            else
            {
                vZ = this._placement.zAxis;
            }
            return dg.Vector.ByCoordinates(vZ.X / 1000.0, vZ.Y / 1000.0, vZ.Z / 1000.0);
        }
        //functions
        /// <summary>
        /// Преобразование в представление Dynamo CoordinateSystem
        /// </summary>
        /// <returns></returns>
        public dg.CoordinateSystem ToDynamoCoordinateSystem()
        {
            return dg.CoordinateSystem.ByOriginVectors(this.Origin(), this.AxisX(), this.AxisY(), this.AxisZ());
        }
        /// <summary>
        /// Проверка, является ли ортогональной данная СК
        /// </summary>
        /// <returns></returns>
        public bool IsOrthogonal => this._i.IsOrthogonal();
        /// <summary>
        /// Проверка, является ли нормальной данная СК
        /// </summary>
        /// <returns></returns>
        public bool IsNormal => this._i.IsNormal();
        /// <summary>
        /// Проверка, является ли данная СК левосторонней
        /// </summary>
        /// <returns></returns>
        public bool IsLeft => this._i.IsLeft();
        /// <summary>
        /// Получение Renga.ITransform3D из текущей СК в глобальную
        /// </summary>
        /// <returns></returns>
        public Transform3D GetTransformFrom => new Transform3D(this._i.GetTransformFrom());
        /// <summary>
        /// Получение Renga.ITransform3D из глобальной СК в текущую
        /// </summary>
        /// <returns></returns>
        public Transform2D GetTransformInto => new Transform2D(this._i.GetTransformInto());
        /// <summary>
        /// Перемещение на указанный вектор
        /// </summary>
        /// <param name="vector_to_moving"></param>
        public void Move (dg.Vector vector_to_moving)
        {
            Renga.Vector3D vec;
            vec.X = vector_to_moving.X * 1000.0;
            vec.Y = vector_to_moving.Y * 1000.0;
            vec.Z = vector_to_moving.Z * 1000.0;
            this._i.Move(vec);
        }
        /// <summary>
        /// Поворот вокруг указанной оси (вектора)
        /// </summary>
        /// <param name="vector_axis_rotate"></param>
        public void Rotate(dg.Vector vector_axis_rotate)
        {
            Renga.Vector3D vec;
            vec.X = vector_axis_rotate.X * 1000.0;
            vec.Y = vector_axis_rotate.Y * 1000.0;
            vec.Z = vector_axis_rotate.Z * 1000.0;
            this._i.Move(vec);
        }
        /// <summary>
        /// Применение трансформации через интерфейс Renga.ITransform3D
        /// </summary>
        /// <param name="transform"></param>
        public void Transform (DynGeometry.Transform3D transform)
        {
            this._i.Transform(transform._i);
        }
        /// <summary>
        /// Получение копии текущей СК
        /// </summary>
        /// <returns></returns>
        public Placement3D GetCopy() => new Placement3D(this._i.GetCopy());
        
        // ========== STATIC HELPER METHODS FOR DYNAMO ==========
        
        /// <summary>
        /// Создание Placement3D из точки и двух векторов
        /// </summary>
        /// <param name="origin">Точка происхождения (x, y, z)</param>
        /// <param name="xAxis">X-ось (x, y, z)</param>
        /// <param name="zAxis">Z-ось (x, y, z)</param>
        /// <returns>Placement3D объект и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Placement3D", "DebugInfo" })]
        public static Dictionary<string, object> ByOriginAndAxes(object origin, object xAxis, object zAxis)
        {
            var debugInfo = "🔧 Creating Placement3D from origin and axes...\n";
            
            try
            {
                var placement = new Renga.Placement3D();
                
                // Set origin
                if (origin is dg.Point point)
                {
                    placement.Origin = new Renga.Point3D
                    {
                        X = point.X * 1000.0,
                        Y = point.Y * 1000.0,
                        Z = point.Z * 1000.0
                    };
                    debugInfo += $"✅ Origin set from Dynamo Point: ({point.X:F2}, {point.Y:F2}, {point.Z:F2}) → ({point.X * 1000.0:F1}, {point.Y * 1000.0:F1}, {point.Z * 1000.0:F1}) mm\n";
                }
                else if (origin is Renga.Point3D originPoint)
                {
                    placement.Origin = originPoint;
                    debugInfo += $"✅ Origin set from Renga Point3D: ({originPoint.X:F1}, {originPoint.Y:F1}, {originPoint.Z:F1}) mm\n";
                }
                else
                {
                    debugInfo += $"⚠️ Origin type not supported: {origin?.GetType().Name ?? "null"}\n";
                }
                
                // Set X-axis
                if (xAxis is dg.Vector vector)
                {
                    placement.xAxis = new Renga.Vector3D
                    {
                        X = vector.X * 1000.0,
                        Y = vector.Y * 1000.0,
                        Z = vector.Z * 1000.0
                    };
                    debugInfo += $"✅ X-Axis set from Dynamo Vector: ({vector.X:F2}, {vector.Y:F2}, {vector.Z:F2}) → ({vector.X * 1000.0:F1}, {vector.Y * 1000.0:F1}, {vector.Z * 1000.0:F1}) mm\n";
                }
                else if (xAxis is Renga.Vector3D xAxisVector)
                {
                    placement.xAxis = xAxisVector;
                    debugInfo += $"✅ X-Axis set from Renga Vector3D: ({xAxisVector.X:F1}, {xAxisVector.Y:F1}, {xAxisVector.Z:F1}) mm\n";
                }
                else
                {
                    debugInfo += $"⚠️ X-Axis type not supported: {xAxis?.GetType().Name ?? "null"}\n";
                }
                
                // Set Z-axis
                if (zAxis is dg.Vector zVector)
                {
                    placement.zAxis = new Renga.Vector3D
                    {
                        X = zVector.X * 1000.0,
                        Y = zVector.Y * 1000.0,
                        Z = zVector.Z * 1000.0
                    };
                    debugInfo += $"✅ Z-Axis set from Dynamo Vector: ({zVector.X:F2}, {zVector.Y:F2}, {zVector.Z:F2}) → ({zVector.X * 1000.0:F1}, {zVector.Y * 1000.0:F1}, {zVector.Z * 1000.0:F1}) mm\n";
                }
                else if (zAxis is Renga.Vector3D zAxisVector)
                {
                    placement.zAxis = zAxisVector;
                    debugInfo += $"✅ Z-Axis set from Renga Vector3D: ({zAxisVector.X:F1}, {zAxisVector.Y:F1}, {zAxisVector.Z:F1}) mm\n";
                }
                else
                {
                    debugInfo += $"⚠️ Z-Axis type not supported: {zAxis?.GetType().Name ?? "null"}\n";
                }
                
                var placement3D = new Placement3D(placement);
                debugInfo += $"✅ Placement3D created successfully!\n";
                debugInfo += $"📍 Final Origin: ({placement.Origin.X:F1}, {placement.Origin.Y:F1}, {placement.Origin.Z:F1}) mm\n";
                debugInfo += $"📐 Final X-Axis: ({placement.xAxis.X:F1}, {placement.xAxis.Y:F1}, {placement.xAxis.Z:F1}) mm\n";
                debugInfo += $"📐 Final Z-Axis: ({placement.zAxis.X:F1}, {placement.zAxis.Y:F1}, {placement.zAxis.Z:F1}) mm\n";
                
                return new Dictionary<string, object>
                {
                    { "Placement3D", placement3D },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Placement3D creation failed!\n";
                debugInfo += $"Origin Type: {origin?.GetType().Name ?? "null"}\n";
                debugInfo += $"X-Axis Type: {xAxis?.GetType().Name ?? "null"}\n";
                debugInfo += $"Z-Axis Type: {zAxis?.GetType().Name ?? "null"}\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Placement3D", null },
                    { "DebugInfo", debugInfo }
                };
            }
        }
        
        /// <summary>
        /// Создание Placement3D из координат
        /// </summary>
        /// <param name="originX">X координата происхождения</param>
        /// <param name="originY">Y координата происхождения</param>
        /// <param name="originZ">Z координата происхождения</param>
        /// <param name="xAxisX">X компонент X-оси</param>
        /// <param name="xAxisY">Y компонент X-оси</param>
        /// <param name="xAxisZ">Z компонент X-оси</param>
        /// <param name="zAxisX">X компонент Z-оси</param>
        /// <param name="zAxisY">Y компонент Z-оси</param>
        /// <param name="zAxisZ">Z компонент Z-оси</param>
        /// <returns>Placement3D объект</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Placement3D ByCoordinates(double originX, double originY, double originZ,
            double xAxisX, double xAxisY, double xAxisZ,
            double zAxisX, double zAxisY, double zAxisZ)
        {
            try
            {
                var placement = new Renga.Placement3D();
                
                placement.Origin = new Renga.Point3D
                {
                    X = originX * 1000.0,
                    Y = originY * 1000.0,
                    Z = originZ * 1000.0
                };
                
                placement.xAxis = new Renga.Vector3D
                {
                    X = xAxisX * 1000.0,
                    Y = xAxisY * 1000.0,
                    Z = xAxisZ * 1000.0
                };
                
                placement.zAxis = new Renga.Vector3D
                {
                    X = zAxisX * 1000.0,
                    Y = zAxisY * 1000.0,
                    Z = zAxisZ * 1000.0
                };
                
                return new Placement3D(placement);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Создание стандартного Placement3D (ориентация по умолчанию)
        /// </summary>
        /// <param name="originX">X координата происхождения</param>
        /// <param name="originY">Y координата происхождения</param>
        /// <param name="originZ">Z координата происхождения</param>
        /// <returns>Placement3D объект и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Placement3D", "DebugInfo" })]
        public static Dictionary<string, object> ByOrigin(double originX, double originY, double originZ)
        {
            try
            {
                // Create a COM object that implements IPlacement3D
                var placementCom = new Renga.Placement3D();
                
                placementCom.Origin = new Renga.Point3D
                {
                    X = originX * 1000.0,
                    Y = originY * 1000.0,
                    Z = originZ * 1000.0
                };
                
                // Default orientation: X=(1,0,0), Z=(0,0,1)
                placementCom.xAxis = new Renga.Vector3D
                {
                    X = 1000.0,
                    Y = 0.0,
                    Z = 0.0
                };
                
                placementCom.zAxis = new Renga.Vector3D
                {
                    X = 0.0,
                    Y = 0.0,
                    Z = 1000.0
                };
                
                // Create a wrapper that stores the struct directly
                var result = new Placement3D(placementCom);
                var debugInfo = $"✅ Placement3D created successfully!\n" +
                               $"Origin: ({originX}, {originY}, {originZ})\n" +
                               $"X-Axis: (1, 0, 0)\n" +
                               $"Z-Axis: (0, 0, 1)\n" +
                               $"Placement3D._i is null: {result._i == null}\n" +
                               $"Placement3D type: {placementCom.GetType().Name}";
                
                return new Dictionary<string, object>
                {
                    { "Placement3D", result },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                var errorDebugInfo = $"❌ Placement3D creation failed!\n" +
                                    $"Origin: ({originX}, {originY}, {originZ})\n" +
                                    $"Error: {ex.Message}\n" +
                                    $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Placement3D", null },
                    { "DebugInfo", errorDebugInfo }
                };
            }
        }
        
        /// <summary>
        /// Создание Placement3D из Dynamo CoordinateSystem
        /// </summary>
        /// <param name="coordinateSystem">Dynamo CoordinateSystem</param>
        /// <returns>Placement3D объект</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Placement3D ByCoordinateSystem(dg.CoordinateSystem coordinateSystem)
        {
            try
            {
                var placement = new Renga.Placement3D();
                
                // Set origin
                var origin = coordinateSystem.Origin;
                placement.Origin = new Renga.Point3D
                {
                    X = origin.X * 1000.0,
                    Y = origin.Y * 1000.0,
                    Z = origin.Z * 1000.0
                };
                
                // Set X-axis
                var xAxis = coordinateSystem.XAxis;
                placement.xAxis = new Renga.Vector3D
                {
                    X = xAxis.X * 1000.0,
                    Y = xAxis.Y * 1000.0,
                    Z = xAxis.Z * 1000.0
                };
                
                // Set Z-axis
                var zAxis = coordinateSystem.ZAxis;
                placement.zAxis = new Renga.Vector3D
                {
                    X = zAxis.X * 1000.0,
                    Y = zAxis.Y * 1000.0,
                    Z = zAxis.Z * 1000.0
                };
                
                return new Placement3D(placement);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Создание Placement3D из точки и направления
        /// </summary>
        /// <param name="origin">Точка происхождения</param>
        /// <param name="direction">Направление (будет использовано как X-ось)</param>
        /// <returns>Placement3D объект</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Placement3D ByOriginAndDirection(object origin, object direction)
        {
            try
            {
                var placement = new Renga.Placement3D();
                
                // Set origin
                if (origin is dg.Point point)
                {
                    placement.Origin = new Renga.Point3D
                    {
                        X = point.X * 1000.0,
                        Y = point.Y * 1000.0,
                        Z = point.Z * 1000.0
                    };
                }
                else if (origin is Renga.Point3D originPoint)
                {
                    placement.Origin = originPoint;
                }
                
                // Set X-axis from direction
                if (direction is dg.Vector vector)
                {
                    placement.xAxis = new Renga.Vector3D
                    {
                        X = vector.X * 1000.0,
                        Y = vector.Y * 1000.0,
                        Z = vector.Z * 1000.0
                    };
                }
                else if (direction is Renga.Vector3D directionVector)
                {
                    placement.xAxis = directionVector;
                }
                
                // Set Z-axis as up (0,0,1) - will be adjusted if needed
                placement.zAxis = new Renga.Vector3D
                {
                    X = 0.0,
                    Y = 0.0,
                    Z = 1000.0
                };
                
                return new Placement3D(placement);
            }
            catch
            {
                return null;
            }
        }

    }
}
