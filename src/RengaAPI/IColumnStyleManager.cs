using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IColumnStyleManager interface
    /// </summary>
    public class IColumnStyleManager : IDisposable
    {
        private Renga.IColumnStyleManager _i;
        private bool _disposed = false;

        public Renga.IColumnStyleManager _i_Internal => _i;

        public IColumnStyleManager(object manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager), "ColumnStyleManager cannot be null");
            _i = manager as Renga.IColumnStyleManager;
            if (_i == null)
                throw new ArgumentException("Object does not implement IColumnStyleManager interface", nameof(manager));
        }

        public IColumnStyleManager(Renga.IColumnStyleManager manager)
        {
            _i = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        ~IColumnStyleManager()
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
        public bool Contains(int id)
        {
            if (_i == null) throw new InvalidOperationException("IColumnStyleManager interface is not initialized.");
            try { return _i.Contains(id); } catch (Exception ex) { throw new InvalidOperationException($"Failed to check Contains: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public List<int> GetIds()
        {
            if (_i == null) throw new InvalidOperationException("IColumnStyleManager interface is not initialized.");
            try { return _i.GetIds().OfType<int>().ToList(); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Ids: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public IColumnStyle GetColumnStyle(int id)
        {
            if (_i == null) throw new InvalidOperationException("IColumnStyleManager interface is not initialized.");
            try { return new IColumnStyle(_i.GetColumnStyle(id)); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get ColumnStyle: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}


