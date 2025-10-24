using System;
using DynRenga.Core.Interfaces;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.Core
{
    /// <summary>
    /// Базовый класс для всех стилей Renga (BeamStyle, ColumnStyle, etc.)
    /// Устраняет дублирование кода между различными стилями
    /// </summary>
    /// <typeparam name="TStyleInterface">Тип интерфейса стиля (IBeamStyle, IColumnStyle, etc.)</typeparam>
    [dr.IsVisibleInDynamoLibrary(false)]
    public abstract class BaseRengaStyle<TStyleInterface> : BaseRengaWrapper<TStyleInterface>, IRengaIdentifiable, IRengaNamed
        where TStyleInterface : class
    {
        /// <summary>
        /// Конструктор базового стиля
        /// </summary>
        /// <param name="styleInterface">COM-объект интерфейса стиля Renga</param>
        protected BaseRengaStyle(object styleInterface) : base(styleInterface) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="styleInterface">Типизированный интерфейс стиля Renga</param>
        protected BaseRengaStyle(TStyleInterface styleInterface) : base(styleInterface) { }
        
        /// <summary>
        /// Получение целочисленного идентификатора стиля
        /// </summary>
        /// <returns>ID стиля</returns>
        public abstract int Id { get; }
        
        /// <summary>
        /// Получение наименования стиля
        /// </summary>
        /// <returns>Название стиля</returns>
        public abstract string Name { get; }
        
        /// <summary>
        /// Получение уникального идентификатора стиля (если поддерживается)
        /// </summary>
        /// <returns>GUID стиля или Guid.Empty если не поддерживается</returns>
        public virtual Guid UniqueId => Guid.Empty;
        
        /// <summary>
        /// Переопределенная отладочная информация для стилей
        /// </summary>
        /// <returns>Расширенная отладочная информация</returns>
        public override string GetDebugInfo()
        {
            return base.GetDebugInfo() + 
                   $"\n📝 Style ID: {Id}" +
                   $"\n🏷️ Style Name: {Name ?? "NULL"}";
        }
        
        /// <summary>
        /// Строковое представление стиля
        /// </summary>
        /// <returns>Информация о стиле</returns>
        public override string ToString()
        {
            return $"{GetType().Name} [ID: {Id}, Name: {Name ?? "NULL"}]";
        }
    }
}