using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    public class Vec
    {
        public static Vector Cross(Vector u, Vector v) => new Vector() { u[1] * v[2] - u[2] * v[1], u[2] * v[0] - u[0] * v[2], u[0] * v[1] - u[1] * v[0] };

        public static double AngleBetween(Vector a, Vector b) => Math.Acos(Dot(a, b) / (Norm(a) * Norm(b)));

        internal static double AngleBetweenNormalized3D(Vector a, Vector b) => double.NaN;

        public static double Domain(Vector a) => a.Last() - a.First();

        public static double PositiveAngleBetween(Vector a, Vector b, Vector n) => double.NaN;

        public static double Dist(Point a, Point b) => Norm(Sub(FromPoint(a), FromPoint(b)));

        public static double SignedAngleBetween(Vector a, Vector b, Vector n) => double.NaN;

        public static double Norm(Vector a) => NormSquared(a) != 0 ? Math.Sqrt(NormSquared(a)) : NormSquared(a);

        public static double NormSquared(Vector a) => Math.Pow(a[0], 2) + Math.Pow(a[1], 2) + Math.Pow(a[2], 2);

        public static double Dot(Vector a, Vector b) => a[0] * b[0] + a[1] * b[1] + a[2] * b[2];

        public static Vector Add(Vector a, Vector b) => new Vector() { a[0] + b[0], a[1] + b[1], a[2] + b[2] };

        public  static List<double> Zeros1d(int rows)
        {
            var list = new List<double>();
            for (int i = 0; i < rows; i++)
                list.Add(0.0);
            return list;
        }

        public static Vector Mul(double a, Vector b) => new Vector() { a * b[0], a * b[1], a * b[2] };
        public static List<double> ScalarMult(double a, List<double> b)
        {
            for (int i = 0; i < b.Count; i++)
                b[i] *= a;
            return b;
        }

        public static Vector Div(Vector a, double b) => new Vector() { a[0] / b, a[1] / b, a[2] / b };

        public static Vector Sub(Vector a, Vector b) => new Vector() { a[0] - b[0], a[1] - b[1], a[2] - b[2] };
        public static Vector Sub(Point a, Point b) => new Vector() { a[0] - b[0], a[1] - b[1], a[2] - b[2] };

        public static Vector FromPoint(Point a) => new Vector() { a[0], a[1], a[2] };
        public static Point ToPoint(Vector a) => new Point() { a[0], a[1], a[2] };

        public static List<T> Rep<T>(int num, T ele)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < num; i++)
                list.Add(ele);
            return list;
        }
    }
}
