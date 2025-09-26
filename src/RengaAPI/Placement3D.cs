using System;

namespace Renga
{
    /// <summary>
    /// Represents a 3D placement in the Renga API.
    /// This struct mirrors the Renga.Placement3D struct from the COM interface.
    /// </summary>
    /// <remarks>
    /// The Placement3D struct defines a 3D coordinate system with an origin point and orientation axes.
    /// It is used to position and orient objects in 3D space.
    /// </remarks>
    public struct Placement3D
    {
        /// <summary>
        /// The origin point of the placement.
        /// </summary>
        public Point3D Origin;

        /// <summary>
        /// The X axis vector of the placement.
        /// </summary>
        public Vector3D xAxis;

        /// <summary>
        /// The Z axis vector of the placement.
        /// </summary>
        public Vector3D zAxis;

        /// <summary>
        /// Initializes a new instance of the Placement3D struct.
        /// </summary>
        /// <param name="origin">The origin point.</param>
        /// <param name="xAxis">The X axis vector.</param>
        /// <param name="zAxis">The Z axis vector.</param>
        public Placement3D(Point3D origin, Vector3D xAxis, Vector3D zAxis)
        {
            Origin = origin;
            this.xAxis = xAxis;
            this.zAxis = zAxis;
        }

        /// <summary>
        /// Returns a string representation of the Placement3D.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"Placement3D(Origin: ({Origin.X}, {Origin.Y}, {Origin.Z}), X: ({xAxis.X}, {xAxis.Y}, {xAxis.Z}), Z: ({zAxis.X}, {zAxis.Y}, {zAxis.Z}))";
        }
    }

    /// <summary>
    /// Represents a 2D placement in the Renga API.
    /// This struct mirrors the Renga.Placement2D struct from the COM interface.
    /// </summary>
    /// <remarks>
    /// The Placement2D struct defines a 2D coordinate system with an origin point and orientation.
    /// It is used to position and orient objects in 2D space.
    /// </remarks>
    public struct Placement2D
    {
        /// <summary>
        /// The origin point of the placement.
        /// </summary>
        public Point2D Origin;

        /// <summary>
        /// The X axis vector of the placement.
        /// </summary>
        public Vector2D xAxis;

        /// <summary>
        /// Initializes a new instance of the Placement2D struct.
        /// </summary>
        /// <param name="origin">The origin point.</param>
        /// <param name="xAxis">The X axis vector.</param>
        public Placement2D(Point2D origin, Vector2D xAxis)
        {
            Origin = origin;
            this.xAxis = xAxis;
        }

        /// <summary>
        /// Returns a string representation of the Placement2D.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            return $"Placement2D(Origin: ({Origin.X}, {Origin.Y}), X: ({xAxis.X}, {xAxis.Y}))";
        }
    }
}
