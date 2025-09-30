using System;
using System.Collections.Generic;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Helper class for working with layered materials in Dynamo
    /// Provides the essential GUIDs and utility methods for setting layered materials on objects
    /// </summary>
    public static class LayeredMaterialHelper
    {
        #region GUIDs - Essential Parameter Identifiers

        /// <summary>
        /// GUID for layered material style parameter - the key to setting layered materials!
        /// </summary>
        public static readonly Guid GuidStyleLayeredMaterial = new Guid("2cbc4964-61a0-4553-8103-36e2dd2ab31c");
        
        /// <summary>
        /// GUID for thickness parameter
        /// </summary>
        public static readonly Guid GuidThickness = new Guid("f2712442-b9df-44fe-ac7b-c3524342c804");
        
        /// <summary>
        /// GUID for vertical offset parameter
        /// </summary>
        public static readonly Guid GuidVerticalOffset = new Guid("337fd89b-763e-46dc-b1a9-7aecb253adbf");
        
        /// <summary>
        /// GUID for rounding radius parameter
        /// </summary>
        public static readonly Guid GuidRoundingRadius = new Guid("3b97850d-d3ea-4a2d-91ff-0bc572d8795d");

        #endregion

        #region Layered Material Group Constants
        
        /// <summary>
        /// Material group enumeration values
        /// </summary>
        public static class LayeredMaterialGroups
        {
            public const int Undefined = 0;
            public const int Wall = 1;
            public const int Floor = 2;
            public const int Roof = 3;
            public const int Insulation = 4;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets layered material on any Renga model object
        /// </summary>
        /// <param name="modelObject">Renga model object to modify</param>
        /// <param name="layeredMaterialId">ID of layered material to assign</param>
        /// <param name="project">Renga project for creating operation</param>
        /// <returns>Success status and debug info</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public static Dictionary<string, object> SetLayeredMaterial(
            DynObjects.ModelObject modelObject, 
            int layeredMaterialId, 
            object project)
        {
            var debugInfo = "🎨 Setting layered material...\n";
            
            try
            {
                if (modelObject?._i == null)
                {
                    debugInfo += "❌ Model object is null or not initialized\n";
                    return CreateResult(false, debugInfo);
                }

                if (layeredMaterialId <= 0)
                {
                    debugInfo += "❌ Invalid layered material ID\n";
                    return CreateResult(false, debugInfo);
                }

                // Get project and create operation
                dynamic rengaProject = project;
                if (rengaProject == null)
                {
                    debugInfo += "❌ Project is null\n";
                    return CreateResult(false, debugInfo);
                }

                debugInfo += $"📋 Object ID: {modelObject.Id}\n";
                debugInfo += $"🎨 Material ID: {layeredMaterialId}\n";

                // Create operation for undo support
                var operation = rengaProject.CreateOperation();
                if (operation == null)
                {
                    debugInfo += "❌ Failed to create operation\n";
                    return CreateResult(false, debugInfo);
                }

                operation.Start();

                try
                {
                    // Get parameter container
                    var parameterContainer = modelObject._i.GetParameters();
                    if (parameterContainer == null)
                    {
                        debugInfo += "❌ Failed to get parameter container\n";
                        operation.Rollback();
                        return CreateResult(false, debugInfo);
                    }

                    // Check if object supports layered materials
                    if (parameterContainer.Contains(GuidStyleLayeredMaterial))
                    {
                        debugInfo += "✅ Object supports layered materials\n";
                        
                        // Get the layered material parameter
                        var layeredMaterialParam = parameterContainer.Get(GuidStyleLayeredMaterial);
                        
                        // Set the layered material ID
                        try
                        {
                            layeredMaterialParam.SetIntValue(layeredMaterialId);
                            debugInfo += "✅ Layered material ID set successfully\n";
                            
                            // Try to set thickness from the layered material's base layer
                            SetThicknessFromLayeredMaterial(parameterContainer, layeredMaterialId, rengaProject, ref debugInfo);
                        }
                        catch (Exception ex)
                        {
                            debugInfo += $"❌ Failed to set layered material parameter: {ex.Message}\n";
                            operation.Rollback();
                            return CreateResult(false, debugInfo);
                        }
                    }
                    else
                    {
                        debugInfo += "❌ Object does not support layered materials\n";
                        operation.Rollback();
                        return CreateResult(false, debugInfo);
                    }

                    // Apply the operation
                    operation.Apply();
                    debugInfo += "✅ Operation applied successfully\n";
                    
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
        /// Helper method to set thickness from layered material's base layer
        /// </summary>
        private static void SetThicknessFromLayeredMaterial(
            dynamic parameterContainer, 
            int layeredMaterialId, 
            dynamic project, 
            ref string debugInfo)
        {
            try
            {
                var materialManager = project.LayeredMaterialManager;
                if (materialManager == null)
                {
                    debugInfo += "⚠️ LayeredMaterialManager not available\n";
                    return;
                }

                var layeredMaterial = materialManager.GetLayeredMaterial(layeredMaterialId);
                if (layeredMaterial?.Layers != null && layeredMaterial.Layers.Count > 0)
                {
                    // Get the base layer thickness
                    var baseLayer = layeredMaterial.Layers.Get(layeredMaterial.BaseLayerIndex);
                    
                    // Set thickness parameter if it exists
                    if (parameterContainer.Contains(GuidThickness))
                    {
                        var thicknessParam = parameterContainer.Get(GuidThickness);
                        try
                        {
                            thicknessParam.SetDoubleValue(baseLayer.Thickness);
                            debugInfo += $"✅ Thickness set to {baseLayer.Thickness}mm\n";
                        }
                        catch
                        {
                            debugInfo += $"⚠️ Could not set thickness parameter\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"⚠️ Could not set thickness from material: {ex.Message}\n";
                // Don't fail the whole operation for thickness setting issues
            }
        }

        /// <summary>
        /// Gets layered materials filtered by type for Dynamo
        /// </summary>
        /// <param name="project">Renga project</param>
        /// <param name="materialType">Material type (Floor=2, Wall=1, Roof=3)</param>
        /// <returns>Lists of material IDs and names</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "MaterialIds", "MaterialNames", "DebugInfo" })]
        public static Dictionary<string, object> GetLayeredMaterialsByType(object project, int materialType)
        {
            var debugInfo = "🔍 Getting layered materials...\n";
            var materialIds = new List<int>();
            var materialNames = new List<string>();
            
            try
            {
                dynamic rengaProject = project;
                if (rengaProject == null)
                {
                    debugInfo += "❌ Project is null\n";
                    return CreateMaterialResult(materialIds, materialNames, debugInfo);
                }

                var materialManager = rengaProject.LayeredMaterialManager;
                var allMaterials = rengaProject.LayeredMaterials;
                
                debugInfo += $"📊 Found {allMaterials.Count} total layered materials\n";
                
                for (int i = 0; i < allMaterials.Count; i++)
                {
                    var entity = allMaterials.GetByIndex(i);
                    var layeredMaterial = materialManager.GetLayeredMaterial(entity.Id);
                    
                    if (layeredMaterial != null)
                    {
                        var idGroupPair = layeredMaterial.GetIdGroupPair();
                        if ((int)idGroupPair.Group == materialType)
                        {
                            materialIds.Add(layeredMaterial.Id);
                            materialNames.Add(layeredMaterial.Name);
                        }
                    }
                }
                
                debugInfo += $"✅ Found {materialIds.Count} materials of type {GetMaterialTypeName(materialType)}\n";
                
                return CreateMaterialResult(materialIds, materialNames, debugInfo);
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Error getting materials: {ex.Message}\n";
                return CreateMaterialResult(materialIds, materialNames, debugInfo);
            }
        }

        /// <summary>
        /// Gets floor layered materials specifically
        /// </summary>
        /// <param name="project">Renga project</param>
        /// <returns>Lists of floor material IDs and names</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "MaterialIds", "MaterialNames", "DebugInfo" })]
        public static Dictionary<string, object> GetFloorLayeredMaterials(object project)
        {
            return GetLayeredMaterialsByType(project, LayeredMaterialGroups.Floor);
        }

        #endregion

        #region Helper Methods for Results

        /// <summary>
        /// Creates a standard result dictionary
        /// </summary>
        private static Dictionary<string, object> CreateResult(bool success, string debugInfo)
        {
            return new Dictionary<string, object>
            {
                { "Success", success },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Creates a material result dictionary
        /// </summary>
        private static Dictionary<string, object> CreateMaterialResult(List<int> ids, List<string> names, string debugInfo)
        {
            return new Dictionary<string, object>
            {
                { "MaterialIds", ids },
                { "MaterialNames", names },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Gets material type name for debugging
        /// </summary>
        private static string GetMaterialTypeName(int materialType)
        {
            return materialType switch
            {
                LayeredMaterialGroups.Undefined => "Undefined",
                LayeredMaterialGroups.Wall => "Wall",
                LayeredMaterialGroups.Floor => "Floor",
                LayeredMaterialGroups.Roof => "Roof",
                LayeredMaterialGroups.Insulation => "Insulation",
                _ => "Unknown"
            };
        }

        #endregion
    }
}