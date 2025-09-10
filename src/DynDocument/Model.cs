using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynObjects;

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Класс для работы с интерфейсом Renga.IModel (модель Проекта)
    /// Рефакторенная версия с объединенными методами и улучшенной структурой
    /// </summary>
    public class Model
    {
        public Renga.IModel _i;
        /// <summary>
        /// Инициализация класса через свойство Проекта (класса Project)
        /// </summary>
        /// <param name="_i"></param>
        public Model(Project.Project renga_project)
        {
            if (renga_project == null)
            {
                throw new ArgumentNullException(nameof(renga_project), "Project cannot be null.");
            }
            
            if (renga_project._i == null)
            {
                throw new InvalidOperationException("Project interface is not initialized.");
            }
            
            this._i = renga_project._i.Model;
            
            if (this._i == null)
            {
                throw new InvalidOperationException("Failed to get Model from Project. Make sure the project is properly loaded.");
            }
        }
        /// <summary>
        /// Приведение Сборки к данному классу
        /// </summary>
        /// <param name="renga_model_object_assembly"></param>
        public Model(ModelObject renga_model_object_assembly)
        {
            if (renga_model_object_assembly._i.ObjectType == ObjectTypes.AssemblyInstance)
                this._i = renga_model_object_assembly._i as Renga.IModel;
            else this._i = null;
        }
        /// <summary>
        /// Проверка валидности модели
        /// </summary>
        /// <returns>true если модель инициализирована корректно</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }
        
        /// <summary>
        /// Отладочная информация для диагностики проблем
        /// </summary>
        /// <returns>Строка с отладочной информацией</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            try
            {
                var info = new System.Text.StringBuilder();
                info.AppendLine("=== RENGA MODEL DEBUG INFO ===");
                info.AppendLine($"Model._i is null: {this._i == null}");
                
                if (this._i != null)
                {
                    info.AppendLine($"Model ID: {this._i.Id}");
                    info.AppendLine($"Model is valid: {this._i != null}");
                    
                    try
                    {
                        var objects = this._i.GetObjects();
                        info.AppendLine($"Objects collection is null: {objects == null}");
                        if (objects != null)
                        {
                            info.AppendLine($"Objects count: {objects.Count}");
                        }
                    }
                    catch (Exception ex)
                    {
                        info.AppendLine($"Error getting objects: {ex.Message}");
                    }
                }
                
                info.AppendLine("=============================");
                var result = info.ToString();
                System.Console.WriteLine(result);
                return result;
            }
            catch (Exception ex)
            {
                var error = $"Debug info failed: {ex.Message}";
                System.Console.WriteLine($"=== RENGA DYNAMO ERROR ===");
                System.Console.WriteLine($"Method: Model.GetDebugInfo");
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Console.WriteLine($"=========================");
                return error;
            }
        }
        
        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ПОЛУЧЕНИЯ ИНФОРМАЦИИ ==========
        
        /// <summary>
        /// Получение списка доступных уровней в модели
        /// </summary>
        /// <returns>Список ID уровней</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<int> GetAvailableLevels()
        {
            try
            {
                if (this._i == null)
                    throw new InvalidOperationException("Model interface is not initialized.");
                
                var objects = this._i.GetObjects();
                if (objects == null)
                    return new List<int>();
                
                var levels = new List<int>();
                for (int i = 0; i < objects.Count; i++)
                {
                    var obj = objects.GetByIndex(i);
                    if (obj?.ObjectType == Renga.ObjectTypes.Level)
                        levels.Add(obj.Id);
                }
                
                return levels;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"GetAvailableLevels failed: {ex.Message}", ex);
            }
        }
        
        // ========== УТИЛИТАРНЫЕ МЕТОДЫ ==========
        
        /// <summary>
        /// Конвертация ID: получение внутреннего целочисленного идентификатора из Guid
        /// </summary>
        /// <param name="uniqueId">Guid идентификатор или строковое представление</param>
        /// <returns>Целочисленный ID</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ConvertUniqueIdToId(object uniqueId)
        {
            if (uniqueId is Guid guidId)
                return this._i.GetIdFromUniqueId(guidId);
            else if (uniqueId is string stringId)
                return this._i.GetIdFromUniqueIdS(stringId);
            else
                throw new ArgumentException("UniqueId должен быть типа Guid или string");
        }
        
        /// <summary>
        /// Конвертация ID: получение Guid идентификатора из целочисленного ID
        /// </summary>
        /// <param name="id">Целочисленный ID</param>
        /// <param name="asString">Вернуть как строку (по умолчанию false)</param>
        /// <returns>Guid или строковое представление</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object ConvertIdToUniqueId(int id, bool asString = false)
        {
            if (asString)
                return this._i.GetUniqueIdFromIdS(id);
            else
                return this._i.GetUniqueIdFromId(id);
        }
        
        /// <summary>
        /// (Устаревший) Получение внутреннего целочисленного идентификатора объекта из его Guid-идентификатора
        /// </summary>
        [Obsolete("Используйте ConvertUniqueIdToId")]
        public int GetIdFromUniqueId(Guid internal_model_guid) => this._i.GetIdFromUniqueId(internal_model_guid);
        
        /// <summary>
        /// (Устаревший) Получение внутреннего Guid-идентификатора объекта из его целочисленного идентификатора
        /// </summary>
        [Obsolete("Используйте ConvertIdToUniqueId")]
        public Guid GetUniqueIdFromId(int internal_model_id) => this._i.GetUniqueIdFromId(internal_model_id);
        
        /// <summary>
        /// (Устаревший) Получение внутреннего целочисленного идентификатора объекта из его Guid-идентификатора (строковый вариант)
        /// </summary>
        [Obsolete("Используйте ConvertUniqueIdToId")]
        public int GetIdFromUniqueIdS(string internal_model_guid_string) => this._i.GetIdFromUniqueIdS(internal_model_guid_string);
        
        /// <summary>
        /// (Устаревший) Получение внутреннего Guid-идентификатора объекта из его целочисленного идентификатора (строковый вариант)
        /// </summary>
        [Obsolete("Используйте ConvertIdToUniqueId")]
        public string GetUniqueIdFromIdS(int internal_model_id) => this._i.GetUniqueIdFromIdS(internal_model_id);
        
        /// <summary>
        /// Создание аргументов для создания новой сущности
        /// </summary>
        /// <returns>NewEntityArgs объект</returns>
        public NewEntityArgs CreateNewEntityArgs()
        {
            try
            {
                if (this._i == null)
                {
                    throw new InvalidOperationException("Model interface is not initialized.");
                }
                
                var newEntityArgs = this._i.CreateNewEntityArgs();
                if (newEntityArgs == null)
                {
                    throw new InvalidOperationException("Failed to create NewEntityArgs from Renga model.");
                }
                
                return new NewEntityArgs(newEntityArgs);
            }
            catch (Exception ex)
            {
                var errorMessage = $"CreateNewEntityArgs failed: {ex.Message}";
                System.Console.WriteLine($"=== RENGA DYNAMO ERROR ===");
                System.Console.WriteLine($"Method: Model.CreateNewEntityArgs");
                System.Console.WriteLine($"Model._i is null: {this._i == null}");
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Console.WriteLine($"=========================");
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
        
        // ========== БАЗОВЫЕ МЕТОДЫ СОЗДАНИЯ ОБЪЕКТОВ ==========
        
        /// <summary>
        /// Универсальный метод создания объектов модели с опциональной отладочной информацией
        /// </summary>
        /// <param name="args">Аргументы для создания объекта</param>
        /// <param name="enableDebug">Включить отладочную информацию</param>
        /// <returns>ModelObject и опциональная отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo" })]
        public Dictionary<string, object> CreateObject(NewEntityArgs args, bool enableDebug = false)
        {
            var debugInfo = enableDebug ? "🔧 Creating model object...\n" : "";
            
            try
            {
                if (this._i == null)
                {
                    var error = "❌ Model interface is not initialized";
                    if (enableDebug) debugInfo += error + "\n";
                    throw new InvalidOperationException("Model interface is not initialized.");
                }
                
                if (args == null)
                {
                    var error = "❌ NewEntityArgs cannot be null";
                    if (enableDebug) debugInfo += error + "\n";
                    throw new ArgumentNullException(nameof(args), "NewEntityArgs cannot be null.");
                }
                
                if (args._i == null)
                {
                    var error = "❌ NewEntityArgs interface is not initialized";
                    if (enableDebug) debugInfo += error + "\n";
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                }
                
                if (enableDebug)
                {
                    debugInfo += $"✅ Model interface initialized\n";
                    debugInfo += $"✅ NewEntityArgs interface initialized\n";
                    debugInfo += $"📋 Object Type ID: {args.TypeId}\n";
                    debugInfo += $"📋 Object Type ID (String): {args.TypeIdS}\n";
                    debugInfo += $"📋 Category ID: {args.CategoryId}\n";
                    debugInfo += $"📋 Host Object ID: {args.HostObjectId}\n";
                    debugInfo += $"📍 Placement3D Type: {args.Placement3D?.GetType().Name ?? "null"}\n";
                }
                
                var modelObject = this._i.CreateObject(args._i);
                if (modelObject == null)
                {
                    var error = "❌ Failed to create model object - Renga returned null";
                    if (enableDebug) debugInfo += error + "\n💡 Check that all required parameters are set correctly\n";
                    throw new InvalidOperationException("Failed to create model object. Check that all required parameters are set correctly.");
                }
                
                var wrappedModelObject = new ModelObject(modelObject);
                
                if (enableDebug)
                {
                    debugInfo += "✅ Model object created successfully!\n";
                    debugInfo += $"🆔 Object ID: {wrappedModelObject.Id}\n";
                    debugInfo += $"📏 Object Type: {wrappedModelObject.ObjectType}\n";
                }
                
                return new Dictionary<string, object>
                {
                    { "ModelObject", wrappedModelObject },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                var errorDebugInfo = enableDebug ? 
                    $"❌ Model object creation failed!\n" +
                    $"Model._i is null: {this._i == null}\n" +
                    $"Args is null: {args == null}\n" +
                    $"Args._i is null: {args?._i == null}\n" +
                    $"Error: {ex.Message}\n" +
                    $"Stack Trace: {ex.StackTrace}" :
                    "";
                
                if (enableDebug)
                {
                    return new Dictionary<string, object>
                    {
                        { "ModelObject", null },
                        { "DebugInfo", errorDebugInfo }
                    };
                }
                else
                {
                    var errorMessage = $"CreateObject failed: {ex.Message}";
                    System.Console.WriteLine($"=== RENGA DYNAMO ERROR ===");
                    System.Console.WriteLine($"Method: Model.CreateObject");
                    System.Console.WriteLine($"Error: {ex.Message}");
                    System.Console.WriteLine($"=========================");
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
        }
        
        /// <summary>
        /// Удобный метод для создания объекта с отладочной информацией (обертка)
        /// </summary>
        /// <param name="args">Аргументы для создания объекта</param>
        /// <returns>Созданный объект модели и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo" })]
        public Dictionary<string, object> CreateObjectWithDebug(NewEntityArgs args)
        {
            return CreateObject(args, enableDebug: true);
        }
        
        /// <summary>
        /// Создание операции
        /// </summary>
        /// <returns>Operation объект</returns>
        public Operation CreateOperation()
        {
            return new Operation(this._i.CreateOperation());
        }
        
        /// <summary>
        /// Получение коллекции всех объектов модели
        /// </summary>
        /// <returns>ModelObjectCollection объект</returns>
        public ModelObjectCollection GetObjects()
        {
            return new ModelObjectCollection(this._i.GetObjects());
        }
        
        /// <summary>
        /// Получение идентификатора модели
        /// </summary>
        public Guid Id => this._i.Id;
        
        // ========== СПЕЦИАЛИЗИРОВАННЫЕ МЕТОДЫ СОЗДАНИЯ ОБЪЕКТОВ ==========
        
        /// <summary>
        /// Универсальный метод создания объектов различных типов
        /// </summary>
        /// <param name="objectType">Тип объекта (Wall, Column, Door, Window, Floor, Level, Room, Equipment)</param>
        /// <param name="hostObjectId">ID объекта-хозяина (уровень для большинства объектов)</param>
        /// <param name="placement3D">3D размещение объекта</param>
        /// <param name="categoryId">ID категории (для оборудования)</param>
        /// <param name="enableDebug">Включить отладочную информацию</param>
        /// <returns>Созданный объект и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo" })]
        public Dictionary<string, object> CreateObjectByType(string objectType, int hostObjectId, object placement3D, int categoryId = -1, bool enableDebug = false)
        {
            try
            {
                var args = CreateNewEntityArgs();
                
                // Установка типа объекта
                switch (objectType.ToLower())
                {
                    case "wall":
                        args.TypeId = Renga.ObjectTypes.Wall;
                        break;
                    case "column":
                        args.TypeId = Renga.ObjectTypes.Column;
                        break;
                    case "door":
                        args.TypeId = Renga.ObjectTypes.Door;
                        break;
                    case "window":
                        args.TypeId = Renga.ObjectTypes.Window;
                        break;
                    case "floor":
                        args.TypeId = Renga.ObjectTypes.Floor;
                        break;
                    case "level":
                        args.TypeId = Renga.ObjectTypes.Level;
                        break;
                    case "room":
                        args.TypeId = Renga.ObjectTypes.Room;
                        break;
                    case "equipment":
                        args.TypeId = Renga.ObjectTypes.Equipment;
                        if (categoryId != -1) args.CategoryId = categoryId;
                        break;
                    case "beam":
                        args.TypeId = Renga.ObjectTypes.Beam;
                        break;
                    default:
                        throw new ArgumentException($"Неподдерживаемый тип объекта: {objectType}");
                }
                
                if (hostObjectId > 0)
                    args.HostObjectId = hostObjectId;
                    
                if (placement3D != null)
                    args.Placement3D = placement3D;
                
                return CreateObject(args, enableDebug);
            }
            catch (Exception ex)
            {
                var errorInfo = $"❌ Ошибка создания объекта типа {objectType}: {ex.Message}";
                
                return new Dictionary<string, object>
                {
                    { "ModelObject", null },
                    { "DebugInfo", enableDebug ? errorInfo : "" }
                };
            }
        }
        
        /// <summary>
        /// Создание нового объекта стены
        /// </summary>
        /// <param name="levelId">ID уровня для размещения стены</param>
        /// <param name="placement3D">3D размещение стены</param>
        /// <returns>ModelObject созданной стены</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateWall(int levelId, object placement3D)
        {
            var result = CreateObjectByType("wall", levelId, placement3D);
            return result["ModelObject"] as ModelObject;
        }
        
        /// <summary>
        /// Создание нового объекта колонны
        /// </summary>
        /// <param name="levelId">ID уровня для размещения колонны</param>
        /// <param name="placement3D">3D размещение колонны</param>
        /// <returns>ModelObject созданной колонны</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateColumn(int levelId, object placement3D)
        {
            var result = CreateObjectByType("column", levelId, placement3D);
            return result["ModelObject"] as ModelObject;
        }
        
        /// <summary>
        /// Создание нового объекта окна
        /// </summary>
        /// <param name="wallId">ID стены-хозяина</param>
        /// <param name="placement3D">3D размещение окна</param>
        /// <returns>ModelObject созданного окна</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateWindow(int wallId, object placement3D)
        {
            var result = CreateObjectByType("window", wallId, placement3D);
            return result["ModelObject"] as ModelObject;
        }
        
        /// <summary>
        /// Создание нового объекта двери
        /// </summary>
        /// <param name="wallId">ID стены-хозяина</param>
        /// <param name="placement3D">3D размещение двери</param>
        /// <returns>ModelObject созданной двери</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateDoor(int wallId, object placement3D)
        {
            var result = CreateObjectByType("door", wallId, placement3D);
            return result["ModelObject"] as ModelObject;
        }
        
            /// <summary>
        /// Создание нового объекта пола с базовой линией
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="baseline">2D базовая линия пола (опционально)</param>
        /// <returns>ModelObject созданного пола и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorWithBaseline(int levelId, object placement3D, object baseline = null)
        {
            try
            {
                // Create the floor first
                var floorResult = CreateFloor(levelId, placement3D, enableDebug: true);
                var floor = floorResult["Floor"] as ModelObject;
                var debugInfo = floorResult["DebugInfo"] as string;
                
                if (floor == null)
                {
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "DebugInfo", debugInfo + "\n❌ Failed to create floor - cannot set baseline" }
                    };
                }
                
                // Try to set baseline if provided
                if (baseline != null)
                {
                    try
                    {
                        var baselineObject = floor.GetBaseline2DObject();
                        if (baselineObject != null)
                        {
                            DynRenga.DynGeometry.Curve2D curve2D = null;
                            
                            // Handle different baseline types
                            if (baseline is DynRenga.DynGeometry.Curve2D directCurve2D)
                            {
                                curve2D = directCurve2D;
                                debugInfo += $"\nℹ️ Using direct Curve2D baseline";
                            }
                            else if (baseline is dg.Curve dynamoCurve)
                            {
                                // For now, provide helpful guidance instead of attempting conversion
                                debugInfo += $"\n⚠️ Dynamo {baseline.GetType().Name} detected. Direct conversion not yet implemented.";
                                debugInfo += $"\n💡 Please use Curve2D.ByLineSegment(x1, y1, x2, y2) to create a 2D baseline curve.";
                                debugInfo += $"\n📝 Example: Curve2D.ByLineSegment(0, 0, 10, 0) for a horizontal line.";
                                
                                // Try to extract start and end points for user convenience
                                try
                                {
                                    var points = dynamoCurve.PointsAtEqualChordLength(2);
                                    if (points != null && points.Length >= 2)
                                    {
                                        var start = points[0];
                                        var end = points[points.Length - 1];
                                        debugInfo += $"\n🔍 Your curve starts at ({start.X:F2}, {start.Y:F2}) and ends at ({end.X:F2}, {end.Y:F2})";
                                        debugInfo += $"\n💡 Try: Curve2D.ByLineSegment({start.X:F2}, {start.Y:F2}, {end.X:F2}, {end.Y:F2})";
                                    }
                                }
                                catch
                                {
                                    // Ignore errors in point extraction
                                }
                            }
                            else if (baseline is dg.PolyCurve dynamoPolyCurve)
                            {
                                // For PolyCurve, provide guidance
                                debugInfo += $"\n⚠️ Dynamo PolyCurve detected. Direct conversion not yet implemented.";
                                debugInfo += $"\n💡 Please use Curve2D.ByLineSegment(x1, y1, x2, y2) to create a 2D baseline curve.";
                                debugInfo += $"\n📝 For complex shapes, consider using multiple Curve2D segments or simplify to a single line.";
                                
                                // Try to extract start and end points for user convenience
                                try
                                {
                                    var points = dynamoPolyCurve.PointsAtEqualChordLength(2);
                                    if (points != null && points.Length >= 2)
                                    {
                                        var start = points[0];
                                        var end = points[points.Length - 1];
                                        debugInfo += $"\n🔍 Your PolyCurve starts at ({start.X:F2}, {start.Y:F2}) and ends at ({end.X:F2}, {end.Y:F2})";
                                        debugInfo += $"\n💡 Try: Curve2D.ByLineSegment({start.X:F2}, {start.Y:F2}, {end.X:F2}, {end.Y:F2})";
                                    }
                                }
                                catch
                                {
                                    // Ignore errors in point extraction
                                }
                            }
                            else
                            {
                                debugInfo += $"\n⚠️ Unsupported baseline type: {baseline?.GetType().Name ?? "null"}. Expected Curve2D or Dynamo Curve.";
                            }
                            
                            if (curve2D != null)
                            {
                                baselineObject.SetBaseline(curve2D);
                                debugInfo += $"\n✅ Baseline set successfully to floor ID: {floor.Id}";
                            }
                        }
                        else
                        {
                            debugInfo += $"\n⚠️ Floor ID {floor.Id} does not support baselines";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"\n⚠️ Failed to set baseline: {ex.Message}";
                    }
                }
                else
                {
                    debugInfo += $"\nℹ️ No baseline provided - floor created without baseline";
                }
                
                return new Dictionary<string, object>
                {
                    { "Floor", floor },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                var errorDebugInfo = $"❌ Floor with baseline creation failed!\n" +
                                    $"LevelId: {levelId}\n" +
                                    $"Placement3D Type: {placement3D?.GetType().Name ?? "null"}\n" +
                                    $"Baseline Type: {baseline?.GetType().Name ?? "null"}\n" +
                                    $"Error: {ex.Message}\n" +
                                    $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "DebugInfo", errorDebugInfo }
                };
            }
        }
        
        /// <summary>
        /// Создание нового объекта пола с операционным контекстом
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="enableDebug">Включить отладочную информацию</param>
        /// <returns>ModelObject созданного пола и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "DebugInfo" })]
        public Dictionary<string, object> CreateFloor(int levelId, object placement3D, bool enableDebug = false)
        {
            try
            {
                // Валидация входных данных
                if (this._i == null)
                    throw new InvalidOperationException("Model interface is not initialized. Make sure you have a valid Renga project loaded.");
                
                if (placement3D == null)
                    throw new ArgumentNullException(nameof(placement3D), "Placement3D cannot be null. Use Placement3D.ByOrigin() or other creation methods to create a valid placement.");
                
                if (levelId <= 0)
                    throw new ArgumentException("LevelId must be a positive integer. Make sure you have a valid level ID.", nameof(levelId));
                
                // Создание с операционным контекстом
                var operation = CreateOperation();
                if (operation?._i == null)
                    throw new InvalidOperationException("Failed to create operation. Cannot create floor without an active operation.");
                
                operation.Start();
                
                try
                {
                    var result = CreateObjectByType("floor", levelId, placement3D, enableDebug: enableDebug);
                    var floor = result["ModelObject"] as ModelObject;
                    var debugInfo = result["DebugInfo"] as string;
                    
                    if (floor == null)
                    {
                        operation.Rollback();
                        var errorInfo = "❌ Failed to create floor object. Check that the level ID exists and the placement is valid.";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "DebugInfo", enableDebug ? debugInfo + "\n" + errorInfo : "" }
                        };
                    }
                    
                    operation.Apply();
                    
                    if (enableDebug)
                    {
                        debugInfo += $"\n✅ Operation applied successfully";
                        debugInfo += $"\n🆔 Floor ID: {floor.Id}";
                    }
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "DebugInfo", debugInfo }
                    };
                }
                catch (Exception ex)
                {
                    operation.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"CreateFloor operation failed: {ex.Message}";
                if (enableDebug)
                {
                    System.Console.WriteLine($"=== RENGA DYNAMO ERROR ===");
                    System.Console.WriteLine($"Method: Model.CreateFloor");
                    System.Console.WriteLine($"LevelId: {levelId}");
                    System.Console.WriteLine($"Placement3D Type: {(placement3D?.GetType().Name ?? "null")}");
                    System.Console.WriteLine($"Error: {ex.Message}");
                    System.Console.WriteLine($"=========================");
                }
                
                var errorDebugInfo = enableDebug ? 
                    $"❌ Floor creation failed!\n" +
                    $"LevelId: {levelId}\n" +
                    $"Placement3D Type: {placement3D?.GetType().Name ?? "null"}\n" +
                    $"Model._i is null: {this._i == null}\n" +
                    $"Error: {ex.Message}\n" +
                    $"Stack Trace: {ex.StackTrace}" : "";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "DebugInfo", errorDebugInfo }
                };
            }
        }
        
        /// <summary>
        /// Создание нового объекта уровня
        /// </summary>
        /// <param name="placement3D">3D размещение уровня</param>
        /// <returns>ModelObject созданного уровня</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateLevel(object placement3D)
        {
            var result = CreateObjectByType("level", -1, placement3D);
            return result["ModelObject"] as ModelObject;
        }
        
        /// <summary>
        /// Создание нового объекта комнаты
        /// </summary>
        /// <param name="levelId">ID уровня для размещения комнаты</param>
        /// <param name="placement3D">3D размещение комнаты</param>
        /// <returns>ModelObject созданной комнаты</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateRoom(int levelId, object placement3D)
        {
            var result = CreateObjectByType("room", levelId, placement3D);
            return result["ModelObject"] as ModelObject;
        }
        
        /// <summary>
        /// Создание нового объекта оборудования
        /// </summary>
        /// <param name="levelId">ID уровня для размещения оборудования</param>
        /// <param name="categoryId">ID категории оборудования</param>
        /// <param name="placement3D">3D размещение оборудования</param>
        /// <returns>ModelObject созданного оборудования</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateEquipment(int levelId, int categoryId, object placement3D)
        {
            var result = CreateObjectByType("equipment", levelId, placement3D, categoryId);
            return result["ModelObject"] as ModelObject;
        }
        
        /// <summary>
        /// Создание нового объекта с настройкой всех параметров через GUID
        /// </summary>
        /// <param name="typeId">GUID типа объекта</param>
        /// <param name="levelId">ID уровня (опционально)</param>
        /// <param name="categoryId">ID категории (опционально)</param>
        /// <param name="placement3D">3D размещение объекта</param>
        /// <param name="enableDebug">Включить отладочную информацию</param>
        /// <returns>ModelObject созданного объекта и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo" })]
        public Dictionary<string, object> CreateObjectWithArgs(Guid typeId, int levelId = -1, int categoryId = -1, object placement3D = null, bool enableDebug = false)
        {
            try
            {
                var args = CreateNewEntityArgs();
                args.TypeId = typeId;
                
                if (levelId != -1)
                    args.HostObjectId = levelId;
                    
                if (categoryId != -1)
                    args.CategoryId = categoryId;
                    
                if (placement3D != null)
                    args.Placement3D = placement3D;
                
                return CreateObject(args, enableDebug);
            }
            catch (Exception ex)
            {
                var errorInfo = $"❌ Ошибка создания объекта с GUID {typeId}: {ex.Message}";
                
                return new Dictionary<string, object>
                {
                    { "ModelObject", null },
                    { "DebugInfo", enableDebug ? errorInfo : "" }
                };
            }
        }
        
        // ========== STATIC HELPER METHODS FOR DYNAMO ==========
        
        /// <summary>
        /// Получение всех доступных типов объектов для создания
        /// </summary>
        /// <returns>Словарь с типами объектов</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Wall", "Column", "Door", "Window", "Floor", "Level", "Room", "Equipment", "Beam", "Element" })]
        public static Dictionary<string, Guid> GetAvailableObjectTypes()
        {
            return new Dictionary<string, Guid>
            {
                { "Wall", Renga.ObjectTypes.Wall },
                { "Column", Renga.ObjectTypes.Column },
                { "Door", Renga.ObjectTypes.Door },
                { "Window", Renga.ObjectTypes.Window },
                { "Floor", Renga.ObjectTypes.Floor },
                { "Level", Renga.ObjectTypes.Level },
                { "Room", Renga.ObjectTypes.Room },
                { "Equipment", Renga.ObjectTypes.Equipment },
                { "Beam", Renga.ObjectTypes.Beam },
                { "Element", Renga.ObjectTypes.Element },
                { "Hatch", Renga.ObjectTypes.Hatch },
                { "Hole", Renga.ObjectTypes.Hole },
                { "IsolatedFoundation", Renga.ObjectTypes.IsolatedFoundation },
                { "Opening", Renga.ObjectTypes.Opening },
                { "Plate", Renga.ObjectTypes.Plate },
                { "Railing", Renga.ObjectTypes.Railing },
                { "Ramp", Renga.ObjectTypes.Ramp },
                { "WallFoundation", Renga.ObjectTypes.WallFoundation }
            };
        }
        
        // ========== КОММЕНТАРИИ К РЕФАКТОРИНГУ ==========
        /*
         * Данная версия Model.cs оптимизирована следующим образом:
         * 
         * 1. ОБЪЕДИНЕНЫ МЕТОДЫ:
         *    - CreateObject() и CreateObjectWithDebug() объединены в один с опциональным параметром enableDebug
         *    - Методы конвертации ID объединены в ConvertUniqueIdToId() и ConvertIdToUniqueId()
         * 
         * 2. ДОБАВЛЕН УНИВЕРСАЛЬНЫЙ МЕТОД:
         *    - CreateObjectByType() - создает любой тип объекта по строковому имени типа
         * 
         * 3. ОРГАНИЗОВАНО ПО СЕКЦИЯМ:
         *    - Конструкторы
         *    - Отладочные методы  
         *    - Базовые методы
         *    - Методы создания объектов
         *    - Специализированные методы
         *    - Утилитарные методы
         *    - Вспомогательные методы получения информации
         *    - Статические вспомогательные методы
         * 
         * 4. УСТАРЕВШИЕ МЕТОДЫ:
         *    - Оставлены для обратной совместимости с пометкой [Obsolete]
         * 
         * 5. ОПТИМИЗАЦИИ:
         *    - Улучшено логирование с эмодзи
         *    - Упрощен код в методах CreateFloor и специализированных  
         *    - Добавлены полезные статические методы
         */
    }
}
