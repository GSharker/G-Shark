using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Mesh
    {
        public static Matrix Mul(double a, Matrix b) => throw new NotImplementedException();
        public static Matrix Mutl(Matrix a, Matrix b) => throw new NotImplementedException();
        public static Matrix Add(Matrix a, Matrix b) => throw new NotImplementedException();
        public static Matrix Add(Matrix a, double b) => throw new NotImplementedException();
        public static Matrix Sub(Matrix a, Matrix b) => throw new NotImplementedException();
        public static Vector Dot(Matrix a, Vector b) => throw new NotImplementedException();
        public static Vector Identity(int i) => throw new NotImplementedException();
        public static Matrix Transpose(Matrix a) => throw new NotImplementedException();
        public static Vector Solve(Matrix a, Vector b) => throw new NotImplementedException();
        public static Vector LUSolve(LUDecomp LUP, Vector b) => throw new NotImplementedException();
        public static LUDecomp LU(Matrix a) => throw new NotImplementedException();
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
