using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga ILine3DParams interface - Complete API Reference
    /// This class provides comprehensive access to all ILine3DParams interface members
    /// </summary>
    public class ILine3DParams : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.ILine3DParams
        /// </summary>
        public Renga.ILine3DParams _i;

        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates ILine3DParams from existing ILine3DParams interface
        /// </summary>
        /// <param name="line3DParams">Existing ILine3DParams interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ILine3DParams(Renga.ILine3DParams line3DParams)
        {
            if (line3DParams == null)
                throw new ArgumentNullException(nameof(line3DParams), "Line3DParams interface cannot be null.");
            
            this._i = line3DParams;
        }

        /// <summary>
        /// Constructor - Creates ILine3DParams from existing ModelObject
        /// </summary>
        /// <param name="modelObject">Existing ModelObject instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ILine3DParams(DynObjects.ModelObject modelObject)
        {
            if (modelObject == null)
                throw new ArgumentNullException(nameof(modelObject), "ModelObject cannot be null.");
            
            if (modelObject._i == null)
                throw new InvalidOperationException("ModelObject interface is not initialized.");
            
            this._i = modelObject._i as Renga.ILine3DParams;
            if (this._i == null)
                throw new InvalidOperationException("ModelObject does not support ILine3DParams interface.");
        }

        #region Properties

        /// <summary>
        /// Gets the color of the line
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Other.Renga_Color Color
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Line3DParams interface is not initialized.");
                
                return new Other.Renga_Color(this._i.Color);
            }
        }

        /// <summary>
        /// Gets the line style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Style
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Line3DParams interface is not initialized.");
                
                return this._i.Style;
            }
        }

        /// <summary>
        /// Gets the vertical offset of the object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double VerticalOffset
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Line3DParams interface is not initialized.");
                
                return this._i.VerticalOffset;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the 3D baseline of the line
        /// </summary>
        /// <returns>ICurve3D object representing the baseline</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public ICurve3D GetBaseline()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Line3DParams interface is not initialized.");
            
            try
            {
                var baseline = this._i.GetBaseline();
                return new ICurve3D(baseline);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get baseline: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the line style as a string representation
        /// </summary>
        /// <returns>String representation of the line style</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetStyleAsString()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Line3DParams interface is not initialized.");
            
            try
            {
                var style = (Renga.Line3DStyle)this._i.Style;
                return GetLine3DStyleAsString(style);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get style as string: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts Line3DStyle enum to string representation
        /// </summary>
        /// <param name="style">Line3DStyle enum value</param>
        /// <returns>String representation of the style</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetLine3DStyleAsString(Renga.Line3DStyle style)
        {
            switch (style)
            {
                case Renga.Line3DStyle.Line3DStyle_None:
                    return "None";
                case Renga.Line3DStyle.Line3DStyle_Solid:
                    return "Solid";
                case Renga.Line3DStyle.Line3DStyle_Dash:
                    return "Dash";
                case Renga.Line3DStyle.Line3DStyle_DashDot:
                    return "DashDot";
                case Renga.Line3DStyle.Line3DStyle_Dot:
                    return "Dot";
                case Renga.Line3DStyle.Line3DStyle_SpecialDash:
                    return "SpecialDash";
                case Renga.Line3DStyle.Line3DStyle_SpecialDashDot:
                    return "SpecialDashDot";
                case Renga.Line3DStyle.Line3DStyle_SpecialDot:
                    return "SpecialDot";
                case Renga.Line3DStyle.Line3DStyle_DashDotDot:
                    return "DashDotDot";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Gets all available Line3DStyle values as a dictionary
        /// </summary>
        /// <returns>Dictionary of Line3DStyle values</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, Renga.Line3DStyle> GetLine3DStyles()
        {
            return new Dictionary<string, Renga.Line3DStyle>
            {
                { "None", Renga.Line3DStyle.Line3DStyle_None },
                { "Solid", Renga.Line3DStyle.Line3DStyle_Solid },
                { "Dash", Renga.Line3DStyle.Line3DStyle_Dash },
                { "DashDot", Renga.Line3DStyle.Line3DStyle_DashDot },
                { "Dot", Renga.Line3DStyle.Line3DStyle_Dot },
                { "SpecialDash", Renga.Line3DStyle.Line3DStyle_SpecialDash },
                { "SpecialDashDot", Renga.Line3DStyle.Line3DStyle_SpecialDashDot },
                { "SpecialDot", Renga.Line3DStyle.Line3DStyle_SpecialDot },
                { "DashDotDot", Renga.Line3DStyle.Line3DStyle_DashDotDot }
            };
        }

        /// <summary>
        /// Gets comprehensive information about the line3D parameters
        /// </summary>
        /// <returns>Dictionary containing all parameter information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Line3DParams interface is not initialized.");
            
            try
            {
                var info = new Dictionary<string, object>
                {
                    { "Color", this.Color },
                    { "Style", this.Style },
                    { "StyleAsString", GetStyleAsString() },
                    { "VerticalOffset", this.VerticalOffset }
                };

                return info;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get line3D parameters info: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets debug information about the object
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDebugInfo()
        {
            if (this._i == null) 
                return "Line3DParams interface is not initialized.";
            
            try
            {
                var info = GetInfo();
                var debugInfo = new System.Text.StringBuilder();
                debugInfo.AppendLine("=== ILine3DParams Debug Info ===");
                debugInfo.AppendLine($"Color: {info["Color"]}");
                debugInfo.AppendLine($"Style: {info["StyleAsString"]}");
                debugInfo.AppendLine($"VerticalOffset: {info["VerticalOffset"]}");
                
                return debugInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting debug info: {ex.Message}";
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of the resources used by this object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(false)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
        [dr.IsVisibleInDynamoLibrary(false)]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here if any
                }
                
                // Release COM object
                if (this._i != null)
                {
                    Marshal.ReleaseComObject(this._i);
                    this._i = null;
                }
                
                _disposed = true;
            }
        }
        
        /// <summary>
        /// Finalizer for releasing unmanaged resources
        /// </summary>
        ~ILine3DParams()
        {
            Dispose(false);
        }

        #endregion
    }
}
