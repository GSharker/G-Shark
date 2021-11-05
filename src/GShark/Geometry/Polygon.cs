using GShark.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using TransformMatrix = GShark.Core.TransformMatrix;

namespace GShark.Geometry
{
    /// <summary>
    /// A closed planar <see cref="PolyLine"/>.
    /// </summary>
    public class Polygon : PolyLine
    {
        public Polygon(IList<Point3> vertices) : base(vertices)
        {
            if (vertices.Count < 3)
            {
                throw new Exception("Insufficient points for a Polygon.");
            }

            Plane fitPlane = Plane.FitPlane(vertices, out double deviation);

            if (!(Math.Abs(deviation) < GSharkMath.MinTolerance))
            {
                throw new Exception("The points must be co-planar.");
            }

            if (IsClosed) return;
            ControlPointLocations.Add(vertices[0]);
        }

        /// <summary>
        /// Gets the centroid averaging the vertices. 
        /// </summary>
        public Point3 CentroidByVertices => Evaluate.Curve.AveragePoint(ControlPointLocations);

        /// <summary>
        /// Gets the centroid of mass of the polygon.<br/>
        /// https://stackoverflow.com/questions/9815699/how-to-calculate-centroid <br/>
        /// http://csharphelper.com/blog/2014/07/find-the-centroid-of-a-polygon-in-c/
        /// </summary>
        public Point3 CentroidByArea
        {
            get
            {
                bool isOnPlaneXy = true;
                var transformBack = new TransformMatrix();
                List<Point3> copiedPts = new List<Point3>(ControlPointLocations);
                if (Math.Abs(ControlPointLocations[0][2]) > GSharkMath.MaxTolerance)
                {
                    isOnPlaneXy = false;
                    Plane polygonPlane = new Plane(ControlPointLocations[0], ControlPointLocations[1], ControlPointLocations[2]);
                    var toOrigin = Core.Transform.PlaneToPlane(polygonPlane, Plane.PlaneXY);
                    transformBack = Core.Transform.PlaneToPlane(Plane.PlaneXY, polygonPlane);
                    copiedPts = Transform(toOrigin).ControlPointLocations;
                }

                double signedArea = 0.0;
                double valueX = 0.0;
                double valueY = 0.0;

                for (int i = 0; i < copiedPts.Count - 1; i++)
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

                Point3 centroid = new Point3(valueX, valueY, 0.0);

                if (!isOnPlaneXy)
                {
                    return centroid.Transform(transformBack);
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
                Vector3 normal = Vector3.CrossProduct(ControlPointLocations[1] - ControlPointLocations[0], ControlPointLocations[2] - ControlPointLocations[0]).Unitize();

                for (int i = 0; i < ControlPointLocations.Count - 1; i++)
                {
                    Vector3 product = Vector3.CrossProduct(ControlPointLocations[i] - ControlPointLocations[0], ControlPointLocations[i + 1] - ControlPointLocations[0]);
                    area += Vector3.DotProduct(product, normal);
                }

                area *= 0.5;
                return Math.Abs(area);
            }
        }

        /// <summary>
        /// Creates a rectangle on a plane.<br/>
        /// The plane is located at the centre of the rectangle.
        /// </summary>
        /// <param name="plane">The plane on where the rectangle will be created.</param>
        /// <param name="xDimension">The value dimension of the rectangle along the x direction of the plane.</param>
        /// <param name="yDimension">The value dimension of the rectangle along the y direction of the plane.</param>
        /// <returns></returns>
        public static Polygon Rectangle(Plane plane, double xDimension, double yDimension)
        {
            double xDimHalf = xDimension / 2;
            double yDimHalf = yDimension / 2;
            Point3 pt0 = plane.PointAt(-xDimHalf, -yDimHalf);
            Point3 pt1 = plane.PointAt(xDimHalf, -yDimHalf);
            Point3 pt2 = plane.PointAt(xDimHalf, yDimHalf);
            Point3 pt3 = plane.PointAt(-xDimHalf, yDimHalf);

            return new Polygon(new List<Point3> { pt0, pt1, pt2, pt3, pt0 });
        }

        /// <summary>
        /// Creates a regular polygon, inscribed into a circle.<br/>
        /// The plane is located at the centre of the polygon.
        /// </summary>
        /// <param name="plane">The plane on where the polygon will be created.</param>
        /// <param name="radius">The distance from the center to the corners of the polygon.</param>
        /// <param name="numberOfSegments">Number of segments of the polygon.</param>
        /// <returns></returns>
        public static Polygon RegularPolygon(Plane plane, double radius, int numberOfSegments)
        {
            if (numberOfSegments < 3)
            {
                throw new Exception("Polygon mast have at least 3 sides.");
            }
            if (radius <= 0.0)
            {
                throw new Exception("Polygon radius cannot be less or equal zero.");
            }
            Point3[] pts = new Point3[numberOfSegments + 1];
            double t = 2.0 * Math.PI / (double)numberOfSegments;
            for (int i = 0; i < numberOfSegments; i++)
            {
                var ty = Math.Sin(t * i) * radius;
                var tx = Math.Cos(t * i) * radius;
                pts[i] = plane.PointAt(tx, ty);
            }

            pts[pts.Length - 1] = pts[0];
            return new Polygon(pts);
        }

        /// <summary>
        /// Applies a transformation to all the points.
        /// </summary>
        /// <param name="transform">Transformation matrix to apply.</param>
        /// <returns>A polygon transformed.</returns>
        public new Polygon Transform(TransformMatrix transform)
        {
            List<Point3> transformedPts = ControlPointLocations.Select(pt => pt.Transform(transform)).ToList();

            return new Polygon(transformedPts);
        }
    }
}
