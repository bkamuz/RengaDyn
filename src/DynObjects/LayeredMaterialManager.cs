using System;
using System.Collections.Generic;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.DynObjects
{
    /// <summary>
    /// Dynamo node class for working with Renga ILayeredMaterialManager interface
    /// Класс для работы с интерфейсом Renga.ILayeredMaterialManager
    /// </summary>
    public class LayeredMaterialManager
    {
        /// <summary>
        /// Internal Renga ILayeredMaterialManager interface
        /// </summary>
        public Renga.ILayeredMaterialManager _i;

        /// <summary>
        /// Initialize class from Renga.ILayeredMaterialManager interface
        /// Инициализация класса из интерфейса Renga.ILayeredMaterialManager
        /// </summary>
        /// <param name="layeredMaterialManager_obj">Renga ILayeredMaterialManager COM object</param>
        internal LayeredMaterialManager(object layeredMaterialManager_obj)
        {
            this._i = layeredMaterialManager_obj as Renga.ILayeredMaterialManager;
        }

        /// <summary>
        /// Get layered material by ID
        /// Получение слоистого материала по ID
        /// </summary>
        /// <param name="id">Material ID</param>
        /// <returns>LayeredMaterial object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public LayeredMaterial GetLayeredMaterial(int id)
        {
            if (this._i == null)
                throw new InvalidOperationException("LayeredMaterialManager is not initialized");
            
            try
            {
                var material = this._i.GetLayeredMaterial(id);
                return new LayeredMaterial(material);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get layered material with ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get layered material by ID group pair (Deprecated)
        /// Получение слоистого материала по паре ID групп (Устарело)
        /// </summary>
        /// <param name="layeredMaterialIdGroupPair">ID group pair object</param>
        /// <returns>LayeredMaterial object</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        [Obsolete("This method is deprecated, use GetLayeredMaterial instead")]
        public LayeredMaterial GetLayeredMaterialByIdGroupPair(object layeredMaterialIdGroupPair)
        {
            if (this._i == null)
                throw new InvalidOperationException("LayeredMaterialManager is not initialized");
            
            try
            {
                var material = this._i.GetLayeredMaterialByIdGroupPair((dynamic)layeredMaterialIdGroupPair);
                return new LayeredMaterial(material);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get layered material by ID group pair: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all layered materials in the project
        /// Получение всех слоистых материалов в проекте
        /// </summary>
        /// <param name="project">Renga project to get materials from</param>
        /// <returns>Dictionary of material ID and name pairs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<int, string> GetAllLayeredMaterials(object project)
        {
            if (this._i == null)
                throw new InvalidOperationException("LayeredMaterialManager is not initialized");
            
            var materials = new Dictionary<int, string>();
            
            try
            {
                // Handle both IProject wrapper and underlying COM object
                dynamic rengaProject = null;
                
                if (project is DynRenga.RengaAPI.IProject projectWrapper)
                {
                    rengaProject = projectWrapper._i; // Access underlying COM object
                }
                else
                {
                    rengaProject = project; // Assume it's already a COM object
                }
                
                if (rengaProject == null)
                    throw new ArgumentException("Invalid project object");

                var allMaterials = rengaProject.LayeredMaterials;
                for (int i = 0; i < allMaterials.Count; i++)
                {
                    var entity = allMaterials.GetByIndex(i);
                    var layeredMaterial = this._i.GetLayeredMaterial(entity.Id);
                    
                    if (layeredMaterial != null)
                    {
                        materials[layeredMaterial.Id] = layeredMaterial.Name;
                    }
                }
                
                return materials;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get all layered materials: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if the manager is properly initialized
        /// Проверка правильной инициализации менеджера
        /// </summary>
        /// <returns>True if initialized</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized()
        {
            return this._i != null;
        }

        /// <summary>
        /// Get debug information about the manager
        /// Получение отладочной информации о менеджере
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        public string GetDebugInfo()
        {
            return $"LayeredMaterialManager - Initialized: {IsInitialized()}";
        }
    }
}