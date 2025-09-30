using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Aw.Plugin.Control
{
    public class RoomPerimeterData
    {
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public PerimeterCurveData PerimeterCurve { get; set; }
        public DateTime ExtractionTimestamp { get; set; }
    }

    public class PerimeterCurveData
    {
        public string CurveType { get; set; }
        public bool IsPolycurve { get; set; }
        public bool IsClosed { get; set; }
        public double Length { get; set; }
        public List<PerimeterSegmentData> Segments { get; set; } = new();
    }

    public class PerimeterSegmentData
    {
        public int Index { get; set; }
        public string Type { get; set; }
        public double Length { get; set; }
        public PerimeterLineGeometry2D Geometry { get; set; } = new();
    }

    public class PerimeterLineGeometry2D
    {
        public BaselinePoint2D Start { get; set; }
        public BaselinePoint2D End { get; set; }
    }

    public class FloorGridParameters
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public double Gap { get; set; }
        public double WallGap { get; set; }
        public double VerticalOffset { get; set; }
        public int MaterialId { get; set; }
        public double Thickness { get; set; }
        public double RoundingRadius { get; set; }
        public double RotationDegrees { get; set; }
        public bool LightFloodMode { get; set; }
        public bool MaskEmptyCells { get; set; }
        public double AlignTolerance { get; set; }
    }

    public class PerimeterBounds
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class FloorImportData
    {
        public BaselineCurve BaselineCurve { get; set; }
        public FloorParameters FloorParameters { get; set; }
    }

    public class BaselineCurve
    {
        public BaselinePoint2D BeginPoint { get; set; }
        public string CurveType { get; set; }
        public BaselinePoint2D EndPoint { get; set; }
        public bool IsClosed { get; set; }
        public bool IsPolycurve { get; set; }
        public double Length { get; set; }
        public List<object> Points { get; set; } = new();
        public string ReconstructionNotes { get; set; }
        public int SegmentCount { get; set; }
        public List<BaselineSegment> Segments { get; set; } = new();
    }

    public struct BaselinePoint2D
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class BaselineSegment
    {
        public string Type { get; set; }
        public JsonElement Geometry { get; set; }
        public double Length { get; set; }
        public int Index { get; set; }
    }

    public class LineSegment
    {
        public BaselinePoint2D Start { get; set; }
        public BaselinePoint2D End { get; set; }
        public double Length { get; set; }
        public string Type { get; set; } = "line";
    }

    public class ArcSegment
    {
        public BaselinePoint2D Center { get; set; }
        public BaselinePoint2D Start { get; set; }
        public BaselinePoint2D End { get; set; }
        public double Radius { get; set; }
        public double ArcAngleRadians { get; set; }
        public string Direction { get; set; }
        public string Type { get; set; } = "arc";
    }

    public class FloorParameters
    {
        public double VerticalOffset { get; set; }
        public int MaterialId { get; set; }
        public double Thickness { get; set; }
        public double RoundingRadius { get; set; }
    }

    public class JsonPoint3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public class PerimeterSegment3D
    {
        public JsonPoint3D Start { get; set; }
        public JsonPoint3D End { get; set; }
        public double Length { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
