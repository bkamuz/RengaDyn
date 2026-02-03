using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Renga;
using Aw.Interface;

namespace Aw.Plugin.Control
{
    /// <summary>
    /// Coordinates room perimeter extraction and grid creation for the Control plugin.
    /// </summary>
    public class RoomPerimeterManager
    {
    private readonly Renga.Application _app;
        private readonly List<RoomPerimeterData> _roomPerimeters = new();

        private readonly List<int> _batchCreatedFloorIds = new();

        private Dictionary<int, IExportedObject3D> _exported3DIndex;
        private Guid? _cachedModelId;
        private readonly Dictionary<int, (double x, double y, double z)> _fixtureCenterCache = new();
        private readonly Dictionary<int, (double minZ, double maxZ)> _roomZBoundsCache = new();

    private static readonly Guid GuidStyleLayeredMaterial = new("2cbc4964-61a0-4553-8103-36e2dd2ab31c");
    private static readonly Guid GuidThickness = new("f2712442-b9df-44fe-ac7b-c3524342c804");
    private static readonly Guid GuidVerticalOffset = new("337fd89b-763e-46dc-b1a9-7aecb253adbf");
    private static readonly Guid GuidRoundingRadius = new("3b97850d-d3ea-4a2d-91ff-0bc572d8795d");
    private static readonly Guid GuidHoleExtent = new("06a6b1fd-d15f-4cf2-8f9a-54d70abdfdf1");
    private static readonly Guid GuidOpeningDepth = new("dc6c60c7-ec3d-4449-b43b-c36eb9db42ee");
    private static readonly Guid GuidHoleDepth = new("fcdc44a1-da23-4bb5-8ff6-30fc180bf92f");
    private static readonly Guid GuidHoleVerticalOffset = new("3bb2c73c-70c8-48de-a4c2-e00634bef4a3");

        private class ColumnFootprint
        {
            public int Id { get; init; }
            public List<(double x, double y)> Polygon { get; init; }
            public (double minX, double minY, double maxX, double maxY) Bounds { get; init; }
            public (double x, double y) Centroid { get; init; }
        }

        private class TilePolygon
        {
            public List<(double x, double y)> Outline { get; set; }
            public List<List<(double x, double y)>> Holes { get; } = new();
            public List<OpeningSpec> OpeningSpecs { get; } = new();
        }

        private class OpeningSpec
        {
            public List<(double x, double y)> Outline { get; init; }
            public List<(double x, double y)> PreviewOutline { get; init; }
            public double VerticalOffset { get; init; }
            public double Thickness { get; init; }
            public double RoundingRadius { get; init; }
        }

        public RoomPerimeterManager(IApplication app, IUI ui)
        {
            _app = (Renga.Application)app;
            _ = ui;
        }

        public void Initialize()
        {
            // Initialization via Awada menu only; no UI hooks required here.
        }

        public void Stop()
        {
            InvalidateCaches();
        }

        #region Extraction

