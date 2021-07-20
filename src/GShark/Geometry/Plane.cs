using GShark.Core;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Text;

namespace GShark.Geometry
{
    /// <summary>
    /// A plane is simply an origin point and normal.
    /// </summary>
    public class Plane : IEquatable<Plane>, ITransformable<Plane>
    {
        /// <summary>
        /// Constructs a plane from a origin and a direction.
        /// </summary>
        /// <param name="origin">The point describing the origin of the plane.</param>
        /// <param name="direction">The vector representing the normal of the plane.</param>
        public Plane(Point3 origin, Vector3 direction)
        {
            ZAxis = direction.Unitize();
            XAxis = Vector3.PerpendicularTo(ZAxis).Unitize();
            YAxis = Vector3.CrossProduct(ZAxis, XAxis).Unitize();
            Origin = origin;
        }

        /// <summary>
        /// Constructs a plane from three non-collinear points.
        /// </summary>
        /// <param name="pt1">Firs point representing the origin.</param>
        /// <param name="pt2">Second point representing the x direction.</param>
        /// <param name="pt3">Third point representing the y direction.</param>
        public Plane(Point3 pt1, Point3 pt2, Point3 pt3)
        {
            if(Trigonometry.ArePointsCollinear(pt1, pt2, pt3))
            {
                throw new Exception("Plane cannot be created, the tree points must not be collinear");
            }

            Vector3 dir1 = pt2 - pt1;
            Vector3 dir2 = pt3 - pt1;
            Vector3 normal = Vector3.CrossProduct(dir1, dir2);

            Origin = pt1;
            XAxis = dir1.Unitize();
            YAxis = Vector3.CrossProduct(normal, dir1).Unitize();
        }

        /// <summary>
        /// Constructs a plane from an point and two directions.
        /// </summary>
        /// <param name="origin">Point representing the origin.</param>
        /// <param name="xDirection">X direction.</param>
        /// <param name="yDirection">Y direction.</param>
        public Plane(Point3 origin, Vector3 xDirection, Vector3 yDirection)
        {
            Origin = origin;
            XAxis = xDirection.IsUnitVector ? xDirection : xDirection.Unitize();
            YAxis = yDirection.IsUnitVector ? yDirection : yDirection.Unitize();
            
        }

        /// <summary>
        /// Constructs a plane from another plane.
        /// </summary>
        /// <param name="plane">Input plane.</param>
        public Plane(Plane plane)
        {
            Origin = plane.Origin;
            XAxis = plane.XAxis;
            YAxis = plane.YAxis;

        }

        /// <summary>
        /// Gets a XY plane.
        /// </summary>
        public static Plane PlaneXY => new Plane(Point3.Origin, Vector3.XAxis, Vector3.YAxis);

        /// <summary>
        /// Gets a YZ plane.
        /// </summary>
        public static Plane PlaneYZ => new Plane(Point3.Origin, Vector3.YAxis, Vector3.ZAxis);

        /// <summary>
        /// Gets a XY plane.
        /// </summary>
        public static Plane PlaneZX => new Plane(Point3.Origin, Vector3.ZAxis, Vector3.XAxis);

        /// <summary>
        /// Gets a plane whose components are Unset.
        /// </summary>
        public static Plane Unset => new Plane(Point3.Unset, Vector3.Unset, Vector3.Unset);

        /// <summary>
        /// Gets the normal of the plane.
        /// </summary>
        public Vector3 Normal => ZAxis;

        /// <summary>
        /// Gets the origin of the plane.
        /// </summary>
        public Point3 Origin { get; set; }

        /// <summary>
        /// Gets the XAxis of the plane.
        /// </summary>
        public Vector3 XAxis { get; set; }

        /// <summary>
        /// Gets the YAxis of the plane.
        /// </summary>
        public Vector3 YAxis { get; set; }

        /// <summary>
        /// Gets the ZAxis of the plane.
        /// </summary>
        public Vector3 ZAxis => Vector3.CrossProduct(XAxis, YAxis).Unitize();

