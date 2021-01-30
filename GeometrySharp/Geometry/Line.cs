using Newtonsoft.Json;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo this class has to be tested.
    /// <summary>
    /// A curve representing a straight line.
    /// </summary>
    public class Line : Serializable<Line>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Line() { }

        /// <summary>
        /// Line by start point and end point.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        public Line(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Start point of the line.
        /// </summary>
        public Vector3 Start { get; set; }

        /// <summary>
        /// End point of the line.
        /// </summary>
        public Vector3 End { get; set; }

        /// <summary>
        /// Length of the line.
        /// </summary>
        public double Length => this.Start.DistanceTo(this.End);

        /// <summary>
        /// Direction of the line.
        /// </summary>
        public Vector3 Direction => (this.End - this.Start).Normalized();

        public override Line FromJson(string s)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Serialize a line to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
