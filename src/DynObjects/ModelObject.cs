using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;


namespace DynRenga.DynObjects
{
    /// <summary>
    /// Класс для работы с объектом модели Renga.IModelObject
    /// </summary>
    public class ModelObject
    {
        public Renga.IModelObject _i;
        /// <summary>
        /// Инициация класса из интерфейса Renga.IModelObject
        /// </summary>
        /// <param name="model_object_com"></param>
        internal ModelObject (object model_object_com)
        {
            this._i = model_object_com as Renga.IModelObject;
        }
        /// <summary>
        /// Получение типа объекта как Guid
        /// </summary>
        /// <returns></returns>
        public Guid ObjectType => this._i.ObjectType;
        /// <summary>
        /// Получение целочисленного идентификатора объекта
        /// </summary>
        /// <returns></returns>
        public int Id => this._i.Id;
        /// <summary>
        /// Получение наименования объекта в Renga
        /// </summary>
        /// <returns></returns>
        public string Name => this._i.Name;
        /// <summary>
        /// Получение внутреннего идентификатора объекта в Renga как Guid
        /// </summary>
        /// <returns></returns>
        public Guid UniqueId => this._i.UniqueId;
        
        /// <summary>
        /// Получение ID уровня, к которому принадлежит объект
        /// </summary>
        /// <returns>ID уровня или -1 если объект не принадлежит уровню</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetLevelId()
        {
            try
            {
                // Try to get ILevelObject interface
                var levelObject = this._i as Renga.ILevelObject;
                if (levelObject != null)
                {
                    return levelObject.LevelId;
                }
                
                // If direct casting fails, try GetInterfaceByName
                var levelInterface = this._i.GetInterfaceByName("ILevelObject");
                if (levelInterface != null)
                {
                    var levelObj = levelInterface as Renga.ILevelObject;
                    if (levelObj != null)
                    {
                        return levelObj.LevelId;
                    }
                }
                
                return -1; // Object doesn't belong to a level
            }
            catch
            {
                return -1; // Error occurred, return -1
            }
        }
        
        /// <summary>
        /// Проверка, принадлежит ли объект какому-либо уровню
        /// </summary>
        /// <returns>True если объект принадлежит уровню, False если нет</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool BelongsToLevel()
        {
            return GetLevelId() != -1;
        }
        
