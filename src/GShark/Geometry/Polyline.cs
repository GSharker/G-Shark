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
    public class Polyline : List<Point3>, ICurve, ITransformable<Polyline>
    {
        /// <summary>
        /// Initializes a new polyline from a collection of points.
        /// </summary>
        /// <param name="vertices">Points used to create the polyline.</param>
        public Polyline(IList<Point3> vertices)
        {
            if (vertices.Count < 2)
            {
                throw new Exception("Insufficient points for a polyline.");
            }

            AddRange(RemoveShortSegments(vertices));
        }

        /// <summary>
        /// Gets the number of segments for this polyline;
        /// </summary>
        public int SegmentsCount => Count - 1;

        /// <summary>
        /// Gets true if the polyline is closed.
        /// A polyline is considered closed, if its start and end point are identical.
        /// </summary>
        public bool IsClosed => this[0] == this[Count - 1];

        /// <summary>
        /// Gets the starting point of the polyline.
        /// </summary>
        public Point3 StartPoint => this[0];

        /// <summary>
        /// Gets the middle point of the polyline.
        /// </summary>
        public Point3 MidPoint => PointAt(0.5);

        /// <summary>
        /// Gets the end point of the polyline.
        /// </summary>
        public Point3 EndPoint => this[Count - 1];

        /// <summary>
        /// Calculates the length of the polyline.
        /// </summary>
        /// <value>The total length of the polyline.</value>
        public double Length
        {
            get
            {
                double length = 0.0;

                for (int i = 0; i < Count - 1; i++)
                {
                    length += this[i].DistanceTo(this[i + 1]);
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
                Line[] lines = new Line[Count - 1];

                for (int i = 0; i < Count - 1; i++)
                {
                    lines[i] = new Line(this[i], this[i + 1]);
                }

                return lines.ToList();
            }
        }

        /// <summary>
        /// Computes the bounding box of the list of points.
        /// </summary>
        /// <returns>The bounding box.</returns>
        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(this);
        }

        /// <summary>
        /// Evaluated the length on the polyline at the given parameter.
        /// </summary>
        /// <param name="t">Evaluate parameter. Parameter should be between 0.0 and segments count.</param>
        /// <returns>The evaluated length at the curve parameter.</returns>
        public double LengthAt(double t)
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
        public Polyline Close()
        {
            List<Point3> copyPts = new List<Point3>(this);
            copyPts.Add(copyPts[0]);
            return new Polyline(copyPts);
        }

        /// <summary>
        /// Evaluates the point on the polyline at the given parameter. The integer part of the parameter indicates the index of the segment.
        /// </summary>
        /// <param name="t">Evaluate parameter. Parameter should be between 0.0 and segments count.</param>
        /// <returns>The evaluated point at the curve parameter.</returns>
        public Point3 PointAt(double t)
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

        public Point3 PointAtLength(double length, bool normalized = false)
        {
            if (length <= 0.0)
            {
                return StartPoint;
            }

            if (length >= Length)
            {
                return EndPoint;
            }

            length = (normalized)
                ? GSharkMath.RemapValue(length, new Interval(0.0, 1.0), new Interval(0.0, Length))
                : length;

            double progressiveEndLength = 0;

            for (int i = 0; i < SegmentsCount; i++)
            {
                double progressiveStartLength = progressiveEndLength;
                progressiveEndLength += Segments[i].Length;
                if (length <= progressiveEndLength) // This is the right segment
                {
                    double segmentLength = i == 0 ? length : length - progressiveStartLength;

                }
            }

            throw new NotImplementedException();
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

            double tempLength = this[0].DistanceTo(this[1]);
            int segIdx = 0;
            for (int i = 0; i < SegmentsCount; i++)
            {
                if (tempLength >= length)
                {
                    segIdx = i;
                    break;
                }

                tempLength += this[i + 1].DistanceTo(this[i + 2]);
            }

            return Segments[segIdx];
        }

        /// <summary>
        /// Gets the parameter along the polyline which is closest to the test point.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <returns>The parameter closest to the test point.</returns>
        public double ClosestParameter(Point3 pt)
        {
            int index = 0;
            double valueT = 0.0;
            double smallestDistance = double.MaxValue;

            for (int i = 0; i < Count - 1; i++)
            {
                Line line = new Line(this[i], this[i + 1]);
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
        public Point3 ClosestPoint(Point3 pt)
        {
            // Brute force
            if (Count <= 4)
            {
                double distance = double.MaxValue;
                Point3 closestPt = Point3.Unset;

                for (int i = 0; i < Count - 1; i++)
                {
                    Line tempLine = new Line(this[i], this[i + 1]);
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
            List<Point3> conquer = new List<Point3>(this);

            while (leftSubCollection.Count != 2 || rightSubCollection.Count != 2)
            {
                int mid = (int)((double)conquer.Count / 2);
                leftSubCollection = conquer.Take(mid + 1).ToList();
                rightSubCollection = conquer.Skip(mid).ToList();

                Polyline leftPoly = new Polyline(leftSubCollection);
                Polyline rightPoly = new Polyline(rightSubCollection);

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
        public new Polyline Reverse()
        {
            List<Point3> copyVertices = new List<Point3>(this);
            copyVertices.Reverse();
            return new Polyline(copyVertices);
        }

        /// <summary>
        /// Applies a transformation to all the points.
        /// </summary>
        /// <param name="transform">Transformation matrix to apply.</param>
        /// <returns>A polyline transformed.</returns>
        public Polyline Transform(Transform transform)
        {
            List<Point3> transformedPts = this.Select(pt => pt.Transform(transform)).ToList();

            return new Polyline(transformedPts);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this polyline.
        /// </summary>
        /// <returns>A Nurbs curve shaped like this polyline.</returns>
        public NurbsCurve ToNurbs()
        {
            double lengthSum = 0;
            KnotVector knots = new KnotVector { 0, 0 };
            List<Point4> ctrlPts = Point4.PointsHomogeniser(this, 1.0);

            for (int i = 0; i < Count - 1; i++)
            {
                lengthSum += Segments[i].Length;
                knots.Add(lengthSum);
            }
            knots.Add(lengthSum);

            return new NurbsCurve(1, knots, ctrlPts);
        }

        /// <summary>
        /// Computes the offset of the polyline.
        /// </summary>
        /// <param name="distance">The distance of the offset. If negative the offset will be in the opposite side.</param>
        /// <param name="pln">The plane for the offset operation.</param>
        /// <returns>The offset polyline.</returns>
        public Polyline Offset(double distance, Plane pln)
        {
            if (distance == 0.0)
            {
                return this;
            }

            int iteration = (IsClosed) ? Count : Count - 1;

            Point3[] offsetPts = new Point3[Count];
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
                Transform xForm = Core.Transform.Translation(vecOffset);
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

            return new Polyline(offsetPts);
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
        /// Constructs the string representation of the polyline.
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return string.Join(" : ", this);
        }
    }
}
