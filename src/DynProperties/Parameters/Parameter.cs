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

namespace DynRenga.DynProperties.Parameters
{
    /// <summary>
    /// Рефакторенный класс для работы с одиночным параметром Renga.IParameter
    /// Наследует от BaseRengaParameter для устранения дублирования кода
    /// </summary>
    public class Parameter : BaseRengaParameter<Renga.IParameter, ParameterValueType>
    {
        /// <summary>
        /// Инициация класса из интерфейса Renga.IParameter
        /// </summary>
        /// <param name="parameterObject">COM-объект параметра</param>
        internal Parameter(object parameterObject) : base(parameterObject) { }
        
        /// <summary>
        /// Прямой конструктор с типизированным интерфейсом
        /// </summary>
        /// <param name="parameter">Интерфейс параметра</param>
        internal Parameter(Renga.IParameter parameter) : base(parameter) { }
        
        /// <summary>
        /// Проверка, есть ли какое-либо значение у параметра
        /// </summary>
        /// <returns>true, если параметр имеет значение</returns>
        public override bool HasValue => _i.HasValue;
        
        /// <summary>
        /// Тип значения параметра
        /// </summary>
        /// <returns>Тип значения параметра</returns>
        public override ParameterValueType ValueType => _i.ValueType;
        // ========== РЕАЛИЗАЦИЯ АБСТРАКТНЫХ МЕТОДОВ ==========
        
        /// <summary>
        /// Внутренняя реализация получения булева значения
        /// </summary>
        /// <returns>Булево значение параметра</returns>
        protected override bool GetBoolValueInternal()
        {
            return _i.GetBoolValue();
        }
        
        /// <summary>
        /// Внутренняя реализация получения целочисленного значения
        /// </summary>
        /// <returns>Целочисленное значение параметра</returns>
        protected override int GetIntValueInternal()
        {
            return _i.GetIntValue();
        }
        
        /// <summary>
        /// Внутренняя реализация получения дробного значения
        /// </summary>
        /// <returns>Дробное значение параметра</returns>
        protected override double GetDoubleValueInternal()
        {
            return _i.GetDoubleValue();
        }
        
        /// <summary>
        /// Внутренняя реализация получения строкового значения
        /// </summary>
        /// <returns>Строковое значение параметра</returns>
        protected override string GetStringValueInternal()
        {
            return _i.GetStringValue();
        }
        
        /// <summary>
        /// Внутренняя реализация установки булева значения
        /// </summary>
        /// <param name="value">Булево значение</param>
        protected override void SetBoolValueInternal(bool value)
        {
            _i.SetBoolValue(value);
        }
        
        /// <summary>
        /// Внутренняя реализация установки целочисленного значения
        /// </summary>
        /// <param name="value">Целочисленное значение</param>
        protected override void SetIntValueInternal(int value)
        {
            _i.SetIntValue(value);
        }
        
        /// <summary>
        /// Внутренняя реализация установки дробного значения
        /// </summary>
        /// <param name="value">Дробное значение</param>
        protected override void SetDoubleValueInternal(double value)
        {
            _i.SetDoubleValue(value);
        }
        
        /// <summary>
        /// Внутренняя реализация установки строкового значения
        /// </summary>
        /// <param name="value">Строковое значение</param>
        protected override void SetStringValueInternal(string value)
        {
            _i.SetStringValue(value);
        }
        
        // ========== ПУБЛИЧНЫЕ МЕТОДЫ ==========
        
        /// <summary>
        /// Получение булева значения параметра с безопасной обработкой
        /// </summary>
        /// <returns>Булево значение или false по умолчанию</returns>
        public bool GetBoolValue()
        {
            return GetBoolValueSafe();
        }
        
        /// <summary>
        /// Получение целочисленного значения параметра с безопасной обработкой
        /// </summary>
        /// <returns>Целочисленное значение или -1 по умолчанию</returns>
        public int GetIntValue()
        {
            return GetIntValueSafe();
        }
        
        /// <summary>
        /// Получение дробного значения параметра с безопасной обработкой
        /// </summary>
        /// <returns>Дробное значение или -1.0 по умолчанию</returns>
        public double GetDoubleValue()
        {
            return GetDoubleValueSafe();
        }
        
        /// <summary>
        /// Получение строкового значения параметра с безопасной обработкой
        /// </summary>
        /// <returns>Строковое значение или null по умолчанию</returns>
        public string GetStringValue()
        {
            return GetStringValueSafe();
        }
        
