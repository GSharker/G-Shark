using VerbNurbsSharp.Geometry;

namespace VerbNurbsSharp.Core
{

    public class CurveCurveIntersection
    {
        /// <summary>
        /// where the intersection took place
        /// </summary>
        public Vector3 Point0 { get; set; }

        /// <summary>
        /// where the intersection took place on the second curve
        /// </summary>
        public Vector3 Point1 { get; set; }

        /// <summary>
        /// the parameter on the first curve
        /// </summary>
        public double U0 { get; set; }

        /// <summary>
        /// the parameter on the second curve
        /// </summary>
        public double U1 { get; set; }

        public CurveCurveIntersection(Vector3 point0, Vector3 point1, double u0, double u1)
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
        public Vector3 CurvePoint { get; set; }
        public Vector3 SurfacePoint { get; set; }

        public CurveSurfaceIntersection(double u, UV uV, Vector3 curvePoint, Vector3 surfacePoint)
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
        public Vector3 Vector3 { get; set; }
        public int FaceIndex0 { get; set; }
        public int FaceIndex1 { get; set; }
        public MeshIntersectionPoint Opp { get; set; } = null;
        public MeshIntersectionPoint Adj { get; set; } = null;
        public bool Visited { get; set; } = false;

        public MeshIntersectionPoint(UV uV0, UV uV1, Vector3 point, int faceIndex0, int faceIndex1)
        {
            UV0 = uV0;
            UV1 = uV1;
            Vector3 = point;
            FaceIndex0 = faceIndex0;
            FaceIndex1 = faceIndex1;
        }
    }

    public class PolylineMeshIntersection
    {
        public Vector3 Vector3 { get; set; }
        public double U { get; set; }
        public UV UV { get; set; }
        public int PolylineIndex { get; set; }
        public int FaceIndex { get; set; }

        public PolylineMeshIntersection(Vector3 point, double u, UV uV, int polylineIndex, int faceIndex)
        {
            Vector3 = point;
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
        public Vector3 Vector3 { get; set; }
        public double Dist { get; set; }

        public SurfaceSurfaceIntersectionPoint(UV uV0, UV uV1, Vector3 point, double dist)
        {
            UV0 = uV0;
            UV1 = uV1;
            Vector3 = point;
            Dist = dist;
        }
    }

    public class TriSegmentIntersection
    {
        public Vector3 Vector3 { get; set; }
        public double S { get; set; }
        public double T { get; set; }
        public double P { get; set; }
        public TriSegmentIntersection(Vector3 point, double s, double t, double p)
        {
            Vector3 = point;
            S = s;
            T = t;
            P = p;
        }
    }

    public class CurveTriPoint
    {
        public double U { get; set; }
        public UV UV { get; set; }
        public Vector3 Vector3 { get; set; }

        public CurveTriPoint(double u, UV uV, Vector3 point)
        {
            U = u;
            UV = uV;
            Vector3 = point;
        }
    }

    public class SurfacePoint
    {
        public UV UV { get; set; }
        public Vector3 Vector3 { get; set; }
        public Vector3 Normal { get; set; }
        public int Id { get; set; }
        public bool Degen { get; set; }

        public SurfacePoint(UV uV, Vector3 point, Vector3 normal, int id = -1, bool degen = false)
        {
            UV = uV;
            Vector3 = point;
            Normal = normal;
            Id = id;
            Degen = degen;
        }

        public static SurfacePoint FromUV(UV uv) => new SurfacePoint(uv, null, null);
    }

    public class CurvePoint
    {
        public double U { get; set; }
        public Vector3 Vector3 { get; set; }
        public CurvePoint(double u, Vector3 point)
        {
            U = u;
            Vector3 = point;
        }
    }
}
