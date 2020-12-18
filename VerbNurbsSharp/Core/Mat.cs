using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Mat
    {
        public static object Solve(Matrix A, List<double> b) => LUSolve(LU(A), b);

        public static Vector LUSolve(object v, List<double> b)
        {
            throw new NotImplementedException();
        }

        private static LUDecomp LU(Matrix A)
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
}
