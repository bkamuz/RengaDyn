using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynDocument.Views;

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Класс для работы с приложением Renga (интерфейсом Renga.IApplication)
    /// </summary>
    public class Application : IDisposable
    {
        /// <summary>
        /// Внутренний COM-объект Renga.IApplication
        /// </summary>
        public Renga.IApplication _i;
        
        /// <summary>
        /// Флаг для отслеживания освобождения ресурсов
        /// </summary>
        private bool _disposed = false;
        
        /// <summary>
        /// Флаг для отслеживания того, создали ли мы новый экземпляр Renga (нужно ли его закрывать)
        /// </summary>
        private bool _ownsInstance = false;
        /// <summary>
        /// Получает первый запущенный процесс Renga в системе и фиксирует интерфейс Renga.IApplication
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Application()
        {
            IRunningObjectTable rot;
            GetRunningObjectTable(0, out rot);

            var rengaMonikers = GetRengaMonikers();
            if (rengaMonikers.Count > 0)
            {
                object comObject;
                // Get first Renga moniker in list
                rot.GetObject(rengaMonikers[0], out comObject);
                this._i = comObject as Renga.IApplication;
            }
        }
        /// <summary>
        /// Проверяет, подключено ли приложение Renga
        /// </summary>
        /// <returns>True если Renga запущена и подключена</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsConnected => this._i != null;
        
        /// <summary>
        /// Получение информации о всех запущенных экземплярах Renga
        /// </summary>
        /// <returns>Список информации о запущенных экземплярах Renga</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<Dictionary<string, object>> GetRunningRengaInstances()
        {
            var instances = new List<Dictionary<string, object>>();
            
            try
            {
                var rengaMonikers = GetRengaMonikers();
                if (rengaMonikers == null || rengaMonikers.Count == 0)
                {
                    return instances;
                }
                
                IRunningObjectTable rot;
                GetRunningObjectTable(0, out rot);
                
                for (int i = 0; i < rengaMonikers.Count; i++)
                {
                    try
                    {
                        object comObject;
                        rot.GetObject(rengaMonikers[i], out comObject);
                        var rengaApp = comObject as Renga.IApplication;
                        
                        if (rengaApp != null)
                        {
                            string projectPath = "No project loaded";
                            var hasProject = rengaApp.HasProject();
                            if (hasProject)
                            {
                                try
                                {
                                    var projWrapper = new DynRenga.RengaAPI.IProject(rengaApp.Project);
                                    projectPath = projWrapper.FilePath ?? "Unknown";
                                }
                                catch
                                {
                                    projectPath = "Unknown";
                                }
                            }

                            var instanceInfo = new Dictionary<string, object>
                            {
                                { "Index", i },
                                { "IsConnected", true },
                                { "HasProject", hasProject },
                                { "ProjectPath", projectPath },
                                { "IsVisible", rengaApp.Visible },
                                { "ProcessId", GetProcessIdFromMoniker(rengaMonikers[i]) }
                            };
                            
                            instances.Add(instanceInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        // If we can't access this instance, add it with error info
                        var errorInfo = new Dictionary<string, object>
                        {
                            { "Index", i },
                            { "IsConnected", false },
                            { "Error", ex.Message },
                            { "HasProject", false },
                            { "ProjectPath", "Error accessing instance" },
                            { "IsVisible", false },
                            { "ProcessId", -1 }
                        };
                        
                        instances.Add(errorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                // Return error information
                var errorInfo = new Dictionary<string, object>
                {
                    { "Index", -1 },
                    { "IsConnected", false },
                    { "Error", $"Failed to enumerate Renga instances: {ex.Message}" },
                    { "HasProject", false },
                    { "ProjectPath", "Error" },
                    { "IsVisible", false },
                    { "ProcessId", -1 }
                };
                
                instances.Add(errorInfo);
            }
            
            return instances;
        }
        
        /// <summary>
        /// Создание подключения к конкретному экземпляру Renga по индексу
        /// </summary>
        /// <param name="instanceIndex">Индекс экземпляра Renga (0-based)</param>
        /// <returns>Application объект подключенный к выбранному экземпляру</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Application ConnectToRengaInstance(int instanceIndex)
        {
            try
            {
                var rengaMonikers = GetRengaMonikers();
                if (rengaMonikers == null || rengaMonikers.Count == 0)
                {
                    throw new InvalidOperationException("No Renga instances found. Please start Renga first.");
                }
                
                if (instanceIndex < 0 || instanceIndex >= rengaMonikers.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(instanceIndex), 
                        $"Instance index {instanceIndex} is out of range. Available instances: 0-{rengaMonikers.Count - 1}");
                }
                
                IRunningObjectTable rot;
                GetRunningObjectTable(0, out rot);
                
                object comObject;
                rot.GetObject(rengaMonikers[instanceIndex], out comObject);
                var rengaApp = comObject as Renga.IApplication;
                
                if (rengaApp == null)
                {
                    throw new InvalidOperationException($"Failed to connect to Renga instance {instanceIndex}. The instance may have been closed.");
                }
                
                return new Application { _i = rengaApp };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to connect to Renga instance {instanceIndex}: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Получение информации о запущенных экземплярах Renga с диагностикой
        /// </summary>
        /// <returns>Результат с информацией об экземплярах и диагностикой</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Instances", "Count", "DebugInfo" })]
        public static Dictionary<string, object> GetRengaInstancesWithDiagnostics()
        {
            var debugInfo = "🔍 Scanning for running Renga instances...\n";
            var instances = new List<Dictionary<string, object>>();
            
            try
            {
                var rengaMonikers = GetRengaMonikers();
                debugInfo += $"Found {rengaMonikers?.Count ?? 0} Renga monikers\n";
                
                if (rengaMonikers == null || rengaMonikers.Count == 0)
                {
                    debugInfo += "❌ No Renga instances found\n";
                    debugInfo += "💡 Make sure Renga is running\n";
                    
                    return new Dictionary<string, object>
                    {
                        { "Instances", instances },
                        { "Count", 0 },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                IRunningObjectTable rot;
                GetRunningObjectTable(0, out rot);
                
                for (int i = 0; i < rengaMonikers.Count; i++)
                {
                    debugInfo += $"🔧 Checking instance {i}...\n";
                    
                    try
                    {
                        object comObject;
                        rot.GetObject(rengaMonikers[i], out comObject);
                        var rengaApp = comObject as Renga.IApplication;
                        
                        if (rengaApp != null)
                        {
                            var hasProject = rengaApp.HasProject();
                            var projectPath = hasProject ? rengaApp.Project.FilePath : "No project loaded";
                            
                            var instanceInfo = new Dictionary<string, object>
                            {
                                { "Index", i },
                                { "IsConnected", true },
                                { "HasProject", hasProject },
                                { "ProjectPath", projectPath },
                                { "IsVisible", rengaApp.Visible },
                                { "ProcessId", GetProcessIdFromMoniker(rengaMonikers[i]) }
                            };
                            
                            instances.Add(instanceInfo);
                            debugInfo += $"✅ Instance {i}: Connected, Project: {(hasProject ? "Yes" : "No")}\n";
                        }
                        else
                        {
                            debugInfo += $"❌ Instance {i}: Failed to cast to IApplication\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        debugInfo += $"❌ Instance {i}: Error - {ex.Message}\n";
                        
                        var errorInfo = new Dictionary<string, object>
                        {
                            { "Index", i },
                            { "IsConnected", false },
                            { "Error", ex.Message },
                            { "HasProject", false },
                            { "ProjectPath", "Error accessing instance" },
                            { "IsVisible", false },
                            { "ProcessId", -1 }
                        };
                        
                        instances.Add(errorInfo);
                    }
                }
                
                debugInfo += $"✅ Found {instances.Count} accessible Renga instances\n";
                
                return new Dictionary<string, object>
                {
                    { "Instances", instances },
                    { "Count", instances.Count },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error scanning for Renga instances: {ex.Message}\n";
                
                return new Dictionary<string, object>
                {
                    { "Instances", instances },
                    { "Count", 0 },
                    { "DebugInfo", debugInfo }
                };
            }
        }
        
        /// <summary>
        /// Получение Process ID из монкера (приблизительная оценка)
        /// </summary>
        /// <param name="moniker">Монкер процесса</param>
        /// <returns>Process ID или -1 если не удалось определить</returns>
        private static int GetProcessIdFromMoniker(IMoniker moniker)
        {
            try
            {
                IBindCtx bindCtx;
                CreateBindCtx(0, out bindCtx);
                if (bindCtx == null)
                    return -1;
                
                string displayName;
                moniker.GetDisplayName(bindCtx, null, out displayName);
                
                // Try to extract process ID from display name
                // Format is usually something like: "!Renga.Application.1:12345"
                var parts = displayName.Split(':');
                if (parts.Length > 1)
                {
                    if (int.TryParse(parts[parts.Length - 1], out int processId))
                    {
                        return processId;
                    }
                }
                
                return -1;
            }
            catch
            {
                return -1;
            }
        }
        
        /// <summary>
        /// Получение простого списка доступных экземпляров Renga для выбора
        /// </summary>
        /// <returns>Список строк с информацией об экземплярах</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<string> GetRengaInstanceList()
        {
            var instanceList = new List<string>();
            
            try
            {
                var instances = GetRunningRengaInstances();
                
                if (instances.Count == 0)
                {
                    instanceList.Add("No Renga instances found - please start Renga first");
                    return instanceList;
                }
                
                for (int i = 0; i < instances.Count; i++)
                {
                    var instance = instances[i];
                    var isConnected = (bool)instance["IsConnected"];
                    var hasProject = (bool)instance["HasProject"];
                    var projectPath = (string)instance["ProjectPath"];
                    var processId = (int)instance["ProcessId"];
                    
                    if (isConnected)
                    {
                        var status = hasProject ? "📁 Project loaded" : "📂 No project";
                        var projectInfo = hasProject ? $" - {System.IO.Path.GetFileName(projectPath)}" : "";
                        var processInfo = processId > 0 ? $" (PID: {processId})" : "";
                        
                        instanceList.Add($"Instance {i}: {status}{projectInfo}{processInfo}");
                    }
                    else
                    {
                        var error = instance.ContainsKey("Error") ? (string)instance["Error"] : "Unknown error";
                        instanceList.Add($"Instance {i}: ❌ Error - {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                instanceList.Add($"Error getting Renga instances: {ex.Message}");
            }
            
            return instanceList;
        }
        
        /// <summary>
        /// Получение только доступных (работающих) экземпляров Renga
        /// </summary>
        /// <returns>Список индексов доступных экземпляров</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<int> GetAvailableRengaInstanceIndices()
        {
            var availableIndices = new List<int>();
            
            try
            {
                var instances = GetRunningRengaInstances();
                
                for (int i = 0; i < instances.Count; i++)
                {
                    var instance = instances[i];
                    var isConnected = (bool)instance["IsConnected"];
                    
                    if (isConnected)
                    {
                        availableIndices.Add(i);
                    }
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }
            
            return availableIndices;
        }
        /// <summary>
        /// Получение текущего активного вида в виде интерфейса Renga.IView
        /// </summary>
        public View ActiveView 
        { 
            get 
            { 
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return new View(this._i.ActiveView); 
            } 
        }
        /// <summary>
        /// Получает запущенный процесс Renga или создает его если такого нет, 
        /// и открывает проект по файловому пути к нему
        /// </summary>
        /// <param name="project_path">Файловый путь к проекту Renga</param>
        [dr.IsVisibleInDynamoLibrary(false)]
        private Application(string project_path)
        {
            IRunningObjectTable rot;
            GetRunningObjectTable(0, out rot);

            var rengaMonikers = GetRengaMonikers();
            if (rengaMonikers.Count() > 0)
            {
                object comObject;
                for (int i1 = 0; i1 < rengaMonikers.Count(); i1++)
                {
                    rot.GetObject(rengaMonikers[i1], out comObject);
                    var renga = comObject as Renga.Application;
                    if (renga.HasProject() && renga.Project.FilePath == project_path)
                    {
                        _i = renga;
                        _i.Visible = true;
                    }
                }
            }
            if (rengaMonikers.Count() > 0)
            {
                object comObject;
                rot.GetObject(rengaMonikers[0], out comObject);
                this._i = comObject as Renga.Application;
                this._i.OpenProject(project_path);
                this._i.Visible = true;
                
            }

            else
            {
                InitRengaAppAsNew(project_path);
            }
        }
        private int InitRengaAppAsNew(string PathToProject, bool IsIfc = false)
        {
            this._i = new Renga.Application();
            this._i.Visible = true;
            this._ownsInstance = true; // Мы создали новый экземпляр, нужно его закрыть при освобождении
            int result;
            string file_extension = Path.GetExtension(PathToProject);

            if (IsIfc == true && file_extension.ToLower() == ".ifc") result = _i.ImportIfcProject(PathToProject);
            else result = this._i.OpenProject(PathToProject);
            if (result != 0)
            {
                //Smth error
                _i.Quit();
                return 1;
            }
            else
            {
                //All right
                this._i.Visible = true;
                return 0;
            }

        }
        [DllImport("ole32.dll")]
        private static extern void GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
        private static List<IMoniker> GetRengaMonikers()
        {
            IRunningObjectTable rot;
            GetRunningObjectTable(0, out rot);

            IEnumMoniker monikerEnumerator = null;
            rot.EnumRunning(out monikerEnumerator);
            if (monikerEnumerator == null)
                return null;

            monikerEnumerator.Reset();

            var registries = new List<IMoniker>();

            IntPtr pNumFetched = new IntPtr();
            IMoniker[] monikers = new IMoniker[1];
            while (monikerEnumerator.Next(1, monikers, pNumFetched) == 0)
            {
                IBindCtx bindCtx;
                CreateBindCtx(0, out bindCtx);
                if (bindCtx == null)
                    continue;

                string displayName;
                monikers[0].GetDisplayName(bindCtx, null, out displayName);
                if (displayName.Contains("!Renga"))
                    registries.Add(monikers[0]);
            }
            return registries;
        }
        
        /// <summary>
        /// Освобождает ресурсы COM-объекта
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Защищенный метод освобождения ресурсов
        /// </summary>
        /// <param name="disposing">true если вызывается из Dispose(), false если из финализатора</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы
                    if (_i != null)
                    {
                        try
                        {
                            // Если мы создали новый экземпляр, закрываем его
                            if (_ownsInstance)
                            {
                                _i.Quit();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Игнорируем ошибки при закрытии, так как объект может быть уже освобожден
                            System.Diagnostics.Debug.WriteLine($"Error closing Renga instance: {ex.Message}");
                        }
                        finally
                        {
                            // Освобождаем COM-объект
                            if (Marshal.IsComObject(_i))
                            {
                                Marshal.ReleaseComObject(_i);
                            }
                            _i = null;
                        }
                    }
                }
                
                _disposed = true;
            }
        }
        
        /// <summary>
        /// Финализатор для освобождения неуправляемых ресурсов
        /// </summary>
        ~Application()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Утилита для принудительной очистки всех COM-объектов и сборки мусора
        /// Используйте этот метод для очистки памяти после работы с Renga
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static void ForceCleanup()
        {
            try
            {
                // Принудительная сборка мусора
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                // Дополнительная очистка для COM-объектов
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during forced cleanup: {ex.Message}");
            }
        }
    }
}
