using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IPort interface - Connection point on MEP objects
    /// This class provides comprehensive access to all IPort interface members
    /// </summary>
    public class IPort : IDisposable
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
        /// Constructor - Creates IPort from COM object
        /// </summary>
        /// <param name="rengaPortObject">The COM object that should implement port functionality</param>
        internal IPort(object rengaPortObject)
        {
            if (rengaPortObject == null)
                throw new ArgumentNullException(nameof(rengaPortObject), "Renga port object cannot be null.");

            // Store as dynamic to avoid casting issues
            this._i = rengaPortObject;
        }

        /// <summary>
        /// Gets the position of the port in 3D space
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Position
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Port interface is not initialized.");

                try
                {
                    // Try to get position - this might be a Point3D or similar
                    return this._i.Position;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get port position: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the direction/orientation of the port
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Direction
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Port interface is not initialized.");

                try
                {
                    // Try to get direction - this might be a Vector3D or similar
                    return this._i.Direction;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get port direction: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the type of the port (inlet, outlet, bidirectional, etc.)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object PortType
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Port interface is not initialized.");

                try
                {
                    // Try to get port type
                    return this._i.PortType;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get port type: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Gets the name or identifier of the port
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Port interface is not initialized.");

                try
                {
                    return this._i.Name?.ToString() ?? "Unnamed Port";
                }
                catch (Exception ex)
                {
                    return $"Error getting name: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Gets the ID of the port
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("Port interface is not initialized.");

                try
                {
                    return Convert.ToInt32(this._i.Id);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get port ID: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Indicates whether the IPort wrapper is initialized and not disposed
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
        [dr.MultiReturn(new[] { "Id", "Name", "Position", "Direction", "PortType", "DebugInfo" })]
        public Dictionary<string, object> GetPortInfo()
        {
            if (this._i == null)
                throw new InvalidOperationException("Port interface is not initialized.");

            var info = new Dictionary<string, object>();
            var debugInfo = "--- IPort Debug Info ---\n";

            try
            {
                info["Id"] = this.Id;
                debugInfo += $"ID: {this.Id}\n";
            }
            catch
            {
                info["Id"] = -1;
                debugInfo += "ID: Error retrieving\n";
            }

            try
            {
                info["Name"] = this.Name;
                debugInfo += $"Name: {this.Name}\n";
            }
            catch
            {
                info["Name"] = "Error";
                debugInfo += "Name: Error retrieving\n";
            }

            try
            {
                info["Position"] = this.Position;
                debugInfo += $"Position: Retrieved\n";
            }
            catch
            {
                info["Position"] = null;
                debugInfo += "Position: Error retrieving\n";
            }

            try
            {
                info["Direction"] = this.Direction;
                debugInfo += $"Direction: Retrieved\n";
            }
            catch
            {
                info["Direction"] = null;
                debugInfo += "Direction: Error retrieving\n";
            }

            try
            {
                info["PortType"] = this.PortType;
                debugInfo += $"Port Type: Retrieved\n";
            }
            catch
            {
                info["PortType"] = null;
                debugInfo += "Port Type: Error retrieving\n";
            }

            debugInfo += $"Interface Status: {(this._i != null ? "✅ Initialized" : "❌ Not Initialized")}\n";
            info["DebugInfo"] = debugInfo;

            return info;
        }

        /// <summary>
        /// Gets port information with debug details
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null)
                return "❌ Port interface is not initialized.";

            try
            {
                var debugInfo = $"--- IPort Debug Info ---\n";
                debugInfo += $"ID: {this.Id}\n";
                debugInfo += $"Name: {this.Name}\n";
                debugInfo += $"Position: {(this.Position != null ? "Available" : "Not Available")}\n";
                debugInfo += $"Direction: {(this.Direction != null ? "Available" : "Not Available")}\n";
                debugInfo += $"Port Type: {(this.PortType != null ? "Available" : "Not Available")}\n";
                debugInfo += $"Interface Status: ✅ Initialized\n";

                return debugInfo;
            }
            catch (Exception ex)
            {
                return $"❌ Failed to get debug information: {ex.Message}";
            }
        }

        /// <summary>
        /// Disposes the COM object
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
        ~IPort()
        {
            Dispose();
        }
    }
}