        /// <summary>
        /// Установка булева значения параметра
        /// </summary>
        /// <param name="value">Булево значение</param>
        public void SetBoolValue(bool value)
        {
            SetBoolValueInternal(value);
        }
        
        /// <summary>
        /// Установка целочисленного значения параметра
        /// </summary>
        /// <param name="value">Целочисленное значение</param>
        public void SetIntValue(int value)
        {
            SetIntValueInternal(value);
        }
        
        /// <summary>
        /// Установка дробного значения параметра
        /// </summary>
        /// <param name="value">Дробное значение</param>
        public void SetDoubleValue(double value)
        {
            SetDoubleValueInternal(value);
        }
        
        /// <summary>
        /// Установка строкового значения параметра
        /// </summary>
        /// <param name="value">Строковое значение</param>
        public void SetStringValue(string value)
        {
            SetStringValueInternal(value);
        }
        
        // ========== СВОЙСТВА ==========
        
        /// <summary>
        /// Получение Guid-идентификатора параметра
        /// </summary>
        /// <returns>GUID параметра</returns>
        public Guid Id => _i.Id;
        
        /// <summary>
        /// Получение определения параметра
        /// </summary>
        /// <returns>Объект ParameterDefinition</returns>
        public ParameterDefinition Definition => new ParameterDefinition(_i.Definition);
        // ========== УНИВЕРСАЛЬНЫЕ МЕТОДЫ ==========
        
        /// <summary>
        /// Универсальная установка значения по типу параметра
        /// </summary>
        /// <param name="value">Значение для установки</param>
        public override void SetValue(object value)
        {
            switch (ValueType)
            {
                case ParameterValueType.ParameterValueType_Bool:
                    SetBoolValueInternal(Convert.ToBoolean(value));
                    break;
                case ParameterValueType.ParameterValueType_Int:
                    SetIntValueInternal(Convert.ToInt32(value));
                    break;
                case ParameterValueType.ParameterValueType_Double:
                    SetDoubleValueInternal(Convert.ToDouble(value));
                    break;
                case ParameterValueType.ParameterValueType_String:
                    SetStringValueInternal(Convert.ToString(value));
                    break;
                default:
                    throw new ArgumentException($"Неподдерживаемый тип параметра: {ValueType}");
            }
        }
        
        /// <summary>
        /// Универсальное получение значения по типу параметра
        /// </summary>
        /// <returns>Значение параметра соответствующего типа или null</returns>
        public override object GetValue()
        {
            if (!HasValue) return null;
            
            return ValueType switch
            {
                ParameterValueType.ParameterValueType_Bool => GetBoolValueSafe(),
                ParameterValueType.ParameterValueType_Int => GetIntValueSafe(),
                ParameterValueType.ParameterValueType_Double => GetDoubleValueSafe(),
                ParameterValueType.ParameterValueType_String => GetStringValueSafe(),
                _ => null
            };
        }
        
        /// <summary>
        /// Получение строкового представления типа параметра
        /// </summary>
        /// <returns>Строковое представление типа</returns>
        public string GetValueTypeAsString()
        {
            return GetValueTypeAsString(GetParameterValueTypes());
        }
        
        /// <summary>
        /// Получение всех доступных типов значений параметров
        /// </summary>
        /// <returns>Словарь типов значений параметров</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, object> GetParameterValueTypes()
        {
            return new Dictionary<string, object>
            {
                { "ParameterValueType_Undefined", Renga.ParameterValueType.ParameterValueType_Undefined },
                { "ParameterValueType_Bool", Renga.ParameterValueType.ParameterValueType_Bool },
                { "ParameterValueType_Int", Renga.ParameterValueType.ParameterValueType_Int },
                { "ParameterValueType_Double", Renga.ParameterValueType.ParameterValueType_Double },
                { "ParameterValueType_String", Renga.ParameterValueType.ParameterValueType_String }
            };
        }
        
        /// <summary>
        /// (Устаревший) Получение типов значений параметров. Используйте GetParameterValueTypes
        /// </summary>
        [Obsolete("Используйте GetParameterValueTypes")]
        [dr.MultiReturn(new[] { "ParameterValueType_Undefined", "ParameterValueType_Bool",
            "ParameterValueType_Int","ParameterValueType_Double","ParameterValueType_String"})]
        public static Dictionary<string, object> ParameterValueTypes()
        {
            return GetParameterValueTypes();
        }
    }
}
