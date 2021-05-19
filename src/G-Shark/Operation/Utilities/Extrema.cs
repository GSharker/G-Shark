using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace GShark.Operation.Utilities
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

        public IList<double> this[int index]
        {
            get
            {
                return index switch
                {
                    0 => Xvalue,
                    1 => Yvalue,
                    2 => Zvalue,
                    3 => Values,
                    _ => throw new IndexOutOfRangeException()
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Xvalue = new List<double>(value);
                        break;
                    case 1:
                        Yvalue = new List<double>(value);
                        break;
                    case 2:
                        Zvalue = new List<double>(value);
                        break;
                    case 3:
                        Values = new List<double>(value);
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }
}
