using System.Collections.Generic;
using GShark.Core;
using GShark.Geometry;

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
    }
}
