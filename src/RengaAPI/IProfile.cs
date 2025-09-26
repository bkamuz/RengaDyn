using System;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IProfile interface
    /// </summary>
    public class IProfile : IDisposable
    {
        private Renga.IProfile _i;
        private bool _disposed = false;

        public Renga.IProfile _i_Internal => _i;

        public IProfile(object profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile), "Profile cannot be null");
            _i = profile as Renga.IProfile;
            if (_i == null)
                throw new ArgumentException("Object does not implement IProfile interface", nameof(profile));
        }

        public IProfile(Renga.IProfile profile)
        {
            _i = profile ?? throw new ArgumentNullException(nameof(profile));
        }

        ~IProfile()
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
        public int DescriptionId
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IProfile interface is not initialized.");
                try { return _i.DescriptionId; } catch (Exception ex) { throw new InvalidOperationException($"Failed to get DescriptionId: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterContainer Parameters
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IProfile interface is not initialized.");
                try { return new IParameterContainer(_i.Parameters); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Parameters: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public IRegion2DCollection Regions
        {
            get
            {
                if (_i == null) throw new InvalidOperationException("IProfile interface is not initialized.");
                try { return new IRegion2DCollection(_i.Regions); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get Regions: {ex.Message}", ex); }
            }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public Renga.Point2D GetCenterOfMass()
        {
            if (_i == null) throw new InvalidOperationException("IProfile interface is not initialized.");
            try { return _i.GetCenterOfMass(); } catch (Exception ex) { throw new InvalidOperationException($"Failed to get center of mass: {ex.Message}", ex); }
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}


