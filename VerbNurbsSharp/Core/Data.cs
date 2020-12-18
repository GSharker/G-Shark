using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VerbNurbsSharp.Core
{
    public class Point : List<double> { }
    public class Vector : List<double> { }
    public class Matrix : List<List<double>> { }

    public class KnotArray : List<double> { }
    public class Tri : List<int> { }
    public class UV : List<double> { }

    public class Plane : Serializable<Plane>
    {
        public Vector Direction { get; set; }
        public Point Origin { get; set; }
        public Plane(Point origin, Vector direction)
        {
            this.Direction = direction;
            this.Origin = origin;
        }
    }

    public class Ray : Serializable<Ray>
    {
        public Vector Direction { get; set; }
        public Point Origin { get; set; }
        public Ray(Point origin, Vector direction)
        {
            this.Direction = direction;
            this.Origin = origin;
        }
    }

    public class NurbsCurveData : Serializable<NurbsCurveData>
    {
        public NurbsCurveData(int degree, KnotArray knots, List<Point> controlPoints)
        {
            Degree = degree;
            ControlPoints = controlPoints;
            Knots = knots;
        }

        public int Degree { get; set; }
        public List<Point> ControlPoints { get; set; }
        public KnotArray Knots { get; }
    }

    public class NurbsSurfaceData : Serializable<NurbsSurfaceData>
    {
        public NurbsSurfaceData(int degreeU, int degreeV, KnotArray knotsU, KnotArray knotsV, List<Point> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            KnotsU = knotsU;
            KnotsV = knotsV;
            ControlPoints = controlPoints;
        }

        public int DegreeU { get; set; }
        public int DegreeV { get; set; }
        public KnotArray KnotsU { get; set; }
        public KnotArray KnotsV { get; set; }
        public List<Point> ControlPoints { get; set; }
    }

    public class MeshData : Serializable<MeshData>
    {
        public List<Tri> Faces { get; set; }
        public List<Point> Points { get; set; }
        public List<Vector> Normals { get; set; }
        public List<UV> UVs { get; set; }

        public MeshData(List<Tri> faces, List<Point> points, List<Vector> normals, List<UV> uVs)
        {
            Faces = faces;
            Points = points;
            Normals = normals;
            UVs = uVs;
        }

        internal static MeshData Empty() => new MeshData(
            new List<Tri>(),
            new List<Point>(),
            new List<Vector>(),
            new List<UV>()
            );
    }

    public class PolylineData : Serializable<PolylineData>
    {
        public List<Point> Points { get; set; }
        public List<double> Parameters { get; set; }
        public PolylineData(List<Point> points, List<double> parameters)
        {
            Points = points;
            Parameters = parameters;
        }
    }

    public class VolumeData : Serializable<VolumeData>
    {
        public int DegreeU { get; set; }
        public int DegreeV { get; set; }
        public int DegreeW { get; set; }
        public KnotArray KnotsU { get; set; }
        public KnotArray KnotsV { get; set; }

        public KnotArray KnotsW { get; set; }
        public List<List<List<Point>>> ControlPoints { get; set; }
        public VolumeData(int degreeU, int degreeV, int degreeW, KnotArray knotsU, KnotArray knotsV, KnotArray knotsW, List<List<List<Point>>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            DegreeW = degreeW;
            KnotsU = knotsU;
            KnotsV = knotsV;
            KnotsW = knotsW;
            ControlPoints = controlPoints;
        }
    }

    public class Pair<T1, T2>
    {
        public T1 Item0 { get; set; }
        public T2 Item1 { get; set; }
        public Pair(T1 item0, T2 item1)
        {
            Item0 = item0;
            Item1 = item1;
        }
    }

    public class Interval<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }
        public Interval(T min, T max)
        {
            Min = min;
            Max = max;
        }
    }
}
