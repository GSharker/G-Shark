using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A Point in verb is represented simply by an list of double point numbers.
    /// So, you would write simply [0,0,0] to create a Point at the origin.
    /// </summary>
    public class Point : List<double>
    {
        public Point()
        {
        }

        public Point(IEnumerable<double> values)
        {
            this.AddRange(values);
        }
        /// <summary>
        /// Gets the value of a point at location Constants.UNSETVALUE,Constants.UNSETVALUE,Constants.UNSETVALUE.
        /// </summary>
        public static Point Unset => new Point() { Constants.UNSETVALUE, Constants.UNSETVALUE, Constants.UNSETVALUE };

        /// <summary>
        /// Constructs the string representation of the point.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Math.Round(this[0], 6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}";
        }
    }
}