        /// <summary>
        /// Returns true if origin and axis vectors are valid and orthogonal.
        /// </summary>
        public bool IsValid
        {
            get
            {
                //check all axes and origin are valid
                var areAxesValid = Origin.IsValid && XAxis.IsValid && YAxis.IsValid && ZAxis.IsValid;

                //check that vectors are orthogonal
                var areAxesOrthogonal = Vector3.DotProduct(XAxis, YAxis) == 0 && Vector3.DotProduct(YAxis, ZAxis) == 0;

                if (areAxesValid && areAxesOrthogonal) return true;
                
                return false;
            }
        }

        /// <summary>
        /// Finds the closest point on a plane.
        /// https://www.parametriczoo.com/index.php/2020/02/29/signed-distance-of-a-point-from-a-plane/
        /// </summary>
        /// <param name="pt">The point to get close to plane.</param>
        /// <param name="distance">The signed distance of point from the plane. If the point is above the plane (positive side) the result is positive, if the point is below the result is negative.</param>
        /// <returns>The point on the plane that is closest to the sample point.</returns>
        public Point3 ClosestPoint(Vector3 pt, out double distance)
        {
            Vector3 ptToOrigin = Origin - pt;

            // signed distance.
            distance = Vector3.DotProduct(ptToOrigin, Normal);
            Point3 projection = pt + Normal * distance;

            return projection;
        }

        /// <summary>
        /// Performs the rotation to align the XAxis of a plane to a given guide vector.
        /// </summary>
        /// <param name="direction">The guide vector.</param>
        /// <returns>The rotated plane (in radians) with XAxis align to the guide vector.</returns>
        public Plane Align(Vector3 direction)
        {
            Point3 tempPt = Origin + direction;

            (double u, double v) = ClosestParameters(tempPt);
            double angle = -(Math.Atan2(u, v)) + Math.PI / 2.0;

            return Rotate(angle);
        }

        /// <summary>
        /// Calculates the parameters of a point on the plane closest to the test point.
        /// </summary>
        /// <param name="pt">Test point, the point to get close to.</param>
        /// <returns>The u parameter is along X-direction and v parameter is along the Y-direction.</returns>
        public (double u, double v) ClosestParameters(Point3 pt)
        {
            Vector3 v1 = pt - Origin;
            double u = Vector3.DotProduct(v1, XAxis);
            double v = Vector3.DotProduct(v1, YAxis);
            return (u, v);
        }

        /// <summary>
        /// Evaluates a point on the plane.
        /// </summary>
        /// <param name="u">Evaluation parameter.</param>
        /// <param name="v">Evaluation parameter.</param>
        /// <returns>The evaluated point.</returns>
        public Point3 PointAt(double u, double v)
        {
            return Origin + XAxis * u + YAxis * v;
        }

        /// <summary>
        /// Flip plane by swapping X and Y axes.
        /// </summary>
        /// <returns>The flipped plane.</returns>
        public Plane Flip()
        {
                var xAxis = YAxis;
                var yAxis = XAxis;
                return  new Plane(Origin, xAxis, yAxis);
        }

        /// <summary>
        /// Changes the origin of a plane.
        /// </summary>
        /// <param name="origin">The new origin point of a plane.</param>
        /// <returns>The plane with the new origin.</returns>
        public Plane SetOrigin(Point3 origin)
        {
            return new Plane(origin, XAxis, YAxis);
        }

