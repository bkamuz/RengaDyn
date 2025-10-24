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
    /// Рефакторенный класс для работы с интерфейсом Renga.IRebarStyle
    /// Наследует от BaseRengaStyle для устранения дублирования кода
    /// </summary>
    public class RebarStyle : BaseRengaStyle<Renga.IRebarStyle>
    {
        /// <summary>
        /// Инициация класса через интерфейс Renga.IRebarStyle
        /// </summary>
        /// <param name="rebarStyleObject">COM-объект стиля арматуры</param>
        internal RebarStyle(object rebarStyleObject) : base(rebarStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="rebarStyle">Интерфейс стиля арматуры</param>
        internal RebarStyle(Renga.IRebarStyle rebarStyle) : base(rebarStyle) { }
        
        /// <summary>
        /// Получение численного идентификатора стиля арматуры
        /// </summary>
        /// <returns>ID стиля</returns>
        public override int Id => _i.Id;
        
        /// <summary>
        /// Получение имени стиля арматуры
        /// </summary>
        /// <returns>Название стиля</returns>
        public override string Name => _i.Name;
        
        /// <summary>
        /// Получение марки арматуры
        /// </summary>
        /// <returns>Марка арматуры</returns>
        public string GradeName => _i.GradeName;
        
        /// <summary>
        /// Получение диаметра арматуры в миллиметрах
        /// </summary>
        /// <returns>Диаметр арматуры</returns>
        public double Diameter => _i.Diameter;
        
        /// <summary>
        /// Получение идентификатора материала арматуры
        /// </summary>
        /// <returns>ID материала</returns>
        public int MaterialId => _i.MaterialId;
        
        /// <summary>
        /// Предел прочности арматуры при растяжении
        /// </summary>
        /// <returns>Предел прочности</returns>
        public double GradeTensileStrength => _i.GradeTensileStrength;
        
        /// <summary>
        /// Переопределенная отладочная информация для стиля арматуры
        /// </summary>
        /// <returns>Расширенная отладочная информация</returns>
        public override string GetDebugInfo()
        {
            return base.GetDebugInfo() + 
                   $"\n🏷️ Grade Name: {GradeName ?? "NULL"}" +
                   $"\n📏 Diameter: {Diameter} mm" +
                   $"\n🏗️ Material ID: {MaterialId}" +
                   $"\n💪 Tensile Strength: {GradeTensileStrength}";
        }
    }
}
