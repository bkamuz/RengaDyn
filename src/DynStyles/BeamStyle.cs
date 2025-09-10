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
    /// Рефакторенный класс для работы с интерфейсом Renga.IBeamStyle
    /// Наследует от BaseRengaStyle для устранения дублирования кода
    /// </summary>
    public class BeamStyle : BaseRengaStyle<Renga.IBeamStyle>
    {
        /// <summary>
        /// Инициализация интерфейса Renga.IBeamStyle из com-объекта
        /// </summary>
        /// <param name="beamStyleObject">COM-объект стиля балки</param>
        internal BeamStyle(object beamStyleObject) : base(beamStyleObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="beamStyle">Интерфейс стиля балки</param>
        internal BeamStyle(Renga.IBeamStyle beamStyle) : base(beamStyle) { }
        
        /// <summary>
        /// Получение целочисленного идентификатора стиля балки
        /// </summary>
        /// <returns>ID стиля</returns>
        public override int Id => _i.Id;
        
        /// <summary>
        /// Получение наименования стиля балки
        /// </summary>
        /// <returns>Название стиля</returns>
        public override string Name => _i.Name;
        
        /// <summary>
        /// Получение профиля балки
        /// </summary>
        /// <returns>Объект профиля балки</returns>
        public Profile Profile => new Profile(_i.Profile);
    }
}
