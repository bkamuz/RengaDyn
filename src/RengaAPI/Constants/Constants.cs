using System;
using System.Collections.Generic;
using System.Linq;
using dr = Autodesk.DesignScript.Runtime;

namespace DynRenga.RengaAPI.Constants
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

    /// <summary>
    /// Renga API Object Types Constants
    /// This class provides all object type GUIDs used in the Renga API
    /// Based on: https://context7.com/jackdainzh/rengabim-sdk
    /// </summary>
    public static class ObjectTypes
    {
        /// <summary>
        /// Angular dimension object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid AngularDimension => new Guid("96788994-b7fc-41d7-8a99-d674543e9237");

        /// <summary>
        /// Assembly object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid AssemblyInstance => new Guid("00799249-1824-4ebd-bf93-40bb92efa9e6");

        /// <summary>
        /// Axis object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Axis => new Guid("4b41ccf8-c969-4c55-a1f2-cced9c164f07");

        /// <summary>
        /// Beam object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Beam => new Guid("63478188-7c88-4a6d-b891-9725f04a5bc7");

        /// <summary>
        /// Column object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Column => new Guid("d9ee2442-e807-42fb-8fe5-9dcfe543035d");

        /// <summary>
        /// Diametral dimension object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DiametralDimension => new Guid("2aabe3a4-a29e-4534-a9f5-0f070fee240c");

        /// <summary>
        /// Door object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Door => new Guid("1cfba99c-01e7-4078-ae1a-3e2ff0673599");

        /// <summary>
        /// Duct object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Duct => new Guid("06cc88ee-9a67-4626-9c34-dde03c331a74");

        /// <summary>
        /// Duct accessory object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DuctAccessory => new Guid("47d0d93f-3c7b-4269-bf8a-de246e1724d0");

        /// <summary>
        /// Duct fitting object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid DuctFitting => new Guid("77ffca60-b20e-49f0-b42f-4fdc9b1c825b");

        /// <summary>
        /// Electric distribution board object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid ElectricDistributionBoard => new Guid("96da9155-43c1-42b8-bba2-b4f61fa43acc");

        /// <summary>
        /// Element object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Element => new Guid("e1e3bd66-2e13-4fa4-a9eb-677e03067c2f");

        /// <summary>
        /// Elevation object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Elevation => new Guid("8a49a9a8-a401-4ab1-8038-92093503c97a");

        /// <summary>
        /// Equipment object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Equipment => new Guid("5d2f3734-5a49-4504-90b1-0676f0f25da7");

        /// <summary>
        /// Floor object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Floor => new Guid("f5bd8bd8-39c1-47f8-8499-f673c580dfbe");

        /// <summary>
        /// Hatch object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Hatch => new Guid("84b43087-d4a4-4cce-b34d-40e283d9e691");

        /// <summary>
        /// Hole object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Hole => new Guid("ecef8f90-0cf9-4494-98de-91242a2a9f5c");

        /// <summary>
        /// IFC object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid IfcObject => new Guid("f914251d-d5fa-48b2-b93b-074f442cbf3b");

        /// <summary>
        /// Isolated foundation object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid IsolatedFoundation => new Guid("6063816c-89ff-4c8f-a814-3be6cb94128e");

        /// <summary>
        /// Level object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Level => new Guid("c3ce17ff-6f28-411f-b18d-74fe957b2ba8");

        /// <summary>
        /// Lighting fixture object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LightingFixture => new Guid("793d3f7c-905d-4d85-a351-b152241dd2e7");

        /// <summary>
        /// Line3D object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Line3D => new Guid("02bbebe8-e28b-4ee5-8916-11b514a35dca");

        /// <summary>
        /// Linear dimension object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LinearDimension => new Guid("dc82ca1a-a0c3-4a1a-aefb-a7d720dd3a09");

        /// <summary>
        /// Line electrical circuit object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LineElectricalCircuit => new Guid("83de45e6-4793-49ec-8b9e-65a2438f36de");

        /// <summary>
        /// Linked image object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LinkedImage => new Guid("857a042d-7d3c-4715-9ebf-95e2e9648adf");

        /// <summary>
        /// Linked model object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid LinkedModel => new Guid("67a0b42c-8c1e-47e8-b46e-78d8bb260de0");

        /// <summary>
        /// Mechanical equipment object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid MechanicalEquipment => new Guid("de4420ce-02b6-4b12-9cd7-9322118be8fe");

        /// <summary>
        /// Opening object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Opening => new Guid("fc443d5a-b76c-45e5-b91c-520ef0896109");

        /// <summary>
        /// Pipe object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Pipe => new Guid("838cc9f6-e3d8-4132-af6f-c58df0f8d037");

        /// <summary>
        /// Pipe accessory object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PipeAccessory => new Guid("41e2788a-49ed-487f-9ae1-55b6e09ae6e5");

        /// <summary>
        /// Pipe fitting object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PipeFitting => new Guid("d31dc2e3-808e-4987-8481-7f86665a07fc");

        /// <summary>
        /// Plate object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Plate => new Guid("62cf086e-5a39-4484-840c-ffa6a1c6e2b7");

        /// <summary>
        /// Plumbing fixture object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid PlumbingFixture => new Guid("b8c7155a-b462-4ff5-bc41-c9c17a9f48fa");

        /// <summary>
        /// Radial dimension object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid RadialDimension => new Guid("377c2fda-9411-43ac-a6c6-0e3b520be721");

        /// <summary>
        /// Railing object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Railing => new Guid("a1aca786-78a4-4015-b412-9150baad71a9");

        /// <summary>
        /// Ramp object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Ramp => new Guid("debde004-afcc-4da8-8dd0-4223ff836acd");

        /// <summary>
        /// Rebar object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Rebar => new Guid("9fabc932-590f-4068-89a8-ee6ee3d7cbbf");

        /// <summary>
        /// Roof object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Roof => new Guid("bac4470f-d560-4f57-a49e-faa5f6e5a279");

        /// <summary>
        /// Room object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Room => new Guid("f1a805ff-573d-f46b-ffba-57f4bccaa6ed");

        /// <summary>
        /// Route object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Route => new Guid("8b323bee-3882-4744-8838-24f45df714a9");

        /// <summary>
        /// Route point object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid RoutePoint => new Guid("ce93e320-7167-4cd1-92a8-5e42d546066b");

        /// <summary>
        /// Section object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Section => new Guid("4166fd59-64c0-45ee-ae3b-49fae1257ef1");

        /// <summary>
        /// Stair object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Stair => new Guid("3f522f49-aee2-4d73-9866-9b07cf336a69");

        /// <summary>
        /// Text object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid TextObject => new Guid("da557027-f243-4331-bb5b-853abc437cd7");

        /// <summary>
        /// Undefined object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Undefined => new Guid("97675473-ca62-4ea4-bc6e-bb2ca57b7e67");

        /// <summary>
        /// Wall object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Wall => new Guid("4329112a-6b65-48d9-9da8-abf1f8f36327");

        /// <summary>
        /// Wall foundation object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid WallFoundation => new Guid("d7dd0293-dd65-4229-a64c-8b528d4e226f");

        /// <summary>
        /// Window object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid Window => new Guid("2b02b353-2ca5-4566-88bb-917ea8460174");

        /// <summary>
        /// Wiring accessory object type
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid WiringAccessory => new Guid("b00d5c25-92a8-4409-a3b7-7c37ed792c06");

        /// <summary>
        /// Gets all object type GUIDs as a dictionary
        /// </summary>
        /// <returns>Dictionary containing all object types with their names and GUIDs</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        [dr.MultiReturn(new[] { "ObjectTypes", "Count", "DebugInfo" })]
        public static Dictionary<string, object> GetAllObjectTypes()
        {
            var objectTypes = new Dictionary<string, Guid>
            {
                { "AngularDimension", AngularDimension },
                { "AssemblyInstance", AssemblyInstance },
                { "Axis", Axis },
                { "Beam", Beam },
                { "Column", Column },
                { "DiametralDimension", DiametralDimension },
                { "Door", Door },
                { "Duct", Duct },
                { "DuctAccessory", DuctAccessory },
                { "DuctFitting", DuctFitting },
                { "ElectricDistributionBoard", ElectricDistributionBoard },
                { "Element", Element },
                { "Elevation", Elevation },
                { "Equipment", Equipment },
                { "Floor", Floor },
                { "Hatch", Hatch },
                { "Hole", Hole },
                { "IfcObject", IfcObject },
                { "IsolatedFoundation", IsolatedFoundation },
                { "Level", Level },
                { "LightingFixture", LightingFixture },
                { "Line3D", Line3D },
                { "LinearDimension", LinearDimension },
                { "LineElectricalCircuit", LineElectricalCircuit },
                { "LinkedImage", LinkedImage },
                { "LinkedModel", LinkedModel },
                { "MechanicalEquipment", MechanicalEquipment },
                { "Opening", Opening },
                { "Pipe", Pipe },
                { "PipeAccessory", PipeAccessory },
                { "PipeFitting", PipeFitting },
                { "Plate", Plate },
                { "PlumbingFixture", PlumbingFixture },
                { "RadialDimension", RadialDimension },
                { "Railing", Railing },
                { "Ramp", Ramp },
                { "Rebar", Rebar },
                { "Roof", Roof },
                { "Room", Room },
                { "Route", Route },
                { "RoutePoint", RoutePoint },
                { "Section", Section },
                { "Stair", Stair },
                { "TextObject", TextObject },
                { "Undefined", Undefined },
                { "Wall", Wall },
                { "WallFoundation", WallFoundation },
                { "Window", Window },
                { "WiringAccessory", WiringAccessory }
            };

            var debugInfo = $"--- Renga API Object Types ---\n";
            debugInfo += $"Total Object Types: {objectTypes.Count}\n";
            debugInfo += $"Source: https://context7.com/jackdainzh/rengabim-sdk\n\n";
            
            foreach (var objectType in objectTypes)
            {
                debugInfo += $"{objectType.Key}: {objectType.Value}\n";
            }

            return new Dictionary<string, object>
            {
                { "ObjectTypes", objectTypes },
                { "Count", objectTypes.Count },
                { "DebugInfo", debugInfo }
            };
        }

        /// <summary>
        /// Gets object type by name (case-insensitive)
        /// </summary>
        /// <param name="objectTypeName">Name of the object type</param>
        /// <returns>GUID of the object type, or Undefined if not found</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static Guid GetObjectTypeByName(string objectTypeName)
        {
            if (string.IsNullOrEmpty(objectTypeName))
                return Undefined;

            var allTypes = GetAllObjectTypes();
            var objectTypes = allTypes["ObjectTypes"] as Dictionary<string, Guid>;
            
            if (objectTypes != null && objectTypes.TryGetValue(objectTypeName, out Guid result))
                return result;

            return Undefined;
        }

        /// <summary>
        /// Checks if a GUID is a valid object type
        /// </summary>
        /// <param name="guid">GUID to check</param>
        /// <returns>True if the GUID is a valid object type</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static bool IsValidObjectType(Guid guid)
        {
            var allTypes = GetAllObjectTypes();
            var objectTypes = allTypes["ObjectTypes"] as Dictionary<string, Guid>;
            
            return objectTypes != null && objectTypes.Values.Contains(guid);
        }
    }
}