        /// <summary>
        /// Fits a plane through a set of points.
        /// http://www.ilikebigbits.com/2015_03_04_plane_from_points.html
        /// </summary>
        /// <param name="pts">Points to fit.</param>
        /// <param name="deviation">Maximum deviation between the points and the plane.</param>
        /// <returns>The defined plane generated.</returns>
        public static Plane FitPlane(IList<Point3> pts, out double deviation)
        {
            if (pts.Count < 3)
            {
                throw new Exception("The collection must have minimum three points.");
            }

            Point3 centroid = Evaluation.CentroidByVertices(pts);
            Vector3 normal = Vector3.Unset;

            double xx = 0.0; double xy = 0.0; double xz = 0.0;
            double yy = 0.0; double yz = 0.0; double zz = 0.0;

            foreach (var pt in pts)
            {
                Vector3 tempDir = pt - centroid;

                xx += tempDir[0] * tempDir[0];
                xy += tempDir[0] * tempDir[1];
                xz += tempDir[0] * tempDir[2];
                yy += tempDir[1] * tempDir[1];
                yz += tempDir[1] * tempDir[2];
                zz += tempDir[2] * tempDir[2];
            }

            double determinantX = yy * zz - yz * yz;
            double determinantY = xx * zz - xz * xz;
            double determinantZ = xx * yy - xy * xy;

            double determinantMax = Math.Max(determinantX, Math.Max(determinantY, determinantZ));

            if(determinantMax <= 0.0)
            {
                throw new Exception("The points don't span a plane.");
            }

            if (Math.Abs(determinantMax - determinantX) < GeoSharkMath.MaxTolerance)
            {
                normal[0] = determinantX;
                normal[1] = xz * yz - xy * zz;
                normal[2] = xy * yz - xz * yy;
            }
            else if (Math.Abs(determinantMax - determinantY) < GeoSharkMath.MaxTolerance)
            {
                normal[0] = xz * yz - xy * zz;
                normal[1] = determinantY;
                normal[2] = xy * xz - yz * xx;
            }
            else
            {
                normal[0] = xy * yz - xz * yy;
                normal[1] = xy * xz - yz * xx;
                normal[2] = determinantZ;
            }

            Plane plane = new Plane(centroid, normal);
            double maxDeviation = double.MinValue;

            foreach (var pt in pts)
            {
                var tempPt = plane.ClosestPoint(pt, out double tempLength);
                maxDeviation = Math.Max(maxDeviation, tempLength);
            }

            deviation = maxDeviation;
            return plane;
        }

        /// <summary>
        /// Rotates the plane around is own Z-axis.
        /// </summary>
        /// <param name="radiansAngle">Angle to rotate the plane, expressed in radians.</param>
        /// <returns>The plan rotated.</returns>
        public Plane Rotate(double radiansAngle)
        {
            Vector3 xRotate = XAxis.Rotate(ZAxis, radiansAngle);
            Vector3 yRotate = Vector3.CrossProduct(ZAxis, xRotate);

            return new Plane(Origin, xRotate, yRotate);
        }

        /// <summary>
        /// Transforms the plane by a transformation matrix.
        /// </summary>
        /// <param name="transformation">The transformation matrix to apply to the plane.</param>
        /// <returns>The transformed plane.</returns>
        public Plane Transform(Transform transformation)
        {
            Point3 transformedOrigin = Origin.Transform(transformation);

           var xDir = (Origin + XAxis).Transform(transformation) - transformedOrigin;
           var yDir = (Origin + YAxis).Transform(transformation) - transformedOrigin;

            return new Plane(transformedOrigin, xDir, yDir);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Plane is equal to this Plane instance.<br/>
        /// The plane are equals if they have same origin and axises.
        /// </summary>
        /// <param name="other">The Plane to compare this instance to.</param>
        /// <returns>True if the other Plane is equal to this instance; False otherwise.</returns>
        public bool Equals(Plane other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return false;
            }

            return (Origin.Equals(other.Origin) &&
                    XAxis.Equals(other.XAxis) &&
                    YAxis.Equals(other.YAxis) &&
                    ZAxis.Equals(other.ZAxis));
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code of the plane.</returns>
        public override int GetHashCode()
        {
            return Origin.GetHashCode() + Normal.GetHashCode();
        }

        /// <summary>
        /// Translate a plane into a readable format.
        /// </summary>
        /// <returns>The text format of a plane.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Default({Origin})");
            builder.AppendLine($"X({XAxis})");
            builder.AppendLine($"Y({YAxis})");
            builder.AppendLine($"Z({ZAxis})");

            return builder.ToString();
        }
    }
}
