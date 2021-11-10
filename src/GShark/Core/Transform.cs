using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GShark.Enumerations;

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
        /// <param name="from">First point.</param>
        /// <param name="to">Second point.</param>
        /// <returns></returns>
        public static TransformMatrix Translation(Point3 from, Point3 to)
        {
            return Translation(to - from);
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
        /// <param name="center">Center point of rotation.</param>
        /// <param name="rotationAxis">One of the standard rotation axes. X, Y, or Z.</param>
        /// <returns>The rotation matrix.</returns>
        public static TransformMatrix Rotation(double theta, Point3 center, RotationAxis rotationAxis = RotationAxis.Z)
        {
            switch (rotationAxis)
            {
                case (RotationAxis.Z):
                    return Rotation(theta, center, Vector3.ZAxis);
                case (RotationAxis.Y):
                    return Rotation(theta, center, Vector3.YAxis);
                case (RotationAxis.X):
                    return Rotation(theta, center, Vector3.XAxis);
                default:
                    throw new ArgumentException("Rotation axis must be X, Y, or Z.");
            }
        }

        /// <summary>
        /// Constructs a new rotation transformation with Sin and Cos radians angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="centerPoint">Rotation center.</param>
        /// <param name="axis">Axis direction.</param>
        /// <returns>The rotation matrix.</returns>
        public static TransformMatrix Rotation(double theta, Point3 centerPoint, Vector3 axis)
        {
            //T(x,y)∗R∗T(−x,−y)(P)
            var origin = new Point3(0, 0, 0);
            var rotationMatrix = TransformMatrix.Rotation(axis, theta);

            if (centerPoint == origin)
            {
                return rotationMatrix;
            }

            var translatePointToOrigin = Translation(centerPoint, origin);
            var translateOriginToPoint = Translation(origin, centerPoint);
            var result = translatePointToOrigin.Combine(rotationMatrix).Combine(translateOriginToPoint);

            return result;
        }

        /// <summary>
        /// Creates a uniform scale transformation matrix with the origin as the fixed point.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns>The scale matrix.</returns>
        public static TransformMatrix Scale(double factor)
        {
            return Scale(new Point3(0, 0, 0), factor);
        }

        /// <summary>
        /// Creates a uniform scale transformation matrix that scales from a given center point.
        /// </summary>
        /// <param name="centerPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>The scale matrix.</returns>
        public static TransformMatrix Scale(Point3 centerPoint, double scaleFactor)
        {
            return Scale(centerPoint, scaleFactor, scaleFactor, scaleFactor);
        }

        /// <summary>
        /// Creates non-uniform scale transformation matrix that scales from a given center point.
        /// </summary>
        /// <param name="centerPoint">The anchor point from the scale transformation is computed.</param>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>The scale matrix.</returns>
        public static TransformMatrix Scale(Point3 centerPoint, double factorX, double factorY, double factorZ)
        {
            var scaleMatrix = new TransformMatrix
            {
                M00 = factorX,
                M11 = factorY,
                M22 = factorZ
            };

            var origin = new Point3(0, 0, 0);
            var translationToOrigin = TransformMatrix.Translation(origin - centerPoint);
            // Translating point (0,0,0) to pt1
            var translationToCentrePoint = TransformMatrix.Translation(centerPoint - origin);
            var result =  translationToCentrePoint * scaleMatrix * translationToOrigin;

            return result;
        }

        /// <summary>
        /// Creates non-uniform scale transformation matrix that scales from the world origin.
        /// </summary>
        /// <param name="factorX">Scale factor x direction.</param>
        /// <param name="factorY">Scale factor y direction.</param>
        /// <param name="factorZ">Scale factor z direction.</param>
        /// <returns>The scale matrix.</returns>
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
        /// Creates a plane to plane transformation matrix.
        /// </summary>
        /// <param name="a">The plane to orient from.</param>
        /// <param name="b">The plane to orient to.</param>
        /// <returns>The transformation matrix.</returns>
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
    }
}
