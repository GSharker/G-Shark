using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Operation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

/// You can find further documentation for this type at
/// [https://github.com/jdonaldson/promhx](https://github.com/jdonaldson/promhx).
namespace GShark.Geometry
{

    /// <summary>
    /// A simple data structure representing a NURBS surface.
    /// </summary>
    //public class NurbsSurface : Serializable<NurbsSurface>, IEquatable<NurbsSurface>
    //{
        ///// <summary>
        ///// Construct a NurbsSurface by degree, knots, control points, weights
        ///// </summary>
        ///// <param name="degreeU">The degree in the U direction</param>
        ///// <param name="degreeV">The degree in the V direction</param>
        ///// <param name="knotsU">The knot array in the U direction</param>
        ///// <param name="knotsV">The knot array in the V direction</param>
        ///// <param name="controlPoints">Two dimensional array of points</param>
        ///// <param name="weights">Two dimensional array of weight values</param>
        ///// <return>A new NurbsSurface</return>
        //public NurbsSurface(int degreeU, int degreeV, KnotVector knotsU, KnotVector knotsV, List<List<Point3d>> controlPoints, List<List<double>>? weights = null)
        //{

        //    if (controlPoints == null) throw new ArgumentNullException("Control points array connot be null!");
        //    if (degreeU < 1) throw new ArgumentException("DegreeU must be greater than 1!");
        //    if (degreeV < 1) throw new ArgumentException("DegreeV must be greater than 1!");
        //    if (knotsU == null) throw new ArgumentNullException("KnotU cannot be null!");
        //    if (knotsV == null) throw new ArgumentNullException("KnotV cannot be null!");
        //    if (knotsU.Count != controlPoints.Count + degreeU + 1)
        //        throw new ArgumentException("controlPointsU.length + degreeU + 1 must equal knotsU.length!");
        //    if (knotsV.Count != controlPoints[0].Count + degreeV + 1)
        //        throw new ArgumentException("controlPointsV.length + degreeV + 1 must equal knotsV.length!");
        //    if (!knotsU.IsValid(degreeU, controlPoints.Count) || !knotsV.IsValid(degreeV, controlPoints[0].Count))
        //        throw new ArgumentException("Invalid knot knots format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
        //    //return data;

        //    DegreeU = degreeU;
        //    DegreeV = degreeV;
        //    KnotsU = knotsU;
        //    KnotsV = knotsV;
        //    Weights = weights ?? Sets.RepeatData(Sets.RepeatData(1.0, controlPoints.Count), controlPoints[0].Count);
        //    HomogenizedPoints = LinearAlgebra.PointsHomogeniser2d(controlPoints, weights);
        //    DomainU = new Interval(this.KnotsU.First(), this.KnotsU.Last());
        //    DomainV = new Interval(this.KnotsV.First(), this.KnotsV.Last());
        //}

        ///// <summary>
        ///// Creates a Nurbs surface object.
        ///// </summary>
        ///// <param name="controlPoints">Control points, as a 2d collection of Vector3.</param>
        ///// <param name="degreeU">Surface degree u</param>
        ///// <param name="degreeV">Surface degree v</param>
        //public NurbsSurface(List<List<Point3d>> controlPoints, int degreeU, int degreeV)
        //    : this(degreeU, degreeV, new KnotVector(degreeU, controlPoints.Count), new KnotVector(degreeV, controlPoints[0].Count), controlPoints)
        //{
        //}

        ///// <summary>
        ///// Construct a NurbsSurface by a NurbsSurface object.
        ///// </summary>
        ///// <param name="curve">The curve object</param>
        //public NurbsSurface(NurbsSurface surface)
        //{
        //    DegreeU = surface.DegreeU;
        //    DegreeV = surface.DegreeV;
        //    HomogenizedPoints = new List<List<Point4d>>(surface.HomogenizedPoints);
        //    KnotsU = new KnotVector(surface.KnotsU);
        //    KnotsV = new KnotVector(surface.KnotsV);
        //    Weights = new List<List<double>>(surface.Weights!);
        //}

