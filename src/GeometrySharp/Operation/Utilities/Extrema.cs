using System.Collections.Generic;

namespace GeometrySharp.Operation.Utilities
{
    /// <summary>
    /// This object describing the extrema of a curve.
    /// Where each dimension lists the array of t values at which an extremum occurs,
    /// and the values property is the aggregate of the t values across all dimensions.
    /// </summary>
    public class Extrema
    {
        public IList<double> Xvalue { get; set; }
        public IList<double> Yvalue { get; set; }
        public IList<double> Zvalue { get; set; }
        public IList<double> Values { get; set; }
    }
}
