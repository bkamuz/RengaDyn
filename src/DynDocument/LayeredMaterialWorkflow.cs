using System;
using System.Collections.Generic;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Simplified wrapper for layered material operations in Dynamo
    /// This class provides the essential functionality to work with layered materials
    /// </summary>
    public class LayeredMaterialWorkflow
    {
        #region Constants

        /// <summary>
        /// GUID for layered material style parameter - CRITICAL for setting layered materials
        /// </summary>
        public static string LayeredMaterialGuid = "2cbc4964-61a0-4553-8103-36e2dd2ab31c";
        
        /// <summary>
        /// GUID for thickness parameter
        /// </summary>
        public static string ThicknessGuid = "f2712442-b9df-44fe-ac7b-c3524342c804";

        #endregion

        /// <summary>
        /// Sets layered material to a model object using the low-level parameter approach
        /// This is the core method that makes layered material assignment work in Dynamo!
        /// </summary>
        /// <param name="modelObject">ModelObject to set material on</param>
        /// <param name="layeredMaterialId">ID of layered material to assign</param>
        /// <param name="model">Renga model for creating operations</param>
        /// <returns>Success status and debug information</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public static Dictionary<string, object> SetLayeredMaterial(
            DynObjects.ModelObject modelObject, 
            int layeredMaterialId, 
            DynDocument.Model model)
        {
            var debugInfo = "🎨 Setting layered material...\n";
            
            try
            {
                if (modelObject?._i == null)
                {
                    debugInfo += "❌ Model object is null or invalid\n";
                    return CreateResult(false, debugInfo);
                }

                if (model?._i == null)
                {
                    debugInfo += "❌ Model is null or invalid\n";
                    return CreateResult(false, debugInfo);
                }

                if (layeredMaterialId <= 0)
                {
                    debugInfo += "❌ Invalid layered material ID\n";
                    return CreateResult(false, debugInfo);
                }

                debugInfo += $"📋 Target Object ID: {modelObject.Id}\n";
                debugInfo += $"🎨 Material ID: {layeredMaterialId}\n";

                // Create operation for undo support
                var operation = model.CreateOperation();
                if (operation?._i == null)
                {
                    debugInfo += "❌ Failed to create operation\n";
                    return CreateResult(false, debugInfo);
                }

                operation.Start();

                try
                {
                    // Get parameter container from the object
                    var parameterContainer = modelObject._i.GetParameters();
                    if (parameterContainer == null)
                    {
                        debugInfo += "❌ Failed to get parameter container\n";
                        operation.Rollback();
                        return CreateResult(false, debugInfo);
                    }

                    // Convert string GUID to proper GUID
                    var materialGuid = new Guid(LayeredMaterialGuid);
                    
                    // Check if object supports layered materials
                    if (parameterContainer.Contains(materialGuid))
                    {
                        debugInfo += "✅ Object supports layered materials\n";
                        
                        // Get and set the layered material parameter
                        var layeredMaterialParam = parameterContainer.Get(materialGuid);
                        layeredMaterialParam.SetIntValue(layeredMaterialId);
                        debugInfo += "✅ Layered material parameter set successfully\n";
                        
                        // Optional: Try to set thickness if the material has layers
                        TrySetThicknessFromMaterial(parameterContainer, layeredMaterialId, model, ref debugInfo);
                    }
                    else
                    {
                        debugInfo += "❌ Object does not support layered materials\n";
                        debugInfo += $"💡 Object type: {modelObject.ObjectType}\n";
                        operation.Rollback();
                        return CreateResult(false, debugInfo);
                    }

                    // Apply the operation
                    operation.Apply();
                    debugInfo += "✅ Changes applied successfully!\n";
                    debugInfo += "🎉 Layered material has been assigned to the object!\n";
                    
                    return CreateResult(true, debugInfo);
                }
                catch (Exception ex)
                {
                    operation.Rollback();
                    debugInfo += $"❌ Error during operation: {ex.Message}\n";
                    return CreateResult(false, debugInfo);
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to set layered material: {ex.Message}\n";
                return CreateResult(false, debugInfo);
            }
        }

        /// <summary>
        /// Checks if a model object supports layered materials
        /// </summary>
        /// <param name="modelObject">ModelObject to check</param>
        /// <returns>True if object supports layered materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static bool SupportsLayeredMaterials(DynObjects.ModelObject modelObject)
        {
            try
            {
                if (modelObject?._i == null) return false;
                
                var parameterContainer = modelObject._i.GetParameters();
                if (parameterContainer == null) return false;
                
                var materialGuid = new Guid(LayeredMaterialGuid);
                return parameterContainer.Contains(materialGuid);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current layered material ID from a model object
        /// </summary>
        /// <param name="modelObject">ModelObject to check</param>
        /// <returns>Current layered material ID, or -1 if none</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static int GetLayeredMaterialId(DynObjects.ModelObject modelObject)
        {
            try
            {
                if (modelObject?._i == null) return -1;
                
                var parameterContainer = modelObject._i.GetParameters();
                if (parameterContainer == null) return -1;
                
                var materialGuid = new Guid(LayeredMaterialGuid);
                if (parameterContainer.Contains(materialGuid))
                {
                    var param = parameterContainer.Get(materialGuid);
                    return param.GetIntValue();
                }
                
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Helper method to try setting thickness from layered material
        /// </summary>
        private static void TrySetThicknessFromMaterial(
            object parameterContainer, 
            int layeredMaterialId, 
            DynDocument.Model model,
            ref string debugInfo)
        {
            try
            {
                var thicknessGuid = new Guid(ThicknessGuid);
                dynamic pc = parameterContainer;
                
                if (pc.Contains(thicknessGuid))
                {
                    debugInfo += "💡 Attempting to set thickness from material...\n";
                    // Note: Getting thickness from layered material would require
                    // additional wrapper classes for ILayeredMaterialManager
                    // For now, we'll skip this step
                    debugInfo += "⚠️ Thickness setting skipped (requires material manager access)\n";
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"⚠️ Could not set thickness: {ex.Message}\n";
            }
        }

        /// <summary>
        /// Creates a result dictionary for Dynamo multi-return
        /// </summary>
        private static Dictionary<string, object> CreateResult(bool success, string debugInfo)
        {
            return new Dictionary<string, object>
            {
                { "Success", success },
                { "DebugInfo", debugInfo }
            };
        }

        #region Material Type Constants (for reference)

        /// <summary>
        /// Layered material group constants for reference
        /// </summary>
        public static class MaterialTypes
        {
            public const int Undefined = 0;
            public const int Wall = 1;
            public const int Floor = 2;
            public const int Roof = 3;
            public const int Insulation = 4;
        }

        #endregion

        /// <summary>
        /// Gets information about layered material workflow
        /// </summary>
        /// <returns>Information about how to use layered materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static string GetUsageInfo()
        {
            return "LayeredMaterialWorkflow Usage:\n" +
                   "1. Use SupportsLayeredMaterials() to check if object supports materials\n" +
                   "2. Get material IDs from Renga (Floor materials have MaterialGroup = 2)\n" +
                   "3. Use SetLayeredMaterial() to assign material to object\n" +
                   "4. Use GetLayeredMaterialId() to check current assignment\n\n" +
                   "Key GUID: " + LayeredMaterialGuid + " (LayeredMaterial Parameter)";
        }
    }
}