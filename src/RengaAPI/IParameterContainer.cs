using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IParameterContainer interface - Complete API Reference
    /// This class provides comprehensive access to all IParameterContainer interface members
    /// </summary>
    public class IParameterContainer : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IParameterContainer
        /// </summary>
        public Renga.IParameterContainer _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IParameterContainer from existing IParameterContainer interface
        /// </summary>
        /// <param name="parameterContainer">Existing IParameterContainer interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterContainer(Renga.IParameterContainer parameterContainer)
        {
            if (parameterContainer == null)
                throw new ArgumentNullException(nameof(parameterContainer), "ParameterContainer interface cannot be null.");
            
            this._i = parameterContainer;
        }

        /// <summary>
        /// Constructor - Creates IParameterContainer from existing ParameterContainer
        /// </summary>
        /// <param name="parameterContainer">Existing ParameterContainer instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterContainer(DynProperties.Parameters.ParameterContainer parameterContainer)
        {
            if (parameterContainer == null)
                throw new ArgumentNullException(nameof(parameterContainer), "ParameterContainer cannot be null.");
            
            if (parameterContainer._i == null)
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            this._i = parameterContainer._i;
        }

        /// <summary>
        /// Check if parameter container interface is valid
        /// </summary>
        /// <returns>True if parameter container interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IParameterContainer Methods

        /// <summary>
        /// Indicates whether the object supports the parameter specified by the identifier
        /// </summary>
        /// <param name="id">Identifier of the parameter</param>
        /// <returns>True if the object supports the parameter; otherwise returns False</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool Contains(Guid id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                return this._i.Contains(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if parameter exists: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Same as Contains, but accepting GUID as a string
        /// </summary>
        /// <param name="id">String identifier of the parameter</param>
        /// <returns>True if the object supports the parameter; otherwise returns False</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool ContainsS(string id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Parameter ID cannot be null or empty", nameof(id));
            
            try
            {
                return this._i.ContainsS(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if parameter exists by string ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a parameter by its id
        /// </summary>
        /// <param name="id">Identifier of the parameter</param>
        /// <returns>The parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameter Get(Guid id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                var parameter = this._i.Get(id);
                if (parameter == null)
                    return null;
                
                return new IParameter(parameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter by ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Same as Get, but accepting GUID as a string
        /// </summary>
        /// <param name="id">String identifier of the parameter</param>
        /// <returns>The parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameter GetS(string id)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Parameter ID cannot be null or empty", nameof(id));
            
            try
            {
                var parameter = this._i.GetS(id);
                if (parameter == null)
                    return null;
                
                return new IParameter(parameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter by string ID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the identifiers of parameters in the collection
        /// </summary>
        /// <returns>The collection of guids</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Other.GuidCollection GetIds()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                var guidCollection = this._i.GetIds();
                if (guidCollection == null)
                    return null;
                
                return new Other.GuidCollection(guidCollection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter IDs: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced ParameterContainer Methods

        /// <summary>
        /// Gets all parameters in the container
        /// </summary>
        /// <returns>List of all parameters</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IParameter> GetAllParameters()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                var parameters = new List<IParameter>();
                var guidCollection = GetIds();
                
                if (guidCollection != null)
                {
                    var guids = guidCollection.GetGuids();
                    foreach (var id in guids)
                    {
                        var parameter = Get(id);
                        if (parameter != null)
                        {
                            parameters.Add(parameter);
                        }
                    }
                }
                
                return parameters;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all parameters: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets parameters with their IDs as a dictionary
        /// </summary>
        /// <returns>Dictionary with parameter IDs and parameters</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ParameterIds", "Parameters" })]
        public Dictionary<string, object> GetParametersWithIds()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                var parameterIds = new List<Guid>();
                var parameters = new List<IParameter>();
                var guidCollection = GetIds();
                
                if (guidCollection != null)
                {
                    var guids = guidCollection.GetGuids();
                    foreach (var id in guids)
                    {
                        var parameter = Get(id);
                        if (parameter != null)
                        {
                            parameterIds.Add(id);
                            parameters.Add(parameter);
                        }
                    }
                }
                
                return new Dictionary<string, object>
                {
                    { "ParameterIds", parameterIds },
                    { "Parameters", parameters }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameters with IDs: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the count of parameters in the container
        /// </summary>
        /// <returns>Number of parameters</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetParameterCount()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                var guidCollection = GetIds();
                return guidCollection?.Count ?? 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter count: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the container has any parameters
        /// </summary>
        /// <returns>True if container has parameters</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool HasParameters()
        {
            return GetParameterCount() > 0;
        }

        /// <summary>
        /// Gets parameter information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with parameter container information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetContainerInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            try
            {
                var parameterCount = GetParameterCount();
                var hasParameters = HasParameters();
                
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "ParameterCount", parameterCount },
                    { "HasParameters", hasParameters },
                    { "ParameterIds", GetIds() }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "ParameterCount", 0 },
                    { "HasParameters", false },
                    { "ParameterIds", null }
                };
            }
        }

        /// <summary>
        /// Finds parameters by name pattern
        /// </summary>
        /// <param name="namePattern">Name pattern to search for (case insensitive)</param>
        /// <returns>List of matching parameters</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IParameter> FindParametersByName(string namePattern)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            if (string.IsNullOrEmpty(namePattern))
                throw new ArgumentException("Name pattern cannot be null or empty", nameof(namePattern));
            
            try
            {
                var matchingParameters = new List<IParameter>();
                var allParameters = GetAllParameters();
                
                foreach (var parameter in allParameters)
                {
                    try
                    {
                        var definition = parameter.Definition;
                        if (definition != null && !string.IsNullOrEmpty(definition.Name))
                        {
                            if (definition.Name.IndexOf(namePattern, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                matchingParameters.Add(parameter);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip parameters that can't be accessed
                        System.Diagnostics.Debug.WriteLine($"Failed to check parameter name: {ex.Message}");
                    }
                }
                
                return matchingParameters;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to find parameters by name: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets parameters by value type
        /// </summary>
        /// <param name="valueType">Value type to filter by</param>
        /// <returns>List of parameters with the specified value type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<IParameter> GetParametersByValueType(object valueType)
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterContainer interface is not initialized.");
            
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType), "Value type cannot be null.");
            
            try
            {
                var matchingParameters = new List<IParameter>();
                var allParameters = GetAllParameters();
                
                foreach (var parameter in allParameters)
                {
                    try
                    {
                        if (parameter.ValueType != null && parameter.ValueType.Equals(valueType))
                        {
                            matchingParameters.Add(parameter);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip parameters that can't be accessed
                        System.Diagnostics.Debug.WriteLine($"Failed to check parameter value type: {ex.Message}");
                    }
                }
                
                return matchingParameters;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameters by value type: {ex.Message}", ex);
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IParameterContainer COM object: {ex.Message}");
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
        ~IParameterContainer()
        {
            Dispose(false);
        }

        #endregion
    }
}
