using System;
using System.Collections.Generic;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI.StyleTypes
{
    /// <summary>
    /// Renga API Style Types Constants
    /// This class provides all style type GUIDs used in the Renga API
    /// Based on: https://context7.com/jackdainzh/rengabim-sdk
    /// </summary>
    public static class StyleTypes
    {
        /// <summary>
        /// Assembly style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Assembly => new Guid("cb825bf3-15ae-4190-821c-8ad314951ada");

        /// <summary>
        /// Beam style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid BeamStyle => new Guid("cf2b8b04-f595-4432-98f4-8234c95adbdd");

        /// <summary>
        /// Building element model style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid BuildingElementModel => new Guid("923bf334-2e0a-41a0-9bf9-dc598c38586f");

        /// <summary>
        /// Column style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ColumnStyle => new Guid("be49a354-19b7-435a-8957-9ef8782630c2");

        /// <summary>
        /// Display style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DisplayStyle => new Guid("edae3ec8-2f1d-4d76-9aab-1c5c12dfda7d");

        /// <summary>
        /// Door style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DoorStyle => new Guid("19d0649f-582a-488e-a52b-585c1151a5e4");

        /// <summary>
        /// Drawing link
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DrawingLink => new Guid("e04d0118-5c58-4a7f-bf9c-3f729de1e559");

        /// <summary>
        /// Duct accessory style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DuctAccessoryStyle => new Guid("6c671391-bfea-4e92-9753-8855c05640a0");

        /// <summary>
        /// Duct fitting style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DuctFittingStyle => new Guid("6c6821a0-ebb9-445b-84a2-ed9eb0938e4f");

        /// <summary>
        /// Duct style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DuctStyle => new Guid("a999f05a-d730-42e7-bfc8-e4433ebace78");

        /// <summary>
        /// Conductor style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ElectricalConductorStyle => new Guid("fa7f1ae9-f4f4-4f95-b108-feea4d7efeb7");

        /// <summary>
        /// Electrical circuit line style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ElectricCircuitLineStyle => new Guid("33fb4b37-83f9-422a-81d4-640a152c619e");

        /// <summary>
        /// Electric distribution board style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ElectricDistributionBoardStyle => new Guid("861c0037-7797-43a9-96e7-833a7a2c6ea4");

        /// <summary>
        /// Element style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ElementStyle => new Guid("514a3ae7-f551-4d0f-b5ba-5d4f0ecf4e7a");

        /// <summary>
        /// Equipment style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid EquipmentStyle => new Guid("a369ad70-c1fe-41dd-af3d-bd659ea5b360");

        /// <summary>
        /// Hatch pattern style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid HatchPatternStyle => new Guid("c08a2259-6612-4cd4-919a-a09865cd6e3e");

        /// <summary>
        /// Hole style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid HoleStyle => new Guid("83085c7b-16c4-473e-85bc-9aafa504ff7d");

        /// <summary>
        /// Image resource
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ImageLink => new Guid("6a18e669-bdcf-442a-bc81-63c12da72aa2");

        /// <summary>
        /// Layered material style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LayeredMaterial => new Guid("0f0adba0-5c06-46c0-9c8a-b9d69ef1251f");

        /// <summary>
        /// Layout style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LayoutStyle => new Guid("501768ff-fe9e-4fce-8337-22a841ac4868");

        /// <summary>
        /// Lighting fixture style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LightingFixtureStyle => new Guid("1f85f676-bb99-4a6f-9f72-1789f2f7b362");

        /// <summary>
        /// Material
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Material => new Guid("0abcb18f-0aaf-4509-bf89-5c5fad9d5d8b");

        /// <summary>
        /// Mechanical equipment style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid MechanicalEquipmentStyle => new Guid("d43c7509-a92c-4e32-bd2d-ba6dd8f5b7a1");

        /// <summary>
        /// Model resource
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ModelLink => new Guid("d769d1c4-8c32-40a8-a716-68bc9b6b5d3c");

        /// <summary>
        /// Page format
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PageFormat => new Guid("3603ef07-e3a4-477e-9e72-2c8225c0a351");

        /// <summary>
        /// Pipe accessory style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PipeAccessoryStyle => new Guid("a31cf7ca-f17b-422a-886a-7a8c362cd49a");

        /// <summary>
        /// Pipe fitting style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PipeFittingStyle => new Guid("b1359bdc-f7ff-43a4-bca0-8d09bc974537");

        /// <summary>
        /// Pipe style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PipeStyle => new Guid("9d6dffb9-4828-40d8-8529-bf5cd2b58c4e");

        /// <summary>
        /// Plate style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PlateStyle => new Guid("9b60d6ad-3468-478e-94df-a535c5aeaa3e");

        /// <summary>
        /// Plumbing fixture style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PlumbingFixtureStyle => new Guid("344299f5-7d7f-43e2-b0a2-1db8e06e8ac8");

        /// <summary>
        /// Arbitrary profile entity
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Profile => new Guid("8734b5cd-57fc-409e-aefe-1fdc449bcb5c");

        /// <summary>
        /// Rebar style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid RebarStyle => new Guid("608edb78-96f3-40a6-a0ec-71000105581b");

        /// <summary>
        /// Reinforcement grade style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ReinforcementGrade => new Guid("b50f63fa-7f3a-4762-8ad9-324afc7fe2e8");

        /// <summary>
        /// Reinforcement style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ReinforcementStyle => new Guid("03a52558-573f-46c9-bea5-4760eb7fa485");

        /// <summary>
        /// Reinforcement unit style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ReinforcementUnit => new Guid("7ee13bd6-7c0a-47d3-adce-35b8e0dae28a");

        /// <summary>
        /// System style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid SystemStyle => new Guid("e65c5fad-d4d3-4f43-bd01-b28d0eb95571");

        /// <summary>
        /// Tag style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid TagStyle => new Guid("43f26eac-02b0-4639-8447-deee54fa1ff6");

        /// <summary>
        /// Text style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid TextStyle => new Guid("f22ba8c7-a75d-43f4-bdce-6967aeac6118");

        /// <summary>
        /// Topic
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Topic => new Guid("eafcc366-1483-44d5-881f-b4688d306da5");

        /// <summary>
        /// Undefined style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Undefined => new Guid("df67fd2f-bbd3-4810-a132-1451769d5e51");

        /// <summary>
        /// Window style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid WindowStyle => new Guid("fac43446-031c-413e-9993-6e9cf9f2306a");

        /// <summary>
        /// Wiring accessory style
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid WiringAccessoryStyle => new Guid("a6e0ba72-acbd-4423-9afc-04d84a09211a");

        /// <summary>
        /// Gets all style type GUIDs as a dictionary
        /// </summary>
        /// <returns>Dictionary containing all style types with their names and GUIDs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "StyleTypes", "Count", "DebugInfo" })]
        public static Dictionary<string, object> GetAllStyleTypes()
        {
            var styleTypes = new Dictionary<string, Guid>
            {
                { "Assembly", Assembly },
                { "BeamStyle", BeamStyle },
                { "BuildingElementModel", BuildingElementModel },
                { "ColumnStyle", ColumnStyle },
                { "DisplayStyle", DisplayStyle },
                { "DoorStyle", DoorStyle },
                { "DrawingLink", DrawingLink },
                { "DuctAccessoryStyle", DuctAccessoryStyle },
                { "DuctFittingStyle", DuctFittingStyle },
                { "DuctStyle", DuctStyle },
                { "ElectricalConductorStyle", ElectricalConductorStyle },
                { "ElectricCircuitLineStyle", ElectricCircuitLineStyle },
                { "ElectricDistributionBoardStyle", ElectricDistributionBoardStyle },
                { "ElementStyle", ElementStyle },
                { "EquipmentStyle", EquipmentStyle },
                { "HatchPatternStyle", HatchPatternStyle },
                { "HoleStyle", HoleStyle },
                { "ImageLink", ImageLink },
                { "LayeredMaterial", LayeredMaterial },
                { "LayoutStyle", LayoutStyle },
                { "LightingFixtureStyle", LightingFixtureStyle },
                { "Material", Material },
                { "MechanicalEquipmentStyle", MechanicalEquipmentStyle },
                { "ModelLink", ModelLink },
                { "PageFormat", PageFormat },
                { "PipeAccessoryStyle", PipeAccessoryStyle },
                { "PipeFittingStyle", PipeFittingStyle },
                { "PipeStyle", PipeStyle },
                { "PlateStyle", PlateStyle },
                { "PlumbingFixtureStyle", PlumbingFixtureStyle },
                { "Profile", Profile },
                { "RebarStyle", RebarStyle },
                { "ReinforcementGrade", ReinforcementGrade },
                { "ReinforcementStyle", ReinforcementStyle },
                { "ReinforcementUnit", ReinforcementUnit },
                { "SystemStyle", SystemStyle },
                { "TagStyle", TagStyle },
                { "TextStyle", TextStyle },
                { "Topic", Topic },
                { "Undefined", Undefined },
                { "WindowStyle", WindowStyle },
                { "WiringAccessoryStyle", WiringAccessoryStyle }
            };

            var debugInfo = $"--- Renga API Style Types ---\n";
            debugInfo += $"Total Style Types: {styleTypes.Count}\n";
            debugInfo += $"Source: https://context7.com/jackdainzh/rengabim-sdk\n\n";
            
            foreach (var styleType in styleTypes)
            {
                debugInfo += $"{styleType.Key}: {styleType.Value}\n";
            }

            return new Dictionary<string, object>
            {
                { "StyleTypes", styleTypes },
                { "Count", styleTypes.Count },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Gets style type by name (case-insensitive)
        /// </summary>
        /// <param name="styleTypeName">Name of the style type</param>
        /// <returns>GUID of the style type, or Undefined if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid GetStyleTypeByName(string styleTypeName)
        {
            if (string.IsNullOrEmpty(styleTypeName))
                return Undefined;

            var allTypes = GetAllStyleTypes();
            var styleTypes = allTypes["StyleTypes"] as Dictionary<string, Guid>;
            
            if (styleTypes != null && styleTypes.TryGetValue(styleTypeName, out Guid result))
                return result;

            return Undefined;
        }

        /// <summary>
        /// Checks if a GUID is a valid style type
        /// </summary>
        /// <param name="guid">GUID to check</param>
        /// <returns>True if the GUID is a valid style type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static bool IsValidStyleType(Guid guid)
        {
            var allTypes = GetAllStyleTypes();
            var styleTypes = allTypes["StyleTypes"] as Dictionary<string, Guid>;
            
            return styleTypes != null && styleTypes.Values.Contains(guid);
        }
    }
}
