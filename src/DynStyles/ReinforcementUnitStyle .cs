using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynProperties.Quantities;
using DynRenga.DynGeometry;
using DynRenga.DynProperties.Parameters.ObjectsParam;
using DynRenga.Core;

namespace DynRenga.DynStyles
{
    /// <summary>
    /// Рефакторенный класс для работы с интерфейсом Renga.IReinforcementUnitStyle
    /// Наследует от BaseRengaStyle для устранения дублирования кода
    /// </summary>
    public class ReinforcementUnitStyle : BaseRengaStyle<Renga.IReinforcementUnitStyle>
    {
        /// <summary>
        /// Инициация класса через интерфейс Renga.IReinforcementUnitStyle
        /// </summary>
        /// <param name="reinforcementUnitStyleObject">COM-объект стиля арматурного элемента</param>
        internal ReinforcementUnitStyle(object reinforcementUnitStyleObject) : base(reinforcementUnitStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="reinforcementUnitStyle">Интерфейс стиля арматурного элемента</param>
        internal ReinforcementUnitStyle(Renga.IReinforcementUnitStyle reinforcementUnitStyle) : base(reinforcementUnitStyle) { }
        
        /// <summary>
        /// Получение идентификатора стиля арматурного элемента
        /// </summary>
        /// <returns>ID стиля</returns>
        public override int Id => _i.Id;
        
        /// <summary>
        /// Получение наименования стиля арматурного элемента
        /// </summary>
        /// <returns>Название стиля</returns>
        public override string Name => _i.Name;
        
        /// <summary>
        /// Получение типа арматурного элемента
        /// </summary>
        /// <returns>Тип арматурного элемента</returns>
        public ReinforcementUnitType UnitType => _i.UnitType;
        
        /// <summary>
        /// Получение строкового представления типа арматурного элемента
        /// </summary>
        /// <returns>Строковое представление типа</returns>
        public string GetUnitTypeAsString()
        {
            var types = GetReinforcementUnitTypes();
            var match = types.FirstOrDefault(kv => 
                ((ReinforcementUnitType)kv.Value).Equals(_i.UnitType));
            
            return match.Key ?? $"Unknown_{_i.UnitType}";
        }
        
        /// <summary>
        /// Получение всех доступных типов арматурных элементов
        /// </summary>
        /// <returns>Словарь типов арматурных элементов</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> GetReinforcementUnitTypes()
        {
            return new Dictionary<string, object>
            {
                { "Undefined", Renga.ReinforcementUnitType.ReinforcementUnitType_Undefined },
                { "Mesh", Renga.ReinforcementUnitType.ReinforcementUnitType_Mesh },
                { "Cage", Renga.ReinforcementUnitType.ReinforcementUnitType_Cage }
            };
        }
        
        /// <summary>
        /// Получение списка использований арматуры
        /// </summary>
        /// <returns>Список объектов RebarUsage</returns>
        public List<RebarUsage> GetRebarUsages()
        {
            var rebars = new List<RebarUsage>();
            var collection = _i.GetRebarUsages();
            
            if (collection != null)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    var rebarUsage = collection.Get(i);
                    if (rebarUsage != null)
                    {
                        rebars.Add(new RebarUsage(rebarUsage));
                    }
                }
            }
            
            return rebars;
        }
        
        /// <summary>
        /// Переопределенная отладочная информация для стиля арматурного элемента
        /// </summary>
        /// <returns>Расширенная отладочная информация</returns>
        public override string GetDebugInfo()
        {
            var rebarUsages = GetRebarUsages();
            
            return base.GetDebugInfo() + 
                   $"\n🔧 Unit Type: {GetUnitTypeAsString()}" +
                   $"\n📊 Rebar Usages Count: {rebarUsages.Count}";
        }
    }
}
