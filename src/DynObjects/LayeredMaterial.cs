using System;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.DynObjects
{
    /// <summary>
    /// Dynamo node class for working with Renga ILayeredMaterial interface
    /// Класс для работы с интерфейсом Renga.ILayeredMaterial
    /// </summary>
    public class LayeredMaterial
    {
        /// <summary>
        /// Internal Renga ILayeredMaterial interface
        /// </summary>
        public Renga.ILayeredMaterial _i;

        /// <summary>
        /// Initialize class from Renga.ILayeredMaterial interface
        /// Инициализация класса из интерфейса Renga.ILayeredMaterial
        /// </summary>
        /// <param name="layeredMaterial_obj">Renga ILayeredMaterial COM object</param>
        internal LayeredMaterial(object layeredMaterial_obj)
        {
            this._i = layeredMaterial_obj as Renga.ILayeredMaterial;
        }

        /// <summary>
        /// Get the unique identifier of the layered material
        /// Получение уникального идентификатора слоистого материала
        /// </summary>
        /// <returns>Material ID</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int Id 
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("LayeredMaterial is not initialized");
                return this._i.Id;
            }
        }

        /// <summary>
        /// Get the name of the layered material
        /// Получение названия слоистого материала
        /// </summary>
        /// <returns>Material name</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Name 
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("LayeredMaterial is not initialized");
                return this._i.Name;
            }
        }

        /// <summary>
        /// Get the index of the base material layer
        /// Получение индекса базового слоя материала
        /// </summary>
        /// <returns>Base layer index</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int BaseLayerIndex 
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("LayeredMaterial is not initialized");
                return this._i.BaseLayerIndex;
            }
        }

        /// <summary>
        /// Get the collection of material layers
        /// Получение коллекции слоев материала
        /// </summary>
        /// <returns>Material layers collection</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object Layers 
        {
            get
            {
                if (this._i == null)
                    throw new InvalidOperationException("LayeredMaterial is not initialized");
                return this._i.Layers;
            }
        }

        /// <summary>
        /// Get the base material layer
        /// Получение базового слоя материала
        /// </summary>
        /// <returns>Base material layer</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetBaseLayer()
        {
            if (this._i == null)
                throw new InvalidOperationException("LayeredMaterial is not initialized");
            
            try
            {
                return this._i.GetBaseLayer();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get base layer: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get the identifier and group of the layered material
        /// Получение идентификатора и группы слоистого материала
        /// </summary>
        /// <returns>ID group pair object</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public object GetIdGroupPair()
        {
            if (this._i == null)
                throw new InvalidOperationException("LayeredMaterial is not initialized");
            
            try
            {
                return this._i.GetIdGroupPair();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get ID group pair: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if the layered material is properly initialized
        /// Проверка правильной инициализации слоистого материала
        /// </summary>
        /// <returns>True if initialized</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized()
        {
            return this._i != null;
        }

        /// <summary>
        /// Get debug information about the layered material
        /// Получение отладочной информации о слоистом материале
        /// </summary>
        /// <returns>Debug information string</returns>
        [dr.IsVisibleInDynamoLibrary(false)]
        public string GetDebugInfo()
        {
            if (!IsInitialized())
                return "LayeredMaterial - Not initialized";
            
            try
            {
                return $"LayeredMaterial - ID: {Id}, Name: {Name}, BaseLayerIndex: {BaseLayerIndex}";
            }
            catch (Exception ex)
            {
                return $"LayeredMaterial - Error getting debug info: {ex.Message}";
            }
        }

        /// <summary>
        /// String representation of the layered material
        /// Строковое представление слоистого материала
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            if (!IsInitialized())
                return "LayeredMaterial (Not initialized)";
            
            try
            {
                return $"LayeredMaterial: {Name} (ID: {Id})";
            }
            catch
            {
                return "LayeredMaterial (Error accessing properties)";
            }
        }
    }
}