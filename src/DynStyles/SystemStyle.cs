using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.Other;
using DynRenga.Core;

namespace DynRenga.DynStyles
{
    /// <summary>
    /// Рефакторенный класс для работы с интерфейсом Renga.ISystemStyle
    /// Наследует от BaseRengaStyle для устранения дублирования кода
    /// </summary>
    public class SystemStyle : BaseRengaStyle<Renga.ISystemStyle>
    {
        /// <summary>
        /// Инициализация класса через интерфейс Renga.ISystemStyle
        /// </summary>
        /// <param name="systemStyleObject">COM-объект стиля системы</param>
        internal SystemStyle(object systemStyleObject) : base(systemStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="systemStyle">Интерфейс стиля системы</param>
        internal SystemStyle(Renga.ISystemStyle systemStyle) : base(systemStyle) { }
        
        /// <summary>
        /// Получение идентификатора стиля системы
        /// </summary>
        /// <returns>ID стиля</returns>
        public override int Id => _i.Id;
        
        /// <summary>
        /// Получение наименования стиля системы
        /// </summary>
        /// <returns>Название стиля</returns>
        public override string Name => _i.Name;
        
        /// <summary>
        /// Получение строкового обозначения системы
        /// </summary>
        /// <returns>Обозначение системы</returns>
        public string Designation => _i.Designation;
        
        /// <summary>
        /// Получение цвета стиля системы
        /// </summary>
        /// <returns>Цвет системы</returns>
        public Renga_Color Color => new Renga_Color(_i.Color);
        
        /// <summary>
        /// Получение строкого типа системы снабжения
        /// Примечание: ISystemStyle не имеет свойства SystemType
        /// </summary>
        /// <returns>Неизвестный тип системы</returns>
        public string GetSystemType()
        {
            return "SystemType_Unknown";
        }
        
        /// <summary>
        /// Получение всех доступных типов систем (справочная информация)
        /// </summary>
        /// <returns>Словарь типов систем</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> GetSystemTypes()
        {
            return new Dictionary<string, object>
            {
                { "SystemType_Unknown", 0 },
                { "SystemType_DomesticColdWater", 2 },
                { "SystemType_DomesticHotWater", 3 },
                { "SystemType_DomesticSewerage", 4 },
                { "SystemType_DomesticGasSupply", 5 },
                { "SystemType_WaterFireExtinguishing", 6 },
                { "SystemType_WaterHeating", 7 },
                { "SystemType_GasFireExtinguishing", 8 },
                { "SystemType_StormDrain", 9 },
                { "SystemType_IndustrialColdWater", 10 },
                { "SystemType_IndustrialHotWater", 11 },
                { "SystemType_IndustrialSewerage", 12 }
            };
        }
        
        /// <summary>
        /// Переопределенная отладочная информация для стиля системы
        /// </summary>
        /// <returns>Расширенная отладочная информация</returns>
        public override string GetDebugInfo()
        {
            return base.GetDebugInfo() + 
                   $"\n🏷️ Designation: {Designation ?? "NULL"}" +
                   $"\n🎨 Color: {Color?.ToString() ?? "NULL"}";
        }
    }
}