        ///// <summary>
        ///// Construct a NurbsSurface from four perimeter points in counter-clockwise order
        ///// </summary>
        ///// <param name="p1">The first point</param>
        ///// <param name="p2">The second point</param>
        ///// <param name="p3">The third point</param>
        ///// <param name="p4">The fourth point</param>
        //public static NurbsSurface ByFourPoints(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        //{
        //    int degree = 1;
        //    var pts = new List<List<Point3d>>();
        //    for (int i = 0; i < degree + 1; i++)
        //    {
        //        var row = new List<Point3d>();
        //        for (int j = 0; j < degree + 1; j++)
        //        {
        //            var l = (1.0d - i / degree);
        //            var p1p2 = Point3d.Interpolate(p1, p2, l);
        //            var p4p3 = Point3d.Interpolate(p4, p3, l);
        //            var res = Point3d.Interpolate(p1p2, p4p3, 1.0d - j / degree);
        //            var res4d = new Point4d(res.X, res.Y, res.Z, 1.0d);
        //            row.Add(res);
        //        }
        //        pts.Add(row);
        //    }
        //    var zeros = Sets.RepeatData(0.0d, degree + 1);
        //    var ones = Sets.RepeatData(1.0d, degree + 1);
        //    KnotVector knotU = zeros.Concat(ones).ToKnot();
        //    KnotVector knotV = zeros.Concat(ones).ToKnot();

        //    return new NurbsSurface(degree, degree, knotU, knotV, pts);
        //}

        ///// <summary>
        ///// Obtain the surface normal at the given u and v parameters
        ///// </summary>
        ///// <param name="u">u parameter</param>
        ///// <param name="v">v parameter</param>
        ///// <returns></returns>
        //public Vector3d Normal(double u, double v) => Evaluation.RationalSurfaceNormal(this, u, v).Unitize();

        ///// <summary>
        ///// Obtain the surface tangent at the given u and v parameters in the u direction
        ///// </summary>
        ///// <param name="u">u parameter</param>
        ///// <param name="v">v parameter</param>
        ///// <returns></returns>
        //public Vector3d TangentAtU(double u, double v) => Evaluation.RationalSurfaceDerivatives(this, u, v)[1][0].Unitize();

        ///// <summary>
        ///// Obtain the surface tangent at the given u and v parameters in the v direction
        ///// </summary>
        ///// <param name="u">u parameter</param>
        ///// <param name="v">v parameter</param>
        ///// <returns></returns>
        //public Vector3d TangentAtV(double u, double v) => Evaluation.RationalSurfaceDerivatives(this, u, v)[0][1].Unitize();

        ///// <summary>
        ///// Integer degree of surface in u direction.
        ///// </summary>
        //public int DegreeU { get; }
        ///// <summary>
        ///// Integer degree of surface in v direction.
        ///// </summary>
        //public int DegreeV { get; }
        ///// <summary>
        ///// List of non-decreasing knot values in u direction.
        ///// </summary>
        //public KnotVector KnotsU { get; }
        ///// <summary>
        ///// List of non-decreasing knot values in v direction.
        ///// </summary>
        //public KnotVector KnotsV { get; }

        ///// <summary>
        ///// Determine the valid u domain of the surface.
        ///// </summary>
        //public Interval DomainU { get; }

        ///// <summary>
        ///// Determine the valid v domain of the surface.
        ///// </summary>
        //public Interval DomainV { get; }

        ///// <summary>
        ///// Obtain a copy of the NurbsSurface.
        ///// </summary>
        ///// <returns>The copied curve.</returns>
        //public NurbsSurface Clone() => new NurbsSurface(this);


        ///// <summary>
        ///// 2d list of control points, the vertical direction (u) increases from top to bottom, the v direction from left to right,
        ///// and where each control point is an list of length (dim).
        ///// </summary>
        //public List<List<Point3d>> ControlPoints => LinearAlgebra.PointDehomogenizer2d(HomogenizedPoints);
        ///// <summary>
        /////Two dimensional array of weight values
        ///// </summary>
        //public List<List<double>> Weights { get; }
        //public List<List<Point4d>> HomogenizedPoints { get; }

        //public bool Equals(NurbsSurface other){
        //    //var pts = this.ControlPoints;
        //    //var otherPts = other?.ControlPoints;

        //    //if (other == null) return false;
        //    //if (pts.Count != otherPts.Count) return false;
        //    //if (this.KnotsU.Count != other.KnotsU.Count) return false;
        //    //if (this.KnotsV.Count != other.KnotsV.Count) return false;
        //    //if (this.DegreeU != other.DegreeU) return false;
        //    //if (this.DegreeV != other.DegreeV) return false;
        //    /////
        //    throw new NotImplementedException();
        //}
        //public override NurbsSurface FromJson(string s) => throw new NotImplementedException();

        ///// <summary>
        ///// Serialize a nurbs surface to JSON
        ///// </summary>
        ///// <returns></returns>
        //public override string ToJson() => JsonConvert.SerializeObject(this);
    //}
}