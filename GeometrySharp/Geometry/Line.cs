using System;
using Newtonsoft.Json;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo this class has to be tested.
    // ToDo this class has to be implemented.
    // ToDo is the serializable be tested?

    /// <summary>
    /// A curve representing a straight line.
    /// </summary>
    public class Line
    {
        /// <summary>
        /// Line by start point and end point.
        /// </summary>
        /// <param name="start">Start point.</param>
        /// <param name="end">End point.</param>
        public Line(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
            this.Length = Start.DistanceTo(End);
            this.Direction = (End - Start).Unitize();
        }

        /// <summary>
        /// Line from a starting point, direction and length.
        /// </summary>
        /// <param name="start">Starting point of the line.</param>
        /// <param name="direction">Direction of the line.</param>
        /// <param name="length">Length of the line.</param>
        public Line(Vector3 start, Vector3 direction, double length)
        {
            this.Start = start;
            this.End = start + direction.Amplify(length);
            this.Length = length;
            this.Direction = direction.Unitize();
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
        public double Length { get; }

        /// <summary>
        /// Direction of the line.
        /// </summary>
        public Vector3 Direction { get; }

        /// <summary>
        /// Get the closest point on the line from this point.
        /// </summary>
        /// <param name="line">The line on which to find the closest point.</param>
        /// <returns>The closest point on the line from this point.</returns>
        public Vector3 ClosestPoint(Vector3 vec)
        {
            var dir = this.Direction;
            var v = vec - this.Start;
            var d = Vector3.Dot(v, dir);

            d = Math.Min(this.Length, d);
            d = Math.Max(d, 0);

            return this.Start + dir * d;
        }
    }
}
