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

namespace DynRenga.DynObjects
{
    /// <summary>
    /// Класс для работы с интерфейсом Renga.IBaseline2DObject
    /// Представляет объект с 2D базовой линией
    /// </summary>
    public class Baseline2DObject
    {
        /// <summary>
        /// Внутренний COM-объект Renga.IBaseline2DObject
        /// </summary>
        public Renga.IBaseline2DObject _i;

        /// <summary>
        /// Инициализация класса через интерфейс Renga.IBaseline2DObject
        /// </summary>
        /// <param name="Baseline2DObject_object"></param>
        internal Baseline2DObject(object Baseline2DObject_object)
        {
            this._i = Baseline2DObject_object as Renga.IBaseline2DObject;
        }

        /// <summary>
        /// Получение Baseline2DObject из ModelObject
        /// Возвращает null если объект не поддерживает интерфейс IBaseline2DObject
        /// </summary>
        /// <param name="modelObject">Объект модели</param>
        /// <returns>Baseline2DObject или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Baseline2DObject ByModelObject(ModelObject modelObject)
        {
            if (modelObject == null) return null;
            return modelObject.GetBaseline2DObject();
        }

        /// <summary>
        /// Получение копии 2D базовой линии объекта в его собственной системе координат
        /// </summary>
        /// <returns>Кривая 2D, представляющая базовую линию</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetBaseline()
        {
            return new Curve2D(this._i.GetBaseline());
        }

        /// <summary>
        /// Получение копии 2D базовой линии объекта в указанной системе координат
        /// </summary>
        /// <param name="placement2D">Система координат, в которую будет преобразована 2D базовая линия</param>
        /// <returns>Кривая 2D, представляющая базовую линию в указанной системе координат</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Curve2D GetBaselineInCS(DynGeometry.Placement2D placement2D)
        {
            return new Curve2D(this._i.GetBaselineInCS(placement2D.ToRengaPlacement2D()));
        }

        /// <summary>
        /// Установка 2D базовой линии объекта в его собственной системе координат
        /// Ограничения: Невозможно редактировать базовую линию объекта с зависимыми объектами (например, Крыша)
        /// </summary>
        /// <param name="baseline">Новая базовая линия объекта</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBaseline(Curve2D baseline)
        {
            this._i.SetBaseline(baseline._i);
        }

        /// <summary>
        /// Установка 2D базовой линии объекта в указанной системе координат
        /// Ограничения: Невозможно редактировать базовую линию объекта с зависимыми объектами (например, Крыша)
        /// </summary>
        /// <param name="placement2D">Система координат, в которую будет преобразована 2D базовая линия</param>
        /// <param name="baselineInCS">Новая базовая линия объекта в указанной системе координат</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void SetBaselineInCS(DynGeometry.Placement2D placement2D, Curve2D baselineInCS)
        {
            this._i.SetBaselineInCS(placement2D.ToRengaPlacement2D(), baselineInCS._i);
        }

        /// <summary>
        /// Получение базовой линии в указанной системе координат с преобразованием в Dynamo
        /// </summary>
        /// <param name="placement2D">Система координат</param>
        /// <returns>Dynamo кривая в указанной СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.Curve GetBaselineInCSAsDynamo(DynGeometry.Placement2D placement2D)
        {
            Curve2D baselineInCS = this.GetBaselineInCS(placement2D);
            if (baselineInCS == null) return null;
            
            // Use the curve type-aware conversion
            return Curve2D.CreateDynamoCurveFromSegment(baselineInCS, 2);
        }

        /// <summary>
        /// Получение базовой линии в глобальной системе координат как Dynamo кривая
        /// </summary>
        /// <returns>Dynamo кривая в глобальной СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.Curve GetBaselineInGlobalCS()
        {
            Curve2D baseline = this.GetBaseline();
            if (baseline == null) return null;
            
            // Use the curve type-aware conversion
            return Curve2D.CreateDynamoCurveFromSegment(baseline, 2);
        }

        /// <summary>
        /// Получение системы координат объекта (если доступна)
        /// </summary>
        /// <returns>Система координат объекта или null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public DynGeometry.Placement2D GetObjectCoordinateSystem()
        {
            try
            {
                // Try to get the object's coordinate system
                // This might need to be implemented based on your specific object type
                return null; // Placeholder - implement based on your needs
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Преобразование базовой линии в Dynamo с использованием указанной системы координат
        /// </summary>
        /// <param name="targetCoordinateSystem">Целевая система координат</param>
        /// <returns>Dynamo кривая в целевой СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public dg.Curve TransformBaselineToCoordinateSystem(DynGeometry.Placement2D targetCoordinateSystem)
        {
            return this.GetBaselineInCSAsDynamo(targetCoordinateSystem);
        }

        /// <summary>
        /// Получение информации о системе координат базовой линии
        /// </summary>
        /// <returns>Строка с информацией о СК</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string GetCoordinateSystemInfo()
        {
            try
            {
                var info = new System.Text.StringBuilder();
                
                // Get baseline in object's coordinate system
                Curve2D baseline = this.GetBaseline();
                if (baseline != null)
                {
                    info.AppendLine("Object's Coordinate System:");
                    info.AppendLine($"Baseline Type: {baseline.Curve2DType}");
                    info.AppendLine($"Is Closed: {baseline.IsClosed}");
                    info.AppendLine($"Length: {baseline._i.GetLength() / 1000.0:F3} m");
                }
                
                return info.ToString();
            }
            catch (System.Exception ex)
            {
                return $"Error getting coordinate system info: {ex.Message}";
            }
        }
    }
}
