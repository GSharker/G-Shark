using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GShark.Core
{
    /// <summary>
    /// A collection of transformation methods.
    /// </summary>
    public static class Transform
    {

        /// <summary>
        /// Constructs a transformation matrix representing a translation from pointA to pointB.
        /// </summary>
        /// <param name="pointA">First point.</param>
        /// <param name="pointB">Second point.</param>
        /// <returns></returns>
        public static TransformMatrix Translation(Point3 pointA, Point3 pointB)
        {
            return Translation(pointB - pointA);
        }

        /// <summary>
        /// Constructs a new translation transformation.
        /// </summary>
        /// <param name="v">Translation vector.</param>
        /// <returns>A transformation matrix which moves the geometry along the vector.</returns>
        public static TransformMatrix Translation(Vector3 v)
        {
            return Translation(v[0], v[1], v[2]);
        }

        /// <summary>
        /// Constructs a new translation transformation.
        /// </summary>
        /// <param name="x">Transform the x value.</param>
        /// <param name="y">Transform the y value.</param>
        /// <param name="z">Transform the z value.</param>
        /// <returns>A transformation matrix which moves the geometry along the vector.</returns>
        public static TransformMatrix Translation(double x, double y, double z)
        {
            var transform = new TransformMatrix
            {
                M30 = x,
                M31 = y,
                M32 = z
            };

            return transform;
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified radians angle, rotation center and rotation axis pointing up.
        /// </summary>
        /// <param name="theta">Angle in radians of the rotation.</param>
        /// <param name="center">Center point of rotation. Rotation axis is vertical.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        public static TransformMatrix Rotation(double theta, Point3 center)
        {
            return Rotation(Vector3.ZAxis, theta, center);
        }

        /// <summary>
        /// Constructs a new rotation transformation with Sin and Cos radians angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="axis">Axis direction.</param>
        /// <param name="theta"></param>
        /// <param name="centerPoint">Rotation center.</param>

        /// <returns>A transformation matrix which rotates geometry around an anchor.</returns>
        public static TransformMatrix Rotation(Vector3 axis, double theta, Point3 centerPoint)
        {
            //T(x,y)∗R∗T(−x,−y)(P)
            var origin = new Point3(0, 0, 0);
            var ptOriginVec = origin - centerPoint;
            var translatePointToOrigin = TransformMatrix.Translation(ptOriginVec);
            var translateOriginToPoint = TransformMatrix.Translation(ptOriginVec.Reverse());
            var rotationMatrix = TransformMatrix.Rotation(axis, theta);
            var result = translateOriginToPoint * rotationMatrix * translatePointToOrigin;

            return result;
        }

        /// <summary>
        /// Creates a uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static TransformMatrix Scale(double factor)
        {
            return Scale(new Point3(0, 0, 0), factor);
        }

        /// <summary>
        /// Creates a uniform scale transformation matrix with a given center point.
        /// </summary>
        /// <param name="centerPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static TransformMatrix Scale(Point3 centerPoint, double scaleFactor)
        {
            return Scale(centerPoint, scaleFactor, scaleFactor, scaleFactor);
        }

        /// <summary>
        /// Creates non uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="centerPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static TransformMatrix Scale(Point3 centerPoint, double factorX, double factorY, double factorZ)
        {
            var scaleMatrix = new TransformMatrix
            {
                M00 = factorX,
                M11 = factorY,
                M22 = factorZ
            };

            var translationVector = new Vector3(centerPoint.X * factorX, centerPoint.Y * factorY, centerPoint.Z * factorZ);
            var translation = TransformMatrix.Translation(translationVector);
            var result =  translation * scaleMatrix;

            return result;
        }

        /// <summary>
        /// Creates non uniform scale transformation matrix with the anchor point in the origin.
        /// </summary>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>Scale transformation matrix where the diagonal is (factorX, factorY, factorZ, 1)</returns>
        public static TransformMatrix Scale(double factorX, double factorY, double factorZ)
        {
            var result = new TransformMatrix
            {
                M00 = factorX,
                M11 = factorY,
                M22 = factorZ
            };
            return result;
        }

       /// <summary>
        /// Creates a transformation that orients a planeA to a planeB.
        /// </summary>
        /// <param name="a">The plane to orient from.</param>
        /// <param name="b">The plane to orient to.</param>
        /// <returns>The translation transformation.</returns>
        public static TransformMatrix PlaneToPlane(Plane a, Plane b)
        {
            var pt0 = a.Origin;
            var x0 = a.XAxis;
            var y0 = a.YAxis;
            var z0 = a.ZAxis;

            var pt1 = b.Origin;
            var x1 = b.XAxis;
            var y1 = b.YAxis;
            var z1 = b.ZAxis;

            var origin = new Point3(0, 0, 0);

            // Translating point pt0 to (0,0,0)
            var translationPlnAToOrigin = TransformMatrix.Translation(origin - pt0);
            // Translating point (0,0,0) to pt1
            var translationOriginToPlnB = TransformMatrix.Translation(pt1 - origin);
            
            //plane a as 4x4 transform matrix with axes as column vectors
            var mapA = new TransformMatrix
            {
                M00 = x0[0],
                M10 = y0[0],
                M20 = z0[0],
                M01 = x0[1],
                M11 = y0[1],
                M21 = z0[1],
                M02 = x0[2],
                M12 = y0[2],
                M22 = z0[2]
            };

            //plane b as 4x4 transform matrix with axes as column vectors
            var mapB = new TransformMatrix
            {
                M00 = x1[0],
                M10 = y1[0],
                M20 = z1[0],
                M01 = x1[1],
                M11 = y1[1],
                M21 = z1[1],
                M02 = x1[2],
                M12 = y1[2],
                M22 = z1[2]
            };

            //Transpose plane a matrix. Square matrix transpose same as inverse but cheaper.
            var mapATransposed = mapA.Transpose();
            var result = translationOriginToPlnB *mapB * mapATransposed * translationPlnAToOrigin;

            return result;
        }

        /// <summary>
        /// Combines two transformations.<br/>
        /// This is the same as the * operator.
        /// </summary>
        /// <param name="t">Transformation to combine.</param>
        /// <returns>Transformation combined.</returns>
        public static TransformMatrix Combine(TransformMatrix a, TransformMatrix b)
        {
            return b * a;
        }

        /// <summary>
        /// Finds the rotation axis used in the transformation.<br/>
        /// https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/index.htm
        /// </summary>
        /// <param name="transform">Transformation to check.</param>
        /// <returns>The rotation axis used for the transformation.</returns>
        public static Vector3 GetRotationAxis(TransformMatrix transform)
        {
            throw new NotImplementedException();
            //ToDo Implement with TransformationMatrix indexing
            //Vector3 axis = Vector3.Unset;

            //if (Math.Abs(transform[0][1] + transform[1][0]) < GSharkMath.MinTolerance ||
            //    Math.Abs(transform[0][2] + transform[2][0]) < GSharkMath.MinTolerance ||
            //    Math.Abs(transform[1][2] + transform[2][1]) < GSharkMath.MinTolerance)
            //{
            //    double xx = (transform[0][0] + 1) / 2;
            //    double yy = (transform[1][1] + 1) / 2;
            //    double zz = (transform[2][2] + 1) / 2;
            //    double xy = (transform[0][1] + transform[1][0]) / 4;
            //    double xz = (transform[0][2] + transform[2][0]) / 4;
            //    double yz = (transform[1][2] + transform[2][1]) / 4;

            //    if ((xx > yy) && (xx > zz))
            //    { // m[0][0] is the largest diagonal term
            //        if (xx < GSharkMath.MinTolerance)
            //        {
            //            axis[0] = 0;
            //            axis[1] = 0.7071;
            //            axis[2] = 0.7071;
            //        }
            //        else
            //        {
            //            axis[0] = Math.Sqrt(xx);
            //            axis[1] = xy / axis[0];
            //            axis[2] = xz / axis[0];
            //        }
            //    }
            //    else if (yy > zz)
            //    { // m[1][1] is the largest diagonal term
            //        if (yy < GSharkMath.MinTolerance)
            //        {
            //            axis[0] = 0.7071;
            //            axis[1] = 0;
            //            axis[2] = 0.7071;
            //        }
            //        else
            //        {
            //            axis[1] = Math.Sqrt(yy);
            //            axis[0] = xy / axis[1];
            //            axis[2] = yz / axis[1];
            //        }
            //    }
            //    else
            //    { // m[2][2] is the largest diagonal term so base result on this
            //        if (zz < GSharkMath.MinTolerance)
            //        {
            //            axis[0] = 0.7071;
            //            axis[1] = 0.7071;
            //            axis[2] = 0;
            //        }
            //        else
            //        {
            //            axis[2] = Math.Sqrt(zz);
            //            axis[0] = xz / axis[2];
            //            axis[1] = yz / axis[2];
            //        }
            //    }
            //    return axis; // return 180 deg rotation
            //}

            //double v = Math.Sqrt(Math.Pow(transform[2][1] - transform[1][2], 2) + Math.Pow(transform[0][2] - transform[2][0], 2) + Math.Pow(transform[1][0] - transform[0][1], 2));

            //axis[0] = (transform[2][1] - transform[1][2]) / v;
            //axis[1] = (transform[0][2] - transform[2][0]) / v;
            //axis[2] = (transform[1][0] - transform[0][1]) / v;

            //return axis;
        }
    }
}
