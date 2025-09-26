using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IProfileDescriptionManager interface
    /// </summary>
    public class IProfileDescriptionManager : IDisposable
    {
        private Renga.IProfileDescriptionManager _i;
        private bool _disposed = false;

        public Renga.IProfileDescriptionManager _i_Internal => _i;

        public IProfileDescriptionManager(object manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager), "ProfileDescriptionManager cannot be null");
            _i = manager as Renga.IProfileDescriptionManager;
            if (_i == null)
                throw new ArgumentException("Object does not implement IProfileDescriptionManager interface", nameof(manager));
        }

        public IProfileDescriptionManager(Renga.IProfileDescriptionManager manager)
        {
            _i = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        ~IProfileDescriptionManager()
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
            if (_i == null) throw new InvalidOperationException("IProfileDescriptionManager interface is not initialized.");
            try { return _i.Contains(id); } catch (Exception ex) { throw new InvalidOperationException($"Failed to check Contains: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public List<int> GetIds()
        {
            if (_i == null) throw new InvalidOperationException("IProfileDescriptionManager interface is not initialized.");
            try { return _i.GetIds().OfType<int>().ToList(); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Ids: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public IProfileDescription GetProfileDescription(int id)
        {
            if (_i == null) throw new InvalidOperationException("IProfileDescriptionManager interface is not initialized.");
            try { return new IProfileDescription(_i.GetProfileDescription(id)); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get ProfileDescription: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}


