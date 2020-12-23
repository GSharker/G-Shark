using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{

    public class CurveCurveIntersection
    {
        /// <summary>
        /// where the intersection took place
        /// </summary>
        public Point Point0 { get; set; }

        /// <summary>
        /// where the intersection took place on the second curve
        /// </summary>
        public Point Point1 { get; set; }

        /// <summary>
        /// the parameter on the first curve
        /// </summary>
        public double U0 { get; set; }

        /// <summary>
        /// the parameter on the second curve
        /// </summary>
        public double U1 { get; set; }

        public CurveCurveIntersection(Point point0, Point point1, double u0, double u1)
        {
            Point0 = point0;
            Point1 = point1;
            U0 = u0;
            U1 = u1;
        }
    }

    public class CurveSurfaceIntersection
    {
        public double U { get; set; }
        public UV UV { get; set; }
        public Point CurvePoint { get; set; }
        public Point SurfacePoint { get; set; }

        public CurveSurfaceIntersection(double u, UV uV, Point curvePoint, Point surfacePoint)
        {
            U = u;
            UV = uV;
            CurvePoint = curvePoint;
            SurfacePoint = surfacePoint;
        }
    }

    public class MeshIntersectionPoint
    {
        public UV UV0 { get; set; }
        public UV UV1 { get; set; }
        public Point Point { get; set; }
        public int FaceIndex0 { get; set; }
        public int FaceIndex1 { get; set; }
        public MeshIntersectionPoint Opp { get; set; } = null;
        public MeshIntersectionPoint Adj { get; set; } = null;
        public bool Visited { get; set; } = false;

        public MeshIntersectionPoint(UV uV0, UV uV1, Point point, int faceIndex0, int faceIndex1)
        {
            UV0 = uV0;
            UV1 = uV1;
            Point = point;
            FaceIndex0 = faceIndex0;
            FaceIndex1 = faceIndex1;
        }
    }

    public class PolylineMeshIntersection
    {
        public Point Point { get; set; }
        public double U { get; set; }
        public UV UV { get; set; }
        public int PolylineIndex { get; set; }
        public int FaceIndex { get; set; }

        public PolylineMeshIntersection(Point point, double u, UV uV, int polylineIndex, int faceIndex)
        {
            Point = point;
            U = u;
            UV = uV;
            PolylineIndex = polylineIndex;
            FaceIndex = faceIndex;
        }
    }

    public class SurfaceSurfaceIntersectionPoint
    {
        public UV UV0 { get; set; }
        public UV UV1 { get; set; }
        public Point Point { get; set; }
        public double Dist { get; set; }

        public SurfaceSurfaceIntersectionPoint(UV uV0, UV uV1, Point point, double dist)
        {
            UV0 = uV0;
            UV1 = uV1;
            Point = point;
            Dist = dist;
        }
    }

    public class TriSegmentIntersection
    {
        public Point Point { get; set; }
        public double S { get; set; }
        public double T { get; set; }
        public double P { get; set; }
        public TriSegmentIntersection(Point point, double s, double t, double p)
        {
            Point = point;
            S = s;
            T = t;
            P = p;
        }
    }

    public class CurveTriPoint
    {
        public double U { get; set; }
        public UV UV { get; set; }
        public Point Point { get; set; }

        public CurveTriPoint(double u, UV uV, Point point)
        {
            U = u;
            UV = uV;
            Point = point;
        }
    }

    public class SurfacePoint
    {
        public UV UV { get; set; }
        public Point Point { get; set; }
        public Vector Normal { get; set; }
        public int Id { get; set; }
        public bool Degen { get; set; }

        public SurfacePoint(UV uV, Point point, Vector normal, int id = -1, bool degen = false)
        {
            UV = uV;
            Point = point;
            Normal = normal;
            Id = id;
            Degen = degen;
        }

        public static SurfacePoint FromUV(UV uv) => new SurfacePoint(uv, null, null);
    }

    public class CurvePoint
    {
        public double U { get; set; }
        public Point Point { get; set; }
        public CurvePoint(double u, Point point)
        {
            U = u;
            Point = point;
        }
    }
}
