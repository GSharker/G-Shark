using GeometrySharp.Core;
using GeometrySharp.Geometry;
using System.Collections.Generic;

namespace GeometrySharp.Test.XUnit.Data
{
    public class NurbsCurveCollection
    {
        public static NurbsCurve NurbsCurveExample()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>
            {
                new Vector3 {-10,15,5},
                new Vector3 {10,5,5},
                new Vector3 {20,0,0}
            };
            Knot knots = new Knot { 0, 0, 0, 1, 1, 1 };

            return new NurbsCurve(degree, knots, pts);
        }

        public static NurbsCurve NurbsCurvePtsAndWeightsExample()
        {
            int degree = 2;
            List<Vector3> pts = new List<Vector3>
            {
                new Vector3 {-10,15,5},
                new Vector3 {10,5,5},
                new Vector3 {20,0,0}
            };
            Knot knots = new Knot { 1, 1, 1, 1, 1, 1 };
            List<double> weights = new List<double> { 0.5, 0.5, 0.5 };

            return new NurbsCurve(degree, knots, pts, weights);
        }

        public static NurbsCurve NurbsCurveExample2()
        {
            Knot knots = new Knot { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 };
            int degree = 3;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {5,5,0},
                new Vector3 {10, 10, 0},
                new Vector3 {20, 15, 0},
                new Vector3 {35, 15, 0},
                new Vector3 {45, 10, 0},
                new Vector3 {50, 5, 0}
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurveExample3()
        {
            Knot knots = new Knot { 0, 0, 0, 0.24, 1, 1, 1 };
            int degree = 2;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {0, 0, 0},
                new Vector3 {1, 0, 0},
                new Vector3 {0.5, 1, 0},
                new Vector3 {2, 0, 0}
            };
            return new NurbsCurve(degree, knots, controlPts);
        }
    }
}
