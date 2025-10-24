using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;
using DynRenga.DynGeometry;
using DynRenga.Core;

namespace DynRenga.DynStyles
{
    /// <summary>
    /// Рефакторенный класс для работы с интерфейсом Renga.IColumnStyle
    /// Наследует от BaseRengaStyle для устранения дублирования кода
    /// </summary>
    public class ColumnStyle : BaseRengaStyle<Renga.IColumnStyle>
    {
        /// <summary>
        /// Инициализация интерфейса Renga.IColumnStyle из com-объекта
        /// </summary>
        /// <param name="columnStyleObject">COM-объект стиля колонны</param>
        internal ColumnStyle(object columnStyleObject) : base(columnStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="columnStyle">Интерфейс стиля колонны</param>
        internal ColumnStyle(Renga.IColumnStyle columnStyle) : base(columnStyle) { }
        
        /// <summary>
        /// Получение целочисленного идентификатора стиля колонны
        /// </summary>
        /// <returns>ID стиля</returns>
        public override int Id => _i.Id;
        
        /// <summary>
        /// Получение наименования стиля колонны
        /// </summary>
        /// <returns>Название стиля</returns>
        public override string Name => _i.Name;
        
        /// <summary>
        /// Получение профиля колонны
        /// </summary>
        /// <returns>Объект профиля колонны</returns>
        public Profile Profile => new Profile(_i.Profile);
    }
}
