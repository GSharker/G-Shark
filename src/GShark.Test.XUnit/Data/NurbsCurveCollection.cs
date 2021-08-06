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
            List<Point3> pts = new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
            };
            KnotVector knots = new KnotVector { 0, 0, 0, 1, 1, 1 };

            return new NurbsCurve(degree, knots, pts);
        }

        public static NurbsCurve NurbsCurvePtsAndWeightsExample()
        {
            int degree = 2;
            List<Point3> pts = new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
            };
            KnotVector knots = new KnotVector { 1, 1, 1, 1, 1, 1 };
            List<double> weights = new List<double> { 0.5, 0.5, 0.5 };

            return new NurbsCurve(degree, knots, pts, weights);
        }

        public static NurbsCurve NurbsCurvePlanarExample()
        {
            KnotVector knots = new KnotVector { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 };
            int degree = 3;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(5, 5, 0),
                new Point3(10, 10, 0),
                new Point3(20, 15, 0),
                new Point3(35, 15, 0),
                new Point3(45, 10, 0),
                new Point3(50, 5, 0)
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurve3DExample()
        {
            #region example
            int degree = 3;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(4, 0, 0),
                new Point3(5, 5, 5),
                new Point3(0, 5, 0),
            };
            NurbsCurve curve = new NurbsCurve(controlPts, degree);
            #endregion

            return curve;
        }

        public static NurbsCurve NurbsCurveCubicBezierPlanar()
        {
            KnotVector knots = new KnotVector { 0, 0, 0, 0, 1, 1, 1, 1 };
            int degree = 3;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(1, 0, 0),
                new Point3(0.5, 1, 0),
                new Point3(2, 0, 0)
            };
            return new NurbsCurve(degree, knots, controlPts);
        }

        public static NurbsCurve NurbsCurveQuadraticBezierPlanar()
        {
            int degree = 2;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
            };
            return new NurbsCurve(controlPts, degree);
        }

        public static NurbsCurve NurbsCurveQuadratic3DBezier()
        {
            int degree = 2;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(4.5,2.5,2.5),
                new Point3(5,5,5),
                new Point3(0,5,0)
            };
            return new NurbsCurve(controlPts, degree);
        }

        public static NurbsCurve PeriodicClosedNurbsCurve()
        {
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(4, 0, 0),
                new Point3(5, 5, 5),
                new Point3( 0, 5, 0 )
            };
            return new NurbsCurve(pts, degree).Close();
        }

        public static NurbsCurve NurbsCurveWithStartingAndEndPointOverlapping()
        {
            int degree = 2;
            List<Point3> pts = new List<Point3>
            {
                new Point3 (4.5,2.5,2.5),
                new Point3 (5,5,5),
                new Point3 (0,5,0),
                new Point3 (4.5,2.5,2.5)
            };
            return new NurbsCurve(pts, degree);
        }

        public static List<NurbsCurve> OpenCurves()
        {
            int degree = 2;
            List<Point3> points1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0) };

            List<Point3> points2 = new List<Point3> { new Point3(-15.0, 10.0, 0.0),
                                                   new Point3(0.0, 10.0, 5.0),
                                                   new Point3(20.0, 10.0, 1.0) };

            List<Point3> points3 = new List<Point3> { new Point3(-5.0, 25.0, 0.0),
                                                   new Point3(0.0, 25.0, 20.0),
                                                   new Point3(10.0, 25.0, 0.0) };

            List<Point3> points4 = new List<Point3> { new Point3(-5.0, 35.0, -2.0),
                                                   new Point3(0.0, 35.0, 20.0),
                                                   new Point3(5.0, 35.0, 0.0) };

            NurbsCurve c1 = new NurbsCurve(points1, degree);
            NurbsCurve c2 = new NurbsCurve(points2, degree);
            NurbsCurve c3 = new NurbsCurve(points3, degree);
            NurbsCurve c4 = new NurbsCurve(points4, degree);

            return new List<NurbsCurve>() { c1, c2, c3, c4 };
        }
        
        public static List<NurbsCurve> ClosedCurves()
        {
            List<NurbsCurve> crvs = NurbsCurveCollection.OpenCurves();
            for (int i = 0; i < crvs.Count; i++)
                crvs[i] = crvs[i].Close();

            return crvs;
        }
    }
}
