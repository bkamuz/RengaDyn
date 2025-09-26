using System;

namespace Renga
{
    // Minimal struct to represent Renga.Placement2D for compile-time typing.
    public struct Placement2D
    {
        public Point2D origin;
        public Vector2D xAxis;
    }

    public struct Point2D { public double X; public double Y; }
    public struct Vector2D { public double X; public double Y; }
}
