using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;
using Renga;

namespace DynRenga.DynObjects
{
    /// <summary>
    /// Общие методы по работе с объектами как со структурами
    /// </summary>
    public class General
    {
        private General() { }
        
        /// <summary>
        /// Получение всех объектных идентификаторов
        /// </summary>
        /// <returns></returns>
        public static List<Guid> AllObjectTypes()
        {
            return ObjectTypes().Select(a => a.Value).ToList();
        }
        /// <summary>
        /// Типы объектов Ренга, для которых есть 3д-представление
        /// </summary>
        /// <returns></returns>
        public static List<Guid> Objects3dTypes()
        {
            List<Guid> objects = new List<Guid>();
            Dictionary<string, Guid> types = ObjectTypes();
            objects.Add(types["Beam"]);
            objects.Add(types["Column"]);
            objects.Add(types["Door"]);
            objects.Add(types["Duct"]); //
            objects.Add(types["Element"]);
            objects.Add(types["Equipment"]);
            objects.Add(types["Floor"]);
            objects.Add(types["IfcObject"]);
            objects.Add(types["IsolatedFoundation"]);
            objects.Add(types["MechanicalEquipment"]);
            objects.Add(types["Opening"]);
            objects.Add(types["Pipe"]);
            objects.Add(types["PipeAccessory"]);
            objects.Add(types["Plate"]);
            objects.Add(types["PlumbingFixture"]);
            objects.Add(types["Railing"]); //
            objects.Add(types["Ramp"]);
            objects.Add(types["Rebar"]);
            objects.Add(types["Roof"]);
            objects.Add(types["Room"]);
            objects.Add(types["Stair"]);
            objects.Add(types["Wall"]);
            objects.Add(types["WallFoundation"]);
            objects.Add(types["Window"]);
            objects.Add(types["PipeAccessory"]);
            objects.Add(types["PipeAccessory"]);

            return objects;
        }
        /// <summary>
        /// Типы объектов (перечисление)
        /// </summary>
        /// <returns>Словарь с типами объектов</returns>
        [dr.MultiReturn(new[] { "AssemblyInstance", "Axis", "Beam", "Column", "Door", "Duct", "DuctAccessory", "DuctFitting",
            "ElectricDistributionBoard", "Element", "Elevation", "Equipment", "Floor", "Hatch", "IfcObject", "IsolatedFoundation",
            "Level", "LightFixture", "Line3D", "LineElectricalCircuit", "MechanicalEquipment", "Opening", "Pipe", "PipeAccessory",
            "PipeFitting", "Plate", "PlumbingFixture", "Railing", "Ramp", "Rebar", "Roof", "Room", "Route", "RoutePoint", "Section",
            "Stair", "Undefined", "Wall", "WallFoundation", "Window", "WiringAccessory" })]
        public static Dictionary<string, Guid> ObjectTypes()
        {
            return new Dictionary<string, Guid>
            {
                { "AssemblyInstance",   DynRenga.RengaAPI.Constants.ObjectTypes.AssemblyInstance},
                { "Axis",   DynRenga.RengaAPI.Constants.ObjectTypes.Axis},
                { "Beam",   DynRenga.RengaAPI.Constants.ObjectTypes.Beam},
                { "Column", DynRenga.RengaAPI.Constants.ObjectTypes.Column},
                { "Door",   DynRenga.RengaAPI.Constants.ObjectTypes.Door},
                { "Duct",   DynRenga.RengaAPI.Constants.ObjectTypes.Duct},
                { "DuctAccessory",  DynRenga.RengaAPI.Constants.ObjectTypes.DuctAccessory},
                { "DuctFitting", DynRenga.RengaAPI.Constants.ObjectTypes.DuctFitting},
                { "ElectricDistributionBoard",  DynRenga.RengaAPI.Constants.ObjectTypes.ElectricDistributionBoard},
                { "Element",  DynRenga.RengaAPI.Constants.ObjectTypes.Element},
                { "Elevation",  DynRenga.RengaAPI.Constants.ObjectTypes.Elevation},
                { "Equipment",  DynRenga.RengaAPI.Constants.ObjectTypes.Equipment},
                { "Floor",  DynRenga.RengaAPI.Constants.ObjectTypes.Floor},
                { "Hatch",  DynRenga.RengaAPI.Constants.ObjectTypes.Hatch},
                { "IfcObject",  DynRenga.RengaAPI.Constants.ObjectTypes.IfcObject},
                { "IsolatedFoundation", DynRenga.RengaAPI.Constants.ObjectTypes.IsolatedFoundation},
                { "Level",  DynRenga.RengaAPI.Constants.ObjectTypes.Level},
                { "LightingFixture",   DynRenga.RengaAPI.Constants.ObjectTypes.LightingFixture},
                { "Line3D", DynRenga.RengaAPI.Constants.ObjectTypes.Line3D},
                { "LineElectricalCircuit",  DynRenga.RengaAPI.Constants.ObjectTypes.LineElectricalCircuit},
                { "MechanicalEquipment", DynRenga.RengaAPI.Constants.ObjectTypes.MechanicalEquipment},
                { "Opening",   DynRenga.RengaAPI.Constants.ObjectTypes.Opening},
                { "Pipe",   DynRenga.RengaAPI.Constants.ObjectTypes.Pipe},
                { "PipeAccessory",  DynRenga.RengaAPI.Constants.ObjectTypes.PipeAccessory},
                { "PipeFitting",  DynRenga.RengaAPI.Constants.ObjectTypes.PipeFitting},
                { "Plate",  DynRenga.RengaAPI.Constants.ObjectTypes.Plate},
                { "PlumbingFixture",  DynRenga.RengaAPI.Constants.ObjectTypes.PlumbingFixture},
                { "Railing", DynRenga.RengaAPI.Constants.ObjectTypes.Railing},
                { "Ramp",   DynRenga.RengaAPI.Constants.ObjectTypes.Ramp},
                { "Rebar",  DynRenga.RengaAPI.Constants.ObjectTypes.Rebar},
                { "Roof",   DynRenga.RengaAPI.Constants.ObjectTypes.Roof},
                { "Room",   DynRenga.RengaAPI.Constants.ObjectTypes.Room},
                { "Route",  DynRenga.RengaAPI.Constants.ObjectTypes.Route},
                { "RoutePoint", DynRenga.RengaAPI.Constants.ObjectTypes.RoutePoint},
                { "Section", DynRenga.RengaAPI.Constants.ObjectTypes.Section},
                { "Stair",  DynRenga.RengaAPI.Constants.ObjectTypes.Stair},
                { "Undefined",  DynRenga.RengaAPI.Constants.ObjectTypes.Undefined},
                { "Wall",   DynRenga.RengaAPI.Constants.ObjectTypes.Wall},
                { "WallFoundation", DynRenga.RengaAPI.Constants.ObjectTypes.WallFoundation},
                { "Window", DynRenga.RengaAPI.Constants.ObjectTypes.Window},
                { "WiringAccessory",DynRenga.RengaAPI.Constants.ObjectTypes.WiringAccessory},
            };
        }
        /// <summary>
        /// ВОзвращает тип объекта модели по его уникальному индентифакатору (Guid)
        /// </summary>
        /// <param name="obj_type_guid"></param>
        /// <returns></returns>
        public static string GetObjectIdAsString(Guid obj_type_guid)
        {
            Dictionary<string,Guid> data = new Dictionary<string, Guid>
            {
                { "AssemblyInstance",   DynRenga.RengaAPI.Constants.ObjectTypes.AssemblyInstance},
                { "Axis",   DynRenga.RengaAPI.Constants.ObjectTypes.Axis},
                { "Beam",   DynRenga.RengaAPI.Constants.ObjectTypes.Beam},
                { "Column", DynRenga.RengaAPI.Constants.ObjectTypes.Column},
                { "Door",   DynRenga.RengaAPI.Constants.ObjectTypes.Door},
                { "Duct",   DynRenga.RengaAPI.Constants.ObjectTypes.Duct},
                { "DuctAccessory",  DynRenga.RengaAPI.Constants.ObjectTypes.DuctAccessory},
                { "DuctFitting", DynRenga.RengaAPI.Constants.ObjectTypes.DuctFitting},
                { "ElectricDistributionBoard",  DynRenga.RengaAPI.Constants.ObjectTypes.ElectricDistributionBoard},
                { "Element",  DynRenga.RengaAPI.Constants.ObjectTypes.Element},
                { "Elevation",  DynRenga.RengaAPI.Constants.ObjectTypes.Elevation},
                { "Equipment",  DynRenga.RengaAPI.Constants.ObjectTypes.Equipment},
                { "Floor",  DynRenga.RengaAPI.Constants.ObjectTypes.Floor},
                { "Hatch",  DynRenga.RengaAPI.Constants.ObjectTypes.Hatch},
                { "IfcObject",  DynRenga.RengaAPI.Constants.ObjectTypes.IfcObject},
                { "IsolatedFoundation", DynRenga.RengaAPI.Constants.ObjectTypes.IsolatedFoundation},
                { "Level",  DynRenga.RengaAPI.Constants.ObjectTypes.Level},
                { "LightingFixture",   DynRenga.RengaAPI.Constants.ObjectTypes.LightingFixture},
                { "Line3D", DynRenga.RengaAPI.Constants.ObjectTypes.Line3D},
                { "LineElectricalCircuit",  DynRenga.RengaAPI.Constants.ObjectTypes.LineElectricalCircuit},
                { "MechanicalEquipment", DynRenga.RengaAPI.Constants.ObjectTypes.MechanicalEquipment},
                { "Opening",   DynRenga.RengaAPI.Constants.ObjectTypes.Opening},
                { "Pipe",   DynRenga.RengaAPI.Constants.ObjectTypes.Pipe},
                { "PipeAccessory",  DynRenga.RengaAPI.Constants.ObjectTypes.PipeAccessory},
                { "PipeFitting",  DynRenga.RengaAPI.Constants.ObjectTypes.PipeFitting},
                { "Plate",  DynRenga.RengaAPI.Constants.ObjectTypes.Plate},
                { "PlumbingFixture",  DynRenga.RengaAPI.Constants.ObjectTypes.PlumbingFixture},
                { "Railing", DynRenga.RengaAPI.Constants.ObjectTypes.Railing},
                { "Ramp",   DynRenga.RengaAPI.Constants.ObjectTypes.Ramp},
                { "Rebar",  DynRenga.RengaAPI.Constants.ObjectTypes.Rebar},
                { "Roof",   DynRenga.RengaAPI.Constants.ObjectTypes.Roof},
                { "Room",   DynRenga.RengaAPI.Constants.ObjectTypes.Room},
                { "Route",  DynRenga.RengaAPI.Constants.ObjectTypes.Route},
                { "RoutePoint", DynRenga.RengaAPI.Constants.ObjectTypes.RoutePoint},
                { "Section", DynRenga.RengaAPI.Constants.ObjectTypes.Section},
                { "Stair",  DynRenga.RengaAPI.Constants.ObjectTypes.Stair},
                { "Undefined",  DynRenga.RengaAPI.Constants.ObjectTypes.Undefined},
                { "Wall",   DynRenga.RengaAPI.Constants.ObjectTypes.Wall},
                { "WallFoundation", DynRenga.RengaAPI.Constants.ObjectTypes.WallFoundation},
                { "Window", DynRenga.RengaAPI.Constants.ObjectTypes.Window},
                { "WiringAccessory",DynRenga.RengaAPI.Constants.ObjectTypes.WiringAccessory},
            };
            return data.Where(a => a.Value == obj_type_guid).First().Key;
        }


    }
}
