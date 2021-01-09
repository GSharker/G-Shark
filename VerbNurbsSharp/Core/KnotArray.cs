using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A KnotArray is a non-decreasing sequence of doubles. Use the methods in <see cref="VerbNurbsSharp.Evaluation.Check"/>/> to validate KnotArray's.
    /// </summary>
    public class KnotArray : List<double>
    {
        public KnotArray()
        {
        }

        public KnotArray(IEnumerable<double> values)
        {
            this.AddRange(values);
        }
    }
}
