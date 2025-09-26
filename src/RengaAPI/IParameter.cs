using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IParameter interface - Complete API Reference
    /// This class provides comprehensive access to all IParameter interface members
    /// </summary>
    public class IParameter : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IParameter
        /// </summary>
        public Renga.IParameter _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IParameter from existing IParameter interface
        /// </summary>
        /// <param name="parameter">Existing IParameter interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameter(Renga.IParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Parameter interface cannot be null.");
            
            this._i = parameter;
        }

        /// <summary>
        /// Constructor - Creates IParameter from existing Parameter
        /// </summary>
        /// <param name="parameter">Existing Parameter instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameter(DynProperties.Parameters.Parameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Parameter cannot be null.");
            
            if (parameter._i == null)
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            this._i = parameter._i;
        }

        /// <summary>
        /// Check if parameter interface is valid
        /// </summary>
        /// <returns>True if parameter interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IParameter Properties

        /// <summary>
        /// Gets the unique identifier of the parameter
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Guid Id
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Parameter interface is not initialized.");
                
                return this._i.Id;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the parameter as string
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string IdS
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Parameter interface is not initialized.");
                
                return this._i.IdS;
            }
        }

        /// <summary>
        /// Gets the definition of the parameter
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterDefinition Definition
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Parameter interface is not initialized.");
                
                return new IParameterDefinition(this._i.Definition);
            }
        }

        /// <summary>
        /// Indicates if the parameter has a value
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool HasValue
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Parameter interface is not initialized.");
                
                return this._i.HasValue;
            }
        }

        /// <summary>
        /// Indicates if the parameter is read only
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsReadOnly
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Parameter interface is not initialized.");
                
                return this._i.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets the value type of the parameter
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object ValueType
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("Parameter interface is not initialized.");
                
                return this._i.ValueType;
            }
        }

        #endregion

        #region IParameter Methods - Get Values

        /// <summary>
        /// Returns the parameter value as bool value
        /// </summary>
        /// <returns>Boolean value of the parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool GetBoolValue()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            try
            {
                return this._i.GetBoolValue();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get boolean value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the parameter value as a double value
        /// </summary>
        /// <returns>Double value of the parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public double GetDoubleValue()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            try
            {
                return this._i.GetDoubleValue();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get double value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the parameter value as an integer value
        /// </summary>
        /// <returns>Integer value of the parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetIntValue()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            try
            {
                return this._i.GetIntValue();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get integer value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Returns the parameter value as a string value
        /// </summary>
        /// <returns>String value of the parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetStringValue()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            try
            {
                return this._i.GetStringValue();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get string value: {ex.Message}", ex);
            }
        }

        #endregion

        #region IParameter Methods - Set Values

        /// <summary>
        /// Sets the parameter value as bool value
        /// </summary>
        /// <param name="value">Boolean value to set</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBoolValue(bool value)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            if (IsReadOnly)
                throw new InvalidOperationException("Cannot set value on read-only parameter.");
            
            try
            {
                this._i.SetBoolValue(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set boolean value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the parameter value as a double value
        /// </summary>
        /// <param name="value">Double value to set</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetDoubleValue(double value)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            if (IsReadOnly)
                throw new InvalidOperationException("Cannot set value on read-only parameter.");
            
            try
            {
                this._i.SetDoubleValue(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set double value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the parameter value as an integer value
        /// </summary>
        /// <param name="value">Integer value to set</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetIntValue(int value)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            if (IsReadOnly)
                throw new InvalidOperationException("Cannot set value on read-only parameter.");
            
            try
            {
                this._i.SetIntValue(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set integer value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets the parameter value as a string value
        /// </summary>
        /// <param name="value">String value to set</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetStringValue(string value)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            if (IsReadOnly)
                throw new InvalidOperationException("Cannot set value on read-only parameter.");
            
            try
            {
                this._i.SetStringValue(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set string value: {ex.Message}", ex);
            }
        }

        #endregion

        #region Advanced Parameter Methods

        /// <summary>
        /// Gets parameter value based on its value type
        /// </summary>
        /// <returns>Parameter value as object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetValue()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            if (!HasValue)
                return null;
            
            try
            {
                switch (this._i.ValueType)
                {
                    case ParameterValueType.ParameterValueType_Bool:
                        return GetBoolValue();
                    case ParameterValueType.ParameterValueType_Int:
                        return GetIntValue();
                    case ParameterValueType.ParameterValueType_Double:
                        return GetDoubleValue();
                    case ParameterValueType.ParameterValueType_String:
                        return GetStringValue();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets parameter value based on its value type
        /// </summary>
        /// <param name="value">Value to set</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetValue(object value)
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            if (IsReadOnly)
                throw new InvalidOperationException("Cannot set value on read-only parameter.");
            
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            
            try
            {
                switch (this._i.ValueType)
                {
                    case ParameterValueType.ParameterValueType_Bool:
                        SetBoolValue(Convert.ToBoolean(value));
                        break;
                    case ParameterValueType.ParameterValueType_Int:
                        SetIntValue(Convert.ToInt32(value));
                        break;
                    case ParameterValueType.ParameterValueType_Double:
                        SetDoubleValue(Convert.ToDouble(value));
                        break;
                    case ParameterValueType.ParameterValueType_String:
                        SetStringValue(Convert.ToString(value));
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported parameter value type: {this._i.ValueType}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set parameter value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the value type as string
        /// </summary>
        /// <returns>String representation of the value type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetValueTypeAsString()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            try
            {
                switch (this._i.ValueType)
                {
                    case ParameterValueType.ParameterValueType_Undefined:
                        return "Undefined";
                    case ParameterValueType.ParameterValueType_Bool:
                        return "Bool";
                    case ParameterValueType.ParameterValueType_Int:
                        return "Int";
                    case ParameterValueType.ParameterValueType_Double:
                        return "Double";
                    case ParameterValueType.ParameterValueType_String:
                        return "String";
                    default:
                        return "Unknown";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get value type as string: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets parameter information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with parameter information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetParameterInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("Parameter interface is not initialized.");
            
            try
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Id", Id },
                    { "IdS", IdS },
                    { "HasValue", HasValue },
                    { "IsReadOnly", IsReadOnly },
                    { "ValueType", ValueType },
                    { "ValueTypeAsString", GetValueTypeAsString() },
                    { "Value", HasValue ? GetValue() : null },
                    { "Definition", Definition }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "Id", Guid.Empty },
                    { "IdS", "Error" },
                    { "HasValue", false },
                    { "IsReadOnly", false },
                    { "ValueType", null },
                    { "ValueTypeAsString", "Error" },
                    { "Value", null },
                    { "Definition", null }
                };
            }
        }

        /// <summary>
        /// Gets available parameter value types
        /// </summary>
        /// <returns>Dictionary with parameter value types</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ParameterValueType_Undefined", "ParameterValueType_Bool", 
            "ParameterValueType_Int", "ParameterValueType_Double", "ParameterValueType_String" })]
        public static Dictionary<string, object> GetParameterValueTypes()
        {
            return new Dictionary<string, object>
            {
                { "ParameterValueType_Undefined", ParameterValueType.ParameterValueType_Undefined },
                { "ParameterValueType_Bool", ParameterValueType.ParameterValueType_Bool },
                { "ParameterValueType_Int", ParameterValueType.ParameterValueType_Int },
                { "ParameterValueType_Double", ParameterValueType.ParameterValueType_Double },
                { "ParameterValueType_String", ParameterValueType.ParameterValueType_String }
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IParameter COM object: {ex.Message}");
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
        ~IParameter()
        {
            Dispose(false);
        }

        #endregion
    }
}
