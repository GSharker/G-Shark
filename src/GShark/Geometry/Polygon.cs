using System;
using System.Collections.Generic;
using System.Linq;
using GShark.Core;
using GShark.Operation;

namespace GShark.Geometry
{
    /// <summary>
    /// A closed planar <see cref="Polyline"/>.
    /// </summary>
    public class Polygon : Polyline
    {
        public Polygon(IList<Vector3> vertices) : base(vertices)
        {
            if (vertices.Count < 3)
            {
                throw new Exception("Insufficient points for a Polygon.");
            }

            Plane fitPlane = Plane.FitPlane(vertices, out double deviation);

            if (!(Math.Abs(deviation) < GeoSharpMath.MIN_TOLERANCE))
            {
                throw new Exception("The points must be co-planar.");
            }

            if (IsClosed) return;
            Add(vertices[0]);
            ToNurbsCurve();
        }

        /// <summary>
        /// Gets the centroid averaging the vertices. 
        /// </summary>
        public Vector3 CentroidByVertices => Evaluation.CentroidByVertices(this);

        /// <summary>
        /// Gets the centroid of mass of the polygon.<br/>
        /// https://stackoverflow.com/questions/9815699/how-to-calculate-centroid <br/>
        /// http://csharphelper.com/blog/2014/07/find-the-centroid-of-a-polygon-in-c/
        /// </summary>
        public Vector3 CentroidByArea
        {
            get
            {
                bool isOnPlaneXY = true;
                Transform transformBack = new Transform();
                List<Vector3> copiedPts = new List<Vector3>(this);
                if (Math.Abs(this[0][2]) > GeoSharpMath.MAX_TOLERANCE)
                {
                    isOnPlaneXY = false;
                    Plane polygonPlane = new Plane(this[0], this[1], this[2]);
                    Transform toOrigin = Core.Transform.PlaneToPlane(polygonPlane, Plane.PlaneXY);
                    transformBack = Core.Transform.PlaneToPlane(Plane.PlaneXY, polygonPlane);
                    copiedPts = this.Transform(toOrigin);
                }

                double signedArea = 0.0;
                double valueX = 0.0;
                double valueY = 0.0;

                for (int i = 0; i < copiedPts.Count-1; i++)
                {
                    double x0 = copiedPts[i][0];
                    double y0 = copiedPts[i][1];

                    double x1 = copiedPts[(i + 1) % copiedPts.Count][0];
                    double y1 = copiedPts[(i + 1) % copiedPts.Count][1];

                    double a = x0 * y1 - x1 * y0;
                    signedArea += a;
                    valueX += (x0 + x1) * a;
                    valueY += (y0 + y1) * a;
                }

                signedArea *= 0.5;
                valueX /= (6.0 * signedArea);
                valueY /= (6.0 * signedArea);

                Vector3 centroid = new Vector3 { valueX, valueY, 0.0 };

                if (!isOnPlaneXY)
                {
                    return centroid * transformBack;
                }

                return centroid;
            }
        }

        /// <summary>
        /// Gets the area from a list of points.<br/>
        /// The list should represent a closed curve and planar.<br/>
        /// https://stackoverflow.com/questions/25340106/boostgeometry-find-area-of-2d-polygon-in-3d-space <br/>
        /// http://geomalgorithms.com/a01-_area.html
        /// </summary>
        /// <param name="pts">Set of points.</param>
        /// <returns>Area calculated.</returns>
        public double Area
        {
            get
            {
                double area = 0.0;
                Vector3 normal = Vector3.Cross(this[1] - this[0], this[2] - this[0]).Unitize();

                for (int i = 0; i < this.Count - 1; i++)
                {
                    Vector3 product = Vector3.Cross(this[i] - this[0], this[i + 1] - this[0]);
                    area += Vector3.Dot(product, normal);
                }

                area *= 0.5;
                return Math.Abs(area);
            }
        }

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
