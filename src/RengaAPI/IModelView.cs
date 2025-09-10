using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IModelView interface - Complete API Reference
    /// This class provides comprehensive access to all IModelView interface members
    /// </summary>
    public class IModelView : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IModelView
        /// </summary>
        public Renga.IModelView _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IModelView from existing IModelView interface
        /// </summary>
        /// <param name="modelView">Existing IModelView interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelView(Renga.IModelView modelView)
        {
            if (modelView == null)
                throw new ArgumentNullException(nameof(modelView), "ModelView interface cannot be null.");
            
            this._i = modelView;
        }

        /// <summary>
        /// Constructor - Creates IModelView from existing IView interface
        /// </summary>
        /// <param name="view">Existing IView interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModelView(Renga.IView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view), "View interface cannot be null.");
            
            try
            {
                this._i = view as Renga.IModelView;
                if (this._i == null)
                    throw new InvalidOperationException("View is not a ModelView. Only model views are supported.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to cast view to IModelView: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if model view interface is valid
        /// </summary>
        /// <returns>True if model view interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IModelView Properties

        /// <summary>
        /// Gets the ID of the entity represented by the model view
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int RepresentedEntityId
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelView interface is not initialized.");
                
                return this._i.RepresentedEntityId;
            }
        }

        /// <summary>
        /// Gets or sets the view's default visual style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.VisualStyle VisualStyle
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelView interface is not initialized.");
                
                return this._i.VisualStyle;
            }
            set
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelView interface is not initialized.");
                
                this._i.VisualStyle = value;
            }
        }

        /// <summary>
        /// Gets the view ID (inherited from IView)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelView interface is not initialized.");
                
                return this._i.Id;
            }
        }

        /// <summary>
        /// Gets the actual type of the view (inherited from IView)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.ViewType Type
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ModelView interface is not initialized.");
                
                return this._i.Type;
            }
        }

        #endregion

        #region IModelView Methods

        /// <summary>
        /// Returns the object's visual style
        /// </summary>
        /// <param name="objectId">Object's identifier</param>
        /// <returns>Object's visual style</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.VisualStyle GetObjectVisualStyle(int objectId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            try
            {
                return this._i.GetObjectVisualStyle(objectId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get visual style for object {objectId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the object's visibility state
        /// </summary>
        /// <param name="objectId">Object's identifier</param>
        /// <returns>True if the object is visible, false otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsObjectVisible(int objectId)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            try
            {
                return this._i.IsObjectVisible(objectId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get visibility for object {objectId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Changes the visibility state of a set of objects
        /// </summary>
        /// <param name="objectIds">Array of object identifiers</param>
        /// <param name="visible">New visibility state</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetObjectsVisibility(List<int> objectIds, bool visible)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            try
            {
                this._i.SetObjectsVisibility(objectIds.ToArray(), visible);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set visibility for objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Changes the visual style of a set of objects
        /// </summary>
        /// <param name="objectIds">Array of object identifiers</param>
        /// <param name="visualStyle">New visual style</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetObjectsVisualStyle(List<int> objectIds, Renga.VisualStyle visualStyle)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            try
            {
                this._i.SetObjectsVisualStyle(objectIds.ToArray(), visualStyle);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set visual style for objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Show objects by a collection of their IDs (implemented only for 3D view)
        /// </summary>
        /// <param name="objectIds">Array of IDs of objects to show</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void ShowObjects(List<int> objectIds)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            try
            {
                this._i.ShowObjects(objectIds.ToArray());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to show objects: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced ModelView Methods

        /// <summary>
        /// Sets visibility for a single object
        /// </summary>
        /// <param name="objectId">Object identifier</param>
        /// <param name="visible">Visibility state</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetObjectVisibility(int objectId, bool visible)
        {
            SetObjectsVisibility(new List<int> { objectId }, visible);
        }

        /// <summary>
        /// Sets visual style for a single object
        /// </summary>
        /// <param name="objectId">Object identifier</param>
        /// <param name="visualStyle">Visual style</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetObjectVisualStyle(int objectId, Renga.VisualStyle visualStyle)
        {
            SetObjectsVisualStyle(new List<int> { objectId }, visualStyle);
        }

        /// <summary>
        /// Shows a single object
        /// </summary>
        /// <param name="objectId">Object identifier</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void ShowObject(int objectId)
        {
            ShowObjects(new List<int> { objectId });
        }

        /// <summary>
        /// Hides a single object
        /// </summary>
        /// <param name="objectId">Object identifier</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void HideObject(int objectId)
        {
            SetObjectVisibility(objectId, false);
        }

        /// <summary>
        /// Gets visibility state for multiple objects
        /// </summary>
        /// <param name="objectIds">List of object identifiers</param>
        /// <returns>Dictionary with object ID and visibility state</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, bool> GetObjectsVisibility(List<int> objectIds)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            try
            {
                var result = new Dictionary<int, bool>();
                foreach (var id in objectIds)
                {
                    result[id] = IsObjectVisible(id);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get visibility for objects: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets visual style for multiple objects
        /// </summary>
        /// <param name="objectIds">List of object identifiers</param>
        /// <returns>Dictionary with object ID and visual style</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, Renga.VisualStyle> GetObjectsVisualStyle(List<int> objectIds)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            if (objectIds == null)
                throw new ArgumentNullException(nameof(objectIds), "Object IDs list cannot be null.");
            
            try
            {
                var result = new Dictionary<int, Renga.VisualStyle>();
                foreach (var id in objectIds)
                {
                    result[id] = GetObjectVisualStyle(id);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get visual style for objects: {ex.Message}", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets model view information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with model view information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetModelViewInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ModelView interface is not initialized.");
            
            try
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Id", Id },
                    { "Type", Type.ToString() },
                    { "RepresentedEntityId", RepresentedEntityId },
                    { "VisualStyle", VisualStyle.ToString() }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "Id", -1 },
                    { "Type", "Error" },
                    { "RepresentedEntityId", -1 },
                    { "VisualStyle", "Error" }
                };
            }
        }

        /// <summary>
        /// Gets available visual styles for reference
        /// </summary>
        /// <returns>List of available visual style names</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<string> GetAvailableVisualStyles()
        {
            return new List<string>
            {
                "VisualStyle_Shaded",
                "VisualStyle_Wireframe",
                "VisualStyle_Monochrome",
                "VisualStyle_Transparent",
                "VisualStyle_XRay"
            };
        }

        /// <summary>
        /// Gets available view types for reference
        /// </summary>
        /// <returns>List of available view type names</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static List<string> GetAvailableViewTypes()
        {
            return new List<string>
            {
                "ViewType_Plan",
                "ViewType_Elevation",
                "ViewType_Section",
                "ViewType_3D",
                "ViewType_Detail",
                "ViewType_Schedule"
            };
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IModelView COM object: {ex.Message}");
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
        ~IModelView()
        {
            Dispose(false);
        }

        #endregion
    }
}
