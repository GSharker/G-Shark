﻿using GShark.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    public class Point4 : IEquatable<Point4>
    {
        public Point4()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4"/> class based on coordinates.
        /// </summary>
        /// <param name="x">The X (first) dimension.</param>
        /// <param name="y">The Y (second) dimension.</param>
        /// <param name="z">The Z (third) dimension.</param>
        /// <param name="w">The W (fourth) dimension, or weight.</param>
        public Point4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4"/> class from the coordinates of a point.
        /// </summary>
        /// <param name="point">Coordinates of the control point.</param>
        public Point4(Point3 point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
            W = 1.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point4"/> class from the coordinates of a point and a weight.
        /// </summary>
        /// <param name="point">Coordinates of the control point.</param>
        /// <param name="weight">Weight factor of the control point. You should not use weights less than or equal to zero.</param>
        public Point4(Point3 point, double weight)
        {
            W = (weight <= 0.0) ? 1.0 : weight;
            X = point.X * W;
            Y = point.Y * W;
            Z = point.Z * W;
        }

        /// <summary>
        /// Gets or sets the X (first) coordinate of this control point.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y (second) coordinate of this control point.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the Z (third) coordinate of this control point.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Gets or sets the W (fourth) coordinate -or weight- of this control point.
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Dimension of point.
        /// </summary>
        public int Size => 4;

        /// <summary>
        /// Gets the value of a control point with all coordinates set as RhinoMath.UnsetValue.
        /// </summary>
        public static Point4 Unset => new Point4(GSharkMath.UnsetValue, GSharkMath.UnsetValue, GSharkMath.UnsetValue, GSharkMath.UnsetValue);

        /// <summary>
        /// Each coordinate of the a control point must pass the <see cref="GSharkMath.IsValidDouble"/> test.
        /// </summary>
        public bool IsValid => GSharkMath.IsValidDouble(X) && GSharkMath.IsValidDouble(Y) && GSharkMath.IsValidDouble(Z);

        /// <summary>
        /// Gets the value of a control point with all coordinates set as zero.
        /// </summary>
        public static Point4 Zero => new Point4(0, 0, 0, 0);

        //Indexer to allow access to properties as array.
        public double this[int i]
        {
            get
            {
                return i switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    3 => W,
                    _ => throw new IndexOutOfRangeException()
                };
            }
            set
            {
                if (i < 0 || i > 3) throw new IndexOutOfRangeException();
                switch (i)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Transforms the control point using a transformation matrix.
        /// </summary>
        /// <param name="t">The transformation matrix.</param>
        /// <returns>The transformed control point as a new instance.</returns>
        public Point4 Transform(Transform t)
        {
            double num1 = t[0][0] * X + t[0][1] * Y + t[0][2] * Z + t[0][3] * W;
            double num2 = t[1][0] * X + t[1][1] * Y + t[1][2] * Z + t[1][3] * W;
            double num3 = t[2][0] * X + t[2][1] * Y + t[2][2] * Z + t[2][3] * W;
            double num4 = t[3][0] * X + t[3][1] * Y + t[3][2] * Z + t[3][3] * W;

            return new Point4(num1, num2, num3, num4);
        }

        /// <summary>
        /// Sums two <see cref="Point4"/> together.
        /// </summary>
        /// <param name="ctrlPoint1">First control point.</param>
        /// <param name="ctrlPoint2">Second control point.</param>
        /// <returns>A new control point that result of the addition of ctrlPoint1 and ctrlPoint2.</returns>
        public static Point4 operator +(Point4 ctrlPoint1, Point4 ctrlPoint2)
        {
            Point4 result = ctrlPoint1; //copy of the value
            result.X += ctrlPoint2.X;
            result.Y += ctrlPoint2.Y;
            result.Z += ctrlPoint2.Z;
            result.W += ctrlPoint2.W;
            return result;
        }

        /// <summary>
        /// Calculates the weighted addition of two <see cref="Point4"/> together.
        /// </summary>
        /// <param name="ctrlPoint1">First control point.</param>
        /// <param name="ctrlPoint2">Second control point.</param>
        /// <returns>A new control point that results from the weighted addition of ctrlPoint1 and ctrlPoint2.</returns>

        public static Point4 WeightedAddiction(Point4 ctrlPoint1, Point4 ctrlPoint2)
        {
            Point4 result = ctrlPoint1; //copy of the value
            if (ctrlPoint2.W == ctrlPoint1.W)
            {
                result.X += ctrlPoint2.X;
                result.Y += ctrlPoint2.Y;
                result.Z += ctrlPoint2.Z;
            }
            else if (ctrlPoint2.W == 0)
            {
                result.X += ctrlPoint2.X;
                result.Y += ctrlPoint2.Y;
                result.Z += ctrlPoint2.Z;
            }
            else if (ctrlPoint1.W == 0)
            {
                result.X += ctrlPoint2.X;
                result.Y += ctrlPoint2.Y;
                result.Z += ctrlPoint2.Z;
                result.W = ctrlPoint2.W;
            }
            else
            {
                double sw1 = (ctrlPoint1.W > 0.0) ? Math.Sqrt(ctrlPoint1.W) : -Math.Sqrt(-ctrlPoint1.W);
                double sw2 = (ctrlPoint2.W > 0.0) ? Math.Sqrt(ctrlPoint2.W) : -Math.Sqrt(-ctrlPoint2.W);
                double s1 = sw2 / sw1;
                double s2 = sw1 / sw2;
                result.X = ctrlPoint1.X * s1 + ctrlPoint2.X * s2;
                result.Y = ctrlPoint1.Y * s1 + ctrlPoint2.Y * s2;
                result.Z = ctrlPoint1.Z * s1 + ctrlPoint2.Z * s2;
                result.W = sw1 * sw2;
            }
            return result;
        }

        /// <summary>
        /// Sums two <see cref="Point4"/> together.
        /// </summary>
        /// <param name="ctrlPoint1">First point.</param>
        /// <param name="ctrlPoint2">Second point.</param>
        /// <returns>A new point that result of the subtraction of ctrlPoint1 and ctrlPoint2.</returns>
        public static Point4 operator -(Point4 ctrlPoint1, Point4 ctrlPoint2)
        {
            Point4 result = ctrlPoint1; //copy of the value
            result.X -= ctrlPoint2.X;
            result.Y -= ctrlPoint2.Y;
            result.Z -= ctrlPoint2.Z;
            result.W -= ctrlPoint2.W;
            return result;
        }

        /// <summary>
        /// Subtracts the second point from the first point.
        /// </summary>
        /// <param name="ctrlPoint1">First point.</param>
        /// <param name="ctrlPoint2">Second point.</param>
        /// <returns>A new point that results from the weighted subtraction of ctrlPoint2 from ctrlPoint1.</returns>
        public static Point4 WeightedSubtraction(Point4 ctrlPoint1, Point4 ctrlPoint2)
        {
            Point4 result = ctrlPoint1; //copy of the value
            if (ctrlPoint2.W == ctrlPoint1.W)
            {
                result.X -= ctrlPoint2.X;
                result.Y -= ctrlPoint2.Y;
                result.Z -= ctrlPoint2.Z;
            }
            else if (ctrlPoint2.W == 0.0)
            {
                result.X -= ctrlPoint2.X;
                result.Y -= ctrlPoint2.Y;
                result.Z -= ctrlPoint2.Z;
            }
            else if (ctrlPoint1.W == 0.0)
            {
                result.X -= ctrlPoint2.X;
                result.Y -= ctrlPoint2.Y;
                result.Z -= ctrlPoint2.Z;
                result.W = ctrlPoint2.W;
            }
            else
            {
                double sw1 = (ctrlPoint1.W > 0.0) ? Math.Sqrt(ctrlPoint1.W) : -Math.Sqrt(-ctrlPoint1.W);
                double sw2 = (ctrlPoint2.W > 0.0) ? Math.Sqrt(ctrlPoint2.W) : -Math.Sqrt(-ctrlPoint2.W);
                double s1 = sw2 / sw1;
                double s2 = sw1 / sw2;
                result.X = ctrlPoint1.X * s1 - ctrlPoint2.X * s2;
                result.Y = ctrlPoint1.Y * s1 - ctrlPoint2.Y * s2;
                result.Z = ctrlPoint1.Z * s1 - ctrlPoint2.Z * s2;
                result.W = sw1 * sw2;
            }
            return result;
        }

        /// <summary>
        /// Computes the distance between two control points.
        /// </summary>
        /// <param name="other">Other control points for distance measurement.</param>
        /// <returns>The length of the line between this and the other control points; or 0 if any of the control points is not valid.</returns>
        public double DistanceTo(Point4 other)
        {
            Point3 pt1 = PointDehomogenizer(this);
            Point3 pt2 = PointDehomogenizer(other);
            return pt1.DistanceTo(pt2);
        }

        /// <summary>
        /// Multiplies a control point by a number.
        /// </summary>
        /// <param name="point">A control point.</param>
        /// <param name="d">A number.</param>
        /// <returns>A new control point that results from the coordinatewise multiplication of control point with d.</returns>
        public static Point4 operator *(Point4 point, double d)
        {
            return new Point4(point.X * d, point.Y * d, point.Z * d, point.W * d);
        }

        /// <summary>
        /// Divides a control point by a number.
        /// </summary>
        /// <param name="point">A control point.</param>
        /// <param name="d">A number.</param>
        /// <returns>A new control point that results from the coordinatewise division of control point with d.</returns>
        public static Point4 operator /(Point4 point, double d)
        {
            return new Point4(point.X / d, point.Y / d, point.Z / d, point.W / d);
        }

        /// <summary>
        /// Multiplies two <see cref="Point4"/> together, returning the dot (internal) product of the two.
        /// This is not the cross product.
        /// </summary>
        /// <param name="ctrlPoint1">The first control point.</param>
        /// <param name="ctrlPoint2">The second control point.</param>
        /// <returns>A value that results from the coordinatewise multiplication of ctrlPoint1 and ctrlPoint2.</returns>
        public static double operator *(Point4 ctrlPoint1, Point4 ctrlPoint2)
        {
            return (ctrlPoint1.X * ctrlPoint2.X) +
              (ctrlPoint1.Y * ctrlPoint2.Y) +
              (ctrlPoint1.Z * ctrlPoint2.Z) +
              (ctrlPoint1.W * ctrlPoint2.W);
        }

        /// <summary>
        /// Determines whether two control point have equal values.
        /// </summary>
        /// <param name="a">The first control point.</param>
        /// <param name="b">The second control point.</param>
        /// <returns>true if the coordinates of the two control points are equal; otherwise false.</returns>
        public static bool operator ==(Point4 a, Point4 b)
        {
            return Math.Abs(a.X - b.X) <= GSharkMath.Epsilon &&
                   Math.Abs(a.Y - b.Y) <= GSharkMath.Epsilon &&
                   Math.Abs(a.Z - b.Z) <= GSharkMath.Epsilon &&
                   Math.Abs(a.W - b.W) <= GSharkMath.Epsilon;
        }

        /// <summary>
        /// Determines whether two control point have different values.
        /// </summary>
        /// <param name="a">The first control point.</param>
        /// <param name="b">The second control point.</param>
        /// <returns>true if the two control points differ in any coordinate; false otherwise.</returns>
        public static bool operator !=(Point4 a, Point4 b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Converts a control point in a vector, without needing casting.
        /// </summary>
        /// <param name="point4">A control point.</param>
        /// <returns>The resulting Vector.</returns>
        public static implicit operator Vector(Point4 point4)
        {
            return new Vector { point4.X, point4.Y, point4.Z, point4.W };
        }

        /// <summary>
        /// Transforms a collection of control points into their homogeneous equivalents.<br/>
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints">Control points, a set of size (points count x points dimension).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (points count x 1).</param>
        /// <returns>A set of control points where each point is (wi*pi, wi)<br/>
        /// where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<Point4> PointsHomogeniser(List<Point3> controlPoints, List<double> weights)
        {
            if (controlPoints.Count < weights.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(weights),
                    "The weights set is bigger than the control points, it must be the same dimension");
            }

            if (controlPoints.Count > weights.Count)
            {
                int diff = controlPoints.Count - weights.Count;
                List<double> dataFilled = CollectionHelpers.RepeatData(1.0, diff);
                weights.AddRange(dataFilled);
            }

            List<Point4> controlPtsHomogenized = new List<Point4>();

            for (int i = 0; i < controlPoints.Count; i++)
            {
                Point4 tempPt = new Point4
                {
                    X = controlPoints[i].X * weights[i],
                    Y = controlPoints[i].Y * weights[i],
                    Z = controlPoints[i].Z * weights[i],
                    W = weights[i]
                };


                controlPtsHomogenized.Add(tempPt);
            }

            return controlPtsHomogenized;
        }

        /// <summary>
        /// Transforms a collection of control points into their homogeneous equivalents, by a given weight value.<br/>
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints">Control points, a set of size (points count x points dimension).</param>
        /// <param name="weight">Weight value for each point.</param>
        /// <returns>A set of control points where each point is (wi*pi, wi)<br/>
        /// where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<Point4> PointsHomogeniser(List<Point3> controlPoints, double weight)
        {
            List<Point4> controlPtsHomogenized = new List<Point4>();

            foreach (var pt in controlPoints)
            {
                Point4 tempPt = new Point4
                {
                    X = pt.X * weight,
                    Y = pt.Y * weight,
                    Z = pt.Z * weight,
                    W = weight
                };

                controlPtsHomogenized.Add(tempPt);
            }

            return controlPtsHomogenized;
        }

        /// <summary>
        /// Transforms a two-dimension collection of control points into their homogeneous equivalents.<br/>
        /// http://deltaorange.com/2012/03/08/the-truth-behind-homogenous-coordinates/
        /// </summary>
        /// <param name="controlPoints">Control points, a two-dimensional set of size (points count x points dimension).</param>
        /// <param name="weights">Control point weights, the same size as the set of control points (points count x 1).</param>
        /// <returns>A two-dimensional set of control points where each point is (wi*pi, wi)<br/>
        /// where wi the ith control point weight and pi is the ith control point, hence the dimension of the point is dim + 1.</returns>
        public static List<List<Point4>> PointsHomogeniser2d(List<List<Point3>> controlPoints, List<List<double>> weights = null)
        {
            int rows = controlPoints.Count;
            List<List<Point4>> controlPtsHomogenized = new List<List<Point4>>();
            List<List<double>> usedWeights = weights;
            if (weights == null || weights.Count == 0)
            {
                usedWeights = new List<List<double>>();
                for (int i = 0; i < rows; i++)
                {
                    usedWeights.Add(CollectionHelpers.RepeatData(1.0, controlPoints[i].Count));
                }
            }
            if (controlPoints.Count < usedWeights.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(weights), "The weights set is bigger than the control points, it must be the same dimension");
            }

            for (int i = 0; i < rows; i++)
            {
                controlPtsHomogenized.Add(PointsHomogeniser(controlPoints[i], usedWeights[i]));
            }

            return controlPtsHomogenized;
        }

        /// <summary>
        /// Obtains the weight from a collection of control points in homogeneous space, assuming all are the same dimension.
        /// </summary>
        /// <param name="homogenizedPoints">Control points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A set of values, represented by a set pi with length (dim).</returns>
        public static List<double> GetWeights(List<Point4> homogenizedPoints)
        {
            return homogenizedPoints.Select(pt => pt.W).ToList();
        }

        /// <summary>
        /// Obtains the weight from a two-dimensional collection of control points in homogeneous space, assuming all are the same dimension.
        /// </summary>
        /// <param name="homogenizedPoints">Two-dimensional set of control points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Two-dimensional set of values, each represented by an array pi with length (dim)</returns>
        public static List<List<double>> GetWeights2d(List<List<Point4>> homogenizedPoints)
        {
            return homogenizedPoints.Select(pts => GetWeights(pts).ToList()).ToList();
        }

        /// <summary>
        /// Gets a dehomogenized point from a homogenized curve point.
        /// </summary>
        /// <param name="homogenizedCurvePoint">A control point represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>A dehomogenized point.</returns>
        public static Point3 PointDehomogenizer(Point4 homogenizedCurvePoint)
        {
            Point3 point = new Point3
            {
                X = homogenizedCurvePoint.X / homogenizedCurvePoint.W,
                Y = homogenizedCurvePoint.Y / homogenizedCurvePoint.W,
                Z = homogenizedCurvePoint.Z / homogenizedCurvePoint.W
            };

            return point;
        }

        /// <summary>
        /// Gets a set of dehomogenized points.
        /// </summary>
        /// <param name="homogenizedPoints">A collection of points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of dehomogenized points.</returns>
        public static List<Point3> PointDehomogenizer1d(List<Point4> homogenizedPoints)
        {
            return homogenizedPoints.Select(PointDehomogenizer).ToList();
        }

        /// <summary>
        /// Gets a two-dimensional set of dehomogenized points.
        /// </summary>
        /// <param name="homogenizedPoints">Two-dimensional set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Two-dimensional set of dehomogenized points.</returns>
        public static List<List<Point3>> PointDehomogenizer2d(List<List<Point4>> homogenizedPoints)
        {
            return homogenizedPoints.Select(PointDehomogenizer1d).ToList();
        }

        /// <summary>
        /// Obtains the point from homogeneous point without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homogenizedPoints">Sets of points represented by an array (wi*pi, wi) with length (dim+1).</param>
        /// <returns>Set of rational points.</returns>
        public static List<Point3> RationalPoints(List<Point4> homogenizedPoints)
        {

            return homogenizedPoints.Select(pt => new Point3(pt.X, pt.Y, pt.Z)).ToList();
        }

        /// <summary>
        /// Obtains the point from a two-dimensional set of homogeneous points without dehomogenization, assuming all are the same length.
        /// </summary>
        /// <param name="homogenizedPoints">Two-dimensional set of points represented by an array (wi*pi, wi) with length (dim+1)</param>
        /// <returns>Two-dimensional set of rational points.</returns>
        public static List<List<Point3>> Rational2d(List<List<Point4>> homogenizedPoints)
        {
            return homogenizedPoints.Select(hpts => RationalPoints(hpts)).ToList();
        }

        /// <summary>
        /// Interpolate between two control points returning a new control points at the given interpolation parameter.
        /// </summary>
        /// <param name="pA">First point.</param>
        /// <param name="pB">Second point.</param>
        /// <param name="t">Interpolation parameter. 
        /// If t=0 then this point is set to pA. 
        /// If t=1 then this point is set to pB. 
        /// Values of t in between 0.0 and 1.0 result in control point between pA and pB.</param>
        public static Point4 Interpolate(Point4 pA, Point4 pB, double t)
        {
            if (t < 0 || t > 1) throw new ArgumentException($"{nameof(t)} must be between 0 and 1.");
            switch (t)
            {
                case 0:
                    return pA;
                case 1:
                    return pB;
            }

            var x = pA.X + t * (pB.X - pA.X);
            var y = pA.Y + t * (pB.Y - pA.Y);
            var z = pA.Z + t * (pB.Z - pA.Z);
            var w = pA.W + t * (pB.W - pA.W);

            return new Point4(x, y, z, w);
        }

        /// <summary>
        /// Determines whether the specified System.Object is Point4d and has same coordinates as the present point.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is Point4d and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Point4 point4 && this == point4;
        }

        /// <summary>
        /// Determines whether the specified point has same value as the present point.
        /// </summary>
        /// <param name="point">The specified point.</param>
        /// <returns>true if point has the same value as this; otherwise false.</returns>
        public bool Equals(Point4 point)
        {
            return this == point;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Point4 other, double epsilon)
        {
            return Math.Abs(X - other.X) <= epsilon &&
                   Math.Abs(Y - other.Y) <= epsilon &&
                   Math.Abs(Z - other.Z) <= epsilon &&
                   Math.Abs(W - other.W) <= epsilon;
        }

        /// <summary>
        /// Computes the hash code for the present point.
        /// </summary>
        /// <returns>A non-unique hash code, which uses all coordiantes of this object.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        /// <summary>
        /// Constructs the string representation for the current point.
        /// </summary>
        /// <returns>The point representation in the form X,Y,Z,W.</returns>
        public override string ToString()
        {
            return $"Point4: ({GSharkMath.Truncate(X)},{GSharkMath.Truncate(Y)},{GSharkMath.Truncate(Z)},{GSharkMath.Truncate(W)})";
        }
    }
}
