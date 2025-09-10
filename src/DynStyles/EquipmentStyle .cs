using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.Core;

namespace DynRenga.DynStyles
{
    /// <summary>
    /// Рефакторенный класс для работы с интерфейсом Renga.IEquipmentStyle
    /// Наследует от BaseRengaWrapper (не BaseRengaStyle, поскольку нет ID)
    /// </summary>
    public class EquipmentStyle : BaseRengaWrapper<Renga.IEquipmentStyle>
    {
        /// <summary>
        /// Инициация класса из интерфейса Renga.IEquipmentStyle
        /// </summary>
        /// <param name="equipmentStyleObject">COM-объект стиля оборудования</param>
        internal EquipmentStyle(object equipmentStyleObject) : base(equipmentStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="equipmentStyle">Интерфейс стиля оборудования</param>
        internal EquipmentStyle(Renga.IEquipmentStyle equipmentStyle) : base(equipmentStyle) { }
        
        /// <summary>
        /// Получение наименования стиля оборудования
        /// </summary>
        /// <returns>Название стиля</returns>
        public string Name => _i.Name;
        
        /// <summary>
        /// Получение категории оборудования
        /// </summary>
        /// <returns>Категория оборудования</returns>
        public EquipmentCategory Category => _i.Category;
        
        /// <summary>
        /// Получение строкового типа оборудования (Renga.EquipmentCategory)
        /// </summary>
        /// <returns>Строковое представление категории</returns>
        public string GetCategoryAsString()
        {
            var categories = GetEquipmentCategories();
            var match = categories.FirstOrDefault(kv => 
                ((EquipmentCategory)kv.Value).Equals(_i.Category));
            
            return match.Key ?? $"Unknown_{_i.Category}";
        }
        
        /// <summary>
        /// Получение всех доступных категорий оборудования
        /// </summary>
        /// <returns>Словарь категорий оборудования</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> GetEquipmentCategories()
        {
            return new Dictionary<string, object>
            {
                { "Other", Renga.EquipmentCategory.EquipmentCategory_Other },
                { "Faucet", Renga.EquipmentCategory.EquipmentCategory_Faucet },
                { "Manifold", Renga.EquipmentCategory.EquipmentCategory_Manifold },
                { "Pump", Renga.EquipmentCategory.EquipmentCategory_Pump },
                { "WashingMachine", Renga.EquipmentCategory.EquipmentCategory_WashingMachine },
                { "Radiator", Renga.EquipmentCategory.EquipmentCategory_Radiator },
                { "TowelRadiator", Renga.EquipmentCategory.EquipmentCategory_TowelRadiator },
                { "ExpansionVessel", Renga.EquipmentCategory.EquipmentCategory_ExpansionVessel },
                { "PlateHeatExchanger", Renga.EquipmentCategory.EquipmentCategory_PlateHeatExchanger },
                { "Boiler", Renga.EquipmentCategory.EquipmentCategory_Boiler }
            };
        }
        
        /// <summary>
        /// Переопределенная отладочная информация для стиля оборудования
        /// </summary>
        /// <returns>Расширенная отладочная информация</returns>
        public override string GetDebugInfo()
        {
            return base.GetDebugInfo() + 
                   $"\n🏷️ Style Name: {Name ?? "NULL"}" +
                   $"\n🔧 Category: {GetCategoryAsString()}";
        }
    }
}
