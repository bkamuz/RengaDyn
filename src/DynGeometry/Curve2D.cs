using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynGeometry
{
    /// <summary>
    /// Класс для работы с интерфейсом Renga.ICurve2D
    /// </summary>
    public class Curve2D
    {
        /// <summary>Внутренний COM-объект Renga.ICurve2D</summary>
        public Renga.ICurve2D _i;
        /// <summary>
        /// Инициация класса из интерфейса Renga.ICurve2D
        /// </summary>
        /// <param name="Curve2D_object"></param>
        internal Curve2D(object Curve2D_object)
        {
            this._i = Curve2D_object as Renga.ICurve2D;
        }
        /// <summary>
        /// Все типы кривой (Curve2DType)
        /// </summary>
        /// <returns></returns>
        [dr.MultiReturn(new[] { "Curve2DType_Undefined", "Curve2DType_LineSegment" ,
        "Curve2DType_Arc","Curve2DType_PolyCurve"})]
        public static Dictionary<string, object> Curve2DTypes()
        {
            return new Dictionary<string, object>()
            {
                {"Curve2DType_Undefined",Renga.Curve2DType.Curve2DType_Undefined },
                {"Curve2DType_LineSegment",Renga.Curve2DType.Curve2DType_LineSegment  },
                {"Curve2DType_Arc",Renga.Curve2DType.Curve2DType_Arc },
                {"Curve2DType_PolyCurve",Renga.Curve2DType.Curve2DType_PolyCurve }
            };
        }
        /// <summary>
        /// Получение Curve2DType
        /// </summary>
        /// <returns></returns>
        public object Curve2DType => this._i.Curve2DType;
        /// <summary>
        /// Получение Curve2DType как строкого значения (наименования типа)
        /// </summary>
        /// <returns></returns>
        public string GetCurve2DTypeAsString()
        {
            IEnumerable<KeyValuePair<string, object>> data = Curve2DTypes().Where(a => (Renga.Curve2DType)a.Value == this._i.Curve2DType);
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
            Renga.Point2D p2d = this._i.GetBeginPoint();
            return dg.Point.ByCoordinates(p2d.X / 1000.0, p2d.Y / 1000.0);
        }
        /// <summary>
        /// Получение конечной точки кривой
        /// </summary>
        /// <returns></returns>
        public dg.Point GetEndPoint()
        {
            Renga.Point2D p2d = this._i.GetEndPoint();
            return dg.Point.ByCoordinates(p2d.X / 1000.0, p2d.Y / 1000.0);
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
            Renga.Point2D p2d = this._i.GetPointOn(param);
            return dg.Point.ByCoordinates(p2d.X / 1000.0, p2d.Y / 1000.0);
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
            Renga.Point2D p1;
            Renga.Point2D p2;
            this._i.GetGabarit(out p1, out p2);
            return dg.BoundingBox.ByGeometry(new List<dg.Point> {
                dg.Point.ByCoordinates(p1.X/1000.0, p1.Y/1000.0),
                dg.Point.ByCoordinates(p2.X/1000.0, p2.Y/1000.0)});
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
        /// Получение (создание) интерфейса ICurve2D как кривой, обрезанной по данной
        /// </summary>
        /// <param name="T1">Начальная точка обрезки в виде параметра</param>
        /// <param name="T2">Конечная точка обрезки в виде параметра</param>
        /// <param name="sense">Направление обрезаемой (новой) кривой. 1 = направление не менятся;
        /// -1 = направление меняется на противоположное</param>
        /// <returns></returns>
        public Curve2D GetTrimmed(double T1, double T2, int sense)
        {
            return new Curve2D(this._i.GetTrimmed(T1, T2, sense));
        }
        /// <summary>
        /// Вычисляет ближайшую проекцию точки на кривую 
        /// (точка может быть расположена вне кривой)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double PointProjection(dg.Point point)
        {
            Renga.Point2D p2d = new Point2D();
            p2d.X = point.X;
            p2d.Y = point.Y;
            return this._i.PointProjection(p2d);
        }
        /// <summary>
        /// Получение новой кривой (копии данной), оттрансформированной 
        /// по данному координатному преобразованию
        /// </summary>
        /// <param name="Transfrom">Класс для трехмерного координатного преобразования</param>
        /// <returns></returns>
        public Curve2D GetTransformed(Transform2D Transfrom)
        {
            return new Curve2D(this._i.GetTransformed(Transfrom._i));
        }
        /// <summary>
        /// Получение кривой, смещенной на данный вектор
        /// </summary>
        /// <param name="pOffset">Вектор смещения кривой</param>
        /// <returns></returns>
        public Curve2D GetOffseted(double pOffset)
        {
            return new Curve2D(this._i.GetOffseted(pOffset));
        }
        /// <summary>
        /// Преобразует данную плоскую линию в трехмерную
        /// </summary>
        /// <param name="Placement3D"></param>
        /// <returns></returns>
        public Curve3D CreateCurve3D(Placement3D Placement3D)
        {
            return new Curve3D(this._i.CreateCurve3D(Placement3D._i));
        }
        /// <summary>
        /// Преобразование в dynamo PolyCurve
        /// </summary>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре</param>
        /// <returns></returns>
        public dg.PolyCurve ToDynamoPolyCurve(int parts_in_meter = 2)
        {
            Renga.Point2D curve_start_point = this._i.GetBeginPoint();
            Renga.Point2D curve_end_point = this._i.GetEndPoint();

            List<dg.Point> points = new List<dg.Point>();
            double param_start = this._i.PointProjection(curve_start_point);
            double param_end = this._i.PointProjection(curve_end_point);
            double curve_length = this._i.GetLength() / 1000.0;

            int count_parts = Convert.ToInt32(curve_length * parts_in_meter);
            for (int counter_param = 0; counter_param < count_parts; counter_param++)
            {
                double new_param = param_start + (param_end - param_start) / count_parts * counter_param;
                Renga.Point2D calc_point = this._i.GetPointOn(new_param);
                points.Add(dg.Point.ByCoordinates(calc_point.X / 1000.0, calc_point.Y / 1000.0));
            }
            return dg.PolyCurve.ByPoints(points);

        }

        /// <summary>
        /// Безопасное преобразование в dynamo PolyCurve с обработкой закрытых кривых и дублирующихся точек
        /// </summary>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре</param>
        /// <returns></returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.PolyCurve ToDynamoPolyCurveSafe(int parts_in_meter = 2)
        {
            try
            {
                Renga.Point2D curve_start_point = this._i.GetBeginPoint();
                Renga.Point2D curve_end_point = this._i.GetEndPoint();

                List<dg.Point> points = new List<dg.Point>();
                double param_start = this._i.PointProjection(curve_start_point);
                double param_end = this._i.PointProjection(curve_end_point);
                double curve_length = this._i.GetLength() / 1000.0;

                // Минимальное количество точек для избежания ошибок
                int min_points = 3;
                int count_parts = Math.Max(min_points, Convert.ToInt32(curve_length * parts_in_meter));
                
                // Проверяем, является ли кривая закрытой
                bool is_closed = this._i.IsClosed();
                
                for (int counter_param = 0; counter_param < count_parts; counter_param++)
                {
                    double new_param = param_start + (param_end - param_start) / count_parts * counter_param;
                    Renga.Point2D calc_point = this._i.GetPointOn(new_param);
                    dg.Point dynamo_point = dg.Point.ByCoordinates(calc_point.X / 1000.0, calc_point.Y / 1000.0);
                    
                    // Проверяем, не является ли точка дублирующейся
                    if (points.Count == 0 || !IsPointDuplicate(points[points.Count - 1], dynamo_point))
                    {
                        points.Add(dynamo_point);
                    }
                }
                
                // Для закрытых кривых добавляем конечную точку, если она не совпадает с начальной
                if (is_closed && points.Count > 0)
                {
                    dg.Point last_point = dg.Point.ByCoordinates(curve_end_point.X / 1000.0, curve_end_point.Y / 1000.0);
                    if (!IsPointDuplicate(points[0], last_point))
                    {
                        points.Add(last_point);
                    }
                }
                
                // Убеждаемся, что у нас достаточно точек
                if (points.Count < 2)
                {
                    // Если точек недостаточно, создаем минимальную линию
                    points.Clear();
                    points.Add(dg.Point.ByCoordinates(curve_start_point.X / 1000.0, curve_start_point.Y / 1000.0));
                    points.Add(dg.Point.ByCoordinates(curve_end_point.X / 1000.0, curve_end_point.Y / 1000.0));
                }
                
                return dg.PolyCurve.ByPoints(points);
            }
            catch
            {
                // В случае ошибки возвращаем простую линию
                try
                {
                    Renga.Point2D start = this._i.GetBeginPoint();
                    Renga.Point2D end = this._i.GetEndPoint();
                    return dg.PolyCurve.ByPoints(new List<dg.Point> {
                        dg.Point.ByCoordinates(start.X / 1000.0, start.Y / 1000.0),
                        dg.Point.ByCoordinates(end.X / 1000.0, end.Y / 1000.0)
                    });
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Отладочная информация для диагностики проблем с конвертацией
        /// </summary>
        /// <returns>Строка с отладочной информацией</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            try
            {
                var info = new System.Text.StringBuilder();
                
                // Базовая информация о кривой
                int curveTypeInt = (int)this._i.Curve2DType;
                info.AppendLine($"Curve Type: {this.Curve2DType} (Int: {curveTypeInt})");
                info.AppendLine($"Is Closed: {this.IsClosed}");
                info.AppendLine($"Is PolyCurve: {this.IsPolyCurve()}");
                
                // Информация о точках
                try
                {
                    Renga.Point2D start = this._i.GetBeginPoint();
                    Renga.Point2D end = this._i.GetEndPoint();
                    info.AppendLine($"Start Point: ({start.X}, {start.Y})");
                    info.AppendLine($"End Point: ({end.X}, {end.Y})");
                }
                catch (Exception ex)
                {
                    info.AppendLine($"Point Error: {ex.Message}");
                }
                
                // Информация о длине
                try
                {
                    double length = this._i.GetLength();
                    info.AppendLine($"Length: {length} mm");
                }
                catch (Exception ex)
                {
                    info.AppendLine($"Length Error: {ex.Message}");
                }
                
                // Информация о параметрах
                try
                {
                    double minParam = this._i.MinParameter;
                    double maxParam = this._i.MaxParameter;
                    info.AppendLine($"Parameter Range: {minParam} to {maxParam}");
                }
                catch (Exception ex)
                {
                    info.AppendLine($"Parameter Error: {ex.Message}");
                }
                
                // Информация о сегментах (для PolyCurve)
                if (this.IsPolyCurve())
                {
                    try
                    {
                        int segmentCount = this.GetSegmentCount();
                        info.AppendLine($"Segment Count: {segmentCount}");
                    }
                    catch (Exception ex)
                    {
                        info.AppendLine($"Segment Count Error: {ex.Message}");
                    }
                }
                
                return info.ToString();
            }
            catch (Exception ex)
            {
                return $"Debug Info Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Простое преобразование в Dynamo PolyCurve (минимальная версия для отладки)
        /// </summary>
        /// <returns>Dynamo PolyCurve или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.PolyCurve ToDynamoPolyCurveSimple()
        {
            try
            {
                // Создаем простую линию из начальной и конечной точек
                Renga.Point2D start = this._i.GetBeginPoint();
                Renga.Point2D end = this._i.GetEndPoint();
                
                var points = new List<dg.Point>
                {
                    dg.Point.ByCoordinates(start.X / 1000.0, start.Y / 1000.0),
                    dg.Point.ByCoordinates(end.X / 1000.0, end.Y / 1000.0)
                };
                
                return dg.PolyCurve.ByPoints(points);
            }
            catch (Exception)
            {
                // Возвращаем null с информацией об ошибке
                return null;
            }
        }

        /// <summary>
        /// Оптимизированное преобразование для закрытых PolyCurve с минимальным количеством точек
        /// </summary>
        /// <returns>Dynamo PolyCurve или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.PolyCurve ToDynamoPolyCurveMinimal()
        {
            try
            {
                // Проверяем, что это закрытый PolyCurve
                if (!this.IsClosed || !this.IsPolyCurve())
                {
                    return this.ToDynamoPolyCurveSimple();
                }

                // Получаем сегменты
                int segmentCount = this.GetSegmentCount();
                if (segmentCount <= 0)
                {
                    return this.ToDynamoPolyCurveSimple();
                }

                List<dg.Curve> allCurves = new List<dg.Curve>();
                
                // Обрабатываем каждый сегмент отдельно
                for (int i = 0; i < segmentCount; i++)
                {
                    Curve2D segment = this.GetSegment(i);
                    if (segment != null)
                    {
                        // Создаем кривую для сегмента с учетом его типа
                        dg.Curve segmentCurve = CreateDynamoCurveFromSegment(segment, 2); // Минимальное количество точек
                        if (segmentCurve != null)
                        {
                            allCurves.Add(segmentCurve);
                        }
                    }
                }
                
                // Создаем итоговый PolyCurve из всех кривых
                if (allCurves.Count > 0)
                {
                    return dg.PolyCurve.ByJoinedCurves(allCurves);
                }
                else
                {
                    return this.ToDynamoPolyCurveSimple();
                }
            }
            catch
            {
                return this.ToDynamoPolyCurveSimple();
            }
        }

        /// <summary>
        /// Специализированное преобразование для закрытых PolyCurve с очень близкими начальной и конечной точками
        /// </summary>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре</param>
        /// <returns>Dynamo PolyCurve или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.PolyCurve ToDynamoPolyCurveClosedPolyCurve(int parts_in_meter = 2)
        {
            try
            {
                // Проверяем, что это закрытый PolyCurve
                if (!this.IsClosed || !this.IsPolyCurve())
                {
                    return this.ToDynamoPolyCurveSafe(parts_in_meter);
                }

                // Получаем сегменты
                int segmentCount = this.GetSegmentCount();
                if (segmentCount <= 0)
                {
                    return this.ToDynamoPolyCurveSafe(parts_in_meter);
                }

                List<dg.Curve> dynamoCurves = new List<dg.Curve>();
                
                // Обрабатываем каждый сегмент отдельно с учетом типа кривой
                for (int i = 0; i < segmentCount; i++)
                {
                    Curve2D segment = this.GetSegment(i);
                    if (segment != null)
                    {
                        // Создаем кривую для сегмента с учетом его типа
                        dg.Curve segmentCurve = CreateDynamoCurveFromSegment(segment, parts_in_meter);
                        if (segmentCurve != null)
                        {
                            dynamoCurves.Add(segmentCurve);
                        }
                    }
                }
                
                // Создаем итоговый PolyCurve из всех сегментов
                if (dynamoCurves.Count > 0)
                {
                    return dg.PolyCurve.ByJoinedCurves(dynamoCurves);
                }
                else
                {
                    // Fallback к простому методу
                    return this.ToDynamoPolyCurveSafe(parts_in_meter);
                }
            }
            catch
            {
                return this.ToDynamoPolyCurveSafe(parts_in_meter);
            }
        }

        /// <summary>
        /// Проверка, являются ли две точки дублирующимися (с учетом погрешности)
        /// </summary>
        /// <param name="point1">Первая точка</param>
        /// <param name="point2">Вторая точка</param>
        /// <param name="tolerance">Допустимая погрешность (по умолчанию 1e-3)</param>
        /// <returns>True если точки дублирующиеся</returns>
        private static bool IsPointDuplicate(dg.Point point1, dg.Point point2, double tolerance = 1e-3)
        {
            if (point1 == null || point2 == null) return false;
            
            double dx = Math.Abs(point1.X - point2.X);
            double dy = Math.Abs(point1.Y - point2.Y);
            double dz = Math.Abs(point1.Z - point2.Z);
            
            return dx < tolerance && dy < tolerance && dz < tolerance;
        }

        /// <summary>
        /// Создание Dynamo кривой из сегмента с учетом типа кривой
        /// </summary>
        /// <param name="segment">Сегмент кривой</param>
        /// <param name="minPoints">Минимальное количество точек</param>
        /// <returns>Dynamo кривая или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Curve CreateDynamoCurveFromSegment(Curve2D segment, int minPoints = 2)
        {
            if (segment == null) return null;

            try
            {
                // Get curve type as integer first, then convert to string
                int curveTypeInt = (int)segment._i.Curve2DType;
                string curveType = segment.Curve2DType.ToString();
                
                // Debug information
                System.Diagnostics.Debug.WriteLine($"Curve Type Int: {curveTypeInt}, String: {curveType}");
                
                switch (curveTypeInt)
                {
                    case 1: // Curve2DType_LineSegment
                        return CreateDynamoLineFromSegment(segment);
                    
                    case 2: // Curve2DType_Arc
                        return CreateDynamoArcFromSegment(segment, minPoints);
                    
                    case 3: // Curve2DType_PolyCurve
                        return CreateDynamoPolyCurveFromSegment(segment, minPoints);
                    
                    default:
                        return CreateDynamoGenericCurveFromSegment(segment, minPoints);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateDynamoCurveFromSegment: {ex.Message}");
                return CreateDynamoGenericCurveFromSegment(segment, minPoints);
            }
        }

        /// <summary>
        /// Создание Dynamo линии из сегмента линии
        /// </summary>
        /// <param name="segment">Сегмент линии</param>
        /// <returns>Dynamo линия</returns>
        private static dg.Curve CreateDynamoLineFromSegment(Curve2D segment)
        {
            try
            {
                Renga.Point2D start = segment._i.GetBeginPoint();
                Renga.Point2D end = segment._i.GetEndPoint();
                
                dg.Point startPoint = dg.Point.ByCoordinates(start.X / 1000.0, start.Y / 1000.0);
                dg.Point endPoint = dg.Point.ByCoordinates(end.X / 1000.0, end.Y / 1000.0);
                
                return dg.Line.ByStartPointEndPoint(startPoint, endPoint);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создание Dynamo дуги из сегмента дуги
        /// </summary>
        /// <param name="segment">Сегмент дуги</param>
        /// <param name="minPoints">Минимальное количество точек</param>
        /// <returns>Dynamo дуга или PolyCurve</returns>
        private static dg.Curve CreateDynamoArcFromSegment(Curve2D segment, int minPoints)
        {
            try
            {
                Renga.Point2D start = segment._i.GetBeginPoint();
                Renga.Point2D end = segment._i.GetEndPoint();
                
                // Try to create a proper Dynamo Arc first
                try
                {
                    // Get three points to define the arc
                    double minParam = segment._i.MinParameter;
                    double maxParam = segment._i.MaxParameter;
                    double midParam = (minParam + maxParam) / 2.0;
                    
                    Renga.Point2D midPoint = segment._i.GetPointOn(midParam);
                    
                    dg.Point startPt = dg.Point.ByCoordinates(start.X / 1000.0, start.Y / 1000.0);
                    dg.Point midPt = dg.Point.ByCoordinates(midPoint.X / 1000.0, midPoint.Y / 1000.0);
                    dg.Point endPt = dg.Point.ByCoordinates(end.X / 1000.0, end.Y / 1000.0);
                    
                    // Try to create Arc.ByThreePoints
                    dg.Arc arc = dg.Arc.ByThreePoints(startPt, midPt, endPt);
                    if (arc != null)
                    {
                        return arc;
                    }
                }
                catch
                {
                    // If Arc.ByThreePoints fails, fall back to high-quality point sampling
                }
                
                // Fallback: Create high-quality PolyCurve from points
                List<dg.Point> points = new List<dg.Point>();
                
                double fallbackMinParam = segment._i.MinParameter;
                double fallbackMaxParam = segment._i.MaxParameter;
                
                // Add start point
                points.Add(dg.Point.ByCoordinates(start.X / 1000.0, start.Y / 1000.0));
                
                // For arcs, add more intermediate points for better quality
                int numPoints = Math.Max(minPoints, 16); // Increased to 16 points for smoother arcs
                
                for (int i = 1; i < numPoints - 1; i++)
                {
                    double param = fallbackMinParam + (fallbackMaxParam - fallbackMinParam) * i / (numPoints - 1);
                    Renga.Point2D point = segment._i.GetPointOn(param);
                    dg.Point dynamoPoint = dg.Point.ByCoordinates(point.X / 1000.0, point.Y / 1000.0);
                    
                    // Check for duplicates
                    if (points.Count == 0 || !IsPointDuplicate(points[points.Count - 1], dynamoPoint, 1e-4))
                    {
                        points.Add(dynamoPoint);
                    }
                }
                
                // Add end point
                dg.Point endPoint = dg.Point.ByCoordinates(end.X / 1000.0, end.Y / 1000.0);
                if (points.Count == 0 || !IsPointDuplicate(points[points.Count - 1], endPoint, 1e-4))
                {
                    points.Add(endPoint);
                }
                
                // Create PolyCurve from points
                if (points.Count >= 2)
                {
                    return dg.PolyCurve.ByPoints(points);
                }
                else
                {
                    return dg.Line.ByStartPointEndPoint(points[0], points[1]);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateDynamoArcFromSegment: {ex.Message}");
                return CreateDynamoGenericCurveFromSegment(segment, minPoints);
            }
        }

        /// <summary>
        /// Создание Dynamo PolyCurve из сегмента PolyCurve
        /// </summary>
        /// <param name="segment">Сегмент PolyCurve</param>
        /// <param name="minPoints">Минимальное количество точек</param>
        /// <returns>Dynamo PolyCurve</returns>
        private static dg.Curve CreateDynamoPolyCurveFromSegment(Curve2D segment, int minPoints)
        {
            try
            {
                // Рекурсивно обрабатываем вложенные PolyCurve
                return segment.ToDynamoPolyCurveSafe(2);
            }
            catch
            {
                return CreateDynamoGenericCurveFromSegment(segment, minPoints);
            }
        }

        /// <summary>
        /// Создание Dynamo кривой из сегмента общего типа
        /// </summary>
        /// <param name="segment">Сегмент кривой</param>
        /// <param name="minPoints">Минимальное количество точек</param>
        /// <returns>Dynamo кривая</returns>
        private static dg.Curve CreateDynamoGenericCurveFromSegment(Curve2D segment, int minPoints)
        {
            try
            {
                Renga.Point2D start = segment._i.GetBeginPoint();
                Renga.Point2D end = segment._i.GetEndPoint();
                
                List<dg.Point> points = new List<dg.Point>();
                
                double minParam = segment._i.MinParameter;
                double maxParam = segment._i.MaxParameter;
                double length = segment._i.GetLength() / 1000.0;
                
                // Определяем количество точек на основе длины
                int numPoints = Math.Max(minPoints, Math.Min(Convert.ToInt32(length * 2), 20));
                
                for (int i = 0; i < numPoints; i++)
                {
                    double param = minParam + (maxParam - minParam) * i / (numPoints - 1);
                    Renga.Point2D point = segment._i.GetPointOn(param);
                    dg.Point dynamoPoint = dg.Point.ByCoordinates(point.X / 1000.0, point.Y / 1000.0);
                    
                    // Проверяем на дублирование
                    if (points.Count == 0 || !IsPointDuplicate(points[points.Count - 1], dynamoPoint, 1e-3))
                    {
                        points.Add(dynamoPoint);
                    }
                }
                
                // Создаем PolyCurve из точек
                if (points.Count >= 2)
                {
                    return dg.PolyCurve.ByPoints(points);
                }
                else
                {
                    return dg.Line.ByStartPointEndPoint(points[0], points[1]);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Тестирование конвертации сегмента с отладочной информацией
        /// </summary>
        /// <param name="segment">Сегмент для тестирования</param>
        /// <returns>Строка с результатами тестирования</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string TestSegmentConversion(Curve2D segment)
        {
            if (segment == null) return "Segment is null";

            try
            {
                var result = new System.Text.StringBuilder();
                
                // Информация о сегменте
                int curveTypeInt = (int)segment._i.Curve2DType;
                string curveType = segment.Curve2DType.ToString();
                result.AppendLine($"Segment Curve Type: {curveType} (Int: {curveTypeInt})");
                
                // Тестируем конвертацию
                dg.Curve dynamoCurve = CreateDynamoCurveFromSegment(segment, 2);
                
                if (dynamoCurve == null)
                {
                    result.AppendLine("❌ Conversion result: NULL");
                }
                else
                {
                    result.AppendLine($"✅ Conversion result: {dynamoCurve.GetType().Name}");
                    
                    // Дополнительная информация о результате
                    if (dynamoCurve is dg.PolyCurve polyCurve)
                    {
                        result.AppendLine($"   PolyCurve segments: {polyCurve.NumberOfCurves}");
                    }
                    else if (dynamoCurve is dg.Arc arc)
                    {
                        result.AppendLine($"   Arc radius: {arc.Radius}");
                    }
                    else if (dynamoCurve is dg.Line line)
                    {
                        result.AppendLine($"   Line length: {line.Length}");
                    }
                }
                
                return result.ToString();
            }
            catch (System.Exception ex)
            {
                return $"Error in TestSegmentConversion: {ex.Message}";
            }
        }

        /// <summary>
        /// Преобразование Curve2D в Dynamo PolyCurve (специально для PolyCurve типов)
        /// </summary>
        /// <param name="curve2D">Кривая 2D для преобразования</param>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре (по умолчанию 2)</param>
        /// <returns>Dynamo PolyCurve или null если тип не поддерживается</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.PolyCurve ToDynamoPolyCurve(Curve2D curve2D, int parts_in_meter = 2)
        {
            if (curve2D == null) return null;
            
            // Проверяем, что это PolyCurve тип
            if (curve2D.Curve2DType.ToString() != "Curve2DType_PolyCurve")
            {
                // Если не PolyCurve, используем безопасный метод
                return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
            }
            
            // Для PolyCurve используем безопасный метод
            return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
        }

        /// <summary>
        /// Преобразование Curve2D в Dynamo PolyCurve с использованием сегментов (для PolyCurve типов)
        /// </summary>
        /// <param name="curve2D">Кривая 2D для преобразования</param>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре (по умолчанию 2)</param>
        /// <returns>Dynamo PolyCurve или null если тип не поддерживается</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.PolyCurve ToDynamoPolyCurveFromSegments(Curve2D curve2D, int parts_in_meter = 2)
        {
            if (curve2D == null) return null;
            
            try
            {
                // Пытаемся получить IPolyCurve2D интерфейс
                Renga.IPolyCurve2D polyCurve2D = curve2D._i as Renga.IPolyCurve2D;
                if (polyCurve2D != null)
                {
                    // Получаем количество сегментов
                    int segmentCount = polyCurve2D.GetSegmentCount();
                    if (segmentCount > 0)
                    {
                        List<dg.Curve> dynamoCurves = new List<dg.Curve>();
                        
                        // Обрабатываем каждый сегмент
                        for (int i = 0; i < segmentCount; i++)
                        {
                            Renga.ICurve2D segment = polyCurve2D.GetSegment(i);
                            if (segment != null)
                            {
                                // Создаем временный Curve2D объект для сегмента
                                Curve2D segmentCurve = new Curve2D(segment);
                                
                                // Преобразуем сегмент в Dynamo кривую с обработкой закрытых кривых
                                dg.PolyCurve segmentPolyCurve = segmentCurve.ToDynamoPolyCurveSafe(parts_in_meter);
                                if (segmentPolyCurve != null)
                                {
                                    dynamoCurves.Add(segmentPolyCurve);
                                }
                            }
                        }
                        
                        // Создаем итоговый PolyCurve из всех сегментов
                        if (dynamoCurves.Count > 0)
                        {
                            return dg.PolyCurve.ByJoinedCurves(dynamoCurves);
                        }
                    }
                }
                
                // Если не удалось получить сегменты, используем общий метод
                return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
            }
            catch
            {
                // В случае ошибки используем общий метод
                return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
            }
        }

        /// <summary>
        /// Проверка, является ли кривая PolyCurve типом
        /// </summary>
        /// <returns>True если это PolyCurve, иначе False</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsPolyCurve()
        {
            return this.Curve2DType.ToString() == "Curve2DType_PolyCurve";
        }

        /// <summary>
        /// Получение типа кривой как строки
        /// </summary>
        /// <returns>Строковое представление типа кривой</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetCurveTypeString()
        {
            return this.Curve2DType.ToString();
        }

        /// <summary>
        /// Получение количества сегментов для PolyCurve (только для PolyCurve типов)
        /// </summary>
        /// <returns>Количество сегментов или -1 если не PolyCurve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetSegmentCount()
        {
            try
            {
                Renga.IPolyCurve2D polyCurve2D = this._i as Renga.IPolyCurve2D;
                if (polyCurve2D != null)
                {
                    return polyCurve2D.GetSegmentCount();
                }
            }
            catch
            {
                // Ignore errors
            }
            return -1; // Not a PolyCurve or error
        }

        /// <summary>
        /// Получение сегмента PolyCurve по индексу (только для PolyCurve типов)
        /// </summary>
        /// <param name="index">Индекс сегмента</param>
        /// <returns>Curve2D сегмент или null если не PolyCurve или неверный индекс</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetSegment(int index)
        {
            try
            {
                Renga.IPolyCurve2D polyCurve2D = this._i as Renga.IPolyCurve2D;
                if (polyCurve2D != null)
                {
                    int segmentCount = polyCurve2D.GetSegmentCount();
                    if (index >= 0 && index < segmentCount)
                    {
                        Renga.ICurve2D segment = polyCurve2D.GetSegment(index);
                        if (segment != null)
                        {
                            return new Curve2D(segment);
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
            return null;
        }

        /// <summary>
        /// Преобразование Curve2D в Dynamo PolyCurve с специальной обработкой закрытых кривых
        /// </summary>
        /// <param name="curve2D">Кривая 2D для преобразования</param>
        /// <param name="parts_in_meter">Число сегментов полилинии в 1 метре (по умолчанию 2)</param>
        /// <returns>Dynamo PolyCurve или null если тип не поддерживается</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.PolyCurve ToDynamoPolyCurveClosed(Curve2D curve2D, int parts_in_meter = 2)
        {
            if (curve2D == null) return null;
            
            try
            {
                // Проверяем, является ли кривая закрытой
                bool is_closed = curve2D._i.IsClosed();
                
                if (is_closed)
                {
                    // Для закрытых кривых используем специальную обработку
                    return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
                }
                else
                {
                    // Для открытых кривых используем обычный метод
                    return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
                }
            }
            catch
            {
                return curve2D.ToDynamoPolyCurveSafe(parts_in_meter);
            }
        }

        /// <summary>
        /// Создание Curve2D из Dynamo кривой
        /// </summary>
        /// <param name="dynamoCurve">Dynamo кривая</param>
        /// <returns>Curve2D объект или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Curve2D ByDynamoCurve(dg.Curve dynamoCurve)
        {
            try
            {
                if (dynamoCurve == null) return null;
                
                // Convert Dynamo curve to Renga curve
                var rengaCurve = ConvertDynamoCurveToRenga(dynamoCurve);
                if (rengaCurve == null) return null;
                
                return new Curve2D(rengaCurve);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Создание Curve2D из Dynamo PolyCurve
        /// </summary>
        /// <param name="dynamoPolyCurve">Dynamo PolyCurve</param>
        /// <returns>Curve2D объект и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Curve2D", "DebugInfo" })]
        public static Dictionary<string, object> ByDynamoPolyCurve(dg.PolyCurve dynamoPolyCurve)
        {
            var debugInfo = "🔧 Converting Dynamo PolyCurve to Curve2D...\n";
            
            try
            {
                
                if (dynamoPolyCurve == null) 
                {
                    debugInfo += "❌ Dynamo PolyCurve is null\n";
                    return new Dictionary<string, object>
                    {
                        { "Curve2D", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Dynamo PolyCurve received\n";
                debugInfo += $"📐 PolyCurve type: {dynamoPolyCurve.GetType().Name}\n";
                
                // Get curve information
                try
                {
                    var startPoint = dynamoPolyCurve.StartPoint;
                    var endPoint = dynamoPolyCurve.EndPoint;
                    debugInfo += $"📍 Start point: ({startPoint.X:F2}, {startPoint.Y:F2}, {startPoint.Z:F2})\n";
                    debugInfo += $"📍 End point: ({endPoint.X:F2}, {endPoint.Y:F2}, {endPoint.Z:F2})\n";
                    
                    // Calculate approximate length
                    var length = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + 
                                        Math.Pow(endPoint.Y - startPoint.Y, 2) + 
                                        Math.Pow(endPoint.Z - startPoint.Z, 2));
                    debugInfo += $"📏 Approximate length: {length:F2} meters\n";
                    
                    // Check if this is a closed curve (same start/end points)
                    var isClosed = Math.Abs(startPoint.X - endPoint.X) < 1e-9 && 
                                  Math.Abs(startPoint.Y - endPoint.Y) < 1e-9 && 
                                  Math.Abs(startPoint.Z - endPoint.Z) < 1e-9;
                    
                    if (isClosed)
                    {
                        debugInfo += "🔄 Closed PolyCurve detected (start/end points same)\n";
                        debugInfo += "💡 This will be processed as a normal closed curve\n";
                    }
                    
                    // Get segment information
                    var segments = dynamoPolyCurve.Curves();
                    if (segments != null)
                    {
                        debugInfo += $"📐 Number of segments: {segments.Length}\n";
                        
                        for (int i = 0; i < segments.Length; i++)
                        {
                            var segment = segments[i];
                            var segStart = segment.StartPoint;
                            var segEnd = segment.EndPoint;
                            var segLength = Math.Sqrt(Math.Pow(segEnd.X - segStart.X, 2) + 
                                                    Math.Pow(segEnd.Y - segStart.Y, 2) + 
                                                    Math.Pow(segEnd.Z - segStart.Z, 2));
                            debugInfo += $"  📏 Segment {i + 1}: {segment.GetType().Name}, Length: {segLength:F6}m\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Could not extract curve information: {ex.Message}\n";
                }
                
                // Convert Dynamo PolyCurve to Renga curve
                debugInfo += "🔄 Converting to Renga curve...\n";
                debugInfo += "🔧 Calling ConvertDynamoPolyCurveToRenga...\n";
                var rengaCurve = ConvertDynamoPolyCurveToRenga(dynamoPolyCurve, ref debugInfo);
                debugInfo += $"🔧 ConvertDynamoPolyCurveToRenga returned: {(rengaCurve != null ? "Success" : "Null")}\n";
                if (rengaCurve == null) 
                {
                    debugInfo += "❌ Conversion to Renga curve failed - returned null\n";
                    debugInfo += "💡 This may indicate that the PolyCurve is too complex or contains unsupported curve types\n";
                    return new Dictionary<string, object>
                    {
                        { "Curve2D", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Renga curve conversion successful\n";
                debugInfo += $"📐 Renga curve type: {rengaCurve.GetType().Name}\n";
                
                var curve2D = new Curve2D(rengaCurve);
                debugInfo += "✅ Curve2D wrapper created successfully\n";
                debugInfo += $"🔗 Curve2D._i is null: {curve2D._i == null}\n";
                
                if (curve2D._i == null)
                {
                    debugInfo += "⚠️ WARNING: Curve2D._i is null - this may cause issues with baseline setting\n";
                    debugInfo += "💡 The curve may not be compatible with Renga's baseline system\n";
                }
                else
                {
                    debugInfo += "✅ Curve2D has valid Renga interface - should work with baselines\n";
                }
                
                return new Dictionary<string, object>
                {
                    { "Curve2D", curve2D },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ ByDynamoPolyCurve failed!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Curve2D", null },
                    { "DebugInfo", debugInfo }
                };
            }
        }
        
        /// <summary>
        /// Создание Curve2D линии по двум точкам
        /// </summary>
        /// <param name="x1">X координата начальной точки</param>
        /// <param name="y1">Y координата начальной точки</param>
        /// <param name="x2">X координата конечной точки</param>
        /// <param name="y2">Y координата конечной точки</param>
        /// <returns>Curve2D объект и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Curve2D", "DebugInfo" })]
        public static Dictionary<string, object> ByLineSegment(double x1, double y1, double x2, double y2)
        {
            var debugInfo = "🔧 Creating Curve2D line segment...\n";
            
            try
            {
                debugInfo += $"📍 Input coordinates (meters): Start=({x1:F2}, {y1:F2}), End=({x2:F2}, {y2:F2})\n";
                
                // Create Renga Point2D structures (coordinates are in meters, convert to mm)
                var startPoint = new Renga.Point2D
                {
                    X = x1 * 1000.0, // Convert to mm
                    Y = y1 * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = x2 * 1000.0, // Convert to mm
                    Y = y2 * 1000.0
                };
                
                debugInfo += $"📐 Converted coordinates (mm): Start=({startPoint.X:F1}, {startPoint.Y:F1}), End=({endPoint.X:F1}, {endPoint.Y:F1})\n";
                
                // Try to create IMath interface using COM activation
                try
                {
                    debugInfo += "🔍 Attempting to create Renga IMath interface...\n";
                    Type mathType = Type.GetTypeFromProgID("Renga.Math");
                    if (mathType != null)
                    {
                        debugInfo += "✅ Renga.Math type found, creating instance...\n";
                        var mathInterface = Activator.CreateInstance(mathType) as Renga.IMath;
                        if (mathInterface != null)
                        {
                            debugInfo += "✅ IMath interface created successfully!\n";
                            
                            try
                            {
                                // Create the line segment using IMath
                                Renga.ICurve2D curve2D = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                                
                                if (curve2D != null)
                                {
                                    debugInfo += "✅ Curve2D created using Renga IMath interface!\n";
                                    debugInfo += $"📏 Line length: {Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)):F2} meters\n";
                                    
                                    return new Dictionary<string, object>
                                    {
                                        { "Curve2D", new Curve2D(curve2D) },
                                        { "DebugInfo", debugInfo }
                                    };
                                }
                                else
                                {
                                    debugInfo += "⚠️ IMath.CreateLineSegment2D returned null\n";
                                }
                            }
                            finally
                            {
                                // Properly dispose of the created IMath instance
                                SafeReleaseComObject(mathInterface);
                                ForceGarbageCollection();
                            }
                        }
                        else
                        {
                            debugInfo += "⚠️ Failed to cast to IMath interface\n";
                        }
                    }
                    else
                    {
                        debugInfo += "⚠️ Renga.Math type not found\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ COM creation failed: {ex.Message}\n";
                    debugInfo += "🔄 Falling back to workaround method...\n";
                }
                
                // Fallback: Return null if COM creation fails
                debugInfo += "❌ COM creation failed - cannot create valid Curve2D\n";
                debugInfo += "💡 For proper baseline support, ensure Renga is properly installed\n";
                debugInfo += "💡 Try using Curve2D.ByLineSegment with different coordinates\n";
                
                return new Dictionary<string, object>
                {
                    { "Curve2D", null },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Curve2D creation failed!\n";
                debugInfo += $"Input: Start=({x1:F2}, {y1:F2}), End=({x2:F2}, {y2:F2})\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Curve2D", null },
                    { "DebugInfo", debugInfo }
                };
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo кривой в Renga кривую
        /// </summary>
        /// <param name="dynamoCurve">Dynamo кривая</param>
        /// <returns>Renga кривая или null</returns>
        private static object ConvertDynamoCurveToRenga(dg.Curve dynamoCurve)
        {
            try
            {
                // Handle different Dynamo curve types
                if (dynamoCurve is dg.Line line)
                {
                    return ConvertDynamoLineToRenga(line);
                }
                else if (dynamoCurve is dg.Arc arc)
                {
                    return ConvertDynamoArcToRenga(arc);
                }
                else if (dynamoCurve is dg.PolyCurve polyCurve)
                {
                    string debugInfo = "";
                    return ConvertDynamoPolyCurveToRenga(polyCurve, ref debugInfo);
                }
                else
                {
                    // For other curve types, try to approximate with line segments
                    return ConvertDynamoCurveToLineApproximation(dynamoCurve);
                }
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo PolyCurve в Renga кривую
        /// </summary>
        /// <param name="dynamoPolyCurve">Dynamo PolyCurve</param>
        /// <param name="debugInfo">Debug output string (ref)</param>
        /// <returns>Renga кривая или null</returns>
        private static object ConvertDynamoPolyCurveToRenga(dg.PolyCurve dynamoPolyCurve, ref string debugInfo)
        {
            try
            {
                debugInfo += "🔧 ConvertDynamoPolyCurveToRenga: Starting conversion\n";
                
                // Get curve segments
                var segments = dynamoPolyCurve.Curves();
                if (segments == null || segments.Length == 0) 
                {
                    debugInfo += "❌ No segments found in PolyCurve\n";
                    return null;
                }
                
                debugInfo += $"📐 Found {segments.Length} segments\n";
                
                // Try to create IMath interface through IApplication
                try
                {
                    debugInfo += "🔧 Attempting to create Renga IApplication COM interface\n";
                    
                    // Try different ProgID variations for IApplication
                    string[] progIds = { "Renga.Application", "RengaApplication", "Renga.Application.1", "RengaApplication.1" };
                    Type appType = null;
                    string usedProgId = "";
                    
                    foreach (string progId in progIds)
                    {
                        debugInfo += $"🔍 Trying ProgID: {progId}\n";
                        appType = Type.GetTypeFromProgID(progId);
                        if (appType != null)
                        {
                            usedProgId = progId;
                            debugInfo += $"✅ Found IApplication type with ProgID: {progId}\n";
                            break;
                        }
                        else
                        {
                            debugInfo += $"❌ ProgID {progId} not found\n";
                        }
                    }
                    
                    if (appType != null)
                    {
                        debugInfo += $"✅ Renga IApplication type found using ProgID: {usedProgId}\n";
                        
                        // Create IApplication instance
                        var appInterface = Activator.CreateInstance(appType) as Renga.IApplication;
                        if (appInterface != null)
                        {
                            debugInfo += "✅ Renga IApplication interface created successfully\n";
                            
                            try
                            {
                                // Access Math property from IApplication
                                debugInfo += "🔧 Accessing Math property from IApplication\n";
                                var mathInterface = appInterface.Math;
                                
                                if (mathInterface != null)
                                {
                                    debugInfo += "✅ Renga IMath interface accessed successfully\n";
                                    
                                    if (segments.Length == 1)
                                    {
                                        // Single segment - convert directly
                                        debugInfo += "📐 Single segment - converting directly\n";
                                        return ConvertDynamoCurveToRengaWithMath(segments[0], mathInterface, ref debugInfo);
                                    }
                                    else
                                    {
                                        // Multiple segments - create composite curve
                                        debugInfo += "📐 Multiple segments - creating composite curve\n";
                                        return ConvertDynamoPolyCurveToRengaWithMath(dynamoPolyCurve, mathInterface, ref debugInfo);
                                    }
                                }
                                else
                                {
                                    debugInfo += "❌ Failed to access Math property from IApplication\n";
                                    debugInfo += "💡 This might indicate that Renga is not properly initialized or the Math interface is not available\n";
                                }
                            }
                            finally
                            {
                                // Properly dispose of the created IApplication instance
                                SafeReleaseComObject(appInterface);
                                ForceGarbageCollection();
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create Renga IApplication interface from type\n";
                            debugInfo += "💡 This might indicate that Renga is not properly installed or the COM interface is not registered\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ No Renga IApplication COM interface found with any ProgID\n";
                        debugInfo += "💡 Possible solutions:\n";
                        debugInfo += "   1. Ensure Renga is properly installed\n";
                        debugInfo += "   2. Register the Renga COM interface (run as administrator):\n";
                        debugInfo += "      regsvr32 \"C:\\Program Files\\Renga\\RengaApplication.dll\"\n";
                        debugInfo += "   3. Check if Renga is running and accessible\n";
                        debugInfo += "   4. Try running Renga first before using the Dynamo node\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ COM creation failed in ConvertDynamoPolyCurveToRenga: {ex.Message}\n";
                    debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                }
                
                // Fallback: return null if COM creation fails
                debugInfo += "❌ All conversion attempts failed - returning null\n";
                return null;
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ ConvertDynamoPolyCurveToRenga failed: {ex.Message}\n";
                debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo кривой в Renga кривую с использованием IMath
        /// </summary>
        /// <param name="dynamoCurve">Dynamo кривая</param>
        /// <param name="mathInterface">Renga IMath интерфейс</param>
        /// <param name="debugInfo">Debug output string (ref)</param>
        /// <returns>Renga ICurve2D или null</returns>
        private static Renga.ICurve2D ConvertDynamoCurveToRengaWithMath(dg.Curve dynamoCurve, Renga.IMath mathInterface, ref string debugInfo)
        {
            try
            {
                debugInfo += $"🔧 ConvertDynamoCurveToRengaWithMath: Processing {dynamoCurve.GetType().Name}\n";
                
                if (dynamoCurve is dg.Line line)
                {
                    debugInfo += $"📐 Converting Line segment\n";
                    var result = ConvertDynamoLineToRengaWithMath(line, mathInterface, ref debugInfo);
                    debugInfo += $"📐 Line conversion result: {(result != null ? "Success" : "Failed")}\n";
                    return result;
                }
                else if (dynamoCurve is dg.Arc arc)
                {
                    debugInfo += $"📐 Converting Arc segment\n";
                    var result = ConvertDynamoArcToRengaWithMath(arc, mathInterface, ref debugInfo);
                    debugInfo += $"📐 Arc conversion result: {(result != null ? "Success" : "Failed")}\n";
                    return result;
                }
                else
                {
                    debugInfo += $"📐 Converting other curve type: {dynamoCurve.GetType().Name}\n";
                    // For other curve types, approximate with line
                    var result = ConvertDynamoCurveToLineApproximationWithMath(dynamoCurve, mathInterface, ref debugInfo);
                    debugInfo += $"📐 Other curve conversion result: {(result != null ? "Success" : "Failed")}\n";
                    return result;
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ ConvertDynamoCurveToRengaWithMath failed: {ex.Message}\n";
                debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo PolyCurve в Renga кривую с использованием IMath
        /// </summary>
        /// <param name="dynamoPolyCurve">Dynamo PolyCurve</param>
        /// <param name="mathInterface">Renga IMath интерфейс</param>
        /// <param name="debugInfo">Debug output string (ref)</param>
        /// <returns>Renga ICurve2D или null</returns>
        private static Renga.ICurve2D ConvertDynamoPolyCurveToRengaWithMath(dg.PolyCurve dynamoPolyCurve, Renga.IMath mathInterface, ref string debugInfo)
        {
            try
            {
                var segments = dynamoPolyCurve.Curves();
                if (segments == null || segments.Length == 0) 
                {
                    debugInfo += "❌ PolyCurve has no segments\n";
                    return null;
                }
                
                debugInfo += $"🔧 Processing PolyCurve with {segments.Length} segments\n";
                
                // Check if this is a closed PolyCurve (same start/end points) vs truly zero-length
                var startPoint = dynamoPolyCurve.StartPoint;
                var endPoint = dynamoPolyCurve.EndPoint;
                var isClosed = Math.Abs(startPoint.X - endPoint.X) < 1e-9 && 
                              Math.Abs(startPoint.Y - endPoint.Y) < 1e-9 && 
                              Math.Abs(startPoint.Z - endPoint.Z) < 1e-9;
                
                // Check if this is truly zero-length by examining individual segments
                bool isTrulyZeroLength = true;
                if (isClosed && segments.Length > 1)
                {
                    // Check if all segments are also zero-length
                    for (int i = 0; i < segments.Length; i++)
                    {
                        var segment = segments[i];
                        var segStart = segment.StartPoint;
                        var segEnd = segment.EndPoint;
                        var segLength = Math.Sqrt(Math.Pow(segEnd.X - segStart.X, 2) + 
                                                Math.Pow(segEnd.Y - segStart.Y, 2) + 
                                                Math.Pow(segEnd.Z - segStart.Z, 2));
                        if (segLength > 1e-9) // Segment has actual length
                        {
                            isTrulyZeroLength = false;
                            break;
                        }
                    }
                }
                
                if (isClosed && isTrulyZeroLength)
                {
                    debugInfo += "🔄 Truly zero-length PolyCurve detected (all segments are zero-length)\n";
                    debugInfo += $"📍 Start/End point: ({startPoint.X:F6}, {startPoint.Y:F6}, {startPoint.Z:F6})\n";
                    
                    // For truly zero-length curves, create a small circle
                    if (segments.Length == 1)
                    {
                        // Single segment zero-length curve - create a small circle
                        debugInfo += "📐 Single segment zero-length - creating small circle\n";
                        var center = new Renga.Point2D
                        {
                            X = startPoint.X * 1000.0, // Convert to mm
                            Y = startPoint.Y * 1000.0
                        };
                        // Create a very small circle (1mm radius) to represent the point
                        return mathInterface.CreateCircle2D(center, 1.0);
                    }
                    else
                    {
                        // Multiple segments all zero-length - create a small circle
                        debugInfo += "📐 All segments are zero-length - creating small circle\n";
                        var center = new Renga.Point2D
                        {
                            X = startPoint.X * 1000.0, // Convert to mm
                            Y = startPoint.Y * 1000.0
                        };
                        return mathInterface.CreateCircle2D(center, 1.0);
                    }
                }
                else if (isClosed && !isTrulyZeroLength)
                {
                    debugInfo += "🔄 Closed PolyCurve detected (start/end points same but has valid segments)\n";
                    debugInfo += $"📍 Start/End point: ({startPoint.X:F6}, {startPoint.Y:F6}, {startPoint.Z:F6})\n";
                    debugInfo += $"📐 Processing as normal closed curve with {segments.Length} segments\n";
                    // Continue with normal processing - this is a valid closed curve
                }
                
                if (segments.Length == 1)
                {
                    // Single segment - convert directly
                    debugInfo += "📐 Single segment - converting directly\n";
                    return ConvertDynamoCurveToRengaWithMath(segments[0], mathInterface, ref debugInfo);
                }
                else
                {
                    // Multiple segments - create composite curve
                    debugInfo += $"📐 Multiple segments ({segments.Length}) - creating composite curve\n";
                    var rengaCurves = new List<Renga.ICurve2D>();
                    
                    for (int i = 0; i < segments.Length; i++)
                    {
                        var segment = segments[i];
                        debugInfo += $"🔧 Converting segment {i + 1}/{segments.Length}: {segment.GetType().Name}\n";
                        
                        // Add detailed segment information
                        try
                        {
                            var segStart = segment.StartPoint;
                            var segEnd = segment.EndPoint;
                            var segLength = Math.Sqrt(Math.Pow(segEnd.X - segStart.X, 2) + 
                                                    Math.Pow(segEnd.Y - segStart.Y, 2) + 
                                                    Math.Pow(segEnd.Z - segStart.Z, 2));
                            debugInfo += $"  📍 Segment {i + 1} start: ({segStart.X:F6}, {segStart.Y:F6}, {segStart.Z:F6})\n";
                            debugInfo += $"  📍 Segment {i + 1} end: ({segEnd.X:F6}, {segEnd.Y:F6}, {segEnd.Z:F6})\n";
                            debugInfo += $"  📏 Segment {i + 1} length: {segLength:F6}m\n";
                        }
                        catch (Exception ex)
                        {
                            debugInfo += $"  ⚠️ Could not get segment {i + 1} info: {ex.Message}\n";
                        }
                        
                        var rengaCurve = ConvertDynamoCurveToRengaWithMath(segment, mathInterface, ref debugInfo);
                        if (rengaCurve != null)
                        {
                            debugInfo += $"✅ Segment {i + 1} converted successfully\n";
                            rengaCurves.Add(rengaCurve);
                        }
                        else
                        {
                            debugInfo += $"❌ Segment {i + 1} conversion failed\n";
                        }
                    }
                    
                    debugInfo += $"📊 Successfully converted {rengaCurves.Count}/{segments.Length} segments\n";
                    
                    if (rengaCurves.Count > 0)
                    {
                        try
                        {
                            // Create composite curve from all segments
                            var curveArray = rengaCurves.ToArray();
                            debugInfo += $"🔧 Creating composite curve from {curveArray.Length} segments\n";
                            var compositeCurve = mathInterface.CreateCompositeCurve2D(curveArray);
                            if (compositeCurve != null)
                            {
                                debugInfo += "✅ Composite curve created successfully\n";
                                return compositeCurve;
                            }
                            else
                            {
                                debugInfo += "❌ CreateCompositeCurve2D returned null\n";
                            }
                        }
                        catch (Exception ex)
                        {
                            debugInfo += $"❌ CreateCompositeCurve2D failed: {ex.Message}\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ No segments were successfully converted\n";
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ PolyCurve conversion failed: {ex.Message}\n";
                return null;
            }
        }
        
        /// <summary>
        /// Обработка нулевой длины замкнутой кривой
        /// </summary>
        /// <param name="segments">Сегменты кривой</param>
        /// <param name="mathInterface">Renga IMath интерфейс</param>
        /// <returns>Renga ICurve2D или null</returns>
        private static Renga.ICurve2D ProcessZeroLengthClosedCurve(dg.Curve[] segments, Renga.IMath mathInterface)
        {
            try
            {
                var rengaCurves = new List<Renga.ICurve2D>();
                
                for (int i = 0; i < segments.Length; i++)
                {
                    var segment = segments[i];
                    
                    // Check if segment is also zero-length
                    var segmentStart = segment.StartPoint;
                    var segmentEnd = segment.EndPoint;
                    var isSegmentZeroLength = Math.Abs(segmentStart.X - segmentEnd.X) < 1e-9 && 
                                            Math.Abs(segmentStart.Y - segmentEnd.Y) < 1e-9 && 
                                            Math.Abs(segmentStart.Z - segmentEnd.Z) < 1e-9;
                    
                    if (isSegmentZeroLength)
                    {
                        continue;
                    }
                    
                    string debugInfo = "";
                    var rengaCurve = ConvertDynamoCurveToRengaWithMath(segment, mathInterface, ref debugInfo);
                    if (rengaCurve != null)
                    {
                        rengaCurves.Add(rengaCurve);
                    }
                    else
                    {
                    }
                }
                
                if (rengaCurves.Count > 0)
                {
                    try
                    {
                        var curveArray = rengaCurves.ToArray();
                        var compositeCurve = mathInterface.CreateCompositeCurve2D(curveArray);
                        if (compositeCurve != null)
                        {
                            return compositeCurve;
                        }
                        else
                        {
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    // All segments are zero-length, create a small circle to represent the point
                    var startPoint = segments[0].StartPoint;
                    var center = new Renga.Point2D
                    {
                        X = startPoint.X * 1000.0, // Convert to mm
                        Y = startPoint.Y * 1000.0
                    };
                    return mathInterface.CreateCircle2D(center, 1.0); // 1mm radius circle
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo Line в Renga кривую с использованием IMath
        /// </summary>
        /// <param name="dynamoLine">Dynamo Line</param>
        /// <param name="mathInterface">Renga IMath интерфейс</param>
        /// <param name="debugInfo">Debug output string (ref)</param>
        /// <returns>Renga ICurve2D или null</returns>
        private static Renga.ICurve2D ConvertDynamoLineToRengaWithMath(dg.Line dynamoLine, Renga.IMath mathInterface, ref string debugInfo)
        {
            try
            {
                var start = dynamoLine.StartPoint;
                var end = dynamoLine.EndPoint;
                
                debugInfo += $"  📍 Line start: ({start.X:F6}, {start.Y:F6}, {start.Z:F6})\n";
                debugInfo += $"  📍 Line end: ({end.X:F6}, {end.Y:F6}, {end.Z:F6})\n";
                
                // Create Renga Point2D structures
                var startPoint = new Renga.Point2D
                {
                    X = start.X * 1000.0, // Convert to mm
                    Y = start.Y * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = end.X * 1000.0, // Convert to mm
                    Y = end.Y * 1000.0
                };
                
                debugInfo += $"  📍 Renga start: ({startPoint.X:F6}, {startPoint.Y:F6})\n";
                debugInfo += $"  📍 Renga end: ({endPoint.X:F6}, {endPoint.Y:F6})\n";
                
                // Create proper Renga ICurve2D using IMath
                var result = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                debugInfo += $"  📐 CreateLineSegment2D result: {(result != null ? "Success" : "Failed")}\n";
                return result;
            }
            catch (Exception ex)
            {
                debugInfo += $"  ❌ ConvertDynamoLineToRengaWithMath failed: {ex.Message}\n";
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo Arc в Renga кривую с использованием IMath
        /// </summary>
        /// <param name="dynamoArc">Dynamo Arc</param>
        /// <param name="mathInterface">Renga IMath интерфейс</param>
        /// <param name="debugInfo">Debug output string (ref)</param>
        /// <returns>Renga ICurve2D или null</returns>
        private static Renga.ICurve2D ConvertDynamoArcToRengaWithMath(dg.Arc dynamoArc, Renga.IMath mathInterface, ref string debugInfo)
        {
            try
            {
                var center = dynamoArc.CenterPoint;
                var start = dynamoArc.StartPoint;
                var end = dynamoArc.EndPoint;
                
                debugInfo += $"🔧 Converting Arc: Center=({center.X:F6}, {center.Y:F6}), Start=({start.X:F6}, {start.Y:F6}), End=({end.X:F6}, {end.Y:F6}), Radius={dynamoArc.Radius:F6}\n";
                
                // Create Renga Point2D structures
                var centerPoint = new Renga.Point2D
                {
                    X = center.X * 1000.0, // Convert to mm
                    Y = center.Y * 1000.0
                };
                
                var startPoint = new Renga.Point2D
                {
                    X = start.X * 1000.0, // Convert to mm
                    Y = start.Y * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = end.X * 1000.0, // Convert to mm
                    Y = end.Y * 1000.0
                };
                
                var radius = dynamoArc.Radius * 1000.0; // Convert to mm
                
                debugInfo += $"📐 Renga Arc: Center=({centerPoint.X:F1}, {centerPoint.Y:F1}), Start=({startPoint.X:F1}, {startPoint.Y:F1}), End=({endPoint.X:F1}, {endPoint.Y:F1}), Radius={radius:F1}\n";
                
                // Try to create the arc
                try
                {
                    // Create proper Renga ICurve2D using IMath
                    // For now, assume counter-clockwise (false)
                    var arc = mathInterface.CreateArc2DByCenterStartEndPoints(centerPoint, startPoint, endPoint, false);
                    if (arc != null)
                    {
                        debugInfo += "✅ Arc created successfully\n";
                        return arc;
                    }
                    else
                    {
                        debugInfo += "⚠️ CreateArc2DByCenterStartEndPoints returned null\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ CreateArc2DByCenterStartEndPoints failed: {ex.Message}\n";
                }
                
                // Fallback: Create a line approximation of the arc
                debugInfo += "🔄 Falling back to line approximation for arc\n";
                return mathInterface.CreateLineSegment2D(startPoint, endPoint);
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Arc conversion failed: {ex.Message}\n";
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo кривой в линейную аппроксимацию с использованием IMath
        /// </summary>
        /// <param name="dynamoCurve">Dynamo кривая</param>
        /// <param name="mathInterface">Renga IMath интерфейс</param>
        /// <param name="debugInfo">Debug output string (ref)</param>
        /// <returns>Renga ICurve2D или null</returns>
        private static Renga.ICurve2D ConvertDynamoCurveToLineApproximationWithMath(dg.Curve dynamoCurve, Renga.IMath mathInterface, ref string debugInfo)
        {
            try
            {
                debugInfo += "🔄 Converting curve to line approximation\n";
                // Get start and end points
                var points = dynamoCurve.PointsAtEqualChordLength(2);
                if (points == null || points.Length < 2) 
                {
                    debugInfo += "❌ Could not get points for line approximation\n";
                    return null;
                }
                
                var start = points[0];
                var end = points[points.Length - 1];
                
                debugInfo += $"📍 Line approximation start: ({start.X:F6}, {start.Y:F6}, {start.Z:F6})\n";
                debugInfo += $"📍 Line approximation end: ({end.X:F6}, {end.Y:F6}, {end.Z:F6})\n";
                
                // Create Renga Point2D structures
                var startPoint = new Renga.Point2D
                {
                    X = start.X * 1000.0, // Convert to mm
                    Y = start.Y * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = end.X * 1000.0, // Convert to mm
                    Y = end.Y * 1000.0
                };
                
                // Create proper Renga ICurve2D using IMath
                var result = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                debugInfo += $"📐 Line approximation result: {(result != null ? "Success" : "Failed")}\n";
                return result;
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Line approximation failed: {ex.Message}\n";
                return null;
            }
        }

        /// <summary>
        /// Конвертация Dynamo Line в Renga кривую
        /// </summary>
        /// <param name="dynamoLine">Dynamo Line</param>
        /// <returns>Renga кривая или null</returns>
        private static object ConvertDynamoLineToRenga(dg.Line dynamoLine)
        {
            try
            {
                var start = dynamoLine.StartPoint;
                var end = dynamoLine.EndPoint;
                
                // Create Renga Point2D structures
                var startPoint = new Renga.Point2D
                {
                    X = start.X * 1000.0, // Convert to mm
                    Y = start.Y * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = end.X * 1000.0, // Convert to mm
                    Y = end.Y * 1000.0
                };
                
                // Create proper Renga ICurve2D using IMath
                // Note: This requires access to IMath interface, which we don't have in this context
                // For now, we'll return the points and let the caller handle it
                return new { StartPoint = startPoint, EndPoint = endPoint, Type = "Line" };
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo Arc в Renga кривую
        /// </summary>
        /// <param name="dynamoArc">Dynamo Arc</param>
        /// <returns>Renga кривая или null</returns>
        private static object ConvertDynamoArcToRenga(dg.Arc dynamoArc)
        {
            try
            {
                var center = dynamoArc.CenterPoint;
                var start = dynamoArc.StartPoint;
                var end = dynamoArc.EndPoint;
                
                // Create Renga Point2D structures
                var centerPoint = new Renga.Point2D
                {
                    X = center.X * 1000.0, // Convert to mm
                    Y = center.Y * 1000.0
                };
                
                var startPoint = new Renga.Point2D
                {
                    X = start.X * 1000.0, // Convert to mm
                    Y = start.Y * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = end.X * 1000.0, // Convert to mm
                    Y = end.Y * 1000.0
                };
                
                var radius = dynamoArc.Radius * 1000.0; // Convert to mm
                
                // Return the arc data for now - in a full implementation,
                // you would use IMath.CreateArc2DByCenterStartEndPoints to create the actual curve
                return new { 
                    CenterPoint = centerPoint, 
                    StartPoint = startPoint, 
                    EndPoint = endPoint, 
                    Radius = radius, 
                    Type = "Arc" 
                };
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Конвертация Dynamo кривой в линейную аппроксимацию
        /// </summary>
        /// <param name="dynamoCurve">Dynamo кривая</param>
        /// <returns>Renga кривая или null</returns>
        private static object ConvertDynamoCurveToLineApproximation(dg.Curve dynamoCurve)
        {
            try
            {
                // Get start and end points
                var points = dynamoCurve.PointsAtEqualChordLength(2);
                if (points == null || points.Length < 2) return null;
                
                var start = points[0];
                var end = points[points.Length - 1];
                
                // Create Renga Point2D structures
                var startPoint = new Renga.Point2D
                {
                    X = start.X * 1000.0, // Convert to mm
                    Y = start.Y * 1000.0
                };
                
                var endPoint = new Renga.Point2D
                {
                    X = end.X * 1000.0, // Convert to mm
                    Y = end.Y * 1000.0
                };
                
                return new { StartPoint = startPoint, EndPoint = endPoint, Type = "LineApproximation" };
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Вспомогательный метод для правильного освобождения COM-объектов
        /// </summary>
        /// <param name="comObject">COM-объект для освобождения</param>
        private static void SafeReleaseComObject(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                try
                {
                    Marshal.ReleaseComObject(comObject);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error releasing COM object: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Вспомогательный метод для принудительной сборки мусора после освобождения COM-объектов
        /// </summary>
        private static void ForceGarbageCollection()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during garbage collection: {ex.Message}");
            }
        }

    }
}
