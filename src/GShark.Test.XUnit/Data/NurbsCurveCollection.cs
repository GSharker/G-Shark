using GShark.Geometry;
using System.Collections.Generic;

namespace GShark.Test.XUnit.Data
{
    public class NurbsCurveCollection
    {
        public static NurbsCurve NurbsBaseExample()
        {
            int degree = 2;
            List<Point3> pts = new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
            };

            return new NurbsCurve(pts, degree);
        }

        public static NurbsCurve NurbsPtsAndWeightsExample()
        {
            int degree = 2;
            List<Point3> pts = new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
            };
            List<double> weights = new List<double> { 0.5, 0.5, 0.5 };

            return new NurbsCurve(pts, weights, degree);
        }

        public static NurbsCurve NurbsPlanarExample()
        {
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(5, 5, 0),
                new Point3(10, 10, 0),
                new Point3(20, 15, 0),
                new Point3(35, 15, 0),
                new Point3(45, 10, 0),
                new Point3(50, 5, 0)
            };
            return new NurbsCurve(pts, degree);
        }

        public static NurbsCurve Nurbs3DExample()
        {
            #region example
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(4, 0, 0),
                new Point3(5, 5, 5),
                new Point3(0, 5, 0),
            };
            NurbsCurve curve = new NurbsCurve(pts, degree);
            #endregion

            return curve;
        }

        public static NurbsCurve NurbsCubicBezierPlanar()
        {
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(1, 0, 0),
                new Point3(0.5, 1, 0),
                new Point3(2, 0, 0)
            };
            return new NurbsCurve(pts, degree);
        }

        public static NurbsCurve NurbsQuadraticBezierPlanar()
        {
            int degree = 2;
            List<Point3> pts = new List<Point3>
            {
                new Point3(-10,15,5),
                new Point3(10,5,5),
                new Point3(20,0,0)
            };
            return new NurbsCurve(pts, degree);
        }

        public static NurbsCurve NurbsBaseQuadratic3DBezier()
        {
            int degree = 2;
            List<Point3> pts = new List<Point3>
            {
                new Point3(4.5,2.5,2.5),
                new Point3(5,5,5),
                new Point3(0,5,0)
            };
            return new NurbsCurve(pts, degree);
        }

        public static NurbsBase PeriodicClosedNurbs()
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

        public static NurbsCurve NurbsWithStartingAndEndPointOverlapping()
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

        public static List<NurbsBase> OpenNurbs()
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

            return new List<NurbsBase> { c1, c2, c3, c4 };
        }

        public static List<NurbsBase> ClosedNurbs()
        {
            List<NurbsBase> crvs = NurbsCurveCollection.OpenCurves();
            for (int i = 0; i < crvs.Count; i++)
                crvs[i] = crvs[i].Close();

            return crvs;
        }
    }
}
