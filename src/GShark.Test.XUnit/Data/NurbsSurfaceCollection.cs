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

            NurbsSurface surface = NurbsSurface.ByFourPoints(p1, p2, p3, p4);
            #endregion

            return surface;
        }
    }
}
