using System;
using System.Collections.Generic;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.Converters.Renga.Geometry2D
{
    /// <summary>
    /// Converter for transforming DynRenga.DynGeometry.Curve2D to Dynamo geometry
    /// Handles all Curve2D types: LineSegment, Arc, PolyCurve, and complex curves
    /// </summary>
    public class Curve2DConverter
    {

        /// <summary>
        /// Converts DynRenga.DynGeometry.Curve2D to Dynamo PolyCurve with optimized handling for closed curves
        /// </summary>
        /// <param name="curve2D">DynRenga.DynGeometry.Curve2D to convert</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>Dynamo PolyCurve representing the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.PolyCurve ToDynamoPolyCurveOptimized(Curve2D curve2D, int partsPerMeter = 2)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Check if it's a closed PolyCurve for optimized handling
                if (curve2D.IsClosed && curve2D.IsPolyCurve())
                {
                    return curve2D.ToDynamoPolyCurveClosedPolyCurve(partsPerMeter);
                }
                
                // Use the standard safe conversion method
                return curve2D.ToDynamoPolyCurveSafe(partsPerMeter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve2D to Dynamo PolyCurve (optimized): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.DynGeometry.Curve2D to a list of Dynamo curves
        /// Useful when you need individual curve segments
        /// </summary>
        /// <param name="curve2D">DynRenga.DynGeometry.Curve2D to convert</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>List of Dynamo curves representing the curve segments</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<dg.Curve> ToDynamoCurves(Curve2D curve2D, int partsPerMeter = 2)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                var curves = new List<dg.Curve>();

                // Check curve type and handle accordingly
                var curveType = curve2D.Curve2DType.ToString();
                
                switch (curveType)
                {
                    case "Curve2DType_LineSegment":
                        // For line segments, create a simple line
                        var startPoint = curve2D.GetBeginPoint();
                        var endPoint = curve2D.GetEndPoint();
                        curves.Add(dg.Line.ByStartPointEndPoint(startPoint, endPoint));
                        break;

                    case "Curve2DType_Arc":
                        // For arcs, create a PolyCurve and extract curves
                        var arcPolyCurve = ToDynamoPolyCurveOptimized(curve2D, partsPerMeter);
                        if (arcPolyCurve != null)
                        {
                            curves.AddRange(arcPolyCurve.Curves());
                        }
                        break;

                    case "Curve2DType_PolyCurve":
                        // For PolyCurves, get individual segments
                        var segmentCount = curve2D.GetSegmentCount();
                        if (segmentCount > 0)
                        {
                            for (int i = 0; i < segmentCount; i++)
                            {
                                var segment = curve2D.GetSegment(i);
                                if (segment != null)
                                {
                                    var segmentPolyCurve = ToDynamoPolyCurveOptimized(segment, partsPerMeter);
                                    if (segmentPolyCurve != null)
                                    {
                                        curves.AddRange(segmentPolyCurve.Curves());
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Fallback to standard conversion
                            var polyCurve = ToDynamoPolyCurveOptimized(curve2D, partsPerMeter);
                            if (polyCurve != null)
                            {
                                curves.AddRange(polyCurve.Curves());
                            }
                        }
                        break;

                    default:
                        // For undefined or other types, use standard conversion
                        var defaultPolyCurve = ToDynamoPolyCurveOptimized(curve2D, partsPerMeter);
                        if (defaultPolyCurve != null)
                        {
                            curves.AddRange(defaultPolyCurve.Curves());
                        }
                        break;
                }

                return curves;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve2D to Dynamo curves: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.DynGeometry.Curve2D to Dynamo Line (only for LineSegment types)
        /// </summary>
        /// <param name="curve2D">DynRenga.DynGeometry.Curve2D to convert (must be LineSegment type)</param>
        /// <returns>Dynamo Line or null if not a LineSegment</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Line ToDynamoLine(Curve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Only convert if it's a line segment
                if (curve2D.Curve2DType.ToString() != "Curve2DType_LineSegment")
                {
                    return null; // Not a line segment
                }

                var startPoint = curve2D.GetBeginPoint();
                var endPoint = curve2D.GetEndPoint();
                
                return dg.Line.ByStartPointEndPoint(startPoint, endPoint);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve2D to Dynamo Line: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.DynGeometry.Curve2D to Dynamo Arc (only for Arc types)
        /// </summary>
        /// <param name="curve2D">DynRenga.DynGeometry.Curve2D to convert (must be Arc type)</param>
        /// <returns>Dynamo Arc or null if not an Arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Arc ToDynamoArc(Curve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Only convert if it's an arc
                if (curve2D.Curve2DType.ToString() != "Curve2DType_Arc")
                {
                    return null; // Not an arc
                }

                // For arcs, we need to create a PolyCurve and extract the arc
                var polyCurve = ToDynamoPolyCurveOptimized(curve2D, 2);
                if (polyCurve != null)
                {
                    var curves = polyCurve.Curves();
                    if (curves.Length > 0 && curves[0] is dg.Arc arc)
                    {
                        return arc;
                    }
                }

                return null; // Could not extract arc
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve2D to Dynamo Arc: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets curve information and type details
        /// </summary>
        /// <param name="curve2D">DynRenga.DynGeometry.Curve2D to analyze</param>
        /// <returns>Dictionary with curve information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "CurveType", "IsClosed", "Length", "ParameterRange", "SegmentCount", "BoundingBox" })]
        public static Dictionary<string, object> GetCurveInfo(Curve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                var info = new Dictionary<string, object>
                {
                    { "CurveType", curve2D.Curve2DType.ToString() },
                    { "IsClosed", curve2D.IsClosed },
                    { "Length", curve2D.GetLength },
                    { "ParameterRange", $"[{curve2D.MinParameter}, {curve2D.MaxParameter}]" },
                    { "SegmentCount", curve2D.IsPolyCurve() ? curve2D.GetSegmentCount() : 1 },
                    { "BoundingBox", curve2D.GetGabarit() }
                };

                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get curve information: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Gets debug information about the conversion process
        /// </summary>
        /// <param name="curve2D">DynRenga.DynGeometry.Curve2D to analyze</param>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetDebugInfo(Curve2D curve2D)
        {
            if (curve2D == null)
                return "❌ Curve2D is null";

            try
            {
                var info = "🔧 Curve2D Converter Debug Information:\n";
                info += $"✅ Curve2D Type: {curve2D.Curve2DType}\n";
                info += $"✅ Is Closed: {curve2D.IsClosed}\n";
                info += $"✅ Length: {curve2D.GetLength} meters\n";
                info += $"✅ Parameter Range: [{curve2D.MinParameter}, {curve2D.MaxParameter}]\n";
                
                if (curve2D.IsPolyCurve())
                {
                    info += $"✅ Segment Count: {curve2D.GetSegmentCount()}\n";
                }

                // Test conversion
                try
                {
                    var polyCurve = ToDynamoPolyCurveOptimized(curve2D, 2);
                    if (polyCurve != null)
                    {
                        info += $"✅ Conversion successful\n";
                        info += $"✅ Dynamo PolyCurve segments: {polyCurve.NumberOfCurves}\n";
                    }
                    else
                    {
                        info += $"❌ Conversion returned null\n";
                    }
                }
                catch (Exception ex)
                {
                    info += $"❌ Conversion failed: {ex.Message}\n";
                }

                return info;
            }
            catch (Exception ex)
            {
                return $"❌ Debug info failed: {ex.Message}";
            }
        }
    }
}
