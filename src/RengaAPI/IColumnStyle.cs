using System;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IColumnStyle interface
    /// </summary>
    public class IColumnStyle : IDisposable
    {
        private Renga.IColumnStyle _i;
        private bool _disposed = false;

        public Renga.IColumnStyle _i_Internal => _i;

        public IColumnStyle(object style)
        {
            if (style == null)
                throw new ArgumentNullException(nameof(style), "ColumnStyle cannot be null");
            _i = style as Renga.IColumnStyle;
            if (_i == null)
                throw new ArgumentException("Object does not implement IColumnStyle interface", nameof(style));
        }

        public IColumnStyle(Renga.IColumnStyle style)
        {
            _i = style ?? throw new ArgumentNullException(nameof(style));
        }

        ~IColumnStyle()
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

        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnStyle interface is not initialized.");
                try { return _i.Id; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Id: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnStyle interface is not initialized.");
                try { return _i.Name; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Name: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public IProfile Profile
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IColumnStyle interface is not initialized.");
                try { return new IProfile(_i.Profile); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Profile: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}


