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
                return result;
            }
            catch (Exception ex)
            {
                var error = $"Debug info failed: {ex.Message}";
                return error;
            }
        }
        
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
                {
                    throw new InvalidOperationException("Model interface is not initialized.");
                }
                
                var objects = this._i.GetObjects();
                if (objects == null)
                {
                    return new List<int>();
                }
                
                var levels = new List<int>();
                for (int i = 0; i < objects.Count; i++)
                {
                    var obj = objects.GetByIndex(i);
                    if (obj != null && obj.ObjectType == Renga.ObjectTypes.Level)
                    {
                        levels.Add(obj.Id);
                    }
                }
                
                return levels;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"GetAvailableLevels failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Получение внутреннего челочисленного идентификатора объекта из его Guid-идентификатора
        /// </summary>
        /// <param name="internal_model_guid"></param>
        /// <returns></returns>
        public int GetIdFromUniqueId(Guid internal_model_guid)
        {
            return this._i.GetIdFromUniqueId(internal_model_guid);
        }
        /// <summary>
        /// Получение внутреннего Guid-идентификатора объекта из его целочисленного идентификатора
        /// </summary>
        /// <param name="internal_model_id"></param>
        /// <returns></returns>
        public Guid GetUniqueIdFromId(int internal_model_id)
        {
            return this._i.GetUniqueIdFromId(internal_model_id);
        }
        
        /// <summary>
        /// Получение внутреннего целочисленного идентификатора объекта из его Guid-идентификатора (строковый вариант)
        /// </summary>
        /// <param name="internal_model_guid_string">Guid в виде строки</param>
        /// <returns></returns>
        public int GetIdFromUniqueIdS(string internal_model_guid_string)
        {
            return this._i.GetIdFromUniqueIdS(internal_model_guid_string);
        }
        
        /// <summary>
        /// Получение внутреннего Guid-идентификатора объекта из его целочисленного идентификатора (строковый вариант)
        /// </summary>
        /// <param name="internal_model_id"></param>
        /// <returns>Guid в виде строки</returns>
        public string GetUniqueIdFromIdS(int internal_model_id)
        {
            return this._i.GetUniqueIdFromIdS(internal_model_id);
        }
        
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
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
        
        /// <summary>
        /// Создание нового объекта модели
        /// </summary>
        /// <param name="args">Аргументы для создания объекта</param>
        /// <returns>ModelObject объект</returns>
        public ModelObject CreateObject(NewEntityArgs args)
        {
            try
            {
                if (this._i == null)
                {
                    throw new InvalidOperationException("Model interface is not initialized.");
                }
                
                if (args == null)
                {
                    throw new ArgumentNullException(nameof(args), "NewEntityArgs cannot be null.");
                }
                
                if (args._i == null)
                {
                    throw new InvalidOperationException("NewEntityArgs interface is not initialized.");
                }
                
                // Debug information is now available through return value
                
                var modelObject = this._i.CreateObject(args._i);
                if (modelObject == null)
                {
                    throw new InvalidOperationException("Failed to create model object. Check that all required parameters are set correctly.");
                }
                
                return new ModelObject(modelObject);
            }
            catch (Exception ex)
            {
                var errorMessage = $"CreateObject failed: {ex.Message}";
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
        
        /// <summary>
        /// Создание объекта модели с отладочной информацией
        /// </summary>
        /// <param name="args">Аргументы для создания объекта</param>
        /// <returns>Созданный объект модели и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ModelObject", "DebugInfo" })]
        public Dictionary<string, object> CreateObjectWithDebug(NewEntityArgs args)
        {
            var debugInfo = "🔧 Creating model object...\n";
            
            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "ModelObject", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (args == null)
                {
                    debugInfo += "❌ NewEntityArgs cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "ModelObject", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (args._i == null)
                {
                    debugInfo += "❌ NewEntityArgs interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "ModelObject", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Model interface initialized\n";
                debugInfo += $"✅ NewEntityArgs interface initialized\n";
                debugInfo += $"📋 Object Type ID: {args.TypeId}\n";
                debugInfo += $"📋 Object Type ID (String): {args.TypeIdS}\n";
                debugInfo += $"📋 Category ID: {args.CategoryId}\n";
                debugInfo += $"📋 Host Object ID: {args.HostObjectId}\n";
                debugInfo += $"📍 Placement3D Type: {args.Placement3D?.GetType().Name ?? "null"}\n";
                
                var modelObject = this._i.CreateObject(args._i);
                if (modelObject == null)
                {
                    debugInfo += "❌ Failed to create model object - Renga returned null\n";
                    debugInfo += "💡 Check that all required parameters are set correctly\n";
                    return new Dictionary<string, object>
                    {
                        { "ModelObject", null },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                var wrappedModelObject = new ModelObject(modelObject);
                debugInfo += "✅ Model object created successfully!\n";
                debugInfo += $"🆔 Object ID: {wrappedModelObject.Id}\n";
                debugInfo += $"📏 Object Type: {wrappedModelObject.ObjectType}\n";
                
                return new Dictionary<string, object>
                {
                    { "ModelObject", wrappedModelObject },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Model object creation failed!\n";
                debugInfo += $"Model._i is null: {this._i == null}\n";
                debugInfo += $"Args is null: {args == null}\n";
                debugInfo += $"Args._i is null: {args?._i == null}\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "ModelObject", null },
                    { "DebugInfo", debugInfo }
                };
            }
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
        
        // ========== DYNAMO NODE HELPER METHODS ==========
        
        /// <summary>
        /// Создание нового объекта стены
        /// </summary>
        /// <param name="levelId">ID уровня для размещения стены</param>
        /// <param name="placement3D">3D размещение стены</param>
        /// <returns>ModelObject созданной стены</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateWall(int levelId, object placement3D)
        {
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Wall;
            args.HostObjectId = levelId;
            args.Placement3D = placement3D;
            return CreateObject(args);
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
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Column;
            args.HostObjectId = levelId;
            args.Placement3D = placement3D;
            return CreateObject(args);
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
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Window;
            args.HostObjectId = wallId;
            args.Placement3D = placement3D;
            return CreateObject(args);
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
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Door;
            args.HostObjectId = wallId;
            args.Placement3D = placement3D;
            return CreateObject(args);
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
                var floorResult = CreateFloor(levelId, placement3D);
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
                                debugInfo += $"\n🔧 Attempting to set baseline on floor ID: {floor.Id}";
                                debugInfo += $"\n📐 Curve2D type: {curve2D.GetType().Name}";
                                debugInfo += $"\n📐 Curve2D._i is null: {curve2D._i == null}";
                                debugInfo += $"\n📐 Baseline2DObject._i is null: {baselineObject._i == null}";
                                
                                try
                                {
                                    baselineObject.SetBaseline(curve2D);
                                    debugInfo += $"\n✅ Baseline set successfully to floor ID: {floor.Id}";
                                }
                                catch (System.Runtime.InteropServices.COMException comEx)
                                {
                                    debugInfo += $"\n❌ COM Error when setting baseline: {comEx.Message}";
                                    debugInfo += $"\n❌ Error Code: 0x{comEx.ErrorCode:X8}";
                                    if (comEx.ErrorCode == unchecked((int)0x80010105))
                                    {
                                        debugInfo += $"\n💡 RPC_E_SERVERFAULT - Possible causes:";
                                        debugInfo += $"\n   • Floor object doesn't support baseline modification";
                                        debugInfo += $"\n   • Floor has dependent objects (like Roof)";
                                        debugInfo += $"\n   • Baseline curve is invalid for floor objects";
                                        debugInfo += $"\n   • Floor is not in correct state for baseline modification";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    debugInfo += $"\n❌ Unexpected error when setting baseline: {ex.Message}";
                                    debugInfo += $"\n❌ Exception type: {ex.GetType().Name}";
                                }
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
        /// Создание нового объекта пола
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <returns>ModelObject созданного пола и отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "DebugInfo" })]
        public Dictionary<string, object> CreateFloor(int levelId, object placement3D)
        {
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    throw new InvalidOperationException("Model interface is not initialized. Make sure you have a valid Renga project loaded.");
                }
                
                if (placement3D == null)
                {
                    throw new ArgumentNullException(nameof(placement3D), "Placement3D cannot be null. Use Placement3D.ByOrigin() or other creation methods to create a valid placement.");
                }
                
                if (levelId <= 0)
                {
                    throw new ArgumentException("LevelId must be a positive integer. Make sure you have a valid level ID.", nameof(levelId));
                }
                
                var args = CreateNewEntityArgs();
                if (args == null || args._i == null)
                {
                    throw new InvalidOperationException("Failed to create NewEntityArgs. The Renga model may not be properly initialized.");
                }
                
                args.TypeId = Renga.ObjectTypes.Floor;
                args.HostObjectId = levelId;
                args.Placement3D = placement3D;
                
                // Start an operation before creating the object
                var operation = CreateOperation();
                if (operation == null || operation._i == null)
                {
                    throw new InvalidOperationException("Failed to create operation. Cannot create floor without an active operation.");
                }
                
                operation.Start();
                
                var result = CreateObject(args);
                if (result == null)
                {
                    operation.Rollback();
                    throw new InvalidOperationException("Failed to create floor object. Check that the level ID exists and the placement is valid.");
                }
                
                operation.Apply();
                
                var debugInfo = $"✅ Floor created successfully!\n" +
                               $"LevelId: {levelId}\n" +
                               $"Placement3D Type: {placement3D?.GetType().Name ?? "null"}\n" +
                               $"Model._i is null: {this._i == null}\n" +
                               $"Floor ID: {result.Id}";
                
                return new Dictionary<string, object>
                {
                    { "Floor", result },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                var errorMessage = $"CreateFloor operation failed: {ex.Message}";
                
                var errorDebugInfo = $"❌ Floor creation failed!\n" +
                                    $"LevelId: {levelId}\n" +
                                    $"Placement3D Type: {placement3D?.GetType().Name ?? "null"}\n" +
                                    $"Model._i is null: {this._i == null}\n" +
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
        /// Создание нового объекта уровня
        /// </summary>
        /// <param name="placement3D">3D размещение уровня</param>
        /// <returns>ModelObject созданного уровня</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateLevel(object placement3D)
        {
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Level;
            args.Placement3D = placement3D;
            return CreateObject(args);
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
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Room;
            args.HostObjectId = levelId;
            args.Placement3D = placement3D;
            return CreateObject(args);
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
            var args = CreateNewEntityArgs();
            args.TypeId = Renga.ObjectTypes.Equipment;
            args.HostObjectId = levelId;
            args.CategoryId = categoryId;
            args.Placement3D = placement3D;
            return CreateObject(args);
        }
        
        /// <summary>
        /// Создание нового объекта с настройкой всех параметров
        /// </summary>
        /// <param name="typeId">GUID типа объекта</param>
        /// <param name="levelId">ID уровня (опционально)</param>
        /// <param name="categoryId">ID категории (опционально)</param>
        /// <param name="placement3D">3D размещение объекта</param>
        /// <returns>ModelObject созданного объекта</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject CreateObjectWithArgs(Guid typeId, int levelId = -1, int categoryId = -1, object placement3D = null)
        {
            var args = CreateNewEntityArgs();
            args.TypeId = typeId;
            
            if (levelId != -1)
                args.HostObjectId = levelId;
                
            if (categoryId != -1)
                args.CategoryId = categoryId;
                
            if (placement3D != null)
                args.Placement3D = placement3D;
                
            return CreateObject(args);
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
    }
}
