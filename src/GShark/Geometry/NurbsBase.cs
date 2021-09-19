#nullable enable
using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Interfaces;
using GShark.Operation;
using GShark.Operation.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using GShark.Optimization;

namespace GShark.Geometry
{
    /// <summary>
    /// This class represents a base class that is common to most curve types.
    /// </summary>
    public abstract class NurbsBase : IEquatable<NurbsBase>
    {
        protected NurbsBase(int degree, KnotVector knots, List<Point4> controlPoints)
        {
            if (controlPoints is null)
            {
                throw new ArgumentNullException(nameof(controlPoints));
            }

            if (knots is null)
            {
                throw new ArgumentNullException(nameof(knots));
            }

            if (degree < 1)
            {
                throw new ArgumentException("Degree must be greater than 1!");
            }

            if (knots.Count != controlPoints.Count + degree + 1)
            {
                throw new ArgumentException("Number of controlPoints + degree + 1 must equal knots length!");
            }

            if (!knots.IsValid(degree, controlPoints.Count))
            {
                throw new ArgumentException("Invalid knot format! Should begin with degree + 1 repeats and end with degree + 1 repeats!");
            }

            Weights = Point4.GetWeights(controlPoints);
            Degree = degree;
            Knots = knots;
            ControlPointLocations = Point4.PointDehomogenizer1d(controlPoints);
            ControlPoints = controlPoints;
        }

        /// <summary>
        /// Gets the list of weight values.
        /// </summary>
        public List<double> Weights { get; }

        /// <summary>
        /// Gets the degree of the curve.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// Gets the control points in their rational form. 
        /// </summary>
        public List<Point3> ControlPointLocations { get; }

        /// <summary>
        /// Gets the control points in their homogenized form.
        /// </summary>
        public List<Point4> ControlPoints { get; }

        /// <summary>
        /// Gets the knots vectors of the curve.
        /// </summary>
        public KnotVector Knots { get; }

        public double Length => Analyze.CurveLength(this);

        public Point3 StartPoint => PointAt(0.0);

        public Point3 MidPoint => PointAt(0.5);

        public Point3 EndPoint => PointAt(1.0);

        public BoundingBox GetBoundingBox()
        {
            NurbsBase curve = this;

            if (IsPeriodic())
            {
                curve = ClampEnds();
            }

            List<Point3> pts = new List<Point3> { curve.ControlPointLocations[0] };
            List<NurbsBase> beziers = Modify.DecomposeCurveIntoBeziers(curve, true);
            foreach (NurbsBase crv in beziers)
            {
                Extrema e = Evaluation.ComputeExtrema(crv);
                pts.AddRange(e.Values.Select(eValue => crv.PointAt(eValue)));
            }

            pts.Add(curve.ControlPointLocations[curve.ControlPointLocations.Count - 1]);
            Point3[] removedDuplicate = Point3.CullDuplicates(pts, GSharkMath.MinTolerance);
            return new BoundingBox(removedDuplicate);
        }

        /// <summary>
        /// Checks if a curve is closed.<br/>
        /// A curve is closed if the first point and the last are the same.
        /// </summary>
        /// <returns>True if the curve is closed.</returns>
        public bool IsClosed()
        {
            Point3 pt0 = Evaluation.CurvePointAt(this, 0.0);
            Point3 pt1 = Evaluation.CurvePointAt(this, 1.0);
            return pt0.EpsilonEquals(pt1, GSharkMath.Epsilon);
        }

