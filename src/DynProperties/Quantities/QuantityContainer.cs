using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynProperties.Quantities
{
    /// <summary>
    /// Класс для работы с перечнем расчетных свойств
    /// </summary>
    public class QuantityContainer
    {
        /// <summary>Внутренний COM-объект Renga.IQuantityContainer</summary>
        public Renga.IQuantityContainer _i;
        /// <summary>
        /// Получение набора расчетных свойств из интерфейса Renga
        /// </summary>
        /// <param name="renga_QuantityContainer_obj"></param>
        internal QuantityContainer(object renga_QuantityContainer_obj)
        {
            this._i = renga_QuantityContainer_obj as Renga.IQuantityContainer;
        }
        /// <summary>
        /// Проверка, содержит ли данный набор расчетных свойств указанный Guid-идентификатор
        /// </summary>
        /// <param name="prop_id"></param>
        /// <returns></returns>
        public bool Contains(Guid prop_id)
        {
            return this._i.Contains(prop_id);
        }
        /// <summary>
        /// Получение Renga.IQuantity по его Guid-идентификатору
        /// </summary>
        /// <param name="prop_id"></param>
        /// <returns></returns>
        public Quantity Get (Guid prop_id)
        {
            if (this._i.Contains(prop_id)) return new Quantity(this._i.Get(prop_id));
            else return null;
        }
        //My
        /// <summary>
        /// Получение всех Guid-идентификаторов расчетных параметров и 
        /// самих параметров в виде интерфейсов Renga.IQuantity
        /// </summary>
        /// <returns></returns>
        [dr.MultiReturn(new[] { "Quantites id", "Quantities objects (Renga.IQuantity)" })]
        public Dictionary<string, object> GetQuantities()
        {
            List<Guid> guids = new List<Guid>();
            List<object> quanities = new List<object>();
            foreach (KeyValuePair<string,Guid> prop2id in QuantityIdentifiers_Objects())
            {
                if (this._i.Contains(prop2id.Value)) 
                {
                    guids.Add(prop2id.Value);
                    quanities.Add(this._i.Get(prop2id.Value));
                }
            }
            return new Dictionary<string, object>()
            {
                {"Quantites id",guids },
                {"Quantities objects (Renga.IQuantity)",quanities }
            };

        }
        /// <summary>
        /// Получение наименования расчетного параметра по его Guid-идентификатору
        /// </summary>
        /// <param name="quantity_guid"></param>
        /// <returns></returns>
        public static string GetQuantityNameByGuid(object quantity_guid)
        {
            if (QuantityIdentifiers_Objects().Where(a => a.Value == (Guid)quantity_guid).Any())
            {
                return QuantityIdentifiers_Objects().Where(a => a.Value == (Guid)quantity_guid).First().Key;
            }
            else return null;
        }
        /// <summary>
        /// Типы расчетных свойств для всех категория объектов
        /// </summary>
        /// <returns></returns>
        [dr.MultiReturn(new[] { "NominalThickness", "NominalLength", "NominalWidth", "NominalHeight", "Perimeter",
                "OverallWidth", "OverallHeight", "OverallDepth", "OverallLength", "Volume", "NetVolume", "NetMass",
                "OuterSurfaceArea", "CrossSectionOverallWidth", "CrossSectionOverallHeight", "NetCrossSectionArea",
                "GrossCrossSectionArea", "GrossWallArea", "GrossCeilingArea", "Area", "NominalArea", "NetArea",
                "NetFootprintArea", "NetFloorArea", "NetSideArea", "NetPerimeter", "NetWallArea", "NetCeilingArea",
                "InnerSurfaceArea", "InnerSurfaceInternalArea", "InnerSurfaceExternalArea", "GlazingArea",
                "TotalSurfaceArea", "GrossArea", "GrossPerimeter", "GrossFloorArea", "GrossVolume", "NumberOfRiser",
                "NumberOfTreads", "TotalRebarLength", "TotalRebarMass", "RelativeObjectBottomElevation",
                "RelativeObjectTopElevation", "RelativeObjectBaselineBottomElevation", "RelativeObjectBaselineTopElevation", "SlopeAngle" })]
        public static Dictionary<string, Guid> QuantityIdentifiers_Objects()
        {
            return new Dictionary<string, Guid>
                {
                    {"NominalThickness",Renga.Quantities.NominalThickness},
                    {"NominalLength",Renga.Quantities.NominalLength},
                    {"NominalWidth",Renga.Quantities.NominalWidth},
                    {"NominalHeight",Renga.Quantities.NominalHeight},
                    {"Perimeter",Renga.Quantities.Perimeter},
                    {"OverallWidth",Renga.Quantities.OverallWidth},
                    {"OverallHeight",Renga.Quantities.OverallHeight},
                    {"OverallDepth",Renga.Quantities.OverallDepth},
                    {"OverallLength",Renga.Quantities.OverallLength},
                    {"Volume",Renga.Quantities.Volume},
                    {"NetVolume",Renga.Quantities.NetVolume},
                    {"NetMass",Renga.Quantities.NetMass},
                    {"OuterSurfaceArea",Renga.Quantities.OuterSurfaceArea},
                    {"CrossSectionOverallWidth",Renga.Quantities.CrossSectionOverallWidth},
                    {"CrossSectionOverallHeight",Renga.Quantities.CrossSectionOverallHeight},
                    {"NetCrossSectionArea",Renga.Quantities.NetCrossSectionArea},
                    {"GrossCrossSectionArea",Renga.Quantities.GrossCrossSectionArea},
                    {"GrossWallArea",Renga.Quantities.GrossWallArea},
                    {"GrossCeilingArea",Renga.Quantities.GrossCeilingArea},
                    {"Area",Renga.Quantities.Area},
                    {"NominalArea",Renga.Quantities.NominalArea},
                    {"NetArea",Renga.Quantities.NetArea},
                    {"NetFootprintArea",Renga.Quantities.NetFootprintArea},
                    {"NetFloorArea",Renga.Quantities.NetFloorArea},
                    {"NetSideArea",Renga.Quantities.NetSideArea},
                    {"NetPerimeter",Renga.Quantities.NetPerimeter},
                    {"NetWallArea",Renga.Quantities.NetWallArea},
                    {"NetCeilingArea",Renga.Quantities.NetCeilingArea},
                    {"InnerSurfaceArea",Renga.Quantities.InnerSurfaceArea},
                    {"InnerSurfaceInternalArea",Renga.Quantities.InnerSurfaceInternalArea},
                    {"InnerSurfaceExternalArea",Renga.Quantities.InnerSurfaceExternalArea},
                    {"GlazingArea",Renga.Quantities.GlazingArea},
                    {"TotalSurfaceArea",Renga.Quantities.TotalSurfaceArea},
                    {"GrossArea",Renga.Quantities.GrossArea},
                    {"GrossPerimeter",Renga.Quantities.GrossPerimeter},
                    {"GrossFloorArea",Renga.Quantities.GrossFloorArea},
                    {"GrossVolume",Renga.Quantities.GrossVolume},
                    {"NumberOfRiser",Renga.Quantities.NumberOfRiser},
                    {"NumberOfTreads",Renga.Quantities.NumberOfTreads},
                    {"TotalRebarLength",Renga.Quantities.TotalRebarLength},
                    {"TotalRebarMass",Renga.Quantities.TotalRebarMass},
                    {"RelativeObjectBottomElevation",Renga.Quantities.RelativeObjectBottomElevation},
                    {"RelativeObjectTopElevation",Renga.Quantities.RelativeObjectTopElevation},
                    {"RelativeObjectBaselineBottomElevation",Renga.Quantities.RelativeObjectBaselineBottomElevation},
                    {"RelativeObjectBaselineTopElevation",Renga.Quantities.RelativeObjectBaselineTopElevation},
                    {"SlopeAngle",Renga.Quantities.SlopeAngle}
                };
        }

    }
}
