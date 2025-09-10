using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynGeometry;

namespace DynRenga.DynObjects
{
    /// <summary>
    /// Класс для работы с интерфейсом Renga.IBaseline2DObject
    /// Представляет объект с 2D базовой линией
    /// </summary>
    public class Baseline2DObject
    {
        /// <summary>
        /// Внутренний COM-объект Renga.IBaseline2DObject
        /// </summary>
        public Renga.IBaseline2DObject _i;

        /// <summary>
        /// Инициализация класса через интерфейс Renga.IBaseline2DObject
        /// </summary>
        /// <param name="Baseline2DObject_object"></param>
        internal Baseline2DObject(object Baseline2DObject_object)
        {
            this._i = Baseline2DObject_object as Renga.IBaseline2DObject;
        }

        /// <summary>
        /// Получение Baseline2DObject из ModelObject
        /// Возвращает null если объект не поддерживает интерфейс IBaseline2DObject
        /// </summary>
        /// <param name="modelObject">Объект модели</param>
        /// <returns>Baseline2DObject или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Baseline2DObject ByModelObject(ModelObject modelObject)
        {
            if (modelObject == null) return null;
            return modelObject.GetBaseline2DObject();
        }

        /// <summary>
        /// Получение копии 2D базовой линии объекта в его собственной системе координат
        /// </summary>
        /// <returns>Кривая 2D, представляющая базовую линию</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetBaseline()
        {
            return new Curve2D(this._i.GetBaseline());
        }

        /// <summary>
        /// Получение копии 2D базовой линии объекта в указанной системе координат
        /// </summary>
        /// <param name="placement2D">Система координат, в которую будет преобразована 2D базовая линия</param>
        /// <returns>Кривая 2D, представляющая базовую линию в указанной системе координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetBaselineInCS(DynGeometry.Placement2D placement2D)
        {
            return new Curve2D(this._i.GetBaselineInCS(placement2D.ToRengaPlacement2D()));
        }

