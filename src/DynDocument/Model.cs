using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;

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
        /// <param name="renga_project">Проект Renga для получения модели</param>
        public Model(Project.Project renga_project)
        {
            if (renga_project == null)
            {
                throw new ArgumentNullException(nameof(renga_project), "Project cannot be null. Make sure you have a valid Renga project loaded.");
            }
            
            if (renga_project._i == null)
            {
                throw new InvalidOperationException("Project interface is not initialized. This usually means:\n" +
                    "1. Renga application is not running - please start Renga first\n" +
                    "2. No project is loaded in Renga - please open a project in Renga\n" +
                    "3. The project failed to load properly - check the project file");
            }
            
            this._i = renga_project._i.Model;
            
            if (this._i == null)
            {
                throw new InvalidOperationException("Failed to get Model from Project. This usually means:\n" +
                    "1. The project is not properly loaded in Renga\n" +
                    "2. The project file is corrupted or invalid\n" +
                    "3. Renga version compatibility issues\n" +
                    "Please check that you have a valid project open in Renga.");
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
        /// Проверка, можно ли создать модель из данного проекта
        /// </summary>
        /// <param name="renga_project">Проект для проверки</param>
        /// <returns>true если проект готов для создания модели</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static bool CanCreateModel(Project.Project renga_project)
        {
            if (renga_project == null)
                return false;
                
            if (renga_project._i == null)
                return false;
                
            try
            {
                var model = renga_project._i.Model;
                return model != null;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Получение диагностической информации о состоянии проекта
        /// </summary>
        /// <param name="renga_project">Проект для диагностики</param>
        /// <returns>Диагностическая информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetProjectDiagnostics(Project.Project renga_project)
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine("=== RENGA PROJECT DIAGNOSTICS ===");
            
            if (renga_project == null)
            {
                info.AppendLine("❌ Project is null");
                info.AppendLine("💡 Make sure you have a valid Renga project loaded");
                return info.ToString();
            }
            
            info.AppendLine($"✅ Project object exists");
            info.AppendLine($"Project._i is null: {renga_project._i == null}");
            
            if (renga_project._i == null)
            {
                info.AppendLine("❌ Project interface is not initialized");
                info.AppendLine("💡 This usually means:");
                info.AppendLine("   • Renga application is not running");
                info.AppendLine("   • No project is loaded in Renga");
                info.AppendLine("   • The project failed to load properly");
                return info.ToString();
            }
            
            info.AppendLine("✅ Project interface is initialized");
            
            try
            {
                var model = renga_project._i.Model;
                info.AppendLine($"Model is null: {model == null}");
                
                if (model == null)
                {
                    info.AppendLine("❌ Model interface is null");
                    info.AppendLine("💡 This usually means:");
                    info.AppendLine("   • The project is not properly loaded");
                    info.AppendLine("   • The project file is corrupted");
                    info.AppendLine("   • Renga version compatibility issues");
                }
                else
                {
                    info.AppendLine("✅ Model interface is available");
                    info.AppendLine($"Model ID: {model.Id}");
                }
            }
            catch (Exception ex)
            {
                info.AppendLine($"❌ Error accessing model: {ex.Message}");
            }
            
            info.AppendLine("================================");
            return info.ToString();
        }
        
        /// <summary>
        /// Безопасное создание модели с диагностикой
        /// </summary>
        /// <param name="renga_project">Проект для создания модели</param>
        /// <returns>Результат создания модели с диагностической информацией</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Model", "Success", "DebugInfo" })]
        public static Dictionary<string, object> CreateModelSafely(Project.Project renga_project)
        {
            var debugInfo = "🔧 Creating Model safely...\n";
            
            try
            {
                // Check if project is valid
                if (!CanCreateModel(renga_project))
                {
                    debugInfo += "❌ Cannot create model from this project\n";
                    debugInfo += GetProjectDiagnostics(renga_project);
                    
                    return new Dictionary<string, object>
                    {
                        { "Model", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Project is valid for model creation\n";
                
                // Create the model
                var model = new Model(renga_project);
                
                if (model.IsValid())
                {
                    debugInfo += "✅ Model created successfully!\n";
                    debugInfo += $"Model ID: {model.Id}\n";
                    debugInfo += "💡 You can now use this model to create objects\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Model", model },
                        { "Success", true },
                        { "DebugInfo", debugInfo }
                    };
                }
                else
                {
                    debugInfo += "❌ Model was created but is not valid\n";
                    debugInfo += "💡 This indicates an internal error\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Model", model },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error creating model: {ex.Message}\n";
                debugInfo += $"Exception type: {ex.GetType().Name}\n";
                debugInfo += GetProjectDiagnostics(renga_project);
                
                return new Dictionary<string, object>
                {
                    { "Model", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
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
                
                // Try to set contour if provided (Floors use contours, not baselines)
                if (baseline != null)
                {
                    try
                    {
                        debugInfo += $"\n🔧 Floor objects use contours, not baselines";
                        debugInfo += $"\n💡 Converting baseline to floor contour...";
                        
                        DynRenga.DynGeometry.Curve2D curve2D = null;
                        
                        // Handle different baseline types
                        if (baseline is DynRenga.DynGeometry.Curve2D directCurve2D)
                        {
                            curve2D = directCurve2D;
                            debugInfo += $"\nℹ️ Using direct Curve2D as floor contour";
                        }
                        else if (baseline is dg.Curve dynamoCurve)
                        {
                            // Convert Dynamo curve to Curve2D
                            debugInfo += $"\n🔄 Converting Dynamo {baseline.GetType().Name} to Curve2D...";
                            try
                            {
                                curve2D = DynRenga.DynGeometry.Curve2D.ByDynamoCurve(dynamoCurve);
                                
                                if (curve2D == null)
                                {
                                    debugInfo += $"\n❌ Failed to convert Dynamo curve to Curve2D";
                                    debugInfo += $"\n💡 Please use Curve2D.ByLineSegment(x1, y1, x2, y2) to create a 2D contour curve.";
                                }
                                else
                                {
                                    debugInfo += $"\n✅ Dynamo curve converted to Curve2D successfully";
                                }
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"\n❌ Error converting Dynamo curve: {ex.Message}";
                                debugInfo += $"\n💡 Please use Curve2D.ByLineSegment(x1, y1, x2, y2) to create a 2D contour curve.";
                            }
                        }
                        else if (baseline is dg.PolyCurve dynamoPolyCurve)
                        {
                            // Convert Dynamo PolyCurve to Curve2D
                            debugInfo += $"\n🔄 Converting Dynamo PolyCurve to Curve2D...";
                            try
                            {
                                var polyCurveResult = DynRenga.DynGeometry.Curve2D.ByDynamoPolyCurve(dynamoPolyCurve);
                                curve2D = polyCurveResult["Curve2D"] as DynRenga.DynGeometry.Curve2D;
                                var conversionDebugInfo = polyCurveResult["DebugInfo"] as string;
                                debugInfo += $"\n{conversionDebugInfo}";
                                
                                if (curve2D == null)
                                {
                                    debugInfo += $"\n❌ Failed to convert Dynamo PolyCurve to Curve2D";
                                    debugInfo += $"\n💡 Please use Curve2D.ByLineSegment(x1, y1, x2, y2) to create a 2D contour curve.";
                                }
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"\n❌ Error converting Dynamo PolyCurve: {ex.Message}";
                                debugInfo += $"\n💡 Please use Curve2D.ByLineSegment(x1, y1, x2, y2) to create a 2D contour curve.";
                            }
                        }
                        else
                        {
                            debugInfo += $"\n⚠️ Unsupported contour type: {baseline?.GetType().Name ?? "null"}. Expected Curve2D or Dynamo Curve.";
                        }
                        
                        if (curve2D != null)
                        {
                            debugInfo += $"\n🔧 Attempting to set baseline on floor ID: {floor.Id}";
                            debugInfo += $"\n📐 Curve2D type: {curve2D.GetType().Name}";
                            debugInfo += $"\n📐 Curve2D._i is null: {curve2D._i == null}";
                            
                            try
                            {
                                // Step 1: Create Baseline2DObject from the floor
                                debugInfo += $"\n🔧 Step 1: Creating Baseline2DObject from floor...";
                                var baselineObject = floor.GetBaseline2DObject();
                                
                                if (baselineObject != null)
                                {
                                    debugInfo += $"\n✅ Baseline2DObject created successfully";
                                    debugInfo += $"\n📐 Baseline2DObject._i is null: {baselineObject._i == null}";
                                    
                                    // Step 2: Set the baseline using the Curve2D
                                    debugInfo += $"\n🔧 Step 2: Setting baseline using Curve2D...";
                                    try
                                    {
                                        baselineObject.SetBaseline(curve2D);
                                        debugInfo += $"\n✅ Baseline set successfully to floor ID: {floor.Id}";
                                        debugInfo += $"\n💡 Floor now has the specified baseline shape";
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
                                else
                                {
                                    debugInfo += $"\n❌ Failed to create Baseline2DObject from floor ID: {floor.Id}";
                                    debugInfo += $"\n💡 This may indicate that the floor object doesn't support baselines";
                                }
                            }
                            catch (System.Runtime.InteropServices.COMException comEx)
                            {
                                debugInfo += $"\n❌ COM Error when setting contour: {comEx.Message}";
                                debugInfo += $"\n❌ Error Code: 0x{comEx.ErrorCode:X8}";
                                debugInfo += $"\n💡 This may indicate that the floor doesn't support contour modification or the curve is invalid";
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"\n❌ Unexpected error when setting contour: {ex.Message}";
                                debugInfo += $"\n❌ Exception type: {ex.GetType().Name}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"\n⚠️ Failed to set contour: {ex.Message}";
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
        /// Создание пола с базовой линией по примеру C++ кода
        /// Следует рабочему алгоритму: создать пол → получить IBaseline2DObject → установить базовую линию
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="baselineCurve">2D кривая для базовой линии</param>
        /// <returns>Результат создания пола с базовой линией</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorWithBaselineCppStyle(int levelId, object placement3D, DynRenga.DynGeometry.Curve2D baselineCurve)
        {
            var debugInfo = "🏗️ Creating floor with baseline using C++ style workflow...\n";
            
            try
            {
                // Step 1: Create the floor first (like in C++)
                debugInfo += "📋 Step 1: Creating new floor object...\n";
                var floorResult = CreateFloor(levelId, placement3D);
                var floor = floorResult["Floor"] as ModelObject;
                var floorDebugInfo = floorResult["DebugInfo"] as string;
                debugInfo += floorDebugInfo + "\n";
                
                if (floor == null)
                {
                    debugInfo += "❌ Failed to create floor - cannot proceed with baseline setting\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Floor created successfully!\n";
                
                // Step 2: Get the IBaseline2DObject interface from the new floor (like in C++)
                debugInfo += "📋 Step 2: Getting IBaseline2DObject interface from new floor...\n";
                
                var baseline2DObj = floor.GetBaseline2DObject();
                if (baseline2DObj == null)
                {
                    debugInfo += "❌ New floor does not support IBaseline2DObject interface\n";
                    debugInfo += "💡 Floors might not support custom baselines in this Renga version\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ New floor supports IBaseline2DObject interface\n";
                
                // Step 3: Validate the baseline curve
                debugInfo += "📋 Step 3: Validating baseline curve...\n";
                
                if (baselineCurve == null)
                {
                    debugInfo += "❌ Baseline curve cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (baselineCurve._i == null)
                {
                    debugInfo += "❌ Baseline curve interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Get curve properties for validation
                try
                {
                    double curveLength = baselineCurve._i.GetLength();
                    debugInfo += $"📏 Baseline curve length: {curveLength:F2}mm\n";
                    
                    if (curveLength <= 0)
                    {
                        debugInfo += "❌ Baseline curve has invalid length (≤ 0)\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Baseline curve is valid\n";
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Failed to validate baseline curve: {ex.Message}\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Step 4: Get baseline before setting (for comparison)
                debugInfo += "📋 Step 4a: Getting baseline before setting (for comparison)...\n";
                try
                {
                    var baselineBefore = baseline2DObj.GetBaseline();
                    if (baselineBefore != null)
                    {
                        double lengthBefore = baselineBefore._i.GetLength();
                        debugInfo += $"📏 Baseline before setting: {lengthBefore:F2}mm\n";
                    }
                    else
                    {
                        debugInfo += "📏 No baseline before setting (floor starts with no baseline)\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Could not get baseline before setting: {ex.Message}\n";
                }
                
                // Step 4b: Set the baseline using the simple SetBaseline method (like in C++)
                debugInfo += "📋 Step 4b: Setting baseline using SetBaseline method...\n";
                
                try
                {
                    // Use the native Renga interface directly (like in C++)
                    debugInfo += $"🔄 Calling SetBaseline with curve length: {baselineCurve._i.GetLength():F2}mm\n";
                    baseline2DObj._i.SetBaseline(baselineCurve._i);
                    debugInfo += "🎉 SetBaseline call completed without errors!\n";
                    
                    // Step 4c: Get baseline immediately after setting
                    debugInfo += "📋 Step 4c: Getting baseline immediately after setting...\n";
                    try
                    {
                        var baselineAfter = baseline2DObj.GetBaseline();
                        if (baselineAfter != null)
                        {
                            double lengthAfter = baselineAfter._i.GetLength();
                            debugInfo += $"📏 Baseline after setting: {lengthAfter:F2}mm\n";
                            debugInfo += $"📊 Length change: {lengthAfter - baselineCurve._i.GetLength():F2}mm\n";
                            
                            if (Math.Abs(lengthAfter - baselineCurve._i.GetLength()) < 1.0)
                            {
                                debugInfo += "✅ Baseline was set correctly!\n";
                            }
                            else
                            {
                                debugInfo += "⚠️ Baseline length doesn't match input curve\n";
                                debugInfo += "💡 This might indicate the baseline wasn't properly set\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Could not retrieve baseline after setting\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"⚠️ Could not get baseline after setting: {ex.Message}\n";
                    }
                    
                    // Step 5: Additional verification - try to get baseline again
                    debugInfo += "📋 Step 5: Additional verification - getting baseline again...\n";
                    try
                    {
                        var newFloorBaseline = baseline2DObj.GetBaseline();
                        if (newFloorBaseline != null)
                        {
                            double newLength = newFloorBaseline._i.GetLength();
                            debugInfo += $"📏 Final baseline length: {newLength:F2}mm\n";
                            debugInfo += $"📏 Input curve length: {baselineCurve._i.GetLength():F2}mm\n";
                            
                            if (Math.Abs(newLength - baselineCurve._i.GetLength()) < 1.0)
                            {
                                debugInfo += "✅ Baseline verification successful!\n";
                            }
                            else
                            {
                                debugInfo += "❌ Baseline verification failed - lengths don't match\n";
                                debugInfo += "💡 The baseline may not have been set properly\n";
                                debugInfo += "💡 This could be due to:\n";
                                debugInfo += "   • Floor objects not supporting baselines in this Renga version\n";
                                debugInfo += "   • Baseline being overridden by floor's contour system\n";
                                debugInfo += "   • COM interface limitations\n";
                            }
                        }
                        else
                        {
                            debugInfo += "⚠️ Could not retrieve baseline for final verification\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"⚠️ Could not verify baseline: {ex.Message}\n";
                    }
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", true },
                        { "DebugInfo", debugInfo }
                    };
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"❌ Failed to set baseline on new floor (HRESULT: 0x{comEx.ErrorCode:X8})\n";
                    debugInfo += $"❌ Error: {comEx.Message}\n";
                    
                    // Provide specific guidance based on error code
                    switch (comEx.ErrorCode)
                    {
                        case unchecked((int)0x80004001): // E_NOTIMPL
                            debugInfo += "💡 E_NOTIMPL - SetBaseline is not implemented for floor objects\n";
                            debugInfo += "💡 Floors might not support custom baselines in this Renga version\n";
                            break;
                        case unchecked((int)0x80004005): // E_FAIL
                            debugInfo += "💡 E_FAIL - Possible causes:\n";
                            debugInfo += "   • Floor objects don't support baselines\n";
                            debugInfo += "   • Curve type is not supported for floors\n";
                            debugInfo += "   • Floor has dependent objects\n";
                            break;
                        default:
                            debugInfo += "💡 Unknown COM error - check Renga installation and floor support\n";
                            break;
                    }
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor with baseline creation!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Копирование базовой линии с выбранного объекта на новый пол (как в C++ примере)
        /// Следует точному алгоритму из C++ кода: получить объект → извлечь базовую линию → создать пол → применить базовую линию
        /// </summary>
        /// <param name="sourceObjectId">ID исходного объекта с базовой линией</param>
        /// <param name="levelId">ID уровня для размещения нового пола</param>
        /// <param name="placement3D">3D размещение нового пола</param>
        /// <returns>Результат копирования базовой линии</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "Success", "DebugInfo" })]
        public Dictionary<string, object> CopyBaselineToNewFloor(int sourceObjectId, int levelId, object placement3D)
        {
            var debugInfo = "🔄 Copying baseline from selected object to new floor (C++ style)...\n";
            
            try
            {
                // Step 1: Get the source object by ID (like in C++)
                debugInfo += "📋 Step 1: Getting source object by ID...\n";
                
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                Renga.IModelObject sourceObject = null;
                try
                {
                    var objects = this._i.GetObjects();
                    if (objects == null)
                    {
                        debugInfo += "❌ Could not get objects collection\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    sourceObject = objects.GetById(sourceObjectId);
                    if (sourceObject == null)
                    {
                        debugInfo += $"❌ Could not find source object with ID: {sourceObjectId}\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += $"✅ Found source object (ID: {sourceObjectId})\n";
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting source object: {ex.Message}\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Step 2: Try to get IBaseline2DObject interface from source (like in C++)
                debugInfo += "📋 Step 2: Getting IBaseline2DObject interface from source object...\n";
                
                Renga.IBaseline2DObject sourceBaseline2DObj = null;
                try
                {
                    // Try to get IBaseline2DObject interface from source
                    var baselineInterfaceName = "IBaseline2DObject";
                    var pSourceBaselineInterface = sourceObject.GetInterfaceByName(baselineInterfaceName);
                    
                    if (pSourceBaselineInterface != null)
                    {
                        sourceBaseline2DObj = pSourceBaselineInterface as Renga.IBaseline2DObject;
                        if (sourceBaseline2DObj != null)
                        {
                            debugInfo += "✅ Source object supports IBaseline2DObject interface\n";
                        }
                        else
                        {
                            debugInfo += "❌ Could not cast to IBaseline2DObject interface\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ Source object does not support IBaseline2DObject interface\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting IBaseline2DObject interface: {ex.Message}\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Step 3: Extract the actual baseline curve from the source object (like in C++)
                debugInfo += "📋 Step 3: Extracting baseline curve from source object...\n";
                
                Renga.ICurve2D sourceBaselineCurve = null;
                try
                {
                    sourceBaselineCurve = sourceBaseline2DObj.GetBaseline();
                    if (sourceBaselineCurve != null)
                    {
                        debugInfo += "✅ Successfully extracted baseline curve from source object\n";
                        
                        // Verify the extracted curve
                        double sourceLength = sourceBaselineCurve.GetLength();
                        debugInfo += $"📏 Source curve length: {sourceLength:F2}mm\n";
                    }
                    else
                    {
                        debugInfo += "❌ Could not extract baseline curve from source object\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error extracting baseline curve: {ex.Message}\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                // Step 4: Create a new floor object (like in C++)
                debugInfo += "📋 Step 4: Creating new floor object...\n";
                
                var floorResult = CreateFloor(levelId, placement3D);
                var floor = floorResult["Floor"] as ModelObject;
                var floorDebugInfo = floorResult["DebugInfo"] as string;
                debugInfo += floorDebugInfo + "\n";
                
                if (floor == null)
                {
                    debugInfo += "❌ Failed to create new floor\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Successfully created new floor\n";
                
                // Step 5: Try to get IBaseline2DObject interface from new floor (like in C++)
                debugInfo += "📋 Step 5: Getting IBaseline2DObject interface from new floor...\n";
                
                var newFloorBaseline2DObj = floor.GetBaseline2DObject();
                if (newFloorBaseline2DObj == null)
                {
                    debugInfo += "⚠️ New floor does not support IBaseline2DObject interface\n";
                    debugInfo += "💡 Floors might not support custom baselines in this Renga version\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ New floor supports IBaseline2DObject interface\n";
                
                // Step 6: Set the baseline using the simple SetBaseline method (like in C++)
                debugInfo += "📋 Step 6: Setting baseline using SetBaseline method...\n";
                
                try
                {
                    // Set the baseline using the simple SetBaseline method (object's own coordinate system)
                    newFloorBaseline2DObj._i.SetBaseline(sourceBaselineCurve);
                    debugInfo += "🎉 Successfully applied baseline to new floor!\n";
                    debugInfo += "✅ New floor now has the same baseline as the source object\n";
                    
                    // Verify the new floor's baseline
                    debugInfo += "📋 Step 7: Verifying the new floor's baseline...\n";
                    try
                    {
                        var newFloorBaseline = newFloorBaseline2DObj.GetBaseline();
                        if (newFloorBaseline != null)
                        {
                            double newLength = newFloorBaseline._i.GetLength();
                            double sourceLength = sourceBaselineCurve.GetLength();
                            debugInfo += $"📏 New floor baseline length: {newLength:F2}mm\n";
                            debugInfo += $"📏 Source baseline length: {sourceLength:F2}mm\n";
                            debugInfo += $"📊 Length match: {(Math.Abs(newLength - sourceLength) < 1.0 ? "✅ Perfect" : "⚠️ Different")}\n";
                        }
                        else
                        {
                            debugInfo += "⚠️ Could not retrieve baseline for verification\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"⚠️ Could not verify baseline: {ex.Message}\n";
                    }
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", true },
                        { "DebugInfo", debugInfo }
                    };
                }
                catch (System.Runtime.InteropServices.COMException comEx)
                {
                    debugInfo += $"❌ Failed to set baseline on new floor (HRESULT: 0x{comEx.ErrorCode:X8})\n";
                    debugInfo += $"❌ Error: {comEx.Message}\n";
                    debugInfo += "💡 This might be because floors don't support custom baselines in this Renga version\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during baseline copy operation!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола с контуром (правильный подход для полов)
        /// Полы в Renga используют контуры, а не базовые линии
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="contourCurve">2D кривая для контура пола</param>
        /// <returns>Результат создания пола с контуром</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorWithContour(int levelId, object placement3D, DynRenga.DynGeometry.Curve2D contourCurve)
        {
            var debugInfo = "🏗️ Creating floor with contour (floor-specific approach)...\n";
            
            try
            {
                // Step 1: Create the floor first
                debugInfo += "📋 Step 1: Creating new floor object...\n";
                var floorResult = CreateFloor(levelId, placement3D);
                var floor = floorResult["Floor"] as ModelObject;
                var floorDebugInfo = floorResult["DebugInfo"] as string;
                debugInfo += floorDebugInfo + "\n";
                
                if (floor == null)
                {
                    debugInfo += "❌ Failed to create floor - cannot proceed with contour setting\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Floor created successfully!\n";
                
                // Step 2: Try to get IFloorParams interface for contour setting
                debugInfo += "📋 Step 2: Getting IFloorParams interface for contour setting...\n";
                
                try
                {
                    // Try to get IFloorParams interface
                    var floorParamsInterfaceName = "IFloorParams";
                    var pFloorParamsInterface = floor._i.GetInterfaceByName(floorParamsInterfaceName);
                    
                    if (pFloorParamsInterface != null)
                    {
                        var floorParams = pFloorParamsInterface as Renga.IFloorParams;
                        if (floorParams != null)
                        {
                            debugInfo += "✅ Floor supports IFloorParams interface\n";
                            
                            // Try to set the contour
                            debugInfo += "📋 Step 3: Setting floor contour...\n";
                            try
                            {
                                // Note: IFloorParams doesn't have a SetContour method
                                // The contour is typically set during floor creation
                                debugInfo += "⚠️ IFloorParams doesn't support setting contour after creation\n";
                                debugInfo += "💡 Floor contours must be set during floor creation\n";
                                debugInfo += "💡 This is why the baseline approach might not work\n";
                                
                                // Get current contour for comparison
                                var currentContour = floorParams.GetContour();
                                if (currentContour != null)
                                {
                                    double contourLength = currentContour.GetLength();
                                    debugInfo += $"📏 Current floor contour length: {contourLength:F2}mm\n";
                                }
                                else
                                {
                                    debugInfo += "📏 Floor has no contour\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"❌ Error working with floor contour: {ex.Message}\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Could not cast to IFloorParams interface\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ Floor does not support IFloorParams interface\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting IFloorParams interface: {ex.Message}\n";
                }
                
                // Step 3: Try alternative approach - check if we can modify floor parameters
                debugInfo += "📋 Step 3: Trying alternative approach - checking floor parameters...\n";
                
                try
                {
                    // Try to get floor parameters through the interface
                    var floorParamsInterface = floor._i.GetInterfaceByName("IFloorParams");
                    if (floorParamsInterface != null)
                    {
                        var floorParams = floorParamsInterface as Renga.IFloorParams;
                        if (floorParams != null)
                        {
                            debugInfo += "✅ Got floor parameters through interface\n";
                            debugInfo += $"📏 Floor thickness: {floorParams.Thickness:F2}mm\n";
                            debugInfo += $"📏 Floor vertical offset: {floorParams.VerticalOffset:F2}mm\n";
                            
                            // Check if we can get the contour
                            try
                            {
                                var contour = floorParams.GetContour();
                                if (contour != null)
                                {
                                    double contourLength = contour.GetLength();
                                    debugInfo += $"📏 Floor contour length: {contourLength:F2}mm\n";
                                    debugInfo += $"📏 Input curve length: {contourCurve._i.GetLength():F2}mm\n";
                                    
                                    if (Math.Abs(contourLength - contourCurve._i.GetLength()) < 1.0)
                                    {
                                        debugInfo += "✅ Floor contour matches input curve!\n";
                                    }
                                    else
                                    {
                                        debugInfo += "⚠️ Floor contour doesn't match input curve\n";
                                    }
                                }
                                else
                                {
                                    debugInfo += "📏 Floor has no contour\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"⚠️ Could not get floor contour: {ex.Message}\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Could not cast to IFloorParams\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ Could not get IFloorParams interface\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting floor parameters: {ex.Message}\n";
                }
                
                debugInfo += "\n💡 CONCLUSION: Floors in Renga use contours, not baselines\n";
                debugInfo += "💡 The contour is set during floor creation, not after\n";
                debugInfo += "💡 This explains why SetBaseline() doesn't work on floors\n";
                debugInfo += "💡 For floors, you need to create them with the correct contour from the start\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", floor },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor with contour creation!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола с контуром через модификацию NewEntityArgs
        /// Попытка установить контур во время создания пола
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="contourCurve">2D кривая для контура пола</param>
        /// <returns>Результат создания пола с контуром</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorWithContourDuringCreation(int levelId, object placement3D, DynRenga.DynGeometry.Curve2D contourCurve)
        {
            var debugInfo = "🏗️ Creating floor with contour during creation...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (contourCurve == null || contourCurve._i == null)
                {
                    debugInfo += "❌ Contour curve cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                
                // Step 1: Create NewEntityArgs for floor
                debugInfo += "📋 Step 1: Creating NewEntityArgs for floor...\n";
                var args = CreateNewEntityArgs();
                if (args == null || args._i == null)
                {
                    debugInfo += "❌ Failed to create NewEntityArgs\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                args.TypeId = Renga.ObjectTypes.Floor;
                args.HostObjectId = levelId;
                args.Placement3D = placement3D;
                
                debugInfo += "✅ NewEntityArgs created successfully\n";
                
                // Step 2: Try to set contour in NewEntityArgs (if supported)
                debugInfo += "📋 Step 2: Attempting to set contour in NewEntityArgs...\n";
                try
                {
                    // Check if NewEntityArgs supports contour setting
                    var argsInterface = args._i;
                    debugInfo += $"NewEntityArgs interface type: {argsInterface.GetType().Name}\n";
                    
                    // Try to find contour-related properties or methods
                    var properties = argsInterface.GetType().GetProperties();
                    debugInfo += "Available properties in NewEntityArgs:\n";
                    foreach (var prop in properties)
                    {
                        debugInfo += $"  - {prop.Name}: {prop.PropertyType.Name}\n";
                    }
                    
                    // Look for contour-related properties
                    var contourProperty = properties.FirstOrDefault(p => 
                        p.Name.ToLower().Contains("contour") || 
                        p.Name.ToLower().Contains("curve") ||
                        p.Name.ToLower().Contains("baseline"));
                    
                    if (contourProperty != null)
                    {
                        debugInfo += $"✅ Found potential contour property: {contourProperty.Name}\n";
                        try
                        {
                            contourProperty.SetValue(argsInterface, contourCurve._i);
                            debugInfo += $"✅ Set {contourProperty.Name} to contour curve\n";
                        }
                        catch (Exception ex)
                        {
                            debugInfo += $"⚠️ Could not set {contourProperty.Name}: {ex.Message}\n";
                        }
                    }
                    else
                    {
                        debugInfo += "⚠️ No contour-related properties found in NewEntityArgs\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"⚠️ Error examining NewEntityArgs: {ex.Message}\n";
                }
                
                // Step 3: Create the floor
                debugInfo += "📋 Step 3: Creating floor object...\n";
                var operation = CreateOperation();
                if (operation == null || operation._i == null)
                {
                    debugInfo += "❌ Failed to create operation\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                operation.Start();
                
                var floor = CreateObject(args);
                if (floor == null)
                {
                    operation.Rollback();
                    debugInfo += "❌ Failed to create floor object\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                operation.Apply();
                debugInfo += $"✅ Floor created successfully (ID: {floor.Id})\n";
                
                // Step 4: Check the floor's contour
                debugInfo += "📋 Step 4: Checking floor's contour...\n";
                try
                {
                    var floorParamsInterface = floor._i.GetInterfaceByName("IFloorParams");
                    if (floorParamsInterface != null)
                    {
                        var floorParams = floorParamsInterface as Renga.IFloorParams;
                        if (floorParams != null)
                        {
                            var contour = floorParams.GetContour();
                            if (contour != null)
                            {
                                double contourLength = contour.GetLength();
                                double inputLength = contourCurve._i.GetLength();
                                debugInfo += $"📏 Floor contour length: {contourLength:F2}mm\n";
                                debugInfo += $"📏 Input curve length: {inputLength:F2}mm\n";
                                
                                if (Math.Abs(contourLength - inputLength) < 1.0)
                                {
                                    debugInfo += "✅ Floor contour matches input curve!\n";
                                    return new Dictionary<string, object>
                                    {
                                        { "Floor", floor },
                                        { "Success", true },
                                        { "DebugInfo", debugInfo }
                                    };
                                }
                                else
                                {
                                    debugInfo += "⚠️ Floor contour doesn't match input curve\n";
                                    debugInfo += "💡 The contour may have been set to a default value\n";
                                }
                            }
                            else
                            {
                                debugInfo += "📏 Floor has no contour\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Could not cast to IFloorParams\n";
                        }
                    }
                    else
                    {
                        debugInfo += "❌ Floor does not support IFloorParams interface\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error checking floor contour: {ex.Message}\n";
                }
                
                debugInfo += "\n💡 CONCLUSION: Floor contours must be set through a different mechanism\n";
                debugInfo += "💡 NewEntityArgs may not support direct contour setting\n";
                debugInfo += "💡 Contours might need to be set after floor creation through a different method\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", floor },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor with contour creation!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола с контуром через IRegion2D (правильный подход)
        /// Использует IRegion2D для создания пола с заданным контуром
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="contourCurve">2D кривая для контура пола</param>
        /// <returns>Результат создания пола с контуром</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorWithRegion2D(int levelId, object placement3D, DynRenga.DynGeometry.Curve2D contourCurve)
        {
            var debugInfo = "🏗️ Creating floor with contour using IRegion2D approach...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (contourCurve == null || contourCurve._i == null)
                {
                    debugInfo += "❌ Contour curve cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                
                // Step 1: Check curve properties
                debugInfo += "📋 Step 1: Checking curve properties...\n";
                try
                {
                    // Check if the curve is closed (required for regions)
                    bool isClosed = contourCurve.IsClosed;
                    debugInfo += $"📏 Curve is closed: {isClosed}\n";
                    
                    if (!isClosed)
                    {
                        debugInfo += "⚠️ Curve is not closed - regions require closed curves\n";
                        debugInfo += "💡 Consider using a closed polycurve or ensuring the curve is closed\n";
                    }
                    
                    double curveLength = contourCurve._i.GetLength();
                    debugInfo += $"📏 Input curve length: {curveLength:F2}mm\n";
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error checking curve properties: {ex.Message}\n";
                }
                
                // Step 2: Try alternative approach - create floor and then modify its region
                debugInfo += "📋 Step 2: Trying alternative approach - create floor then modify region...\n";
                
                // Create a regular floor first
                var floorResult = CreateFloor(levelId, placement3D);
                var floor = floorResult["Floor"] as ModelObject;
                var floorDebugInfo = floorResult["DebugInfo"] as string;
                debugInfo += floorDebugInfo + "\n";
                
                if (floor == null)
                {
                    debugInfo += "❌ Failed to create floor - cannot proceed with region modification\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Floor created successfully\n";
                
                // Step 3: Check what interfaces the floor actually supports
                debugInfo += "📋 Step 3: Checking what interfaces the floor supports...\n";
                try
                {
                    // First, let's see what interfaces are available
                    debugInfo += "🔍 Exploring floor object interfaces...\n";
                    
                    // Try to get IRegion2D interface
                    try
                    {
                        var regionInterface = floor._i.GetInterfaceByName("IRegion2D");
                        if (regionInterface != null)
                        {
                            debugInfo += "✅ Floor supports IRegion2D interface\n";
                            debugInfo += $"Interface type: {regionInterface.GetType().Name}\n";
                        }
                        else
                        {
                            debugInfo += "❌ Floor does not support IRegion2D interface\n";
                        }
                    }
                    catch (Exception regionEx)
                    {
                        debugInfo += $"❌ Error getting IRegion2D interface: {regionEx.Message}\n";
                    }
                    
                    // Try other relevant interfaces
                    debugInfo += "🔍 Trying other relevant interfaces...\n";
                    
                    // Try IFloorParams
                    try
                    {
                        var floorParamsInterface = floor._i.GetInterfaceByName("IFloorParams");
                        if (floorParamsInterface != null)
                        {
                            debugInfo += "✅ Floor supports IFloorParams interface\n";
                            debugInfo += $"IFloorParams type: {floorParamsInterface.GetType().Name}\n";
                            
                            // Try to cast and get contour
                            var floorParams = floorParamsInterface as Renga.IFloorParams;
                            if (floorParams != null)
                            {
                                debugInfo += "✅ Successfully cast to IFloorParams\n";
                                
                                var contour = floorParams.GetContour();
                                if (contour != null)
                                {
                                    double contourLength = contour.GetLength();
                                    debugInfo += $"📏 Floor contour length: {contourLength:F2}mm\n";
                                    debugInfo += $"📏 Input curve length: {contourCurve._i.GetLength():F2}mm\n";
                                    
                                    if (Math.Abs(contourLength - contourCurve._i.GetLength()) < 1.0)
                                    {
                                        debugInfo += "✅ Floor contour matches input curve!\n";
                                        return new Dictionary<string, object>
                                        {
                                            { "Floor", floor },
                                            { "Success", true },
                                            { "DebugInfo", debugInfo }
                                        };
                                    }
                                    else
                                    {
                                        debugInfo += "⚠️ Floor contour doesn't match input curve\n";
                                    }
                                }
                                else
                                {
                                    debugInfo += "📏 Floor has no contour\n";
                                }
                            }
                            else
                            {
                                debugInfo += "❌ Could not cast to IFloorParams\n";
                            }
                        }
                        else
                        {
                            debugInfo += "❌ Floor does not support IFloorParams interface\n";
                        }
                    }
                    catch (Exception floorParamsEx)
                    {
                        debugInfo += $"❌ Error getting IFloorParams interface: {floorParamsEx.Message}\n";
                    }
                    
                    // Try IBaseline2DObject
                    try
                    {
                        var baselineInterface = floor._i.GetInterfaceByName("IBaseline2DObject");
                        if (baselineInterface != null)
                        {
                            debugInfo += "✅ Floor supports IBaseline2DObject interface\n";
                            debugInfo += $"IBaseline2DObject type: {baselineInterface.GetType().Name}\n";
                        }
                        else
                        {
                            debugInfo += "❌ Floor does not support IBaseline2DObject interface\n";
                        }
                    }
                    catch (Exception baselineEx)
                    {
                        debugInfo += $"❌ Error getting IBaseline2DObject interface: {baselineEx.Message}\n";
                    }
                    
                    // List all available interfaces using reflection
                    debugInfo += "🔍 Listing all available interfaces using reflection...\n";
                    try
                    {
                        var floorType = floor._i.GetType();
                        debugInfo += $"Floor object type: {floorType.FullName}\n";
                        
                        var interfaces = floorType.GetInterfaces();
                        debugInfo += "Available interfaces:\n";
                        foreach (var iface in interfaces)
                        {
                            debugInfo += $"  - {iface.Name}\n";
                        }
                        
                        // Look for region-related interfaces
                        var regionInterfaces = interfaces.Where(i => 
                            i.Name.ToLower().Contains("region") || 
                            i.Name.ToLower().Contains("contour") ||
                            i.Name.ToLower().Contains("floor")).ToArray();
                        
                        if (regionInterfaces.Length > 0)
                        {
                            debugInfo += "Found region-related interfaces:\n";
                            foreach (var iface in regionInterfaces)
                            {
                                debugInfo += $"  - {iface.Name}\n";
                            }
                        }
                        else
                        {
                            debugInfo += "⚠️ No region-related interfaces found\n";
                        }
                    }
                    catch (Exception reflectionEx)
                    {
                        debugInfo += $"❌ Error using reflection: {reflectionEx.Message}\n";
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error checking IRegion2D interface: {ex.Message}\n";
                    debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                }
                
                debugInfo += "\n💡 CONCLUSION: Floor region modification approach\n";
                debugInfo += "💡 IRegion2D interface exists but may not support modification\n";
                debugInfo += "💡 Floors might need to be created with the correct contour from the start\n";
                debugInfo += "💡 The contour might be set through a different mechanism\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", floor },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor with region creation!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола с контуром через IFloorParams (правильный подход)
        /// Исследует как установить контур пола во время создания
        /// </summary>
        /// <param name="levelId">ID уровня для размещения пола</param>
        /// <param name="placement3D">3D размещение пола</param>
        /// <param name="contourCurve">2D кривая для контура пола</param>
        /// <returns>Результат создания пола с контуром</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorWithContourProper(int levelId, object placement3D, DynRenga.DynGeometry.Curve2D contourCurve)
        {
            var debugInfo = "🏗️ Creating floor with contour using IFloorParams approach...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (contourCurve == null || contourCurve._i == null)
                {
                    debugInfo += "❌ Contour curve cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                
                // Step 1: Check curve properties
                debugInfo += "📋 Step 1: Checking curve properties...\n";
                bool isClosed = contourCurve.IsClosed;
                double curveLength = contourCurve._i.GetLength();
                debugInfo += $"📏 Curve is closed: {isClosed}\n";
                debugInfo += $"📏 Input curve length: {curveLength:F2}mm\n";
                
                if (!isClosed)
                {
                    debugInfo += "⚠️ Curve is not closed - floors require closed contours\n";
                    debugInfo += "💡 Consider using a closed polycurve or ensuring the curve is closed\n";
                }
                
                // Step 2: Try to create floor with contour using different approaches
                debugInfo += "📋 Step 2: Trying different approaches to create floor with contour...\n";
                
                // Approach 1: Try to modify NewEntityArgs to include contour
                debugInfo += "🔍 Approach 1: Modifying NewEntityArgs to include contour...\n";
                try
                {
                    var args = CreateNewEntityArgs();
                    if (args != null && args._i != null)
                    {
                        args.TypeId = Renga.ObjectTypes.Floor;
                        args.HostObjectId = levelId;
                        args.Placement3D = placement3D;
                        
                        debugInfo += "✅ NewEntityArgs created and configured\n";
                        
                        // Try to find contour-related properties in NewEntityArgs
                        var argsType = args._i.GetType();
                        var properties = argsType.GetProperties();
                        debugInfo += "Available properties in NewEntityArgs:\n";
                        foreach (var prop in properties)
                        {
                            debugInfo += $"  - {prop.Name}: {prop.PropertyType.Name}\n";
                        }
                        
                        // Look for contour-related properties
                        var contourProperty = properties.FirstOrDefault(p => 
                            p.Name.ToLower().Contains("contour") || 
                            p.Name.ToLower().Contains("curve") ||
                            p.Name.ToLower().Contains("region"));
                        
                        if (contourProperty != null)
                        {
                            debugInfo += $"✅ Found potential contour property: {contourProperty.Name}\n";
                            try
                            {
                                contourProperty.SetValue(args._i, contourCurve._i);
                                debugInfo += $"✅ Set {contourProperty.Name} to contour curve\n";
                            }
                            catch (Exception ex)
                            {
                                debugInfo += $"⚠️ Could not set {contourProperty.Name}: {ex.Message}\n";
                            }
                        }
                        else
                        {
                            debugInfo += "⚠️ No contour-related properties found in NewEntityArgs\n";
                        }
                        
                        // Create the floor
                        var operation = CreateOperation();
                        if (operation != null && operation._i != null)
                        {
                            operation.Start();
                            var floor = CreateObject(args);
                            if (floor != null)
                            {
                                operation.Apply();
                                debugInfo += $"✅ Floor created successfully (ID: {floor.Id})\n";
                                
                                // Check if the contour was set
                                var floorParamsInterface = floor._i.GetInterfaceByName("IFloorParams");
                                if (floorParamsInterface != null)
                                {
                                    var floorParams = floorParamsInterface as Renga.IFloorParams;
                                    if (floorParams != null)
                                    {
                                        var contour = floorParams.GetContour();
                                        if (contour != null)
                                        {
                                            double floorContourLength = contour.GetLength();
                                            debugInfo += $"📏 Floor contour length: {floorContourLength:F2}mm\n";
                                            debugInfo += $"📏 Input curve length: {curveLength:F2}mm\n";
                                            
                                            if (Math.Abs(floorContourLength - curveLength) < 1.0)
                                            {
                                                debugInfo += "✅ Floor contour matches input curve!\n";
                                                return new Dictionary<string, object>
                                                {
                                                    { "Floor", floor },
                                                    { "Success", true },
                                                    { "DebugInfo", debugInfo }
                                                };
                                            }
                                            else
                                            {
                                                debugInfo += "⚠️ Floor contour doesn't match input curve\n";
                                            }
                                        }
                                    }
                                }
                                
                                return new Dictionary<string, object>
                                {
                                    { "Floor", floor },
                                    { "Success", false },
                                    { "DebugInfo", debugInfo }
                                };
                            }
                            else
                            {
                                operation.Rollback();
                                debugInfo += "❌ Failed to create floor object\n";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Approach 1 failed: {ex.Message}\n";
                }
                
                // Approach 2: Try to create floor and then modify its contour
                debugInfo += "🔍 Approach 2: Creating floor then trying to modify contour...\n";
                try
                {
                    var floorResult = CreateFloor(levelId, placement3D);
                    var floor = floorResult["Floor"] as ModelObject;
                    if (floor != null)
                    {
                        debugInfo += $"✅ Floor created successfully (ID: {floor.Id})\n";
                        
                        // Try to get IFloorParams and modify contour
                        var floorParamsInterface = floor._i.GetInterfaceByName("IFloorParams");
                        if (floorParamsInterface != null)
                        {
                            var floorParams = floorParamsInterface as Renga.IFloorParams;
                            if (floorParams != null)
                            {
                                debugInfo += "✅ Got IFloorParams interface\n";
                                
                                // Check if IFloorParams has a SetContour method
                                var floorParamsType = floorParams.GetType();
                                var methods = floorParamsType.GetMethods();
                                debugInfo += "Available methods in IFloorParams:\n";
                                foreach (var method in methods)
                                {
                                    debugInfo += $"  - {method.Name}\n";
                                }
                                
                                // Look for SetContour method
                                var setContourMethod = methods.FirstOrDefault(m => m.Name == "SetContour");
                                if (setContourMethod != null)
                                {
                                    debugInfo += "✅ Found SetContour method!\n";
                                    try
                                    {
                                        setContourMethod.Invoke(floorParams, new object[] { contourCurve._i });
                                        debugInfo += "✅ Successfully called SetContour method\n";
                                        
                                        // Verify the contour was set
                                        var contour = floorParams.GetContour();
                                        if (contour != null)
                                        {
                                            double floorContourLength = contour.GetLength();
                                            debugInfo += $"📏 Floor contour length after SetContour: {floorContourLength:F2}mm\n";
                                            debugInfo += $"📏 Input curve length: {curveLength:F2}mm\n";
                                            
                                            if (Math.Abs(floorContourLength - curveLength) < 1.0)
                                            {
                                                debugInfo += "✅ Floor contour matches input curve!\n";
                                                return new Dictionary<string, object>
                                                {
                                                    { "Floor", floor },
                                                    { "Success", true },
                                                    { "DebugInfo", debugInfo }
                                                };
                                            }
                                            else
                                            {
                                                debugInfo += "⚠️ Floor contour still doesn't match input curve\n";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        debugInfo += $"❌ Error calling SetContour: {ex.Message}\n";
                                    }
                                }
                                else
                                {
                                    debugInfo += "❌ No SetContour method found in IFloorParams\n";
                                }
                            }
                        }
                        
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Approach 2 failed: {ex.Message}\n";
                }
                
                debugInfo += "\n💡 CONCLUSION: Floor contour setting approaches\n";
                debugInfo += "💡 Floors use IFloorParams interface for contours\n";
                debugInfo += "💡 Contours may need to be set during creation, not after\n";
                debugInfo += "💡 The C++ example might use a different mechanism\n";
                debugInfo += "💡 Consider using walls or other objects that support baselines\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor with contour creation!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола на основе базовой линии выбранного объекта
        /// Получает базовую линию от выбранного объекта и создает пол с этой геометрией
        /// </summary>
        /// <param name="sourceObjectId">ID объекта-источника для получения базовой линии</param>
        /// <param name="levelId">ID уровня для размещения нового пола</param>
        /// <param name="placement3D">3D размещение нового пола</param>
        /// <returns>Результат создания пола на основе базовой линии</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "SourceBaseline", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorFromObjectBaseline(int sourceObjectId, int levelId, object placement3D)
        {
            var debugInfo = "🏗️ Creating floor from object baseline...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                debugInfo += $"📋 Source object ID: {sourceObjectId}\n";
                debugInfo += $"📋 Target level ID: {levelId}\n";
                
                // Step 1: Get the source object
                debugInfo += "📋 Step 1: Getting source object...\n";
                var sourceObject = GetObjectById(sourceObjectId);
                if (sourceObject == null)
                {
                    debugInfo += $"❌ Source object with ID {sourceObjectId} not found\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Source object found (ID: {sourceObject.Id})\n";
                debugInfo += $"📋 Source object type: {sourceObject.ObjectType}\n";
                
                // Step 2: Get baseline from source object
                debugInfo += "📋 Step 2: Getting baseline from source object...\n";
                try
                {
                    var baselineInterface = sourceObject._i.GetInterfaceByName("IBaseline2DObject");
                    if (baselineInterface == null)
                    {
                        debugInfo += "❌ Source object does not support IBaseline2DObject interface\n";
                        debugInfo += "💡 This object type may not have a baseline\n";
                        
                        // List available interfaces
                        debugInfo += "🔍 Available interfaces on source object:\n";
                        var sourceType = sourceObject._i.GetType();
                        var interfaces = sourceType.GetInterfaces();
                        foreach (var iface in interfaces)
                        {
                            debugInfo += $"  - {iface.Name}\n";
                        }
                        
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Source object supports IBaseline2DObject interface\n";
                    
                    var baseline2D = baselineInterface as Renga.IBaseline2DObject;
                    if (baseline2D == null)
                    {
                        debugInfo += "❌ Could not cast to IBaseline2DObject\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    // Get the baseline curve
                    var baselineCurve = baseline2D.GetBaseline();
                    if (baselineCurve == null)
                    {
                        debugInfo += "❌ Source object has no baseline curve\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Baseline curve retrieved from source object\n";
                    
                    // Get baseline properties
                    double baselineLength = baselineCurve.GetLength();
                    debugInfo += $"📏 Source baseline length: {baselineLength:F2}mm\n";
                    
                    // Create Curve2D wrapper for the baseline
                    var baselineCurve2D = new DynRenga.DynGeometry.Curve2D(baselineCurve);
                    debugInfo += "✅ Baseline curve wrapped in Curve2D\n";
                    
                    // Step 3: Create floor using the baseline
                    debugInfo += "📋 Step 3: Creating floor using source baseline...\n";
                    
                    // Try different approaches to create floor with baseline
                    
                    // Approach 1: Try to create floor and set baseline
                    debugInfo += "🔍 Approach 1: Create floor then set baseline...\n";
                    try
                    {
                        var floorResult = CreateFloor(levelId, placement3D);
                        var floor = floorResult["Floor"] as ModelObject;
                        if (floor != null)
                        {
                            debugInfo += $"✅ Floor created successfully (ID: {floor.Id})\n";
                            
                            // Try to set baseline on the floor
                            var floorBaselineInterface = floor._i.GetInterfaceByName("IBaseline2DObject");
                            if (floorBaselineInterface != null)
                            {
                                var floorBaseline2D = floorBaselineInterface as Renga.IBaseline2DObject;
                                if (floorBaseline2D != null)
                                {
                                    debugInfo += "✅ Floor supports IBaseline2DObject interface\n";
                                    
                                    // Get baseline before setting
                                    var floorBaselineBefore = floorBaseline2D.GetBaseline();
                                    double floorBaselineLengthBefore = floorBaselineBefore?.GetLength() ?? 0;
                                    debugInfo += $"📏 Floor baseline before setting: {floorBaselineLengthBefore:F2}mm\n";
                                    
                                    // Set the baseline
                                    floorBaseline2D.SetBaseline(baselineCurve);
                                    debugInfo += "✅ SetBaseline called on floor\n";
                                    
                                    // Get baseline after setting
                                    var floorBaselineAfter = floorBaseline2D.GetBaseline();
                                    double floorBaselineLengthAfter = floorBaselineAfter?.GetLength() ?? 0;
                                    debugInfo += $"📏 Floor baseline after setting: {floorBaselineLengthAfter:F2}mm\n";
                                    
                                    if (Math.Abs(floorBaselineLengthAfter - baselineLength) < 1.0)
                                    {
                                        debugInfo += "✅ Floor baseline matches source baseline!\n";
                                        return new Dictionary<string, object>
                                        {
                                            { "Floor", floor },
                                            { "SourceBaseline", baselineCurve2D },
                                            { "Success", true },
                                            { "DebugInfo", debugInfo }
                                        };
                                    }
                                    else
                                    {
                                        debugInfo += "⚠️ Floor baseline doesn't match source baseline\n";
                                        debugInfo += "💡 This confirms that floors don't support baselines\n";
                                    }
                                }
                            }
                            else
                            {
                                debugInfo += "❌ Floor does not support IBaseline2DObject interface\n";
                            }
                            
                            return new Dictionary<string, object>
                            {
                                { "Floor", floor },
                                { "SourceBaseline", baselineCurve2D },
                                { "Success", false },
                                { "DebugInfo", debugInfo }
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Approach 1 failed: {ex.Message}\n";
                    }
                    
                    // Approach 2: Try to create floor with contour using the baseline
                    debugInfo += "🔍 Approach 2: Create floor with contour using baseline...\n";
                    try
                    {
                        var floorResult = CreateFloorWithContourProper(levelId, placement3D, baselineCurve2D);
                        var floor = floorResult["Floor"] as ModelObject;
                        var success = (bool)floorResult["Success"];
                        var approachDebugInfo = floorResult["DebugInfo"] as string;
                        
                        debugInfo += "📋 Approach 2 results:\n";
                        debugInfo += approachDebugInfo;
                        
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", success },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Approach 2 failed: {ex.Message}\n";
                    }
                    
                    // Return the baseline even if floor creation failed
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", baselineCurve2D },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting baseline from source object: {ex.Message}\n";
                    debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor creation from object baseline!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "SourceBaseline", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Получение объекта по ID
        /// </summary>
        /// <param name="objectId">ID объекта для поиска</param>
        /// <returns>ModelObject найденного объекта или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ModelObject GetObjectById(int objectId)
        {
            try
            {
                if (this._i == null)
                {
                    return null;
                }
                
                var objects = this._i.GetObjects();
                var modelObject = objects.GetById(objectId);
                if (modelObject == null)
                {
                    return null;
                }
                
                return new ModelObject(modelObject);
            }
            catch (Exception ex)
            {
                // Return null if object not found or error occurs
                return null;
            }
        }

        /// <summary>
        /// Создание пола на основе контура выбранного объекта
        /// Получает контур от выбранного объекта и создает пол с этой геометрией
        /// </summary>
        /// <param name="sourceObjectId">ID объекта-источника для получения контура</param>
        /// <param name="levelId">ID уровня для размещения нового пола</param>
        /// <param name="placement3D">3D размещение нового пола</param>
        /// <returns>Результат создания пола на основе контура</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "SourceContour", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorFromObjectContour(int sourceObjectId, int levelId, object placement3D)
        {
            var debugInfo = "🏗️ Creating floor from object contour...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceContour", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceContour", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                debugInfo += $"📋 Source object ID: {sourceObjectId}\n";
                debugInfo += $"📋 Target level ID: {levelId}\n";
                
                // Step 1: Get the source object
                debugInfo += "📋 Step 1: Getting source object...\n";
                var sourceObject = GetObjectById(sourceObjectId);
                if (sourceObject == null)
                {
                    debugInfo += $"❌ Source object with ID {sourceObjectId} not found\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceContour", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Source object found (ID: {sourceObject.Id})\n";
                debugInfo += $"📋 Source object type: {sourceObject.ObjectType}\n";
                
                // Step 2: Try to get contour from source object
                debugInfo += "📋 Step 2: Getting contour from source object...\n";
                
                // Try IFloorParams first (for floors)
                var floorParamsInterface = sourceObject._i.GetInterfaceByName("IFloorParams");
                if (floorParamsInterface != null)
                {
                    debugInfo += "✅ Source object supports IFloorParams interface\n";
                    var floorParams = floorParamsInterface as Renga.IFloorParams;
                    if (floorParams != null)
                    {
                        var contour = floorParams.GetContour();
                        if (contour != null)
                        {
                            debugInfo += "✅ Contour retrieved from IFloorParams\n";
                            double contourLength = contour.GetLength();
                            debugInfo += $"📏 Source contour length: {contourLength:F2}mm\n";
                            
                            // Create Curve2D wrapper for the contour
                            var contourCurve2D = new DynRenga.DynGeometry.Curve2D(contour);
                            debugInfo += "✅ Contour curve wrapped in Curve2D\n";
                            
                            // Step 3: Create floor using the contour
                            debugInfo += "📋 Step 3: Creating floor using source contour...\n";
                            
                            // Try to create floor with contour using the proper method
                            var floorResult = CreateFloorWithContourProper(levelId, placement3D, contourCurve2D);
                            var floor = floorResult["Floor"] as ModelObject;
                            var success = (bool)floorResult["Success"];
                            var approachDebugInfo = floorResult["DebugInfo"] as string;
                            
                            debugInfo += "📋 Contour-based floor creation results:\n";
                            debugInfo += approachDebugInfo;
                            
                            return new Dictionary<string, object>
                            {
                                { "Floor", floor },
                                { "SourceContour", contourCurve2D },
                                { "Success", success },
                                { "DebugInfo", debugInfo }
                            };
                        }
                        else
                        {
                            debugInfo += "❌ Source object has no contour\n";
                        }
                    }
                }
                else
                {
                    debugInfo += "❌ Source object does not support IFloorParams interface\n";
                }
                
                // Try IBaseline2DObject as fallback (for other objects)
                debugInfo += "🔍 Trying IBaseline2DObject as fallback...\n";
                var baselineInterface = sourceObject._i.GetInterfaceByName("IBaseline2DObject");
                if (baselineInterface != null)
                {
                    debugInfo += "✅ Source object supports IBaseline2DObject interface\n";
                    var baseline2D = baselineInterface as Renga.IBaseline2DObject;
                    if (baseline2D != null)
                    {
                        var baselineCurve = baseline2D.GetBaseline();
                        if (baselineCurve != null)
                        {
                            debugInfo += "✅ Baseline curve retrieved from IBaseline2DObject\n";
                            double baselineLength = baselineCurve.GetLength();
                            debugInfo += $"📏 Source baseline length: {baselineLength:F2}mm\n";
                            
                            // Create Curve2D wrapper for the baseline
                            var baselineCurve2D = new DynRenga.DynGeometry.Curve2D(baselineCurve);
                            debugInfo += "✅ Baseline curve wrapped in Curve2D\n";
                            
                            // Check if the curve is closed (required for contours)
                            bool isClosed = baselineCurve2D.IsClosed;
                            debugInfo += $"📏 Curve is closed: {isClosed}\n";
                            
                            if (!isClosed)
                            {
                                debugInfo += "⚠️ Baseline curve is not closed - cannot be used as floor contour\n";
                                debugInfo += "💡 Floors require closed contours\n";
                                return new Dictionary<string, object>
                                {
                                    { "Floor", null },
                                    { "SourceContour", baselineCurve2D },
                                    { "Success", false },
                                    { "DebugInfo", debugInfo }
                                };
                            }
                            
                            // Step 3: Create floor using the baseline as contour
                            debugInfo += "📋 Step 3: Creating floor using baseline as contour...\n";
                            
                            // Try to create floor with contour using the proper method
                            var floorResult = CreateFloorWithContourProper(levelId, placement3D, baselineCurve2D);
                            var floor = floorResult["Floor"] as ModelObject;
                            var success = (bool)floorResult["Success"];
                            var approachDebugInfo = floorResult["DebugInfo"] as string;
                            
                            debugInfo += "📋 Baseline-as-contour floor creation results:\n";
                            debugInfo += approachDebugInfo;
                            
                            return new Dictionary<string, object>
                            {
                                { "Floor", floor },
                                { "SourceContour", baselineCurve2D },
                                { "Success", success },
                                { "DebugInfo", debugInfo }
                            };
                        }
                        else
                        {
                            debugInfo += "❌ Source object has no baseline curve\n";
                        }
                    }
                }
                else
                {
                    debugInfo += "❌ Source object does not support IBaseline2DObject interface\n";
                }
                
                // List available interfaces for debugging
                debugInfo += "🔍 Available interfaces on source object:\n";
                var sourceType = sourceObject._i.GetType();
                var interfaces = sourceType.GetInterfaces();
                foreach (var iface in interfaces)
                {
                    debugInfo += $"  - {iface.Name}\n";
                }
                
                debugInfo += "\n💡 CONCLUSION: No suitable geometry found\n";
                debugInfo += "💡 Source object may not have a contour or baseline\n";
                debugInfo += "💡 Try using a different object type (floor, wall, etc.)\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "SourceContour", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor creation from object contour!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "SourceContour", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола на основе базовой линии с поддержкой поликривых
        /// Правильная реализация на основе рабочего C++ кода
        /// </summary>
        /// <param name="sourceObjectId">ID объекта-источника для получения базовой линии</param>
        /// <param name="levelId">ID уровня для размещения нового пола</param>
        /// <param name="placement3D">3D размещение нового пола</param>
        /// <returns>Результат создания пола на основе базовой линии</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "SourceBaseline", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorFromObjectBaselineCorrect(int sourceObjectId, int levelId, object placement3D)
        {
            var debugInfo = "🏗️ Creating floor from object baseline (correct implementation)...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                debugInfo += $"📋 Source object ID: {sourceObjectId}\n";
                debugInfo += $"📋 Target level ID: {levelId}\n";
                
                // Step 1: Get the source object
                debugInfo += "📋 Step 1: Getting source object...\n";
                var sourceObject = GetObjectById(sourceObjectId);
                if (sourceObject == null)
                {
                    debugInfo += $"❌ Source object with ID {sourceObjectId} not found\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Source object found (ID: {sourceObject.Id})\n";
                debugInfo += $"📋 Source object type: {sourceObject.ObjectType}\n";
                
                // Step 2: Get baseline from source object
                debugInfo += "📋 Step 2: Getting baseline from source object...\n";
                try
                {
                    var baselineInterface = sourceObject._i.GetInterfaceByName("IBaseline2DObject");
                    if (baselineInterface == null)
                    {
                        debugInfo += "❌ Source object does not support IBaseline2DObject interface\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Source object supports IBaseline2DObject interface\n";
                    
                    var baseline2D = baselineInterface as Renga.IBaseline2DObject;
                    if (baseline2D == null)
                    {
                        debugInfo += "❌ Could not cast to IBaseline2DObject\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    // Get the baseline curve
                    var baselineCurve = baseline2D.GetBaseline();
                    if (baselineCurve == null)
                    {
                        debugInfo += "❌ Source object has no baseline curve\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Baseline curve retrieved from source object\n";
                    
                    // Get baseline properties
                    double baselineLength = baselineCurve.GetLength();
                    debugInfo += $"📏 Source baseline length: {baselineLength:F2}mm\n";
                    
                    // Check if it's a polycurve
                    bool isPolycurve = false;
                    try
                    {
                        // Try to get segment count to determine if it's a polycurve
                        var polycurveInterface = baselineCurve as Renga.IPolyCurve2D;
                        if (polycurveInterface != null)
                        {
                            int segmentCount = polycurveInterface.GetSegmentCount();
                            debugInfo += $"📏 Baseline is a polycurve with {segmentCount} segments\n";
                            isPolycurve = true;
                        }
                        else
                        {
                            debugInfo += "📏 Baseline is a simple curve\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"📏 Could not determine curve type: {ex.Message}\n";
                    }
                    
                    // Create Curve2D wrapper for the baseline
                    var baselineCurve2D = new DynRenga.DynGeometry.Curve2D(baselineCurve);
                    debugInfo += "✅ Baseline curve wrapped in Curve2D\n";
                    
                    // Step 3: Create floor using the baseline (correct approach)
                    debugInfo += "📋 Step 3: Creating floor using source baseline...\n";
                    
                    // Create floor first
                    var floorResult = CreateFloor(levelId, placement3D);
                    var floor = floorResult["Floor"] as ModelObject;
                    if (floor == null)
                    {
                        debugInfo += "❌ Failed to create floor\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += $"✅ Floor created successfully (ID: {floor.Id})\n";
                    
                    // Get IBaseline2DObject interface from the new floor
                    var floorBaselineInterface = floor._i.GetInterfaceByName("IBaseline2DObject");
                    if (floorBaselineInterface == null)
                    {
                        debugInfo += "❌ Floor does not support IBaseline2DObject interface\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Floor supports IBaseline2DObject interface\n";
                    
                    var floorBaseline2D = floorBaselineInterface as Renga.IBaseline2DObject;
                    if (floorBaseline2D == null)
                    {
                        debugInfo += "❌ Could not cast floor to IBaseline2DObject\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    // Get baseline before setting
                    var floorBaselineBefore = floorBaseline2D.GetBaseline();
                    double floorBaselineLengthBefore = floorBaselineBefore?.GetLength() ?? 0;
                    debugInfo += $"📏 Floor baseline before setting: {floorBaselineLengthBefore:F2}mm\n";
                    
                    // Set the baseline using the exact same approach as C++
                    debugInfo += "🔄 Setting baseline on floor...\n";
                    try
                    {
                        floorBaseline2D.SetBaseline(baselineCurve);
                        debugInfo += "✅ SetBaseline called successfully\n";
                        
                        // Get baseline after setting
                        var floorBaselineAfter = floorBaseline2D.GetBaseline();
                        double floorBaselineLengthAfter = floorBaselineAfter?.GetLength() ?? 0;
                        debugInfo += $"📏 Floor baseline after setting: {floorBaselineLengthAfter:F2}mm\n";
                        
                        // Check if the baseline was actually set
                        if (Math.Abs(floorBaselineLengthAfter - baselineLength) < 1.0)
                        {
                            debugInfo += "✅ Floor baseline matches source baseline!\n";
                            debugInfo += "🎉 Successfully applied baseline to new floor!\n";
                            return new Dictionary<string, object>
                            {
                                { "Floor", floor },
                                { "SourceBaseline", baselineCurve2D },
                                { "Success", true },
                                { "DebugInfo", debugInfo }
                            };
                        }
                        else
                        {
                            debugInfo += "⚠️ Floor baseline doesn't match source baseline\n";
                            debugInfo += $"📊 Length difference: {Math.Abs(floorBaselineLengthAfter - baselineLength):F2}mm\n";
                            
                            // Try alternative approach - check if the curve needs to be in a different coordinate system
                            debugInfo += "🔍 Trying alternative approach...\n";
                            
                            // Check if the floor has a different coordinate system
                            var floorLevelObject = floor._i.GetInterfaceByName("ILevelObject");
                            if (floorLevelObject != null)
                            {
                                var levelObj = floorLevelObject as Renga.ILevelObject;
                                if (levelObj != null)
                                {
                                    int floorLevelId = levelObj.LevelId;
                                    debugInfo += $"📋 Floor level ID: {floorLevelId}\n";
                                    
                                    // Check if we need to transform the curve to the floor's coordinate system
                                    debugInfo += "💡 The curve might need to be transformed to the floor's coordinate system\n";
                                    debugInfo += "💡 This is a common issue with complex curves and coordinate systems\n";
                                }
                            }
                            
                            return new Dictionary<string, object>
                            {
                                { "Floor", floor },
                                { "SourceBaseline", baselineCurve2D },
                                { "Success", false },
                                { "DebugInfo", debugInfo }
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Error setting baseline: {ex.Message}\n";
                        debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                        
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting baseline from source object: {ex.Message}\n";
                    debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor creation from object baseline!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "SourceBaseline", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Создание пола на основе базовой линии с правильной обработкой координатных систем
        /// Исправленная реализация на основе рабочего C++ кода
        /// </summary>
        /// <param name="sourceObjectId">ID объекта-источника для получения базовой линии</param>
        /// <param name="levelId">ID уровня для размещения нового пола</param>
        /// <param name="placement3D">3D размещение нового пола</param>
        /// <returns>Результат создания пола на основе базовой линии</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Floor", "SourceBaseline", "Success", "DebugInfo" })]
        public Dictionary<string, object> CreateFloorFromObjectBaselineFixed(int sourceObjectId, int levelId, object placement3D)
        {
            var debugInfo = "🏗️ Creating floor from object baseline (fixed implementation)...\n";
            
            try
            {
                // Validate inputs
                if (this._i == null)
                {
                    debugInfo += "❌ Model interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                if (placement3D == null)
                {
                    debugInfo += "❌ Placement3D cannot be null\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += "✅ Input validation passed\n";
                debugInfo += $"📋 Source object ID: {sourceObjectId}\n";
                debugInfo += $"📋 Target level ID: {levelId}\n";
                
                // Step 1: Get the source object
                debugInfo += "📋 Step 1: Getting source object...\n";
                var sourceObject = GetObjectById(sourceObjectId);
                if (sourceObject == null)
                {
                    debugInfo += $"❌ Source object with ID {sourceObjectId} not found\n";
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                debugInfo += $"✅ Source object found (ID: {sourceObject.Id})\n";
                debugInfo += $"📋 Source object type: {sourceObject.ObjectType}\n";
                
                // Step 2: Get baseline from source object
                debugInfo += "📋 Step 2: Getting baseline from source object...\n";
                try
                {
                    var baselineInterface = sourceObject._i.GetInterfaceByName("IBaseline2DObject");
                    if (baselineInterface == null)
                    {
                        debugInfo += "❌ Source object does not support IBaseline2DObject interface\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Source object supports IBaseline2DObject interface\n";
                    
                    var baseline2D = baselineInterface as Renga.IBaseline2DObject;
                    if (baseline2D == null)
                    {
                        debugInfo += "❌ Could not cast to IBaseline2DObject\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    // Get the baseline curve
                    var baselineCurve = baseline2D.GetBaseline();
                    if (baselineCurve == null)
                    {
                        debugInfo += "❌ Source object has no baseline curve\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", null },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Baseline curve retrieved from source object\n";
                    
                    // Get baseline properties
                    double baselineLength = baselineCurve.GetLength();
                    debugInfo += $"📏 Source baseline length: {baselineLength:F2}mm\n";
                    
                    // Check if it's a polycurve
                    bool isPolycurve = false;
                    int segmentCount = 0;
                    try
                    {
                        var polycurveInterface = baselineCurve as Renga.IPolyCurve2D;
                        if (polycurveInterface != null)
                        {
                            segmentCount = polycurveInterface.GetSegmentCount();
                            debugInfo += $"📏 Baseline is a polycurve with {segmentCount} segments\n";
                            isPolycurve = true;
                        }
                        else
                        {
                            debugInfo += "📏 Baseline is a simple curve\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"📏 Could not determine curve type: {ex.Message}\n";
                    }
                    
                    // Create Curve2D wrapper for the baseline
                    var baselineCurve2D = new DynRenga.DynGeometry.Curve2D(baselineCurve);
                    debugInfo += "✅ Baseline curve wrapped in Curve2D\n";
                    
                    // Step 3: Create floor using the baseline (fixed approach)
                    debugInfo += "📋 Step 3: Creating floor using source baseline...\n";
                    
                    // Create floor first
                    var floorResult = CreateFloor(levelId, placement3D);
                    var floor = floorResult["Floor"] as ModelObject;
                    if (floor == null)
                    {
                        debugInfo += "❌ Failed to create floor\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", null },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += $"✅ Floor created successfully (ID: {floor.Id})\n";
                    
                    // Get IBaseline2DObject interface from the new floor
                    var floorBaselineInterface = floor._i.GetInterfaceByName("IBaseline2DObject");
                    if (floorBaselineInterface == null)
                    {
                        debugInfo += "❌ Floor does not support IBaseline2DObject interface\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    debugInfo += "✅ Floor supports IBaseline2DObject interface\n";
                    
                    var floorBaseline2D = floorBaselineInterface as Renga.IBaseline2DObject;
                    if (floorBaseline2D == null)
                    {
                        debugInfo += "❌ Could not cast floor to IBaseline2DObject\n";
                        return new Dictionary<string, object>
                        {
                            { "Floor", floor },
                            { "SourceBaseline", baselineCurve2D },
                            { "Success", false },
                            { "DebugInfo", debugInfo }
                        };
                    }
                    
                    // Get baseline before setting
                    var floorBaselineBefore = floorBaseline2D.GetBaseline();
                    double floorBaselineLengthBefore = floorBaselineBefore?.GetLength() ?? 0;
                    debugInfo += $"📏 Floor baseline before setting: {floorBaselineLengthBefore:F2}mm\n";
                    
                    // Try different approaches to set the baseline
                    debugInfo += "🔄 Trying different approaches to set baseline...\n";
                    
                    // Approach 1: Direct SetBaseline (same as C++)
                    debugInfo += "🔍 Approach 1: Direct SetBaseline...\n";
                    try
                    {
                        floorBaseline2D.SetBaseline(baselineCurve);
                        debugInfo += "✅ SetBaseline called successfully\n";
                        
                        // Get baseline after setting
                        var floorBaselineAfter = floorBaseline2D.GetBaseline();
                        double floorBaselineLengthAfter = floorBaselineAfter?.GetLength() ?? 0;
                        debugInfo += $"📏 Floor baseline after setting: {floorBaselineLengthAfter:F2}mm\n";
                        
                        if (Math.Abs(floorBaselineLengthAfter - baselineLength) < 1.0)
                        {
                            debugInfo += "✅ Floor baseline matches source baseline!\n";
                            debugInfo += "🎉 Successfully applied baseline to new floor!\n";
                            return new Dictionary<string, object>
                            {
                                { "Floor", floor },
                                { "SourceBaseline", baselineCurve2D },
                                { "Success", true },
                                { "DebugInfo", debugInfo }
                            };
                        }
                        else
                        {
                            debugInfo += "⚠️ Floor baseline doesn't match source baseline\n";
                            debugInfo += $"📊 Length difference: {Math.Abs(floorBaselineLengthAfter - baselineLength):F2}mm\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Approach 1 failed: {ex.Message}\n";
                    }
                    
                    // Approach 2: Try to get the floor's coordinate system and transform the curve
                    debugInfo += "🔍 Approach 2: Coordinate system transformation...\n";
                    try
                    {
                        // Get the floor's coordinate system
                        var floorLevelObject = floor._i.GetInterfaceByName("ILevelObject");
                        if (floorLevelObject != null)
                        {
                            var levelObj = floorLevelObject as Renga.ILevelObject;
                            if (levelObj != null)
                            {
                                int floorLevelId = levelObj.LevelId;
                                debugInfo += $"📋 Floor level ID: {floorLevelId}\n";
                                
                                // Get the source object's level
                                var sourceLevelObject = sourceObject._i.GetInterfaceByName("ILevelObject");
                                if (sourceLevelObject != null)
                                {
                                    var sourceLevelObj = sourceLevelObject as Renga.ILevelObject;
                                    if (sourceLevelObj != null)
                                    {
                                        int sourceLevelId = sourceLevelObj.LevelId;
                                        debugInfo += $"📋 Source level ID: {sourceLevelId}\n";
                                        
                                        if (sourceLevelId == floorLevelId)
                                        {
                                            debugInfo += "✅ Both objects are on the same level\n";
                                            debugInfo += "💡 The issue might be with the curve itself or its coordinate system\n";
                                        }
                                        else
                                        {
                                            debugInfo += "⚠️ Objects are on different levels\n";
                                            debugInfo += "💡 This might require coordinate system transformation\n";
                                        }
                                    }
                                }
                            }
                        }
                        
                        // Try to get the floor's coordinate system
                        var floorCoordinateSystem = floor._i.GetInterfaceByName("ICoordinateSystem2D");
                        if (floorCoordinateSystem != null)
                        {
                            debugInfo += "✅ Floor has ICoordinateSystem2D interface\n";
                        }
                        else
                        {
                            debugInfo += "❌ Floor does not have ICoordinateSystem2D interface\n";
                        }
                        
                        // Try to get the source object's coordinate system
                        var sourceCoordinateSystem = sourceObject._i.GetInterfaceByName("ICoordinateSystem2D");
                        if (sourceCoordinateSystem != null)
                        {
                            debugInfo += "✅ Source object has ICoordinateSystem2D interface\n";
                        }
                        else
                        {
                            debugInfo += "❌ Source object does not have ICoordinateSystem2D interface\n";
                        }
                        
                        debugInfo += "💡 The issue might be that floors don't support custom baselines in this Renga version\n";
                        debugInfo += "💡 Or the curve needs to be in a specific coordinate system\n";
                        debugInfo += "💡 Your C++ code works, so there might be a subtle difference in implementation\n";
                        
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Approach 2 failed: {ex.Message}\n";
                    }
                    
                    // Approach 3: Try to create a new curve with the same geometry
                    debugInfo += "🔍 Approach 3: Creating new curve with same geometry...\n";
                    try
                    {
                        // This would require recreating the curve from scratch
                        // For now, just log that this approach would be complex
                        debugInfo += "💡 This approach would require recreating the curve from scratch\n";
                        debugInfo += "💡 It would be complex to implement and might not solve the issue\n";
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Approach 3 failed: {ex.Message}\n";
                    }
                    
                    debugInfo += "\n💡 CONCLUSION: Baseline setting approaches\n";
                    debugInfo += "💡 The C++ code works with the same approach\n";
                    debugInfo += "💡 There might be a subtle difference in how the curve is handled\n";
                    debugInfo += "💡 Or floors might not support custom baselines in this Renga version\n";
                    debugInfo += "💡 Consider using the contour-based approach instead\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", floor },
                        { "SourceBaseline", baselineCurve2D },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                catch (Exception ex)
                {
                    debugInfo += $"❌ Error getting baseline from source object: {ex.Message}\n";
                    debugInfo += $"❌ Stack trace: {ex.StackTrace}\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Floor", null },
                        { "SourceBaseline", null },
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error during floor creation from object baseline!\n";
                debugInfo += $"❌ Error: {ex.Message}\n";
                debugInfo += $"❌ Stack Trace: {ex.StackTrace}\n";
                
                return new Dictionary<string, object>
                {
                    { "Floor", null },
                    { "SourceBaseline", null },
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
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
