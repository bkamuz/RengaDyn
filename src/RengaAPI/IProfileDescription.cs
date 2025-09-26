using System;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IProfileDescription interface
    /// </summary>
    public class IProfileDescription : IDisposable
    {
        private Renga.IProfileDescription _i;
        private bool _disposed = false;

        public Renga.IProfileDescription _i_Internal => _i;

        public IProfileDescription(object profileDescription)
        {
            if (profileDescription == null)
                throw new ArgumentNullException(nameof(profileDescription), "ProfileDescription cannot be null");
            _i = profileDescription as Renga.IProfileDescription;
            if (_i == null)
                throw new ArgumentException("Object does not implement IProfileDescription interface", nameof(profileDescription));
        }

        public IProfileDescription(Renga.IProfileDescription profileDescription)
        {
            _i = profileDescription ?? throw new ArgumentNullException(nameof(profileDescription));
        }

        ~IProfileDescription()
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
                if (_i == null) throw new InvalidOperationException("IProfileDescription interface is not initialized.");
                try { return _i.Id; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Id: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IProfileDescription interface is not initialized.");
                try { return _i.Name; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Name: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}


