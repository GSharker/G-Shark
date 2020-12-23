using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Vec
    {
          /// <summary>
        /// Multiply a n dimension vector by a constant
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector Mul(double a, Vector b) {
            var nV = new Vector();
            foreach (var c in b)
                nV.Add(c * a);
            return nV;
        }

        public static List<T> Rep<T>(int num, T ele)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < num; i++)
                list.Add(ele);
            return list;
        }
    }
}
