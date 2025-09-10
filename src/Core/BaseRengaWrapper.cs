using System;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.Core
{
    /// <summary>
    /// Базовый абстрактный класс для всех wrapper'ов интерфейсов Renga
    /// Устраняет дублирование конструкторов и базовой функциональности
    /// </summary>
    [dr.IsVisibleInDynamoLibrary(false)]
    public abstract class BaseRengaWrapper<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Внутренний интерфейс Renga
        /// </summary>
        public TInterface _i { get; protected set; }
        
        /// <summary>
        /// Базовый конструктор для wrapper'а интерфейса Renga
        /// </summary>
        /// <param name="rengaInterface">COM-объект интерфейса Renga</param>
        protected BaseRengaWrapper(object rengaInterface)
        {
            if (rengaInterface == null)
                throw new ArgumentNullException(nameof(rengaInterface));
            
            _i = rengaInterface as TInterface;
            if (_i == null)
                throw new ArgumentException($"Объект не может быть приведен к типу {typeof(TInterface).Name}", nameof(rengaInterface));
        }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="rengaInterface">Типизированный интерфейс Renga</param>
        protected BaseRengaWrapper(TInterface rengaInterface)
        {
            _i = rengaInterface ?? throw new ArgumentNullException(nameof(rengaInterface));
        }
        
        /// <summary>
        /// Проверка валидности wrapper'а
        /// </summary>
        /// <returns>true, если интерфейс инициализирован</returns>
        public virtual bool IsValid()
        {
            return _i != null;
        }
        
        /// <summary>
        /// Получение отладочной информации о wrapper'е
        /// </summary>
        /// <returns>Строка с отладочной информацией</returns>
        public virtual string GetDebugInfo()
        {
            return $"🔧 Wrapper Type: {GetType().Name}\n" +
                   $"📋 Interface Type: {typeof(TInterface).Name}\n" +
                   $"✅ Is Valid: {IsValid()}\n" +
                   $"🆔 Interface Hash: {_i?.GetHashCode().ToString("X8") ?? "NULL"}";
        }
    }
}