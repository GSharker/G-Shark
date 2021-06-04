using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;

namespace GShark.Test.XUnit.Data
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
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 1, 1 };

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
            KnotVector knots = new KnotVector { 1, 1, 1, 1, 1, 1 };
            List<double> weights = new List<double> { 0.5, 0.5, 0.5 };

            return new NurbsCurve(degree, knots, pts, weights);
        }

        public static NurbsCurve NurbsCurvePlanarExample()
        {
            KnotVector knots = new KnotVector { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 };
            int degree = 3;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {5, 5, 0},
                new Vector3 {10, 10, 0},
                new Vector3 {20, 15, 0},
                new Vector3 {35, 15, 0},
                new Vector3 {45, 10, 0},
                new Vector3 {50, 5, 0}
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurve3DExample()
        {
            #region example
            int degree = 3;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {0, 5, 5},
                new Vector3 {0, 0, 0},
                new Vector3 {4, 0, 0},
                new Vector3 {5, 5, 5},
                new Vector3 {0, 5, 0},
            };
            NurbsCurve curve = new NurbsCurve(controlPts, degree);
            #endregion

            return curve;
        }

        public static NurbsCurve NurbsCurveCubicBezierPlanar()
        {
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };
            int degree = 3;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {0, 0, 0},
                new Vector3 {1, 0, 0},
                new Vector3 {0.5, 1, 0},
                new Vector3 {2, 0, 0}
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurveQuadraticBezierPlanar()
        {
            int degree = 2;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {-10,15,5},
                new Vector3 {10,5,5},
                new Vector3 {20,0,0}
            };
            return new NurbsCurve(controlPts, degree);
        }

        public static NurbsCurve NurbsCurveQuadratic3DBezier()
        {
            int degree = 2;
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {4.5,2.5,2.5},
                new Vector3 {5,5,5},
                new Vector3 {0,5,0}
            };
            return new NurbsCurve(controlPts, degree);
        }
    }
}
