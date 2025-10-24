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
    /// Рефакторенный класс для работы с интерфейсом Renga.IPlumbingFixtureStyle
    /// Наследует от BaseRengaWrapper (нет ID, поэтому не BaseRengaStyle)
    /// </summary>
    public class PlumbingFixtureStyle : BaseRengaWrapper<Renga.IPlumbingFixtureStyle>
    {
        /// <summary>
        /// Инициация класса через интерфейс Renga.IPlumbingFixtureStyle
        /// </summary>
        /// <param name="plumbingFixtureStyleObject">COM-объект стиля сантехнического оборудования</param>
        internal PlumbingFixtureStyle(object plumbingFixtureStyleObject) : base(plumbingFixtureStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="plumbingFixtureStyle">Интерфейс стиля сантехнического оборудования</param>
        internal PlumbingFixtureStyle(Renga.IPlumbingFixtureStyle plumbingFixtureStyle) : base(plumbingFixtureStyle) { }
        
        /// <summary>
        /// Получение имени стиля сантехнического оборудования
        /// </summary>
        /// <returns>Название стиля</returns>
        public string Name => _i.Name;
        
        /// <summary>
        /// Получение категории сантехнического оборудования
        /// </summary>
        /// <returns>Категория оборудования</returns>
        public PlumbingFixtureCategory Category => _i.Category;
        
        /// <summary>
        /// Получение строкового типа оборудования (Renga.PlumbingFixtureCategory)
        /// </summary>
        /// <returns>Строковое представление категории</returns>
        public string GetCategoryAsString()
        {
            var categories = GetPlumbingFixtureCategories();
            var match = categories.FirstOrDefault(kv => 
                ((PlumbingFixtureCategory)kv.Value).Equals(_i.Category));
            
            return match.Key ?? $"Unknown_{_i.Category}";
        }
        
        /// <summary>
        /// Получение всех доступных категорий сантехнического оборудования
        /// </summary>
        /// <returns>Словарь категорий сантехнического оборудования</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> GetPlumbingFixtureCategories()
        {
            return new Dictionary<string, object>
            {
                { "Other", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_Other },
                { "ToiletPan", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_ToiletPan },
                { "WashBasin", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_WashBasin },
                { "Bath", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_Bath },
                { "Sink", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_Sink },
                { "Shower", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_Shower },
                { "FloorDrain", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_FloorDrain },
                { "RoofDrain", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_RoofDrain },
                { "Bidet", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_Bidet },
                { "Urinal", Renga.PlumbingFixtureCategory.PlumbingFixtureCategory_Urinal }
            };
        }
        
        /// <summary>
        /// Переопределенная отладочная информация для стиля сантехнического оборудования
        /// </summary>
        /// <returns>Расширенная отладочная информация</returns>
        public override string GetDebugInfo()
        {
            return base.GetDebugInfo() + 
                   $"\n🏷️ Style Name: {Name ?? "NULL"}" +
                   $"\n🚿 Category: {GetCategoryAsString()}";
        }
    }
}
