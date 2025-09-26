using System;
using System.Collections.Generic;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.Converters.Dynamo
{
    /// <summary>
    /// Converter for transforming Dynamo geometry to DynRenga.RengaAPI.ICurve2D
    /// Handles conversion from Dynamo PolyCurve, Line, Arc, and other curve types to Renga Curve2D
    /// </summary>
    public class PolyCurveConverter
    {
        /// <summary>
        /// Converts Dynamo PolyCurve to DynRenga.RengaAPI.ICurve2D
        /// Uses RengaBIM SDK IMath interface to create composite curves
        /// </summary>
        /// <param name="polyCurve">Dynamo PolyCurve to convert</param>
        /// <param name="math">DynRenga.RengaAPI.IMath interface for curve creation</param>
        /// <param name="tolerance">Tolerance for curve connection (default: 1e-6)</param>
        /// <returns>DynRenga.RengaAPI.ICurve2D representing the polycurve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static DynRenga.RengaAPI.ICurve2D FromDynamoPolyCurve(dg.PolyCurve polyCurve, DynRenga.RengaAPI.IMath math, double tolerance = 1e-6)
        {
            if (polyCurve == null)
                throw new ArgumentNullException(nameof(polyCurve), "PolyCurve cannot be null");
            
            if (math == null)
                throw new ArgumentNullException(nameof(math), "IMath interface cannot be null");

            try
            {
                var debugInfo = new System.Text.StringBuilder();
                debugInfo.AppendLine("🔧 Dynamo PolyCurve to Curve2D Conversion:");
                debugInfo.AppendLine($"✅ Input PolyCurve segments: {polyCurve.NumberOfCurves}");
                debugInfo.AppendLine($"✅ Is closed: {polyCurve.IsClosed}");

                // Get individual curve segments
                var curves = polyCurve.Curves();
                if (curves == null || curves.Length == 0)
                {
                    throw new InvalidOperationException("PolyCurve contains no curve segments");
                }

                debugInfo.AppendLine($"✅ Extracted {curves.Length} curve segments");

                // Convert each segment to Renga Curve2D
                var rengaCurves = new List<ICurve2D>();
                
                for (int i = 0; i < curves.Length; i++)
                {
                    var curve = curves[i];
                    if (curve == null) continue;

                    try
                    {
                        var rengaCurve = ConvertSingleCurve(curve, math, tolerance);
                        if (rengaCurve != null)
                        {
                            rengaCurves.Add(rengaCurve);
                            debugInfo.AppendLine($"✅ Converted segment {i + 1}: {GetCurveTypeName(curve)}");
                        }
                        else
                        {
                            debugInfo.AppendLine($"⚠️ Failed to convert segment {i + 1}: {GetCurveTypeName(curve)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo.AppendLine($"❌ Error converting segment {i + 1}: {ex.Message}");
                        throw new InvalidOperationException($"Failed to convert curve segment {i + 1}: {ex.Message}", ex);
                    }
                }

                if (rengaCurves.Count == 0)
                {
                    throw new InvalidOperationException("No curve segments were successfully converted");
                }

                debugInfo.AppendLine($"✅ Successfully converted {rengaCurves.Count} segments");

                // Create composite curve if we have multiple segments
                ICurve2D resultCurve;
                if (rengaCurves.Count == 1)
                {
                    resultCurve = rengaCurves[0];
                    debugInfo.AppendLine("✅ Single segment - using directly");
                }
                else
                {
                    // Create SAFEARRAY for composite curve
                    var curveArray = new ICurve2D[rengaCurves.Count];
                    for (int i = 0; i < rengaCurves.Count; i++)
                    {
                        curveArray[i] = rengaCurves[i];
                    }

                    // Create composite curve
                    resultCurve = math._i_Internal.CreateCompositeCurve2D(curveArray);
                    debugInfo.AppendLine("✅ Created composite curve from segments");
                }

                // Wrap in DynRenga.RengaAPI.ICurve2D
                var dynCurve2D = new DynRenga.RengaAPI.ICurve2D(resultCurve);
                
                // Output debug info
                System.Diagnostics.Debug.WriteLine(debugInfo.ToString());
                
                return dynCurve2D;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Dynamo PolyCurve to Curve2D: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts a single Dynamo curve to Renga ICurve2D
        /// </summary>
        /// <param name="curve">Dynamo curve to convert</param>
        /// <param name="math">DynRenga.RengaAPI.IMath interface</param>
        /// <param name="tolerance">Tolerance for calculations</param>
        /// <returns>Renga ICurve2D or null if conversion fails</returns>
        private static ICurve2D ConvertSingleCurve(dg.Curve curve, DynRenga.RengaAPI.IMath math, double tolerance)
        {
            if (curve == null) return null;

            try
            {
                // Handle different curve types
                if (curve is dg.Line line)
                {
                    return ConvertLine(line, math);
                }
                else if (curve is dg.Arc arc)
                {
                    return ConvertArc(arc, math);
                }
                else if (curve is dg.Circle circle)
                {
                    return ConvertCircle(circle, math);
                }
                else if (curve is dg.NurbsCurve nurbsCurve)
                {
                    return ConvertNurbsCurve(nurbsCurve, math, tolerance);
                }
                else
                {
                    // For other curve types, try to approximate with line segments
                    return ConvertCurveByApproximation(curve, math, tolerance);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting curve: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts Dynamo Line to Renga ICurve2D
        /// </summary>
        private static ICurve2D ConvertLine(dg.Line line, DynRenga.RengaAPI.IMath math)
        {
            // Convert from Dynamo (meters) to Renga (millimeters)
            var startPoint = new Point2D { X = line.StartPoint.X * 1000.0, Y = line.StartPoint.Y * 1000.0 };
            var endPoint = new Point2D { X = line.EndPoint.X * 1000.0, Y = line.EndPoint.Y * 1000.0 };
            
            return math._i_Internal.CreateLineSegment2D(startPoint, endPoint);
        }

        /// <summary>
        /// Converts Dynamo Arc to Renga ICurve2D
        /// </summary>
        private static ICurve2D ConvertArc(dg.Arc arc, DynRenga.RengaAPI.IMath math)
        {
            // Get three points on the arc and convert from Dynamo (meters) to Renga (millimeters)
            var startPoint = new Point2D { X = arc.StartPoint.X * 1000.0, Y = arc.StartPoint.Y * 1000.0 };
            var midPoint = new Point2D { X = arc.PointAtParameter(0.5).X * 1000.0, Y = arc.PointAtParameter(0.5).Y * 1000.0 };
            var endPoint = new Point2D { X = arc.EndPoint.X * 1000.0, Y = arc.EndPoint.Y * 1000.0 };
            
            return math._i_Internal.CreateArc2DByThreePoints(startPoint, midPoint, endPoint);
        }

        /// <summary>
        /// Converts Dynamo Circle to Renga ICurve2D
        /// </summary>
        private static ICurve2D ConvertCircle(dg.Circle circle, DynRenga.RengaAPI.IMath math)
        {
            // Convert from Dynamo (meters) to Renga (millimeters)
            var centerPoint = new Point2D { X = circle.CenterPoint.X * 1000.0, Y = circle.CenterPoint.Y * 1000.0 };
            var radius = circle.Radius * 1000.0; // Convert radius from meters to millimeters
            
            return math._i_Internal.CreateCircle2D(centerPoint, radius);
        }

        /// <summary>
        /// Converts Dynamo NurbsCurve to Renga ICurve2D by approximation
        /// </summary>
        private static ICurve2D ConvertNurbsCurve(dg.NurbsCurve nurbsCurve, DynRenga.RengaAPI.IMath math, double tolerance)
        {
            // Sample points along the curve
            var points = new List<Point2D>();
            var startParam = 0.0;
            var endParam = 1.0;
            var step = (endParam - startParam) / 10; // Sample 10 points
            
            for (int i = 0; i <= 10; i++)
            {
                var param = startParam + i * step;
                var point = nurbsCurve.PointAtParameter(param);
                // Convert from Dynamo (meters) to Renga (millimeters)
                points.Add(new Point2D { X = point.X * 1000.0, Y = point.Y * 1000.0 });
            }

            // Create line segments between points
            var lineSegments = new List<ICurve2D>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                var lineSegment = math._i_Internal.CreateLineSegment2D(points[i], points[i + 1]);
                if (lineSegment != null)
                {
                    lineSegments.Add(lineSegment);
                }
            }

            if (lineSegments.Count == 0)
                return null;

            if (lineSegments.Count == 1)
                return lineSegments[0];

            // Create composite curve
            var curveArray = lineSegments.ToArray();
            return math._i_Internal.CreateCompositeCurve2D(curveArray);
        }

        /// <summary>
        /// Converts any Dynamo curve by approximation with line segments
        /// </summary>
        private static ICurve2D ConvertCurveByApproximation(dg.Curve curve, DynRenga.RengaAPI.IMath math, double tolerance)
        {
            // Sample points along the curve
            var points = new List<Point2D>();
            var startParam = 0.0;
            var endParam = 1.0;
            var step = (endParam - startParam) / 20; // Sample 20 points
            
            for (int i = 0; i <= 20; i++)
            {
                var param = startParam + i * step;
                var point = curve.PointAtParameter(param);
                // Convert from Dynamo (meters) to Renga (millimeters)
                points.Add(new Point2D { X = point.X * 1000.0, Y = point.Y * 1000.0 });
            }

            // Create line segments between points
            var lineSegments = new List<ICurve2D>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                var lineSegment = math._i_Internal.CreateLineSegment2D(points[i], points[i + 1]);
                if (lineSegment != null)
                {
                    lineSegments.Add(lineSegment);
                }
            }

            if (lineSegments.Count == 0)
                return null;

            if (lineSegments.Count == 1)
                return lineSegments[0];

            // Create composite curve
            var curveArray = lineSegments.ToArray();
            return math._i_Internal.CreateCompositeCurve2D(curveArray);
        }

        /// <summary>
        /// Gets a human-readable name for the curve type
        /// </summary>
        private static string GetCurveTypeName(dg.Curve curve)
        {
            if (curve is dg.Line) return "Line";
            if (curve is dg.Arc) return "Arc";
            if (curve is dg.Circle) return "Circle";
            if (curve is dg.NurbsCurve) return "NurbsCurve";
            if (curve is dg.PolyCurve) return "PolyCurve";
            return curve.GetType().Name;
        }

        /// <summary>
        /// Converts Dynamo Line to DynRenga.RengaAPI.ICurve2D
        /// </summary>
        /// <param name="line">Dynamo Line to convert</param>
        /// <param name="math">DynRenga.RengaAPI.IMath interface for curve creation</param>
        /// <returns>DynRenga.RengaAPI.ICurve2D representing the line</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static DynRenga.RengaAPI.ICurve2D FromDynamoLine(dg.Line line, DynRenga.RengaAPI.IMath math)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line), "Line cannot be null");
            
            if (math == null)
                throw new ArgumentNullException(nameof(math), "IMath interface cannot be null");

            try
            {
                var rengaCurve = ConvertLine(line, math);
                return new DynRenga.RengaAPI.ICurve2D(rengaCurve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Dynamo Line to Curve2D: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts Dynamo Arc to DynRenga.RengaAPI.ICurve2D
        /// </summary>
        /// <param name="arc">Dynamo Arc to convert</param>
        /// <param name="math">DynRenga.RengaAPI.IMath interface for curve creation</param>
        /// <returns>DynRenga.RengaAPI.ICurve2D representing the arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static DynRenga.RengaAPI.ICurve2D FromDynamoArc(dg.Arc arc, DynRenga.RengaAPI.IMath math)
        {
            if (arc == null)
                throw new ArgumentNullException(nameof(arc), "Arc cannot be null");
            
            if (math == null)
                throw new ArgumentNullException(nameof(math), "IMath interface cannot be null");

            try
            {
                var rengaCurve = ConvertArc(arc, math);
                return new DynRenga.RengaAPI.ICurve2D(rengaCurve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Dynamo Arc to Curve2D: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts Dynamo Circle to DynRenga.RengaAPI.ICurve2D
        /// </summary>
        /// <param name="circle">Dynamo Circle to convert</param>
        /// <param name="math">DynRenga.RengaAPI.IMath interface for curve creation</param>
        /// <returns>DynRenga.RengaAPI.ICurve2D representing the circle</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static DynRenga.RengaAPI.ICurve2D FromDynamoCircle(dg.Circle circle, DynRenga.RengaAPI.IMath math)
        {
            if (circle == null)
                throw new ArgumentNullException(nameof(circle), "Circle cannot be null");
            
            if (math == null)
                throw new ArgumentNullException(nameof(math), "IMath interface cannot be null");

            try
            {
                var rengaCurve = ConvertCircle(circle, math);
                return new DynRenga.RengaAPI.ICurve2D(rengaCurve);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Dynamo Circle to Curve2D: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets conversion information and statistics
        /// </summary>
        /// <param name="polyCurve">Dynamo PolyCurve to analyze</param>
        /// <returns>Dictionary with conversion information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "SegmentCount", "IsClosed", "TotalLength", "CurveTypes", "DebugInfo" })]
        public static Dictionary<string, object> GetConversionInfo(dg.PolyCurve polyCurve)
        {
            if (polyCurve == null)
                throw new ArgumentNullException(nameof(polyCurve), "PolyCurve cannot be null");

            try
            {
                var curves = polyCurve.Curves();
                var curveTypes = new List<string>();
                double totalLength = 0;

                if (curves != null)
                {
                    foreach (var curve in curves)
                    {
                        if (curve != null)
                        {
                            curveTypes.Add(GetCurveTypeName(curve));
                            totalLength += curve.Length;
                        }
                    }
                }

                var debugInfo = $"🔧 Dynamo PolyCurve Analysis:\n";
                debugInfo += $"✅ Segments: {polyCurve.NumberOfCurves}\n";
                debugInfo += $"✅ Is Closed: {polyCurve.IsClosed}\n";
                debugInfo += $"✅ Total Length: {totalLength:F3} units\n";
                debugInfo += $"✅ Curve Types: {string.Join(", ", curveTypes.Distinct())}\n";

                return new Dictionary<string, object>
                {
                    { "SegmentCount", polyCurve.NumberOfCurves },
                    { "IsClosed", polyCurve.IsClosed },
                    { "TotalLength", totalLength },
                    { "CurveTypes", curveTypes.Distinct().ToList() },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to analyze PolyCurve: {ex.Message}", ex);
            }
        }
    }
}
