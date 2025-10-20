using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IObjectWithPorts interface - Complete API Reference
    /// This class provides comprehensive access to all IObjectWithPorts interface members
    /// </summary>
    public class IObjectWithPorts : IDisposable
    {
        /// <summary>
        /// Internal COM object - using dynamic to avoid casting issues
        /// </summary>
        public dynamic _i;

        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IObjectWithPorts from COM object
        /// </summary>
        /// <param name="rengaObjectWithPortsObject">The COM object that should implement port functionality</param>
        internal IObjectWithPorts(object rengaObjectWithPortsObject)
        {
            if (rengaObjectWithPortsObject == null)
                throw new ArgumentNullException(nameof(rengaObjectWithPortsObject), "COM object cannot be null.");

            // Store as dynamic to avoid casting issues
            this._i = rengaObjectWithPortsObject;
        }

        /// <summary>
        /// Gets the number of ports on this object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("ObjectWithPorts interface is not initialized.");

                try
                {
                    return this._i.Count;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get port count: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets a port by its index
        /// </summary>
        /// <param name="index">The zero-based index of the port to retrieve</param>
        /// <returns>The IPort object at the specified index</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IPort GetByIndex(int index)
        {
            if (this._i == null)
                throw new InvalidOperationException("ObjectWithPorts interface is not initialized.");

            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {this.Count - 1}.");

            try
            {
                // Get the raw COM port object
                var portObject = this._i.GetByIndex(index);
                if (portObject == null)
                    return null;

                // Wrap it in our IPort class
                return new IPort(portObject);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get port at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all ports as a list
        /// </summary>
        /// <returns>List of IPort objects</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IPort> GetAllPorts()
        {
            if (this._i == null)
                throw new InvalidOperationException("ObjectWithPorts interface is not initialized.");

            try
            {
                var ports = new List<IPort>();
                for (int i = 0; i < this.Count; i++)
                {
                    var port = this.GetByIndex(i);
                    if (port != null)
                    {
                        ports.Add(port);
                    }
                }
                return ports;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all ports: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Indicates whether the object has any ports
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool HasPorts
        {
            get
            {
                return this.Count > 0;
            }
        }

        /// <summary>
        /// Indicates whether the IObjectWithPorts wrapper is initialized and not disposed
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized()
        {
            return this._i != null && !_disposed;
        }

        /// <summary>
        /// Gets comprehensive port information for debugging
        /// </summary>
        /// <returns>Dictionary containing port-related properties</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Count", "HasPorts", "Ports", "DebugInfo" })]
        public Dictionary<string, object> GetPortInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("ObjectWithPorts interface is not initialized.");

            try
            {
                var info = new Dictionary<string, object>();
                info["Count"] = this.Count;
                info["HasPorts"] = this.HasPorts;
                info["Ports"] = this.GetAllPorts();

                var debugInfo = $"--- IObjectWithPorts Debug Info ---\n";
                debugInfo += $"Port Count: {this.Count}\n";
                debugInfo += $"Has Ports: {this.HasPorts}\n";
                debugInfo += $"Interface Status: {(this._i != null ? "✅ Initialized" : "❌ Not Initialized")}\n";

                info["DebugInfo"] = debugInfo;
                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get port information: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets port information with debug details
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ ObjectWithPorts interface is not initialized.";

            try
            {
                var debugInfo = $"--- IObjectWithPorts Debug Info ---\n";
                debugInfo += $"Port Count: {this.Count}\n";
                debugInfo += $"Has Ports: {this.HasPorts}\n";
                debugInfo += $"Interface Status: ✅ Initialized\n";

                return debugInfo;
            }
            catch (Exception ex)
            {
                return $"❌ Failed to get debug information: {ex.Message}";
            }
        }

        /// <summary>
        /// Disposes the Renga.IObjectWithPorts COM object
        /// </summary>
        public void Dispose()
        {
            if (_i != null && !_disposed)
            {
                Marshal.ReleaseComObject(_i);
                _i = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer: release COM resources if Dispose wasn't called
        /// </summary>
        ~IObjectWithPorts()
        {
            Dispose();
        }
    }
}