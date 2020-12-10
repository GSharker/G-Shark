using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeometryLib.Core
{
    public class Coordinates
    {
        [JsonProperty]
        public double X { get; set; }
        [JsonProperty]
        public double Y { get; set; }
        [JsonProperty]
        public double Z { get; set; }
    }

    public class Point : Serializable<Point>
    {
        [JsonConstructor]
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public class Vector : Serializable<Vector>
    {
        [JsonConstructor]
        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public class Plane : Serializable<Plane>
    {

        [JsonProperty]  public Point Origin { get; set; }
        [JsonProperty]  public Vector Normal { get; set; }

        [JsonConstructor]
        public Plane(Point origin, Vector normal)
        {
            Origin = origin;
            Normal = normal;
        }
    }
    public class Ray
    {
        public Point Origin { get; set; }
        public Vector Direction { get; set; }

        public Ray(Point origin, Vector direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }
    public class Face
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }
    public class UV
    {
        public double U { get; set; }
        public double V { get; set; }
    }
    public class NurbsCurveData
    {
        public int Degree { get; set; }
        public List<double> Knots { get; set; }
        public List<Point> ControlPoints { get; set; }
        public NurbsCurveData(int degree, List<double> knots, List<Point> controlPoints)
        {
            Degree = degree;
            Knots = knots;
            ControlPoints = controlPoints;
        }

    }
    public class NurbsSurfaceData
    {
        public int DegreeU { get; set; }
        public int DegreeV { get; set; }
        public List<double> KnotsU { get; set; }
        public List<double> KnotsV { get; set; }
        public List<List<Point>> ControlPoints { get; set; }
        public NurbsSurfaceData(int degreeU, int degreeV, List<double> knotsU, List<double> knotsV, List<List<Point>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            KnotsU = knotsU;
            KnotsV = knotsV;
            ControlPoints = controlPoints;
        }

    }
    public class MeshData
    {

        public MeshData(List<Face> faces, List<Point> points, List<Vector> normals, List<UV> uvs)
        {
            Faces = faces;
            Points = points;
            Normals = normals;
            UVs = uvs;
        }

        public List<Face> Faces { get; set; }
        public List<Point> Points { get; set; }
        public List<Vector> Normals { get; set; }
        public List<UV> UVs { get; set; }

        public static MeshData Empty() => new MeshData(new List<Face>(), new List<Point>(), new List<Vector>(), new List<UV>());
    }
    public class PolylineData
    {
        public PolylineData(List<Point> points, List<double> parameters)
        {
            Points = points;
            Parameters = parameters;
        }

        public List<Point> Points { get; }
        public List<double> Parameters { get; }
    }
    public class VolumeData
    {
        public VolumeData(int degreeU, int degreeV, int degreeW, List<double> knotsU, List<double> knotsV, List<double> knotsW, List<List<List<Point>>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            DegreeW = degreeW;
            KnotsU = knotsU;
            KnotsV = knotsV;
            KnotsW = knotsW;
            ControlPoints = controlPoints;
        }

        public int DegreeU { get; }
        public int DegreeV { get; }
        public int DegreeW { get; }
        public List<double> KnotsU { get; }
        public List<double> KnotsV { get; }
        public List<double> KnotsW { get; }
        public List<List<List<Point>>> ControlPoints { get; }
    }
    public class Pair<T1, T2>
    {
        public Pair(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; }
        public T2 Item2 { get; }
    }
    public class Interval<T>
    {
        public Interval(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public T Min { get; }
        public T Max { get; }
    }
}
