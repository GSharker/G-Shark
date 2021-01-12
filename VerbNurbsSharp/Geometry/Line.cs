using Newtonsoft.Json;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Geometry
{
    /// <summary>
    /// A curve representing a straight line
    /// </summary>
    public class Line : Serializable<Line>
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        public Line() { }

        /// <summary>
        /// Line by start point and  end point
        /// </summary>
        /// <param name="start">Start Vector3</param>
        /// <param name="end">End Vector3</param>
        public Line(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Length of the line.
        /// </summary>
        /// <returns></returns>
        public double Length => this.Start.DistanceTo(this.End);

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
