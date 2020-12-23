using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Mat
    {
        public static Vector Solve(Matrix A, Vector b) => LUSolve(LU(A), b);

        public static Vector LUSolve(object v, List<double> b)
        {
            throw new NotImplementedException();
        }

        private static LUDecomp LU(Matrix A)
        {
            throw new NotImplementedException();
        }

        internal static List<List<T>> Transpose<T>(List<List<T>> xs)
        {
            throw new NotImplementedException();
        }
    }

    public class LUDecomp
    {
        public Matrix LU { get; set; }
        public List<int> P { get; set; }

        public LUDecomp(Matrix lU, List<int> p)
        {
            LU = lU;
            P = p;
        }
    }
}
