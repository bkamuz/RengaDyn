using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.Converters.Renga
{
    /// <summary>
    /// Converter for Renga Point3D to Dynamo Point and vice versa
    /// Note: Renga coordinates are in millimeters; Dynamo uses meters.
    /// This converter applies millimeters to meters and meters to millimeters scaling
    /// (divide by 1000 for Renga->Dynamo, multiply by 1000 for Dynamo->Renga).
    /// </summary>
    public static class Point3DConverter
    {
        private const double MmToMeter = 1.0 / 1000.0;
        private const double MeterToMm = 1000.0;

        /// <summary>
        /// Convert Renga Point3D to Dynamo Point
        /// </summary>
        /// <param name="rengaPoint">Renga Point3D object</param>
        /// <returns>Dynamo Point</returns>
        [MultiReturn(new[] { "point", "debugInfo" })]
        public static Dictionary<string, object> RengaToDynamo(Point3D rengaPoint)
        {
            string debugInfo = "";
            Point dynamoPoint = null;

            try
            {
                // Apply scaling mm -> m
                dynamoPoint = Point.ByCoordinates(rengaPoint.X * MmToMeter, rengaPoint.Y * MmToMeter, rengaPoint.Z * MmToMeter);
                debugInfo = $"Successfully converted Renga Point3D ({rengaPoint.X}, {rengaPoint.Y}, {rengaPoint.Z}) mm to Dynamo Point ({dynamoPoint.X}, {dynamoPoint.Y}, {dynamoPoint.Z}) m";
            }
            catch (Exception ex)
            {
                debugInfo = $"Error converting Renga Point3D to Dynamo Point: {ex.Message}";
            }

            return new Dictionary<string, object>
            {
                { "point", dynamoPoint },
                { "debugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Convert Dynamo Point to Renga Point3D
        /// </summary>
        /// <param name="dynamoPoint">Dynamo Point object</param>
        /// <returns>Renga Point3D</returns>
        [MultiReturn(new[] { "rengaPoint", "debugInfo" })]
        public static Dictionary<string, object> DynamoToRenga(Point dynamoPoint)
        {
            string debugInfo = "";
            object rengaPoint = null;

            try
            {
                if (dynamoPoint == null)
                {
                    debugInfo = "Error: Input Dynamo Point is null";
                    return new Dictionary<string, object>
                    {
                        { "rengaPoint", null },
                        { "debugInfo", debugInfo }
                    };
                }

                // Create new Renga Point3D (scale m -> mm)
                var tempPoint = new Point3D();
                tempPoint.X = dynamoPoint.X * MeterToMm;
                tempPoint.Y = dynamoPoint.Y * MeterToMm;
                tempPoint.Z = dynamoPoint.Z * MeterToMm;
                rengaPoint = tempPoint;

                debugInfo = $"Successfully converted Dynamo Point ({dynamoPoint.X}, {dynamoPoint.Y}, {dynamoPoint.Z}) m to Renga Point3D ({tempPoint.X}, {tempPoint.Y}, {tempPoint.Z}) mm";
            }
            catch (Exception ex)
            {
                debugInfo = $"Error converting Dynamo Point to Renga Point3D: {ex.Message}";
            }

            return new Dictionary<string, object>
            {
                { "rengaPoint", rengaPoint },
                { "debugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Convert array of Renga Point3D to array of Dynamo Points
        /// </summary>
        /// <param name="rengaPoints">Array of Renga Point3D objects</param>
        /// <returns>Array of Dynamo Points</returns>
        [MultiReturn(new[] { "points", "debugInfo" })]
        public static Dictionary<string, object> RengaToDynamoArray(Point3D[] rengaPoints)
        {
            string debugInfo = "";
            Point[] dynamoPoints = null;

            try
            {
                if (rengaPoints == null || rengaPoints.Length == 0)
                {
                    debugInfo = "Error: Input Renga Point3D array is null or empty";
                    return new Dictionary<string, object>
                    {
                        { "points", new Point[0] },
                        { "debugInfo", debugInfo }
                    };
                }

                dynamoPoints = new Point[rengaPoints.Length];
                int successCount = 0;

                for (int i = 0; i < rengaPoints.Length; i++)
                {
                    // scale mm -> m
                    dynamoPoints[i] = Point.ByCoordinates(rengaPoints[i].X * MmToMeter, rengaPoints[i].Y * MmToMeter, rengaPoints[i].Z * MmToMeter);
                    successCount++;
                }

                debugInfo = $"Successfully converted {successCount} of {rengaPoints.Length} Renga Point3D objects to Dynamo Points (applied mm->m scaling)";
            }
            catch (Exception ex)
            {
                debugInfo = $"Error converting Renga Point3D array to Dynamo Points: {ex.Message}";
            }

            return new Dictionary<string, object>
            {
                { "points", dynamoPoints ?? new Point[0] },
                { "debugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Convert array of Dynamo Points to array of Renga Point3D
        /// </summary>
        /// <param name="dynamoPoints">Array of Dynamo Point objects</param>
        /// <returns>Array of Renga Point3D</returns>
        [MultiReturn(new[] { "rengaPoints", "debugInfo" })]
        public static Dictionary<string, object> DynamoToRengaArray(Point[] dynamoPoints)
        {
            string debugInfo = "";
            Point3D[] rengaPoints = null;

            try
            {
                if (dynamoPoints == null || dynamoPoints.Length == 0)
                {
                    debugInfo = "Error: Input Dynamo Point array is null or empty";
                    return new Dictionary<string, object>
                    {
                        { "rengaPoints", new Point3D[0] },
                        { "debugInfo", debugInfo }
                    };
                }

                rengaPoints = new Point3D[dynamoPoints.Length];
                int successCount = 0;

                for (int i = 0; i < dynamoPoints.Length; i++)
                {
                    if (dynamoPoints[i] != null)
                    {
                        rengaPoints[i] = new Point3D();
                        // scale m -> mm
                        rengaPoints[i].X = dynamoPoints[i].X * MeterToMm;
                        rengaPoints[i].Y = dynamoPoints[i].Y * MeterToMm;
                        rengaPoints[i].Z = dynamoPoints[i].Z * MeterToMm;
                        successCount++;
                    }
                }

                debugInfo = $"Successfully converted {successCount} of {dynamoPoints.Length} Dynamo Points to Renga Point3D objects (applied m->mm scaling)";
            }
            catch (Exception ex)
            {
                debugInfo = $"Error converting Dynamo Point array to Renga Point3D: {ex.Message}";
            }

            return new Dictionary<string, object>
            {
                { "rengaPoints", rengaPoints ?? new Point3D[0] },
                { "debugInfo", debugInfo }
            };
        }
    }
}