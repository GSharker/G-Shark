using System;
using System.Collections.Generic;

namespace GShark.Core
{
    /// <summary>
    /// A simple parametric data representing an "interval" between two numbers.
    /// </summary>
    public class Interval
    {
        /// <summary>
        /// Creates an instance of an interval given a minimum and a maximum value.
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public Interval(double t0, double t1)
        {
            T0 = t0;
            T1 = t1;
        }

        /// <summary>
        /// Gets the minimum value of the interval.
        /// </summary>
        public double T0 { get; }

        /// <summary>
        /// Gets the maximum value of the interval.
        /// </summary>
        public double T1 { get; }

        /// <summary>
        /// Gets the value between the interval's min and max values.
        /// </summary>
        public double Mid => Math.Abs(T0 - T1) > GSharkMath.MinTolerance ? 0.5 * (T0 + T1) : T0;

        /// <summary>
        /// Gets the length of the interval.<br/>
        /// If the interval is decreasing, negative number will be returned.
        /// </summary>
        public double Length => Math.Abs(T1 - T0);

        /// <summary>
        /// True if t0 is less than t1.
        /// </summary>
        public bool IsDecreasing => T0 - T1 > 0;

        /// <summary>
        /// True if t1 is greater than t0.
        /// </summary>
        public bool IsIncreasing => T1 - T0 > 0;

        /// <summary>
        /// True if t0 == t1.
        /// </summary>
        public bool IsSingleton => Math.Abs(T1 - T0) <= GSharkMath.Epsilon;

        /// <summary>
        /// Returns the largest value in the interval.
        /// </summary>
        public double Max => Math.Max(T1, T0);

        /// <summary>
        /// Returns the smallest value in the interval.
        /// </summary>
        public double Min => Math.Min(T1, T0);

        /// <summary>
        /// Converts normalized parameter to interval value, or pair of values.
        /// </summary>
        /// <param name="normalizedParameter">The normalized parameter between 0 and 1.</param>
        /// <returns>Interval parameter t0*(1.0-normalizedParameter) + t1*normalizedParameter.</returns>
        public double ParameterAt(double normalizedParameter)
        {
            return !GSharkMath.IsValidDouble(normalizedParameter)
                ? GSharkMath.UnsetValue
                : (1.0 - normalizedParameter) * T0 + normalizedParameter * T1;
        }

        /// <summary>
        /// Computes a collection of evenly spaced numbers over a specified domain.
        /// </summary>
        /// <param name="domain">Numeric domain.</param>
        /// <param name="step">Number of steps.</param>
        /// <returns>A collection of equally spaced numbers.</returns>
        public static IList<double> Divide(Interval domain, int step)
        {
            if (Math.Abs(domain.T0 - domain.T1) <= GSharkMath.Epsilon)
            {
                return new List<double>() { domain.T0 };
            }

            List<double> linearSpace = new List<double>();

            if (step <= 1)
            {
                linearSpace.Add(domain.T0);
                return linearSpace;
            }

            int div = step - 1;
            double delta = domain.T1 - domain.T0;
            for (int i = 0; i < step; i++)
            {
                linearSpace.Add(domain.T0 + (i * delta / div));
            }

            return linearSpace;
        }
    }
}