        /// <summary>
        /// Получение дополнительной информации об уровне объекта
        /// </summary>
        /// <returns>Словарь с информацией об уровне</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "LevelId", "ElevationAboveLevel", "PlacementElevation", "VerticalOffset" })]
        public Dictionary<string, object> GetLevelInfo()
        {
            try
            {
                // Try to get ILevelObject interface
                var levelObject = this._i as Renga.ILevelObject;
                if (levelObject != null)
                {
                    return new Dictionary<string, object>
                    {
                        { "LevelId", levelObject.LevelId },
                        { "ElevationAboveLevel", levelObject.ElevationAboveLevel },
                        { "PlacementElevation", levelObject.PlacementElevation },
                        { "VerticalOffset", levelObject.VerticalOffset }
                    };
                }
                
                // If direct casting fails, try GetInterfaceByName
                var levelInterface = this._i.GetInterfaceByName("ILevelObject");
                if (levelInterface != null)
                {
                    var levelObj = levelInterface as Renga.ILevelObject;
                    if (levelObj != null)
                    {
                        return new Dictionary<string, object>
                        {
                            { "LevelId", levelObj.LevelId },
                            { "ElevationAboveLevel", levelObj.ElevationAboveLevel },
                            { "PlacementElevation", levelObj.PlacementElevation },
                            { "VerticalOffset", levelObj.VerticalOffset }
                        };
                    }
                }
                
                // Object doesn't belong to a level
                return new Dictionary<string, object>
                {
                    { "LevelId", -1 },
                    { "ElevationAboveLevel", 0.0 },
                    { "PlacementElevation", 0.0 },
                    { "VerticalOffset", 0.0 }
                };
            }
            catch
            {
                // Error occurred
                return new Dictionary<string, object>
                {
                    { "LevelId", -1 },
                    { "ElevationAboveLevel", 0.0 },
                    { "PlacementElevation", 0.0 },
                    { "VerticalOffset", 0.0 }
                };
            }
        }
        /// <summary>
        /// Приведение объекта модели к интерфейсу Renga.IObjectWithMaterial для получения материала, если таковой вообще назначен.
        /// Если не назначен - будет возвращено "-1"
        /// </summary>
        /// <returns></returns>
        public int GetAssotiatedMaterialId()
        {
            Renga.IObjectWithMaterial obj_mat = this._i as Renga.IObjectWithMaterial;
            if (obj_mat.HasMaterial()) return obj_mat.MaterialId;
            else return -1;
        }
        /// <summary>
        /// Получает ассоциированный с объектом набор слоев LayeredMaterial 
        /// (только в случае если это объект типа Крыша, Стена или Перекрытие
        /// </summary>
        /// <returns></returns>
        public int GetAssotiatedLayerMaterialId()
        {
            Renga.IObjectWithLayeredMaterial obj_lay_mat = this._i as Renga.IObjectWithLayeredMaterial;
            if (obj_lay_mat.HasLayeredMaterial()) return obj_lay_mat.LayeredMaterialId;
            else return -1;
        }
        /// <summary>
        /// Получает строкое наименование метки, ассоциированной с объектом
        /// </summary>
        /// <returns>Если марка отсутствует, то возвращается пустая строка. В противном случае - марка.</returns>
        public string GetAssotiatedMark()
        {
            Renga.IObjectWithMark obj_mark = this._i as Renga.IObjectWithMark;
            if (obj_mark == null) return "";
            else return obj_mark.Mark;
        }
        /// <summary>
        /// Получение свойств объекта - собственных свойств, количества и параметров 
        /// </summary>
        /// <returns>Словарь со свойствами</returns>
        [dr.MultiReturn(new[] { "Properties_IPropertyContainer", 
            "Quantities_IQuantityContainer", "Parameters_IParameterContainer" })]
        public Dictionary<string, object> GetObjectsData()
        {
            return new Dictionary<string, object>
            {
                { "Properties_IPropertyContainer", new DynProperties.Properties.PropertyContainer (this._i.GetProperties())},
                { "Quantities_IQuantityContainer", new DynProperties.Quantities.QuantityContainer(this._i.GetQuantities())},
                { "Parameters_IParameterContainer", new DynProperties.Parameters.ParameterContainer (this._i.GetParameters())}
            };

        }
        /// <summary>
        /// Получение интерфейса IBaseline2DObject для объектов с 2D базовой линией
        /// Возвращает null если объект не поддерживает данный интерфейс
        /// </summary>
        /// <returns>Baseline2DObject или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Baseline2DObject GetBaseline2DObject()
        {
            try
            {
                // First try direct casting
                Renga.IBaseline2DObject baseline2DObject = this._i as Renga.IBaseline2DObject;
                if (baseline2DObject != null) return new Baseline2DObject(baseline2DObject);
                
                // If direct casting fails, try GetInterfaceByName
                object interfaceObj = this._i.GetInterfaceByName("IBaseline2DObject");
                if (interfaceObj != null)
                {
                    return new Baseline2DObject(interfaceObj);
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Отладочный метод для проверки доступных интерфейсов объекта
        /// </summary>
        /// <returns>Список доступных интерфейсов</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public List<string> GetAvailableInterfaces()
        {
            List<string> interfaces = new List<string>();
            
            // Test common interfaces
            string[] interfaceNames = {
                "IBaseline2DObject",
                "IObjectWithMaterial", 
                "IObjectWithLayeredMaterial",
                "IObjectWithMark",
                "IObjectWithLink",
                "ITextObject"
            };
            
            foreach (string interfaceName in interfaceNames)
            {
                try
                {
                    object interfaceObj = this._i.GetInterfaceByName(interfaceName);
                    if (interfaceObj != null)
                    {
                        interfaces.Add(interfaceName + " ✓");
                    }
                    else
                    {
                        interfaces.Add(interfaceName + " ✗");
                    }
                }
                catch
                {
                    interfaces.Add(interfaceName + " ✗");
                }
            }
            
            return interfaces;
        }

        /// <summary>
        /// Sets layered material to this model object
        /// </summary>
        /// <param name="layeredMaterialId">ID of layered material to assign</param>
        /// <param name="project">Renga project for creating operation</param>
        /// <returns>Success status and debug info</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> SetLayeredMaterial(int layeredMaterialId, object project)
        {
            // Convert this ModelObject to IModelObject and put it in an array
            var iModelObject = new DynRenga.RengaAPI.IModelObject(this);
            var modelObjects = new DynRenga.RengaAPI.IModelObject[] { iModelObject };
            
            // Convert project to IProject
            var iProject = new DynRenga.RengaAPI.IProject(project);
            
            return DynRenga.DynDocument.LayeredMaterialHelper.SetLayeredMaterial(modelObjects, layeredMaterialId, iProject);
        }

        /// <summary>
        /// Checks if this object supports layered materials
        /// </summary>
        /// <returns>True if object supports layered materials</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool SupportsLayeredMaterials()
        {
            try
            {
                if (this._i == null) return false;
                
                var parameterContainer = this._i.GetParameters();
                if (parameterContainer == null) return false;
                
                return parameterContainer.Contains(DynRenga.DynDocument.LayeredMaterialHelper.GuidStyleLayeredMaterial);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current layered material ID of this object (if any)
        /// </summary>
        /// <returns>Current layered material ID or -1 if none set</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int GetLayeredMaterialId()
        {
            try
            {
                if (this._i == null) return -1;
                
                var parameterContainer = this._i.GetParameters();
                if (parameterContainer == null) return -1;
                
                if (parameterContainer.Contains(DynRenga.DynDocument.LayeredMaterialHelper.GuidStyleLayeredMaterial))
                {
                    var layeredMaterialParam = parameterContainer.Get(DynRenga.DynDocument.LayeredMaterialHelper.GuidStyleLayeredMaterial);
                    try
                    {
                        return layeredMaterialParam.GetIntValue();
                    }
                    catch
                    {
                        return -1; // Parameter exists but couldn't get value
                    }
                }
                
                return -1;
            }
            catch
            {
                return -1;
            }
        }

    }
}
