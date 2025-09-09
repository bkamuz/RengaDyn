using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynObjects;

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Класс для работы с коллекцией объектов модели (Renga.IModelObjectCollection)
    /// </summary>
    public class ModelObjectCollection
    {
        public Renga.IModelObjectCollection _i;
        
        /// <summary>
        /// Инициализация класса из COM-объекта Renga.IModelObjectCollection
        /// </summary>
        /// <param name="model_object_collection_com">COM-объект IModelObjectCollection</param>
        internal ModelObjectCollection(object model_object_collection_com)
        {
            this._i = model_object_collection_com as Renga.IModelObjectCollection;
        }
        
        /// <summary>
        /// Получение количества объектов в коллекции
        /// </summary>
        public int Count => this._i.Count;
        
        /// <summary>
        /// Получение объекта по индексу
        /// </summary>
        /// <param name="index">Индекс объекта</param>
        /// <returns>ModelObject объект</returns>
        public ModelObject GetByIndex(int index)
        {
            return new ModelObject(this._i.GetByIndex(index));
        }
        
        /// <summary>
        /// Получение объекта по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>ModelObject объект</returns>
        public ModelObject GetById(int id)
        {
            return new ModelObject(this._i.GetById(id));
        }
        
        /// <summary>
        /// Получение объекта по уникальному идентификатору (GUID)
        /// </summary>
        /// <param name="uniqueId">Уникальный идентификатор объекта</param>
        /// <returns>ModelObject объект</returns>
        public ModelObject GetByUniqueId(Guid uniqueId)
        {
            return new ModelObject(this._i.GetByUniqueId(uniqueId));
        }
        
        /// <summary>
        /// Получение всех идентификаторов объектов в коллекции
        /// </summary>
        /// <returns>Список идентификаторов</returns>
        public List<int> GetIds()
        {
            return this._i.GetIds().OfType<int>().ToList();
        }
        
        /// <summary>
        /// Получение всех объектов в коллекции
        /// </summary>
        /// <returns>Список ModelObject объектов</returns>
        public List<ModelObject> GetAllObjects()
        {
            List<ModelObject> objects = new List<ModelObject>();
            for (int i = 0; i < this._i.Count; i++)
            {
                objects.Add(new ModelObject(this._i.GetByIndex(i)));
            }
            return objects;
        }
    }
}
