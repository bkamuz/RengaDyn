using Aw.Common;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aw.Plugin.Control
{
    public partial class ControlWeb : WebForm
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static ControlWeb _instance; public static ControlWeb Instance => _instance;

    private RoomPerimeterManager _manager;
    

    public static ControlWeb GetInstance(RoomPerimeterManager manager = null)
        {
            // Only create once; reuse the existing singleton window
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new ControlWeb(manager);
            }
            else
            {
                if (_instance.WindowState == FormWindowState.Minimized)
                    _instance.WindowState = FormWindowState.Normal;
                _instance.Activate();
            }
            return _instance;
        }

    private ControlWeb(RoomPerimeterManager manager)
        {
            _manager = manager;
            StartURL = @"file:///" + Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)).Replace(@"\\", @"/") + "/UI/html/control.html";

            InitializeComponent();
            var cfg = Aw.Common.Configuration<dynamic>.Instance; // lightweight
            cfg.LoadConfig("\\Control.json", true);
            LoadWindowProperty(cfg);
            // WebView2 is created asynchronously in InitializeWebView2; event hookup and Source assignment are done there to avoid race conditions
        }

        protected async override Task InitializeWebView2()
        {
            await base.InitializeWebView2();
            // Hook message handler and navigate only after WebView2 is fully initialized
            this.webView2.WebMessageReceived += WebView2_WebMessageReceived;
            this.webView2.Source = new Uri(StartURL);
            // Send initial materials; let the UI request Preview once it’s fully ready
            try { SendMaterials(); } catch (Exception ex) { _logger.Error(ex, "SendMaterials on init failed"); }
            // UI will send PageLoaded and trigger Preview itself when ready
        }

        private void SendMaterials()
        {
            try
            {
                var dict = _manager.GetLayeredMaterials();
                var list = dict.Select(kv => new { id = kv.Key, name = kv.Value }).ToList();
                SendMessage("Materials", list);
            }
            catch (Exception ex) { _logger.Error(ex, "SendMaterials failed"); }
        }

    private void SendPreview(Newtonsoft.Json.Linq.JToken param = null)
        {
            try
            {
                var perims = _manager.ExtractSelectedRoomPerimeters();
                
                if (perims.Count == 0) { SendMessageToast("Генератор контролов", "warning", new List<string>{"Выберите помещение"}, true); SendMessage("PreviewData", null); return; }
                var r = perims[0];
                var b = _manager.CalcBounds(r.PerimeterCurve);
                
                if (b == null) { SendMessageToast("Генератор контролов", "warning", new List<string>{"Не удалось определить границы помещения"}, true); SendMessage("PreviewData", null); return; }
                var room = new { name = r.ObjectName, width = b.Width, height = b.Height, minX = b.MinX, maxX = b.MaxX, minY = b.MinY, maxY = b.MaxY };
                var roomPolyRaw = _manager.GetRoomPolygon(r);
                if (roomPolyRaw == null || roomPolyRaw.Count < 3) { SendMessageToast("Генератор контролов", "warning", new List<string>{"Периметр помещения не найден"}, true); SendMessage("PreviewData", new { room = room, polygon = (object)null, inset = (object)null, cells = new List<List<object>>(), lights = new List<object>() }); return; }
                var roomPoly = roomPolyRaw.Select(p => new { x = p.x, y = p.y }).ToList();
                // Send minimal payload first (polygon only) so UI always has something to draw
                bool lightFloodFlag = false;
                double roundingRadius = 0.0;
                if (param is JObject p0 && p0["lightFlood"] != null) lightFloodFlag = p0["lightFlood"].Value<bool>();
                if (param is JObject pRound && pRound["round"] != null) roundingRadius = Math.Max(0.0, pRound["round"].Value<double>());
                var payloadMin = new { room = room, polygon = roomPoly, inset = (object)null, cells = new List<object>(), lights = new List<object>(), columns = new List<List<object>>(), lightFlood = lightFloodFlag, round = roundingRadius };
                SendMessage("PreviewData", payloadMin);
                // Try to enrich with inset, cells and lights, but don't break minimal preview on failures
                List<object> insetPoly = null; List<object> lights = null; List<object> cells = new(); List<List<object>> columnShapes = null;
                // 1) Compute inset (offset). If it fails, fall back to room polygon and continue
                List<(double x,double y)> insetPolyRaw = null;
                double wallGap = 0.0;
                if (param is JObject jo && jo["wallGap"] != null)
                    wallGap = Math.Max(0.0, jo["wallGap"].Value<double>());
                try
                {
                    insetPolyRaw = _manager.OffsetPolygonInward(roomPolyRaw, wallGap);
                    if (insetPolyRaw == null || insetPolyRaw.Count < 3)
                    {
                        insetPolyRaw = roomPolyRaw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Inset computation failed");
                    insetPolyRaw = roomPolyRaw;
                    
                }
                insetPoly = insetPolyRaw?.Select(p => (object)new { x = p.x, y = p.y }).ToList();

                // 2) Compute grid cells for preview (always proceed); refresh export index if needed so lights/geometry are consistent
                try
                {
                    // Keep RoomPerimeterManager caches current for this preview round
                    try { _manager?.GetType()?.GetMethod("EnsureExportIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(_manager, new object[]{ false }); } catch {}
                    var previewCells = ComputePreviewCells(param, r, insetPolyRaw ?? roomPolyRaw);
                    if (previewCells != null)
                    {
                        if (previewCells.Cells != null && previewCells.Cells.Count > 0)
                        {
                            foreach (var cell in previewCells.Cells)
                            {
                                var outline = cell.Outline?.Select(pt => (object)new { x = pt.x, y = pt.y }).ToList();
                                var holes = cell.Holes?.Select(h => (object)h.Select(pt => (object)new { x = pt.x, y = pt.y }).ToList()).ToList();
                                cells.Add(new { outline, holes });
                            }
                        }
                        if (previewCells.Columns != null && previewCells.Columns.Count > 0)
                        {
                            columnShapes = previewCells.Columns
                                .Select(poly => poly.Select(pt => (object)new { x = pt.x, y = pt.y }).ToList())
                                .ToList();
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "ComputePreviewCells failed");
                    try { SendMessageToast("Генератор контролов", "warning", new List<string>{ "Ошибка расчёта ячеек" }, true); } catch { }
                }
                try
                {
                    var lightsRaw = _manager.DetectLightingFixturesInsideRoom(r);
                    if (lightFloodFlag)
                        lights = lightsRaw.Select(p => (object)new { x = p.x, y = p.y }).ToList();
                }
                catch (Exception ex) { _logger.Warn(ex, "Lights detection failed"); }
                var payload = new { room = room, polygon = roomPoly, inset = insetPoly, cells = cells, lights = lights, columns = columnShapes ?? new List<List<object>>(), lightFlood = lightFloodFlag, round = roundingRadius };
                SendMessage("PreviewData", payload);
                
            }
            catch (Exception ex) { _logger.Error(ex, "SendPreview failed"); }
        }

        protected new void WebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                base.WebView2_WebMessageReceived(sender, e);
                var json = JObject.Parse(e.WebMessageAsJson);
                string key = json["Name"]?.ToString();
                var data = json["Data"];

                switch (key)
                {
                    case "PageLoaded":
        try { SendMaterials(); } catch (Exception ex) { _logger.Error(ex, "SendMaterials on PageLoaded failed"); SendMessageToast("Генератор контролов", "warning", new List<string>{"Не удалось загрузить материалы"}, true); }
                        break;
                    case "Preview":
                        try { SendPreview(data); } catch (Exception ex) { _logger.Error(ex, "SendPreview failed"); SendMessageToast("Генератор контролов", "warning", new List<string>{"Ошибка предпросмотра"}, true); }
                        break;
                    case "CalcGridFromLights":
                        // compute rows/cols proposal and update preview
                        var ok = false; try { ok = TryCalcGridFromLights(data); } catch (Exception ex) { _logger.Error(ex, "CalcGridFromLights failed"); }
                        if (!ok) SendMessageToast("Генератор контролов", "warning", new List<string>{"Светильники не найдены"}, true);
                        // Do not call SendPreview here; UI will refresh once after UpdateParams
                        break;
                    case "Create":
                        try
                        {
                            var created = CreateFromParams(data);
                            SendMessageToast("Генератор контролов", created>0?"gray-100":"warning", new List<string>{ created>0? $"Создано: {created}" : "Ничего не создано" }, false);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Create failed");
                            var msg = ex.Message ?? "Ошибка создания полов";
                            SendMessageToast("Генератор контролов", "warning", new List<string>{ msg }, true);
                        }
                        break;
                    case "Undo":
                        try
                        {
                            var undone = _manager.UndoTwoSteps();
                            SendMessageToast("Генератор контролов", undone?"gray-100":"warning", new List<string>{ undone? "Отменено" : "Не удалось выполнить отмену" }, true);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Undo failed");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ControlWeb message error");
            }
            finally
            {
                // Always hide spinner
                try { SendMessageJobDone(); } catch { }
            }
        }

        private bool TryCalcGridFromLights(Newtonsoft.Json.Linq.JToken data)
        {
            try
            {
                var perims = _manager.ExtractSelectedRoomPerimeters();
                if (perims == null || perims.Count == 0) return false;
                var r = perims[0];
                var lights = _manager.DetectLightingFixturesInsideRoom(r);
                if (lights == null || lights.Count == 0) return false;

                // Rotation from UI (degrees)
                double rotDeg = 0.0;
                if (data is JObject jo && jo["rot"] != null) rotDeg = jo["rot"].Value<double>();
                double angleRad = (rotDeg % 360.0) * Math.PI / 180.0;
                bool doRotate = Math.Abs(angleRad) > 1e-6;
                double cosA = Math.Cos(angleRad), sinA = Math.Sin(angleRad);
                (double x,double y) ToLocal((double x,double y) p)
                {
                    if (!doRotate) return p;
                    return ( p.x * cosA + p.y * sinA, -p.x * sinA + p.y * cosA );
                }
                var local = lights.Select(ToLocal).ToList();

                // Cluster along each axis with tolerance (mm)
                const double tol = 500.0;
                List<double> ClusterAxis(List<double> coords)
                {
                    var res = new List<double>();
                    if (coords == null || coords.Count == 0) return res;
                    coords.Sort();
                    double accum = coords[0]; int count = 1; double last = coords[0];
                    for (int i=1;i<coords.Count;i++)
                    {
                        double v = coords[i];
                        if (Math.Abs(v - last) <= tol) { accum += v; count++; last = v; }
                        else { res.Add(accum / count); accum = v; count = 1; last = v; }
                    }
                    res.Add(accum / count);
                    return res;
                }

                var xCenters = ClusterAxis(local.Select(p=>p.x).ToList());
                var yCenters = ClusterAxis(local.Select(p=>p.y).ToList());
                int cols = Math.Max(1, xCenters.Count);
                int rows = Math.Max(1, yCenters.Count);

                // Send back to UI so inputs reflect suggestion
                SendMessage("UpdateParams", new { rows = rows, cols = cols });

                // Also update preview using these values
                if (data is JObject j2)
                {
                    j2["rows"] = rows; j2["cols"] = cols;
                }
                return true;
            }
            catch { return false; }
        }

        private int CreateFromParams(Newtonsoft.Json.Linq.JToken data)
        {
            try
            {
                var p = data.ToObject<ParamsDto>();
                var perims = _manager.ExtractSelectedRoomPerimeters();
                
                if (perims.Count == 0) return 0;
                var grid = new FloorGridParameters
                {
                    Rows = Math.Max(1, p.rows),
                    Columns = Math.Max(1, p.cols),
                    // Manager expects millimeters for polygon-related values
                    Gap = Math.Max(0.0, p.gap),
                    AlignTolerance = Math.Max(0.0, p.alignTol),
                    WallGap = Math.Max(0.0, p.wallGap),
                    VerticalOffset = p.vOff,
                    MaterialId = p.materialId,
                    Thickness = 0,
                    RoundingRadius = p.round,
                    RotationDegrees = p.rot,
                    MaskEmptyCells = p.maskHoles,
                    LightFloodMode = p.lightFlood
                };
                _manager.ResolveMaterialParameters(ref grid);
                int count = _manager.CreateFloorsFromPerimeter(perims[0], grid);
                return count;
            }
            catch (Exception ex) { _logger.Error(ex, "CreateFromParams failed"); return 0; }
        }

        private class PreviewCellsResult
        {
            public List<CellPreview> Cells { get; } = new();
            public List<List<(double x, double y)>> Columns { get; } = new();
        }

        private class CellPreview
        {
            public List<(double x, double y)> Outline { get; init; }
            public List<List<(double x, double y)>> Holes { get; init; } = new();
        }

        // Build grid cells for preview using manager helpers; returns polygons in global coordinates (mm)
        private PreviewCellsResult ComputePreviewCells(JToken param, RoomPerimeterData r, List<(double x,double y)> insetPoly)
        {
            try
            {
                var p = param?.ToObject<ParamsDto>() ?? new ParamsDto();
                int rows = Math.Max(1, p.rows);
                int cols = Math.Max(1, p.cols);
                double gap = Math.Max(0.0, p.gap);
                double alignTol = Math.Max(0.0, p.alignTol);
                double wallGap = Math.Max(0.0, p.wallGap);
                double round = Math.Max(0.0, p.round);
                double rotDeg = p.rot;
                bool lightFlood = p.lightFlood;
                bool maskHoles = p.maskHoles;

                var roomPoly = _manager.GetRoomPolygon(r);
                if (roomPoly == null || roomPoly.Count < 4) return new PreviewCellsResult();

                // Ensure inset polygon is valid; if no wall gap, use room polygon as inset
                if (insetPoly == null || insetPoly.Count < 4)
                {
                    insetPoly = wallGap <= 0.0
                        ? roomPoly
                        : _manager.OffsetPolygonInward(roomPoly, wallGap);
                }
                if (insetPoly == null || insetPoly.Count < 4) insetPoly = roomPoly;

                var previewGrid = new FloorGridParameters
                {
                    Rows = rows,
                    Columns = cols,
                    Gap = gap,
                    AlignTolerance = alignTol,
                    WallGap = wallGap,
                    RoundingRadius = round,
                    RotationDegrees = rotDeg,
                    MaskEmptyCells = maskHoles,
                    LightFloodMode = lightFlood,
                    VerticalOffset = p.vOff,
                    Thickness = 0,
                    MaterialId = 0
                };

                var result = new PreviewCellsResult();
                try
                {
                    var columnPolys = _manager.GetColumnFootprints(r, wallGap);
                    if (columnPolys != null && columnPolys.Count > 0)
                        result.Columns.AddRange(columnPolys);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Preview columns fetch failed");
                }

                if (lightFlood)
                {
                    var lights = _manager.DetectLightingFixturesInsideRoom(r);
                    var cells = _manager.BuildOrthogonalTessellation(insetPoly, lights, gap, round, rotDeg, alignTol);
                    foreach (var cell in cells)
                    {
                        if (cell == null || cell.Count < 3) continue;
                        var tiles = _manager.BuildColumnAwareTilesForPreview(cell, r, previewGrid);
                        if (tiles == null || tiles.Count == 0)
                        {
                            result.Cells.Add(new CellPreview { Outline = cell });
                            continue;
                        }
                        foreach (var tile in tiles)
                        {
                            result.Cells.Add(new CellPreview
                            {
                                Outline = tile.outline,
                                Holes = tile.openings
                            });
                        }
                    }
                    return result;
                }

                // Regular grid cell build mirroring CreateFloorsFromPerimeter (without creating objects)
                var insetBounds = new PerimeterBounds
                {
                    MinX = insetPoly.Min(pt => pt.x),
                    MaxX = insetPoly.Max(pt => pt.x),
                    MinY = insetPoly.Min(pt => pt.y),
                    MaxY = insetPoly.Max(pt => pt.y)
                };
                insetBounds.Width = insetBounds.MaxX - insetBounds.MinX;
                insetBounds.Height = insetBounds.MaxY - insetBounds.MinY;

                double cxGlobal = (insetBounds.MinX + insetBounds.MaxX) / 2.0;
                double cyGlobal = (insetBounds.MinY + insetBounds.MaxY) / 2.0;
                double angleRad = (rotDeg % 360.0) * Math.PI / 180.0;
                bool doRotate = Math.Abs(angleRad) > 1e-6;
                double cosA = Math.Cos(angleRad), sinA = Math.Sin(angleRad);
                if (!doRotate && !_manager.IsPolygonOrthogonal(roomPoly))
                {
                    for (int i = 0; i < roomPoly.Count - 1; i++)
                    {
                        var a = roomPoly[i];
                        var b = roomPoly[i + 1];
                        double dx = b.x - a.x, dy = b.y - a.y;
                        double len = Math.Sqrt(dx * dx + dy * dy);
                        if (len > 1e-6)
                        {
                            angleRad = Math.Atan2(dy, dx);
                            cosA = Math.Cos(angleRad);
                            sinA = Math.Sin(angleRad);
                            doRotate = Math.Abs(angleRad) > 1e-6;
                            break;
                        }
                    }
                }

                double floorW, floorH;
                double localMinX, localMinY;
                if (!doRotate)
                {
                    double availW = insetBounds.Width;
                    double availH = insetBounds.Height;
                    floorW = (availW - (cols - 1) * gap) / cols;
                    floorH = (availH - (rows - 1) * gap) / rows;
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
                    floorW = (availW - (cols - 1) * gap) / cols;
                    floorH = (availH - (rows - 1) * gap) / rows;
                    localMinX = minLX;
                    localMinY = minLY;
                }

                if (floorW <= 0 || floorH <= 0) return result;

                var lightsGlobal = maskHoles ? _manager.DetectLightingFixturesInsideRoom(r) : null;
                List<(double x, double y)> lightsLocal = null;
                if (maskHoles && doRotate && lightsGlobal != null && lightsGlobal.Count > 0)
                {
                    lightsLocal = new List<(double x, double y)>(lightsGlobal.Count);
                    foreach (var pL in lightsGlobal)
                    {
                        double dx = pL.x - cxGlobal;
                        double dy = pL.y - cyGlobal;
                        double lx = dx * cosA + dy * sinA;
                        double ly = -dx * sinA + dy * cosA;
                        lightsLocal.Add((lx, ly));
                    }
                }

                for (int ri = 0; ri < rows; ri++)
                {
                    for (int ci = 0; ci < cols; ci++)
                    {
                        double x = insetBounds.MinX + ci * (floorW + gap);
                        double y = insetBounds.MinY + ri * (floorH + gap);
                        List<List<(double x, double y)>> clippedParts;
                        if (!doRotate)
                        {
                            if (maskHoles && lightsGlobal != null && lightsGlobal.Count > 0)
                            {
                                double x2 = x + floorW, y2 = y + floorH;
                                bool any = false;
                                foreach (var L in lightsGlobal)
                                {
                                    if (L.x >= x && L.x <= x2 && L.y >= y && L.y <= y2)
                                    {
                                        any = true;
                                        break;
                                    }
                                }
                                if (!any) continue; // skip empty cells (without lights)
                            }
                            clippedParts = _manager.IntersectRectWithPolygon(x, y, floorW, floorH, insetPoly);
                        }
                        else
                        {
                            double baseLX = localMinX + ci * (floorW + gap);
                            double baseLY = localMinY + ri * (floorH + gap);
                            if (maskHoles && lightsLocal != null && lightsLocal.Count > 0)
                            {
                                double lx2 = baseLX + floorW, ly2 = baseLY + floorH;
                                bool any = false;
                                foreach (var L in lightsLocal)
                                {
                                    if (L.x >= baseLX && L.x <= lx2 && L.y >= baseLY && L.y <= ly2)
                                    {
                                        any = true;
                                        break;
                                    }
                                }
                                if (!any) continue; // skip empty cells (without lights)
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
                            clippedParts = _manager.IntersectPolygonWithPolygon(rectGlobal, insetPoly);
                        }

                        if (clippedParts == null || clippedParts.Count == 0) continue;
                        foreach (var part in clippedParts)
                        {
                            if (part.Count < 3) continue;
                            var tiles = _manager.BuildColumnAwareTilesForPreview(part, r, previewGrid);
                            if (tiles == null || tiles.Count == 0)
                            {
                                result.Cells.Add(new CellPreview { Outline = part });
                                continue;
                            }
                            foreach (var tile in tiles)
                            {
                                result.Cells.Add(new CellPreview
                                {
                                    Outline = tile.outline,
                                    Holes = tile.openings
                                });
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ComputePreviewCells failed");
                return new PreviewCellsResult();
            }
        }

    // Removed automatic preview on activation; UI drives preview explicitly to avoid duplicate updates

    private class ParamsDto { public int rows; public int cols; public double wallGap; public double gap; public double alignTol; public double vOff; public double round; public double rot; public int materialId; public bool lightFlood; public bool maskHoles; }
    }
}
