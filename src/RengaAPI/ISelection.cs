using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynDocument;
using DynRenga.DynObjects;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ISelection interface - Complete API Reference
    /// This class provides comprehensive access to all ISelection interface members
    /// </summary>
    public class ISelection : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.ISelection
        /// </summary>
        public Renga.ISelection _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates DynISelection from IApplication
        /// </summary>
        /// <param name="application">Renga application instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ISelection(IApplication application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application), "Application cannot be null. Make sure you have a valid Renga application loaded.");
            
            if (application._i == null)
                throw new InvalidOperationException("Application interface is not initialized. This usually means:\n" +
                    "1. Renga application is not running - please start Renga first\n" +
                    "2. No project is loaded in Renga - please open a project in Renga");
            
            this._i = application._i.Selection as Renga.ISelection;
            
            if (this._i == null)
                throw new InvalidOperationException("Failed to get Selection from Application. This usually means:\n" +
                    "1. The project is not properly loaded in Renga\n" +
                    "2. Renga version compatibility issues\n" +
                    "Please check that you have a valid project open in Renga.");
        }

        /// <summary>
        /// Constructor - Creates DynISelection from existing ISelection interface
        /// </summary>
        /// <param name="selection">Existing ISelection interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ISelection(Renga.ISelection selection)
        {
            if (selection == null)
                throw new ArgumentNullException(nameof(selection), "Selection interface cannot be null.");
            
            this._i = selection;
        }

        /// <summary>
        /// Check if selection interface is valid
        /// </summary>
        /// <returns>True if selection interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region ISelection Methods

        /// <summary>
        /// Returns a collection of IDs of objects selected in the current view
        /// </summary>
        /// <returns>Collection of IDs of selected objects</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<int> GetSelectedObjects()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var objectIds = this._i.GetSelectedObjects();
                if (objectIds == null)
                    return new List<int>();
                
                return objectIds.Cast<int>().ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get selected objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Select objects by a collection of their IDs
        /// </summary>
        /// <param name="objectIds">An array of IDs of objects to select</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetSelectedObjects(List<int> objectIds)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            try
            {
                this._i.SetSelectedObjects(objectIds.ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set selected objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Select a single object by its ID
        /// </summary>
        /// <param name="objectId">ID of the object to select</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SelectObject(int objectId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                this._i.SetSelectedObjects(new int[] { objectId });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to select object {objectId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Clear all selected objects
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void ClearSelection()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                this._i.SetSelectedObjects(new int[0]);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to clear selection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get the count of currently selected objects
        /// </summary>
        /// <returns>Number of selected objects</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetSelectedObjectsCount()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var selectedObjects = GetSelectedObjects();
                return selectedObjects.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get selected objects count: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if a specific object is selected
        /// </summary>
        /// <param name="objectId">ID of the object to check</param>
        /// <returns>True if the object is selected, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsObjectSelected(int objectId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var selectedObjects = GetSelectedObjects();
                return selectedObjects.Contains(objectId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if object {objectId} is selected: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Selection Methods

        /// <summary>
        /// Get selected objects as ModelObject instances
        /// </summary>
        /// <param name="model">Model instance to get objects from</param>
        /// <returns>List of selected ModelObject instances</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<ModelObject> GetSelectedModelObjects(Model model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null.");
            
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var selectedIds = GetSelectedObjects();
                var selectedObjects = new List<ModelObject>();
                
                foreach (int id in selectedIds)
                {
                    try
                    {
                        var modelObject = Selection.GetModelObjectById(model, id);
                        selectedObjects.Add(modelObject);
                    }
                    catch (Exception ex)
                    {
                        // Log warning but continue with other objects
                        System.Diagnostics.Debug.WriteLine($"Warning: Could not get model object with ID {id}: {ex.Message}");
                    }
                }
                
                return selectedObjects;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get selected model objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Select objects by their ModelObject instances
        /// </summary>
        /// <param name="modelObjects">List of ModelObject instances to select</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SelectModelObjects(List<ModelObject> modelObjects)
        {
            if (modelObjects == null)
                throw new ArgumentNullException(nameof(modelObjects), "Model objects list cannot be null.");
            
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var objectIds = new List<int>();
                
                foreach (var modelObject in modelObjects)
                {
                    if (modelObject != null && modelObject._i != null)
                    {
                        objectIds.Add(modelObject._i.Id);
                    }
                }
                
                SetSelectedObjects(objectIds);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to select model objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Add objects to current selection (preserves existing selection)
        /// </summary>
        /// <param name="objectIds">IDs of objects to add to selection</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void AddToSelection(List<int> objectIds)
        {
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var currentSelection = GetSelectedObjects();
                var newSelection = currentSelection.Union(objectIds).ToList();
                SetSelectedObjects(newSelection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to add objects to selection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Remove objects from current selection
        /// </summary>
        /// <param name="objectIds">IDs of objects to remove from selection</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void RemoveFromSelection(List<int> objectIds)
        {
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var currentSelection = GetSelectedObjects();
                var newSelection = currentSelection.Except(objectIds).ToList();
                SetSelectedObjects(newSelection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove objects from selection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Toggle selection of objects (select if not selected, deselect if selected)
        /// </summary>
        /// <param name="objectIds">IDs of objects to toggle selection</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void ToggleSelection(List<int> objectIds)
        {
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var currentSelection = GetSelectedObjects();
                var newSelection = new List<int>(currentSelection);
                
                foreach (int id in objectIds)
                {
                    if (currentSelection.Contains(id))
                    {
                        newSelection.Remove(id);
                    }
                    else
                    {
                        newSelection.Add(id);
                    }
                }
                
                SetSelectedObjects(newSelection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to toggle selection: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get selection information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with selection information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetSelectionInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Selection interface is not initialized. Please check that Renga is running and a project is loaded.");
            
            try
            {
                var selectedObjects = GetSelectedObjects();
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "SelectedCount", selectedObjects.Count },
                    { "SelectedObjectIds", selectedObjects },
                    { "HasSelection", selectedObjects.Count > 0 }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "SelectedCount", 0 },
                    { "SelectedObjectIds", new List<int>() },
                    { "HasSelection", false }
                };
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing ISelection COM object: {ex.Message}");
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
        ~ISelection()
        {
            Dispose(false);
        }

        #endregion
    }
}
