using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A Point in verb is represented simply by an list of double point numbers.
    /// So, you would write simply [0,0,0] to create a Point at the origin.
    /// </summary>
    public class Point : List<double> { }

    /// <summary>
    /// Like a Point, a Vector in verb is represented simply by an list of double point numbers.
    /// So, you would write simply [1,0,0] to create a Vector in the X direction.
    /// </summary>
    public class Vector : List<double> { }

    /// <summary>
    /// A Matrix is represented by a nested list of double point numbers.
    /// So, you would write simply [[1,0],[0,1]] to create a 2x2 identity matrix.
    /// </summary>
    public class Matrix : List<List<double>> { }

    /// <summary>
    /// A KnotArray is a non-decreasing sequence of doubles. Use the methods in <see cref="VerbNurbsSharp.Evaluation.Check"/>/> to validate KnotArray's.
    /// </summary>
    public class KnotArray : List<double> { }

    /// <summary>
    /// A Plane is simply an origin point and normal.
    /// </summary>
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
    //ToDo why Ray an Plan are the same?
    /// <summary>
    /// A Ray is simply an origin point and normal.
    /// </summary>
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
    /// <summary>
    /// A simple data structure representing a NURBS curve.
    /// NurbsCurveData does no checks for legality. You can use <see cref="VerbNurbsSharp.Evaluation.Check"/> for that.
    /// </summary>
    public class NurbsCurveData : Serializable<NurbsCurveData>
    {
        public NurbsCurveData(int degree, List<Point> controlPoints, List<double> knots)
        {
            Degree = degree;
            ControlPoints = controlPoints;
            Knots = knots;
        }
        /// <summary>
        /// Integer degree of curve.
        /// </summary>
        public int Degree { get; set; }
        /// <summary>
        /// 2d list of control points, where each control point is an list of length (dim).
        /// </summary>
        public List<Point> ControlPoints { get; set; }
        /// <summary>
        /// List of non-decreasing knot values.
        /// </summary>
        public List<double> Knots { get; }
    }
    /// <summary>
    /// A simple data structure representing a NURBS surface.
    /// NurbsSurfaceData does no checks for legality. You can use <see cref="VerbNurbsSharp.Evaluation.Check"/> for that.
    /// </summary>
    public class NurbsSurfaceData : Serializable<NurbsSurfaceData>
    {
        public NurbsSurfaceData(int degreeU, int degreeV, KnotArray knotsU, KnotArray knotsV, List<List<Point>> controlPoints)
        {
            DegreeU = degreeU;
            DegreeV = degreeV;
            KnotsU = knotsU;
            KnotsV = knotsV;
            ControlPoints = controlPoints;
        }
        /// <summary>
        /// Integer degree of surface in u direction.
        /// </summary>
        public int DegreeU { get; set; }
        /// <summary>
        /// Integer degree of surface in v direction.
        /// </summary>
        public int DegreeV { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in u direction.
        /// </summary>
        public KnotArray KnotsU { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in v direction.
        /// </summary>
        public KnotArray KnotsV { get; set; }
        /// <summary>
        /// 2d list of control points, the vertical direction (u) increases from top to bottom, the v direction from left to right,
        /// and where each control point is an list of length (dim).
        /// </summary>
        public List<List<Point>> ControlPoints { get; set; }
    }

    /// <summary>
    /// A triangular face of a mesh.
    /// </summary>
    public class Tri : List<int> { }
    /// <summary>
    /// A UV is simply an list of double point numbers.
    /// So, you would write simply [1,0] to create a UV.
    /// </summary>
    public class UV : List<double> { }

    /// <summary>
    /// A simple data structure representing a mesh. MeshData does not check for legality.
    /// </summary>
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
    /// <summary>
    /// A simple data structure representing a polyline.
    /// PolylineData is useful, for example, as the result of a curve tessellation.
    /// </summary>
    public class PolylineData : Serializable<PolylineData>
    {
        /// <summary>
        /// The points in the polyline.
        /// </summary>
        public List<Point> Points { get; set; }
        /// <summary>
        /// The parameters of the individual points.
        /// </summary>
        public List<double> Parameters { get; set; }
        public PolylineData(List<Point> points, List<double> parameters)
        {
            Points = points;
            Parameters = parameters;
        }
    }
    /// <summary>
    /// A simple data structure representing a NURBS volume. This data structure is largely experimental in intent. Like CurveData
    /// and SurfaceData, this data structure does no legality checks.
    /// </summary>
    public class VolumeData : Serializable<VolumeData>
    {
        /// <summary>
        /// Integer degree in u direction.
        /// </summary>
        public int DegreeU { get; set; }
        /// <summary>
        /// Integer degree in v direction.
        /// </summary>
        public int DegreeV { get; set; }
        /// <summary>
        /// Integer degree in w direction.
        /// </summary>
        public int DegreeW { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in u direction.
        /// </summary>
        public KnotArray KnotsU { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in v direction.
        /// </summary>
        public KnotArray KnotsV { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in w direction.
        /// </summary>
        public KnotArray KnotsW { get; set; }
        /// <summary>
        /// 3d list of control points, where rows are the u dir, and columns run along the positive v direction,
        /// and where each control point is an list of length (dim)
        /// </summary>
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
    /// <summary>
    /// A simple parametric data type representing a pair of two objects.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
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
    /// <summary>
    /// A simple parametric data type representing an "interval" between two numbers. This data structure does no legality checks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
