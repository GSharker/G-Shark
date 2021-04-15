using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.Operation;

namespace GeometrySharp.Geometry
{
    // ToDo: Contains a point https://stackoverflow.com/questions/217578/how-can-i-determine-whether-a-2d-point-is-within-a-polygon
    // ToDo: Centroid by area.
    // ToDo: throw exception if the polyline self intersect.
    /// <summary>
    /// A closed planar Polyline.
    /// </summary>
    public class Polygon : Polyline
    {
        public Polygon(IList<Vector3> vertices) 
            : base(vertices)
        {
            if (vertices.Count < 3)
            {
                throw new Exception("Insufficient points for a Polygon.");
            }

            Plane fitPlane = Plane.FitPlane(vertices, out double deviation);

            if (!(Math.Abs(deviation) < GeoSharpMath.MINTOLERANCE))
            {
                throw new Exception("The points must be co-planar.");
            }

            IList<Vector3> cleanedVertices = CleanVerticesForShortLength(vertices);
            if (!vertices[0].Equals(vertices[^1]))
            {
                cleanedVertices.Add(cleanedVertices[0]);
            }

            AddRange(cleanedVertices);
        }

        /// <summary>
        /// Gets the centroid averaging the vertices. 
        /// </summary>
        public Vector3 CentroidByVertices => Evaluation.CentroidByVertices(this);

        /// <summary>
        /// Gets the area of the polyline.
        /// </summary>
        public double Area => Evaluation.CalculateArea(this);

        /// <summary>
        /// Applies a transformation to all the points.
        /// </summary>
        /// <param name="transform">Transformation matrix to apply.</param>
        /// <returns>A polygon transformed.</returns>
        public new Polygon Transform(Transform transform)
        {
            List<Vector3> transformedPts = this.Select(pt => pt * transform).ToList();

            return new Polygon(transformedPts);
        }
    }
}