        public List<RoomPerimeterData> ExtractSelectedRoomPerimeters()
        {
            var results = new List<RoomPerimeterData>();
            try
            {
                Guid? currentModelId = null;
                try { currentModelId = _app?.Project?.Model?.Id; } catch { currentModelId = null; }
                if (currentModelId != _cachedModelId)
                {
                    InvalidateCaches();
                    _cachedModelId = currentModelId;
                    _roomPerimeters.Clear();
                    EnsureExportIndex(forceRebuild: true);
                }

                var selection = _app.Selection;
                var arr = selection.GetSelectedObjects();
                if (arr == null || arr.Length == 0) return results;

                var ids = new int[arr.Length];
                arr.CopyTo(ids, 0);
                var model = _app.Project?.Model;
                if (model == null) return results;
                var objects = model.GetObjects();

                foreach (var id in ids)
                {
                    var obj = objects.GetById(id);
                    if (obj == null) continue;
                    var data = ExtractPerimeter(obj, id);
                    if (data != null) results.Add(data);
                }

                if (!results.Any())
                {
                    try
                    {
                        EnsureExportIndex(forceRebuild: true);
                        arr = selection.GetSelectedObjects();
                        if (arr != null && arr.Length > 0)
                        {
                            ids = new int[arr.Length];
                            arr.CopyTo(ids, 0);
                            objects = model.GetObjects();
                            foreach (var id in ids)
                            {
                                var obj2 = objects.GetById(id);
                                if (obj2 == null) continue;
                                var data2 = ExtractPerimeter(obj2, id);
                                if (data2 != null) results.Add(data2);
                            }
                        }
                    }
                    catch
                    {
                        // ignore retry failures
                    }
                }

                if (results.Any())
                {
                    _roomPerimeters.Clear();
                    _roomPerimeters.AddRange(results);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка извлечения: {ex.Message}");
            }

            return results;
        }

        private RoomPerimeterData ExtractPerimeter(IModelObject modelObject, int objectId)
        {
            try
            {
                if (modelObject.ObjectType != ObjectTypes.Room) return null;
                var room = (IRoom)modelObject;
                var meshSegments = ExtractPerimeterFrom3DMesh(modelObject);
                if (meshSegments.Count > 0)
                {
                    return new RoomPerimeterData
                    {
                        ObjectId = objectId,
                        ObjectName = string.IsNullOrEmpty(room.RoomName) ? room.RoomNumber : room.RoomName,
                        ObjectType = "Room",
                        PerimeterCurve = ConvertSegmentsToPerimeterCurve(meshSegments),
                        ExtractionTimestamp = DateTime.Now
                    };
                }
            }
            catch
            {
            }

            return null;
        }

        private List<PerimeterSegment3D> ExtractPerimeterFrom3DMesh(IModelObject roomModelObject)
        {
            var segments = new List<PerimeterSegment3D>();
            try
            {
                if (_app?.Project == null) return segments;
                var exporter = _app.Project.DataExporter;
                if (exporter == null) return segments;
                var room3DObject = TryGetExportedObject3D(roomModelObject.Id);
                if (room3DObject == null) return segments;

                var allVertices = new List<FloatPoint3D>();
                var allTriangles = new List<(int v0, int v1, int v2)>();

                for (int meshIndex = 0; meshIndex < room3DObject.MeshCount; meshIndex++)
                {
                    var mesh = room3DObject.GetMesh(meshIndex);
                    for (int gridIndex = 0; gridIndex < mesh.GridCount; gridIndex++)
                    {
                        var grid = mesh.GetGrid(gridIndex);
                        int baseIndex = allVertices.Count;
                        for (int vi = 0; vi < grid.VertexCount; vi++)
                            allVertices.Add(grid.GetVertex(vi));
                        for (int ti = 0; ti < grid.TriangleCount; ti++)
                        {
                            grid.GetTriangleComponents(ti, out uint tv0, out uint tv1, out uint tv2);
                            allTriangles.Add(((int)tv0 + baseIndex, (int)tv1 + baseIndex, (int)tv2 + baseIndex));
                        }
                    }
                }

                if (allVertices.Count == 0) return segments;

                double minZ = allVertices.Min(p => p.Z);
                const double zTol = 0.001;
                var edgeCount = new Dictionary<(int, int), int>();

                (int, int) Normalize(int a, int b) => a < b ? (a, b) : (b, a);

                foreach (var tri in allTriangles)
                {
                    var p0 = allVertices[tri.v0];
                    var p1 = allVertices[tri.v1];
                    var p2 = allVertices[tri.v2];
                    if (Math.Abs(p0.Z - minZ) > zTol || Math.Abs(p1.Z - minZ) > zTol || Math.Abs(p2.Z - minZ) > zTol)
                        continue;

                    var e1 = Sub(p1, p0);
                    var e2 = Sub(p2, p0);
                    var normal = Cross(e1, e2);
                    double nLen = Len(normal);
                    if (nLen < 1e-9) continue;
                    if (Math.Abs(normal.Z) / nLen < 0.9) continue;

                    var edges = new[] { Normalize(tri.v0, tri.v1), Normalize(tri.v1, tri.v2), Normalize(tri.v2, tri.v0) };
                    foreach (var key in edges)
                    {
                        if (edgeCount.ContainsKey(key)) edgeCount[key] += 1; else edgeCount[key] = 1;
                    }
                }

                var boundaryEdges = edgeCount.Where(kv => kv.Value == 1).Select(kv => kv.Key).ToList();
                if (boundaryEdges.Count == 0) return segments;

                var adjacency = new Dictionary<int, List<int>>();
                foreach (var e in boundaryEdges)
                {
                    if (!adjacency.ContainsKey(e.Item1)) adjacency[e.Item1] = new();
                    if (!adjacency.ContainsKey(e.Item2)) adjacency[e.Item2] = new();
                    adjacency[e.Item1].Add(e.Item2);
                    adjacency[e.Item2].Add(e.Item1);
                }

                var visitedEdges = new HashSet<(int, int)>();
                var loops = new List<List<int>>();

                foreach (var e in boundaryEdges)
                {
                    if (visitedEdges.Contains(e)) continue;
                    int start = e.Item1;
                    int curr = start;
                    var loop = new List<int>();
                    int safety = 0;
                    while (true)
                    {
                        safety++;
                        if (safety > 50000) break;
                        loop.Add(curr);
                        var neighs = adjacency[curr];
                        int next = -1;
                        foreach (var n in neighs)
                        {
                            var key = Normalize(curr, n);
                            if (!visitedEdges.Contains(key)) { next = n; break; }
                        }
                        if (next == -1) break;
                        visitedEdges.Add(Normalize(curr, next));
                        curr = next;
                        if (curr == start) break;
                    }

                    if (loop.Count >= 2) loops.Add(loop);
                }

                if (loops.Count > 0)
                {
                    int bestIdx = -1;
                    double bestAreaAbs = 0.0;
                    for (int li = 0; li < loops.Count; li++)
                    {
                        var loop = loops[li];
                        if (loop.Count < 3) continue;
                        double area = 0.0;
                        for (int i = 0; i < loop.Count; i++)
                        {
                            var pa = allVertices[loop[i]];
                            var pb = allVertices[loop[(i + 1) % loop.Count]];
                            area += pa.X * pb.Y - pb.X * pa.Y;
                        }
                        double absArea = Math.Abs(0.5 * area);
                        if (absArea > bestAreaAbs) { bestAreaAbs = absArea; bestIdx = li; }
                    }

                    if (bestIdx >= 0)
                    {
                        var loop = loops[bestIdx];
                        for (int i = 0; i < loop.Count; i++)
                        {
                            int a = loop[i];
                            int b = loop[(i + 1) % loop.Count];
                            var pa = allVertices[a];
                            var pb = allVertices[b];
                            double length = Math.Sqrt(Math.Pow(pb.X - pa.X, 2) + Math.Pow(pb.Y - pa.Y, 2) + Math.Pow(pb.Z - pa.Z, 2));
                            segments.Add(new PerimeterSegment3D
                            {
                                Start = new JsonPoint3D { X = pa.X, Y = pa.Y, Z = pa.Z },
                                End = new JsonPoint3D { X = pb.X, Y = pb.Y, Z = pb.Z },
                                Length = length,
                                StartIndex = i,
                                EndIndex = (i + 1) % loop.Count
                            });
                        }
                    }
                }
            }
            catch
            {
            }

            return segments;
        }

        private static FloatVector3D Sub(FloatPoint3D a, FloatPoint3D b) => new() { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
        private static FloatVector3D Cross(FloatVector3D a, FloatVector3D b) => new() { X = a.Y * b.Z - a.Z * b.Y, Y = a.Z * b.X - a.X * b.Z, Z = a.X * b.Y - a.Y * b.X };
        private static double Len(FloatVector3D v) => Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

        private PerimeterCurveData ConvertSegmentsToPerimeterCurve(List<PerimeterSegment3D> segments)
        {
            var curveData = new PerimeterCurveData
            {
                CurveType = "2D",
                IsPolycurve = true,
                IsClosed = true,
                Length = segments.Sum(s => s.Length),
                Segments = new List<PerimeterSegmentData>()
            };

            for (int i = 0; i < segments.Count; i++)
            {
                var s = segments[i];
                curveData.Segments.Add(new PerimeterSegmentData
                {
                    Index = i,
                    Type = "line",
                    Length = s.Length,
                    Geometry = new PerimeterLineGeometry2D
                    {
                        Start = new BaselinePoint2D { X = s.Start.X, Y = s.Start.Y },
                        End = new BaselinePoint2D { X = s.End.X, Y = s.End.Y }
                    }
                });
            }

            return curveData;
        }

        #endregion

        #region Column avoidance helpers

        public List<(double x, double y)> OffsetPolygonOutward(List<(double x, double y)> polygon, double offset)
        {
            if (polygon == null || polygon.Count < 4 || Math.Abs(offset) < 1e-6) return polygon;
            const double scale = 1000.0;
            var subj = new Clipper2Lib.Path64();
            for (int i = 0; i < polygon.Count; i++)
                subj.Add(new Clipper2Lib.Point64((long)Math.Round(polygon[i].x * scale), (long)Math.Round(polygon[i].y * scale)));
            var paths = new Clipper2Lib.Paths64 { subj };
            var solution = Clipper2Lib.Clipper.InflatePaths(paths, offset * scale, Clipper2Lib.JoinType.Miter, Clipper2Lib.EndType.Polygon, 2.0);
            if (solution.Count == 0) return polygon;
            var best = solution.OrderByDescending(p => Math.Abs(Clipper2Lib.Clipper.Area(p))).First();
            var result = new List<(double x, double y)>();
            foreach (var pt in best)
                result.Add((pt.X / scale, pt.Y / scale));
            if (result.Count > 0 && (result[0].x != result[^1].x || result[0].y != result[^1].y)) result.Add(result[0]);
            return result;
        }

        private List<List<(double x, double y)>> DifferencePolygonWithPolygon(List<(double x, double y)> subject, List<(double x, double y)> clip)
        {
            var results = new List<List<(double x, double y)>>();
            if (subject == null || clip == null || subject.Count < 3 || clip.Count < 3) return results;
            const double scale = 1000.0;
            var subj = new Clipper2Lib.Paths64
            {
                new Clipper2Lib.Path64(subject.Select(p => new Clipper2Lib.Point64((long)Math.Round(p.x * scale), (long)Math.Round(p.y * scale))))
            };
            var clipPath = new Clipper2Lib.Paths64
            {
                new Clipper2Lib.Path64(clip.Select(p => new Clipper2Lib.Point64((long)Math.Round(p.x * scale), (long)Math.Round(p.y * scale))))
            };
            var solution = Clipper2Lib.Clipper.Difference(subj, clipPath, Clipper2Lib.FillRule.NonZero);
            foreach (var path in solution)
            {
                if (path.Count < 3) continue;
                var poly = new List<(double x, double y)>();
                foreach (var pt in path)
                    poly.Add((pt.X / scale, pt.Y / scale));
                if (poly.Count > 0 && (poly[0].x != poly[^1].x || poly[0].y != poly[^1].y)) poly.Add(poly[0]);
                results.Add(poly);
            }
            return results;
        }

        private static (double minX, double minY, double maxX, double maxY) ComputeBounds(List<(double x, double y)> polygon)
        {
            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
            for (int i = 0; i < polygon.Count; i++)
            {
                var p = polygon[i];
                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;
                if (p.y < minY) minY = p.y;
                if (p.y > maxY) maxY = p.y;
            }
            return (minX, minY, maxX, maxY);
        }

        private static bool BoundsIntersect((double minX, double minY, double maxX, double maxY) a, (double minX, double minY, double maxX, double maxY) b)
            => !(a.maxX < b.minX || a.minX > b.maxX || a.maxY < b.minY || a.minY > b.maxY);

        private static double PolygonArea(List<(double x, double y)> polygon)
        {
            if (polygon == null || polygon.Count < 3) return 0;
            double area = 0;
            for (int i = 0; i < polygon.Count - 1; i++)
            {
                area += polygon[i].x * polygon[i + 1].y - polygon[i + 1].x * polygon[i].y;
            }
            return area / 2.0;
        }

        private static (double x, double y) PolygonCentroid(List<(double x, double y)> polygon)
        {
            double area = PolygonArea(polygon);
            if (Math.Abs(area) < 1e-9) return (polygon[0].x, polygon[0].y);
            double cx = 0, cy = 0;
            for (int i = 0; i < polygon.Count - 1; i++)
            {
                double factor = (polygon[i].x * polygon[i + 1].y) - (polygon[i + 1].x * polygon[i].y);
                cx += (polygon[i].x + polygon[i + 1].x) * factor;
                cy += (polygon[i].y + polygon[i + 1].y) * factor;
            }
            area *= 6.0;
            return (cx / area, cy / area);
        }

        private List<ColumnFootprint> DetectColumnsInsideRoom(RoomPerimeterData room, double wallGap = 0.0)
        {
            var result = new List<ColumnFootprint>();
            try
            {
                var model = _app?.Project?.Model;
                if (model == null || room == null) return result;
                var roomPoly = GetRoomPolygon(room);
                if (roomPoly.Count < 4) return result;
                EnsureExportIndex(forceRebuild: false);
                var objects = model.GetObjects();
                for (int i = 0; i < objects.Count; i++)
                {
                    var obj = objects.GetByIndex(i);
                    if (obj == null || obj.ObjectType != ObjectTypes.Column) continue;
                    var footprint = ExtractColumnFootprint(obj);
                    if (footprint == null || footprint.Count < 4) continue;
                    var processedFootprint = wallGap > 0.0 ? OffsetPolygonOutward(footprint, wallGap) : footprint;
                    if (processedFootprint == null || processedFootprint.Count < 4) continue;
                    var centroid = PolygonCentroid(processedFootprint);
                    if (!PointInPolygon(roomPoly, centroid.x, centroid.y)) continue;
                    var bounds = ComputeBounds(processedFootprint);
                    if (!BoundsIntersect(bounds, ComputeBounds(roomPoly))) continue;
                    result.Add(new ColumnFootprint
                    {
                        Id = obj.Id,
                        Polygon = processedFootprint,
                        Bounds = bounds,
                        Centroid = centroid
                    });
                }
            }
            catch
            {
            }
            return result;
        }

        private List<(double x, double y)> ExtractColumnFootprint(IModelObject column)
        {
            try
            {
                var segments = ExtractPerimeterFrom3DMesh(column);
                if (segments == null || segments.Count == 0) return new List<(double x, double y)>();
                var ordered = segments.OrderBy(s => s.StartIndex).ToList();
                var poly = new List<(double x, double y)>();
                foreach (var seg in ordered)
                {
                    poly.Add((seg.Start.X, seg.Start.Y));
                }
                if (ordered.Count > 0)
                {
                    var last = ordered[^1];
                    poly.Add((last.End.X, last.End.Y));
                }
                if (poly.Count > 0 && (poly[0].x != poly[^1].x || poly[0].y != poly[^1].y)) poly.Add(poly[0]);
                return poly;
            }
            catch
            {
                return new List<(double x, double y)>();
            }
        }

        private List<TilePolygon> ApplyColumnAvoidanceToPolygon(List<(double x, double y)> polygon, List<ColumnFootprint> columns, double wallGap, double openingThickness, double openingVerticalOffset, double roundingRadius)
        {
            var results = new List<TilePolygon>();
            if (polygon == null || polygon.Count < 4) return results;
            if (columns == null || columns.Count == 0)
            {
                results.Add(new TilePolygon { Outline = polygon });
                return results;
            }

            var queue = new Queue<TilePolygon>();
            queue.Enqueue(new TilePolygon { Outline = polygon });

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                bool modified = false;
                var currentBounds = ComputeBounds(current.Outline);

                foreach (var column in columns)
                {
                    if (!BoundsIntersect(currentBounds, column.Bounds)) continue;
                    var expanded = column.Polygon;
                    if (expanded == null || expanded.Count < 4) continue;
                    var expandedBounds = column.Bounds;
                    if (!BoundsIntersect(currentBounds, expandedBounds)) continue;

                    var intersect = IntersectPolygonWithPolygon(expanded, current.Outline);
                    double intersectArea = intersect.Sum(poly => Math.Abs(PolygonArea(poly)));
                    bool containsExpanded = PolygonContainsPolygonApprox(current.Outline, expanded);
                    bool containsFootprint = containsExpanded || PolygonContainsPolygonApprox(current.Outline, column.Polygon);

                    if (containsFootprint)
                    {
                        var openingPolys = intersect
                            .Where(poly => poly != null && poly.Count >= 4)
                            .ToList();

                        if (openingPolys.Count == 0 && expanded != null && expanded.Count >= 4)
                            openingPolys.Add(expanded);

                        if (openingPolys.Count == 0 && column.Polygon != null && column.Polygon.Count >= 4)
                            openingPolys.Add(column.Polygon);

                        if (openingPolys.Count == 0)
                        {
                            var fallback = BuildOpeningOutline(column, wallGap);
                            if (fallback != null && fallback.Count >= 4)
                                openingPolys.Add(fallback);
                        }

                        foreach (var openingOutline in openingPolys)
                        {
                            var previewOutline = openingOutline;
                            if (OpeningAlreadyRegistered(current.OpeningSpecs, previewOutline))
                                continue;

                            current.OpeningSpecs.Add(new OpeningSpec
                            {
                                Outline = openingOutline,
                                PreviewOutline = previewOutline,
                                Thickness = openingThickness,
                                VerticalOffset = openingVerticalOffset,
                                RoundingRadius = roundingRadius
                            });
                        }
                        continue;
                    }

                    if (intersectArea < 1e-3)
                    {
                        // Column doesn't intersect; maybe it is fully inside without expansion
                        if (!containsFootprint) continue;
                        if (!OpeningAlreadyRegistered(current.OpeningSpecs, column.Polygon))
                        {
                            var openingOutline = BuildOpeningOutline(column, wallGap);
                            if (openingOutline != null)
                            {
                                current.OpeningSpecs.Add(new OpeningSpec
                                {
                                    Outline = openingOutline,
                                    PreviewOutline = column.Polygon,
                                    Thickness = openingThickness,
                                    VerticalOffset = openingVerticalOffset,
                                    RoundingRadius = roundingRadius
                                });
                            }
                        }
                        continue;
                    }

                    // Column intersects tile edge, subtract expanded footprint
                    var diff = DifferencePolygonWithPolygon(current.Outline, expanded);
                    if (diff.Count == 0)
                    {
                        // Entire polygon removed; skip
                        modified = true;
                        current = null;
                        break;
                    }

                    foreach (var part in diff)
                    {
                        var tp = new TilePolygon { Outline = part };
                        foreach (var existingOpening in current.OpeningSpecs)
                        {
                            if (PolygonContainsPolygonApprox(part, existingOpening.Outline))
                                tp.OpeningSpecs.Add(existingOpening);
                        }
                        queue.Enqueue(tp);
                    }
                    modified = true;
                    break;
                }

                if (!modified && current != null)
                {
                    results.Add(current);
                }
            }

            return results;
        }

        private bool PolygonContainsPolygonApprox(List<(double x, double y)> container, List<(double x, double y)> inner)
        {
            if (container == null || inner == null || container.Count < 4 || inner.Count < 4) return false;
            var centroid = PolygonCentroid(inner);
            if (!PointInPolygon(container, centroid.x, centroid.y)) return false;
            var intersection = IntersectPolygonWithPolygon(inner, container);
            if (intersection == null || intersection.Count == 0) return false;
            double intersectArea = intersection.Sum(poly => Math.Abs(PolygonArea(poly)));
            double innerArea = Math.Abs(PolygonArea(inner));
            if (innerArea < 1e-6) return false;
            return Math.Abs(innerArea - intersectArea) <= Math.Max(1e-6, 0.01 * innerArea);
        }

        private bool OpeningAlreadyRegistered(List<OpeningSpec> specs, List<(double x, double y)> candidatePreview)
        {
            if (specs == null || candidatePreview == null || candidatePreview.Count < 4) return false;
            foreach (var spec in specs)
            {
                var reference = spec?.PreviewOutline ?? spec?.Outline;
                if (reference == null || reference.Count < 4) continue;
                if (PolygonContainsPolygonApprox(reference, candidatePreview) && PolygonContainsPolygonApprox(candidatePreview, reference))
                    return true;
            }
            return false;
        }

        private List<(double x, double y)> BuildOpeningOutline(ColumnFootprint column, double wallGap)
        {
            try
            {
                double baseSize = Math.Max(50.0, wallGap * 2.0);
                double half = baseSize / 2.0;
                var center = column.Centroid;
                var outline = new List<(double x, double y)>
                {
                    (center.x - half, center.y - half),
                    (center.x + half, center.y - half),
                    (center.x + half, center.y + half),
                    (center.x - half, center.y + half),
                    (center.x - half, center.y - half)
                };
                return outline;
            }
            catch
            {
                return null;
            }
        }

        public List<List<(double x, double y)>> GetColumnFootprints(RoomPerimeterData room, double wallGap = 0.0)
        {
            var columns = DetectColumnsInsideRoom(room, wallGap);
            return columns.Select(c => c.Polygon).ToList();
        }

        public List<(List<(double x, double y)> outline, List<List<(double x, double y)>> openings)> BuildColumnAwareTilesForPreview(List<(double x, double y)> polygon, RoomPerimeterData room, FloorGridParameters grid)
        {
            var columns = DetectColumnsInsideRoom(room, grid?.WallGap ?? 0.0);
            var tiles = ApplyColumnAvoidanceToPolygon(polygon, columns, grid.WallGap, Math.Max(0.0, grid.Thickness) + 4.0, grid.VerticalOffset, grid.RoundingRadius);
            return tiles.Select(t => (t.Outline, t.OpeningSpecs.Select(o => o.PreviewOutline ?? o.Outline).ToList())).ToList();
        }

        #endregion

        #region Grid Creation

        public int CreateFloorsFromPerimeter(RoomPerimeterData roomPerimeter, FloorGridParameters grid)
        {
            if (_app?.Project?.Model == null) throw new InvalidOperationException("Проект не открыт");
            if (grid.Rows < 1) grid.Rows = 1;
            if (grid.Columns < 1) grid.Columns = 1;
            var model = _app.Project.Model;
            var created = 0;
            _batchCreatedFloorIds.Clear();
            var pendingOpenings = new List<(int floorId, List<OpeningSpec> specs)>();
            var columns = DetectColumnsInsideRoom(roomPerimeter, grid?.WallGap ?? 0.0);
            double openingThickness = Math.Max(0.0, grid.Thickness) + 4.0;
            double openingVerticalOffset = grid.VerticalOffset;
            double openingRounding = grid.RoundingRadius;

            if (grid.LightFloodMode)
            {
                var roomPolyLf = GetRoomPolygon(roomPerimeter);
                if (roomPolyLf.Count < 4) throw new InvalidOperationException("Периметр помещения не найден");
                var insetLf = OffsetPolygonInward(roomPolyLf, grid.WallGap);
                if (insetLf.Count < 4) throw new InvalidOperationException("Слишком большой отступ от стен");
                var lights = DetectLightingFixturesInsideRoom(roomPerimeter);
                if (lights.Count == 0) throw new InvalidOperationException("Нет светильников внутри помещения");
                var voronoiCells = BuildOrthogonalTessellation(insetLf, lights, grid.Gap, grid.RoundingRadius, grid.RotationDegrees, grid.AlignTolerance);

                var op = _app.Project.CreateOperationWithUndo(model.Id);
                op.Start();
                try
                {
                    foreach (var cell in voronoiCells)
                    {
                        if (cell.Count < 3) continue;
                        var tiles = ApplyColumnAvoidanceToPolygon(cell, columns, grid.WallGap, openingThickness, openingVerticalOffset, openingRounding);
                        if (tiles.Count == 0) continue;
                        foreach (var tile in tiles)
                        {
                            var f = PolygonToFloor(tile.Outline, grid);
                            if (f == null) continue;
                            var id = CreateFloorObjectInternal(model, f);
                            if (id > 0)
                            {
                                created++;
                                _batchCreatedFloorIds.Add(id);
                                if (tile.OpeningSpecs != null && tile.OpeningSpecs.Count > 0)
                                    pendingOpenings.Add((id, new List<OpeningSpec>(tile.OpeningSpecs)));
                            }
                        }
                    }
                    op.Apply();
                }
                catch
                {
                    try { op.Rollback(); } catch { }
                    throw;
                }

                if (pendingOpenings.Count > 0)
                    CreateOpeningsBatch(model, pendingOpenings);

                ApplyMaterialSecondPass(model, _batchCreatedFloorIds, grid.MaterialId);
                return created;
            }

            var roomPoly = GetRoomPolygon(roomPerimeter);
            if (roomPoly.Count < 4) throw new InvalidOperationException("Периметр помещения не найден");
            var insetPoly = OffsetPolygonInward(roomPoly, grid.WallGap);
            if (insetPoly.Count < 4) throw new InvalidOperationException("Слишком большой отступ от стен");

            var insetBounds = new PerimeterBounds
            {
                MinX = insetPoly.Min(p => p.x),
                MaxX = insetPoly.Max(p => p.x),
                MinY = insetPoly.Min(p => p.y),
                MaxY = insetPoly.Max(p => p.y)
            };
            insetBounds.Width = insetBounds.MaxX - insetBounds.MinX;
            insetBounds.Height = insetBounds.MaxY - insetBounds.MinY;

            double cxGlobal = (insetBounds.MinX + insetBounds.MaxX) / 2.0;
            double cyGlobal = (insetBounds.MinY + insetBounds.MaxY) / 2.0;
            double angleRad = (grid.RotationDegrees % 360.0) * Math.PI / 180.0;
            bool doRotate = Math.Abs(angleRad) > 1e-6;
            double cosA = Math.Cos(angleRad);
            double sinA = Math.Sin(angleRad);

            if (!doRotate && !IsPolygonOrthogonal(roomPoly))
            {
                for (int i = 0; i < roomPoly.Count - 1; i++)
                {
                    var a = roomPoly[i];
                    var b2 = roomPoly[i + 1];
                    double dx = b2.x - a.x;
                    double dy = b2.y - a.y;
                    double len = Math.Sqrt(dx * dx + dy * dy);
                    if (len > 1e-6)
                    {
                        angleRad = Math.Atan2(dy, dx);
                        cosA = Math.Cos(angleRad);
                        sinA = Math.Sin(angleRad);
                        doRotate = Math.Abs(angleRad) > 1e-6;
                        grid.RotationDegrees = angleRad * 180.0 / Math.PI;
                        break;
                    }
                }
            }

            double floorW;
            double floorH;
            double localMinX;
            double localMinY;

            if (!doRotate)
            {
                double availW = insetBounds.Width;
                double availH = insetBounds.Height;
                floorW = (availW - (grid.Columns - 1) * grid.Gap) / grid.Columns;
                floorH = (availH - (grid.Rows - 1) * grid.Gap) / grid.Rows;
                localMinX = insetBounds.MinX;
                localMinY = insetBounds.MinY;
            }
            else
            {
                var insetLocal = new List<(double x, double y)>();
                foreach (var pt in insetPoly)
                {
                    double dx = pt.x - cxGlobal;
                    double dy = pt.y - cyGlobal;
                    double lx = dx * cosA + dy * sinA;
                    double ly = -dx * sinA + dy * cosA;
                    insetLocal.Add((lx, ly));
                }

                double minLX = insetLocal.Min(p => p.x);
                double maxLX = insetLocal.Max(p => p.x);
                double minLY = insetLocal.Min(p => p.y);
                double maxLY = insetLocal.Max(p => p.y);
                double availW = maxLX - minLX;
                double availH = maxLY - minLY;
                floorW = (availW - (grid.Columns - 1) * grid.Gap) / grid.Columns;
                floorH = (availH - (grid.Rows - 1) * grid.Gap) / grid.Rows;
                localMinX = minLX;
                localMinY = minLY;
            }

            if (floorW <= 0 || floorH <= 0) throw new InvalidOperationException("Некорректные размеры ячеек");

            var lightsGlobal = grid.MaskEmptyCells ? DetectLightingFixturesInsideRoom(roomPerimeter) : null;
            bool hasLightsForMask = grid.MaskEmptyCells && lightsGlobal != null && lightsGlobal.Count > 0;
            List<(double x, double y)> lightsLocal = null;
            if (hasLightsForMask && doRotate)
            {
                lightsLocal = new List<(double x, double y)>(lightsGlobal.Count);
                foreach (var p in lightsGlobal)
                {
                    double dx = p.x - cxGlobal;
                    double dy = p.y - cyGlobal;
                    double lx = dx * cosA + dy * sinA;
                    double ly = -dx * sinA + dy * cosA;
                    lightsLocal.Add((lx, ly));
                }
            }

            var op1 = _app.Project.CreateOperationWithUndo(model.Id);
            op1.Start();
            try
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    for (int c = 0; c < grid.Columns; c++)
                    {
                        double x = insetBounds.MinX + c * (floorW + grid.Gap);
                        double y = insetBounds.MinY + r * (floorH + grid.Gap);
                        List<List<(double x, double y)>> clippedParts;
                        if (!doRotate)
                        {
                            if (hasLightsForMask)
                            {
                                double x2 = x + floorW;
                                double y2 = y + floorH;
                                bool any = false;
                                foreach (var L in lightsGlobal)
                                {
                                    if (L.x >= x && L.x <= x2 && L.y >= y && L.y <= y2) { any = true; break; }
                                }
                                if (!any) continue;
                            }
                            clippedParts = IntersectRectWithPolygon(x, y, floorW, floorH, insetPoly);
                        }
                        else
                        {
                            double baseLX = localMinX + c * (floorW + grid.Gap);
                            double baseLY = localMinY + r * (floorH + grid.Gap);
                            if (hasLightsForMask)
                            {
                                double lx2 = baseLX + floorW;
                                double ly2 = baseLY + floorH;
                                bool any = false;
                                foreach (var L in lightsLocal)
                                {
                                    if (L.x >= baseLX && L.x <= lx2 && L.y >= baseLY && L.y <= ly2) { any = true; break; }
                                }
                                if (!any) continue;
                            }
                            var rectLocal = new List<(double x, double y)>
                            {
                                (baseLX, baseLY),
                                (baseLX + floorW, baseLY),
                                (baseLX + floorW, baseLY + floorH),
                                (baseLX, baseLY + floorH)
                            };
                            var rectGlobal = new List<(double x, double y)>();
                            foreach (var lp in rectLocal)
                            {
                                double gx = cxGlobal + lp.x * cosA - lp.y * sinA;
                                double gy = cyGlobal + lp.x * sinA + lp.y * cosA;
                                rectGlobal.Add((gx, gy));
                            }
                            rectGlobal.Add(rectGlobal[0]);
                            clippedParts = IntersectPolygonWithPolygon(rectGlobal, insetPoly);
                        }

                        if (clippedParts.Count == 0) continue;
                        foreach (var part in clippedParts)
                        {
                            var tiles = ApplyColumnAvoidanceToPolygon(part, columns, grid.WallGap, openingThickness, openingVerticalOffset, openingRounding);
                            foreach (var tile in tiles)
                            {
                                var floorData = PolygonToFloor(tile.Outline, grid);
                                if (floorData == null) continue;
                                var id = CreateFloorObjectInternal(model, floorData);
                                if (id > 0)
                                {
                                    created++;
                                    _batchCreatedFloorIds.Add(id);
                                    if (tile.OpeningSpecs != null && tile.OpeningSpecs.Count > 0)
                                        pendingOpenings.Add((id, new List<OpeningSpec>(tile.OpeningSpecs)));
                                }
                            }
                        }
                    }
                }
                op1.Apply();
            }
            catch
            {
                try { op1.Rollback(); } catch { }
                throw;
            }

