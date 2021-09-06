using System;
using System.Collections.Generic;

namespace GShark.Operation.Utilities
{
    /// <summary>
    /// This object describing the extrema of a curve.<br/>
    /// Where each dimension lists the array of t values at which an extremum occurs,<br/>
    /// and the values property is the aggregate of the t values across all dimensions.
    /// </summary>
    public class Extrema
    {
        public List<double> Xvalue { get; set; }
        public List<double> Yvalue { get; set; }
        public List<double> Zvalue { get; set; }
        public List<double> Values { get; set; }

        public List<double> this[int index]
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
