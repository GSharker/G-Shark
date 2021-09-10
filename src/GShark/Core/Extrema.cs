using System;
using System.Collections.Generic;

namespace GShark.Operation.Utilities
{
    /// <summary>
    /// This object describing the extrema of a curve.<br/>
    /// Where each dimension lists the array of parameter values on the curve at which an extremum occurs,<br/>
    /// and the values property is the aggregate of the parameter values across all dimensions.
    /// </summary>
    internal class Extrema
    {
        internal List<double> Xvalue { get; set; }
        internal List<double> Yvalue { get; set; }
        internal List<double> Zvalue { get; set; }
        internal List<double> Values { get; set; }

        internal List<double> this[int index]
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
