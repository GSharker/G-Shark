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

            return new NurbsCurve(pts, degree);
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
            List<double> weights = new List<double> { 0.5, 0.5, 0.5 };

            return new NurbsCurve(pts, weights, degree);
        }

        public static NurbsCurve NurbsCurvePlanarExample()
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

        public static NurbsCurve NurbsCurve3DExample()
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

        public static NurbsCurve NurbsCurveCubicBezierPlanar()
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

        public static NurbsCurve NurbsCurveQuadraticBezierPlanar()
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

        public static NurbsCurve NurbsCurveQuadratic3DBezier()
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
    }
}
