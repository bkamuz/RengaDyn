using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynDocument.Project
{
    /// <summary>
    /// Класс для работы с чертежом (интерфейсом Renga.IDrawing)
    /// </summary>
    public class Drawing
    {
        public Renga.IDrawing _i;
        /// <summary>When _i is null (e.g. entity from Drawings2 as IEntityCollection), store raw entity for GetModel().</summary>
        internal object _entityFallback;

        /// <summary>
        /// Инициация класса из интерфейса Renga.IDrawing
        /// </summary>
        /// <param name="Drawing_object"></param>
        internal Drawing(object Drawing_object)
        {
            this._i = Drawing_object as Renga.IDrawing;
            if (this._i == null && Drawing_object is Renga.IEntity)
                _entityFallback = Drawing_object;
        }
        //properties
        /// <summary>
        /// Наименование чертежа
        /// </summary>
        public string Name => this._i != null ? this._i.Name : throw new InvalidOperationException("Drawing interface is not initialized. Use GetModel() for model access.");
        /// <summary>
        /// Идентификатор чертежа
        /// </summary>
        public Guid Id => this._i != null ? this._i.Id : throw new InvalidOperationException("Drawing interface is not initialized. Use GetModel() for model access.");
        //functions
        /// <summary>
        /// Получение спецификаций на листе (интерфейс Renga.TitleBlockInstance)
        /// </summary>
        /// <returns></returns>
        public List<TitleBlockInstance> GetTitleBlockInstances()
        {
            List<TitleBlockInstance> os = new List<TitleBlockInstance>();
            for (int i = 0; i < this._i.TitleBlockInstanceCount; i++)
            {
                os.Add(new TitleBlockInstance(this._i.GetTitleBlockInstance(i)));
            }
            return os;
        }

        /// <summary>
        /// Returns the drawing model (IModel) for this drawing.
        /// Use to create drawing objects (DrawingText, DrawingImage, DrawingReferenceDrawing) on the sheet.
        /// When created from Drawings2 (IEntityCollection), gets IModel directly from the entity.
        /// </summary>
        /// <returns>IModel wrapper for the drawing model</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynRenga.RengaAPI.IModel GetModel()
        {
            if (this._i != null)
                return new DynRenga.RengaAPI.IModel(this);
            if (_entityFallback is Renga.IEntity entity)
            {
                var modelObj = entity.GetInterfaceByName("IModel");
                var rengaModel = modelObj as Renga.IModel;
                if (rengaModel != null)
                    return new DynRenga.RengaAPI.IModel(rengaModel);
            }
            throw new InvalidOperationException("Drawing interface is not initialized.");
        }
    }
}
