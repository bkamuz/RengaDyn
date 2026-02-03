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
    /// Класс для работы с интерфейсом Renga.ICurve3D
    /// </summary>
    public class Curve3D
    {
        /// <summary>Внутренний COM-объект Renga.ICurve3D</summary>
        public Renga.ICurve3D _i;
        /// <summary>
        /// Инициация класса из интерфейса Renga.ICurve3D
        /// </summary>
        /// <param name="Curve3D_object"></param>
        internal Curve3D(object Curve3D_object)
        {
            this._i = Curve3D_object as Renga.ICurve3D;
        }
        /// <summary>
        /// Все типы кривой (Curve3DType)
        /// </summary>
        /// <returns></returns>
        [dr.MultiReturn(new[] { "Curve3DType_Undefined", "Curve3DType_LineSegment" ,
        "Curve3DType_Arc","Curve3DType_PolyCurve"})]
        public static Dictionary<string, object> Curve3DTypes()
        {
            return new Dictionary<string, object>()
            {
                {"Curve3DType_Undefined",Renga.Curve3DType.Curve3DType_Undefined },
                {"Curve3DType_LineSegment",Renga.Curve3DType.Curve3DType_LineSegment  },
                {"Curve3DType_Arc",Renga.Curve3DType.Curve3DType_Arc },
                {"Curve3DType_PolyCurve",Renga.Curve3DType.Curve3DType_PolyCurve }
            };
        }
        /// <summary>
        /// Получение Curve3DType
        /// </summary>
        /// <returns></returns>
        public object Curve3DType => this._i.Curve3DType;
        /// <summary>
        /// Получение Curve3DType как строкого значения (наименования типа)
        /// </summary>
        /// <returns></returns>
        public string GetCurve3DTypeAsString()
        {
            IEnumerable<KeyValuePair<string, object>> data = Curve3DTypes().Where(a => (Renga.Curve3DType)a.Value == this._i.Curve3DType);
            if (data.Any()) return data.First().Key;
            else return null;
        }
        /// <summary>
        /// Получение минимального параметра кривой
        /// </summary>
        /// <returns></returns>
        public double MinParameter => this._i.MinParameter;
        /// <summary>
        /// Получение максимального параметра кривой
        /// </summary>
        /// <returns></returns>
        public double MaxParameter => this._i.MaxParameter;
        //functions
        /// <summary>
        /// Получение начальной точки кривой
        /// </summary>
        /// <returns></returns>
        public dg.Point GetBeginPoint()
        {
            Renga.Point3D p3d = this._i.GetBeginPoint();
            return dg.Point.ByCoordinates(p3d.X / 1000.0, p3d.Y / 1000.0, p3d.Z / 1000.0);
        }
        /// <summary>
        /// Получение конечной точки кривой
        /// </summary>
        /// <returns></returns>
        public dg.Point GetEndPoint()
        {
            Renga.Point3D p3d = this._i.GetEndPoint();
            return dg.Point.ByCoordinates(p3d.X / 1000.0, p3d.Y / 1000.0, p3d.Z / 1000.0);
        }
        /// <summary>
        /// Вычисление точки на кривой по заданному значению параметра.
        /// Если param меньше BeginParameter или больше EndParameter, то
        /// param фиксируется до ближайшего допустимого
        /// </summary>
        /// <param name="param">параметр кривой для расчета точки на кривой</param>
        /// <returns></returns>
        public dg.Point GetPointOn(double param)
        {
            Renga.Point3D p3d = this._i.GetPointOn(param);
            return dg.Point.ByCoordinates(p3d.X / 1000.0, p3d.Y / 1000.0, p3d.Z / 1000.0);
        }
        /// <summary>
        /// Вычисление длины кривой (в м.)
        /// </summary>
        /// <returns></returns>
        public double GetLength => this._i.GetLength();
        /// <summary>
        /// Получает ограничивающий BoundingBox вокруг кривой
        /// </summary>
        /// <returns></returns>
        public dg.BoundingBox GetGabarit()
        {
            Renga.Cube bb = this._i.GetGabarit();
            return dg.BoundingBox.ByGeometry(new List<dg.Point> {
                dg.Point.ByCoordinates(bb.MIN.X/1000.0, bb.MIN.Y/1000.0,bb.MIN.Z/1000.0),
                dg.Point.ByCoordinates(bb.MAX.X/1000.0, bb.MAX.Y/1000.0,bb.MAX.Z/1000.0)});
        }
        /// <summary>
        /// Проверка, замкнутая ли кривая
        /// </summary>
        /// <returns></returns>
        public bool IsClosed => this._i.IsClosed();
        /// <summary>
        /// Вычисляет точку на кривой по смешению и расстоянию от кривой
        /// </summary>
        /// <param name="startT">Значение параметра, характеризиющее опорную точку</param>
        /// <param name="distance">Величина смещения вдоль кривой</param>
        /// <param name="direction">Направление смещения. Неотрицательное значение означает, 
        /// что смещение выполняется в сторону увеличения параметра; 
        /// в противном случае оно выполняется в сторону уменьшения параметра.</param>
        /// <returns>Велична параметра для результирующей точки. Для получения точки используйте
        /// ноду GetPointOn</returns>
        public double GetParameterAtDistance(double startT, double distance, int direction)
        {
            return this._i.GetParameterAtDistance(startT, distance, direction);
        }
        /// <summary>
        /// Получение (создание) интерфейса ICurve3D как кривой, обрезанной по данной
        /// </summary>
        /// <param name="T1">Начальная точка обрезки в виде параметра</param>
        /// <param name="T2">Конечная точка обрезки в виде параметра</param>
        /// <param name="sense">Направление обрезаемой (новой) кривой. 1 = направление не менятся;
        /// -1 = направление меняется на противоположное</param>
        /// <returns></returns>
        public Curve3D GetTrimmed(double T1, double T2, int sense)
        {
            return new Curve3D(this._i.GetTrimmed(T1, T2, sense));
        }
        /// <summary>
        /// Вычисляет ближайшую проекцию точки на кривую 
        /// (точка может быть расположена вне кривой)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double PointProjection(dg.Point point)
        {
            Renga.Point3D p3d = new Point3D();
            p3d.X = point.X;
            p3d.Y = point.Y;
            p3d.Z = point.Z;
            return this._i.PointProjection(ref p3d);
        }
        /// <summary>
        /// Получение новой кривой (копии данной), оттрансформированной 
        /// по данному координатному преобразованию
        /// </summary>
        /// <param name="Transfrom">Класс для трехмерного координатного преобразования</param>
        /// <returns></returns>
        public Curve3D GetTransformed(Transform3D Transfrom)
        {
            return new Curve3D(this._i.GetTransformed(Transfrom._i));
        }
        /// <summary>
        /// Получение кривой, смещенной на данный вектор
        /// </summary>
        /// <param name="pOffset">Вектор смещения кривой</param>
        /// <returns></returns>
        public Curve3D GetOffseted(dg.Vector pOffset)
        {
            Renga.Vector3D v3d = new Vector3D();
            v3d.X = pOffset.X;
            v3d.Y = pOffset.Y;
            v3d.Z = pOffset.Z;
            return new Curve3D(this._i.GetOffseted(ref v3d));
        }
        /// <summary>
        /// Преобразование в dynamo PolyCurve
        /// </summary>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре</param>
        /// <returns></returns>
        public dg.PolyCurve ToDynamoPolyCurve(int parts_in_meter = 2)
        {
            Renga.Point3D curve_start_point = this._i.GetBeginPoint();
            Renga.Point3D curve_end_point = this._i.GetEndPoint();

            List<dg.Point> points = new List<dg.Point>();
            double param_start = this._i.PointProjection(ref curve_start_point);
            double param_end = this._i.PointProjection(ref curve_end_point);
            double curve_length = this._i.GetLength()/1000.0;

            int count_parts = Convert.ToInt32(curve_length * parts_in_meter);
            for (int counter_param = 0; counter_param < count_parts; counter_param++)
            {
                double new_param = param_start + (param_end - param_start) / count_parts * counter_param;
                Renga.Point3D calc_point = this._i.GetPointOn(new_param);
                points.Add(dg.Point.ByCoordinates(calc_point.X / 1000.0, calc_point.Y / 1000.0, calc_point.Z / 1000.0));
            }
            return dg.PolyCurve.ByPoints(points);
            
        }

        /// <summary>
        /// Безопасное преобразование в Dynamo PolyCurve с сохранением геометрии
        /// </summary>
        /// <returns></returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.PolyCurve ToDynamoPolyCurveSafe()
        {
            try
            {
                var curveType = this._i.Curve3DType;
                
                switch (curveType)
                {
                    case Renga.Curve3DType.Curve3DType_LineSegment:
                        return ConvertLineSegmentToPolyCurve();
                    
                    case Renga.Curve3DType.Curve3DType_Arc:
                        return ConvertArcToPolyCurve();
                    
                    case Renga.Curve3DType.Curve3DType_PolyCurve:
                        return ConvertPolyCurveToPolyCurve();
                    
                    default:
                        // Для неизвестных типов используем дискретизацию
                        return ConvertByDiscretization();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve3D to Dynamo PolyCurve (safe): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Преобразование LineSegment в PolyCurve
        /// </summary>
        private dg.PolyCurve ConvertLineSegmentToPolyCurve()
        {
            Renga.Point3D startPoint = this._i.GetBeginPoint();
            Renga.Point3D endPoint = this._i.GetEndPoint();
            
            // Создаем Dynamo точки
            dg.Point start = dg.Point.ByCoordinates(startPoint.X / 1000.0, startPoint.Y / 1000.0, startPoint.Z / 1000.0);
            dg.Point end = dg.Point.ByCoordinates(endPoint.X / 1000.0, endPoint.Y / 1000.0, endPoint.Z / 1000.0);
            
            // Создаем линию
            dg.Line line = dg.Line.ByStartPointEndPoint(start, end);
            
            // Создаем PolyCurve из одной линии
            return dg.PolyCurve.ByJoinedCurves(new[] { line });
        }

        /// <summary>
        /// Преобразование Arc в PolyCurve
        /// </summary>
        private dg.PolyCurve ConvertArcToPolyCurve()
        {
            // Для дуги получаем три точки: начало, середину и конец
            Renga.Point3D startPoint = this._i.GetBeginPoint();
            Renga.Point3D endPoint = this._i.GetEndPoint();
            
            // Получаем точку в середине дуги
            double midParam = (this._i.MinParameter + this._i.MaxParameter) / 2.0;
            Renga.Point3D midPoint = this._i.GetPointOn(midParam);
            
            // Создаем Dynamo точки
            dg.Point start = dg.Point.ByCoordinates(startPoint.X / 1000.0, startPoint.Y / 1000.0, startPoint.Z / 1000.0);
            dg.Point mid = dg.Point.ByCoordinates(midPoint.X / 1000.0, midPoint.Y / 1000.0, midPoint.Z / 1000.0);
            dg.Point end = dg.Point.ByCoordinates(endPoint.X / 1000.0, endPoint.Y / 1000.0, endPoint.Z / 1000.0);
            
            try
            {
                // Создаем дугу по трем точкам
                dg.Arc arc = dg.Arc.ByThreePoints(start, mid, end);
                return dg.PolyCurve.ByJoinedCurves(new[] { arc });
            }
            catch
            {
                // Если не удается создать дугу, используем дискретизацию
                return ConvertByDiscretization();
            }
        }

        /// <summary>
        /// Преобразование PolyCurve в PolyCurve
        /// </summary>
        private dg.PolyCurve ConvertPolyCurveToPolyCurve()
        {
            try
            {
                // Получаем интерфейс IPolyCurve3D
                var polyCurveInterface = this._i.GetInterfaceByName("IPolyCurve3D");
                if (polyCurveInterface != null)
                {
                    var polyCurve3D = polyCurveInterface as Renga.IPolyCurve3D;
                    if (polyCurve3D != null)
                    {
                        int segmentCount = polyCurve3D.GetSegmentCount();
                        if (segmentCount > 0)
                        {
                            var allCurves = new List<dg.Curve>();
                            
                            for (int i = 0; i < segmentCount; i++)
                            {
                                var segment = polyCurve3D.GetSegment(i);
                                if (segment != null)
                                {
                                    var segmentCurve3D = new Curve3D(segment);
                                    var segmentCurve = ConvertSingleCurveToDynamo(segmentCurve3D);
                                    if (segmentCurve != null)
                                    {
                                        allCurves.Add(segmentCurve);
                                    }
                                }
                            }
                            
                            if (allCurves.Count > 0)
                            {
                                return dg.PolyCurve.ByJoinedCurves(allCurves);
                            }
                        }
                    }
                }
                
                // Fallback to discretization
                return ConvertByDiscretization();
            }
            catch
            {
                // Fallback to discretization
                return ConvertByDiscretization();
            }
        }

        /// <summary>
        /// Преобразование одиночной кривой в Dynamo кривую
        /// </summary>
        private dg.Curve ConvertSingleCurveToDynamo(Curve3D curve3D)
        {
            var curveType = curve3D._i.Curve3DType;
            
            switch (curveType)
            {
                case Renga.Curve3DType.Curve3DType_LineSegment:
                    return ConvertSingleLineSegment(curve3D);
                
                case Renga.Curve3DType.Curve3DType_Arc:
                    return ConvertSingleArc(curve3D);
                
                default:
                    // Для других типов используем дискретизацию
                    return ConvertSingleByDiscretization(curve3D);
            }
        }

        /// <summary>
        /// Преобразование одиночной LineSegment
        /// </summary>
        private dg.Line ConvertSingleLineSegment(Curve3D curve3D)
        {
            Renga.Point3D startPoint = curve3D._i.GetBeginPoint();
            Renga.Point3D endPoint = curve3D._i.GetEndPoint();
            
            dg.Point start = dg.Point.ByCoordinates(startPoint.X / 1000.0, startPoint.Y / 1000.0, startPoint.Z / 1000.0);
            dg.Point end = dg.Point.ByCoordinates(endPoint.X / 1000.0, endPoint.Y / 1000.0, endPoint.Z / 1000.0);
            
            return dg.Line.ByStartPointEndPoint(start, end);
        }

        /// <summary>
        /// Преобразование одиночной Arc
        /// </summary>
        private dg.Curve ConvertSingleArc(Curve3D curve3D)
        {
            Renga.Point3D startPoint = curve3D._i.GetBeginPoint();
            Renga.Point3D endPoint = curve3D._i.GetEndPoint();
            
            double midParam = (curve3D._i.MinParameter + curve3D._i.MaxParameter) / 2.0;
            Renga.Point3D midPoint = curve3D._i.GetPointOn(midParam);
            
            dg.Point start = dg.Point.ByCoordinates(startPoint.X / 1000.0, startPoint.Y / 1000.0, startPoint.Z / 1000.0);
            dg.Point mid = dg.Point.ByCoordinates(midPoint.X / 1000.0, midPoint.Y / 1000.0, midPoint.Z / 1000.0);
            dg.Point end = dg.Point.ByCoordinates(endPoint.X / 1000.0, endPoint.Y / 1000.0, endPoint.Z / 1000.0);
            
            try
            {
                return dg.Arc.ByThreePoints(start, mid, end);
            }
            catch
            {
                // Fallback to line if arc creation fails
                return dg.Line.ByStartPointEndPoint(start, end);
            }
        }

        /// <summary>
        /// Преобразование одиночной кривой через дискретизацию
        /// </summary>
        private dg.Curve ConvertSingleByDiscretization(Curve3D curve3D)
        {
            Renga.Point3D startPoint = curve3D._i.GetBeginPoint();
            Renga.Point3D endPoint = curve3D._i.GetEndPoint();
            
            dg.Point start = dg.Point.ByCoordinates(startPoint.X / 1000.0, startPoint.Y / 1000.0, startPoint.Z / 1000.0);
            dg.Point end = dg.Point.ByCoordinates(endPoint.X / 1000.0, endPoint.Y / 1000.0, endPoint.Z / 1000.0);
            
            return dg.Line.ByStartPointEndPoint(start, end);
        }

        /// <summary>
        /// Преобразование через дискретизацию (для сложных кривых)
        /// </summary>
        private dg.PolyCurve ConvertByDiscretization()
        {
            Renga.Point3D curve_start_point = this._i.GetBeginPoint();
            Renga.Point3D curve_end_point = this._i.GetEndPoint();

            List<dg.Point> points = new List<dg.Point>();
            double param_start = this._i.PointProjection(ref curve_start_point);
            double param_end = this._i.PointProjection(ref curve_end_point);
            double curve_length = this._i.GetLength() / 1000.0;

            int min_points = 3;
            int parts_in_meter = 2; // Fixed value for discretization
            int count_parts = Math.Max(min_points, Convert.ToInt32(curve_length * parts_in_meter));
            
            for (int counter_param = 0; counter_param < count_parts; counter_param++)
            {
                double new_param = param_start + (param_end - param_start) / count_parts * counter_param;
                Renga.Point3D calc_point = this._i.GetPointOn(new_param);
                dg.Point dynamo_point = dg.Point.ByCoordinates(calc_point.X / 1000.0, calc_point.Y / 1000.0, calc_point.Z / 1000.0);
                
                if (points.Count == 0 || !IsPointDuplicate(points[points.Count - 1], dynamo_point))
                {
                    points.Add(dynamo_point);
                }
            }

            // Добавляем конечную точку
            dg.Point end_point = dg.Point.ByCoordinates(curve_end_point.X / 1000.0, curve_end_point.Y / 1000.0, curve_end_point.Z / 1000.0);
            if (!IsPointDuplicate(points[points.Count - 1], end_point))
            {
                points.Add(end_point);
            }

            if (points.Count < 2)
            {
                throw new InvalidOperationException("Cannot create PolyCurve from less than 2 points");
            }

            return dg.PolyCurve.ByPoints(points);
        }

        /// <summary>
        /// Оптимизированное преобразование закрытых PolyCurve в Dynamo PolyCurve
        /// </summary>
        /// <returns></returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.PolyCurve ToDynamoPolyCurveClosedPolyCurve()
        {
            try
            {
                // Проверяем, является ли это PolyCurve
                if (this._i.Curve3DType == Renga.Curve3DType.Curve3DType_PolyCurve)
                {
                    // Получаем интерфейс IPolyCurve3D
                    var polyCurveInterface = this._i.GetInterfaceByName("IPolyCurve3D");
                    if (polyCurveInterface != null)
                    {
                        var polyCurve3D = polyCurveInterface as Renga.IPolyCurve3D;
                        if (polyCurve3D != null)
                        {
                            int segmentCount = polyCurve3D.GetSegmentCount();
                            if (segmentCount > 0)
                            {
                                var allCurves = new List<dg.Curve>();
                                
                                for (int i = 0; i < segmentCount; i++)
                                {
                                    var segment = polyCurve3D.GetSegment(i);
                                    if (segment != null)
                                    {
                                        var segmentCurve3D = new Curve3D(segment);
                                        var segmentCurve = ConvertSingleCurveToDynamo(segmentCurve3D);
                                        if (segmentCurve != null)
                                        {
                                            allCurves.Add(segmentCurve);
                                        }
                                    }
                                }
                                
                                if (allCurves.Count > 0)
                                {
                                    return dg.PolyCurve.ByJoinedCurves(allCurves);
                                }
                            }
                        }
                    }
                }
                
                // Fallback to safe conversion
                return this.ToDynamoPolyCurveSafe();
            }
            catch (Exception)
            {
                // Fallback to safe conversion
                return this.ToDynamoPolyCurveSafe();
            }
        }

        /// <summary>
        /// Проверка на дублирование точек
        /// </summary>
        /// <param name="point1">Первая точка</param>
        /// <param name="point2">Вторая точка</param>
        /// <returns>True если точки дублируются</returns>
        private bool IsPointDuplicate(dg.Point point1, dg.Point point2)
        {
            if (point1 == null || point2 == null) return false;
            
            double tolerance = 1e-9; // Очень маленькая толерантность
            return Math.Abs(point1.X - point2.X) < tolerance &&
                   Math.Abs(point1.Y - point2.Y) < tolerance &&
                   Math.Abs(point1.Z - point2.Z) < tolerance;
        }
    }
}
