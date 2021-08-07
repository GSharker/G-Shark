using GShark.ExtendedMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Core
{
    /// <summary>
    /// The Knot Vector is a sequence of parameter values that determines where and how the control points affect the NURBS curve.<br/>
    /// The number of knot vector is always equal to the number of control points plus curve degree plus one (i.e. number of control points plus curve order).
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
            if (degree < 1 || numberOfControlPts < 1)
            {
                throw new Exception("Input values must be positive and different than zero.");
            }

            if (numberOfControlPts < degree)
            {
                throw new Exception("Degree must be less than the number of control point.");
            }

            UniformNonPeriodic(degree, numberOfControlPts, clamped);
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
        /// Checks the validity of the input knot vector.<br/>
        /// Confirm the relations between degree (p), number of control points(n+1), and the number of knots (m+1).<br/>
        /// Refer to The NURBS Book (2nd Edition), p.50 for details.<br/>
        /// <br/>
        /// More specifically, this method checks if the knot vector is of the following structure:<br/>
        /// The knot knots must be non-decreasing and of length (degree + 1) * 2 or greater<br/>
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

            for (int i = 0; i < Count; i++)
            {
                if (!GeoSharkMath.IsValidDouble(this[i]))
                {
                    return false;
                }
            }

            bool hasMultiplicity = Multiplicity(0) > 1 || Multiplicity(this.Count - 1) > 1;
            double rep = this[0];
            for (int i = 0; i < Count; i++)
            {
                if (hasMultiplicity)
                {
                    if (i < degree + 1)
                    {
                        if (Math.Abs(this[i] - rep) > GeoSharkMath.Epsilon)
                        {
                            return false;
                        }
                    }

                    if (i > Count - degree - 1 && i < Count)
                    {
                        if (Math.Abs(this[i] - rep) > GeoSharkMath.Epsilon)
                        {
                            return false;
                        }
                    }
                }

                if (this[i] < rep - GeoSharkMath.Epsilon)
                {
                    return false;
                }

                rep = this[i];
            }
            return true;
        }

        /// <summary>
        /// Checks if the knot is clamped, so if they are in the following structure.<br/>
        /// [ (degree + 1 copies of the first knot), internal non-decreasing knots, (degree + 1 copies of the last knot) ].
        /// </summary>
        /// <param name="degree">Curve degree.</param>
        /// <returns>If true the knot is clamped, if false is unclamped.</returns>
        public bool IsClamped(int degree)
        {
            if (Math.Abs(this[0] - this[degree]) > GeoSharkMath.Epsilon)
            {
                return false;
            }

            return !(Math.Abs(this[Count - 1] - this[Count - degree - 1]) > GeoSharkMath.Epsilon);
        }

        /// <summary>
        /// Checks if the knot is periodic, so if they are in the following structure.<br/>
        /// [ (degree, negative values), internal non-decreasing knots between 0 and 1, (degree, positive values bigger than 1.0) ].
        /// </summary>
        /// <param name="degree"></param>
        /// <returns>If true the knot is periodic, if false is clamped or unclamped.</returns>
        public bool IsKnotVectorPeriodic(int degree)
        {
            if (this[0] > this[degree])
            {
                return false;
            }

            if (this[degree] > 0)
            {
                return false;
            }

            return this[Count - degree - 1] < this[Count - 1] && !(this[Count - degree - 1] < 1);
        }

        /// <summary>
        /// Gets the domain of the knots, as the max value - min value.
        /// </summary>
        public double Domain => this[Count - 1] - this[0];

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
            if (parameter > this[n + 1] - GeoSharkMath.Epsilon)
            {
                return n;
            }

            if (parameter < this[degree] + GeoSharkMath.Epsilon)
            {
                return degree;
            }

            int low = degree;
            int high = n + 1;
            int mid = (int)Math.Floor((double)(low + high) / 2);

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

                mid = (int)Math.Floor((double)(low + high) / 2);
            }

            return mid;
        }

        /// <summary>
        /// Calculates the multiplicity of a knot.
        /// </summary>
        /// <param name="knot">The index of the knot to determine multiplicity.</param>
        /// <returns>The multiplicity of the knot, or 0 if the knot is not part of the knot vector.</returns>
        public int Multiplicity(double knot)
        {
            return this.Count(x => Math.Abs(x-knot) <= GeoSharkMath.MaxTolerance);
        }

        /// <summary>
        /// Returns the multiplicity values of the all knots in this knot vector.
        /// </summary>
        /// <returns>Dictionary of [knot, multiplicity].</returns>
        public Dictionary<double, int> Multiplicities()
        {
            Dictionary<double, int> multiplicities = new Dictionary<double, int>(Count);
            foreach (double knot in this)
            {
                var multiplicity =  Multiplicity(knot);
                if (!multiplicities.Keys.Contains(knot))
                {
                    multiplicities.Add(knot, multiplicity);
                }

                multiplicities[knot] = multiplicity;
            }
            return multiplicities;
        }

        /// <summary>
        /// Generates an equally spaced knot vector.<br/>
        /// Clamp curve is tangent to the first and the last legs at the first and last control points.
        /// </summary>
        /// <param name="degree">Degree.</param>
        /// <param name="numberOfControlPts">Number of control points.</param>
        /// <param name="clamped">Option to choose from clamped or un-clamped knot vector options, default true.</param>
        private void UniformNonPeriodic(int degree, int numberOfControlPts, bool clamped = true)
        {
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

            this.AddRange(Sets.RepeatData(0.0, numOfRepeat));
            this.AddRange(Sets.LinearSpace(new Interval(0.0, 1.0), numOfSegments + 2));
            this.AddRange(Sets.RepeatData(1.0, numOfRepeat));
        }

        /// <summary>
        /// Generates a periodic uniform spaced knot vectors.
        /// </summary>
        /// <param name="degree">Degree.</param>
        /// <param name="numberOfControlPts">Number of control points.</param>
        public static KnotVector UniformPeriodic(int degree, int numberOfControlPts)
        {
            if (numberOfControlPts < 2)
            {
                throw new Exception("Number of control points must be bigger than two.");
            }

            if (degree < 2)
            {
                throw new Exception("Degree must be equal or bigger than two.");
            }

            if (numberOfControlPts < degree)
            {
                throw new Exception("Degree must be less than the number of points.");
            }

            double[] knot = new double[degree + numberOfControlPts + 1];

            double k;
            int i, knotCount = numberOfControlPts + degree + 1;

            double delta = 1.0 / (numberOfControlPts - degree);

            for (i = degree, k = 0.0; i < knotCount; i++, k += delta)
            {
                knot[i] = k;
            }

            for (i = degree - 1, k = -delta; i >= 0; i--, k -= delta)
            {
                knot[i] = k;
            }

            return knot.ToKnot();
        }

        /// <summary>
        /// Normalizes the input knot vector to a range from 0 to 1.
        /// </summary>
        /// <param name="knots">Knots vector to be normalized.</param>
        /// <returns>Normalized knots vector.</returns>
        public KnotVector Normalize()
        {
            if (this.Count == 0)
            {
                throw new Exception("Input knot vector cannot be empty");
            }

            double firstKnot = this[0];
            double lastKnot = this[this.Count - 1];
            double denominator = lastKnot - firstKnot;

            return this.Select(kv => (kv - firstKnot) / denominator).ToKnot();
        }

        /// <summary>
        /// Reverses the order of the knot vector.
        /// </summary>
        /// <param name="knots">Knot vectors to be reversed.</param>
        /// <returns>Reversed knot vectors.</returns>
        public static KnotVector Reverse(KnotVector knots)
        {
            KnotVector reversedKnots = new KnotVector { knots[0] };
            for (int i = 1; i < knots.Count; i++)
            {
                reversedKnots.Add(reversedKnots[i - 1] + (knots[knots.Count - i] - knots[knots.Count - i - 1]));
            }

            return reversedKnots;
        }

        /// <summary>
        /// Creates a copy of the knotVector.
        /// </summary>
        /// <returns>The copy of the knotVector.</returns>
        public KnotVector Copy()
        {
            List<double> knotVectorCopy = new List<double>(this);
            return knotVectorCopy.ToKnot();
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