            if (pendingOpenings.Count > 0)
                CreateOpeningsBatch(model, pendingOpenings);

            ApplyMaterialSecondPass(model, _batchCreatedFloorIds, grid.MaterialId);
            return created;
        }

        private void ApplyMaterialSecondPass(IModel model, List<int> ids, int materialId)
        {
            try
            {
                if (ids == null || ids.Count == 0) return;
                if (materialId <= 0) return;
                var op2 = _app.Project.CreateOperationWithUndo(model.Id);
                op2.Start();
                try
                {
                    var modelObjs = model.GetObjects();
                    var matMgr = _app.Project.LayeredMaterialManager;
                    for (int i = 0; i < ids.Count; i++)
                    {
                        var obj = modelObjs.GetById(ids[i]);
                        if (obj == null) continue;
                        var pc = obj.GetParameters();
                        if (pc.Contains(GuidStyleLayeredMaterial))
                        {
                            var p = pc.Get(GuidStyleLayeredMaterial);
                            if (p.ValueType == ParameterValueType.ParameterValueType_Int)
                                p.SetIntValue(materialId);
                        }
                        if (matMgr != null)
                        {
                            var lm = matMgr.GetLayeredMaterial(materialId);
                            if (lm != null && lm.Layers != null && lm.Layers.Count > 0)
                            {
                                try
                                {
                                    if (pc.Contains(GuidThickness))
                                    {
                                        var baseLayer = lm.Layers.Get(lm.BaseLayerIndex);
                                        var tp = pc.Get(GuidThickness);
                                        if (tp.ValueType == ParameterValueType.ParameterValueType_Double)
                                            tp.SetDoubleValue(baseLayer.Thickness);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    op2.Apply();
                }
                catch
                {
                    try { op2.Rollback(); } catch { }
                }
            }
            catch
            {
            }
        }

        public List<List<(double x, double y)>> IntersectPolygonWithPolygon(List<(double x, double y)> subjPoly, List<(double x, double y)> clipPoly)
        {
            var results = new List<List<(double x, double y)>>();
            if (subjPoly == null || clipPoly == null || subjPoly.Count < 3 || clipPoly.Count < 3) return results;

            const double scale = 1000.0;
            var subj = new Clipper2Lib.Paths64
            {
                new Clipper2Lib.Path64(subjPoly.Select(p => new Clipper2Lib.Point64((long)Math.Round(p.x * scale), (long)Math.Round(p.y * scale))))
            };
            var clip = new Clipper2Lib.Paths64
            {
                new Clipper2Lib.Path64(clipPoly.Select(p => new Clipper2Lib.Point64((long)Math.Round(p.x * scale), (long)Math.Round(p.y * scale))))
            };
            var solution = Clipper2Lib.Clipper.Intersect(subj, clip, Clipper2Lib.FillRule.NonZero);
            foreach (var path in solution)
            {
                if (path.Count < 3) continue;
                var poly = new List<(double x, double y)>();
                foreach (var pt in path) poly.Add((pt.X / scale, pt.Y / scale));
                if (poly.Count > 0 && (poly[0].x != poly[^1].x || poly[0].y != poly[^1].y)) poly.Add(poly[0]);
                results.Add(poly);
            }
            return results;
        }

        private FloorImportData PolygonToFloor(List<(double x, double y)> poly, FloorGridParameters p)
        {
            if (poly == null || poly.Count < 4) return null;
            var segments = new List<BaselineSegment>();
            int idx = 0;
            double length = 0;
            double longestEdge = 0.0;
            for (int i = 0; i < poly.Count - 1; i++)
            {
                double dx = poly[i + 1].x - poly[i].x;
                double dy = poly[i + 1].y - poly[i].y;
                double elen = Math.Sqrt(dx * dx + dy * dy);
                if (elen > longestEdge) longestEdge = elen;
            }

            double maxAllowed = longestEdge / 3.0;
            double userRoundingMM = p.RoundingRadius;
            double effectiveRoundingMM = Math.Min(userRoundingMM, maxAllowed);
            if (effectiveRoundingMM < 0.1) effectiveRoundingMM = 0.0;
            bool doRound = effectiveRoundingMM > 0.0;

            if (doRound)
            {
                double area = 0;
                for (int i = 0; i < poly.Count - 1; i++)
                {
                    area += poly[i].x * poly[i + 1].y - poly[i + 1].x * poly[i].y;
                }
                bool ccw = area > 0;
                int n = poly.Count - 1;
                var cornerData = new List<(int idxCorner, double lenPrev, double lenNext, double interior, double maxR, bool convex, double uxPrev, double uyPrev, double uxNext, double uyNext)>();
                for (int i = 0; i < n; i++)
                {
                    var prev = poly[(i - 1 + n) % n];
                    var curr = poly[i];
                    var next = poly[(i + 1) % n];
                    double vx1 = curr.x - prev.x;
                    double vy1 = curr.y - prev.y;
                    double vx2 = next.x - curr.x;
                    double vy2 = next.y - curr.y;
                    double len1 = Math.Sqrt(vx1 * vx1 + vy1 * vy1);
                    double len2 = Math.Sqrt(vx2 * vx2 + vy2 * vy2);
                    if (len1 < 1e-6 || len2 < 1e-6)
                    {
                        cornerData.Add((i, 0, 0, 0, 0, false, 0, 0, 0, 0));
                        continue;
                    }
                    double ux1 = vx1 / len1;
                    double uy1 = vy1 / len1;
                    double ux2 = vx2 / len2;
                    double uy2 = vy2 / len2;
                    double dot = (-ux1) * ux2 + (-uy1) * uy2;
                    dot = Math.Clamp(dot, -1, 1);
                    double interior = Math.Acos(dot);
                    if (interior < 1e-6)
                    {
                        cornerData.Add((i, len1, len2, interior, 0, false, ux1, uy1, ux2, uy2));
                        continue;
                    }
                    double cross = (vx1 * vy2 - vy1 * vx2);
                    bool convex = ccw ? cross > 0 : cross < 0;
                    double angleHalf = convex ? (interior / 2.0) : ((Math.PI - interior) / 2.0);
                    if (angleHalf <= 1e-9)
                    {
                        cornerData.Add((i, len1, len2, interior, 0, convex, ux1, uy1, ux2, uy2));
                        continue;
                    }
                    double tanHalf = Math.Tan(angleHalf);
                    double maxRcorner = Math.Min(len1 * tanHalf, len2 * tanHalf);
                    cornerData.Add((i, len1, len2, interior, maxRcorner, convex, ux1, uy1, ux2, uy2));
                }

                var radii = new double[n];
                for (int i = 0; i < n; i++)
                {
                    var c = cornerData[i];
                    radii[i] = c.maxR <= 0 ? 0 : Math.Min(effectiveRoundingMM, c.maxR);
                }

                for (int pass = 0; pass < 3; pass++)
                {
                    bool adjusted = false;
                    for (int i = 0; i < n; i++)
                    {
                        int j = (i + 1) % n;
                        var c1 = cornerData[i];
                        var c2 = cornerData[j];
                        double edgeLen = c2.lenPrev;
                        if (edgeLen < 1e-6) continue;
                        if (radii[i] <= 0 && radii[j] <= 0) continue;
                        double angHalf1 = c1.convex ? (c1.interior / 2.0) : ((Math.PI - c1.interior) / 2.0);
                        double angHalf2 = c2.convex ? (c2.interior / 2.0) : ((Math.PI - c2.interior) / 2.0);
                        double off1 = radii[i] > 0 ? radii[i] / Math.Tan(Math.Max(angHalf1, 1e-9)) : 0;
                        double off2 = radii[j] > 0 ? radii[j] / Math.Tan(Math.Max(angHalf2, 1e-9)) : 0;
                        double sum = off1 + off2;
                        if (sum > edgeLen * 0.999)
                        {
                            double scale = (edgeLen * 0.999) / sum;
                            radii[i] *= scale;
                            radii[j] *= scale;
                            adjusted = true;
                        }
                    }
                    if (!adjusted) break;
                }

                int nCorners = cornerData.Count;
                var t1Pts = new (double x, double y)[nCorners];
                var t2Pts = new (double x, double y)[nCorners];
                var hasArc = new bool[nCorners];
                for (int i = 0; i < nCorners; i++)
                {
                    var c = cornerData[i];
                    var curr = poly[i];
                    double R = radii[i];
                    if (R > 0)
                    {
                        double angleHalf = c.convex ? (c.interior / 2.0) : ((Math.PI - c.interior) / 2.0);
                        double tanHalf = Math.Tan(angleHalf);
                        double offset = R / (Math.Abs(tanHalf) < 1e-12 ? 1e6 : tanHalf);
                        t1Pts[i] = (curr.x - c.uxPrev * offset, curr.y - c.uyPrev * offset);
                        t2Pts[i] = (curr.x + c.uxNext * offset, curr.y + c.uyNext * offset);
                        hasArc[i] = true;
                    }
                    else
                    {
                        t1Pts[i] = (curr.x, curr.y);
                        t2Pts[i] = (curr.x, curr.y);
                        hasArc[i] = false;
                    }
                }

                for (int i = 0; i < nCorners; i++)
                {
                    int next = (i + 1) % nCorners;
                    var lineStart = t2Pts[i];
                    var lineEnd = t1Pts[next];
                    if (Dist(lineStart.x, lineStart.y, lineEnd.x, lineEnd.y) > 1e-6)
                    {
                        var aPt = new BaselinePoint2D { X = lineStart.x, Y = lineStart.y };
                        var bPt = new BaselinePoint2D { X = lineEnd.x, Y = lineEnd.y };
                        var segL = Line(aPt, bPt, idx++);
                        length += segL.Length;
                        segments.Add(segL);
                    }
                    if (hasArc[next])
                    {
                        var c = cornerData[next];
                        double R = radii[next];
                        double bx = -c.uxPrev + c.uxNext;
                        double by = -c.uyPrev + c.uyNext;
                        double bl = Math.Sqrt(bx * bx + by * by);
                        if (bl < 1e-6) continue;
                        bx /= bl;
                        by /= bl;
                        double angleHalf = c.convex ? (c.interior / 2.0) : ((Math.PI - c.interior) / 2.0);
                        double distC = R / Math.Max(Math.Sin(angleHalf), 1e-9);
                        var center = (x: poly[next].x + bx * distC, y: poly[next].y + by * distC);
                        var t1 = t1Pts[next];
                        var t2 = t2Pts[next];
                        double cross2 = (t1.x - center.x) * (t2.y - center.y) - (t1.y - center.y) * (t2.x - center.x);
                        string dir = cross2 > 0 ? "counterclockwise" : "clockwise";
                        double arcAngle = Math.PI - c.interior;
                        var arc = new ArcSegment
                        {
                            Center = new BaselinePoint2D { X = center.x, Y = center.y },
                            Start = new BaselinePoint2D { X = t1.x, Y = t1.y },
                            End = new BaselinePoint2D { X = t2.x, Y = t2.y },
                            Radius = R,
                            ArcAngleRadians = arcAngle,
                            Direction = dir
                        };
                        var arcElem = System.Text.Json.JsonSerializer.SerializeToElement(arc);
                        var segArc = new BaselineSegment { Type = "arc", Geometry = arcElem, Length = R * arcAngle, Index = idx++ };
                        length += segArc.Length;
                        segments.Add(segArc);
                    }
                }

                if (segments.Count == 0)
                {
                    for (int i = 0; i < poly.Count - 1; i++)
                    {
                        var a = new BaselinePoint2D { X = poly[i].x, Y = poly[i].y };
                        var b = new BaselinePoint2D { X = poly[i + 1].x, Y = poly[i + 1].y };
                        var seg = Line(a, b, idx++);
                        length += seg.Length;
                        segments.Add(seg);
                    }
                }
            }
            else
            {
                for (int i = 0; i < poly.Count - 1; i++)
                {
                    var a = new BaselinePoint2D { X = poly[i].x, Y = poly[i].y };
                    var b = new BaselinePoint2D { X = poly[i + 1].x, Y = poly[i + 1].y };
                    var seg = Line(a, b, idx++);
                    length += seg.Length;
                    segments.Add(seg);
                }
            }

            var curve = new BaselineCurve
            {
                BeginPoint = segments.Count > 0
                    ? (segments[0].Type == "line"
                        ? segments[0].Geometry.Deserialize<LineSegment>().Start
                        : segments[0].Geometry.Deserialize<ArcSegment>().Start)
                    : new BaselinePoint2D { X = poly[0].x, Y = poly[0].y },
                EndPoint = segments.Count > 0
                    ? (segments[0].Type == "line"
                        ? segments[0].Geometry.Deserialize<LineSegment>().Start
                        : segments[0].Geometry.Deserialize<ArcSegment>().Start)
                    : new BaselinePoint2D { X = poly[0].x, Y = poly[0].y },
                CurveType = "2D",
                IsClosed = true,
                IsPolycurve = true,
                Length = length,
                Points = new List<object>(),
                ReconstructionNotes = "ClippedPolygon",
                SegmentCount = segments.Count,
                Segments = segments
            };
            var fp = new FloorParameters
            {
                VerticalOffset = p.VerticalOffset,
                MaterialId = p.MaterialId,
                Thickness = p.Thickness,
                RoundingRadius = effectiveRoundingMM
            };
            return new FloorImportData { BaselineCurve = curve, FloorParameters = fp };
        }

        private static double Dist(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private BaselineSegment Line(BaselinePoint2D a, BaselinePoint2D b, int i) => new()
        {
            Type = "line",
            Geometry = System.Text.Json.JsonSerializer.SerializeToElement(new LineSegment
            {
                Start = a,
                End = b,
                Length = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2)),
                Type = "line"
            }),
            Length = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2)),
            Index = i
        };

        public PerimeterBounds CalcBounds(PerimeterCurveData cd)
        {
            if (cd?.Segments == null || cd.Segments.Count == 0) return null;
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (var s in cd.Segments)
            {
                if (s.Geometry == null) continue;
                var st = s.Geometry.Start;
                var en = s.Geometry.End;
                minX = Math.Min(minX, st.X);
                maxX = Math.Max(maxX, st.X);
                minY = Math.Min(minY, st.Y);
                maxY = Math.Max(maxY, st.Y);
                minX = Math.Min(minX, en.X);
                maxX = Math.Max(maxX, en.X);
                minY = Math.Min(minY, en.Y);
                maxY = Math.Max(maxY, en.Y);
            }
            if (minX == double.MaxValue) return null;
            return new PerimeterBounds
            {
                MinX = minX,
                MinY = minY,
                MaxX = maxX,
                MaxY = maxY,
                Width = maxX - minX,
                Height = maxY - minY
            };
        }

        #endregion

        #region Materials & Undo

        public Dictionary<int, string> GetLayeredMaterials()
        {
            var dict = new Dictionary<int, string>();
            try
            {
                if (_app?.Project == null) return dict;
                var mgr = _app.Project.LayeredMaterialManager;
                var all = _app.Project.LayeredMaterials;
                for (int i = 0; i < all.Count; i++)
                {
                    var ent = all.GetByIndex(i);
                    var lm = mgr.GetLayeredMaterial(ent.Id);
                    if (lm != null)
                    {
                        var pair = lm.GetIdGroupPair();
                        if (pair.Group == LayeredMaterialGroup.LayeredMaterialGroup_Floor)
                            dict[lm.Id] = lm.Name;
                    }
                }
            }
            catch
            {
            }
            return dict;
        }

        public void ResolveMaterialParameters(ref FloorGridParameters p)
        {
            try
            {
                if (p.MaterialId <= 0 || _app?.Project?.LayeredMaterialManager == null) return;
                var matMgr = _app.Project.LayeredMaterialManager;
                var lm = matMgr.GetLayeredMaterial(p.MaterialId);
                if (lm == null) return;
                if (lm.Layers != null && lm.Layers.Count > 0)
                {
                    int baseIdx = lm.BaseLayerIndex;
                    if (baseIdx >= 0 && baseIdx < lm.Layers.Count)
                    {
                        var baseLayer = lm.Layers.Get(baseIdx);
                        p.Thickness = baseLayer.Thickness;
                    }
                }
            }
            catch
            {
            }
        }

        public bool UndoTwoSteps()
        {
            try
            {
                var proj = _app?.Project;
                if (proj == null) return false;
                var model = proj.Model;
                if (model == null) return false;
                var stack = proj.GetUndoStack(model.Id);
                if (stack == null) return false;
                try { stack.Undo(); }
                catch { return false; }
                try { stack.Undo(); }
                catch { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private int CreateFloorObjectInternal(IModel model, FloorImportData floorData)
        {
            var args = model.CreateNewEntityArgs();
            args.TypeId = ObjectTypes.Floor;
            args.Placement3D = new Placement3D
            {
                Origin = new Point3D { X = 0, Y = 0, Z = 0 },
                xAxis = new Vector3D { X = 1, Y = 0, Z = 0 },
                zAxis = new Vector3D { X = 0, Y = 0, Z = 1 }
            };
            var floor = model.CreateObject(args);
            try
            {
                if (floor is IBaseline2DObject base2D && floorData.BaselineCurve != null)
                {
                    var math = _app.Math;
                    var curve = BuildCurve(math, floorData.BaselineCurve);
                    if (curve != null)
                    {
                        var placement2D = new Placement2D { Origin = new Point2D { X = 0, Y = 0 }, xAxis = new Vector2D { X = 1, Y = 0 } };
                        base2D.SetBaselineInCS(placement2D, curve);
                    }
                }
                if (floorData.FloorParameters != null)
                {
                    var pc = floor.GetParameters();
                    if (pc.Contains(GuidThickness)) pc.Get(GuidThickness).SetDoubleValue(floorData.FloorParameters.Thickness);
                    if (pc.Contains(GuidVerticalOffset)) pc.Get(GuidVerticalOffset).SetDoubleValue(floorData.FloorParameters.VerticalOffset);
                    if (pc.Contains(GuidRoundingRadius)) pc.Get(GuidRoundingRadius).SetDoubleValue(floorData.FloorParameters.RoundingRadius / 1000.0);
                    if (floorData.FloorParameters.MaterialId > 0)
                    {
                        if (pc.Contains(GuidStyleLayeredMaterial))
                        {
                            var mp = pc.Get(GuidStyleLayeredMaterial);
                            if (mp.ValueType == ParameterValueType.ParameterValueType_Int)
                                mp.SetIntValue(floorData.FloorParameters.MaterialId);
                        }
                    }
                }
            }
            catch
            {
            }
            return floor.Id;
        }

        private void CreateOpeningsBatch(IModel model, List<(int floorId, List<OpeningSpec> specs)> pending)
        {
            if (model == null || pending == null || pending.Count == 0) return;

            var project = _app?.Project;
            var op = project?.CreateOperationWithUndo(model.Id);
            if (op == null)
            {
                foreach (var entry in pending)
                    CreateOpeningsForTile(model, entry.floorId, entry.specs);
                return;
            }

            op.Start();
            try
            {
                foreach (var entry in pending)
                    CreateOpeningsForTile(model, entry.floorId, entry.specs);
                op.Apply();
            }
            catch
            {
                try { op.Rollback(); } catch { }
                throw;
            }
        }

        private void CreateOpeningsForTile(IModel model, int hostFloorId, List<OpeningSpec> specs)
        {
            if (specs == null || specs.Count == 0) return;
            foreach (var spec in specs)
            {
                var openingParams = new FloorGridParameters
                {
                    VerticalOffset = spec.VerticalOffset,
                    Thickness = spec.Thickness,
                    RoundingRadius = spec.RoundingRadius,
                    MaterialId = 0
                };
                var openingData = PolygonToFloor(spec.Outline, openingParams);
                if (openingData == null) continue;
                CreateOpeningObjectInternal(model, hostFloorId, openingData, spec.Outline);
            }
        }

        private void CreateOpeningObjectInternal(IModel model, int hostFloorId, FloorImportData openingData, List<(double x, double y)> outline)
        {
            if (model == null || openingData?.BaselineCurve == null) return;
            var args = model.CreateNewEntityArgs();
            args.TypeId = ObjectTypes.Opening;
            args.HostObjectId = hostFloorId;
            args.Placement3D = new Placement3D
            {
                Origin = new Point3D { X = 0, Y = 0, Z = 0 },
                xAxis = new Vector3D { X = 1, Y = 0, Z = 0 },
                zAxis = new Vector3D { X = 0, Y = 0, Z = 1 }
            };
            var opening = model.CreateObject(args);
            if (opening == null) return;

            if (opening is IBaseline2DObject base2D)
            {
                var math = _app.Math;
                var curve = BuildCurve(math, openingData.BaselineCurve);
                if (curve != null)
                {
                    var placement2D = new Placement2D
                    {
                        Origin = new Point2D { X = 0, Y = 0 },
                        xAxis = new Vector2D { X = 1, Y = 0 }
                    };
                    base2D.SetBaselineInCS(placement2D, curve);
                }
            }

            var pc = opening.GetParameters();
            if (pc != null && openingData.FloorParameters != null)
            {
                double thickness = Math.Max(0.0, openingData.FloorParameters.Thickness);
                double verticalOffset = openingData.FloorParameters.VerticalOffset;
                double rounding = openingData.FloorParameters.RoundingRadius / 1000.0;

                if (pc.Contains(GuidThickness))
                    pc.Get(GuidThickness).SetDoubleValue(thickness);
                if (pc.Contains(GuidOpeningDepth))
                    pc.Get(GuidOpeningDepth).SetDoubleValue(thickness);
                if (pc.Contains(GuidHoleDepth))
                    pc.Get(GuidHoleDepth).SetDoubleValue(thickness);
                if (pc.Contains(GuidHoleExtent))
                    pc.Get(GuidHoleExtent).SetIntValue(0);
                if (pc.Contains(GuidVerticalOffset))
                    pc.Get(GuidVerticalOffset).SetDoubleValue(verticalOffset);
                if (pc.Contains(GuidHoleVerticalOffset))
                    pc.Get(GuidHoleVerticalOffset).SetDoubleValue(verticalOffset);
                if (pc.Contains(GuidRoundingRadius))
                    pc.Get(GuidRoundingRadius).SetDoubleValue(rounding);
            }
        }

        private void TranslateBaselineCurve(BaselineCurve curve, double dx, double dy)
        {
            if (curve == null) return;

            curve.BeginPoint = new BaselinePoint2D
            {
                X = curve.BeginPoint.X + dx,
                Y = curve.BeginPoint.Y + dy
            };
            curve.EndPoint = new BaselinePoint2D
            {
                X = curve.EndPoint.X + dx,
                Y = curve.EndPoint.Y + dy
            };

            for (int i = 0; i < curve.Segments.Count; i++)
            {
                var seg = curve.Segments[i];
                if (seg.Type == "line")
                {
                    var geo = seg.Geometry.Deserialize<LineSegment>();
                    geo.Start = new BaselinePoint2D { X = geo.Start.X + dx, Y = geo.Start.Y + dy };
                    geo.End = new BaselinePoint2D { X = geo.End.X + dx, Y = geo.End.Y + dy };
                    curve.Segments[i].Geometry = JsonSerializer.SerializeToElement(geo);
                }
                else if (seg.Type == "arc")
                {
                    try
                    {
                        var geo = seg.Geometry.Deserialize<ArcSegment>();
                        geo.Center = new BaselinePoint2D { X = geo.Center.X + dx, Y = geo.Center.Y + dy };
                        geo.Start = new BaselinePoint2D { X = geo.Start.X + dx, Y = geo.Start.Y + dy };
                        geo.End = new BaselinePoint2D { X = geo.End.X + dx, Y = geo.End.Y + dy };
                        curve.Segments[i].Geometry = JsonSerializer.SerializeToElement(geo);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private ICurve2D BuildCurve(IMath math, BaselineCurve curveData)
        {
            try
            {
                if (curveData == null || curveData.Segments == null || curveData.Segments.Count == 0) return null;
                var segCurves = new List<ICurve2D>();
                foreach (var seg in curveData.Segments)
                {
                    if (seg.Type == "line")
                    {
                        var geo = seg.Geometry.Deserialize<LineSegment>();
                        var ls = math.CreateLineSegment2D(new Point2D { X = geo.Start.X, Y = geo.Start.Y }, new Point2D { X = geo.End.X, Y = geo.End.Y });
                        segCurves.Add(ls);
                    }
                    else if (seg.Type == "arc")
                    {
                        try
                        {
                            var geo = seg.Geometry.Deserialize<ArcSegment>();
                            var center = new Point2D { X = geo.Center.X, Y = geo.Center.Y };
                            var start = new Point2D { X = geo.Start.X, Y = geo.Start.Y };
                            var end = new Point2D { X = geo.End.X, Y = geo.End.Y };
                            bool cw = geo.Direction == "clockwise";
                            ICurve2D arc = null;
                            try { arc = math.CreateArc2DByCenterStartEndPoints(center, start, end, cw); }
                            catch
                            {
                                try { arc = math.CreateArc2DByCenterStartEndPoints(center, start, end, !cw); }
                                catch { arc = math.CreateArc2DByCenterStartEndPoints(center, end, start, cw); }
                            }
                            if (arc != null) segCurves.Add(arc);
                        }
                        catch
                        {
                        }
                    }
                }
                if (segCurves.Count == 1) return segCurves[0];
                if (segCurves.Count > 1) return math.CreateCompositeCurve2D(segCurves.ToArray());
                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<(double x, double y)> GetRoomPolygon(RoomPerimeterData room)
        {
            var pts = new List<(double x, double y)>();
            if (room?.PerimeterCurve?.Segments == null) return pts;
            foreach (var s in room.PerimeterCurve.Segments)
            {
                if (s.Geometry != null)
                {
                    var st = s.Geometry.Start;
                    double x = st.X;
                    double y = st.Y;
                    if (pts.Count == 0 || pts[^1].x != x || pts[^1].y != y) pts.Add((x, y));
                }
            }
            if (pts.Count > 0 && (pts[0].x != pts[^1].x || pts[0].y != pts[^1].y)) pts.Add(pts[0]);
            return pts;
        }

        public List<(double x, double y)> OffsetPolygonInward(List<(double x, double y)> polygon, double inset)
        {
            if (polygon == null || polygon.Count < 4 || inset <= 0) return polygon;
            const double scale = 1000.0;
            var subj = new Clipper2Lib.Path64();
            for (int i = 0; i < polygon.Count; i++)
                subj.Add(new Clipper2Lib.Point64((long)Math.Round(polygon[i].x * scale), (long)Math.Round(polygon[i].y * scale)));
            var paths = new Clipper2Lib.Paths64 { subj };
            var solution = Clipper2Lib.Clipper.InflatePaths(paths, -inset * scale, Clipper2Lib.JoinType.Miter, Clipper2Lib.EndType.Polygon, 2.0);
            if (solution.Count == 0) return polygon;
            var best = solution.OrderByDescending(p => Clipper2Lib.Clipper.Area(p)).First();
            var result = new List<(double x, double y)>();
            foreach (var pt in best)
            {
                result.Add((pt.X / scale, pt.Y / scale));
            }
            if (result.Count > 0 && (result[0].x != result[^1].x || result[0].y != result[^1].y)) result.Add(result[0]);
            return result;
        }

        public List<List<(double x, double y)>> IntersectRectWithPolygon(double x, double y, double w, double h, List<(double x, double y)> polygon)
        {
            var rect = new List<(double x, double y)>
            {
                (x, y), (x + w, y), (x + w, y + h), (x, y + h), (x, y)
            };
            return IntersectPolygonWithPolygon(rect, polygon);
        }

        public bool IsPolygonOrthogonal(List<(double x, double y)> poly, double toleranceDegrees = 1.0)
        {
            if (poly == null || poly.Count < 3) return true;
            double tol = Math.Max(0.1, toleranceDegrees) * Math.PI / 180.0;
            for (int i = 0; i < poly.Count - 1; i++)
            {
                var a = poly[i];
                var b = poly[i + 1];
                double dx = b.x - a.x;
                double dy = b.y - a.y;
                double len = Math.Sqrt(dx * dx + dy * dy);
                if (len < 1e-6) continue;
                double ang = Math.Abs(Math.Atan2(dy, dx));
                double halfPi = Math.PI / 2.0;
                double rem = ang % halfPi;
                double dev = Math.Min(rem, halfPi - rem);
                if (dev > tol) return false;
            }
            return true;
        }

        public List<(double x, double y)> DetectLightingFixturesInsideRoom(RoomPerimeterData room)
        {
            var lights = new List<(double x, double y)>();
            try
            {
                if (_app?.Project?.Model == null) return lights;

                EnsureExportIndex(forceRebuild: true);
                _fixtureCenterCache.Clear();
                _roomZBoundsCache.Remove(room.ObjectId);

                var poly = GetRoomPolygon(room);
                if (poly.Count < 4) return lights;
                double minX = poly.Min(p => p.x);
                double maxX = poly.Max(p => p.x);
                double minY = poly.Min(p => p.y);
                double maxY = poly.Max(p => p.y);
                var objs = _app.Project.Model.GetObjects();
                var roomZ = TryGetObjectZBounds(room.ObjectId);
                const double upperBuffer = 0.05;
                const double lowerTol = 0.01;
                for (int i = 0; i < objs.Count; i++)
                {
                    var o = objs.GetByIndex(i);
                    if (o.ObjectType != ObjectTypes.LightingFixture) continue;
                    if (!TryGetFixtureCenter(o.Id, out double x, out double y, out double z)) continue;
                    if (roomZ.HasValue)
                    {
                        var (rzMin, rzMax) = roomZ.Value;
                        if (z < rzMin - lowerTol || z > rzMax + upperBuffer) continue;
                    }
                    if (x < minX || x > maxX || y < minY || y > maxY) continue;
                    if (PointInPolygon(poly, x, y)) lights.Add((x, y));
                }
            }
            catch
            {
            }
            return lights;
        }

        public List<List<(double x, double y)>> BuildOrthogonalTessellation(List<(double x, double y)> polygon, List<(double x, double y)> lights, double gap, double roundingRadius, double rotationDegrees, double alignTolerance)
        {
            var result = new List<List<(double x, double y)>>();
            try
            {
                if (polygon == null || polygon.Count < 4 || lights == null || lights.Count == 0) return result;
                double angleRad = (rotationDegrees % 360.0) * Math.PI / 180.0;
                bool doRotate = Math.Abs(angleRad) > 1e-6;
                double cosA = Math.Cos(angleRad);
                double sinA = Math.Sin(angleRad);

                (double x, double y) ToLocal((double x, double y) p)
                    => !doRotate ? p : (p.x * cosA + p.y * sinA, -p.x * sinA + p.y * cosA);
                (double x, double y) ToGlobal((double x, double y) p)
                    => !doRotate ? p : (p.x * cosA - p.y * sinA, p.x * sinA + p.y * cosA);

                var polyLocal = polygon.Select(ToLocal).ToList();
                var lightsLocalOriginal = lights.Select(ToLocal).ToList();
                double alignTol = Math.Max(0.0, alignTolerance);

                List<double> ClusterAxis(List<double> coords)
                {
                    coords.Sort();
                    var centers = new List<double>();
                    double accum = 0;
                    int count = 0;
                    double last = 0;
                    bool first = true;
                    foreach (var v in coords)
                    {
                        if (first)
                        {
                            accum = v;
                            count = 1;
                            last = v;
                            first = false;
                            continue;
                        }
                        if (Math.Abs(v - last) <= alignTol)
                        {
                            accum += v;
                            count++;
                            last = v;
                        }
                        else
                        {
                            centers.Add(accum / count);
                            accum = v;
                            count = 1;
                            last = v;
                        }
                    }
                    if (count > 0) centers.Add(accum / count);
                    return centers;
                }

                var xs = lightsLocalOriginal.Select(p => p.x).ToList();
                var ys = lightsLocalOriginal.Select(p => p.y).ToList();
                var xCenters = ClusterAxis(xs);
                var yCenters = ClusterAxis(ys);

                double Nearest(List<double> centers, double v)
                {
                    double best = centers[0];
                    double bd = Math.Abs(v - best);
                    for (int i = 1; i < centers.Count; i++)
                    {
                        double d = Math.Abs(v - centers[i]);
                        if (d < bd)
                        {
                            bd = d;
                            best = centers[i];
                        }
                    }
                    return best;
                }

                var lightsLocal = lightsLocalOriginal.Select(p => (x: Nearest(xCenters, p.x), y: Nearest(yCenters, p.y))).ToList();
                double minX = polyLocal.Min(p => p.x);
                double maxX = polyLocal.Max(p => p.x);
                double minY = polyLocal.Min(p => p.y);
                double maxY = polyLocal.Max(p => p.y);
                double minDim = 50.0;
                double g = Math.Max(0, gap);

                var queue = new Queue<(double x1, double y1, double x2, double y2, List<(double x, double y)> pts, int depth)>();
                queue.Enqueue((minX, minY, maxX, maxY, lightsLocal, 0));

                while (queue.Count > 0)
                {
                    var (x1, y1, x2, y2, pts, depth) = queue.Dequeue();
                    if (pts.Count <= 1 || ((x2 - x1) < minDim * 1.2 && (y2 - y1) < minDim * 1.2))
                    {
                        var rect = new List<(double x, double y)>{ (x1, y1), (x2, y1), (x2, y2), (x1, y2), (x1, y1) };
                        var clipped = IntersectPolygonWithPolygon(rect, polyLocal);
                        foreach (var c in clipped)
                        {
                            var back = c.Select(ToGlobal).ToList();
                            if (back.Count > 0 && (back[0] != back[^1])) back.Add(back[0]);
                            result.Add(back);
                        }
                        continue;
                    }

                    double spanX = x2 - x1;
                    double spanY = y2 - y1;
                    bool horizontalSplit = spanY >= spanX;
                    if (horizontalSplit)
                    {
                        double minLy = pts.Min(p => p.y);
                        double maxLy = pts.Max(p => p.y);
                        if (Math.Abs(maxLy - minLy) < 1e-6) horizontalSplit = false;
                    }
                    else
                    {
                        double minLx = pts.Min(p => p.x);
                        double maxLx = pts.Max(p => p.x);
                        if (Math.Abs(maxLx - minLx) < 1e-6) horizontalSplit = true;
                    }

                    if (horizontalSplit)
                    {
                        var sorted = pts.OrderBy(p => p.y).ToList();
                        double bestGap = -1;
                        double cutY = 0;
                        int gapIndex = -1;
                        for (int i = 1; i < sorted.Count; i++)
                        {
                            double gsz = sorted[i].y - sorted[i - 1].y;
                            if (gsz > bestGap)
                            {
                                bestGap = gsz;
                                gapIndex = i;
                            }
                        }
                        if (gapIndex > 0)
                            cutY = 0.5 * (sorted[gapIndex - 1].y + sorted[gapIndex].y);
                        else
                            cutY = 0.5 * (sorted[0].y + sorted[^1].y);
                        double halfGap = g / 2.0;
                        double upperY2 = cutY - halfGap;
                        double lowerY1 = cutY + halfGap;
                        if (upperY2 <= y1) upperY2 = cutY;
                        if (lowerY1 >= y2) lowerY1 = cutY;
                        var topPts = pts.Where(p => p.y <= cutY).ToList();
                        var botPts = pts.Where(p => p.y > cutY).ToList();
                        if (topPts.Count == 0 || botPts.Count == 0)
                        {
                            var rect = new List<(double x, double y)>{ (x1, y1), (x2, y1), (x2, y2), (x1, y2), (x1, y1) };
                            var clipped = IntersectPolygonWithPolygon(rect, polyLocal);
                            foreach (var c in clipped)
                            {
                                var back = c.Select(ToGlobal).ToList();
                                if (back.Count > 0 && (back[0] != back[^1])) back.Add(back[0]);
                                result.Add(back);
                            }
                            continue;
                        }
                        queue.Enqueue((x1, y1, x2, upperY2, topPts, depth + 1));
                        queue.Enqueue((x1, lowerY1, x2, y2, botPts, depth + 1));
                    }
                    else
                    {
                        var sorted = pts.OrderBy(p => p.x).ToList();
                        double bestGap = -1;
                        double cutX = 0;
                        int gapIndex = -1;
                        for (int i = 1; i < sorted.Count; i++)
                        {
                            double gsz = sorted[i].x - sorted[i - 1].x;
                            if (gsz > bestGap)
                            {
                                bestGap = gsz;
                                gapIndex = i;
                            }
                        }
                        if (gapIndex > 0)
                            cutX = 0.5 * (sorted[gapIndex - 1].x + sorted[gapIndex].x);
                        else
                            cutX = 0.5 * (sorted[0].x + sorted[^1].x);
                        double halfGap = g / 2.0;
                        double leftX2 = cutX - halfGap;
                        double rightX1 = cutX + halfGap;
                        if (leftX2 <= x1) leftX2 = cutX;
                        if (rightX1 >= x2) rightX1 = cutX;
                        var leftPts = pts.Where(p => p.x <= cutX).ToList();
                        var rightPts = pts.Where(p => p.x > cutX).ToList();
                        if (leftPts.Count == 0 || rightPts.Count == 0)
                        {
                            var rect = new List<(double x, double y)>{ (x1, y1), (x2, y1), (x2, y2), (x1, y2), (x1, y1) };
                            var clipped = IntersectPolygonWithPolygon(rect, polyLocal);
                            foreach (var c in clipped)
                            {
                                var back = c.Select(ToGlobal).ToList();
                                if (back.Count > 0 && (back[0] != back[^1])) back.Add(back[0]);
                                result.Add(back);
                            }
                            continue;
                        }
                        queue.Enqueue((x1, y1, leftX2, y2, leftPts, depth + 1));
                        queue.Enqueue((rightX1, y1, x2, y2, rightPts, depth + 1));
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        #endregion

        #region Caching helpers

        private bool PointInPolygon(List<(double x, double y)> poly, double x, double y)
        {
            bool inside = false;
            for (int i = 0, j = poly.Count - 1; i < poly.Count; i++)
            {
                var pi = poly[i];
                var pj = poly[j];
                bool intersect = (pi.y > y) != (pj.y > y) && (x < (pj.x - pi.x) * (y - pi.y) / (pj.y - pi.y + 1e-12) + pi.x);
                if (intersect) inside = !inside;
                j = i;
            }
            return inside;
        }

        private void InvalidateCaches()
        {
            try
            {
                _exported3DIndex = null;
                _fixtureCenterCache.Clear();
                _roomZBoundsCache.Clear();
            }
            catch
            {
            }
        }

        private void EnsureExportIndex(bool forceRebuild = false)
        {
            Guid? currentModelId = null;
            try { currentModelId = _app?.Project?.Model?.Id; } catch { currentModelId = null; }
            bool modelChanged = currentModelId != _cachedModelId;
            if (!forceRebuild && !modelChanged && _exported3DIndex != null && _exported3DIndex.Count > 0) return;
            if (modelChanged)
            {
                _fixtureCenterCache.Clear();
                _roomZBoundsCache.Clear();
            }
            _exported3DIndex = new Dictionary<int, IExportedObject3D>();
            try
            {
                var exporter = _app?.Project?.DataExporter;
                if (exporter == null) return;
                var exported = exporter.GetObjects3D();
                if (exported == null) return;
                for (int i = 0; i < exported.Count; i++)
                {
                    var eo = exported.Get(i);
                    _exported3DIndex[eo.ModelObjectId] = eo;
                }
                _cachedModelId = currentModelId;
            }
            catch
            {
                _exported3DIndex.Clear();
            }
        }

        private IExportedObject3D TryGetExportedObject3D(int modelObjectId)
        {
            try
            {
                EnsureExportIndex();
                if (_exported3DIndex != null && _exported3DIndex.TryGetValue(modelObjectId, out var eo)) return eo;
                return null;
            }
            catch
            {
                return null;
            }
        }

        private bool TryGetFixtureCenter(int fixtureObjectId, out double x, out double y, out double z)
        {
            if (_fixtureCenterCache.TryGetValue(fixtureObjectId, out var p))
            {
                x = p.x;
                y = p.y;
                z = p.z;
                return true;
            }
            x = y = z = 0;
            try
            {
                var eo = TryGetExportedObject3D(fixtureObjectId);
                if (eo == null || eo.MeshCount == 0) return false;
                double sx = 0;
                double sy = 0;
                double sz = 0;
                int count = 0;
                for (int mi = 0; mi < eo.MeshCount; mi++)
                {
                    var mesh = eo.GetMesh(mi);
                    for (int gi = 0; gi < mesh.GridCount; gi++)
                    {
                        var grid = mesh.GetGrid(gi);
                        int vc = Math.Min(grid.VertexCount, 64);
                        for (int vi = 0; vi < vc; vi++)
                        {
                            var v = grid.GetVertex(vi);
                            sx += v.X;
                            sy += v.Y;
                            sz += v.Z;
                            count++;
                        }
                        if (count >= 64) break;
                    }
                    if (count >= 64) break;
                }
                if (count == 0) return false;
                x = sx / count;
                y = sy / count;
                z = sz / count;
                _fixtureCenterCache[fixtureObjectId] = (x, y, z);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private (double minZ, double maxZ)? TryGetObjectZBounds(int modelObjectId)
        {
            try
            {
                if (_roomZBoundsCache.TryGetValue(modelObjectId, out var cached)) return cached;
                var target = TryGetExportedObject3D(modelObjectId);
                if (target == null) return null;
                bool hasAny = false;
                double minZ = double.MaxValue;
                double maxZ = double.MinValue;
                for (int mi = 0; mi < target.MeshCount; mi++)
                {
                    var mesh = target.GetMesh(mi);
                    for (int gi = 0; gi < mesh.GridCount; gi++)
                    {
                        var grid = mesh.GetGrid(gi);
                        for (int vi = 0; vi < grid.VertexCount; vi++)
                        {
                            var v = grid.GetVertex(vi);
                            if (v.Z < minZ) minZ = v.Z;
                            if (v.Z > maxZ) maxZ = v.Z;
                            hasAny = true;
                        }
                    }
                }
                if (!hasAny) return null;
                var bounds = (minZ, maxZ);
                _roomZBoundsCache[modelObjectId] = bounds;
                return bounds;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
