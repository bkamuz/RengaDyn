using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IParameterDefinition interface - Complete API Reference
    /// This class provides comprehensive access to all IParameterDefinition interface members
    /// </summary>
    public class IParameterDefinition : IDisposable
    {
        /// <summary>
        /// Internal COM object Renga.IParameterDefinition
        /// </summary>
        public Renga.IParameterDefinition _i;
        
        /// <summary>
        /// Flag for tracking resource disposal
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Constructor - Creates IParameterDefinition from existing IParameterDefinition interface
        /// </summary>
        /// <param name="parameterDefinition">Existing IParameterDefinition interface</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterDefinition(Renga.IParameterDefinition parameterDefinition)
        {
            if (parameterDefinition == null)
                throw new ArgumentNullException(nameof(parameterDefinition), "ParameterDefinition interface cannot be null.");
            
            this._i = parameterDefinition;
        }

        /// <summary>
        /// Constructor - Creates IParameterDefinition from existing ParameterDefinition
        /// </summary>
        /// <param name="parameterDefinition">Existing ParameterDefinition instance</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IParameterDefinition(DynProperties.Parameters.ParameterDefinition parameterDefinition)
        {
            if (parameterDefinition == null)
                throw new ArgumentNullException(nameof(parameterDefinition), "ParameterDefinition cannot be null.");
            
            if (parameterDefinition._i == null)
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            this._i = parameterDefinition._i;
        }

        /// <summary>
        /// Check if parameter definition interface is valid
        /// </summary>
        /// <returns>True if parameter definition interface is initialized correctly</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsValid()
        {
            return this._i != null;
        }

        #region IParameterDefinition Properties

        /// <summary>
        /// Gets the name of the parameter. For STDL parameters format: group_name.param_name. 
        /// Empty for buildin parameters, use Text to get description.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
                
                return this._i.Name;
            }
        }

        /// <summary>
        /// Gets the type of the parameter
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object ParameterType
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
                
                return this._i.ParameterType;
            }
        }

        /// <summary>
        /// Gets the localized text that describes the meaning of the parameter
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Text
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
                
                return this._i.Text;
            }
        }

        /// <summary>
        /// Gets user defined enumeration items
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object UserDefinedEnumerationItems
        {
            get
            {
                if (this._i == null) 
                    throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
                
                try
                {
                    return this._i.UserDefinedEnumerationItems;
                }
                catch (Exception ex)
                {
                    // Return null if enumeration items are not available
                    System.Diagnostics.Debug.WriteLine($"Failed to get user defined enumeration items: {ex.Message}");
                    return null;
                }
            }
        }

        #endregion

        #region Advanced ParameterDefinition Methods

        /// <summary>
        /// Gets the parameter type as string
        /// </summary>
        /// <returns>String representation of the parameter type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetParameterTypeAsString()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            try
            {
                switch (this._i.ParameterType)
                {
                    case Renga.ParameterType.ParameterType_Undefined:
                        return "Undefined";
                    case Renga.ParameterType.ParameterType_Bool:
                        return "Bool";
                    case Renga.ParameterType.ParameterType_Int:
                        return "Int";
                    case Renga.ParameterType.ParameterType_Double:
                        return "Double";
                    case Renga.ParameterType.ParameterType_String:
                        return "String";
                    case Renga.ParameterType.ParameterType_Length:
                        return "Length";
                    case Renga.ParameterType.ParameterType_Angle:
                        return "Angle";
                    case Renga.ParameterType.ParameterType_IntID:
                        return "IntID";
                    default:
                        return "Unknown";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter type as string: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the display name for the parameter (uses Text if Name is empty)
        /// </summary>
        /// <returns>Display name for the parameter</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetDisplayName()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            try
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name;
                
                if (!string.IsNullOrEmpty(Text))
                    return Text;
                
                return "Unknown Parameter";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get display name: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the parameter is a user-defined parameter
        /// </summary>
        /// <returns>True if parameter is user-defined</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsUserDefined()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            try
            {
                return !string.IsNullOrEmpty(Name) && Name.Contains(".");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if parameter is user-defined: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the parameter is a built-in parameter
        /// </summary>
        /// <returns>True if parameter is built-in</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsBuiltIn()
        {
            return !IsUserDefined();
        }

        /// <summary>
        /// Gets the group name for STDL parameters
        /// </summary>
        /// <returns>Group name or null if not applicable</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetGroupName()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            try
            {
                if (IsUserDefined() && !string.IsNullOrEmpty(Name))
                {
                    var parts = Name.Split('.');
                    if (parts.Length >= 2)
                        return parts[0];
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get group name: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the parameter name without group for STDL parameters
        /// </summary>
        /// <returns>Parameter name without group or full name if not applicable</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetParameterName()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            try
            {
                if (IsUserDefined() && !string.IsNullOrEmpty(Name))
                {
                    var parts = Name.Split('.');
                    if (parts.Length >= 2)
                        return parts[1];
                }
                
                return Name;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get parameter name: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets parameter definition information as a dictionary for debugging
        /// </summary>
        /// <returns>Dictionary with parameter definition information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, object> GetDefinitionInfo()
        {
            if (this._i == null) 
                throw new InvalidOperationException("ParameterDefinition interface is not initialized.");
            
            try
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", IsValid() },
                    { "Name", Name },
                    { "Text", Text },
                    { "DisplayName", GetDisplayName() },
                    { "ParameterType", ParameterType },
                    { "ParameterTypeAsString", GetParameterTypeAsString() },
                    { "IsUserDefined", IsUserDefined() },
                    { "IsBuiltIn", IsBuiltIn() },
                    { "GroupName", GetGroupName() },
                    { "ParameterName", GetParameterName() },
                    { "UserDefinedEnumerationItems", UserDefinedEnumerationItems }
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "IsValid", false },
                    { "Error", ex.Message },
                    { "Name", "Error" },
                    { "Text", "Error" },
                    { "DisplayName", "Error" },
                    { "ParameterType", null },
                    { "ParameterTypeAsString", "Error" },
                    { "IsUserDefined", false },
                    { "IsBuiltIn", false },
                    { "GroupName", null },
                    { "ParameterName", "Error" },
                    { "UserDefinedEnumerationItems", null }
                };
            }
        }

        /// <summary>
        /// Gets available parameter types
        /// </summary>
        /// <returns>Dictionary with parameter types</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ParameterType_Undefined", "ParameterType_Bool", "ParameterType_Int", 
            "ParameterType_Double", "ParameterType_String", "ParameterType_Length", 
            "ParameterType_Angle", "ParameterType_IntID" })]
        public static Dictionary<string, object> GetParameterTypes()
        {
            return new Dictionary<string, object>
            {
                { "ParameterType_Undefined", Renga.ParameterType.ParameterType_Undefined },
                { "ParameterType_Bool", Renga.ParameterType.ParameterType_Bool },
                { "ParameterType_Int", Renga.ParameterType.ParameterType_Int },
                { "ParameterType_Double", Renga.ParameterType.ParameterType_Double },
                { "ParameterType_String", Renga.ParameterType.ParameterType_String },
                { "ParameterType_Length", Renga.ParameterType.ParameterType_Length },
                { "ParameterType_Angle", Renga.ParameterType.ParameterType_Angle },
                { "ParameterType_IntID", Renga.ParameterType.ParameterType_IntID }
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
                            System.Diagnostics.Debug.WriteLine($"Error releasing IParameterDefinition COM object: {ex.Message}");
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
        ~IParameterDefinition()
        {
            Dispose(false);
        }

        #endregion
    }
}
