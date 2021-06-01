using System;
using System.Data.SqlTypes;

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
        public double Mid => Math.Abs(T0 - T1) > GeoSharpMath.MAX_TOLERANCE ? 0.5 * (T0 + T1) : T0;

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
        public bool IsSingleton => Math.Abs(T1 - T0) >= GeoSharpMath.EPSILON;

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
            return !GeoSharpMath.IsValidDouble(normalizedParameter)
                ? GeoSharpMath.UNSET_VALUE
                : (1.0 - normalizedParameter) * T0 + normalizedParameter * T1;
        }
    }
}
