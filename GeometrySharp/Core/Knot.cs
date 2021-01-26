using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.ExtendedMethods;

namespace GeometrySharp.Core
{
    /// <summary>
    /// A Knot is a non-decreasing sequence of doubles. Use the methods in <see cref="GeometrySharp.Evaluation.Check"/>/> to validate Knot's.
    /// </summary>
    public class Knot : List<double>
    {
        public Knot(){}

        public Knot(int degree, int numberOfControlPts, bool clamped = true)
        {
            Create(degree, numberOfControlPts, clamped);
        }

        public Knot(IEnumerable<double> values)
        {
            this.AddRange(values);
        }

        /// <summary>
        /// Check the validity of the input knots.
        /// Confirm the relations between degree (p), number of control points(n+1), and the number of knots (m+1).
        /// Refer to The NURBS Book (2nd Edition), p.50 for details.
        /// 
        /// More specifically, this method checks if the knot knots is of the following structure:
        /// The knot knots must be non-decreasing and of length (degree + 1) * 2 or greater
        /// [ (degree + 1 copies of the first knot), internal non-decreasing knots, (degree + 1 copies of the last knot) ]
        /// </summary>
        /// <param name="degree">The degree of the curve.</param>
        /// <param name="numControlPts">The number of control points.</param>
        /// <returns>Whether the array is a valid knot knots or knot</returns>
        public bool AreValidKnots(int degree, int numControlPts)
        {
            if (this.Count == 0) return false;
            if (this.Count < (degree + 1) * 2) return false;
            // Check the formula: m = p + n + 1
            if (numControlPts + degree + 1 - this.Count != 0) return false;

            var rep = this.First();
            for (int i = 0; i < this.Count; i++)
            {
                // Verb doesn't allow for periodic knots, these two ifs should be removed.
                if (i < degree + 1)
                    if (Math.Abs(this[i] - rep) > GeoSharpMath.EPSILON) return false;

                if (i > this.Count - degree - 1 && i < this.Count)
                    if (Math.Abs(this[i] - rep) > GeoSharpMath.EPSILON) return false;

                if (this[i] < rep - GeoSharpMath.EPSILON) return false;
                rep = this[i];
            }
            return true;
        }

        /// <summary>
        /// Find the span on the knot Array without supplying a number of control points.
        /// </summary>
        /// <param name="degree">Integer degree of function.</param>
        /// <param name="u">Parameter.</param>
        /// <param name="knots">Array of non decreasing knot values.</param>
        /// <returns></returns>
        public int Span(int degree, double u) => Span(this.Count - degree - 2, degree, u);

        /// <summary>
        /// Find the span on the knot list of the given parameter,
        /// (corresponds to algorithm 2.1 from the NURBS book, piegl & Tiller 2nd edition).
        /// </summary>
        /// <param name="n">Integer number of basis functions - 1 = knots.length - degree - 2.</param>
        /// <param name="degree">Integer degree of function.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="knots">Array of non decreasing knot values.</param>
        /// <returns>The index of the knot span.</returns>
        public int Span(int n, int degree, double parameter)
        {
            // special case if parameter == knots[n+1]
            if (parameter > this[n + 1] - GeoSharpMath.EPSILON) return n;

            if (parameter < this[degree] + GeoSharpMath.EPSILON) return degree;

            var low = degree;
            var high = n + 1;
            int mid = (int) Math.Floor((double)(low + high) / 2);

            while (parameter < this[mid] || parameter >= this[mid + 1])
            {
                if (parameter < this[mid])
                    high = mid;
                else
                    low = mid;

                mid = (int) Math.Floor((double)(low + high) / 2);
            }

            return mid;
        }

        /// <summary>
        /// Normalizes the input knot vector to [0, 1] domain.
        /// Overwrite the knots.
        /// </summary>
        public void Normalize()
        {
            var denominator = this[^1] - this[0];
            var normalizedKnots = new List<double>();
            foreach (var knot in this)
                normalizedKnots.Add((knot - this[0]) / denominator);
            this.Clear();
            this.AddRange(normalizedKnots);
        }

        /// <summary>
        /// Generates an equally spaced knot vector.
        /// Clamp curve is tangent to the first and the last legs at the first and last control points.
        /// </summary>
        /// <param name="degree">Degree.</param>
        /// <param name="numberOfControlPts">Number of control points.</param>
        /// <param name="clamped">Flag to choose from clamped or unclamped knot vector options, default true.</param>
        public void Create(int degree, int numberOfControlPts, bool clamped = true)
        {
            if (degree <= 0 || numberOfControlPts <= 0)
                throw new Exception("Input values must be positive and different than zero.");

            // Number of repetitions at the start and end of the array.
            var numOfRepeat = degree;
            // Number of knots in the middle.
            var numOfSegments = numberOfControlPts - (degree + 1);

            if (!clamped)
            {
                // No repetitions at the start and end.
                numOfRepeat = 0;
                // Should conform the rule m = n+p+1
                numOfSegments = degree + numberOfControlPts - 1;
            }

            var knotVector = new List<double>();
            knotVector.AddRange(Sets.RepeatData(0.0, numOfRepeat));
            knotVector.AddRange(Sets.LinearSpace(new Interval(0.0,1.0), numOfSegments+2));
            knotVector.AddRange(Sets.RepeatData(1.0, numOfRepeat));

            this.AddRange(knotVector);
        }

        public override string ToString()
        {
            return $"{{{string.Join(",", this)}}}";
        }
    }
}
