using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using Autodesk.DesignScript.Geometry.Core;
using DynRenga.DynProperties.Properties;

namespace DynRenga.DynDocument.Project
{
    /// <summary>
    /// Класс для работы с одиночной сущностью проекта
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Внутренний COM-объект Renga.IEntity
        /// </summary>
        public Renga.IEntity _i;
        /// <summary>
        /// Инициация класса из com-объекта Entity
        /// </summary>
        /// <param name="Entity_object">COM-объект Entity</param>
        /// <param name="some_value">Дополнительный параметр (по умолчанию 0)</param>
        internal Entity(object Entity_object, int some_value = 0)
        {
            this._i = Entity_object as Renga.IEntity;
        }
        /// <summary>
        /// Приведение любого объекта Renga к Entity
        /// </summary>
        /// <param name="AnyRengaObject"></param>
        /// <exception cref="Exception">Invalid casting</exception>
        public Entity (dynamic AnyRengaObject)
        {
            this._i = AnyRengaObject as Renga.IEntity;
            if (this._i == null) throw new Exception("Invalid casting");
        }
        /// <summary>
        /// Возвращает свойства связанные с объектом, если их возможно получить
        /// </summary>
        /// <returns></returns>
        public PropertyContainer GetProperties()
        {
            var cont = this._i.GetInterfaceByName("IPropertyContainer");
            if (cont != null)
            {
                return new PropertyContainer(cont);
            }
            else return null;
        }
        //properties
        /// <summary>
        /// Получение идентификатора объекта (int)
        /// </summary>
        public int Id => this._i.Id;
        /// <summary>
        /// Получение наименования объекта
        /// </summary>
        public string Name => this._i.Name;
        /// <summary>
        /// Получение внутреннего идентификатора (Guid)
        /// </summary>
        public Guid UniqueId => this._i.UniqueId;
        /// <summary>
        /// Получение типа объекта (что это за Entity)
        /// </summary>
        public Guid TypeId => this._i.TypeId;

        /// <summary>
        /// Типы Entity
        /// </summary>
        /// <returns></returns>
        [dr.MultiReturn(new[] { "Building", "Layer", "MaterialLayer",
        "Project", "Schedule", "Drawing", "Site", "Table"})]
        public static Dictionary<string, Guid> TypeIds()
        {
            return new Dictionary<string, Guid>()
            {
                {"Building", Renga.EntityTypes.Building },
                {"Layer", Renga.EntityTypes.Layer },
                {"MaterialLayer", Renga.EntityTypes.MaterialLayer },
                {"Project", Renga.EntityTypes.Project },
                {"Schedule", Renga.EntityTypes.Schedule },
                {"Drawing", Renga.EntityTypes.Drawing },
                {"Site", Renga.EntityTypes.Site },
                {"Table", Renga.EntityTypes.Table }
            };
        }
        /// <summary>
        /// Получение типа Entity в строчном виде
        /// </summary>
        /// <returns></returns>
        public string GetTypeAsString()
        {
            Guid type = this._i.TypeId;
            var pair = TypeIds().FirstOrDefault(a => a.Value == type);
            return pair.Key ?? type.ToString();
        }
    }
}
