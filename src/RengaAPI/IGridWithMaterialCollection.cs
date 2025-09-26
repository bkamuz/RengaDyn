using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using Renga;
using DynRenga.DynObjects;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IGridWithMaterialCollection interface - Complete API Reference
    /// This class provides comprehensive access to all IGridWithMaterialCollection interface members
    /// </summary>
    public class IGridWithMaterialCollection : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IGridWithMaterialCollection
        /// </summary>
        public Renga.IGridWithMaterialCollection _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IGridWithMaterialCollection from existing IGridWithMaterialCollection interface
        /// </summary>
        /// <param name="collection">Existing IGridWithMaterialCollection interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IGridWithMaterialCollection(Renga.IGridWithMaterialCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "GridWithMaterialCollection interface cannot be null.");
            
            this._i = collection;
        }

        /// <summary>
        /// Check if grid with material collection interface is valid
        /// </summary>
        /// <returns>True if collection interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IGridWithMaterialCollection Properties

        /// <summary>
        /// Gets the number of grids in the collection
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Count
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
                
                return this._i.Count;
            }
        }

        #endregion

        #region IGridWithMaterialCollection Methods

        /// <summary>
        /// Gets a grid with material by the given index
        /// </summary>
        /// <param name="index">Index of the grid</param>
        /// <returns>GridWithMaterial instance</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public GridWithMaterial GetByIndex(int index)
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Collection has {Count} items.");
            
            try
            {
                var gridWithMaterial = this._i.Get(index);
                return new GridWithMaterial(gridWithMaterial);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get grid by index {index}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Collection Methods

        /// <summary>
        /// Gets all grids in the collection as a list
        /// </summary>
        /// <returns>List of GridWithMaterial instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<GridWithMaterial> GetAllGrids()
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            try
            {
                var grids = new List<GridWithMaterial>();
                for (int i = 0; i < Count; i++)
                {
                    grids.Add(GetByIndex(i));
                }
                return grids;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all grids: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets grids by material ID
        /// </summary>
        /// <param name="materialId">Material ID to filter by</param>
        /// <returns>List of GridWithMaterial instances with the specified material</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<GridWithMaterial> GetGridsByMaterialId(int materialId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            try
            {
                var allGrids = GetAllGrids();
                return allGrids.Where(grid => grid.IGridMaterial.Id == materialId).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get grids by material ID {materialId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets unique material IDs from all grids
        /// </summary>
        /// <returns>List of unique material IDs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<int> GetUniqueMaterialIds()
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            try
            {
                var allGrids = GetAllGrids();
                return allGrids.Select(grid => grid.IGridMaterial.Id).Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get unique material IDs: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the count of grids with a specific material
        /// </summary>
        /// <param name="materialId">Material ID to count</param>
        /// <returns>Number of grids with the specified material</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetGridCountByMaterialId(int materialId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            try
            {
                var gridsWithMaterial = GetGridsByMaterialId(materialId);
                return gridsWithMaterial.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get grid count by material ID {materialId}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets collection information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with collection information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetCollectionInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            try
            {
                var uniqueMaterialIds = GetUniqueMaterialIds();
                var materialCounts = uniqueMaterialIds.ToDictionary(
                    id => id.ToString(), 
                    id => GetGridCountByMaterialId(id)
                );
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Count", Count },
                    { "HasGrids", Count > 0 },
                    { "UniqueMaterialIds", uniqueMaterialIds },
                    { "MaterialCounts", materialCounts },
                    { "UniqueMaterialCount", uniqueMaterialIds.Count }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "Count", 0 },
                    { "HasGrids", false },
                    { "UniqueMaterialIds", new List<int>() },
                    { "MaterialCounts", new Dictionary<string, int>() },
                    { "UniqueMaterialCount", 0 }
                };
            }
        }

        /// <summary>
        /// Gets a summary of material usage across all grids
        /// </summary>
        /// <returns>Dictionary with material ID and grid count pairs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, int> GetMaterialUsageSummary()
        {
            if (this._i == null) 
                throw new InvalidOperationException("GridWithMaterialCollection interface is not initialized.");
            
            try
            {
                var allGrids = GetAllGrids();
                return allGrids.GroupBy(grid => grid.IGridMaterial.Id)
                              .ToDictionary(group => group.Key, group => group.Count());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get material usage summary: {ex.Message}", ex);
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
                            // Release COM object
                            if (Marshal.IsComObject(_i))
                            {
                                Marshal.ReleaseComObject(_i);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ignore errors when releasing, as object may already be released
                            System.Diagnostics.Debug.WriteLine($"Error releasing IGridWithMaterialCollection COM object: {ex.Message}");
                        }
                        finally
                        {
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
        ~IGridWithMaterialCollection()
        {
            Dispose(false);
        }

        #endregion
    }
}
