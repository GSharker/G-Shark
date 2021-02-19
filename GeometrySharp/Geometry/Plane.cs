

// ToDo this class need to be tested.
// ToDo this class need to be commented.
// Note this class can be developed bit more looking Hypar or RhinoCommon.
namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A Plane is simply an origin point and normal.
    /// </summary>
    public class Plane
    {
        /// <summary>
        /// Construct a plane from a origin and a direction.
        /// </summary>
        /// <param name="origin">The point describing the origin of the plane.</param>
        /// <param name="direction">The vector representing the normal of the plane.</param>
        public Plane(Vector3 origin, Vector3 direction)
        {
            this.ZAxis = direction.Unitize();
            this.XAxis = ZAxis.PerpendicularTo(Vector3.XAxis).Unitize();
            this.YAxis = Vector3.Cross(ZAxis, XAxis).Unitize();
            this.Origin = origin;
        }

        /// <summary>
        /// Construct a plane from three points.
        /// </summary>
        /// <param name="pt1">Firs point representing the origin.</param>
        /// <param name="pt2">Second point representing the x direction.</param>
        /// <param name="pt3">Third point representing the y direction.</param>
        public Plane(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            this.Origin = pt1;
            var xDir = (pt2 - pt1).Unitize();
            var yDir = (pt3 - pt1).Unitize();
            this.XAxis = xDir;
            this.YAxis = yDir;
            this.ZAxis = Vector3.Cross(xDir, yDir).Unitize();
        }

        /// <summary>
        /// Get a XY plane.
        /// </summary>
        public static Plane PlaneXY => new Plane(new Vector3{0.0,0.0,0.0}, Vector3.ZAxis);

        /// <summary>
        /// Get a YZ plane.
        /// </summary>
        public static Plane PlaneYZ => new Plane(new Vector3 { 0.0, 0.0, 0.0 }, Vector3.XAxis);

        /// <summary>
        /// Get a XY plane.
        /// </summary>
        public static Plane PlaneXZ => new Plane(new Vector3 { 0.0, 0.0, 0.0 }, Vector3.YAxis);

        /// <summary>
        /// The normal of the plan.
        /// </summary>
        public Vector3 Normal => ZAxis;

        /// <summary>
        /// The origin of the plane.
        /// </summary>
        public Vector3 Origin { get; }

        /// <summary>
        /// The XAxis of the plane.
        /// </summary>
        public Vector3 XAxis { get; }

        /// <summary>
        /// The YAxis of the plane.
        /// </summary>
        public Vector3 YAxis { get; }

        /// <summary>
        /// The ZAxis of the plane.
        /// </summary>
        public Vector3 ZAxis { get; }
    }
}
