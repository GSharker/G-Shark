using GeometryLib.Core;

namespace GeometryLib.Geometry
{
    public class Arc
    {
        public Arc(Point center, Vector xAxis, Vector yAxis, double radius, double minAngle, double maxAngle)
        {
            Center = center;
            XAxis = xAxis;
            YAxis = yAxis;
            Radius = radius;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }

        public Point Center { get; }
        public Vector XAxis { get; }
        public Vector YAxis { get; }
        public double Radius { get; }
        public double MinAngle { get; }
        public double MaxAngle { get; }

        public Point GetCenter() => Center;
    }
}
