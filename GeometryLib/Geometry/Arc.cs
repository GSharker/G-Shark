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

        public Point Center { get; set; }
        public Vector XAxis { get; set; }
        public Vector YAxis { get; set; }
        public double Radius { get; set; }
        public double MinAngle { get; set; }
        public double MaxAngle { get; set; }
    }
}
