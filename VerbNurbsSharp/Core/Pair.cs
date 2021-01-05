using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A simple parametric data type representing a pair of two objects.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Pair<T1, T2>
    {
        public T1 Item0 { get; set; }
        public T2 Item1 { get; set; }
        public Pair(T1 item0, T2 item1)
        {
            Item0 = item0;
            Item1 = item1;
        }
    }
}
