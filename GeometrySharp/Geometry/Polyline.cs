using GeometrySharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Geometry.Interfaces;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a polyline.
    /// </summary>
    public class Polyline : List<Vector3>, ICurve
    {
        /// <summary>
        /// Initializes a new polyline from a collection of points.
        /// </summary>
        /// <param name="vertices">Points used to create the polyline.</param>
        public Polyline(IList<Vector3> vertices)
        {
            if(vertices.Count < 2)
            {
                throw new Exception("Insufficient points for a polyline.");
            }

            AddRange(CleanVerticesForShortLength(vertices));
            ToNurbsCurve();
        }

        public int Degree => 1;

        public List<Vector3> ControlPoints => this;

        public List<Vector3> HomogenizedPoints { get; private set; }

        public Knot Knots { get; private set; }

        public Interval Domain => new Interval(0, this.Count - 1);

        /// <summary>
        /// Gets the number of segments for this polyline;
        /// </summary>
        public int SegmentsCount => this.Count - 1;

        /// <summary>
        /// Computes the bounding box of the list of points.
        /// </summary>
        /// <returns>The bounding box.</returns>
        public BoundingBox BoundingBox
        {
            get
            {
                double minX = double.MaxValue;
                double minY = double.MaxValue;
                double minZ = double.MaxValue;
                double maxX = double.MinValue;
                double maxY = double.MinValue;
                double maxZ = double.MinValue;

                for (int i = 0; i < Count; i++)
                {
                    minX = Math.Min(minX, this[i][0]);
                    maxX = Math.Max(maxX, this[i][0]);
                    minY = Math.Min(minY, this[i][1]);
                    maxY = Math.Max(maxY, this[i][1]);
                    minZ = Math.Min(minZ, this[i][2]);
                    maxZ = Math.Max(maxZ, this[i][2]);
                }

                Vector3 minPt = new Vector3 { minX, minY, minZ };
                Vector3 maxPt = new Vector3 { maxX, maxY, maxZ };

                return new BoundingBox(minPt, maxPt);
            }
        }

        /// <summary>
        /// Calculates the length of the polyline.
        /// </summary>
        /// <returns>The total length of the polyline.</returns>
        public double Length()
        {
            double length = 0.0;

            for (int i = 0; i < Count - 1; i++)
            {
                length += this[i].DistanceTo(this[i + 1]);
            }

            return length;
        }

        /// <summary>
        /// Constructs a collections of lines, which make the polyline.
        /// </summary>
        /// <returns>A collection of lines.</returns>
        public Line[] Segments()
        {
            int count = Count;
            Line[] lines = new Line[count - 1];

            for (int i = 0; i < count - 1; i++)
            {
                lines[i] = new Line(this[i], this[i + 1]);
            }

            return lines;
        }

        /// <summary>
        /// Gets the line segment at the given index.
        /// </summary>
        /// <param name="index">Index of the segment to find.</param>
        /// <returns>The line segment at the index.</returns>
        public Line SegmentAt(int index)
        {
            if (index < 0 || index > this.Count - 2)
            {
                throw new Exception("Impossible to find the segment, index is to big or to small.");
            }

            return new Line(this[index], this[index + 1]);
        }

        /// <summary>
        /// Computes the unit tangent vector along the polyline at the given parameter.
        /// </summary>
        /// <param name="t">The polyline parameter.</param>
        /// <returns>The unit tangent at the parameter t.</returns>
        public Vector3 TangentAt(double t)
        {
            int index = (int) Math.Truncate(t);
            if (index < 0)
            {
                index = 0;
            }
            if (index > this.Count - 2)
            {
                index = this.Count - 2;
            }

            return SegmentAt(index).Direction.Unitize();
        }

        /// <summary>
        /// Computes the point on the polyline at the given parameter.
        /// </summary>
        /// <param name="t">The polyline parameter.</param>
        /// <returns>The point on the polyline at the parameter.</returns>
        public Vector3 PointAt(double t)
        {
            if (t < 0 || t > this.Count - 1)
            {
                throw new Exception("Parameter t must be between the polyline's domain.");
            }

            int index = (int)Math.Truncate(t);

            if (index > this.Count - 2)
            {
                index = this.Count - 2;
            }

            double t2 = Math.Abs(t - index);
            Line segment = SegmentAt(index);
            return segment.PointAt(t2);
        }

        /// <summary>
        /// Gets the parameter along the polyline which is closest to the point.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <returns>The parameter closest to the point.</returns>
        public double ClosestParameter(Vector3 pt)
        {
            int index = 0;
            double valueT = 0.0;
            double smallestDistance = double.MaxValue;

            for (int i = 0; i < this.Count - 1; i++)
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
                if (distFromPt < smallestDistance)
                {
                    smallestDistance = distFromPt;
                    valueT = tempT;
                    index = i;
                }
            }

            return valueT + index;
        }

        /// <summary>
        /// Computes the point which is the closest point to the given point.
        /// </summary>
        /// <param name="pt">Point to test.</param>
        /// <returns>The point closest to the given point.</returns>
        public Vector3 ClosestPt(Vector3 pt)
        {
            // Brute force
            if (this.Count <= 4)
            {
                double distance = double.MaxValue;
                Vector3 closestPt = Vector3.Unset;

                for (int i = 0; i < this.Count - 1; i++)
                {
                    Line tempLine = new Line(this[i], this[i + 1]);
                    Vector3 tempPt = tempLine.ClosestPt(pt);
                    double tempDistance = tempPt.DistanceTo(pt);

                    if (!(tempDistance < distance)) continue;
                    closestPt = tempPt;
                    distance = tempDistance;
                }

                return closestPt;
            }

            // Divide and conquer.
            List<Vector3> leftSubCollection = new List<Vector3>();
            List<Vector3> rightSubCollection = new List<Vector3>();
            List<Vector3> conquer = new List<Vector3>(this);

            while (leftSubCollection.Count != 2 || rightSubCollection.Count != 2)
            {
                int mid = (int) ((double) conquer.Count / 2);
                leftSubCollection = conquer.Take(mid+1).ToList();
                rightSubCollection = conquer.Skip(mid).ToList();

                Polyline leftPoly = new Polyline(leftSubCollection);
                Polyline rightPoly = new Polyline(rightSubCollection);

                Vector3 leftPt = leftPoly.PointAt(0.5);
                Vector3 rightPt = rightPoly.PointAt(0.5);

                double leftDistance = leftPt.DistanceTo(pt);
                double rightDistance = rightPt.DistanceTo(pt);

                conquer  = leftDistance > rightDistance 
                    ? new List<Vector3>(rightSubCollection) 
                    : new List<Vector3>(leftSubCollection);
            }

            Line l = new Line(conquer [0], conquer [1]);

            return l.ClosestPt(pt);
        }

        /// <summary>
        /// Reverses the order of the polyline.
        /// </summary>
        /// <returns>A polyline reversed.</returns>
        public Polyline Reverse()
        {
            List<Vector3> copyVertices = new List<Vector3>(this);
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
            List<Vector3> transformedPts = this.Select(pt => pt * transform).ToList();

            return new Polyline(transformedPts);
        }

        /// <summary>
        /// Constructs a nurbs curve representation of this polyline.
        /// </summary>
        /// <returns>A Nurbs curve shaped like this polyline.</returns>
        private void ToNurbsCurve()
        {
            double lengthSum = 0;
            Knot knots = new Knot{0};
            List<double> weights = new List<double>();

            for (int i = 0; i < this.Count; i++)
            {
                lengthSum += 1;
                knots.Add(i);
                weights.Add(1.0);
            }
            knots.Add(lengthSum-1);

            Knots = knots;
            HomogenizedPoints = LinearAlgebra.PointsHomogeniser(this, weights);
        }

        /// <summary>
        /// Compute the segments length and removes the segments which are shorter than a tolerance.
        /// </summary>
        /// <param name="vertices">Points used to create the polyline.</param>
        /// <returns>A cleaned collections of points if necessary otherwise the same collection of points.</returns>
        protected static IList<Vector3> CleanVerticesForShortLength(IList<Vector3> vertices)
        {
            int verticesCount = vertices.Count;

            int[] flag = new int[verticesCount];
            flag[0] = 0;

            for (int i = 1; i < verticesCount; i++)
            {
                flag[i] = 0;
                if (vertices[i - 1].DistanceTo(vertices[i]) <= GeoSharpMath.MAXTOLERANCE)
                {
                    flag[i] = 1;
                }
            }

            int numberOfCoincidence = flag.Sum();
            if (numberOfCoincidence == 0)
            {
                return vertices;
            }

            Vector3[] cleanedList = new Vector3[verticesCount - numberOfCoincidence];

            int counter = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (flag[i] != 0)
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
            return string.Join<Vector3>(" : ", this);
        }
    }
}
