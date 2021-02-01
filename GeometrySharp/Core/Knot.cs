using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.ExtendedMethods;

namespace GeometrySharp.Core
{
    /// <summary>
    /// A Knot is a non-decreasing sequence of doubles. Use the methods in <see cref="GeometrySharp.Evaluation.Check"/>/> to validate Knot's.
    /// </summary>
    public class Knot : List<double>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Knot()
        {

        }

        /// <summary>
        /// Create an instance of the class knot.
        /// </summary>
        /// <param name="degree">Degree of the curve.</param>
        /// <param name="numberOfControlPts">Number of control points of the curve.</param>
        /// <param name="clamped">If the curve is clamped or not.</param>
        public Knot(int degree, int numberOfControlPts, bool clamped = true)
        {
            Create(degree, numberOfControlPts, clamped);
        }

        /// <summary>
        /// Create an instance of the class from a list of values.
        /// </summary>
        /// <param name="values">Set of knots.</param>
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
        public int Span(int degree, double u)
        {
            return Span(this.Count - degree - 2, degree, u);
        }

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
        /// Calculate the multiplicity of a knot.
        /// </summary>
        /// <param name="knotIndex">The index of the knot to determine multiplicity.</param>
        /// <returns>The multiplicity of the knot.</returns>
        public int MultiplicityByIndex(int knotIndex)
        {
            if (knotIndex < 0 || knotIndex > this.Count)
                throw new Exception("Input values must be in the dimension of the knot set.");

            var index = knotIndex;
            var knot = this[knotIndex];
            var multiplicity = 1;
            while (index < this.Count-1)
            {
                if (Math.Abs(this[index + 1] - knot) > GeoSharpMath.EPSILON)
                    break;
                index += 1;
                multiplicity += 1;
            }

            return multiplicity;
        }

        /// <summary>
        /// Determine the multiplicity values of the knot.
        /// </summary>
        /// <returns>Dictionary where the key is the knot and the value the multiplicity</returns>
        public Dictionary<double, int> Multiplicities()
        {
            var multiplicities = new Dictionary<double, int> {{this[0], 0}};
            var tempKnot = this[0];

            foreach (var knot in this)
            {
                if (Math.Abs(knot - tempKnot) > GeoSharpMath.EPSILON)
                {
                    multiplicities.Add(knot, 0);
                    tempKnot = knot;
                }

                multiplicities[tempKnot] += 1;
            }

            return multiplicities;
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

        /// <summary>
        /// Normalizes the input knot vector to [0, 1] domain.
        /// </summary>
        /// <param name="knots">Knot vector to be normalized,</param>
        /// <returns>Normalized knot vector.</returns>
        public static Knot Normalize(Knot knots)
        {
            if (knots.Count == 0)
                throw new Exception("Input knot vector cannot be empty");

            var firstKnot = knots[0];
            var lastKnot = knots[^1];
            var denominator = lastKnot - firstKnot;

            return knots.Select(kv => (kv - firstKnot) / denominator).ToKnot();
        }

        /// <summary>
        /// Override ToString method.
        /// </summary>
        /// <returns>A knot in a string version.</returns>
        public override string ToString()
        {
            return $"{{{string.Join(",", this)}}}";
        }
    }
}