        /// <summary>
        /// Checks if a curve is periodic.<br/>
        /// A curve is periodic, where the number of overlapping points is equal the curve degree.
        /// </summary>
        /// <returns>True if the curve is periodic.</returns>
        public bool IsPeriodic()
        {
            if (!Knots.IsPeriodic(Degree)) return false;
            int i, j;
            for (i = 0, j = ControlPointLocations.Count - Degree; i < Degree; i++, j++)
            {
                if (ControlPointLocations[i].DistanceTo(ControlPointLocations[j]) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a periodic curve.<br/>
        /// This method uses the control point wrapping solution.
        /// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/B-spline/bspline-curve-closed.html
        /// </summary>
        /// <returns>A periodic curve.</returns>
        public NurbsBase Close()
        {
            // Wrapping control points
            List<Point4> pts = new List<Point4>(ControlPoints);
            for (int i = 0; i < Degree; i++)
            {
                pts.Add(pts[i]);
            }

            KnotVector knots = KnotVector.UniformPeriodic(Degree, pts.Count);
            return new NurbsCurve(Degree, knots, pts);
        }

        /// <summary>
        /// <inheritdoc cref="ICurve.PointAt"/>
        /// </summary>
        public Point3 PointAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }
            return Evaluation.CurvePointAt(this, t);
        }

        /// <summary>
        /// <inheritdoc cref="ICurve.PointAtLength"/>
        /// </summary>
        public Point3 PointAtLength(double length)
        {
            double parameter = Analyze.CurveParameterAtLength(this, length);
            return Evaluation.CurvePointAt(this, parameter);
        }

        /// <summary>
        /// Evaluates a point at the normalized length.
        /// </summary>
        /// <param name="normalizedLength">The length factor is normalized between 0.0 and 1.0.</param>
        /// <returns>The point at the length.</returns>
        public Point3 PointAtNormalizedLength(double normalizedLength)
        {
            double length = GSharkMath.RemapValue(normalizedLength, new Interval(0.0, 1.0), new Interval(0.0, Length));
            return PointAtLength(length);
        }

        /// <summary>
        /// Evaluates a point at a given chord length from a parameter on the curve.
        /// </summary>
        /// <returns></returns>
        public double ParameterAtChordLength(double t, double chordLength)
        {
            IObjectiveFunction objectiveFunction = new ChordLengthObjective(this, t, chordLength);
            Minimizer min = new Minimizer(objectiveFunction);
            var lengthAtPrevious = LengthAt(t);
            var initialGuess = ParameterAtLength(lengthAtPrevious + chordLength);
            MinimizationResult solution = min.UnconstrainedMinimizer(new Vector { initialGuess , initialGuess });

            return solution.SolutionPoint[0];
        }

        /// <summary>
        /// Divides a curve by a given chord length. Last chord will be of whatever length is left at the end of the curve.
        /// </summary>
        /// <param name="chordLength">Desired chord length.</param>
        /// <returns>Collection of curve parameters along the curve.</returns>
        public List<double> DivideByChordLength(double chordLength)
        {
            if (chordLength <= 0)
            {
                throw new ArgumentException("Chord length must be greater than 0.");
            }

            var t = 0.0;
            var length = 0.0;
            var resultParams = new List<double>();

            while(length + chordLength < Length)
            {
                var parmAtChordLength = ParameterAtChordLength(t, chordLength);
                resultParams.Add(parmAtChordLength);
                t = parmAtChordLength;
                length += chordLength;
            }

            return resultParams;
        }

        /// <summary>
        /// Computes the curve tangent at the given parameter.
        /// </summary>
        /// <param name="t">The parameter to sample the curve. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The unitized tangent vector at the given parameter.</returns>
        public Vector3 TangentAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            return Evaluation.RationalCurveTangent(this, t).Unitize();
        }

        /// <summary>
        /// Determines the derivatives of a curve at a given parameter.<br/>
        /// </summary>
        /// <param name="t">Parameter on the curve at which the point is to be evaluated. Parameter should be between 0.0 and 1.0.</param>
        /// <param name="numberOfDerivatives">The number of derivatives required.</param>
        /// <returns>The derivatives.</returns>
        public List<Vector3> DerivativeAt(double t, int numberOfDerivatives = 1)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            return Evaluation.RationalCurveDerivatives(this, t, numberOfDerivatives);
        }

        /// <summary>
        /// Computes the parameter values of all local extrema.
        /// </summary>
        /// <returns>The parameter values of all the local extrema.</returns>
        public IReadOnlyList<double> Extrema()
        {
            Extrema extremaResult = Evaluation.ComputeExtrema(this);
            return extremaResult.Values.ToList();
        }

