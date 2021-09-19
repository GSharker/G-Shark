using GShark.Core;
using GShark.Enumerations;
using GShark.ExtendedMethods;
using GShark.Interfaces;
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
            KnotsU = (Math.Abs(knotsU.GetDomain(degreeU).Length - 1.0) > GSharkMath.Epsilon) ? knotsU.Normalize() : knotsU;
            KnotsV = (Math.Abs(knotsV.GetDomain(degreeV).Length - 1.0) > GSharkMath.Epsilon) ? knotsV.Normalize() : knotsV;
            Weights = Point4.GetWeights2d(controlPts);
            ControlPointLocations = Point4.PointDehomogenizer2d(controlPts);
            ControlPoints = controlPts;
            DomainU = new Interval(KnotsU.First(), KnotsU.Last());
            DomainV = new Interval(KnotsV.First(), KnotsV.Last());
        }

        /// <summary>
        /// The degree in U direction.
        /// </summary>
        public int DegreeU { get; }

        /// <summary>
        /// The degree in V direction.
        /// </summary>
        public int DegreeV { get; }

        /// <summary>
        /// The knotVector in U direction.
        /// </summary>
        public KnotVector KnotsU { get; }

        /// <summary>
        /// The knotVector in V direction.
        /// </summary>
        public KnotVector KnotsV { get; }

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
        /// A 2D collection of points, V direction increases from left to right, the U direction from bottom to top.
        /// </summary>
        public List<List<Point3>> ControlPointLocations { get; }

        /// <summary>
        /// A 2d collection of control points, V direction increases from left to right, the U direction from bottom to top.
        /// </summary>
        public List<List<Point4>> ControlPoints { get; }

        /// <summary>
        /// Checks if a NURBS surface is closed.<br/>
        /// A surface is closed if the first points and the lasts in a direction are coincident.
        /// </summary>
        /// <returns>True if the curve is closed.</returns>
        public bool IsClosed(SurfaceDirection direction)
        {
            var pts2d = (direction == SurfaceDirection.U) ? CollectionHelpers.Transpose2DArray(ControlPointLocations) : ControlPointLocations;
            return pts2d.All(pts => pts[0].DistanceTo(pts.Last()) < GSharkMath.Epsilon);
        }

        /// <summary>
        /// Constructs a NURBS surface from four corners.<br/>
        /// If the corners are ordered ccw the normal of the surface will point up otherwise, if corners ordered cw the normal will point down.<br/>
        /// The surface is defined of degree 1.
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
        /// The grid of points should be organized as, the V direction from left to right and the U direction increases from bottom to top.
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
        /// <param name="curves">Set of a minimum of two curves to create the surface.</param>
        /// <param name="loftType">Enum to choose the type of loft generation.</param>
        /// <returns>A NURBS surface.</returns>
        public static NurbsSurface CreateLoftedSurface(IList<NurbsBase> curves, LoftType loftType = LoftType.Normal)
        {
            if (curves == null)
                throw new ArgumentException("An invalid number of curves to perform the loft.");

            if (curves.Count < 2)
                throw new ArgumentException("An invalid number of curves to perform the loft.");

            if (curves.Any(x => x == null))
                throw new ArgumentException("The input set contains null curves.");

            bool isClosed = curves[0].IsClosed;
            foreach (NurbsBase c in curves.Skip(1))
                if (isClosed != c.IsClosed)
                    throw new ArgumentException("Loft only works if all curves are open, or all curves are closed.");

            // Copy curves for possible operation of homogenization.
            IList<NurbsBase> copyCurves = new List<NurbsBase>(curves);

            // Clamp curves if periodic.
            if (copyCurves[0].IsPeriodic)
            {
                for (int i = 0; i < copyCurves.Count; i++)
                {
                    copyCurves[i] = copyCurves[i].ClampEnds();
                }
            }

            // If necessary, the curves can be brought to a common degree and knots, as we do for the ruled surface.
            // In fact, the ruled surface is a special case of a skinned surface.
            if (copyCurves.Any(c => c.Degree != copyCurves[0].Degree))
            {
                copyCurves = CurveHelpers.NormalizedDegree(copyCurves);
                copyCurves = CurveHelpers.NormalizedKnots(copyCurves);
            }

            int degreeV = copyCurves[0].Degree;
            int degreeU = 3;
            KnotVector knotVectorU = new KnotVector();
            KnotVector knotVectorV = copyCurves[0].Knots;
            List<List<Point4>> surfaceControlPoints = new List<List<Point4>>();

            switch (loftType)
            {
                case LoftType.Normal:
                    List<List<Point4>> tempPts = new List<List<Point4>>();
                    for (int n = 0; n < copyCurves[0].ControlPointLocations.Count; n++)
                    {
                        List<Point3> pts = copyCurves.Select(c => c.ControlPointLocations[n]).ToList();
                        NurbsBase crv = Fitting.Curve.Interpolated(pts, degreeU);
                        tempPts.Add(crv.ControlPoints);
                        knotVectorU = crv.Knots;
                    }
                    surfaceControlPoints = CollectionHelpers.Transpose2DArray(tempPts);
                    break;

                case LoftType.Loose:
                    surfaceControlPoints = copyCurves.Select(c => c.ControlPoints).ToList();
                    knotVectorU = new KnotVector(degreeU, copyCurves.Count);
                    break;
            }
            return new NurbsSurface(degreeU, degreeV, knotVectorU, knotVectorV, surfaceControlPoints);
        }

        /// <summary>
        /// Constructs a ruled surface between two curves.
        /// <em>Follows the algorithm at page 337 of The NURBS Book by Piegl and Tiller.</em>
        /// </summary>
        /// <param name="curveA">The first curve.</param>
        /// <param name="curveB">The second curve.</param>
        /// <returns>A ruled surface.</returns>
        public static NurbsSurface CreateRuledSurface(NurbsBase curveA, NurbsBase curveB)
        {
            IList<NurbsBase> curves = new[] { curveA, curveB };
            curves = CurveHelpers.NormalizedDegree(curves);
            curves = CurveHelpers.NormalizedKnots(curves);

            return new NurbsSurface(1, curves[0].Degree, new KnotVector(1, 2), curves[0].Knots,
                new List<List<Point4>> { curves[0].ControlPoints, curves[1].ControlPoints });
        }

        /// <summary>
        /// Creates a surface of revolution through an arbitrary angle, and axis.
        /// </summary>
        /// <param name="curveProfile">Profile curve.</param>
        /// <param name="axis">Revolution axis.</param>
        /// <param name="rotationAngle">Angle in radiance.</param>
        /// <returns>The revolution surface.</returns>
        public static NurbsSurface CreateRevolvedSurface(NurbsBase curveProfile, Line axis, double rotationAngle)
        {
            // if angle is less than 90.
            int arcCount = 1;
            KnotVector knotsU = Vector.Zero1d(6).ToKnot();

            if (rotationAngle <= Math.PI && rotationAngle > Math.PI / 2)
            {
                arcCount = 2;
                knotsU[3] = knotsU[4] = 0.5;
            }

            if (rotationAngle <= 3 * Math.PI / 2 && rotationAngle > Math.PI)
            {
                arcCount = 3;
                knotsU = Vector.Zero1d(6 + 2 * (arcCount - 1)).ToKnot();
                knotsU[3] = knotsU[4] = (double)1 / 3;
                knotsU[5] = knotsU[6] = (double)2 / 3;
            }

            if (rotationAngle <= 4 * Math.PI && rotationAngle > 3 * Math.PI / 2)
            {
                arcCount = 4;
                knotsU = Vector.Zero1d(6 + 2 * (arcCount - 1)).ToKnot();
                knotsU[3] = knotsU[4] = (double)1 / 4;
                knotsU[5] = knotsU[6] = (double)1 / 2;
                knotsU[7] = knotsU[8] = (double)3 / 4;
            }

            // load start and end knots.
            int t = 3 + 2 * (arcCount - 1);
            for (int i = 0; i < 3; i++, t++)
            {
                knotsU[i] = 0.0;
                knotsU[t] = 1.0;
            }

            // some initialization.
            double divideAngle = rotationAngle / arcCount;
            int n = 2 * arcCount;
            double wm = divideAngle / 2; // is the base angle.

            // initialize the sines and cosines only once.
            double angle = 0.0;
            double[] sines = new double[arcCount + 1];
            double[] cosines = new double[arcCount + 1];
            for (int i = 1; i <= arcCount; i++)
            {
                angle += divideAngle;
                sines[i] = Math.Sin(angle);
                cosines[i] = Math.Cos(angle);
            }

            // loop and compute each u row of control points and weights.
            List<List<Point4>> controlPts = new List<List<Point4>>();
            for (int r = 0; r < 2 * arcCount + 1; r++)
            {
                List<Point4> temp = CollectionHelpers.RepeatData(Point4.Zero, curveProfile.ControlPoints.Count);
                controlPts.Add(temp);
            }

            for (int j = 0; j < curveProfile.ControlPointLocations.Count; j++)
            {
                Point3 ptO = axis.ClosestPoint(curveProfile.ControlPointLocations[j]);
                Vector3 vectorX = curveProfile.ControlPointLocations[j] - ptO;
                double radius = vectorX.Length; // the radius at that length.
                Vector3 vectorY = Vector3.CrossProduct(axis.Direction, vectorX);

                if (radius > GSharkMath.Epsilon)
                {
                    vectorX *= (1 / radius);
                    vectorY *= (1 / radius);
                }

                // initialize the first control points and weights.
                Point3 pt0 = curveProfile.ControlPointLocations[j];
                controlPts[0][j] = new Point4(pt0, curveProfile.Weights[j]);

                Vector3 tangent0 = vectorY;
                int index = 0;

                for (int i = 1; i <= arcCount; i++)
                {
                    // rotated generatrix point.
                    Point3 pt2 = (radius == 0.0)
                        ? ptO
                        : ptO + (vectorX * (cosines[i] * radius) + vectorY * (sines[i] * radius));

                    controlPts[index + 2][j] = new Point4(pt2, curveProfile.Weights[j]);

                    // construct the vector tangent to the rotation.
                    Vector3 rotationTangent = vectorX * (-1 * sines[i]) + vectorY * cosines[i];

                    // construct the next control point.
                    if (radius == 0.0)
                    {
                        controlPts[index + 1][j] = ptO;
                        continue;
                    }

                    Line ln0 = new Line(pt0, tangent0, tangent0.Length);
                    Line ln1 = new Line(pt2, rotationTangent, rotationTangent.Length);
                    Intersection.Intersect.LineLine(ln0, ln1, out Point3 intersectionPt, out _, out _, out _);
                    controlPts[index + 1][j] = new Point4(intersectionPt, wm * curveProfile.Weights[j]);

                    index += 2;
                    if (i >= arcCount) continue;
                    pt0 = pt2;
                    tangent0 = rotationTangent;
                }

            }
            
            return new NurbsSurface(2, curveProfile.Degree, knotsU, curveProfile.Knots, controlPts.Select(pts => pts.ToList()).ToList());
        }

        /// <summary>
        /// Evaluates a point at a given U and V parameters.
        /// </summary>
        /// <param name="u">Evaluation U parameter.</param>
        /// <param name="v">Evaluation V parameter.</param>
        /// <returns>A evaluated point.</returns>
        public Point3 PointAt(double u, double v) => new Point3(Evaluate.Surface.PointAt(this, u, v));

        /// <summary>
        /// Computes the point on the surface that is closest to the test point.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The closest point on the surface.</returns>
        public Point3 ClosestPoint(Point3 point)
        {
            var (u, v) = Analyze.Surface.ClosestParameter(this, point);
            return new Point3(Evaluate.Surface.PointAt(this, u, v));
        }

        /// <summary>
        /// Computes the U and V parameters of the surface that is closest to the test point.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The U and V parameters of the surface that are closest to the test point.</returns>
        public (double U, double V) ClosestParameter(Point3 point)
        {
            return Analyze.Surface.ClosestParameter(this, point);
        }

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
                    ? Evaluate.Surface.RationalDerivatives(this, u, v)[1, 0].Unitize()
                    : Evaluate.Surface.RationalDerivatives(this, u, v)[0, 1].Unitize();

            Vector3[,] derivatives = Evaluate.Surface.RationalDerivatives(this, u, v);
            Vector3 normal = Vector3.CrossProduct(derivatives[1, 0], derivatives[0, 1]);
            return normal.Unitize();
        }

        /// <summary>
        /// Splits (divides) the surface into two parts at the specified parameter
        /// </summary>
        /// <param name="parameter">The parameter at which to split the surface, parameter should be between 0 and 1.</param>
        /// <param name="direction">Where to split in the U or V direction of the surface.</param>
        /// <returns>If the surface is split vertically (U direction) the left side is returned as the first surface and the right side is returned as the second surface.<br/>
        /// If the surface is split horizontally (V direction) the bottom side is returned as the first surface and the top side is returned as the second surface.</returns>
        public NurbsSurface[] SplitAt(double parameter, SplitDirection direction)
        {
            if (parameter < 0.0 || parameter > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameter), "The parameter is not into the domain 0.0 to 1.0.");
            }

            return Sampling.Surface.Split(this, parameter, direction);
        }

        /// <summary>
        /// Extracts the isoparametric curves (isocurves) at the given parameter and surface direction.
        /// </summary>
        /// <param name="parameter">The parameter between 0.0 to 1.0 whether the isocurve will be extracted.</param>
        /// <param name="direction">The U or V direction whether the isocurve will be extracted.</param>
        /// <returns>The isocurve extracted.</returns>
        public NurbsCurve IsoCurve(double parameter, SurfaceDirection direction)
        {
            return Analyze.Surface.Isocurve(this, parameter, direction);
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
            if (other == null)
            {
                return false;
            }

            if (ControlPointLocations.Count != other.ControlPointLocations.Count)
            {
                return false;
            }

            if (ControlPointLocations.Where((pt, i) => !pt.SequenceEqual(other.ControlPointLocations[i])).Any())
            {
                return false;
            }

            if (KnotsU.Count != other.KnotsU.Count || KnotsV.Count != other.KnotsV.Count)
            {
                return false;
            }

            if (Weights.Where((w, i) => !w.SequenceEqual(other.Weights[i])).Any())
            {
                return false;
            }

            return DegreeU == other.DegreeU && DegreeV == other.DegreeV;
        }

        /// <summary>
        /// Implements the override method to string.
        /// </summary>
        /// <returns>The representation of a NURBS surface in string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string controlPts = string.Join("\n", ControlPointLocations.Select(first => $"({string.Join(",", first)})"));
            string degreeU = $"DegreeU = {DegreeU}";
            string degreeV = $"DegreeV = {DegreeV}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(degreeU);
            stringBuilder.AppendLine(degreeV);

            return stringBuilder.ToString();
        }
    }
}