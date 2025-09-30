using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ILayeredMaterial interface
    /// Represents a layered material with access to layers, ID, and name properties
    /// </summary>
    public class ILayeredMaterial : IDisposable
    {
        private Renga.ILayeredMaterial _i;
        private bool _disposed = false;

        public Renga.ILayeredMaterial _i_Internal => _i;

        /// <summary>
        /// Constructor - Creates ILayeredMaterial from COM object
        /// </summary>
        /// <param name="material">The Renga.ILayeredMaterial COM object</param>
        public ILayeredMaterial(object material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material), "LayeredMaterial cannot be null");
            _i = material as Renga.ILayeredMaterial;
            if (_i == null)
                throw new ArgumentException("Object does not implement ILayeredMaterial interface", nameof(material));
        }

        /// <summary>
        /// Constructor - Creates ILayeredMaterial from typed COM object
        /// </summary>
        /// <param name="material">The typed Renga.ILayeredMaterial object</param>
        public ILayeredMaterial(Renga.ILayeredMaterial material)
        {
            _i = material ?? throw new ArgumentNullException(nameof(material));
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ILayeredMaterial()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose implementation
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the layered material
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id
        {
            get
            {
                if (_i == null) 
                    throw new InvalidOperationException("ILayeredMaterial interface is not initialized.");
                try 
                { 
                    return _i.Id; 
                } 
                catch (Exception ex) 
                { 
                    throw new InvalidOperationException($"Failed to get layered material ID: {ex.Message}", ex); 
                }
            }
        }

        /// <summary>
        /// Gets the name of the layered material
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (_i == null) 
                    throw new InvalidOperationException("ILayeredMaterial interface is not initialized.");
                try 
                { 
                    return _i.Name; 
                } 
                catch (Exception ex) 
                { 
                    throw new InvalidOperationException($"Failed to get layered material name: {ex.Message}", ex); 
                }
            }
        }

        /// <summary>
        /// Gets the index of the base material layer
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int BaseLayerIndex
        {
            get
            {
                if (_i == null) 
                    throw new InvalidOperationException("ILayeredMaterial interface is not initialized.");
                try 
                { 
                    return _i.BaseLayerIndex; 
                } 
                catch (Exception ex) 
                { 
                    throw new InvalidOperationException($"Failed to get base layer index: {ex.Message}", ex); 
                }
            }
        }

        /// <summary>
        /// Gets the collection of material layers
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Layers
        {
            get
            {
                if (_i == null) 
                    throw new InvalidOperationException("ILayeredMaterial interface is not initialized.");
                try 
                { 
                    return _i.Layers; 
                } 
                catch (Exception ex) 
                { 
                    throw new InvalidOperationException($"Failed to get material layers: {ex.Message}", ex); 
                }
            }
        }

        /// <summary>
        /// Returns the base material layer
        /// </summary>
        /// <returns>The base material layer object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetBaseLayer()
        {
            if (_i == null) 
                throw new InvalidOperationException("ILayeredMaterial interface is not initialized.");
            try 
            { 
                return _i.GetBaseLayer();
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get base layer: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Returns the identifier and group of the layered material
        /// </summary>
        /// <returns>The ID group pair object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetIdGroupPair()
        {
            if (_i == null) 
                throw new InvalidOperationException("ILayeredMaterial interface is not initialized.");
            try 
            { 
                return _i.GetIdGroupPair();
            } 
            catch (Exception ex) 
            { 
                throw new InvalidOperationException($"Failed to get ID group pair: {ex.Message}", ex); 
            }
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        /// <returns>True if initialized and not disposed</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;

        /// <summary>
        /// Gets debug information about the layered material
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        public string GetDebugInfo()
        {
            if (!IsInitialized())
                return "ILayeredMaterial - Not initialized or disposed";
            
            try
            {
                return $"ILayeredMaterial - ID: {Id}, Name: {Name}, BaseLayerIndex: {BaseLayerIndex}, Initialized: {IsInitialized()}";
            }
            catch (Exception ex)
            {
                return $"ILayeredMaterial - Error getting debug info: {ex.Message}";
            }
        }

        /// <summary>
        /// Returns a string representation of the layered material
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            if (!IsInitialized())
                return "ILayeredMaterial (Not initialized)";
            
            try
            {
                return $"ILayeredMaterial: {Name} (ID: {Id})";
            }
            catch
            {
                return "ILayeredMaterial (Error accessing properties)";
            }
        }
    }
}