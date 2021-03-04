using GeometrySharp.Core;
using System;
using System.Collections.Generic;
using System.Text;
using GeometrySharp.Operation;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A Plane is simply an origin point and normal.
    /// </summary>
    public class Plane : IEquatable<Plane>
    {
        /// <summary>
        /// Construct a plane from a origin and a direction.
        /// </summary>
        /// <param name="origin">The point describing the origin of the plane.</param>
        /// <param name="direction">The vector representing the normal of the plane.</param>
        public Plane(Vector3 origin, Vector3 direction)
        {
            ZAxis = direction.Unitize();
            XAxis = Vector3.XAxis.PerpendicularTo(ZAxis).Unitize();
            YAxis = Vector3.Cross(ZAxis, XAxis).Unitize();
            Origin = origin;
        }

        /// <summary>
        /// Construct a plane from three non-collinear points.
        /// </summary>
        /// <param name="pt1">Firs point representing the origin.</param>
        /// <param name="pt2">Second point representing the x direction.</param>
        /// <param name="pt3">Third point representing the y direction.</param>
        public Plane(Vector3 pt1, Vector3 pt2, Vector3 pt3)
        {
            if(LinearAlgebra.Orientation(pt1, pt2, pt3) == 0)
            {
                throw new Exception("Plane cannot be created, the tree points must not be collinear");
            }

            Vector3 dir1 = pt2 - pt1;
            Vector3 dir2 = pt3 - pt1;
            Vector3 normal = Vector3.Cross(dir1, dir2);

            Origin = pt1;
            XAxis = dir1.Unitize();
            YAxis = Vector3.Cross(normal, dir1).Unitize();
            ZAxis = normal.Unitize();
        }

        /// <summary>
        /// Construct a plane from an point and two directions.
        /// </summary>
        /// <param name="origin">Point representing the origin.</param>
        /// <param name="xDirection">X direction.</param>
        /// <param name="yDirection">Y direction.</param>
        /// <param name="zDirection">Z direction.</param>
        public Plane(Vector3 origin, Vector3 xDirection, Vector3 yDirection, Vector3 zDirection)
        {
            Origin = origin;
            XAxis = xDirection.Unitize();
            YAxis = yDirection.Unitize();
            ZAxis = zDirection.Unitize();
        }

        /// <summary>
        /// Get a XY plane.
        /// </summary>
        public static Plane PlaneXY => new Plane(new Vector3 { 0.0, 0.0, 0.0 }, Vector3.ZAxis);

        /// <summary>
        /// Get a YZ plane.
        /// </summary>
        public static Plane PlaneYZ => new Plane(new Vector3 { 0.0, 0.0, 0.0 }, Vector3.XAxis);

        /// <summary>
        /// Get a XY plane.
        /// </summary>
        public static Plane PlaneXZ => new Plane(new Vector3 { 0.0, 0.0, 0.0 }, Vector3.YAxis);

        /// <summary>
        /// The normal of the plan.
        /// </summary>
        public Vector3 Normal => ZAxis;

        /// <summary>
        /// The origin of the plane.
        /// </summary>
        public Vector3 Origin { get; }

        /// <summary>
        /// The XAxis of the plane.
        /// </summary>
        public Vector3 XAxis { get; }

        /// <summary>
        /// The YAxis of the plane.
        /// </summary>
        public Vector3 YAxis { get; }

        /// <summary>
        /// The ZAxis of the plane.
        /// </summary>
        public Vector3 ZAxis { get; }

        /// <summary>
        /// Finds the closest point on a plane.
        /// </summary>
        /// <param name="pt">The point to get close to plane.</param>
        /// <param name="length">The distance between the point and his projection.</param>
        /// <returns>The point on the plane that is closest to the sample point.</returns>
        public Vector3 ClosestPoint(Vector3 pt, out double length)
        {
            Vector3 ptToOrigin = Origin - pt;

            Vector3 projection = Normal * (Vector3.Dot(ptToOrigin, Normal));

            length = projection.Length();
            return pt + projection;
        }

        /// <summary>
        /// Perform the rotation to align the XAxis of a plane to a given guide vector.
        /// </summary>
        /// <param name="direction">The guide vector.</param>
        /// <returns>The rotated plane with XAxis align to the guide vector.</returns>
        public Plane Align(Vector3 direction)
        {
            Vector3 tempPt = this.Origin + direction;
            Vector3 tempDir = tempPt - this.Origin;

            double u = Vector3.Dot(this.XAxis, tempDir);
            double v = Vector3.Dot(this.YAxis, tempDir);

            double angle = -(Math.Atan2(u, v)) + Math.PI / 2.0;

            return this.Rotate(angle);
        }

        /// <summary>
        /// Swapping out the X and Y axes and inverting the Z axis.
        /// </summary>
        /// <returns>The flipped plane.</returns>
        public Plane Flip()
        {
            Vector3 zDir = Vector3.Reverse(Normal);
            return  new Plane(Origin, YAxis, XAxis, zDir);
        }

        /// <summary>
        /// Changes the origin of a plane.
        /// </summary>
        /// <param name="origin">The new origin point of a plane.</param>
        /// <returns>The plane with the new origin.</returns>
        public Plane SetOrigin(Vector3 origin)
        {
            return new Plane(origin, this.XAxis, this.YAxis, this.ZAxis);
        }

        /// <summary>
        /// Fits a plane through a set of points.
        /// http://www.ilikebigbits.com/2015_03_04_plane_from_points.html
        /// </summary>
        /// <param name="pts">Points to fit.</param>
        /// <param name="deviation">Maximum deviation between the points and the plane.</param>
        /// <returns>The defined plane generated.</returns>
        public static Plane FitPlane(IList<Vector3> pts, out double deviation)
        {
            if (pts.Count < 3)
                throw new Exception("The collection must have minimum three points.");

            Vector3 centroid = Evaluation.CentroidByVertices(pts);
            Vector3 normal = Vector3.Unset;

            double xx = 0.0; double xy = 0.0; double xz = 0.0;
            double yy = 0.0; double yz = 0.0; double zz = 0.0;

            foreach (Vector3 pt in pts)
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
                throw new Exception("The points don't span a plane.");

            if (Math.Abs(determinantMax - determinantX) < GeoSharpMath.MAXTOLERANCE)
            {
                normal[0] = determinantX;
                normal[1] = xz * yz - xy * zz;
                normal[2] = xy * yz - xz * yy;
            }
            else if (Math.Abs(determinantMax - determinantY) < GeoSharpMath.MAXTOLERANCE)
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

            foreach (Vector3 pt in pts)
            {
                Vector3 tempPt = plane.ClosestPoint(pt, out double tempLength);
                maxDeviation = Math.Max(maxDeviation, tempLength);
            }

            deviation = maxDeviation;
            return plane;
        }

        /// <summary>
        /// Rotates the plane around is own Z-axis.
        /// </summary>
        /// <param name="angle">Angle to rotate the plane, expressed in radians.</param>
        /// <returns>The plan rotated.</returns>
        public Plane Rotate(double angle)
        {
            Vector3 xRotate = this.XAxis.Rotate(this.ZAxis, angle);
            Vector3 yRotate = Vector3.Cross(this.ZAxis, xRotate);

            return new Plane(this.Origin, xRotate, yRotate, this.ZAxis);
        }

        /// <summary>
        /// Transforms the plane by a transformation matrix.
        /// </summary>
        /// <param name="t">The transformation matrix to apply to the plane.</param>
        /// <returns>The transformed plane.</returns>
        public Plane Transform(Transform t)
        {
            Vector3 origin = Origin * t;

            bool check = (Math.Abs(t[3][0]) <= GeoSharpMath.MAXTOLERANCE &&
                          Math.Abs(t[3][1]) <= GeoSharpMath.MAXTOLERANCE &&
                          Math.Abs(t[3][2]) <= GeoSharpMath.MAXTOLERANCE &&
                          Math.Abs(1.0 - t[3][3]) <= GeoSharpMath.MAXTOLERANCE);

            Vector3 xDir = check ? ((Origin + XAxis) * t) - origin : XAxis * t;
            Vector3 yDir = check ? ((Origin + YAxis) * t) - origin : YAxis * t;
            Vector3 zDir = check ? ((Origin + ZAxis) * t) - origin : ZAxis * t;

            return new Plane(origin, xDir, yDir, zDir);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given Planes are equal.
        /// </summary>
        /// <param name="plane1">The first Plane to compare.</param>
        /// <param name="plane2">The second Plane to compare.</param>
        /// <returns>True if the Planes are equal; False otherwise.</returns>
        public static bool operator ==(Plane plane1, Plane plane2)
        {
            return Equals(plane1, plane2);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given Planes are not equal.
        /// </summary>
        /// <param name="plane1">The first Plane to compare.</param>
        /// <param name="plane2">The second Plane to compare.</param>
        /// <returns>True if the Planes are not equal; False if they are equal.</returns>
        public static bool operator !=(Plane plane1, Plane plane2)
        {
            return !Equals(plane1, plane2);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Plane is equal to this Plane instance.
        /// </summary>
        /// <param name="other">The Plane to compare this instance to.</param>
        /// <returns>True if the other Plane is equal to this instance; False otherwise.</returns>
        public bool Equals(Plane other)
        {
            return other != null && (Origin.Equals(other.Origin) &&
                                     XAxis.Equals(other.XAxis) &&
                                     YAxis.Equals(other.YAxis) &&
                                     ZAxis.Equals(other.ZAxis));
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this Plane instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this Plane; False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Plane plane)
            {
                return Equals(plane);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
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

            builder.AppendLine($"Origin({Origin})");
            builder.AppendLine($"X({XAxis})");
            builder.AppendLine($"Y({YAxis})");
            builder.AppendLine($"Z({ZAxis})");

            return builder.ToString();
        }
    }
}
