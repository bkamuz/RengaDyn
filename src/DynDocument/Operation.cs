using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Класс для работы с операциями (Renga.IOperation)
    /// </summary>
    public class Operation
    {
        public Renga.IOperation _i;
        
        /// <summary>
        /// Инициализация класса из COM-объекта Renga.IOperation
        /// </summary>
        /// <param name="operation_com">COM-объект IOperation</param>
        internal Operation(object operation_com)
        {
            this._i = operation_com as Renga.IOperation;
        }
        
        /// <summary>
        /// Запуск операции
        /// </summary>
        public void Start()
        {
            this._i.Start();
        }
        
        /// <summary>
        /// Применение операции (фиксация изменений)
        /// </summary>
        public void Apply()
        {
            this._i.Apply();
        }
        
        /// <summary>
        /// Откат операции (отмена изменений)
        /// </summary>
        public void Rollback()
        {
            this._i.Rollback();
        }
    }
}
