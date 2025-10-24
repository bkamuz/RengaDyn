using System;
using System.Collections.Generic;
using System.Linq;
using Renga;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.Core
{
    /// <summary>
    /// Базовый класс для работы с параметрами и значениями в Renga
    /// Содержит общую логику для типизированного получения и установки значений
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса параметра</typeparam>
    /// <typeparam name="TValueType">Тип перечисления для типов значений</typeparam>
    [dr.IsVisibleInDynamoLibrary(false)]
    public abstract class BaseRengaParameter<TInterface, TValueType> : BaseRengaWrapper<TInterface>
        where TInterface : class
        where TValueType : struct, Enum
    {
        /// <summary>
        /// Конструктор базового параметра
        /// </summary>
        /// <param name="parameterInterface">COM-объект интерфейса параметра</param>
        protected BaseRengaParameter(object parameterInterface) : base(parameterInterface) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="parameterInterface">Типизированный интерфейс параметра</param>
        protected BaseRengaParameter(TInterface parameterInterface) : base(parameterInterface) { }
        
        /// <summary>
        /// Получение типа значения параметра
        /// </summary>
        /// <returns>Тип значения</returns>
        public abstract TValueType ValueType { get; }
        
        /// <summary>
        /// Проверка наличия значения у параметра
        /// </summary>
        /// <returns>true, если параметр имеет значение</returns>
        public abstract bool HasValue { get; }
        
        /// <summary>
        /// Безопасное получение булева значения с значением по умолчанию
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Булево значение или значение по умолчанию</returns>
        protected bool GetBoolValueSafe(bool defaultValue = false)
        {
            try
            {
                return HasValue ? GetBoolValueInternal() : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Безопасное получение целочисленного значения с значением по умолчанию
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Целочисленное значение или значение по умолчанию</returns>
        protected int GetIntValueSafe(int defaultValue = -1)
        {
            try
            {
                return HasValue ? GetIntValueInternal() : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Безопасное получение дробного значения с значением по умолчанию
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Дробное значение или значение по умолчанию</returns>
        protected double GetDoubleValueSafe(double defaultValue = -1.0)
        {
            try
            {
                return HasValue ? GetDoubleValueInternal() : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        /// Безопасное получение строкового значения с значением по умолчанию
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Строковое значение или значение по умолчанию</returns>
        protected string GetStringValueSafe(string defaultValue = null)
        {
            try
            {
                return HasValue ? GetStringValueInternal() : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        
        // ========== АБСТРАКТНЫЕ МЕТОДЫ ДЛЯ РЕАЛИЗАЦИИ В НАСЛЕДНИКАХ ==========
        
        /// <summary>
        /// Внутренняя реализация получения булева значения
        /// </summary>
        /// <returns>Булево значение</returns>
        protected abstract bool GetBoolValueInternal();
        
        /// <summary>
        /// Внутренняя реализация получения целочисленного значения
        /// </summary>
        /// <returns>Целочисленное значение</returns>
        protected abstract int GetIntValueInternal();
        
        /// <summary>
        /// Внутренняя реализация получения дробного значения
        /// </summary>
        /// <returns>Дробное значение</returns>
        protected abstract double GetDoubleValueInternal();
        
        /// <summary>
        /// Внутренняя реализация получения строкового значения
        /// </summary>
        /// <returns>Строковое значение</returns>
        protected abstract string GetStringValueInternal();
        
        /// <summary>
        /// Внутренняя реализация установки булева значения
        /// </summary>
        /// <param name="value">Булево значение</param>
        protected abstract void SetBoolValueInternal(bool value);
        
        /// <summary>
        /// Внутренняя реализация установки целочисленного значения
        /// </summary>
        /// <param name="value">Целочисленное значение</param>
        protected abstract void SetIntValueInternal(int value);
        
        /// <summary>
        /// Внутренняя реализация установки дробного значения
        /// </summary>
        /// <param name="value">Дробное значение</param>
        protected abstract void SetDoubleValueInternal(double value);
        
        /// <summary>
        /// Внутренняя реализация установки строкового значения
        /// </summary>
        /// <param name="value">Строковое значение</param>
        protected abstract void SetStringValueInternal(string value);
        
        // ========== УТИЛИТАРНЫЕ МЕТОДЫ ==========
        
        /// <summary>
        /// Получение строкового представления типа значения
        /// </summary>
        /// <param name="valueTypeDict">Словарь типов значений</param>
        /// <returns>Строковое представление типа</returns>
        protected string GetValueTypeAsString(Dictionary<string, object> valueTypeDict)
        {
            var currentValueType = ValueType;
            var match = valueTypeDict.FirstOrDefault(kvp => 
                kvp.Value.Equals(currentValueType));
            
            return match.Key ?? $"Unknown_{currentValueType}";
        }
        
        /// <summary>
        /// Универсальное получение значения по типу
        /// </summary>
        /// <returns>Значение соответствующего типа или null</returns>
        public abstract object GetValue();
        
        /// <summary>
        /// Универсальная установка значения по типу
        /// </summary>
        /// <param name="value">Значение для установки</param>
        public abstract void SetValue(object value);
    }
}