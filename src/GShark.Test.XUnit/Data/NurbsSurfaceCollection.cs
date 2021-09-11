using GShark.Geometry;
using System.Collections.Generic;

namespace GShark.Test.XUnit.Data
{
    public class NurbsSurfaceCollection
    {
        public static NurbsSurface QuadrilateralSurface()
        {
            #region example
            Point3 p1 = new Point3(0.0, 0.0, 0.0);
            Point3 p2 = new Point3(10.0, 0.0, 0.0);
            Point3 p3 = new Point3(10.0, 10.0, 2.0);
            Point3 p4 = new Point3(0.0, 10.0, 4.0);

            NurbsSurface surface = NurbsSurface.CreateFromCorners(p1, p2, p3, p4);
            #endregion

            return surface;
        }

        public static NurbsSurface SurfaceFromPoints()
        {
            List<List<Point3>> pts = new List<List<Point3>>
            {
                new List<Point3>{ new Point3(0.0, 0.0, 0.0), new Point3(0.0, 10.0, 4.0)},
                new List<Point3>{ new Point3(5.0, 0.0, 0.0), new Point3(5.0,10.0,5.0)},
                new List<Point3>{ new Point3(10.0, 0.0, 0.0), new Point3(10.0, 10.0, 2.0)}
            };

            List<List<double>> weight = new List<List<double>>
            {
                new List<double>{1, 1},
                new List<double>{1, 2},
                new List<double>{1, 1},
            };

            return NurbsSurface.CreateFromPoints(2, 1, pts, weight);
        }

        public static NurbsSurface Loft()
        {
            List<Point3> pts0 = new List<Point3>
            {
                new (0, 2.5, 0),
                new (3, 2.5, 0),
                new (5, 2.5, 2),
                new (6, 2.5, -1),
                new (8, 2.5, 0),
                new (10, 2.5, 0)
            };

            List<Point3> pts1 = new List<Point3>
            {
                new (0, 5, 0),
                new (2, 5, 0),
                new (4, 5, 0),
                new (6, 5, 2),
                new (8, 5, 0),
                new (10, 5, 0)
            };

            List<Point3> pts2 = new List<Point3>
            {
                new (0, 7.5, 0),
                new (2, 7.5, 0),
                new (4, 7.5, 2),
                new (5, 7.5, -1),
                new (7, 7.5, 0),
                new (10, 7.5, 0)
            };

            List<Point3> pts3 = new List<Point3>
            {
                new (0, 10, 0),
                new (1, 10, 0),
                new (4, 10, -1.5),
                new (5, 10, 4),
                new (7, 10, -1.5),
                new (10, 10, 0)
            };

            Line ln = new Line(new Point3(0, 0, 0), new Point3(10, 0, 0));
            NurbsCurve crv0 = new NurbsCurve(pts0, 2);
            PolyLine poly = new PolyLine(pts1);
            NurbsCurve crv1 = new NurbsCurve(pts2, 2);
            NurbsCurve crv2 = new NurbsCurve(pts3, 3);
            List<NurbsBase> crvs = new List<NurbsBase> { ln, crv0, poly, crv1, crv2 };

            return NurbsSurface.CreateLoftedSurface(crvs);
        }
    }
}
