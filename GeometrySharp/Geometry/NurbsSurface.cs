using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

/// You can find further documentation for this type at
/// [https://github.com/jdonaldson/promhx](https://github.com/jdonaldson/promhx).
namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a NURBS surface.
    /// NurbsSurfaceData does no checks for legality. You can use <see cref="GeometrySharp.Evaluation.Check"/> for that.
    /// </summary>
    public class NurbsSurface : Serializable<NurbsSurface>, IEquatable<NurbsSurface>, ISurface
    {
        /// <summary>
        /// Construct a NurbsSurface by degree, knots, control points, weights
        /// </summary>
        /// <param name="degreeU">The degree in the U direction</param>
        /// <param name="degreeV">The degree in the V direction</param>
        /// <param name="knotsU">The knot array in the U direction</param>
        /// <param name="knotsV">The knot array in the V direction</param>
        /// <param name="controlPoints">Two dimensional array of points</param>
        /// <param name="weights">Two dimensional array of weight values</param>
        /// <return>A new NurbsSurface</return>
        private NurbsSurface(int degreeU, int degreeV, Knot knotsU, Knot knotsV, List<List<Vector3>> controlPoints, List<List<double>>? weights = null)
        {

            if (controlPoints == null) throw new ArgumentNullException("Control points array connot be null!");
            if (degreeU < 1) throw new ArgumentException("DegreeU must be greater than 1!");
            if (degreeV < 1) throw new ArgumentException("DegreeV must be greater than 1!");
            if (knotsU == null) throw new ArgumentNullException("KnotU cannot be null!");
            if (knotsV == null) throw new ArgumentNullException("KnotV cannot be null!");
            if (knotsU.Count != controlPoints.Count + degreeU + 1)
                throw new ArgumentException("controlPointsU.length + degreeU + 1 must equal knotsU.length!");
            if (knotsV.Count != controlPoints[0].Count + degreeV + 1)
                throw new ArgumentException("controlPointsV.length + degreeV + 1 must equal knotsV.length!");
            if (!knotsU.AreValidKnots(degreeU, controlPoints.Count) || !knotsV.AreValidKnots(degreeV, controlPoints[0].Count))
                throw new ArgumentException("Invalid knot knots format!  Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            //return data;

            DegreeU = degreeU;
            DegreeV = degreeV;
            KnotsU = knotsU;
            KnotsV = knotsV;
            HomogenizedPoints = LinearAlgebra.Homogenize2d(controlPoints, weights);
            Weights = weights;
            DomainU = new Interval(this.KnotsU.First(), this.KnotsU.Last());
            DomainV = new Interval(this.KnotsV.First(), this.KnotsV.Last());
        }

        /// <summary>
        /// Creates a Nurbs surface object.
        /// </summary>
        /// <param name="controlPoints">Control points, as a 2d collection of Vector3.</param>
        /// <param name="degreeU">Surface degree u</param>
        /// <param name="degreeV">Surface degree v</param>
        public NurbsSurface(List<List<Vector3>> controlPoints, int degreeU, int degreeV)
            : this(degreeU, degreeV, new Knot(degreeU, controlPoints.Count), new Knot(degreeV, controlPoints[0].Count), controlPoints)
        {
        }

        /// <summary>
        /// Construct a NurbsSurface by a NurbsSurface object.
        /// </summary>
        /// <param name="curve">The curve object</param>
        public NurbsSurface(NurbsSurface surface)
        {
            DegreeU = surface.DegreeU;
            DegreeV = surface.DegreeV;
            HomogenizedPoints = new List<List<Vector3>>(surface.HomogenizedPoints);
            KnotsU = new Knot(surface.KnotsU);
            KnotsV = new Knot(surface.KnotsV);
            Weights = new List<List<double>>(surface.Weights!);
        }

        /// <summary>
        /// Construct a NurbsSurface from four perimeter points in counter-clockwise order
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">The third point</param>
        /// <param name="p4">The fourth point</param>
        public static NurbsSurface ByFourPoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int degree = 3)
        {
            var pts = new List<List<Vector3>>();
            for (int i = 0; i < degree + 1; i++)
            {
                var row = new List<Vector3>();
                for (int j = 0; j < degree + 1; j++)
                {
                    var l = (1.0d - i / degree);
                    var p1p2 = Vector3.Lerp(p1, p2, l);
                    var p4p3 = Vector3.Lerp(p4, p3, l);
                    var res = Vector3.Lerp(p1p2, p4p3, 1.0d - j / degree);
                    res.Add(1.0d);
                    row.Add(res);
                }
                pts.Add(row);
            }
            var zeros = Sets.RepeatData(0.0d, degree + 1);
            var ones = Sets.RepeatData(1.0d, degree + 1);
            Knot knotU = zeros.Concat(ones).ToKnot();
            Knot knotV = zeros.Concat(ones).ToKnot();

            return new NurbsSurface(degree, degree, knotU, knotV, pts);
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
        public Knot KnotsU { get; set; }
        /// <summary>
        /// List of non-decreasing knot values in v direction.
        /// </summary>
        public Knot KnotsV { get; set; }

        /// <summary>
        /// Determine the valid u domain of the surface.
        /// </summary>
        public Interval DomainU { get; set; }

        /// <summary>
        /// Determine the valid v domain of the surface.
        /// </summary>
        public Interval DomainV { get; set; }

        /// <summary>
        /// Obtain a copy of the NurbsSurface.
        /// </summary>
        /// <returns>The copied curve.</returns>
        public NurbsSurface Clone() => new NurbsSurface(this);


        /// <summary>
        /// 2d list of control points, the vertical direction (u) increases from top to bottom, the v direction from left to right,
        /// and where each control point is an list of length (dim).
        /// </summary>
        public List<List<Vector3>> ControlPoints => LinearAlgebra.Dehomogenize2d(HomogenizedPoints);
        /// <summary>
        ///Two dimensional array of weight values
        /// </summary>
        public List<List<double>> Weights { get; set; }
        public List<List<Vector3>> HomogenizedPoints { get; }

        public bool Equals(NurbsSurface other) => throw new NotImplementedException();

        public override NurbsSurface FromJson(string s) => throw new NotImplementedException();

        /// <summary>
        /// Serialize a nurbs surface to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
