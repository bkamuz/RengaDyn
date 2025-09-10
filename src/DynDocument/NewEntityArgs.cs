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

namespace DynRenga.DynDocument
{
    /// <summary>
    /// Класс для работы с аргументами создания новой сущности (Renga.INewEntityArgs)
    /// </summary>
    public class NewEntityArgs
    {
        public Renga.INewEntityArgs _i;
        
        /// <summary>
        /// Инициализация класса из COM-объекта Renga.INewEntityArgs
        /// </summary>
        /// <param name="new_entity_args_com">COM-объект INewEntityArgs</param>
        internal NewEntityArgs(object new_entity_args_com)
        {
            this._i = new_entity_args_com as Renga.INewEntityArgs;
        }
        
        /// <summary>
        /// Получение типа сущности
        /// </summary>
        public Guid TypeId
        {
            get { return this._i.TypeId; }
            set { this._i.TypeId = value; }
        }
        
        /// <summary>
        /// Получение типа сущности в виде строки
        /// </summary>
        public string TypeIdS
        {
            get { return this._i.TypeIdS; }
            set { this._i.TypeIdS = value; }
        }
        
        /// <summary>
        /// Получение ID категории
        /// </summary>
        public int CategoryId
        {
            get { return this._i.CategoryId; }
            set { this._i.CategoryId = value; }
        }
        
        /// <summary>
        /// Получение ID объекта-хозяина
        /// </summary>
        public int HostObjectId
        {
            get { return this._i.HostObjectId; }
            set { this._i.HostObjectId = value; }
        }
        
        /// <summary>
        /// Получение 3D размещения объекта
        /// </summary>
        public object Placement3D
        {
            get { return this._i.Placement3D; }
            set 
            { 
                // Handle both DynRenga.DynGeometry.Placement3D wrapper and direct Renga.Placement3D
                if (value is DynRenga.DynGeometry.Placement3D dynPlacement)
                {
                    
                    // Use the struct if available, otherwise try the COM interface
                    if (!dynPlacement._placement.Equals(default(Renga.Placement3D)))
                    {
                        this._i.Placement3D = dynPlacement._placement;
                    }
                    else if (dynPlacement._i != null)
                    {
                        this._i.Placement3D = dynPlacement._i.Placement;
                    }
                    else
                    {
                        throw new InvalidOperationException("Placement3D wrapper has neither COM interface nor struct data");
                    }
                }
                else if (value is Renga.Placement3D rengaPlacement)
                {
                    // Direct Renga.Placement3D struct
                    this._i.Placement3D = rengaPlacement;
                }
                else
                {
                    // Try direct cast as fallback
                    this._i.Placement3D = (Renga.Placement3D)value;
                }
            }
        }
    }
}
