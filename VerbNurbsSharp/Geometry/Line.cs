using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Geometry
{
    /// <summary>
    /// A curve representing a straight line
    /// </summary>
    public class Line : Serializable<Line>
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        public Line() { }

        /// <summary>
        /// Line by start point and  end point
        /// </summary>
        /// <param name="start">Start Point</param>
        /// <param name="end">End Point</param>
        public Line(Point start, Point end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Length of the line
        /// </summary>
        /// <returns></returns>
        public double Length => Constants.DistanceTo(this.Start, this.End);
    }
}
