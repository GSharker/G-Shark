using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometrySharp.Core
{
    // ToDo originally interval took T as type. If necessary re-introduce as a T.
    /// <summary>
    /// A simple parametric data type representing an "interval" between two numbers. This data structure does no legality checks.
    /// </summary>
    /// <typeparam name=""></typeparam>
    public class Interval
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double ParameterAt(double normalizedParameter)
        {
            return !Math.IsValidDouble(normalizedParameter)
                ? -1.23432101234321E+308
                : (1.0 - normalizedParameter) * this.Min + normalizedParameter * this.Max;
        }
    }
}
