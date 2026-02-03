using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ITextObject interface.
    /// Represents a model text object. Obtain from IModelObject via GetInterfaceByName("ITextObject").
    /// </summary>
    public class ITextObject : IDisposable
    {
        private Renga.ITextObject _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object Renga.ITextObject
        /// </summary>
        public Renga.ITextObject _i_Internal => _i;

        /// <summary>
        /// Constructor - Creates ITextObject from COM object
        /// </summary>
        /// <param name="textObject">The Renga.ITextObject COM object</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ITextObject(object textObject)
        {
            if (textObject == null)
                throw new ArgumentNullException(nameof(textObject), "TextObject cannot be null");
            _i = textObject as Renga.ITextObject;
            if (_i == null)
                throw new ArgumentException("Object does not implement ITextObject interface", nameof(textObject));
        }

        /// <summary>
        /// Constructor - Creates ITextObject from typed COM object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ITextObject(Renga.ITextObject textObject)
        {
            _i = textObject ?? throw new ArgumentNullException(nameof(textObject));
        }

        ~ITextObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
        /// Returns the rich text document. Use to read or edit paragraph and token content.
        /// </summary>
        /// <returns>IRichTextDocument wrapper</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextDocument GetRichTextDocument()
        {
            if (_i == null)
                throw new InvalidOperationException("ITextObject interface is not initialized.");
            try
            {
                var doc = _i.GetRichTextDocument();
                return doc != null ? new IRichTextDocument(doc) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get rich text document: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the text bounding rect in object LCS as a dictionary: Left, Top, Right, Bottom (double).
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, double> BoundRect
        {
            get
            {
                if (_i == null)
                    throw new InvalidOperationException("ITextObject interface is not initialized.");
                try
                {
                    var r = _i.BoundRect;
                    return new Dictionary<string, double>
                    {
                        { "Left", r.Left },
                        { "Top", r.Top },
                        { "Right", r.Right },
                        { "Bottom", r.Bottom }
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get BoundRect: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Sets the text bounding rect in object LCS. Rect in object LCS (e.g. for ILevelObject in its Placement XOY plane).
        /// See https://help.rengabim.com/api/interface_i_text_object.html
        /// </summary>
        /// <param name="left">Left edge (X)</param>
        /// <param name="top">Top edge (Y)</param>
        /// <param name="right">Right edge (X)</param>
        /// <param name="bottom">Bottom edge (Y)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBoundRect(double left, double top, double right, double bottom)
        {
            if (_i == null)
                throw new InvalidOperationException("ITextObject interface is not initialized.");
            try
            {
                _i.BoundRect = new Renga.Rect
                {
                    Left = left,
                    Top = top,
                    Right = right,
                    Bottom = bottom
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set BoundRect: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}
