using GeometrySharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a polyline.
    /// PolylineData is useful, for example, as the result of a curve tessellation.
    /// </summary>
    public class Polyline : List<Vector3>
    {
        /// <summary>
        /// Initializes a new empty polyline.
        /// </summary>
        public Polyline()
        {
        }

        // ToDo: throw exception if the polyline self intersect.
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
        }

        /// <summary>
        /// Calculate the length of the polyline.
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
                lines[i] = new Line(this[i], this[i+1]);
            }

            return lines;
        }

        /// <summary>
        /// Gets the point which is the closest point to the given point.
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
                    Vector3 tempPt = tempLine.ClosestPoint(pt);
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

                Vector3 leftPt = leftPoly.PointAt(0.5, out _);
                Vector3 rightPt = rightPoly.PointAt(0.5, out _);

                double leftDistance = leftPt.DistanceTo(pt);
                double rightDistance = rightPt.DistanceTo(pt);

                conquer  = leftDistance > rightDistance 
                    ? new List<Vector3>(rightSubCollection) 
                    : new List<Vector3>(leftSubCollection);
            }

            Line l = new Line(conquer [0], conquer [1]);

            return l.ClosestPoint(pt);
        }

        /// <summary>
        /// Calculates the bounding box of the list of points.
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
        /// Get the point on the polyline at the give parameter.
        /// The parameter must be between 0.0 and 1.0.
        /// </summary>
        /// <param name="t">Parameter to evaluate at.</param>
        /// <returns>The point on the polyline at t.</returns>
        public Vector3 PointAt(double t, out Vector3 tangent)
        {
            if (t < 0.0 || t > 1.0)
            {
                throw new Exception($"The value of t ({t}) must be between 0.0 and 1.0.");
            }

            int verticesCount = Count;
            if (t <= GeoSharpMath.EPSILON)
            {
                tangent = (this[1] - this[0]).Unitize();
                return this[0];
            }

            if (Math.Abs(t - 1) <= GeoSharpMath.EPSILON)
            {
                tangent = (this[verticesCount - 1] - this[verticesCount - 2]).Unitize();
                return this[verticesCount - 1];
            }

            double tRemapped = GeoSharpMath.RemapValue(t, new Interval(0.0, 1.0), new Interval(0.0, verticesCount - 1));

            double floorValue = Math.Floor(tRemapped);
            int segmentIndex = (int)floorValue;
            tRemapped -= floorValue;

            tangent = (this[segmentIndex + 1] - this[segmentIndex]).Unitize();
            return this[segmentIndex] * (1 - tRemapped) + this[segmentIndex + 1] * tRemapped;
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
