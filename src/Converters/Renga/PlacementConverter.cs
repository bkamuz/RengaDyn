using System;
using dr = Autodesk.DesignScript.Runtime;
using dg = Autodesk.DesignScript.Geometry;

namespace DynRenga.Converters.Renga
{
    public static class PlacementConverter
    {
        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem Placement3DToCoordinateSystem(DynRenga.DynGeometry.Placement3D placement)
        {
            if (placement == null) throw new ArgumentNullException(nameof(placement));
            return placement.ToDynamoCoordinateSystem();
        }

        [dr.IsVisibleInDynamoLibrary(true)]
        public static dg.CoordinateSystem Placement2DToCoordinateSystem(DynRenga.DynGeometry.Placement2D placement)
        {
            if (placement == null) throw new ArgumentNullException(nameof(placement));
            return placement.ToDynamoCoordinateSystem();
        }
    }
}