        /// <summary>
        /// Computes the curvature vector of the curve at the parameter.
        /// The vector has length equal to the radius of the curvature circle and with direction to the center of the circle.
        /// </summary>
        /// <param name="t">Evaluation parameter. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The curvature vector.</returns>
        public Vector3 CurvatureAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            List<Vector3> derivatives = Evaluation.RationalCurveDerivatives(this, t, 2);
            return Analyze.Curvature(derivatives[1], derivatives[2]);
        }

        /// <summary>
        /// Calculates the 3D plane at the given parameter.
        /// Defined as the Frenet frame, is constructed from the velocity and the acceleration of the curve.
        /// https://janakiev.com/blog/framing-parametric-curves/
        /// </summary>
        /// <param name="t">Evaluation parameter. Parameter should be between 0.0 and 1.0.</param>
        /// <returns>The perpendicular frame.</returns>
        public Plane PerpendicularFrameAt(double t)
        {
            if (t <= 0.0)
            {
                t = 0.0;
            }

            if (t >= 1.0)
            {
                t = 1.0;
            }

            List<Vector3> derivatives = Evaluation.RationalCurveDerivatives(this, t, 2);

            Vector3 normal = (derivatives[2].Length == 0.0)
                ? Vector3.PerpendicularTo(derivatives[1])
                : Analyze.Curvature(derivatives[1], derivatives[2]);

            Vector3 yDir = Vector3.CrossProduct(derivatives[1], normal);
            return new Plane(derivatives[0], normal, yDir);
        }

        /// <summary>
        /// Reverses the parametrization of the curve.
        /// </summary>
        /// <returns>A reversed curve.</returns>
        public NurbsBase Reverse()
        {
            return (NurbsBase)Modify.ReverseCurve(this);
        }

        /// <summary>
        /// <inheritdoc cref="ICurve.ClosestPoint"/>
        /// </summary>
        public Point3 ClosestPoint(Point3 point)
        {
            return Point4.PointDehomogenizer(Analyze.CurveClosestPoint(this, point, out _));
        }

        /// <summary>
        /// <inheritdoc cref="ICurve.ClosestParameter"/>
        /// </summary>
        public double ClosestParameter(Point3 pt)
        {
            return Analyze.CurveClosestParameter(this, pt);
        }

        /// <summary>
        /// Computes the parameter along the curve which coincides with a given length.
        /// </summary>
        /// <param name="segmentLength">Length of segment to measure. Must be less than or equal to the length of the curve.</param>
        /// <returns>The parameter on the curve at the given length.</returns>
        public double ParameterAtLength(double segmentLength)
        {
            if (segmentLength <= 0.0)
            {
                return 0.0;
            }

            if (segmentLength >= Length)
            {
                return 1.0;
            }

            return Analyze.CurveParameterAtLength(this, segmentLength);
        }

        /// <summary>
        /// <inheritdoc cref="ICurve.LengthAt"/>
        /// </summary>
        public double LengthAt(double t)
        {
            if (t <= 0.0)
            {
                return 0.0;
            }

            if (t >= 1.0)
            {
                return Length;
            }

            return Analyze.CurveLength(this, t);
        }

        /// <summary>
        /// Converts a curve where the knotVector is clamped.
        /// </summary>
        /// <returns>A curve with clamped knots.</returns>
        public NurbsBase ClampEnds()
        {
            List<Point4> evalPts = new List<Point4>(ControlPoints);
            KnotVector clampedKnots = new KnotVector(Knots);
            int j = 2;

            while (j-- > 0)
            {
                Evaluation.DeBoor(ref evalPts, clampedKnots, Degree, clampedKnots[Degree]);
                for (int i = 0; i < Degree; i++)
                {
                    clampedKnots[i] = clampedKnots[Degree];
                }
                evalPts.Reverse();
                clampedKnots.Reverse();
            }

            return new NurbsCurve(Degree, clampedKnots, evalPts);
        }

        /// <summary>
        /// Computes the offset of the curve.
        /// </summary>
        /// <param name="distance">The distance of the offset. If negative the offset will be in the opposite side.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset curve.</returns>
        public NurbsBase Offset(double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return this;
            }

            var (tValues, pts) = Sampling.Curve.AdaptiveSample(this);

            List<Point3> offsetPts = new List<Point3>();
            for (int i = 0; i < pts.Count; i++)
            {
                Vector3 tangent = Evaluation.RationalCurveTangent(this, tValues[i]);
                Vector3 vecOffset = Vector3.CrossProduct(tangent, pln.ZAxis).Amplify(distance);
                offsetPts.Add(pts[i] + vecOffset);
            }

            return Fitting.Curve.Interpolated(offsetPts, Degree);
        }

        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="numberOfSegments">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        public (List<Point3> Points, List<double> Parameters) Divide(int numberOfSegments)
        {
            if (numberOfSegments < 2)
            {
                throw new ArgumentException("Number of segments must be greater than 1.", nameof(numberOfSegments));
            }

            var divideResult = Sampling.Curve.ByCount(this, numberOfSegments);
            var points = divideResult.Select(PointAt).ToList();
            return (points, divideResult);
        }

        /// <summary>
        /// Divides a curve for a given max segment length, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="maxSegmentLength">The maximum length the segments have to be split in.</param>
        /// <param name="equalSegmentLengths">Force to have all the segments of the same lengths.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        public (List<Point3> Points, List<double> Parameters) Divide(double maxSegmentLength, bool equalSegmentLengths = false)
        {
            if (maxSegmentLength <= 0)
            {
                throw new ArgumentException("Segment length must be greater than 0.", nameof(maxSegmentLength));
            }

            var len = maxSegmentLength;
            if (equalSegmentLengths)
            {
                List<NurbsBase> curves = Modify.DecomposeCurveIntoBeziers(this);
                List<double> curveLengths = curves.Select(curve => Analyze.BezierCurveLength(curve)).ToList();
                double totalLength = curveLengths.Sum();

                len = totalLength / Math.Ceiling(totalLength / maxSegmentLength);
            }

            var (tValues, lengths) = Sampling.Curve.ByLength(this, len);
            var points = tValues.Select(PointAt).ToList();

            return (points, tValues);
        }

        /// <summary>
        /// Creates rotation minimized perpendicular frames (RMF) at given t parameters along the curve.<br/>
        /// Double reflection method taken from Wang, W., J¨uttler, B., Zheng, D., and Liu, Y. 2008. "Computation of rotation minimizing frame."<br/>
        /// https://www.microsoft.com/en-us/research/wp-content/uploads/2016/12/Computation-of-rotation-minimizing-frames.pdf
        /// </summary>
        ///<param name="uValues">The curve parameter values to locate perpendicular curve frames</param>
        /// <returns>A collection of planes.</returns>
        public List<Plane> PerpendicularFrames(List<double> uValues)
        {
            var pointsOnCurve = uValues.Select(PointAt).ToList(); //get points at t values
            var pointsOnCurveTan = uValues.Select(t => Evaluation.RationalCurveTangent(this, t)).ToList(); //get tangents at t values
            var firstParameter = uValues[0]; //get first t value

            //Create initial frame at first parameter
            var origin = PointAt(firstParameter);
            var crvTan = Evaluation.RationalCurveTangent(this, firstParameter);
            var crvNormal = Vector3.PerpendicularTo(crvTan);
            var yAxis = Vector3.CrossProduct(crvTan, crvNormal);
            var xAxis = Vector3.CrossProduct(yAxis, crvTan);

            //Set initial frame
            Plane[] perpFrames = new Plane[pointsOnCurve.Count];
            perpFrames[0] = new Plane(origin, xAxis, yAxis);

            //Given boundary data(x0, t0; x1, t1) and an initial right-handed
            //orthonormal frame U0 = (r0, s0, t0) at x0, the next frame U1 = (r1, s1, t1)
            //at x1 for RMF approximation is computed by the double reflection method in
            //the following two steps.
            //
            //Step 1.Let R1 denote the reflection in the bisecting plane of the points x0
            //and x1(see Figure 4).Use R1 to map U0 to a left - handed orthonormal frame
            //UL0 = (rL0, sL0, tL0).
            //
            //Step 2.Let R2 denote the reflection in the bisecting plane of the points x1 + tL
            //0 and x1 +t1. Use R2 to map UL0 to a right - handed orthonormal frame U1 = (r1, s1, t1).
            //Output U1.

            for (int i = 0; i < pointsOnCurve.Count - 1; i++)
            {
                Vector3 v1 = pointsOnCurve[i + 1] - pointsOnCurve[i]; //compute reflection vector of R1
                double c1 = v1 * v1;
                Vector3 rLi = perpFrames[i].XAxis - (2 / c1) * (v1 * perpFrames[i].XAxis) * v1; //compute reflected rL vector by R1
                Vector3 tLi = pointsOnCurveTan[i] - (2 / c1) * (v1 * pointsOnCurveTan[i]) * v1; //compute reflected tL vector by R1
                Vector3 v2 = pointsOnCurveTan[i + 1] - tLi; //compute reflection vector of R2
                double c2 = v2 * v2;
                Vector3 rNext = rLi - (2 / c2) * (v2 * rLi) * v2; //compute reflected r vector by R2
                var sNext = Vector3.CrossProduct(pointsOnCurveTan[i + 1], rNext); //compute vector s[i+1] of next frame

                //create output frame
                var frameNext = new Plane { Origin = pointsOnCurve[i + 1], XAxis = rNext, YAxis = sNext };
                perpFrames[i + 1] = frameNext; //output frame
            }

            return perpFrames.ToList();
        }

        /// <summary>
        /// Splits a curve into two parts at a given parameter.
        /// </summary>
        /// <param name="t">The parameter at which to split the curve.</param>
        /// <returns>Two curves.</returns>
        public List<NurbsBase> SplitAt(double t)
        {
            int degree = Degree;

            List<double> knotsToInsert = CollectionHelpers.RepeatData(t, degree + 1);

            NurbsBase refinedCurve = Modify.CurveKnotRefine(this, knotsToInsert);

            int s = Knots.Span(degree, t);

            KnotVector knots0 = refinedCurve.Knots.ToList().GetRange(0, s + degree + 2).ToKnot();
            KnotVector knots1 = refinedCurve.Knots.GetRange(s + 1, refinedCurve.Knots.Count - (s + 1)).ToKnot();

            List<Point4> controlPoints0 = refinedCurve.ControlPoints.GetRange(0, s + 1);
            List<Point4> controlPoints1 = refinedCurve.ControlPoints.GetRange(s + 1, refinedCurve.ControlPointLocations.Count - (s + 1));

            return new List<NurbsBase> { new NurbsCurve(degree, knots0, controlPoints0), new NurbsCurve(degree, knots1, controlPoints1) };
        }

        /// <summary>
        /// Splits a curve at given parameters and returns the segments as curves.
        /// </summary>
        /// <param name="parameters">The parameters at which to split the curve. Values should be between 0.0 and 1.0.</param>
        /// <returns>Collection of curve segments.</returns>
        public List<NurbsBase> SplitAt(double[] parameters)
        {
            var curves = new List<NurbsBase>();
            if (parameters.Length == 0)
            {
                curves.Add(this);
                return curves;
            }

            var sortedParameters = parameters.OrderBy(x => x).ToArray();
            Interval curveDomain = Knots.GetDomain(this.Degree);

            if (Math.Abs(sortedParameters[0] - curveDomain.T0) > GSharkMath.MaxTolerance)
            {
                var tempParams = new double[sortedParameters.Length + 1];
                tempParams[0] = curveDomain.T0;
                for (var i = 0; i < sortedParameters.Length; i++)
                {
                    tempParams[i + 1] = sortedParameters[i];
                }
                sortedParameters = tempParams;
            }

            if (Math.Abs(sortedParameters[sortedParameters.Length - 1] - curveDomain.T1) > GSharkMath.MaxTolerance)
            {
                Array.Resize(ref sortedParameters, sortedParameters.Length + 1);
                sortedParameters[sortedParameters.Length - 1] = curveDomain.T1;
            }

            for (int i = 0; i < sortedParameters.Length - 1; i++)
            {
                curves.Add(SubCurve(new Interval(sortedParameters[i], sortedParameters[i + 1])));
            }

            return curves;
        }

        /// <summary>
        /// Extract sub-curve defined by domain.
        /// </summary>
        /// <param name="domain">Domain of sub-curve</param>
        /// <returns>The sub curve.</returns>
        public NurbsBase SubCurve(Interval domain)
        {
            int degree = Degree;
            int order = degree + 1;
            Interval subCurveDomain = domain;

            //NOTE: Handling decreasing domain by flipping it to maintain direction of original curve in sub-curve. Is this what we want?
            if (domain.IsDecreasing)
            {
                subCurveDomain = new Interval(domain.T1, domain.T0);
            }

            var isT0AtStart = Math.Abs(subCurveDomain.T0 - Knots[0]) < GSharkMath.MaxTolerance;
            var isT1AtEnd = Math.Abs(subCurveDomain.T1 - Knots[Knots.Count - 1]) < GSharkMath.MaxTolerance;

            if (isT0AtStart && isT1AtEnd)
            {
                return this;
            }

            if (isT0AtStart || isT1AtEnd)
            {
                return isT0AtStart ? SplitAt(subCurveDomain.T1)[0] : SplitAt(subCurveDomain.T0)[1];
            }

            List<double> knotsToInsert = CollectionHelpers.RepeatData(domain.T0, order).Concat(CollectionHelpers.RepeatData(domain.T1, degree + 1)).ToList();
            NurbsBase refinedCurve = Modify.CurveKnotRefine(this, knotsToInsert);

            var subCurveControlPoints = refinedCurve.ControlPoints.GetRange(order, order);
            var subCrvCrtlPtsLocations = subCurveControlPoints.Select(Point4.PointDehomogenizer).ToList();

            var subCurve = new NurbsCurve(subCrvCrtlPtsLocations, Degree);

            return subCurve;
        }

        /// <summary>
        /// Implements the override method to string.
        /// </summary>
        /// <returns>The representation of a curve in string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string controlPts = string.Join("\n", ControlPointLocations.Select(first => $"({string.Join(",", first)})"));
            string knots = $"Knots = ({string.Join(",", Knots)})";
            string degree = $"CurveDegree = {Degree}";

            stringBuilder.AppendLine(controlPts);
            stringBuilder.AppendLine(knots);
            stringBuilder.AppendLine(degree);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Compares two curves for equality.<br/>
        /// Two curves are equal when the have same control points, weights, knots and degree.
        /// </summary>
        /// <param name="other">The other curve.</param>
        /// <returns>Return true if the curves are equal.</returns>
        public bool Equals(NurbsBase? other)
        {
            if (other == null)
            {
                return false;
            }

            if (!ControlPointLocations.SequenceEqual(other.ControlPointLocations))
            {
                return false;
            }

            if (!Knots.SequenceEqual(other.Knots))
            {
                return false;
            }

            if (Degree != other.Degree)
            {
                return false;
            }

            return Weights.SequenceEqual(other.Weights);
        }

        /// <summary>
        /// Compares if two curves are the same.<br/>
        /// Two curves are equal when the have same degree, same control points order and dimension, and same knots.
        /// </summary>
        /// <param name="obj">The curve object.</param>
        /// <returns>Return true if the curves are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is NurbsBase curve)
                return Equals(curve);
            return false;
        }

        public override int GetHashCode()
        {
            var sBldr = new StringBuilder();
            sBldr.Append(Degree);
            sBldr.Append(Knots);

            foreach (var ptStr in ControlPointLocations.Select(p => p.ToString().ToList()))
            {
                sBldr.Append(ptStr);
            }

            foreach (var wtStr in Weights.Select(w => w.ToString(CultureInfo.InvariantCulture).ToList()))
            {
                sBldr.Append(wtStr);
            }

            return sBldr.ToString().GetHashCode();
        }
    }
}
