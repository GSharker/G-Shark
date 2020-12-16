using GeometryLib.Core;

namespace GeometryLib.Geometry
{
    /// <summary>
    /// A curve representing a straight line
    /// </summary>
    public class Line : Serializable<Line>
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Line(Point start, Point end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
