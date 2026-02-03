using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using DynRenga.DynDocument.Project;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IDrawing interface.
    /// Represents an architectural drawing. Export to DWG, DXF, PDF, OpenXPS; access title blocks and drawing model.
    /// API: https://help.rengabim.com/api/interface_i_drawing.html
    /// </summary>
    public class IDrawing : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IDrawing
        /// </summary>
        public Renga.IDrawing _i;

        /// <summary>
        /// When _i is null (e.g. drawing from Drawings2 as IEntityCollection), store raw entity for GetModel()
        /// </summary>
        internal object _entityFallback;

        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IDrawing from COM object (Renga.IDrawing or Renga.IEntity for Drawings2)
        /// </summary>
        /// <param name="drawingObject">Renga.IDrawing or Renga.IEntity (Drawings2) COM object</param>
        internal IDrawing(object drawingObject)
        {
            if (drawingObject == null)
                throw new ArgumentNullException(nameof(drawingObject), "Drawing object cannot be null.");
            _i = drawingObject as Renga.IDrawing;
            if (_i == null && drawingObject is Renga.IEntity)
                _entityFallback = drawingObject;
            else if (_i == null)
                throw new InvalidCastException("The provided object cannot be cast to Renga.IDrawing or Renga.IEntity.");
        }

        private void EnsureDrawing()
        {
            if (_i == null)
                throw new InvalidOperationException("Drawing interface is not initialized (entity-only fallback). Use GetModel() for model access.");
        }

        /// <summary>
        /// The Id of the drawing (GUID)
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid Id
        {
            get
            {
                EnsureDrawing();
                try { return _i.Id; }
                catch (Exception ex) { throw new InvalidOperationException($"Failed to get Id: {ex.Message}", ex); }
            }
        }

        /// <summary>
        /// The Id of the drawing as a string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string IdS
        {
            get
            {
                EnsureDrawing();
                try { return _i.IdS; }
                catch (Exception ex) { throw new InvalidOperationException($"Failed to get IdS: {ex.Message}", ex); }
            }
        }

        /// <summary>
        /// The name of the drawing
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                EnsureDrawing();
                try { return _i.Name; }
                catch (Exception ex) { throw new InvalidOperationException($"Failed to get Name: {ex.Message}", ex); }
            }
        }

        /// <summary>
        /// The drawing title block instance count
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int TitleBlockInstanceCount
        {
            get
            {
                EnsureDrawing();
                try { return _i.TitleBlockInstanceCount; }
                catch (Exception ex) { throw new InvalidOperationException($"Failed to get TitleBlockInstanceCount: {ex.Message}", ex); }
            }
        }

        /// <summary>
        /// Returns the drawing title block instance by index (0-based)
        /// </summary>
        /// <param name="index">Zero-based index</param>
        /// <returns>TitleBlockInstance wrapper (DynDocument.Project)</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public TitleBlockInstance GetTitleBlockInstance(int index)
        {
            EnsureDrawing();
            if (index < 0 || index >= TitleBlockInstanceCount)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. TitleBlockInstanceCount = {TitleBlockInstanceCount}.");
            try
            {
                var rengaInstance = _i.GetTitleBlockInstance(index);
                return new TitleBlockInstance(rengaInstance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get TitleBlockInstance at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all title block instances on the drawing
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<TitleBlockInstance> GetTitleBlockInstances()
        {
            EnsureDrawing();
            var list = new List<TitleBlockInstance>();
            for (int i = 0; i < TitleBlockInstanceCount; i++)
                list.Add(GetTitleBlockInstance(i));
            return list;
        }

        /// <summary>
        /// Exports the drawing to DWG file format.
        /// </summary>
        /// <param name="filePath">The file path to export to</param>
        /// <param name="autocadVersion">AutoCAD format version (use AcadExportFormats node: 0=v2000, 1=v2004, 2=v2007, 3=v2010, 4=v2013, 5=v2018)</param>
        /// <param name="overwrite">Overwrite the file if it exists</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ExportToDwg(string filePath, int autocadVersion, bool overwrite)
        {
            EnsureDrawing();
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath cannot be null or empty.", nameof(filePath));
            try
            {
                var version = (Renga.AutocadVersion)autocadVersion;
                return _i.ExportToDwg(filePath, version, overwrite);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export to DWG: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exports the drawing to DXF file format.
        /// </summary>
        /// <param name="filePath">The file path to export to</param>
        /// <param name="autocadVersion">AutoCAD format version (use AcadExportFormats node)</param>
        /// <param name="overwrite">Overwrite the file if it exists</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ExportToDxf(string filePath, int autocadVersion, bool overwrite)
        {
            EnsureDrawing();
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath cannot be null or empty.", nameof(filePath));
            try
            {
                var version = (Renga.AutocadVersion)autocadVersion;
                return _i.ExportToDxf(filePath, version, overwrite);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export to DXF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exports the drawing to PDF file format.
        /// </summary>
        /// <param name="filePath">The file path to export to</param>
        /// <param name="overwrite">Overwrite the file if it exists</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ExportToPdf(string filePath, bool overwrite)
        {
            EnsureDrawing();
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath cannot be null or empty.", nameof(filePath));
            try
            {
                return _i.ExportToPdf(filePath, overwrite);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exports the drawing to OpenXPS file format.
        /// </summary>
        /// <param name="filePath">The file path to export to</param>
        /// <param name="overwrite">Overwrite the file if it exists</param>
        /// <returns>Zero if successful, non-zero otherwise</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ExportToOpenXps(string filePath, bool overwrite)
        {
            EnsureDrawing();
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("filePath cannot be null or empty.", nameof(filePath));
            try
            {
                return _i.ExportToOpenXps(filePath, overwrite);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export to OpenXPS: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the drawing model (IModel) for this drawing.
        /// Use to create drawing objects (DrawingText, DrawingImage, etc.) on the sheet.
        /// When created from Drawings2 (IEntityCollection), gets IModel from the entity.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IModel GetModel()
        {
            Renga.IModel rengaModel = null;
            if (_i != null)
            {
                rengaModel = _i as Renga.IModel;
                if (rengaModel == null && _i is Renga.IEntity entity)
                {
                    var modelObj = entity.GetInterfaceByName("IModel");
                    rengaModel = modelObj as Renga.IModel;
                }
            }
            else if (_entityFallback is Renga.IEntity entity)
            {
                var modelObj = entity.GetInterfaceByName("IModel");
                rengaModel = modelObj as Renga.IModel;
            }
            if (rengaModel != null)
                return new IModel(rengaModel);
            throw new InvalidOperationException("Drawing interface is not initialized. Use drawings from Drawings2 for GetModel().");
        }

        /// <summary>
        /// Disposes the Renga.IDrawing COM object (does not release entity fallback)
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

        ~IDrawing()
        {
            Dispose();
        }
    }
}
