using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A simple data structure representing a polyline.
    /// PolylineData is useful, for example, as the result of a curve tessellation.
    /// </summary>
    public class Polyline : List<Vector3>
    {
        // PointAt
        // TangentAt
        // ClosestPointTo
        // Center

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
                throw new Exception("Insufficient points for a polyline.");

            this.AddRange(CleanVerticesForShortLength(vertices));
        }

        /// <summary>
        /// Calculate the length of the polyline.
        /// </summary>
        /// <returns>The total length of the polyline.</returns>
        public double Length()
        {
            double length = 0.0;

            for (int i = 0; i < this.Count - 1; i++)
                length += this[i].DistanceTo(this[i + 1]);

            return length;
        }

        /// <summary>
        /// Constructs a collections of lines, which make the polyline.
        /// </summary>
        /// <returns>A collection of lines.</returns>
        public Line[] Segments()
        {
            int count = this.Count;
            Line[] lines = new Line[count - 1];

            for (int i = 0; i < count - 1; i++)
                lines[i] = new Line(this[i], this[i+1]);

            return lines;
        }

        /// <summary>
        /// Compute the segments length and removes the segments which are shorter than a tolerance.
        /// </summary>
        /// <param name="vertices">Points used to create the polyline.</param>
        /// <returns>A cleaned collections of points if necessary otherwise the same collection of points.</returns>
        private static IList<Vector3> CleanVerticesForShortLength(IList<Vector3> vertices)
        {
            int verticesCount = vertices.Count;

            int[] flag = new int[verticesCount];
            flag[0] = 0;

            for (int i = 1; i < verticesCount; i++)
            {
                flag[i] = 0;
                if (vertices[i - 1].DistanceTo(vertices[i]) <= GeoSharpMath.MAXTOLERANCE)
                    flag[i] = 1;
            }

            int numberOfCoincidence = flag.Sum();
            if (numberOfCoincidence == 0) return vertices;

            Vector3[] cleanedList = new Vector3[verticesCount - numberOfCoincidence];

            int counter = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (flag[i] != 0) continue;
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
