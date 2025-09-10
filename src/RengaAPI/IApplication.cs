using System;
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

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IApplication interface - Complete API Reference
    /// This class provides comprehensive access to all IApplication interface members
    /// </summary>
    public class IApplication : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IApplication
        /// </summary>
        public Renga.IApplication _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;
        
        /// <summary>
        /// Flag for tracking if we created a new Renga instance (needs to be closed)
        /// </summary>
        private bool _ownsInstance = false;

        /// <summary>
        /// Constructor - Gets the first running Renga process and captures the IApplication interface
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IApplication()
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
        /// Constructor - Creates connection to specific Renga instance by index
        /// </summary>
        /// <param name="instanceIndex">Index of Renga instance (0-based)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IApplication(int instanceIndex)
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
                
                this._i = rengaApp;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to connect to Renga instance {instanceIndex}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if Renga application is connected
        /// </summary>
        /// <returns>True if Renga is running and connected</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsConnected => this._i != null;

        #region IApplication Properties

        /// <summary>
        /// Gets the currently active view
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
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
        /// Gets or sets whether the user interface of Renga is enabled
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool Enabled
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.Enabled;
            }
            set
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                this._i.Enabled = value;
            }
        }

        /// <summary>
        /// Gets whether there was an error
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool HasLastError
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.HasLastError;
            }
        }

        /// <summary>
        /// Gets or sets the last error message
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string LastError
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.LastError;
            }
            set
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                this._i.LastError = value;
            }
        }

        /// <summary>
        /// Gets the Math interface instance
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Math
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.Math;
            }
        }

        /// <summary>
        /// Gets the name of the product
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string ProductName
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.ProductName;
            }
        }

        /// <summary>
        /// Gets the currently opened project
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Project
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.Project;
            }
        }

        /// <summary>
        /// Gets the selection interface
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Selection
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.Selection;
            }
        }

        /// <summary>
        /// Gets the UI interface
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object UI
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.UI;
            }
        }

        /// <summary>
        /// Gets the product version
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Version
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.Version;
            }
        }

        /// <summary>
        /// Gets the product version as a string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string VersionS
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.VersionS;
            }
        }

        /// <summary>
        /// Gets or sets the visibility state of Renga's user interface
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool Visible
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                return this._i.Visible;
            }
            set
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
                this._i.Visible = value;
            }
        }

        #endregion

        #region IApplication Methods

        /// <summary>
        /// Closes the current project
        /// </summary>
        /// <param name="discardChanges">If true, discard changes without saving</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int CloseProject(bool discardChanges)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return this._i.CloseProject(discardChanges);
        }

        /// <summary>
        /// Creates a new instance of IIfcExportSettings
        /// </summary>
        /// <returns>IIfcExportSettings object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object CreateIfcExportSettings()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return this._i.CreateIfcExportSettings();
        }

        /// <summary>
        /// Creates a new project based on the default template
        /// </summary>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int CreateProject()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return this._i.CreateProject();
        }

        /// <summary>
        /// Creates a new project based on the specified template
        /// </summary>
        /// <param name="fileName">The template file name</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int CreateProjectFromTemplate(string fileName)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Template file name cannot be null or empty", nameof(fileName));
            
            return this._i.CreateProjectFromTemplate(fileName);
        }

        /// <summary>
        /// Disables the user interface of Renga and blocks user input
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Disable()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            this._i.Disable();
        }

        /// <summary>
        /// Re-enables the user interface of Renga and unblocks user input
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Enable()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            this._i.Enable();
        }

        /// <summary>
        /// Returns the current locale
        /// </summary>
        /// <returns>Current locale string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetCurrentLocale()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return this._i.GetCurrentLocale();
        }

        /// <summary>
        /// Returns the HWND of the main window
        /// </summary>
        /// <returns>Main window handle</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetMainWindowHandle()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return this._i.GetMainWindowHandle();
        }

        /// <summary>
        /// Indicates if the application has an open project
        /// </summary>
        /// <returns>True if an open project exists, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool HasProject()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return this._i.HasProject();
        }

        /// <summary>
        /// Creates a new project and imports data from the specified IFC file
        /// </summary>
        /// <param name="fileName">File name of the IFC project to import</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ImportIfcProject(string fileName)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("IFC file name cannot be null or empty", nameof(fileName));
            
            return this._i.ImportIfcProject(fileName);
        }

        /// <summary>
        /// Opens the project by the given filename
        /// </summary>
        /// <param name="fileName">The project file name</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int OpenProject(string fileName)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("Project file name cannot be null or empty", nameof(fileName));
            
            return this._i.OpenProject(fileName);
        }

        /// <summary>
        /// Opens or creates the view assigned to entity with representedEntityId
        /// </summary>
        /// <param name="representedEntityId">ID of the entity represented by the model view</param>
        /// <returns>IView object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public View OpenViewByEntity(int representedEntityId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            return new View(this._i.OpenViewByEntity(representedEntityId));
        }

        /// <summary>
        /// Quits Renga
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Quit()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Renga application is not running. Please start Renga first.");
            
            this._i.Quit();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets information about all running Renga instances
        /// </summary>
        /// <returns>List of information about running Renga instances</returns>
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
                            var hasProject = rengaApp.HasProject();
                            
                            var instanceInfo = new Dictionary<string, object>
                            {
                                { "Index", i },
                                { "IsConnected", true },
                                { "HasProject", hasProject },
                                { "ProjectPath", hasProject ? rengaApp.Project.GetType().GetProperty("FilePath")?.GetValue(rengaApp.Project)?.ToString() ?? "Unknown" : "No project loaded" },
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
        /// Gets a simple list of available Renga instances for selection
        /// </summary>
        /// <returns>List of strings with information about instances</returns>
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
        /// Gets only available (working) Renga instances
        /// </summary>
        /// <returns>List of indices of available instances</returns>
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
            catch (Exception ex)
            {
                // Return empty list on error
            }
            
            return availableIndices;
        }

        #endregion

        #region COM Interop Methods

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
        /// Gets Process ID from moniker (approximate estimation)
        /// </summary>
        /// <param name="moniker">Process moniker</param>
        /// <returns>Process ID or -1 if unable to determine</returns>
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

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases COM object resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Protected method for releasing resources
        /// </summary>
        /// <param name="disposing">true if called from Dispose(), false if from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    if (_i != null)
                    {
                        try
                        {
                            // If we created a new instance, close it
                            if (_ownsInstance)
                            {
                                _i.Quit();
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ignore errors when closing, as object may already be released
                            System.Diagnostics.Debug.WriteLine($"Error closing Renga instance: {ex.Message}");
                        }
                        finally
                        {
                            // Release COM object
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
        /// Finalizer for releasing unmanaged resources
        /// </summary>
        ~IApplication()
        {
            Dispose(false);
        }

        #endregion
    }
}
