using GShark.Core;
using GShark.Interfaces;
using GShark.Intersection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// A simple data structure representing a polyline.
    /// </summary>
    /// <example>
    /// [!code-csharp[Example](../../src/GShark.Test.XUnit/Geometry/PolylineTests.cs?name=example)]
    /// </example>
    public class PolyLine : NurbsBase, ICurve<PolyLine>
    {
        /// <summary>
        /// Initializes a new polyline from a collection of points.
        /// </summary>
        /// <param name="vertices">Points used to create the polyline.</param>
        public PolyLine(IList<Point3> vertices)
        {
            if (vertices.Count < 2)
            {
                throw new Exception("Insufficient points for a polyline.");
            }

            ControlPointLocations.AddRange(RemoveShortSegments(vertices));
            ToNurbs();
        }

        /// <summary>
        /// Gets the number of segments for this polyline;
        /// </summary>
        public int SegmentsCount => ControlPointLocations.Count - 1;

        /// <summary>
        /// Gets the starting point of the polyline.
        /// </summary>
        public override Point3 StartPoint => ControlPointLocations[0];

        /// <summary>
        /// Gets the middle point of the polyline.
        /// </summary>
        public override Point3 MidPoint => PointAtNormalizedLength(0.5);

        /// <summary>
        /// Gets the end point of the polyline.
        /// </summary>
        public override Point3 EndPoint => ControlPointLocations.Last();

        /// <summary>
        /// Calculates the length of the polyline.
        /// </summary>
        /// <value>The total length of the polyline.</value>
        public override double Length
        {
            get
            {
                double length = 0.0;

                for (int i = 0; i < ControlPointLocations.Count - 1; i++)
                {
                    length += ControlPointLocations[i].DistanceTo(ControlPointLocations[i + 1]);
                }

                return length;
            }
        }

        /// <summary>
        /// Constructs a collections of lines, which make the polyline.
        /// </summary>
        /// <value>A collection of lines.</value>
        public List<Line> Segments
        {
            get
            {
                Line[] lines = new Line[ControlPointLocations.Count - 1];

                for (int i = 0; i < ControlPointLocations.Count - 1; i++)
                {
                    lines[i] = new Line(ControlPointLocations[i], ControlPointLocations[i + 1]);
                }

                return lines.ToList();
            }
        }

        /// <summary>
        /// Gets true if the polyline is closed.
        /// A polyline is considered closed, if its start and end point are identical.
        /// </summary>
        public override bool IsClosed => ControlPointLocations[0] == ControlPointLocations.Last();

        /// <summary>
        /// Computes the bounding box of the list of points.
        /// </summary>
        /// <returns>The bounding box.</returns>
        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(ControlPointLocations);
        }

        /// <summary>
        /// Evaluated the length on the polyline at the given parameter.
        /// </summary>
        /// <param name="t">Evaluate parameter. Parameter should be between 0.0 and segments count.</param>
        /// <returns>The evaluated length at the curve parameter.</returns>
        public override double LengthAt(double t)
        {
            if (t <= 0)
            {
                return 0.0;
            }

            if (t >= SegmentsCount)
            {
                return Length;
            }

            int segIdx = (int)Math.Truncate(t);
            double t2 = Math.Abs(t - segIdx);

            double length = Segments[segIdx].Length * t2;
            length += Segments.GetRange(0, segIdx).Sum(seg => seg.Length);
            return length;
        }

        /// <summary>
        /// Creates a closed polyline, where the first and last point are the same.
        /// </summary>
        /// <returns>A closed polyline.</returns>
        public new PolyLine Close()
        {
            List<Point3> copyPts = new List<Point3>(ControlPointLocations);
            copyPts.Add(copyPts[0]);
            return new PolyLine(copyPts);
        }

        /// <summary>
        /// Evaluates the point on the polyline at the given parameter. The integer part of the parameter indicates the index of the segment.
        /// </summary>
        /// <param name="t">Evaluate parameter. Parameter should be between 0.0 and segments count.</param>
        /// <returns>The evaluated point at the curve parameter.</returns>
        public override Point3 PointAt(double t)
        {
            if (t <= 0)
            {
                return StartPoint;
            }

            if (t >= SegmentsCount)
            {
                return EndPoint;
            }

            int segIdx = (int)Math.Truncate(t);
            double t2 = Math.Abs(t - segIdx);
            return Segments[segIdx].PointAt(t2);
        }

        /// <summary>
        /// <inheritdoc cref="ICurve.PointAtLength"/>
        /// </summary>
        public override Point3 PointAtLength(double length)
        {
            if (length <= 0.0)
            {
                return StartPoint;
            }

            return length >= Length ? EndPoint : PointAt(ParameterAtLength(length));
        }

        /// <summary>
        /// Evaluates a point at the normalized length.
        /// </summary>
        /// <param name="normalizedLength">The length factor is normalized between 0.0 and 1.0.</param>
        /// <returns>The point at the length.</returns>
        public override Point3 PointAtNormalizedLength(double normalizedLength)
        {
            double length = GSharkMath.RemapValue(normalizedLength, new Interval(0.0, 1.0), new Interval(0.0, Length));
            return PointAtLength(length);
        }

        /// <summary>
        /// Calculates the parameter of the polyline at the given length.
        /// </summary>
        /// <param name="length">Length from start of polyline.</param>
        /// <returns>The parameter at the given length.</returns>
        public override double ParameterAtLength(double length)
        {
            if (length <= 0.0)
            {
                return 0.0;
            }

            if (length < Length)
            {
                double progressiveEndLength = 0.0;

                for (int i = 0; i < SegmentsCount; i++)
                {
                    double progressiveStartLength = progressiveEndLength;
                    progressiveEndLength += Segments[i].Length;
                    if (!(length <= progressiveEndLength)) continue;
                    return (Math.Abs(length - progressiveStartLength) / Segments[i].Length) + i;
                }
            }

            return SegmentsCount;
        }

        /// <summary>
        /// Returns the index of the segment at a given length along the polyline.
        /// </summary>
        /// <param name="length">Length from start of polyline.</param>
        /// <returns>Zero based index of polyline segment.</returns>
        public Line SegmentAtLength(double length)
        {
            if (length <= 0)
            {
                return Segments.First();
            }

            if (length >= Length)
            {
                return Segments.Last();
            }

            double tempLength = ControlPointLocations[0].DistanceTo(ControlPointLocations[1]);
            int segIdx = 0;
            for (int i = 0; i < SegmentsCount; i++)
            {
                if (tempLength >= length)
                {
                    segIdx = i;
                    break;
                }

                tempLength += ControlPointLocations[i + 1].DistanceTo(ControlPointLocations[i + 2]);
            }

            return Segments[segIdx];
        }

        /// <summary>
        /// Gets the parameter along the polyline which is closest to the test point.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <returns>The parameter closest to the test point.</returns>
        public override double ClosestParameter(Point3 pt)
        {
            int index = 0;
            double valueT = 0.0;
            double smallestDistance = double.MaxValue;

            for (int i = 0; i < ControlPointLocations.Count - 1; i++)
            {
                Line line = new Line(ControlPointLocations[i], ControlPointLocations[i + 1]);
                double tempT = line.ClosestParameter(pt);
                if (tempT < 0.0)
                {
                    tempT = 0.0;
                }
                if (tempT > 1.0)
                {
                    tempT = 1.0;
                }

                double distFromPt = line.PointAt(tempT).DistanceTo(pt);
                if (!(distFromPt < smallestDistance)) continue;
                smallestDistance = distFromPt;
                valueT = tempT;
                index = i;
            }

            return valueT + index;
        }

        /// <summary>
        /// Computes the point which is the closest point to the given point.
        /// </summary>
        /// <param name="pt">Point to test.</param>
        /// <returns>The point closest to the given point.</returns>
        public override Point3 ClosestPoint(Point3 pt)
        {
            // Brute force
            if (ControlPointLocations.Count <= 4)
            {
                double distance = double.MaxValue;
                Point3 closestPt = Point3.Unset;

                for (int i = 0; i < ControlPointLocations.Count - 1; i++)
                {
                    Line tempLine = new Line(ControlPointLocations[i], ControlPointLocations[i + 1]);
                    Point3 tempPt = tempLine.ClosestPoint(pt);
                    double tempDistance = tempPt.DistanceTo(pt);

                    if (!(tempDistance < distance))
                    {
                        continue;
                    }

                    closestPt = tempPt;
                    distance = tempDistance;
                }

                return closestPt;
            }

            // Divide and conquer.
            List<Point3> leftSubCollection = new List<Point3>();
            List<Point3> rightSubCollection = new List<Point3>();
            List<Point3> conquer = new List<Point3>(ControlPointLocations);

            while (leftSubCollection.Count != 2 || rightSubCollection.Count != 2)
            {
                int mid = (int)((double)conquer.Count / 2);
                leftSubCollection = conquer.Take(mid + 1).ToList();
                rightSubCollection = conquer.Skip(mid).ToList();

                PolyLine leftPoly = new PolyLine(leftSubCollection);
                PolyLine rightPoly = new PolyLine(rightSubCollection);

                Point3 leftPt = leftPoly.PointAt(0.5);
                Point3 rightPt = rightPoly.PointAt(0.5);

                double leftDistance = leftPt.DistanceTo(pt);
                double rightDistance = rightPt.DistanceTo(pt);

                conquer = leftDistance > rightDistance
                    ? new List<Point3>(rightSubCollection)
                    : new List<Point3>(leftSubCollection);
            }

            Line line = new Line(conquer[0], conquer[1]);

            return line.ClosestPoint(pt);
        }

        /// <summary>
        /// Reverses the order of the polyline.
        /// </summary>
        /// <returns>A polyline reversed.</returns>
        public new PolyLine Reverse()
        {
            List<Point3> copyVertices = new List<Point3>(ControlPointLocations);
            copyVertices.Reverse();
            return new PolyLine(copyVertices);
        }

        /// <summary>
        /// Applies a transformation to all the points.
        /// </summary>
        /// <param name="transform">Transformation matrix to apply.</param>
        /// <returns>A polyline transformed.</returns>
        public PolyLine Transform(TransformMatrix transform)
        {
            List<Point3> transformedPts = ControlPointLocations.Select(pt => pt.Transform(transform)).ToList();

            return new PolyLine(transformedPts);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this polyline.
        /// </summary>
        /// <returns>A Nurbs curve shaped like this polyline.</returns>
        private void ToNurbs()
        {
            double lengthSum = 0;
            List<double> weights = new List<double>();
            KnotVector knots = new KnotVector { 0, 0 };
            List<Point4> ctrlPts = new List<Point4>();

            for (int i = 0; i < ControlPointLocations.Count - 1; i++)
            {
                lengthSum += Segments[i].Length;
                knots.Add(lengthSum);
                weights.Add(1.0);
                ctrlPts.Add(new Point4(ControlPointLocations[i], 1.0));
            }
            knots.Add(lengthSum);
            weights.Add(1.0);
            ctrlPts.Add(new Point4(ControlPointLocations.Last(), 1.0));

            Weights = weights;
            Knots = knots.Normalize();
            Degree = 1;
            ControlPoints = ctrlPts;
        }

        /// <summary>
        /// Computes the offset of the polyline.
        /// </summary>
        /// <param name="distance">The distance of the offset. If negative the offset will be in the opposite side.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset polyline.</returns>
        public new PolyLine Offset(double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return this;
            }

            int iteration = (IsClosed) ? ControlPointLocations.Count : ControlPointLocations.Count - 1;

            Point3[] offsetPts = new Point3[ControlPointLocations.Count];
            List<Line> segments = Segments;
            Line[] offsetSegments = new Line[segments.Count + 1];

            for (int i = 0; i < iteration; i++)
            {
                int k = (i == iteration - 1 && IsClosed) ? 0 : i;
                if (i == iteration - 1 && k == 0)
                {
                    goto Intersection;
                }

                Vector3 vecOffset = Vector3.CrossProduct(segments[k].Direction, pln.ZAxis).Amplify(distance);
                var xForm = Core.Transform.Translation(vecOffset);
                offsetSegments[k] = segments[k].Transform(xForm);

                if (i == 0 && IsClosed)
                {
                    continue;
                }
                if (k == 0 && !IsClosed)
                {
                    offsetPts[k] = offsetSegments[k].StartPoint;
                    continue;
                }

            Intersection:
                bool ccx = Intersect.LineLine(offsetSegments[(i == iteration - 1 && IsClosed) ? iteration - 2 : k - 1], offsetSegments[k], out Point3 pt, out _, out _, out _);
                if (!ccx)
                {
                    continue;
                }

                offsetPts[k] = pt;

                if (i == iteration - 1)
                {
                    offsetPts[(IsClosed) ? i : i + 1] = (IsClosed) ? offsetPts[0] : offsetSegments[k].EndPoint;
                }
            }

            return new PolyLine(offsetPts);
        }

        /// <summary>
        /// Compute the segments length and removes the segments which are shorter than a tolerance.
        /// </summary>
        /// <param name="vertices">Points used to create the polyline.</param>
        /// <returns>A cleaned collections of points if necessary otherwise the same collection of points.</returns>
        private static IList<Point3> RemoveShortSegments(IList<Point3> vertices)
        {
            int[] coincidenceFlag = new int[vertices.Count];
            coincidenceFlag[0] = 0;

            for (int i = 1; i < vertices.Count; i++)
            {
                coincidenceFlag[i] = 0;
                if (vertices[i - 1].DistanceTo(vertices[i]) <= GSharkMath.MaxTolerance)
                {
                    coincidenceFlag[i] = 1;
                }
            }

            int numberOfCoincidences = coincidenceFlag.Sum();
            if (numberOfCoincidences == 0)
            {
                return vertices;
            }

            Point3[] cleanedList = new Point3[vertices.Count - numberOfCoincidences];

            int counter = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (coincidenceFlag[i] != 0)
                {
                    continue;
                }

                cleanedList[counter] = vertices[i];
                counter++;
            }

            return cleanedList;
        }

        /// <summary>
        /// Checks if a polyline is equal to the provided polyline.<br/>
        /// Two polyline are equal if all the points are the same.
        /// </summary>
        /// <param name="other">The polyline to compare.</param>
        /// <returns>True if the points are equal, otherwise false.</returns>
        public bool Equals(PolyLine other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return false;
            }

            return ControlPointLocations.SequenceEqual(other.ControlPointLocations);
        }

        /// <summary>
        /// Constructs the string representation of the polyline.
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return string.Join(" : ", this);
        }
    }
}
