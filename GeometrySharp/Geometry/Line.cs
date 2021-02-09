using System;

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
        /// Gets the BoundingBox in ascending fashion.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                var bBox = new BoundingBox(Start, End);
                var validBBox = bBox.MakeItValid();
                return validBBox;
            }
        }

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

        /// <summary>
        /// Evaluate the line at the specified parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate the line. Parameter should be between 0.0 and 1.0</param>
        /// <returns>The point at the specific parameter.</returns>
        public Vector3 PointAt(double t)
        {
            if (t > 1.0 || t < 0.0)
                throw new ArgumentOutOfRangeException(nameof(t), "Parameter is outside the domain 0.0 to 1.0");

            return this.Start + this.Direction * (this.Length * t);
        }


        // PointAtLength
        // Flip
        // Extend
        // ToString
        // ToNurbsCurve
        // Transform
        // Equality
    }
}
