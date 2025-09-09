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
        
        /// <summary>
        /// Запуск операции с отладочной информацией
        /// </summary>
        /// <returns>Отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> StartWithDebug()
        {
            var debugInfo = "🔧 Starting Renga operation...\n";
            
            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ Operation interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                this._i.Start();
                debugInfo += "✅ Operation started successfully!\n";
                debugInfo += "💡 Changes are now being tracked\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to start operation!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }
        
        /// <summary>
        /// Применение операции с отладочной информацией
        /// </summary>
        /// <returns>Отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> ApplyWithDebug()
        {
            var debugInfo = "🔧 Applying Renga operation...\n";
            
            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ Operation interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                this._i.Apply();
                debugInfo += "✅ Operation applied successfully!\n";
                debugInfo += "💡 All changes have been committed to the model\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to apply operation!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }
        
        /// <summary>
        /// Откат операции с отладочной информацией
        /// </summary>
        /// <returns>Отладочная информация</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "Success", "DebugInfo" })]
        public Dictionary<string, object> RollbackWithDebug()
        {
            var debugInfo = "🔧 Rolling back Renga operation...\n";
            
            try
            {
                if (this._i == null)
                {
                    debugInfo += "❌ Operation interface is not initialized\n";
                    return new Dictionary<string, object>
                    {
                        { "Success", false },
                        { "DebugInfo", debugInfo }
                    };
                }
                
                this._i.Rollback();
                debugInfo += "✅ Operation rolled back successfully!\n";
                debugInfo += "💡 All changes have been discarded\n";
                
                return new Dictionary<string, object>
                {
                    { "Success", true },
                    { "DebugInfo", debugInfo }
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"❌ Failed to rollback operation!\n";
                debugInfo += $"Error: {ex.Message}\n";
                debugInfo += $"Stack Trace: {ex.StackTrace}";
                
                return new Dictionary<string, object>
                {
                    { "Success", false },
                    { "DebugInfo", debugInfo }
                };
            }
        }
    }
}
