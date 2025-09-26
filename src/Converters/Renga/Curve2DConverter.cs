using System;
using System.Collections.Generic;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.Converters.Renga
{
    /// <summary>
    /// Converter for transforming DynRenga.RengaAPI.ICurve2D to Dynamo geometry
    /// Handles all Curve2D types: LineSegment, Arc, PolyCurve, and complex curves
    /// </summary>
    public class Curve2DConverter
    {

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve2D to Dynamo PolyCurve with optimized handling for closed curves
        /// </summary>
        /// <param name="curve2D">DynRenga.RengaAPI.ICurve2D to convert</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>Dynamo PolyCurve representing the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.PolyCurve ToDynamoPolyCurveOptimized(DynRenga.RengaAPI.ICurve2D curve2D, int partsPerMeter = 2)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve2D wrapper for compatibility
                var dynCurve2D = new DynRenga.DynGeometry.Curve2D(curve2D._i_Internal);
                
                // Check if it's a closed PolyCurve for optimized handling
                if (dynCurve2D.IsClosed && dynCurve2D.IsPolyCurve())
                {
                    return dynCurve2D.ToDynamoPolyCurveClosedPolyCurve(partsPerMeter);
                }
                
                // Use the standard safe conversion method
                return dynCurve2D.ToDynamoPolyCurveSafe(partsPerMeter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve2D to Dynamo PolyCurve (optimized): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve2D to a list of Dynamo curves
        /// Useful when you need individual curve segments
        /// </summary>
        /// <param name="curve2D">DynRenga.RengaAPI.ICurve2D to convert</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>List of Dynamo curves representing the curve segments</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<dg.Curve> ToDynamoCurves(DynRenga.RengaAPI.ICurve2D curve2D, int partsPerMeter = 2)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                var curves = new List<dg.Curve>();

                // Create a DynRenga.DynGeometry.Curve2D wrapper for compatibility
                var dynCurve2D = new DynRenga.DynGeometry.Curve2D(curve2D._i_Internal);

                // Check curve type and handle accordingly
                var curveType = dynCurve2D.Curve2DType.ToString();
                
                switch (curveType)
                {
                    case "Curve2DType_LineSegment":
                        // For line segments, create a simple line
                        var startPoint = dynCurve2D.GetBeginPoint();
                        var endPoint = dynCurve2D.GetEndPoint();
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
                        var segmentCount = dynCurve2D.GetSegmentCount();
                        if (segmentCount > 0)
                        {
                            for (int i = 0; i < segmentCount; i++)
                            {
                                var segment = dynCurve2D.GetSegment(i);
                                if (segment != null)
                                {
                                    // Create ICurve2D wrapper for segment
                                    var segmentICurve2D = new DynRenga.RengaAPI.ICurve2D(segment._i);
                                    var segmentPolyCurve = ToDynamoPolyCurveOptimized(segmentICurve2D, partsPerMeter);
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
        /// Converts DynRenga.RengaAPI.ICurve2D to Dynamo Line (only for LineSegment types)
        /// </summary>
        /// <param name="curve2D">DynRenga.RengaAPI.ICurve2D to convert (must be LineSegment type)</param>
        /// <returns>Dynamo Line or null if not a LineSegment</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Line ToDynamoLine(DynRenga.RengaAPI.ICurve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve2D wrapper for compatibility
                var dynCurve2D = new DynRenga.DynGeometry.Curve2D(curve2D._i_Internal);
                
                // Only convert if it's a line segment
                if (dynCurve2D.Curve2DType.ToString() != "Curve2DType_LineSegment")
                {
                    return null; // Not a line segment
                }

                var startPoint = dynCurve2D.GetBeginPoint();
                var endPoint = dynCurve2D.GetEndPoint();
                
                return dg.Line.ByStartPointEndPoint(startPoint, endPoint);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve2D to Dynamo Line: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve2D to Dynamo Arc (only for Arc types)
        /// </summary>
        /// <param name="curve2D">DynRenga.RengaAPI.ICurve2D to convert (must be Arc type)</param>
        /// <returns>Dynamo Arc or null if not an Arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Arc ToDynamoArc(DynRenga.RengaAPI.ICurve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve2D wrapper for compatibility
                var dynCurve2D = new DynRenga.DynGeometry.Curve2D(curve2D._i_Internal);
                
                // Only convert if it's an arc
                if (dynCurve2D.Curve2DType.ToString() != "Curve2DType_Arc")
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
        /// <param name="curve2D">DynRenga.RengaAPI.ICurve2D to analyze</param>
        /// <returns>Dictionary with curve information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "CurveType", "IsClosed", "Length", "ParameterRange", "SegmentCount", "BoundingBox" })]
        public static Dictionary<string, object> GetCurveInfo(DynRenga.RengaAPI.ICurve2D curve2D)
        {
            if (curve2D == null)
                throw new ArgumentNullException(nameof(curve2D), "Curve2D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve2D wrapper for compatibility
                var dynCurve2D = new DynRenga.DynGeometry.Curve2D(curve2D._i_Internal);
                
                var info = new Dictionary<string, object>
                {
                    { "CurveType", dynCurve2D.Curve2DType.ToString() },
                    { "IsClosed", dynCurve2D.IsClosed },
                    { "Length", dynCurve2D.GetLength },
                    { "ParameterRange", $"[{dynCurve2D.MinParameter}, {dynCurve2D.MaxParameter}]" },
                    { "SegmentCount", dynCurve2D.IsPolyCurve() ? dynCurve2D.GetSegmentCount() : 1 },
                    { "BoundingBox", dynCurve2D.GetGabarit() }
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
        /// <param name="curve2D">DynRenga.RengaAPI.ICurve2D to analyze</param>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetDebugInfo(DynRenga.RengaAPI.ICurve2D curve2D)
        {
            if (curve2D == null)
                return "❌ Curve2D is null";

            try
            {
                // Create a DynRenga.DynGeometry.Curve2D wrapper for compatibility
                var dynCurve2D = new DynRenga.DynGeometry.Curve2D(curve2D._i_Internal);
                
                var info = "🔧 Curve2D Converter Debug Information:\n";
                info += $"✅ Curve2D Type: {dynCurve2D.Curve2DType}\n";
                info += $"✅ Is Closed: {dynCurve2D.IsClosed}\n";
                info += $"✅ Length: {dynCurve2D.GetLength} meters\n";
                info += $"✅ Parameter Range: [{dynCurve2D.MinParameter}, {dynCurve2D.MaxParameter}]\n";
                
                if (dynCurve2D.IsPolyCurve())
                {
                    info += $"✅ Segment Count: {dynCurve2D.GetSegmentCount()}\n";
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
