using GShark.Core;
using GShark.Geometry.Enum;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GShark.Geometry
{
    /// <summary>
    /// This class represents a NURBS surface.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Data/NurbsSurfaceCollection.cs?name=example)]
    /// </example>
    public class NurbsSurface : IEquatable<NurbsSurface>, ITransformable<NurbsSurface>
    {
        /// <summary>
        /// Internal constructor used to validate the NURBS surface.
        /// </summary>
        /// <param name="degreeU">The degree in the U direction.</param>
        /// <param name="degreeV">The degree in the V direction.</param>
        /// <param name="knotsU">The knotVector in the U direction.</param>
        /// <param name="knotsV">The knotVector in the V direction.</param>
        /// <param name="controlPts">Two dimensional array of points.</param>
        internal NurbsSurface(int degreeU, int degreeV, KnotVector knotsU, KnotVector knotsV, List<List<Point4>> controlPts)
        {
            if (controlPts == null) throw new ArgumentNullException("Control points array connot be null!");
            if (degreeU < 1) throw new ArgumentException("DegreeU must be greater than 1!");
            if (degreeV < 1) throw new ArgumentException("DegreeV must be greater than 1!");
            if (knotsU == null) throw new ArgumentNullException("KnotU cannot be null!");
            if (knotsV == null) throw new ArgumentNullException("KnotV cannot be null!");
            if (knotsU.Count != controlPts.Count + degreeU + 1)
                throw new ArgumentException("Points count + degreeU + 1 must equal knotsU count!");
            if (knotsV.Count != controlPts[0].Count + degreeV + 1)
                throw new ArgumentException("Points count + degreeV + 1 must equal knotsV count!");
            if (!knotsU.IsValid(degreeU, controlPts.Count))
                throw new ArgumentException("Invalid knotsU!");
            if (!knotsV.IsValid(degreeV, controlPts[0].Count))
                throw new ArgumentException("Invalid knotsV!");

            DegreeU = degreeU;
            DegreeV = degreeV;
            KnotsU = (Math.Abs(knotsU.Domain - 1.0) > GeoSharkMath.Epsilon) ? knotsU.Normalize() : knotsU;
            KnotsV = (Math.Abs(knotsV.Domain - 1.0) > GeoSharkMath.Epsilon) ? knotsV.Normalize() : knotsV;
            Weights = Point4.GetWeights2d(controlPts);
            LocationPoints = Point4.PointDehomogenizer2d(controlPts);
            ControlPoints = controlPts;
            DomainU = new Interval(this.KnotsU.First(), this.KnotsU.Last());
            DomainV = new Interval(this.KnotsV.First(), this.KnotsV.Last());
        }

        /// <summary>
        /// The degree in U direction.
        /// </summary>
        internal int DegreeU { get; }

        /// <summary>
        /// The degree in V direction.
        /// </summary>
        internal int DegreeV { get; }

        /// <summary>
        /// The knotVector in U direction.
        /// </summary>
        internal KnotVector KnotsU { get; }

        /// <summary>
        /// The knotVector in V direction.
        /// </summary>
        internal KnotVector KnotsV { get; }

        /// <summary>
        /// The interval domain in U direction.
        /// </summary>
        internal Interval DomainU { get; }

        /// <summary>
        /// The interval domain in V direction.
        /// </summary>
        internal Interval DomainV { get; }

        /// <summary>
        /// A 2d collection of weight values.
        /// </summary>
        public List<List<double>> Weights { get; }

        /// <summary>
        /// A 2D collection of points, U direction increases from left to right, the V direction from bottom to top.
        /// </summary>
        public List<List<Point3>> LocationPoints { get; }

        /// <summary>
        /// A 2d collection of control points, U direction increases from left to right, the V direction from bottom to top.
        /// </summary>
        internal List<List<Point4>> ControlPoints { get; }

        /// <summary>
        /// Checks if a NURBS surface is closed.<br/>
        /// A surface is closed if the first points and the lasts in a direction are coincident.
        /// </summary>
        /// <returns>True if the curve is closed.</returns>
        // ToDo: Add test using lofts.
        public bool IsClosed(SurfaceDirection direction)
        {
            KnotVector knot = (direction == SurfaceDirection.U) ? KnotsU : KnotsV;
            int degree = (direction == SurfaceDirection.U) ? DegreeU : DegreeV;

            if (!knot.IsClamped(degree)) return knot.IsKnotVectorPeriodic(degree);

            var pts2d = (direction == SurfaceDirection.U) ? LocationPoints : Sets.Reverse2DMatrixData(LocationPoints);
            return pts2d.All(pts => pts[0].DistanceTo(pts.Last()) < GeoSharkMath.Epsilon);
        }

        /// <summary>
        /// Constructs a NURBS surface from four corners are expected in counter-clockwise order.<br/>
        /// The surface is defined with degree 1.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="p3">The third point.</param>
        /// <param name="p4">The fourth point.</param>
        public static NurbsSurface CreateFromCorners(Point3 p1, Point3 p2, Point3 p3, Point3 p4)
        {
            List<List<Point4>> pts = new List<List<Point4>>
            {
                new List<Point4>{p1, p4},
                new List<Point4>{p2, p3},
            };

            KnotVector knotU = new KnotVector { 0, 0, 1, 1 };
            KnotVector knotV = new KnotVector { 0, 0, 1, 1 };

            return new NurbsSurface(1, 1, knotU, knotV, pts);
        }

        /// <summary>
        /// Constructs a NURBS surface from a 2D grid of points.<br/>
        /// The grid of points should be organized as, U direction increases from bottom to top, the V direction from left to right.
        /// </summary>
        /// <param name="degreeU">Degree of surface in U direction.</param>
        /// <param name="degreeV">Degree of surface in V direction.</param>
        /// <param name="points">Points locations.</param>
        /// <param name="weight">A 2D collection of weights.</param>
        /// <returns>A NURBS surface.</returns>
        public static NurbsSurface CreateFromPoints(int degreeU, int degreeV, List<List<Point3>> points, List<List<double>> weight = null)
        {
            KnotVector knotU = new KnotVector(degreeU, points.Count);
            KnotVector knotV = new KnotVector(degreeV, points[0].Count);
            var controlPts = points.Select((pts, i) => pts.Select((pt, j) => weight != null ? new Point4(pt, weight[i][j]) : new Point4(pt)).ToList()).ToList();
            return new NurbsSurface(degreeU, degreeV, knotU, knotV, controlPts);
        }

        /// <summary>
        /// Constructs a NURBS surface from a set of NURBS curves.<br/>
        /// </summary>
        /// <param name="crvs">Set of a minimum of two curves to create the surface.</param>
        /// <param name="degreeV">Degree of surface in V direction.</param>
        /// <param name="loftType">Enum to choose the type of loft generation.</param>
        /// <returns>A NURBS surface.</returns>
        public static NurbsSurface CreateLoftedSurface(List<NurbsCurve> curves, int degreeV = 3, LoftType loftType = LoftType.Normal)
        {
            if (curves == null)
                throw new ArgumentException("An invalid number of curves to perform the loft.");

            if (degreeV > curves.Count - 1)
                throw new ArgumentException("The degreeV of the surface cannot be greater than the number of curves minus one.");

            if (curves.Any(x => x == null))
                throw new ArgumentException("The input set contains null curves.");

            if(curves.Count < 2)
                throw new ArgumentException("An invalid number of curves to perform the loft.");

            //Replace IsPerdiodic() with IsClosed() when the issue is solved
            bool isClosed = curves[0].IsPeriodic();
            foreach (NurbsCurve c in curves)
                if (isClosed != c.IsPeriodic())
                    throw new ArgumentException("Loft only works if all curves are open, or all curves are closed.");

            int degreeU = curves[0].Degree;
            KnotVector knotVectorU = curves[0].Knots;
            KnotVector knotVectorV = new KnotVector();
            List<List<Point4>> surfaceControlPoints = new List<List<Point4>>();

            switch (loftType)
            {
                case LoftType.Normal:
                    for (int n = 0; n < curves[0].LocationPoints.Count; n++)
                    {
                        List<Point3> pts = curves.Select(c => c.LocationPoints[n]).ToList();
                        NurbsCurve crv = Fitting.InterpolatedCurve(pts, degreeV);
                        surfaceControlPoints.Add(crv.ControlPoints);
                        knotVectorV = crv.Knots;
                    }
                    break;

                case LoftType.Loose:
                    surfaceControlPoints = Sets.Reverse2DMatrixData(curves.Select(c => c.ControlPoints).ToList());
                    knotVectorV = new KnotVector(degreeV, curves.Count);
                    break;
            }
            return new NurbsSurface(degreeU, degreeV, knotVectorU, knotVectorV, surfaceControlPoints);
        }

        /// <summary>
        /// Evaluates a point at a given U and V parameters.
        /// </summary>
        /// <param name="u">Evaluation U parameter.</param>
        /// <param name="v">Evaluation V parameter.</param>
        /// <returns>A evaluated point.</returns>
        public Point3 PointAt(double u, double v) => new Point3(Evaluation.SurfacePointAt(this, u, v));

        /// <summary>
        /// Computes the point on the surface that is closest to the test point.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The closest point on the surface.</returns>
        public Point3 ClosestPoint(Point3 point)
        {
            var (u, v) = Analyze.SurfaceClosestParameter(this, point);
            return new Point3(Evaluation.SurfacePointAt(this, u, v));
        }

        /// <summary>
        /// Computes the U and V parameters of the surface that is closest to the test point.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The U and V parameters of the surface that are closest to the test point.</returns>
        public (double U, double V) ClosestParameters(Point3 point) => Analyze.SurfaceClosestParameter(this, point);

        /// <summary>
        /// Evaluate the surface at the given U and V parameters.
        /// </summary>
        /// <param name="u">U parameter.</param>
        /// <param name="v">V parameter.</param>
        /// <param name="direction">The evaluate direction required as result.</param>
        /// <returns>The unitized tangent vector in the direction selected.</returns>
        public Vector3 EvaluateAt(double u, double v, EvaluateSurfaceDirection direction)
        {
            if (direction != EvaluateSurfaceDirection.Normal)
                return (direction == EvaluateSurfaceDirection.U)
                    ? Evaluation.RationalSurfaceDerivatives(this, u, v)[1, 0].Unitize()
                    : Evaluation.RationalSurfaceDerivatives(this, u, v)[0, 1].Unitize();

            Vector3[,] derivatives = Evaluation.RationalSurfaceDerivatives(this, u, v);
            Vector3 normal = Vector3.CrossProduct(derivatives[1, 0], derivatives[0, 1]);
            return normal.Unitize();
        }

        /// <summary>
        /// Transforms a NURBS surface with the given transformation matrix.
        /// </summary>
        /// <param name="transformation">The transformation matrix.</param>
        /// <returns>A new NURBS surface transformed.</returns>
        public NurbsSurface Transform(Transform transformation)
        {
            List<List<Point4>> transformedControlPts = ControlPoints;
            transformedControlPts.ForEach(pts => pts.ForEach(pt => pt.Transform(transformation)));
            return new NurbsSurface(DegreeU, DegreeV, KnotsU, KnotsV, transformedControlPts);
        }

        /// <summary>
        /// Compares if two NURBS surfaces are the same.<br/>
        /// Two NURBS curves are equal when the have same degrees, same control points order and dimension, and same knots.
        /// </summary>
        /// <param name="other">The NURBS surface.</param>
        /// <returns>Return true if the NURBS surface are equal.</returns>
        public bool Equals(NurbsSurface other)
        {
            List<List<Point3>> otherPts = other?.LocationPoints;

            if (other == null)
            {
                return false;
            }

            if (LocationPoints.Count != otherPts?.Count)
            {
                return false;
            }

            if (!LocationPoints.All(otherPts.Contains))
            {
                return false;
            }

            if (KnotsU.Count != other.KnotsU.Count || KnotsV.Count != other.KnotsU.Count)
            {
                return false;
            }

            return DegreeU == other.DegreeU && DegreeV == other.DegreeV && Weights.All(other.Weights.Contains);
        }

        /// <summary>
        /// Implements the override method to string.
        /// </summary>
        /// <returns>The representation of a NURBS surface in string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string controlPts = string.Join("\n", LocationPoints.Select(first => $"({string.Join(",", first)})"));
            string degreeU = $"DegreeU = {DegreeU}";
            string degreeV = $"DegreeV = {DegreeV}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(degreeU);
            stringBuilder.AppendLine(degreeV);

            return stringBuilder.ToString();
        }
    }
}