        /// <summary>
        /// Создание Baseline2D из Curve2D с использованием RengaBIM SDK
        /// Валидирует кривую и создает объект, который можно использовать для установки базовых линий
        /// </summary>
        /// <param name="curve2D">Кривая 2D для создания базовой линии</param>
        /// <returns>Результат валидации и информация для отладки</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "IsValid", "Curve2D", "DebugInfo" })]
        public static Dictionary<string, object> ValidateCurveForBaseline(Curve2D curve2D)
        {
            var debugInfo = "🔧 Validating Curve2D for baseline creation using RengaBIM SDK...\n";
            
            try
            {
                // Validate input curve
                if (curve2D == null)
                {
                    debugInfo += "❌ Curve2D cannot be null\n";
                    debugInfo += "💡 Provide a valid Curve2D object (e.g., from Curve2D.ByLineSegment)\n";
                    return new Dictionary<string, object>
                    {
                        { "IsValid", false },
                        { "Curve2D", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (curve2D._i == null)
                {
                    debugInfo += "❌ Curve2D interface is not initialized\n";
                    debugInfo += "💡 Make sure to use Curve2D.ByLineSegment() or similar method\n";
                    debugInfo += "💡 Check that the curve creation was successful\n";
                    return new Dictionary<string, object>
                    {
                        { "IsValid", false },
                        { "Curve2D", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                debugInfo += "✅ Curve2D interface validated\n";
                
                // Validate curve properties
                try
                {
                    double length = curve2D._i.GetLength();
                    debugInfo += $"📏 Curve length: {length:F2}mm ({length/1000.0:F3}m)\n";
                    
                    if (length <= 0)
                    {
                        debugInfo += "❌ Curve has invalid length (≤ 0)\n";
                        debugInfo += "💡 Curve must have positive length\n";
                        return new Dictionary<string, object>
                        {
                            { "IsValid", false },
                            { "Curve2D", curve2D },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    if (length < 1.0)
                    {
                        debugInfo += "⚠️ Curve is very small (< 1mm)\n";
                        debugInfo += "💡 Consider using larger coordinates (multiply by 1000 for mm)\n";
                    }
                    
                    if (length > 1000000.0)
                    {
                        debugInfo += "⚠️ Curve is very large (> 1000m)\n";
                        debugInfo += "💡 Consider using smaller coordinates (divide by 1000 for m)\n";
                    }
                    
                    debugInfo += $"📐 Curve type: {curve2D.GetType().Name}\n";
                    debugInfo += $"📐 Curve2D type: {curve2D.Curve2DType}\n";
                    debugInfo += $"📐 Is closed: {curve2D.IsClosed}\n";
                    
                    // Additional validation for baseline compatibility
                    var curveType = curve2D.Curve2DType;
                    if (curveType != null)
                    {
                        debugInfo += $"🔍 Checking curve type compatibility...\n";
                        
                        switch (curveType.ToString())
                        {
                            case "Curve2DType_LineSegment":
                                debugInfo += "✅ Line segment is fully supported for baselines\n";
                                break;
                            case "Curve2DType_Arc":
                                debugInfo += "✅ Arc is supported for baselines\n";
                                break;
                            case "Curve2DType_PolyCurve":
                                debugInfo += "✅ PolyCurve is supported for baselines\n";
                                break;
                            case "Curve2DType_Undefined":
                                debugInfo += "⚠️ Undefined curve type - may cause issues\n";
                                break;
                            default:
                                debugInfo += $"⚠️ Unknown curve type: {curveType}\n";
                                break;
                        }
                    }
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"❌ Failed to validate curve: {comEx.Message}\n";
                    debugInfo += $"❌ COM Error Code: 0x{comEx.ErrorCode:X8}\n";
                    debugInfo += "💡 The curve may be corrupted or invalid\n";
                    return new Dictionary<string, object>
                    {
                        { "IsValid", false },
                        { "Curve2D", curve2D },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Curve2D is valid for baseline operations!\n";
                debugInfo += "💡 This curve can be used with SetBaseline() method on target objects\n";
                debugInfo += "💡 Use this curve with objects that support IBaseline2DObject interface\n";
                
                return new Dictionary<string, object>
                {
                    { "IsValid", true },
                    { "Curve2D", curve2D },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                debugInfo += $"❌ COM Error when validating curve!\n";
                debugInfo += $"❌ Error Code: 0x{comEx.ErrorCode:X8}\n";
                debugInfo += $"❌ Error Message: {comEx.Message}\n";
                
                // Provide specific guidance based on error code
                switch (comEx.ErrorCode)
                {
                    case unchecked((int)0x80010105): // RPC_E_SERVERFAULT
                        debugInfo += "💡 RPC_E_SERVERFAULT - Possible causes:\n";
                        debugInfo += "   • Renga application is not properly initialized\n";
                        debugInfo += "   • Curve geometry is invalid for baseline operations\n";
                        debugInfo += "   • Insufficient permissions for baseline operations\n";
                        break;
                        
                    case unchecked((int)0x80004005): // E_FAIL
                        debugInfo += "💡 E_FAIL - Possible causes:\n";
                        debugInfo += "   • Curve type is not supported for baseline operations\n";
                        debugInfo += "   • Curve geometry is invalid\n";
                        debugInfo += "   • Curve is not in correct coordinate system\n";
                        break;
                        
                    case unchecked((int)0x80070057): // E_INVALIDARG
                        debugInfo += "💡 E_INVALIDARG - Possible causes:\n";
                        debugInfo += "   • Curve has invalid geometry\n";
                        debugInfo += "   • Curve coordinates are out of valid range\n";
                        debugInfo += "   • Curve type is not supported\n";
                        break;
                        
                    case unchecked((int)0x80004001): // E_NOTIMPL
                        debugInfo += "💡 E_NOTIMPL - Curve validation not implemented for this curve type\n";
                        break;
                        
                    default:
                        debugInfo += "💡 Unknown COM error - check curve validity and Renga installation\n";
                        break;
                }
                
                debugInfo += $"\n🔍 Debug Information:\n";
                debugInfo += $"   Curve2D is null: {curve2D == null}\n";
                debugInfo += $"   Curve2D._i is null: {curve2D?._i == null}\n";
                
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Curve2D", curve2D },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when validating curve!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                debugInfo += $"\n🔍 Debug Information:\n";
                debugInfo += $"   Curve2D is null: {curve2D == null}\n";
                debugInfo += $"   Curve2D._i is null: {curve2D?._i == null}\n";
                
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Curve2D", curve2D },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Установка 2D базовых линий для массива объектов с использованием массива кривых
        /// Поддерживает пакетную установку базовых линий для множественных объектов
        /// </summary>
        /// <param name="baselineObjects">Массив объектов Baseline2DObject</param>
        /// <param name="curves">Массив кривых Curve2D</param>
        /// <returns>Результаты операций и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "Results", "DebugInfo" })]
        public static Dictionary<string, object> SetBaselinesWithDebug(Baseline2DObject[] baselineObjects, Curve2D[] curves)
        {
            var debugInfo = "🔧 Setting baselines for multiple objects...\n";
            var results = new List<Dictionary<string, object>>();
            bool overallSuccess = true;
            
            try
            {
                if (baselineObjects == null || baselineObjects.Length == 0)
                {
                    debugInfo += "❌ Baseline2DObject array cannot be null or empty\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "Results", new Dictionary<string, object>[0] },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (curves == null || curves.Length == 0)
                {
                    debugInfo += "❌ Curves array cannot be null or empty\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "Results", new Dictionary<string, object>[0] },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"📊 Processing {baselineObjects.Length} objects with {curves.Length} curves\n";
                
                // Check if arrays have compatible lengths
                int maxLength = Math.Max(baselineObjects.Length, curves.Length);
                debugInfo += $"📏 Using maximum length: {maxLength}\n";
                
                for (int i = 0; i < maxLength; i++)
                {
                    debugInfo += $"\n🔄 Processing object {i + 1}/{maxLength}...\n";
                    
                    var result = new Dictionary<string, object>
                    {
                        { "Index", i },
                        { "Success", false },
                        { "ObjectId", "N/A" },
                        { "CurveId", "N/A" },
                        { "Error", "Not processed" }
                    };
                    
                    try
                    {
                        // Get object and curve for this iteration
                        var obj = i < baselineObjects.Length ? baselineObjects[i] : baselineObjects[baselineObjects.Length - 1];
                        var curve = i < curves.Length ? curves[i] : curves[curves.Length - 1];
                        
                        if (obj == null)
                        {
                            debugInfo += $"❌ Object {i + 1} is null\n";
                            result["Error"] = "Object is null";
                            results.Add(result);
                            overallSuccess = false;
                            continue;
                        }
                        
                        if (curve == null)
                        {
                            debugInfo += $"❌ Curve {i + 1} is null\n";
                            result["Error"] = "Curve is null";
                            results.Add(result);
                            overallSuccess = false;
                            continue;
                        }
                        
                        debugInfo += $"✅ Object {i + 1} and Curve {i + 1} are valid\n";
                        
                        // Try to set baseline using the existing method
                        var setResult = obj.SetBaselineWithDebug(curve);
                        
                        if (setResult.ContainsKey("Success") && (bool)setResult["Success"])
                        {
                            debugInfo += $"✅ Object {i + 1} baseline set successfully\n";
                            result["Success"] = true;
                            result["ObjectId"] = obj._i != null ? "Valid" : "Invalid";
                            result["CurveId"] = "Set successfully";
                            result["Error"] = "None";
                        }
                        else
                        {
                            debugInfo += $"❌ Object {i + 1} baseline setting failed\n";
                            result["Success"] = false;
                            result["ObjectId"] = obj._i != null ? "Valid" : "Invalid";
                            result["CurveId"] = "Failed to set";
                            result["Error"] = setResult.ContainsKey("DebugInfo") ? setResult["DebugInfo"].ToString() : "Unknown error";
                            overallSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Error processing object {i + 1}: {ex.Message}\n";
                        result["Error"] = ex.Message;
                        overallSuccess = false;
                    }
                    
                    results.Add(result);
                }
                
                debugInfo += $"\n📊 Batch processing completed: {results.Count(r => (bool)r["Success"])}/{results.Count} successful\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", overallSuccess },
                    { "Results", results.ToArray() },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error in batch processing: {ex.Message}\n";
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "Results", results.ToArray() },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Установка 2D базовых линий для массива объектов с использованием массива кривых (альтернативная версия)
        /// Поддерживает различные комбинации входных данных
        /// </summary>
        /// <param name="baselineObjects">Массив объектов Baseline2DObject</param>
        /// <param name="curves">Массив кривых Curve2D</param>
        /// <returns>Результаты операций и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "Results", "DebugInfo" })]
        public static Dictionary<string, object> SetBaselinesWithDebugAlt(object baselineObjects, object curves)
        {
            var debugInfo = "🔧 Setting baselines for objects (alternative method)...\n";
            
            try
            {
                // Convert inputs to arrays
                Baseline2DObject[] objArray = null;
                Curve2D[] curveArray = null;
                
                // Handle baselineObjects
                if (baselineObjects is Baseline2DObject[] objs)
                {
                    objArray = objs;
                    debugInfo += $"✅ Baseline2DObject array: {objArray.Length} objects\n";
                }
                else if (baselineObjects is Baseline2DObject singleObj)
                {
                    objArray = new Baseline2DObject[] { singleObj };
                    debugInfo += "✅ Single Baseline2DObject converted to array\n";
                }
                else if (baselineObjects is object[] objObjects)
                {
                    objArray = objObjects.OfType<Baseline2DObject>().ToArray();
                    debugInfo += $"✅ Object array converted to Baseline2DObject array: {objArray.Length} objects\n";
                }
                else
                {
                    debugInfo += $"❌ Unsupported baselineObjects type: {baselineObjects?.GetType().Name ?? "null"}\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "Results", new Dictionary<string, object>[0] },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Handle curves
                if (curves is Curve2D[] curvesArray)
                {
                    curveArray = curvesArray;
                    debugInfo += $"✅ Curve2D array: {curveArray.Length} curves\n";
                }
                else if (curves is Curve2D singleCurve)
                {
                    curveArray = new Curve2D[] { singleCurve };
                    debugInfo += "✅ Single Curve2D converted to array\n";
                }
                else if (curves is object[] curveObjects)
                {
                    curveArray = curveObjects.OfType<Curve2D>().ToArray();
                    debugInfo += $"✅ Object array converted to Curve2D array: {curveArray.Length} curves\n";
                }
                    else if (curves is Renga.ICurve2D rengaCurve)
                {
                    // Handle native Renga ICurve2D - convert to Curve2D wrapper
                    debugInfo += "✅ Single Renga ICurve2D detected - converting to Curve2D wrapper\n";
                    curveArray = new Curve2D[] { new Curve2D(rengaCurve) };
                }
                else if (curves is Renga.ICurve2D[] rengaCurves)
                {
                    // Handle array of native Renga ICurve2D
                    debugInfo += $"✅ Renga ICurve2D array: {rengaCurves.Length} curves - converting to Curve2D wrappers\n";
                    curveArray = rengaCurves.Select(c => new Curve2D(c)).ToArray();
                }
                else
                {
                    debugInfo += $"❌ Unsupported curves type: {curves?.GetType().Name ?? "null"}\n";
                    debugInfo += "💡 Supported types: Curve2D, Curve2D[], Renga.ICurve2D, Renga.ICurve2D[], object[]\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "Results", new Dictionary<string, object>[0] },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Call the main method
                return SetBaselinesWithDebug(objArray, curveArray);
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error in alternative method: {ex.Message}\n";
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "Results", new Dictionary<string, object>[0] },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание DynRenga Curve2D из массива отдельных Dynamo кривых (Line, Arc)
        /// Простой и надежный способ создания составной кривой из отдельных сегментов
        /// </summary>
        /// <param name="dynamoCurves">Массив отдельных Dynamo кривых (Line, Arc)</param>
        /// <returns>Созданная DynRenga Curve2D и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Curve2D", "Success", "DebugInfo" })]
        public static Dictionary<string, object> CreateDynRengaCurve2DFromDynamoCurves(object[] dynamoCurves)
        {
            var debugInfo = "🔧 Creating DynRenga Curve2D from individual Dynamo curves...\n";
            
            try
            {
                if (dynamoCurves == null || dynamoCurves.Length == 0)
                {
                    debugInfo += "❌ Dynamo curves array cannot be null or empty\n";
                    return new Dictionary<string, object>
                    {
                        { "Curve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"📊 Processing {dynamoCurves.Length} individual Dynamo curves\n";
                
                // Get IMath interface for curve creation
                debugInfo += "🔧 Getting IMath interface...\n";
                Renga.IMath mathInterface = null;
                
                try
                {
                    // Try to create IMath interface through IApplication (same as Curve2D.cs)
                    debugInfo += "🔧 Attempting to create Renga IApplication COM interface\n";
                    
                    // Try different ProgID variations for IApplication
                    string[] progIds = { "Renga.Application", "RengaApplication", "Renga.Application.1", "RengaApplication.1" };
                    Type appType = null;
                    string usedProgId = "";
                    
                    foreach (string progId in progIds)
                    {
                        try
                        {
                            appType = Type.GetTypeFromProgID(progId);
                            if (appType != null)
                            {
                                usedProgId = progId;
                                debugInfo += $"✅ Found Renga application type: {usedProgId}\n";
                                break;
                            }
                        }
                        catch
                        {
                            // Continue to next ProgID
                        }
                    }
                    
                    if (appType != null)
                    {
                        var rengaApp = Activator.CreateInstance(appType);
                        if (rengaApp != null)
                        {
                            debugInfo += "✅ Renga application instance created\n";
                            
                            // Cast to IApplication interface
                            var appInterface = rengaApp as Renga.IApplication;
                            if (appInterface != null)
                            {
                                debugInfo += "✅ IApplication interface obtained\n";
                                
                                // Access Math property from IApplication
                                debugInfo += "🔧 Accessing Math property from IApplication\n";
                                mathInterface = appInterface.Math;
                                
                                if (mathInterface != null)
                                {
                                    debugInfo += "✅ Renga IMath interface accessed successfully\n";
                                }
                                else
                                {
                                    debugInfo += "❌ Failed to access Math property from IApplication\n";
                                }
                            }
                            else
                            {
                                debugInfo += "❌ Failed to cast to IApplication interface\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create Renga application instance\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ No Renga application type found\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Error getting IMath interface: {ex.Message}\n";
                }
                
                if (mathInterface == null)
                {
                    debugInfo += "❌ Failed to get IMath interface\n";
                    return new Dictionary<string, object>
                    {
                        { "Curve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ IMath interface obtained successfully\n";
                
                // Convert each Dynamo curve to Renga Curve2D
                debugInfo += "🔄 Converting individual Dynamo curves to Renga Curve2D segments...\n";
                
                var rengaCurves = new List<Renga.ICurve2D>();
                
                for (int i = 0; i < dynamoCurves.Length; i++)
                {
                    var curve = dynamoCurves[i];
                    debugInfo += $"🔄 Converting curve {i + 1}/{dynamoCurves.Length}...\n";
                    
                    try
                    {
                        Renga.ICurve2D rengaSegment = null;
                        
                        if (curve is dg.Line line)
                        {
                            debugInfo += $"  📐 Converting Line segment\n";
                            var startPoint = new Renga.Point2D
                            {
                                X = line.StartPoint.X * 1000.0, // Convert to mm
                                Y = line.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = line.EndPoint.X * 1000.0,
                                Y = line.EndPoint.Y * 1000.0
                            };
                            
                            rengaSegment = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                            debugInfo += $"    📏 Line: ({startPoint.X:F2}, {startPoint.Y:F2}) to ({endPoint.X:F2}, {endPoint.Y:F2})\n";
                        }
                        else if (curve is dg.Arc arc)
                        {
                            debugInfo += $"  📐 Converting Arc segment\n";
                            var centerPoint = new Renga.Point2D
                            {
                                X = arc.CenterPoint.X * 1000.0,
                                Y = arc.CenterPoint.Y * 1000.0
                            };
                            var startPoint = new Renga.Point2D
                            {
                                X = arc.StartPoint.X * 1000.0,
                                Y = arc.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = arc.EndPoint.X * 1000.0,
                                Y = arc.EndPoint.Y * 1000.0
                            };
                            
                            // Determine if arc is counter-clockwise
                            bool isCounterClockwise = arc.SweepAngle > 0;
                            
                            rengaSegment = mathInterface.CreateArc2DByCenterStartEndPoints(
                                centerPoint, startPoint, endPoint, isCounterClockwise);
                            
                            debugInfo += $"    📏 Arc: Center({centerPoint.X:F2}, {centerPoint.Y:F2}), R={arc.Radius * 1000.0:F2}mm, Sweep={arc.SweepAngle:F2}°\n";
                        }
                        else
                        {
                            debugInfo += $"  ⚠️ Unsupported curve type: {curve.GetType().Name}\n";
                            debugInfo += $"  💡 Converting to line approximation\n";
                            
                            // Try to get start and end points for line approximation
                            try
                            {
                                var startPoint = new Renga.Point2D
                                {
                                    X = ((dynamic)curve).StartPoint.X * 1000.0,
                                    Y = ((dynamic)curve).StartPoint.Y * 1000.0
                                };
                                var endPoint = new Renga.Point2D
                                {
                                    X = ((dynamic)curve).EndPoint.X * 1000.0,
                                    Y = ((dynamic)curve).EndPoint.Y * 1000.0
                                };
                                
                                rengaSegment = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                                debugInfo += $"    📏 Line approximation: ({startPoint.X:F2}, {startPoint.Y:F2}) to ({endPoint.X:F2}, {endPoint.Y:F2})\n";
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"    ❌ Failed to create line approximation: {ex.Message}\n";
                            }
                        }
                        
                        if (rengaSegment != null)
                        {
                            rengaCurves.Add(rengaSegment);
                            debugInfo += $"  ✅ Curve {i + 1} converted successfully\n";
                        }
                        else
                        {
                            debugInfo += $"  ❌ Failed to convert curve {i + 1}\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"  ❌ Error converting curve {i + 1}: {ex.Message}\n";
                    }
                }
                
                if (rengaCurves.Count == 0)
                {
                    debugInfo += "❌ No curves were successfully converted\n";
                    return new Dictionary<string, object>
                    {
                        { "Curve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Successfully converted {rengaCurves.Count} curves\n";
                
                // Create composite curve if we have multiple segments
                Renga.ICurve2D finalRengaCurve = null;
                
                if (rengaCurves.Count == 1)
                {
                    finalRengaCurve = rengaCurves[0];
                    debugInfo += "✅ Using single segment curve\n";
                }
                else
                {
                    debugInfo += "🔄 Creating composite curve from segments...\n";
                    try
                    {
                        var curveArray = rengaCurves.ToArray();
                        finalRengaCurve = mathInterface.CreateCompositeCurve2D(curveArray);
                        
                        if (finalRengaCurve != null)
                        {
                            debugInfo += "✅ Composite curve created successfully\n";
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create composite curve\n";
                            debugInfo += "💡 Using first segment as fallback\n";
                            finalRengaCurve = rengaCurves[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Error creating composite curve: {ex.Message}\n";
                        debugInfo += "💡 Using first segment as fallback\n";
                        finalRengaCurve = rengaCurves[0];
                    }
                }
                
                if (finalRengaCurve != null)
                {
                    debugInfo += "✅ Renga ICurve2D created successfully!\n";
                    try
                    {
                        double length = finalRengaCurve.GetLength();
                        debugInfo += $"📏 Final curve length: {length:F2}mm ({length/1000.0:F3}m)\n";
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"⚠️ Could not get curve length: {ex.Message}\n";
                    }
                    
                    // Create DynRenga Curve2D wrapper
                    debugInfo += "🔄 Creating DynRenga Curve2D wrapper...\n";
                    try
                    {
                        var dynRengaCurve2D = new Curve2D(finalRengaCurve);
                        debugInfo += "✅ DynRenga Curve2D wrapper created successfully!\n";
                        debugInfo += "💡 This curve can be used with SetBaselinesWithDebugAlt() method\n";
                        
                        return new Dictionary<string, object>
                        {
                            { "Curve2D", dynRengaCurve2D },
                            { "Success", true },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Failed to create DynRenga Curve2D wrapper: {ex.Message}\n";
                        return new Dictionary<string, object>
                        {
                            { "Curve2D", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                }
                else
                {
                    debugInfo += "❌ Failed to create final Renga ICurve2D\n";
                    return new Dictionary<string, object>
                    {
                        { "Curve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when creating DynRenga Curve2D!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "Curve2D", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание Renga ICurve2D из массива отдельных Dynamo кривых (Line, Arc)
        /// Простой и надежный способ создания составной кривой из отдельных сегментов
        /// </summary>
        /// <param name="dynamoCurves">Массив отдельных Dynamo кривых (Line, Arc)</param>
        /// <returns>Созданная Renga ICurve2D и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "RengaCurve2D", "Success", "DebugInfo" })]
        public static Dictionary<string, object> CreateRengaCurve2DFromDynamoCurves(object[] dynamoCurves)
        {
            var debugInfo = "🔧 Creating Renga Curve2D from individual Dynamo curves...\n";
            
            try
            {
                if (dynamoCurves == null || dynamoCurves.Length == 0)
                {
                    debugInfo += "❌ Dynamo curves array cannot be null or empty\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"📊 Processing {dynamoCurves.Length} individual Dynamo curves\n";
                
                // Get IMath interface for curve creation
                debugInfo += "🔧 Getting IMath interface...\n";
                Renga.IMath mathInterface = null;
                
                try
                {
                    // Try to create IMath interface through IApplication (same as Curve2D.cs)
                    debugInfo += "🔧 Attempting to create Renga IApplication COM interface\n";
                    
                    // Try different ProgID variations for IApplication
                    string[] progIds = { "Renga.Application", "RengaApplication", "Renga.Application.1", "RengaApplication.1" };
                    Type appType = null;
                    string usedProgId = "";
                    
                    foreach (string progId in progIds)
                    {
                        try
                        {
                            appType = Type.GetTypeFromProgID(progId);
                            if (appType != null)
                            {
                                usedProgId = progId;
                                debugInfo += $"✅ Found Renga application type: {usedProgId}\n";
                                break;
                            }
                        }
                        catch
                        {
                            // Continue to next ProgID
                        }
                    }
                    
                    if (appType != null)
                    {
                        var rengaApp = Activator.CreateInstance(appType);
                        if (rengaApp != null)
                        {
                            debugInfo += "✅ Renga application instance created\n";
                            
                            // Cast to IApplication interface
                            var appInterface = rengaApp as Renga.IApplication;
                            if (appInterface != null)
                            {
                                debugInfo += "✅ IApplication interface obtained\n";
                                
                                // Access Math property from IApplication
                                debugInfo += "🔧 Accessing Math property from IApplication\n";
                                mathInterface = appInterface.Math;
                                
                                if (mathInterface != null)
                                {
                                    debugInfo += "✅ Renga IMath interface accessed successfully\n";
                                }
                                else
                                {
                                    debugInfo += "❌ Failed to access Math property from IApplication\n";
                                }
                            }
                            else
                            {
                                debugInfo += "❌ Failed to cast to IApplication interface\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create Renga application instance\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ No Renga application type found\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Error getting IMath interface: {ex.Message}\n";
                }
                
                if (mathInterface == null)
                {
                    debugInfo += "❌ Failed to get IMath interface\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ IMath interface obtained successfully\n";
                
                // Convert each Dynamo curve to Renga Curve2D
                debugInfo += "🔄 Converting individual Dynamo curves to Renga Curve2D segments...\n";
                
                var rengaCurves = new List<Renga.ICurve2D>();
                
                for (int i = 0; i < dynamoCurves.Length; i++)
                {
                    var curve = dynamoCurves[i];
                    debugInfo += $"🔄 Converting curve {i + 1}/{dynamoCurves.Length}...\n";
                    
                    try
                    {
                        Renga.ICurve2D rengaSegment = null;
                        
                        if (curve is dg.Line line)
                        {
                            debugInfo += $"  📐 Converting Line segment\n";
                            var startPoint = new Renga.Point2D
                            {
                                X = line.StartPoint.X * 1000.0, // Convert to mm
                                Y = line.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = line.EndPoint.X * 1000.0,
                                Y = line.EndPoint.Y * 1000.0
                            };
                            
                            rengaSegment = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                            debugInfo += $"    📏 Line: ({startPoint.X:F2}, {startPoint.Y:F2}) to ({endPoint.X:F2}, {endPoint.Y:F2})\n";
                        }
                        else if (curve is dg.Arc arc)
                        {
                            debugInfo += $"  📐 Converting Arc segment\n";
                            var centerPoint = new Renga.Point2D
                            {
                                X = arc.CenterPoint.X * 1000.0,
                                Y = arc.CenterPoint.Y * 1000.0
                            };
                            var startPoint = new Renga.Point2D
                            {
                                X = arc.StartPoint.X * 1000.0,
                                Y = arc.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = arc.EndPoint.X * 1000.0,
                                Y = arc.EndPoint.Y * 1000.0
                            };
                            
                            // Determine if arc is counter-clockwise
                            bool isCounterClockwise = arc.SweepAngle > 0;
                            
                            rengaSegment = mathInterface.CreateArc2DByCenterStartEndPoints(
                                centerPoint, startPoint, endPoint, isCounterClockwise);
                            
                            debugInfo += $"    📏 Arc: Center({centerPoint.X:F2}, {centerPoint.Y:F2}), R={arc.Radius * 1000.0:F2}mm, Sweep={arc.SweepAngle:F2}°\n";
                        }
                        else
                        {
                            debugInfo += $"  ⚠️ Unsupported curve type: {curve.GetType().Name}\n";
                            debugInfo += $"  💡 Converting to line approximation\n";
                            
                            // Try to get start and end points for line approximation
                            try
                            {
                                var startPoint = new Renga.Point2D
                                {
                                    X = ((dynamic)curve).StartPoint.X * 1000.0,
                                    Y = ((dynamic)curve).StartPoint.Y * 1000.0
                                };
                                var endPoint = new Renga.Point2D
                                {
                                    X = ((dynamic)curve).EndPoint.X * 1000.0,
                                    Y = ((dynamic)curve).EndPoint.Y * 1000.0
                                };
                                
                                rengaSegment = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                                debugInfo += $"    📏 Line approximation: ({startPoint.X:F2}, {startPoint.Y:F2}) to ({endPoint.X:F2}, {endPoint.Y:F2})\n";
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"    ❌ Failed to create line approximation: {ex.Message}\n";
                            }
                        }
                        
                        if (rengaSegment != null)
                        {
                            rengaCurves.Add(rengaSegment);
                            debugInfo += $"  ✅ Curve {i + 1} converted successfully\n";
                        }
                        else
                        {
                            debugInfo += $"  ❌ Failed to convert curve {i + 1}\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"  ❌ Error converting curve {i + 1}: {ex.Message}\n";
                    }
                }
                
                if (rengaCurves.Count == 0)
                {
                    debugInfo += "❌ No curves were successfully converted\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Successfully converted {rengaCurves.Count} curves\n";
                
                // Create composite curve if we have multiple segments
                Renga.ICurve2D finalCurve = null;
                
                if (rengaCurves.Count == 1)
                {
                    finalCurve = rengaCurves[0];
                    debugInfo += "✅ Using single segment curve\n";
                }
                else
                {
                    debugInfo += "🔄 Creating composite curve from segments...\n";
                    try
                    {
                        var curveArray = rengaCurves.ToArray();
                        finalCurve = mathInterface.CreateCompositeCurve2D(curveArray);
                        
                        if (finalCurve != null)
                        {
                            debugInfo += "✅ Composite curve created successfully\n";
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create composite curve\n";
                            debugInfo += "💡 Using first segment as fallback\n";
                            finalCurve = rengaCurves[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Error creating composite curve: {ex.Message}\n";
                        debugInfo += "💡 Using first segment as fallback\n";
                        finalCurve = rengaCurves[0];
                    }
                }
                
                if (finalCurve != null)
                {
                    debugInfo += "✅ Renga Curve2D created successfully!\n";
                    try
                    {
                        double length = finalCurve.GetLength();
                        debugInfo += $"📏 Final curve length: {length:F2}mm ({length/1000.0:F3}m)\n";
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"⚠️ Could not get curve length: {ex.Message}\n";
                    }
                    debugInfo += "💡 This curve can be used with SetBaseline() method\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", finalCurve },
                        { "Success", true },
                        { "DebugInfo", debugInfo }
                    };
                }
                else
                {
                    debugInfo += "❌ Failed to create final Renga Curve2D\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when creating Renga Curve2D!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "RengaCurve2D", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Установка 2D базовых линий для массива объектов с использованием нативных Renga ICurve2D
        /// Прямое использование RengaBIM SDK интерфейсов для максимальной совместимости
        /// </summary>
        /// <param name="baselineObjects">Массив объектов Baseline2DObject</param>
        /// <param name="rengaCurves">Массив нативных Renga ICurve2D</param>
        /// <returns>Результаты операций и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "Results", "DebugInfo" })]
        public static Dictionary<string, object> SetBaselinesWithRengaCurves(Baseline2DObject[] baselineObjects, Renga.ICurve2D[] rengaCurves)
        {
            var debugInfo = "🔧 Setting baselines with native Renga ICurve2D curves...\n";
            var results = new List<Dictionary<string, object>>();
            bool overallSuccess = true;
            
            try
            {
                if (baselineObjects == null || baselineObjects.Length == 0)
                {
                    debugInfo += "❌ Baseline2DObject array cannot be null or empty\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "Results", new Dictionary<string, object>[0] },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (rengaCurves == null || rengaCurves.Length == 0)
                {
                    debugInfo += "❌ Renga ICurve2D array cannot be null or empty\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "Results", new Dictionary<string, object>[0] },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"📊 Processing {baselineObjects.Length} objects with {rengaCurves.Length} native Renga curves\n";
                
                // Check if arrays have compatible lengths
                int maxLength = Math.Max(baselineObjects.Length, rengaCurves.Length);
                debugInfo += $"📏 Using maximum length: {maxLength}\n";
                
                for (int i = 0; i < maxLength; i++)
                {
                    debugInfo += $"\n🔄 Processing object {i + 1}/{maxLength}...\n";
                    
                    var result = new Dictionary<string, object>
                    {
                        { "Index", i },
                        { "Success", false },
                        { "ObjectId", "N/A" },
                        { "CurveId", "N/A" },
                        { "Error", "Not processed" }
                    };
                    
                    try
                    {
                        // Get object and curve for this iteration
                        var obj = i < baselineObjects.Length ? baselineObjects[i] : baselineObjects[baselineObjects.Length - 1];
                        var curve = i < rengaCurves.Length ? rengaCurves[i] : rengaCurves[rengaCurves.Length - 1];
                        
                        if (obj == null)
                        {
                            debugInfo += $"❌ Object {i + 1} is null\n";
                            result["Error"] = "Object is null";
                            results.Add(result);
                            overallSuccess = false;
                            continue;
                        }
                        
                        if (curve == null)
                        {
                            debugInfo += $"❌ Renga curve {i + 1} is null\n";
                            result["Error"] = "Renga curve is null";
                            results.Add(result);
                            overallSuccess = false;
                            continue;
                        }
                        
                        debugInfo += $"✅ Object {i + 1} and Renga curve {i + 1} are valid\n";
                        
                        // Try to set baseline using the native Renga curve directly
                        var setResult = obj.SetBaselineWithRengaCurve2D(curve);
                        
                        if (setResult.ContainsKey("Success") && (bool)setResult["Success"])
                        {
                            debugInfo += $"✅ Object {i + 1} baseline set successfully with native Renga curve\n";
                            result["Success"] = true;
                            result["ObjectId"] = obj._i != null ? "Valid" : "Invalid";
                            result["CurveId"] = "Native Renga curve set successfully";
                            result["Error"] = "None";
                        }
                        else
                        {
                            debugInfo += $"❌ Object {i + 1} baseline setting failed with native Renga curve\n";
                            result["Success"] = false;
                            result["ObjectId"] = obj._i != null ? "Valid" : "Invalid";
                            result["CurveId"] = "Native Renga curve failed to set";
                            result["Error"] = setResult.ContainsKey("DebugInfo") ? setResult["DebugInfo"].ToString() : "Unknown error";
                            overallSuccess = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Error processing object {i + 1}: {ex.Message}\n";
                        result["Error"] = ex.Message;
                        overallSuccess = false;
                    }
                    
                    results.Add(result);
                }
                
                debugInfo += $"\n📊 Batch processing with native Renga curves completed: {results.Count(r => (bool)r["Success"])}/{results.Count} successful\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", overallSuccess },
                    { "Results", results.ToArray() },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error in batch processing with native Renga curves: {ex.Message}\n";
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "Results", results.ToArray() },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Установка 2D базовой линии объекта с использованием нативного Renga ICurve2D
        /// Прямое использование RengaBIM SDK интерфейса для максимальной совместимости
        /// </summary>
        /// <param name="rengaCurve2D">Нативный Renga ICurve2D для установки базовой линии</param>
        /// <returns>Результат операции и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> SetBaselineWithRengaCurve2D(Renga.ICurve2D rengaCurve2D)
        {
            var debugInfo = "🔧 Setting baseline with native Renga ICurve2D...\n";
            
            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ Baseline2DObject interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (rengaCurve2D == null)
                {
                    debugInfo += "❌ Renga ICurve2D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Both interfaces are valid\n";
                
                // Get curve information
                try
                {
                    double length = rengaCurve2D.GetLength();
                    debugInfo += $"📏 Curve length: {length:F2}mm ({length/1000.0:F3}m)\n";
                    
                    // Get curve type
                    var curveType = rengaCurve2D.Curve2DType;
                    debugInfo += $"📐 Curve type: {curveType}\n";
                    
                    // Check if closed
                    bool isClosed = rengaCurve2D.IsClosed();
                    debugInfo += $"📐 Is closed: {isClosed}\n";
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Could not get curve properties: {ex.Message}\n";
                }
                
                // Set the baseline using native Renga interface
                debugInfo += "🔄 Setting baseline with native Renga interface...\n";
                this._i.SetBaseline(rengaCurve2D);
                
                debugInfo += "✅ Baseline set successfully!\n";
                debugInfo += "💡 The object's 2D baseline has been updated using native Renga interface\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                debugInfo += $"❌ COM Error when setting baseline!\n";
                debugInfo += $"❌ Error Code: 0x{comEx.ErrorCode:X8}\n";
                debugInfo += $"❌ Error Message: {comEx.Message}\n";
                
                // Provide specific guidance based on error code
                switch (comEx.ErrorCode)
                {
                    case unchecked((int)0x80010105): // RPC_E_SERVERFAULT
                        debugInfo += "💡 RPC_E_SERVERFAULT - Possible causes:\n";
                        debugInfo += "   • Object doesn't support baseline modification\n";
                        debugInfo += "   • Object has dependent objects (like Roof)\n";
                        debugInfo += "   • Baseline curve is invalid for this object type\n";
                        debugInfo += "   • Object is not in correct state for baseline modification\n";
                        debugInfo += "   • Curve coordinates are in wrong units (should be millimeters)\n";
                        debugInfo += "   • Curve geometry is not compatible with the object type\n";
                        break;
                        
                    case unchecked((int)0x80004005): // E_FAIL
                        debugInfo += "💡 E_FAIL - Possible causes:\n";
                        debugInfo += "   • Object type doesn't support baseline modification\n";
                        debugInfo += "   • Baseline curve is geometrically invalid\n";
                        debugInfo += "   • Curve is not in correct coordinate system\n";
                        debugInfo += "   • Object has constraints that prevent baseline modification\n";
                        break;
                        
                    case unchecked((int)0x80070057): // E_INVALIDARG
                        debugInfo += "💡 E_INVALIDARG - Possible causes:\n";
                        debugInfo += "   • Baseline curve has invalid geometry\n";
                        debugInfo += "   • Curve coordinates are out of valid range\n";
                        debugInfo += "   • Curve type is not supported for this object\n";
                        break;
                        
                    case unchecked((int)0x80004001): // E_NOTIMPL
                        debugInfo += "💡 E_NOTIMPL - SetBaseline is not implemented for this object type\n";
                        break;
                        
                    default:
                        debugInfo += "💡 Unknown COM error - check curve validity and Renga installation\n";
                        break;
                }
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when setting baseline!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Установка 2D базовой линии объекта в его собственной системе координат
        /// Ограничения: Невозможно редактировать базовую линию объекта с зависимыми объектами (например, Крыша)
        /// Использует RengaBIM SDK для корректной работы с базовыми линиями
        /// </summary>
        /// <param name="baseline">Новая базовая линия объекта (должна быть создана через Curve2D.ByLineSegment() или аналогичные методы)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBaseline(Curve2D baseline)
        {
            // Pre-validation checks
            if (this._i == null)
            {
                throw new InvalidOperationException("Baseline2DObject interface is not initialized. Cannot set baseline.");
            }

            if (baseline == null)
            {
                throw new ArgumentNullException(nameof(baseline), "Baseline Curve2D cannot be null.");
            }

            if (baseline._i == null)
            {
                throw new InvalidOperationException("Baseline Curve2D interface is not initialized. Make sure to use Curve2D.ByLineSegment() or similar method.");
            }

            // Validate curve properties using RengaBIM SDK patterns
            try
            {
                double length = baseline._i.GetLength();
                if (length <= 0)
                {
                    throw new ArgumentException($"Baseline curve has invalid length: {length}. Curve must have positive length.");
                }

                // Check if curve is too small (less than 1mm) - RengaBIM SDK uses millimeters
                if (length < 1.0)
                {
                    throw new ArgumentException($"Baseline curve is too small (length: {length}mm). Minimum length is 1mm.");
                }

                // Check if curve is too large (more than 1000m)
                if (length > 1000000.0)
                {
                    throw new ArgumentException($"Baseline curve is too large (length: {length/1000.0:F2}m). Maximum length is 1000m.");
                }

                // Validate curve type compatibility with baseline operations
                var curveType = baseline.Curve2DType;
                if (curveType != null)
                {
                    switch (curveType.ToString())
                    {
                        case "Curve2DType_Undefined":
                            throw new ArgumentException("Baseline curve has undefined type. Use Curve2D.ByLineSegment() or similar method to create a proper curve.");
                        case "Curve2DType_LineSegment":
                        case "Curve2DType_Arc":
                        case "Curve2DType_PolyCurve":
                            // These are supported
                            break;
                        default:
                            throw new ArgumentException($"Unsupported curve type for baseline: {curveType}");
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                throw new InvalidOperationException($"Failed to validate baseline curve: {comEx.Message}", comEx);
            }

            try
            {
                // Extract the native Renga ICurve2D interface from DynRenga Curve2D
                Renga.ICurve2D rengaCurve2D = baseline._i;
                
                if (rengaCurve2D == null)
                {
                    throw new InvalidOperationException("Failed to extract Renga ICurve2D interface from DynRenga Curve2D. The curve may not be properly initialized.");
                }
                
                // Use RengaBIM SDK SetBaseline method with native interface
                this._i.SetBaseline(rengaCurve2D);
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                // Handle specific COM errors with detailed messages based on RengaBIM SDK documentation
                switch (comEx.ErrorCode)
                {
                    case unchecked((int)0x80010105): // RPC_E_SERVERFAULT
                        throw new InvalidOperationException(
                            "RPC_E_SERVERFAULT when setting baseline. This may indicate:\n" +
                            "1. The object doesn't support baseline modification\n" +
                            "2. The object has dependent objects (like Roof)\n" +
                            "3. The baseline curve is invalid for this object type\n" +
                            "4. The object is not in the correct state for baseline modification\n" +
                            "5. The curve coordinates are in wrong units (should be in millimeters)\n" +
                            "6. The curve geometry is not compatible with the object type\n" +
                            "7. Renga application is not properly initialized", comEx);

                    case unchecked((int)0x80004005): // E_FAIL
                        throw new InvalidOperationException(
                            "E_FAIL when setting baseline. This may indicate:\n" +
                            "1. The object type doesn't support baseline modification\n" +
                            "2. The baseline curve is geometrically invalid\n" +
                            "3. The curve is not in the correct coordinate system\n" +
                            "4. The object has constraints that prevent baseline modification\n" +
                            "5. The curve type is not supported for this object", comEx);

                    case unchecked((int)0x80070057): // E_INVALIDARG
                        throw new ArgumentException(
                            "Invalid argument when setting baseline. This may indicate:\n" +
                            "1. The baseline curve has invalid geometry\n" +
                            "2. The curve coordinates are out of valid range\n" +
                            "3. The curve type is not supported for this object\n" +
                            "4. The curve is not properly initialized", comEx);

                    case unchecked((int)0x80004001): // E_NOTIMPL
                        throw new NotSupportedException(
                            "SetBaseline is not implemented for this object type. " +
                            "This object may not support baseline modification.", comEx);

                    default:
                        throw new InvalidOperationException(
                            $"COM error when setting baseline (Error Code: 0x{comEx.ErrorCode:X8}): {comEx.Message}", comEx);
                }
            }
        }
        
        /// <summary>
        /// Установка 2D базовой линии объекта с отладочной информацией
        /// Включает проверку совместимости объекта и кривой
        /// </summary>
        /// <param name="baseline">Новая базовая линия объекта</param>
        /// <returns>Результат операции и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> SetBaselineWithDebug(Curve2D baseline)
        {
            var debugInfo = "🔧 Setting 2D baseline with enhanced validation...\n";
            
            try
            {
                // Check Baseline2DObject interface
                if (this._i == null)
                {
                    debugInfo += "❌ Baseline2DObject interface is not initialized\n";
                    debugInfo += "💡 This usually means the object doesn't support IBaseline2DObject\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                debugInfo += "✅ Baseline2DObject interface initialized\n";
                
                // Check baseline parameter
                if (baseline == null)
                {
                    debugInfo += "❌ Baseline Curve2D cannot be null\n";
                    debugInfo += "💡 Provide a valid Curve2D object (e.g., from Curve2D.ByLineSegment)\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (baseline._i == null)
                {
                    debugInfo += "❌ Baseline Curve2D interface is not initialized\n";
                    debugInfo += "💡 Make sure to use Curve2D.ByLineSegment() or similar method\n";
                    debugInfo += "💡 Check that the curve creation was successful\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                debugInfo += "✅ Baseline Curve2D interface initialized\n";
                
                // Validate curve properties
                try
                {
                    double length = baseline._i.GetLength();
                    debugInfo += $"📏 Baseline curve length: {length:F2}mm ({length/1000.0:F3}m)\n";
                    
                    if (length <= 0)
                    {
                        debugInfo += "❌ Baseline curve has invalid length (≤ 0)\n";
                        debugInfo += "💡 Curve must have positive length\n";
                        return new Dictionary<string, object>
                        {
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    if (length < 1.0)
                    {
                        debugInfo += "⚠️ Baseline curve is very small (< 1mm)\n";
                        debugInfo += "💡 Consider using larger coordinates (multiply by 1000 for mm)\n";
                    }
                    
                    if (length > 1000000.0)
                    {
                        debugInfo += "⚠️ Baseline curve is very large (> 1000m)\n";
                        debugInfo += "💡 Consider using smaller coordinates (divide by 1000 for m)\n";
                    }
                    
                    debugInfo += $"📐 Baseline type: {baseline.GetType().Name}\n";
                    debugInfo += $"📐 Baseline curve type: {baseline.Curve2DType}\n";
                    debugInfo += $"📐 Is closed: {baseline.IsClosed}\n";
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"❌ Failed to validate baseline curve: {comEx.Message}\n";
                    debugInfo += $"❌ COM Error Code: 0x{comEx.ErrorCode:X8}\n";
                    debugInfo += "💡 The curve may be corrupted or invalid\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Check object compatibility before attempting to set baseline
                debugInfo += "🔍 Checking object compatibility for baseline modification...\n";
                
                // Try to get current baseline to test if object supports baseline operations
                try
                {
                    var currentBaseline = this.GetBaseline();
                    if (currentBaseline != null)
                    {
                        debugInfo += "✅ Object has existing baseline - baseline operations are supported\n";
                        debugInfo += $"📏 Current baseline length: {currentBaseline._i.GetLength():F2}mm\n";
                    }
                    else
                    {
                        debugInfo += "⚠️ Object doesn't have existing baseline\n";
                        debugInfo += "💡 This might indicate the object doesn't support baselines\n";
                    }
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"⚠️ Failed to get current baseline: {comEx.Message}\n";
                    debugInfo += $"⚠️ COM Error Code: 0x{comEx.ErrorCode:X8}\n";
                    debugInfo += "💡 This suggests the object may not support baseline operations\n";
                }
                
                // Check if the curve is closed and if the object supports closed curves
                if (baseline.IsClosed)
                {
                    debugInfo += "🔍 Curve is closed - checking if object supports closed baselines...\n";
                    debugInfo += "💡 Some object types may not support closed baseline curves\n";
                    debugInfo += "💡 Consider using an open curve (not closed) for this object type\n";
                }
                
                // Check curve type compatibility with object
                var curveType = baseline.Curve2DType;
                if (curveType != null)
                {
                    debugInfo += $"🔍 Curve type: {curveType} - checking compatibility...\n";
                    
                    switch (curveType.ToString())
                    {
                        case "Curve2DType_PolyCurve":
                            debugInfo += "⚠️ PolyCurve baselines may not be supported by all object types\n";
                            debugInfo += "💡 Consider converting to LineSegment or Arc if possible\n";
                            break;
                        case "Curve2DType_LineSegment":
                            debugInfo += "✅ LineSegment is most compatible with baseline operations\n";
                            break;
                        case "Curve2DType_Arc":
                            debugInfo += "✅ Arc is generally supported for baseline operations\n";
                            break;
                    }
                }
                
                // Attempt to set baseline
                debugInfo += "🔄 Attempting to set baseline...\n";
                
                // Extract the native Renga ICurve2D interface from DynRenga Curve2D
                debugInfo += "🔧 Extracting native Renga ICurve2D interface...\n";
                Renga.ICurve2D rengaCurve2D = baseline._i;
                
                if (rengaCurve2D == null)
                {
                    debugInfo += "❌ Failed to extract Renga ICurve2D interface from DynRenga Curve2D\n";
                    debugInfo += "💡 The curve may not be properly initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Native Renga ICurve2D interface extracted successfully\n";
                debugInfo += "🔄 Calling RengaBIM SDK SetBaseline method...\n";
                
                // Use RengaBIM SDK SetBaseline method with native interface
                this._i.SetBaseline(rengaCurve2D);
                debugInfo += "✅ Baseline set successfully!\n";
                debugInfo += "💡 The object's 2D baseline has been updated\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                debugInfo += $"❌ COM Error when setting baseline!\n";
                debugInfo += $"❌ Error Code: 0x{comEx.ErrorCode:X8}\n";
                debugInfo += $"❌ Error Message: {comEx.Message}\n";
                
                // Provide specific guidance based on error code
                switch (comEx.ErrorCode)
                {
                    case unchecked((int)0x80010105): // RPC_E_SERVERFAULT
                        debugInfo += "💡 RPC_E_SERVERFAULT - Possible causes:\n";
                        debugInfo += "   • Object doesn't support baseline modification\n";
                        debugInfo += "   • Object has dependent objects (like Roof)\n";
                        debugInfo += "   • Baseline curve is invalid for this object type\n";
                        debugInfo += "   • Object is not in correct state for baseline modification\n";
                        debugInfo += "   • Curve coordinates are in wrong units (should be millimeters)\n";
                        break;
                        
                    case unchecked((int)0x80004005): // E_FAIL
                        debugInfo += "💡 E_FAIL - Possible causes:\n";
                        debugInfo += "   • Object type doesn't support baseline modification\n";
                        debugInfo += "   • Baseline curve is geometrically invalid\n";
                        debugInfo += "   • Curve is not in correct coordinate system\n";
                        break;
                        
                    case unchecked((int)0x80070057): // E_INVALIDARG
                        debugInfo += "💡 E_INVALIDARG - Possible causes:\n";
                        debugInfo += "   • Baseline curve has invalid geometry\n";
                        debugInfo += "   • Curve coordinates are out of valid range\n";
                        debugInfo += "   • Curve type is not supported for this object\n";
                        break;
                        
                    case unchecked((int)0x80004001): // E_NOTIMPL
                        debugInfo += "💡 E_NOTIMPL - This object type doesn't support baseline modification\n";
                        break;
                        
                    default:
                        debugInfo += "💡 Unknown COM error - check object type and curve validity\n";
                        break;
                }
                
                debugInfo += $"\n🔍 Debug Information:\n";
                debugInfo += $"   Baseline2DObject._i is null: {this._i == null}\n";
                debugInfo += $"   Baseline is null: {baseline == null}\n";
                debugInfo += $"   Baseline._i is null: {baseline?._i == null}\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when setting baseline!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                debugInfo += $"\n🔍 Debug Information:\n";
                debugInfo += $"   Baseline2DObject._i is null: {this._i == null}\n";
                debugInfo += $"   Baseline is null: {baseline == null}\n";
                debugInfo += $"   Baseline._i is null: {baseline?._i == null}\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Установка 2D базовой линии объекта в указанной системе координат
        /// Ограничения: Невозможно редактировать базовую линию объекта с зависимыми объектами (например, Крыша)
        /// </summary>
        /// <param name="placement2D">Система координат, в которую будет преобразована 2D базовая линия</param>
        /// <param name="baselineInCS">Новая базовая линия объекта в указанной системе координат</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBaselineInCS(DynGeometry.Placement2D placement2D, Curve2D baselineInCS)
        {
            this._i.SetBaselineInCS(placement2D.ToRengaPlacement2D(), baselineInCS._i);
        }

        /// <summary>
        /// Получение базовой линии в указанной системе координат с преобразованием в Dynamo
        /// </summary>
        /// <param name="placement2D">Система координат</param>
        /// <returns>Dynamo кривая в указанной СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.Curve GetBaselineInCSAsDynamo(DynGeometry.Placement2D placement2D)
        {
            Curve2D baselineInCS = this.GetBaselineInCS(placement2D);
            if (baselineInCS == null) return null;
            
            // Use the curve type-aware conversion
            return Curve2D.CreateDynamoCurveFromSegment(baselineInCS, 2);
        }

        /// <summary>
        /// Получение базовой линии в глобальной системе координат как Dynamo кривая
        /// </summary>
        /// <returns>Dynamo кривая в глобальной СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.Curve GetBaselineInGlobalCS()
        {
            Curve2D baseline = this.GetBaseline();
            if (baseline == null) return null;
            
            // Use the curve type-aware conversion
            return Curve2D.CreateDynamoCurveFromSegment(baseline, 2);
        }

        /// <summary>
        /// Получение системы координат объекта (если доступна)
        /// </summary>
        /// <returns>Система координат объекта или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynGeometry.Placement2D GetObjectCoordinateSystem()
        {
            try
            {
                // Try to get the object's coordinate system
                // This might need to be implemented based on your specific object type
                return null; // Placeholder - implement based on your needs
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Преобразование базовой линии в Dynamo с использованием указанной системы координат
        /// </summary>
        /// <param name="targetCoordinateSystem">Целевая система координат</param>
        /// <returns>Dynamo кривая в целевой СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.Curve TransformBaselineToCoordinateSystem(DynGeometry.Placement2D targetCoordinateSystem)
        {
            return this.GetBaselineInCSAsDynamo(targetCoordinateSystem);
        }

        /// <summary>
        /// Получение информации о системе координат базовой линии
        /// </summary>
        /// <returns>Строка с информацией о СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetCoordinateSystemInfo()
        {
            try
            {
                var info = new System.Text.StringBuilder();
                
                // Get baseline in object's coordinate system
                Curve2D baseline = this.GetBaseline();
                if (baseline != null)
                {
                    info.AppendLine("Object's Coordinate System:");
                    info.AppendLine($"Baseline Type: {baseline.Curve2DType}");
                    info.AppendLine($"Is Closed: {baseline.IsClosed}");
                    info.AppendLine($"Length: {baseline._i.GetLength() / 1000.0:F3} m");
                }
                
                return info.ToString();
            }
            catch (System.Exception ex)
            {
                return $"Error getting coordinate system info: {ex.Message}";
            }
        }

        /// <summary>
        /// Создание Renga Curve2D из Dynamo PolyCurve для использования с базовыми линиями
        /// Правильно конвертирует Dynamo геометрию в RengaBIM SDK формат
        /// </summary>
        /// <param name="dynamoPolyCurve">Dynamo PolyCurve для преобразования</param>
        /// <returns>Renga Curve2D и информация об операции</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "RengaCurve2D", "Success", "DebugInfo" })]
        public static Dictionary<string, object> CreateRengaCurve2DFromDynamoPolyCurve(dg.PolyCurve dynamoPolyCurve)
        {
            var debugInfo = "🔧 Creating Renga Curve2D from Dynamo PolyCurve...\n";
            
            try
            {
                if (dynamoPolyCurve == null)
                {
                    debugInfo += "❌ Dynamo PolyCurve cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"📐 Dynamo PolyCurve is closed: {dynamoPolyCurve.IsClosed}\n";
                debugInfo += $"📏 Dynamo PolyCurve length: {dynamoPolyCurve.Length:F2}m\n";
                
                // Get IMath interface for curve creation using the same pattern as Curve2D.cs
                debugInfo += "🔧 Getting IMath interface...\n";
                Renga.IMath mathInterface = null;
                
                try
                {
                    // Try to create IMath interface through IApplication (same as Curve2D.cs)
                    debugInfo += "🔧 Attempting to create Renga IApplication COM interface\n";
                    
                    // Try different ProgID variations for IApplication
                    string[] progIds = { "Renga.Application", "RengaApplication", "Renga.Application.1", "RengaApplication.1" };
                    Type appType = null;
                    string usedProgId = "";
                    
                    foreach (string progId in progIds)
                    {
                        try
                        {
                            appType = Type.GetTypeFromProgID(progId);
                            if (appType != null)
                            {
                                usedProgId = progId;
                                debugInfo += $"✅ Found Renga application type: {usedProgId}\n";
                                break;
                            }
                        }
                        catch
                        {
                            // Continue to next ProgID
                        }
                    }
                    
                    if (appType != null)
                    {
                        var rengaApp = Activator.CreateInstance(appType);
                        if (rengaApp != null)
                        {
                            debugInfo += "✅ Renga application instance created\n";
                            
                            // Cast to IApplication interface
                            var appInterface = rengaApp as Renga.IApplication;
                            if (appInterface != null)
                            {
                                debugInfo += "✅ IApplication interface obtained\n";
                                
                                // Access Math property from IApplication
                                debugInfo += "🔧 Accessing Math property from IApplication\n";
                                mathInterface = appInterface.Math;
                                
                                if (mathInterface != null)
                                {
                                    debugInfo += "✅ Renga IMath interface accessed successfully\n";
                                }
                                else
                                {
                                    debugInfo += "❌ Failed to access Math property from IApplication\n";
                                }
                            }
                            else
                            {
                                debugInfo += "❌ Failed to cast to IApplication interface\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create Renga application instance\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ No Renga application type found\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Error getting IMath interface: {ex.Message}\n";
                }
                if (mathInterface == null)
                {
                    debugInfo += "❌ Failed to get IMath interface\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ IMath interface obtained successfully\n";
                
                // Convert Dynamo PolyCurve to Renga Curve2D
                debugInfo += "🔄 Converting Dynamo PolyCurve to Renga Curve2D...\n";
                
                // Get curve segments from Dynamo PolyCurve
                var curveSegments = dynamoPolyCurve.Curves();
                debugInfo += $"📊 Found {curveSegments.Length} curve segments\n";
                
                if (curveSegments.Length == 0)
                {
                    debugInfo += "❌ No curve segments found in Dynamo PolyCurve\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Convert each segment to Renga Curve2D
                var rengaCurves = new List<Renga.ICurve2D>();
                
                for (int i = 0; i < curveSegments.Length; i++)
                {
                    var segment = curveSegments[i];
                    debugInfo += $"🔄 Converting segment {i + 1}/{curveSegments.Length}...\n";
                    
                    try
                    {
                        Renga.ICurve2D rengaSegment = null;
                        
                        if (segment is dg.Line line)
                        {
                            debugInfo += $"  📐 Converting Line segment\n";
                            var startPoint = new Renga.Point2D
                            {
                                X = line.StartPoint.X * 1000.0, // Convert to mm
                                Y = line.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = line.EndPoint.X * 1000.0,
                                Y = line.EndPoint.Y * 1000.0
                            };
                            
                            rengaSegment = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                        }
                        else if (segment is dg.Arc arc)
                        {
                            debugInfo += $"  📐 Converting Arc segment\n";
                            var centerPoint = new Renga.Point2D
                            {
                                X = arc.CenterPoint.X * 1000.0,
                                Y = arc.CenterPoint.Y * 1000.0
                            };
                            var startPoint = new Renga.Point2D
                            {
                                X = arc.StartPoint.X * 1000.0,
                                Y = arc.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = arc.EndPoint.X * 1000.0,
                                Y = arc.EndPoint.Y * 1000.0
                            };
                            
                            // Determine if arc is counter-clockwise
                            bool isCounterClockwise = arc.SweepAngle > 0;
                            
                            rengaSegment = mathInterface.CreateArc2DByCenterStartEndPoints(
                                centerPoint, startPoint, endPoint, isCounterClockwise);
                        }
                        else
                        {
                            debugInfo += $"  ⚠️ Unsupported segment type: {segment.GetType().Name}\n";
                            debugInfo += $"  💡 Converting to line approximation\n";
                            
                            // Convert to line approximation
                            var startPoint = new Renga.Point2D
                            {
                                X = segment.StartPoint.X * 1000.0,
                                Y = segment.StartPoint.Y * 1000.0
                            };
                            var endPoint = new Renga.Point2D
                            {
                                X = segment.EndPoint.X * 1000.0,
                                Y = segment.EndPoint.Y * 1000.0
                            };
                            
                            rengaSegment = mathInterface.CreateLineSegment2D(startPoint, endPoint);
                        }
                        
                        if (rengaSegment != null)
                        {
                            rengaCurves.Add(rengaSegment);
                            debugInfo += $"  ✅ Segment {i + 1} converted successfully\n";
                        }
                        else
                        {
                            debugInfo += $"  ❌ Failed to convert segment {i + 1}\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"  ❌ Error converting segment {i + 1}: {ex.Message}\n";
                    }
                }
                
                if (rengaCurves.Count == 0)
                {
                    debugInfo += "❌ No segments were successfully converted\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Successfully converted {rengaCurves.Count} segments\n";
                
                // Create composite curve if we have multiple segments
                Renga.ICurve2D finalCurve = null;
                
                if (rengaCurves.Count == 1)
                {
                    finalCurve = rengaCurves[0];
                    debugInfo += "✅ Using single segment curve\n";
                }
                else
                {
                    debugInfo += "🔄 Creating composite curve from segments...\n";
                    try
                    {
                        var curveArray = rengaCurves.ToArray();
                        finalCurve = mathInterface.CreateCompositeCurve2D(curveArray);
                        
                        if (finalCurve != null)
                        {
                            debugInfo += "✅ Composite curve created successfully\n";
                        }
                        else
                        {
                            debugInfo += "❌ Failed to create composite curve\n";
                            debugInfo += "💡 Using first segment as fallback\n";
                            finalCurve = rengaCurves[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Error creating composite curve: {ex.Message}\n";
                        debugInfo += "💡 Using first segment as fallback\n";
                        finalCurve = rengaCurves[0];
                    }
                }
                
                if (finalCurve != null)
                {
                    debugInfo += "✅ Renga Curve2D created successfully!\n";
                    debugInfo += $"📏 Final curve length: {finalCurve.GetLength():F2}mm\n";
                    debugInfo += "💡 This curve can be used with SetBaseline() method\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", finalCurve },
                        { "Success", true },
                        { "DebugInfo", debugInfo }
                    };
                }
                else
                {
                    debugInfo += "❌ Failed to create final Renga Curve2D\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when creating Renga Curve2D!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "RengaCurve2D", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание нативного Renga ICurve2D из DynRenga Curve2D для использования с базовыми линиями
        /// Обеспечивает правильное извлечение интерфейса RengaBIM SDK
        /// </summary>
        /// <param name="dynRengaCurve2D">DynRenga Curve2D для преобразования</param>
        /// <returns>Нативный Renga ICurve2D и информация об операции</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "RengaCurve2D", "Success", "DebugInfo" })]
        public static Dictionary<string, object> ExtractRengaCurve2D(Curve2D dynRengaCurve2D)
        {
            var debugInfo = "🔧 Extracting native Renga ICurve2D from DynRenga Curve2D...\n";
            
            try
            {
                if (dynRengaCurve2D == null)
                {
                    debugInfo += "❌ DynRenga Curve2D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (dynRengaCurve2D._i == null)
                {
                    debugInfo += "❌ DynRenga Curve2D interface is not initialized\n";
                    debugInfo += "💡 Make sure to use Curve2D.ByLineSegment() or similar method\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ DynRenga Curve2D interface is available\n";
                debugInfo += $"📐 Curve type: {dynRengaCurve2D.Curve2DType}\n";
                debugInfo += $"📐 Is closed: {dynRengaCurve2D.IsClosed}\n";
                debugInfo += $"📏 Length: {dynRengaCurve2D._i.GetLength():F2}mm\n";
                
                // Extract the native Renga ICurve2D interface
                Renga.ICurve2D rengaCurve2D = dynRengaCurve2D._i;
                
                if (rengaCurve2D == null)
                {
                    debugInfo += "❌ Failed to extract Renga ICurve2D interface\n";
                    debugInfo += "💡 The DynRenga Curve2D may not contain a valid Renga interface\n";
                    return new Dictionary<string, object>
                    {
                        { "RengaCurve2D", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Native Renga ICurve2D interface extracted successfully\n";
                debugInfo += "💡 This interface can be used directly with RengaBIM SDK methods\n";
                debugInfo += "💡 Use this with SetBaseline() method on Baseline2DObject\n";
                
                return new Dictionary<string, object>
                {
                    { "RengaCurve2D", rengaCurve2D },
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error extracting Renga interface!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "RengaCurve2D", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание открытой кривой из закрытой PolyCurve для совместимости с базовыми линиями
        /// Некоторые объекты не поддерживают закрытые кривые в качестве базовых линий
        /// </summary>
        /// <param name="closedCurve">Закрытая кривая для преобразования</param>
        /// <returns>Открытая кривая и информация об операции</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "OpenCurve", "Success", "DebugInfo" })]
        public static Dictionary<string, object> CreateOpenCurveFromClosed(Curve2D closedCurve)
        {
            var debugInfo = "🔧 Creating open curve from closed curve for baseline compatibility...\n";
            
            try
            {
                if (closedCurve == null)
                {
                    debugInfo += "❌ Closed curve cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "OpenCurve", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (closedCurve._i == null)
                {
                    debugInfo += "❌ Closed curve interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "OpenCurve", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"📐 Original curve type: {closedCurve.Curve2DType}\n";
                debugInfo += $"📐 Is closed: {closedCurve.IsClosed}\n";
                debugInfo += $"📏 Original length: {closedCurve._i.GetLength():F2}mm\n";
                
                if (!closedCurve.IsClosed)
                {
                    debugInfo += "✅ Curve is already open - no conversion needed\n";
                    return new Dictionary<string, object>
                    {
                        { "OpenCurve", closedCurve },
                        { "Success", true },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // For closed curves, we'll create a trimmed version that removes the closing segment
                debugInfo += "🔄 Creating trimmed open curve...\n";
                
                try
                {
                    // Get the curve parameters
                    double minParam = closedCurve._i.MinParameter;
                    double maxParam = closedCurve._i.MaxParameter;
                    
                    debugInfo += $"📊 Parameter range: {minParam:F6} to {maxParam:F6}\n";
                    
                    // Trim the curve to remove the last segment (closing segment)
                    // We'll trim to 95% of the parameter range to ensure we get an open curve
                    double trimEnd = minParam + (maxParam - minParam) * 0.95;
                    
                    debugInfo += $"✂️ Trimming curve to parameter: {trimEnd:F6}\n";
                    
                    // Create trimmed curve
                    var trimmedCurve = closedCurve._i.GetTrimmed(minParam, trimEnd, 1); // 1 = positive direction
                    
                    if (trimmedCurve != null)
                    {
                        var openCurve = new Curve2D(trimmedCurve);
                        debugInfo += "✅ Open curve created successfully!\n";
                        debugInfo += $"📏 New length: {openCurve._i.GetLength():F2}mm\n";
                        debugInfo += $"📐 Is closed: {openCurve.IsClosed}\n";
                        debugInfo += "💡 This open curve should be compatible with baseline operations\n";
                        
                        return new Dictionary<string, object>
                        {
                            { "OpenCurve", openCurve },
                            { "Success", true },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    else
                    {
                        debugInfo += "❌ Failed to create trimmed curve\n";
                        debugInfo += "💡 Falling back to original curve\n";
                        
                        return new Dictionary<string, object>
                        {
                            { "OpenCurve", closedCurve },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"❌ COM Error when trimming curve: {comEx.Message}\n";
                    debugInfo += $"❌ Error Code: 0x{comEx.ErrorCode:X8}\n";
                    debugInfo += "💡 Falling back to original curve\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "OpenCurve", closedCurve },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Unexpected error when creating open curve!\n";
                debugInfo += $"❌ Error Type: {ex.GetType().Name}\n";
                debugInfo += $"❌ Error Message: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "OpenCurve", closedCurve },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Проверка возможности установки базовой линии для данного объекта
        /// </summary>
        /// <returns>Результат проверки с подробной информацией</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "CanSetBaseline", "Reason", "DebugInfo" })]
        public Dictionary<string, object> CanSetBaseline()
        {
            var debugInfo = "🔍 Checking if object can accept baseline modifications...\n";
            var canSet = false;
            var reason = "";
            
            try
            {
                // Check if interface is available
                if (this._i == null)
                {
                    reason = "Baseline2DObject interface is not initialized";
                    debugInfo += "❌ Baseline2DObject interface is not initialized\n";
                    debugInfo += "💡 This usually means the object doesn't support IBaseline2DObject\n";
                    return new Dictionary<string, object>
                    {
                        { "CanSetBaseline", false },
                        { "Reason", reason },
                        { "DebugInfo", debugInfo }
                    };
                }
                debugInfo += "✅ Baseline2DObject interface is available\n";
                
                // Try to get current baseline to test if object supports baseline operations
                try
                {
                    var currentBaseline = this.GetBaseline();
                    if (currentBaseline != null)
                    {
                        debugInfo += "✅ Object has existing baseline - baseline operations are supported\n";
                        debugInfo += $"📏 Current baseline length: {currentBaseline._i.GetLength():F2}mm\n";
                        canSet = true;
                        reason = "Object supports baseline modification";
                    }
                    else
                    {
                        debugInfo += "⚠️ Object doesn't have existing baseline\n";
                        debugInfo += "💡 This might indicate the object doesn't support baselines\n";
                        reason = "Object doesn't have existing baseline - may not support baseline operations";
                    }
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"❌ Failed to get current baseline: {comEx.Message}\n";
                    debugInfo += $"❌ COM Error Code: 0x{comEx.ErrorCode:X8}\n";
                    
                    switch (comEx.ErrorCode)
                    {
                        case unchecked((int)0x80010105): // RPC_E_SERVERFAULT
                            reason = "RPC_E_SERVERFAULT when accessing baseline - object may not support baseline operations";
                            debugInfo += "💡 RPC_E_SERVERFAULT suggests object doesn't support baseline operations\n";
                            break;
                        case unchecked((int)0x80004005): // E_FAIL
                            reason = "E_FAIL when accessing baseline - object may not support baseline operations";
                            debugInfo += "💡 E_FAIL suggests object doesn't support baseline operations\n";
                            break;
                        case unchecked((int)0x80004001): // E_NOTIMPL
                            reason = "E_NOTIMPL - baseline operations not implemented for this object type";
                            debugInfo += "💡 E_NOTIMPL - this object type doesn't support baseline operations\n";
                            break;
                        default:
                            reason = $"COM error when accessing baseline (0x{comEx.ErrorCode:X8}) - object may not support baseline operations";
                            debugInfo += "💡 Unknown COM error suggests object may not support baseline operations\n";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    reason = $"Unexpected error when accessing baseline: {ex.Message}";
                    debugInfo += $"❌ Unexpected error: {ex.Message}\n";
                    debugInfo += "💡 This suggests the object may not support baseline operations\n";
                }
                
                // Additional checks for object state
                if (canSet)
                {
                    debugInfo += "✅ Object appears to support baseline modification\n";
                    debugInfo += "💡 You can try to set a baseline using SetBaseline() or SetBaselineWithDebug()\n";
                }
                else
                {
                    debugInfo += "❌ Object does not appear to support baseline modification\n";
                    debugInfo += "💡 Consider using a different object type (e.g., structural elements)\n";
                    debugInfo += "💡 Or check if the object has dependent objects that prevent modification\n";
                }
                
                return new Dictionary<string, object>
                {
                    { "CanSetBaseline", canSet },
                    { "Reason", reason },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                reason = $"Error during baseline capability check: {ex.Message}";
                debugInfo += $"❌ Error during capability check: {ex.Message}\n";
                debugInfo += $"❌ Exception type: {ex.GetType().Name}\n";
                
                return new Dictionary<string, object>
                {
                    { "CanSetBaseline", false },
                    { "Reason", reason },
                    { "DebugInfo", debugInfo }
                };
            }
        }
    }
}
