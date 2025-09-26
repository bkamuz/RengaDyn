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
    /// Converter for transforming DynRenga.RengaAPI.ICurve3D to Dynamo geometry
    /// Handles all Curve3D types: LineSegment, Arc, PolyCurve, and complex curves
    /// </summary>
    public class Curve3DConverter
    {
        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve3D to a list of Dynamo curves
        /// Useful when you need individual curve segments
        /// </summary>
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to convert</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>List of Dynamo curves representing the curve segments</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<dg.Curve> ToDynamoCurves(DynRenga.RengaAPI.ICurve3D curve3D, int partsPerMeter = 2)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null");

            try
            {
                var curves = new List<dg.Curve>();

                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);

                // Check curve type and handle accordingly
                var curveType = dynCurve3D.GetCurve3DTypeAsString();
                
                switch (curveType)
                {
                    case "Curve3DType_LineSegment":
                        // For line segments, create a simple line
                        var startPoint = dynCurve3D.GetBeginPoint();
                        var endPoint = dynCurve3D.GetEndPoint();
                        curves.Add(dg.Line.ByStartPointEndPoint(startPoint, endPoint));
                        break;

                    case "Curve3DType_Arc":
                        // For arcs, create a PolyCurve and extract curves
                        var arcPolyCurve = ToDynamoPolyCurveOptimized(curve3D);
                        if (arcPolyCurve != null)
                        {
                            curves.AddRange(arcPolyCurve.Curves());
                        }
                        break;

                    case "Curve3DType_PolyCurve":
                        // For PolyCurves, use the existing conversion method
                        var polyCurve = ToDynamoPolyCurveOptimized(curve3D);
                        if (polyCurve != null)
                        {
                            curves.AddRange(polyCurve.Curves());
                        }
                        break;

                    default:
                        // For undefined or other types, use standard conversion
                        var defaultPolyCurve = ToDynamoPolyCurveOptimized(curve3D);
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
                throw new InvalidOperationException($"Failed to convert Curve3D to Dynamo curves: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve3D to Dynamo PolyCurve with optimized handling for closed curves
        /// </summary>
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to convert</param>
        /// <returns>Dynamo PolyCurve representing the curve</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.PolyCurve ToDynamoPolyCurveOptimized(DynRenga.RengaAPI.ICurve3D curve3D)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);
                
                // Check if it's a closed PolyCurve for optimized handling
                if (dynCurve3D.IsClosed && dynCurve3D.GetCurve3DTypeAsString() == "Curve3DType_PolyCurve")
                {
                    return dynCurve3D.ToDynamoPolyCurveClosedPolyCurve();
                }
                
                // Use the standard safe conversion method
                return dynCurve3D.ToDynamoPolyCurveSafe();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve3D to Dynamo PolyCurve (optimized): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve3D to Dynamo Line (only for LineSegment types)
        /// </summary>
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to convert (must be LineSegment type)</param>
        /// <returns>Dynamo Line or null if not a LineSegment</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Line ToDynamoLine(DynRenga.RengaAPI.ICurve3D curve3D)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);
                
                // Only convert if it's a line segment
                if (dynCurve3D.GetCurve3DTypeAsString() != "Curve3DType_LineSegment")
                {
                    return null; // Not a line segment
                }

                var startPoint = dynCurve3D.GetBeginPoint();
                var endPoint = dynCurve3D.GetEndPoint();
                
                return dg.Line.ByStartPointEndPoint(startPoint, endPoint);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve3D to Dynamo Line: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve3D to Dynamo Arc (only for Arc types)
        /// </summary>
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to convert (must be Arc type)</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>Dynamo Arc or null if not an Arc</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.Arc ToDynamoArc(DynRenga.RengaAPI.ICurve3D curve3D, int partsPerMeter = 2)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);
                
                // Only convert if it's an arc
                if (dynCurve3D.GetCurve3DTypeAsString() != "Curve3DType_Arc")
                {
                    return null; // Not an arc
                }

                // For arcs, we need to create a PolyCurve and extract the arc
                var polyCurve = ToDynamoPolyCurveOptimized(curve3D);
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
                throw new InvalidOperationException($"Failed to convert Curve3D to Dynamo Arc: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets curve information and type details
        /// </summary>
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to analyze</param>
        /// <returns>Dictionary with curve information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "CurveType", "IsClosed", "Length", "ParameterRange", "BoundingBox" })]
        public static Dictionary<string, object> GetCurveInfo(DynRenga.RengaAPI.ICurve3D curve3D)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);
                
                var info = new Dictionary<string, object>
                {
                    { "CurveType", dynCurve3D.GetCurve3DTypeAsString() },
                    { "IsClosed", dynCurve3D.IsClosed },
                    { "Length", dynCurve3D.GetLength },
                    { "ParameterRange", $"[{dynCurve3D.MinParameter}, {dynCurve3D.MaxParameter}]" },
                    { "BoundingBox", dynCurve3D.GetGabarit() }
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
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to analyze</param>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetDebugInfo(DynRenga.RengaAPI.ICurve3D curve3D)
        {
            if (curve3D == null)
                return "❌ Curve3D is null";

            try
            {
                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);
                
                var info = "🔧 Curve3D Converter Debug Information:\n";
                info += $"✅ Curve3D Type: {dynCurve3D.GetCurve3DTypeAsString()}\n";
                info += $"✅ Is Closed: {dynCurve3D.IsClosed}\n";
                info += $"✅ Length: {dynCurve3D.GetLength} meters\n";
                info += $"✅ Parameter Range: [{dynCurve3D.MinParameter}, {dynCurve3D.MaxParameter}]\n";

                // Test conversion
                try
                {
                    var polyCurve = ToDynamoPolyCurveOptimized(curve3D);
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

        /// <summary>
        /// Converts multiple DynRenga.RengaAPI.ICurve3D objects to a list of Dynamo curves
        /// </summary>
        /// <param name="curves3D">List of DynRenga.RengaAPI.ICurve3D to convert</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>List of Dynamo curves from all input curves</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<dg.Curve> ToDynamoCurvesMultiple(List<DynRenga.RengaAPI.ICurve3D> curves3D, int partsPerMeter = 2)
        {
            if (curves3D == null)
                throw new ArgumentNullException(nameof(curves3D), "Curves3D list cannot be null");

            try
            {
                var allCurves = new List<dg.Curve>();

                foreach (var curve3D in curves3D)
                {
                    if (curve3D != null)
                    {
                        var curves = ToDynamoCurves(curve3D, partsPerMeter);
                        allCurves.AddRange(curves);
                    }
                }

                return allCurves;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert multiple Curve3D objects to Dynamo curves: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts DynRenga.RengaAPI.ICurve3D to Dynamo NurbsCurve by approximation
        /// </summary>
        /// <param name="curve3D">DynRenga.RengaAPI.ICurve3D to convert</param>
        /// <param name="degree">Degree of the NURBS curve (default: 3)</param>
        /// <param name="partsPerMeter">Number of points per meter for curve discretization (default: 2)</param>
        /// <returns>Dynamo NurbsCurve or null if conversion fails</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.NurbsCurve ToDynamoNurbsCurve(DynRenga.RengaAPI.ICurve3D curve3D, int degree = 3, int partsPerMeter = 2)
        {
            if (curve3D == null)
                throw new ArgumentNullException(nameof(curve3D), "Curve3D cannot be null");

            try
            {
                // Create a DynRenga.DynGeometry.Curve3D wrapper for compatibility
                var dynCurve3D = new DynRenga.DynGeometry.Curve3D(curve3D._i);
                
                // Sample points along the curve
                var points = new List<dg.Point>();
                var startParam = dynCurve3D.MinParameter;
                var endParam = dynCurve3D.MaxParameter;
                var curveLength = dynCurve3D.GetLength;
                var step = curveLength / (curveLength * partsPerMeter);
                
                for (double param = startParam; param <= endParam; param += step)
                {
                    var point = dynCurve3D.GetPointOn(param);
                    points.Add(point);
                }

                if (points.Count < 2)
                    return null;

                // Create NURBS curve through points
                return dg.NurbsCurve.ByPoints(points, degree);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Curve3D to Dynamo NurbsCurve: {ex.Message}", ex);
            }
        }
    }
}
