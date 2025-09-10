using System;
using System.Collections.Generic;
using dr = Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Interface Reference for Renga.IOperation
    /// Provides functions to edit a Renga project and manage operation lifecycle
    /// </summary>
    public class IOperation : IDisposable
    {
        internal Renga.IOperation _i;
        private bool _isDisposed = false;

        /// <summary>
        /// Creates IOperation from Renga IOperation COM object
        /// </summary>
        /// <param name="rengaOperationObject">Renga IOperation COM object</param>
        internal IOperation(object rengaOperationObject)
        {
            if (rengaOperationObject == null)
                throw new ArgumentNullException(nameof(rengaOperationObject), "Renga IOperation cannot be null.");
            this._i = rengaOperationObject as Renga.IOperation;
            if (this._i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IOperation.");
        }

        /// <summary>
        /// Starts an operation
        /// You should start an operation before modifying any model objects
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Start()
        {
            if (this._i == null)
                throw new InvalidOperationException("IOperation interface is not initialized. Please check that Renga is running and a project is loaded.");

            try
            {
                this._i.Start();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to start operation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Applies the operation (commits changes)
        /// Only active operations can be applied
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Apply()
        {
            if (this._i == null)
                throw new InvalidOperationException("IOperation interface is not initialized. Please check that Renga is running and a project is loaded.");

            try
            {
                this._i.Apply();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to apply operation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Rolls back the operation (discards changes)
        /// Discards any changes made to objects since the operation started
        /// It is impossible to roll the changes back after the operation is applied
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void Rollback()
        {
            if (this._i == null)
                throw new InvalidOperationException("IOperation interface is not initialized. Please check that Renga is running and a project is loaded.");

            try
            {
                this._i.Rollback();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to rollback operation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Starts an operation with debug information
        /// </summary>
        /// <returns>Dictionary containing success status and debug information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> StartWithDebug()
        {
            var debugInfo = "🔧 Starting Renga operation...\n";

            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ IOperation interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }

                this._i.Start();
                debugInfo += "✅ Operation started successfully!\n";
                debugInfo += "💡 Changes are now being tracked\n";

                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to start operation!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";

                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Applies the operation with debug information
        /// </summary>
        /// <returns>Dictionary containing success status and debug information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> ApplyWithDebug()
        {
            var debugInfo = "🔧 Applying Renga operation...\n";

            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ IOperation interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }

                this._i.Apply();
                debugInfo += "✅ Operation applied successfully!\n";
                debugInfo += "💡 All changes have been committed to the model\n";

                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to apply operation!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";

                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Rolls back the operation with debug information
        /// </summary>
        /// <returns>Dictionary containing success status and debug information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> RollbackWithDebug()
        {
            var debugInfo = "🔧 Rolling back Renga operation...\n";

            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ IOperation interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }

                this._i.Rollback();
                debugInfo += "✅ Operation rolled back successfully!\n";
                debugInfo += "💡 All changes have been discarded\n";

                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to rollback operation!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";

                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }

        /// <summary>
        /// Gets debug information about the operation
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ IOperation interface is not initialized";

            try
            {
                var info = "🔧 IOperation Debug Information:\n";
                info += $"✅ IOperation Type: {this._i.GetType().Name}\n";
                info += $"✅ Interface Status: Initialized\n";
                info += $"💡 Use Start() to begin tracking changes\n";
                info += $"💡 Use Apply() to commit changes\n";
                info += $"💡 Use Rollback() to discard changes\n";

                return info;
            }
            catch (Exception ex)
            {
                return $"❌ Debug info failed: {ex.Message}";
            }
        }

        /// <summary>
        /// Disposes of the IOperation object
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (this._i != null)
                {
                    // Release COM object
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this._i);
                    this._i = null;
                }
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~IOperation()
        {
            Dispose();
        }
    }
}
