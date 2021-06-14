using GShark.ExtendedMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Core
{
    /// <summary>
    /// The Knot Vector is a sequence of parameter values that determines where and how the control points affect the NURBS curve.
    /// The number of knots is always equal to the number of control points plus curve degree plus one (i.e. number of control points plus curve order).
    /// </summary>
    public class KnotVector : List<double>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public KnotVector()
        {
        }

        /// <summary>
        /// Creates an instance of the knot vector given degree, number of control points and whether it is clamped or unclamped.
        /// </summary>
        /// <param name="degree">Degree of the curve.</param>
        /// <param name="numberOfControlPts">Number of control points of the curve.</param>
        /// <param name="clamped">If the curve is clamped or not.</param>
        public KnotVector(int degree, int numberOfControlPts, bool clamped = true)
        {
            Create(degree, numberOfControlPts, clamped);
        }

        /// <summary>
        /// Creates an instance of the knot vector from a list of double values.
        /// </summary>
        /// <param name="values">Set of knots.</param>
        public KnotVector(IEnumerable<double> values)
        {
            AddRange(values);
        }

        /// <summary>
        /// Checks the validity of the input knots.<br/>
        /// Confirm the relations between degree (p), number of control points(n+1), and the number of knots (m+1).<br/>
        /// Refer to The NURBS Book (2nd Edition), p.50 for details.<br/>
        /// <br/>
        /// More specifically, this method checks if the knot knots is of the following structure:<br/>
        /// The knot knots must be non-decreasing and of length (degree + 1) * 2 or greater<br/>
        /// [ (degree + 1 copies of the first knot), internal non-decreasing knots, (degree + 1 copies of the last knot) ]
        /// </summary>
        /// <param name="degree">The degree of the curve.</param>
        /// <param name="numberOfControlPts"></param>
        /// <returns>Whether the knots are valid.</returns>
        public bool IsValid(int degree, int numberOfControlPts)
        {
            if (Count == 0)
            {
                return false;
            }

            if (Count < (degree + 1) * 2)
            {
                return false;
            }
            // Check the formula: m = p + n + 1
            if (numberOfControlPts + degree + 1 - Count != 0)
            {
                return false;
            }

            double rep = this.First();
            for (int i = 0; i < Count; i++)
            {
                // Verb doesn't allow for periodic knots, these two ifs should be removed.
                if (i < degree + 1)
                {
                    if (Math.Abs(this[i] - rep) > GeoSharpMath.Epsilon)
                    {
                        return false;
                    }
                }

                if (i > Count - degree - 1 && i < Count)
                {
                    if (Math.Abs(this[i] - rep) > GeoSharpMath.Epsilon)
                    {
                        return false;
                    }
                }

                if (this[i] < rep - GeoSharpMath.Epsilon)
                {
                    return false;
                }

                rep = this[i];
            }
            return true;
        }

        /// <summary>
        /// Gets the domain of the knots, as the max value - min value.
        /// </summary>
        public double Domain => this[^1] - this[0];

        /// <summary>
        /// Finds the span of the knot vector from curve degree and a parameter u on the curve.
        /// </summary>
        /// <param name="degree">Curve degree.</param>
        /// <param name="u">Parameter on curve.</param>
        /// <returns>The index of the knot span.</returns>
        public int Span(int degree, double u)
        {
            return Span(Count - degree - 2, degree, u);
        }

        /// <summary>
        /// Finds the span on the knot vector of the given parameter.<br/>
        /// <em>Corresponds to algorithm 2.1 from the NURBS book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="n">Number of basis functions.</param>
        /// <param name="degree">Degree of function.</param>
        /// <param name="parameter">Parameter.</param>
        /// <returns>The index of the knot span.</returns>
        public int Span(int n, int degree, double parameter)
        {
            // special case if parameter == knots[n+1]
            if (parameter > this[n + 1] - GeoSharpMath.Epsilon)
            {
                return n;
            }

            if (parameter < this[degree] + GeoSharpMath.Epsilon)
            {
                return degree;
            }

            int low = degree;
            int high = n + 1;
            int mid = (int) Math.Floor((double)(low + high) / 2);

            while (parameter < this[mid] || parameter >= this[mid + 1])
            {
                if (parameter < this[mid])
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }

                mid = (int) Math.Floor((double)(low + high) / 2);
            }

            return mid;
        }

        /// <summary>
        /// Calculates the multiplicity of a knot.
        /// </summary>
        /// <param name="knotIndex">The index of the knot to determine multiplicity.</param>
        /// <returns>The multiplicity of the knot.</returns>
        public int MultiplicityByIndex(int knotIndex)
        {
            if (knotIndex < 0 || knotIndex > Count)
            {
                throw new Exception("Input values must be in the dimension of the knot set.");
            }

            int index = knotIndex;
            double knot = this[knotIndex];
            int multiplicity = 1;
            while (index < Count-1)
            {
                if (Math.Abs(this[index + 1] - knot) > GeoSharpMath.Epsilon)
                {
                    break;
                }

                index += 1;
                multiplicity += 1;
            }

            return multiplicity;
        }

        /// <summary>
        /// Determines the multiplicity values of the knots.
        /// </summary>
        /// <returns>Dictionary where the key is the knot and the value the multiplicity.</returns>
        public Dictionary<double, int> Multiplicities()
        {
            Dictionary<double, int> multiplicities = new Dictionary<double, int> {{this[0], 0}};
            double tempKnot = this[0];

            foreach (double knot in this)
            {
                if (Math.Abs(knot - tempKnot) > GeoSharpMath.Epsilon)
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
        /// <param name="clamped">Flag to choose from clamped or un-clamped knot vector options, default true.</param>
        public void Create(int degree, int numberOfControlPts, bool clamped = true)
        {
            if (degree <= 0 || numberOfControlPts <= 0)
            {
                throw new Exception("Input values must be positive and different than zero.");
            }

            // Number of repetitions at the start and end of the array.
            int numOfRepeat = degree;
            // Number of knots in the middle.
            int numOfSegments = numberOfControlPts - (degree + 1);

            if (!clamped)
            {
                // No repetitions at the start and end.
                numOfRepeat = 0;
                // Should conform the rule m = n+p+1
                numOfSegments = degree + numberOfControlPts - 1;
            }

            List<double> knotVector = new List<double>();
            knotVector.AddRange(Sets.RepeatData(0.0, numOfRepeat));
            knotVector.AddRange(Sets.LinearSpace(new Interval(0.0,1.0), numOfSegments+2));
            knotVector.AddRange(Sets.RepeatData(1.0, numOfRepeat));

            AddRange(knotVector);
        }

        /// <summary>
        /// Normalizes the input knot vector to a range from 0 to 1.
        /// </summary>
        /// <param name="knots">Knots vector to be normalized.</param>
        /// <returns>Normalized knots vector.</returns>
        public static KnotVector Normalize(KnotVector knots)
        {
            if (knots.Count == 0)
            {
                throw new Exception("Input knot vector cannot be empty");
            }

            double firstKnot = knots[0];
            double lastKnot = knots[^1];
            double denominator = lastKnot - firstKnot;

            return knots.Select(kv => (kv - firstKnot) / denominator).ToKnot();
        }

        /// <summary>
        /// Reverses the order of the knots in the knot vector.
        /// </summary>
        /// <param name="knots">Knot vectors to be reversed.</param>
        /// <returns>Reversed knot vectors.</returns>
        public static KnotVector Reverse(KnotVector knots)
        {
            double firstKnot = knots[0];

            KnotVector reversedKnots = new KnotVector {firstKnot};
            for (int i = 1; i < knots.Count; i++)
            {
                reversedKnots.Add(reversedKnots[i-1] + (knots[^i] - knots[knots.Count - i - 1]));
            }

            return reversedKnots;
        }

        /// <summary>
        /// Creates a text representation of the knot vector.
        /// </summary>
        /// <returns>Knots in a string version.</returns>
        public override string ToString()
        {
            return $"{{{string.Join(",", this)}}}";
        }
    }
}
