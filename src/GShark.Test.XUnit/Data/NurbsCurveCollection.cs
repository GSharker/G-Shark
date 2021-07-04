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
            List<Point3d> pts = new List<Point3d>
            {
                new Point3d(-10,15,5),
                new Point3d(10,5,5),
                new Point3d(20,0,0)
            };
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 1, 1 };

            return new NurbsCurve(degree, knots, pts);
        }

        public static NurbsCurve NurbsCurvePtsAndWeightsExample()
        {
            int degree = 2;
            List<Point3d> pts = new List<Point3d>
            {
                new Point3d(-10,15,5),
                new Point3d(10,5,5),
                new Point3d(20,0,0)
            };
            KnotVector knots = new KnotVector { 1, 1, 1, 1, 1, 1 };
            List<double> weights = new List<double> { 0.5, 0.5, 0.5 };

            return new NurbsCurve(degree, knots, pts, weights);
        }

        public static NurbsCurve NurbsCurvePlanarExample()
        {
            KnotVector knots = new KnotVector { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 };
            int degree = 3;
            List<Point3d> controlPts = new List<Point3d>
            {
                new Point3d(5, 5, 0),
                new Point3d(10, 10, 0),
                new Point3d(20, 15, 0),
                new Point3d(35, 15, 0),
                new Point3d(45, 10, 0),
                new Point3d(50, 5, 0)
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurve3DExample()
        {
            #region example
            int degree = 3;
            List<Point3d> controlPts = new List<Point3d>
            {
                new Point3d(0, 5, 5),
                new Point3d(0, 0, 0),
                new Point3d(4, 0, 0),
                new Point3d(5, 5, 5),
                new Point3d(0, 5, 0),
            };
            NurbsCurve curve = new NurbsCurve(controlPts, degree);
            #endregion

            return curve;
        }

        public static NurbsCurve NurbsCurveCubicBezierPlanar()
        {
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };
            int degree = 3;
            List<Point3d> controlPts = new List<Point3d>
            {
                new Point3d(0, 0, 0),
                new Point3d(1, 0, 0),
                new Point3d(0.5, 1, 0),
                new Point3d(2, 0, 0)
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurveQuadraticBezierPlanar()
        {
            int degree = 2;
            List<Point3d> controlPts = new List<Point3d>
            {
                new Point3d(-10,15,5),
                new Point3d(10,5,5),
                new Point3d(20,0,0)
            };
            return new NurbsCurve(controlPts, degree);
        }

        public static NurbsCurve NurbsCurveQuadratic3DBezier()
        {
            int degree = 2;
            List<Point3d> controlPts = new List<Point3d>
            {
                new Point3d(4.5,2.5,2.5),
                new Point3d(5,5,5),
                new Point3d(0,5,0)
            };
            return new NurbsCurve(controlPts, degree);
        }
    }
}
