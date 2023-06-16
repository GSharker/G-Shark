using GShark.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    /// Defines a Vector in Euclidean space with coordinates X, Y, and Z.
    /// Referenced from https://github.com/mcneel/rhinocommon/blob/master/dotnet/opennurbs/opennurbs_point.cs
    /// </summary>
    public class Point3 : IEquatable<Point3>, IComparable<Point3>, IComparable
    {
        /// <summary>
        /// Initializes a new point with zero valued coordinates.
        /// </summary>
        public Point3()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// Initializes a new point by defining the X, Y and Z coordinates.
        /// </summary>
        /// <param name="x">The value of the X (first) coordinate.</param>
        /// <param name="y">The value of the Y (second) coordinate.</param>
        /// <param name="z">The value of the Z (third) coordinate.</param>
        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from the components of a vector.
        /// </summary>
        /// <param name="vector">A vector.</param>
        public Point3(Vector3 vector) : this(vector.X, vector.Y, vector.Z)
        {
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from another point.
        /// </summary>
        /// <param name="point">A point.</param>
        public Point3(Point3 point) : this(point.X, point.Y, point.Z)
        {
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from a four-dimensional point.
        /// The first three coordinates are divided by the last one.
        /// If the W (fourth) dimension of the input point is zero, then it will be discarded.
        /// </summary>
        /// <param name="point">A point.</param>
        public Point3(Point4 point)
        {
            double w = (Math.Abs(point.W - 1.0) > GSharkMath.Epsilon && point.W != 0.0) ? 1.0 / point.W : 1.0;

            X = point.X * w;
            Y = point.Y * w;
            Z = point.Z * w;
        }

        /// <summary>
        /// Dimension of point.
        /// </summary>
        public int Size => 3;

        /// <summary>
        /// Gets the value of a point at location 0,0,0.
        /// </summary>
        public static Point3 Origin => new Point3(0, 0, 0);

        /// <summary>
        /// Gets the value of a point at location RhinoMath.UnsetValue,RhinoMath.UnsetValue,RhinoMath.UnsetValue.
        /// </summary>
        public static Point3 Unset => new Point3(GSharkMath.UnsetValue, GSharkMath.UnsetValue, GSharkMath.UnsetValue);

        /// <summary>
        /// Multiplies a <see cref="Point3"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinate-wise multiplied by t.</returns>
        public static Point3 operator *(Point3 point, double t)
        {
            return new Point3(point.X * t, point.Y * t, point.Z * t);
        }

        /// <summary>
        /// Multiplies a <see cref="Point3"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinate-wise multiplied by t.</returns>
        public static Point3 operator *(double t, Point3 point)
        {
            return new Point3(point.X * t, point.Y * t, point.Z * t);
        }

        /// <summary>
        /// Divides a <see cref="Point3"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinate-wise divided by t.</returns>
        public static Point3 operator /(Point3 point, double t)
        {
            return new Point3(point.X / t, point.Y / t, point.Z / t);
        }

        /// <summary>
        /// Sums two <see cref="Point3"/> instances.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">A point.</param>
        /// <returns>A new point that results from the addition of point1 and point2.</returns>
        public static Point3 operator +(Point3 point1, Point3 point2)
        {
            return new Point3(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3 operator +(Point3 point, Vector3 vector)
        {
            return new Point3(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3 operator +(Vector3 vector, Point3 point)
        {
            return new Point3(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// <para>(Provided for languages that do not support operator overloading. You can use the + operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3 Add(Vector3 vector, Point3 point)
        {
            return new Point3(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);
        }

        /// <summary>
        /// Subtracts a vector from a point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that is the difference of point minus vector.</returns>
        public static Point3 operator -(Point3 point, Vector3 vector)
        {
            return new Point3(point.X - vector.X, point.Y - vector.Y, point.Z - vector.Z);
        }

        /// <summary>
        /// Subtracts a point from another point.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">Another point.</param>
        /// <returns>A new vector that is the difference of point minus vector.</returns>
        public static Vector3 operator -(Point3 point1, Point3 point2)
        {
            return new Vector3(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
        }

        /// <summary>
        /// Computes the additive inverse of all coordinates in the point, and returns the new point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>A point value that, when summed with the point input, yields the <see cref="Origin"/>.</returns>
        public static Point3 operator -(Point3 point)
        {
            return new Point3(-point.X, -point.Y, -point.Z);
        }

        /// <summary>
        /// Determines whether two Point3 have equal values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the coordinates of the two points are exactly equal; otherwise false.</returns>
        public static bool operator ==(Point3 a, Point3 b)
        {
            if (a is null && b is null)
                return false;
            if (a is null || b is null)
                return false;
            if (ReferenceEquals(a, b))
                return true;
            return (Math.Abs(a.X - b.X) < GSharkMath.MaxTolerance
                    && Math.Abs(a.Y - b.Y) < GSharkMath.MaxTolerance
                    && Math.Abs(a.Z - b.Z) < GSharkMath.MaxTolerance);
        }

        /// <summary>
        /// Determines whether two Point3 have different values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the two points differ in any coordinate; false otherwise.</returns>
        public static bool operator !=(Point3 a, Point3 b)
        {
            return (Math.Abs(a.X - b.X) > GSharkMath.MaxTolerance
                    || Math.Abs(a.Y - b.Y) > GSharkMath.MaxTolerance
                    || Math.Abs(a.Z - b.Z) > GSharkMath.MaxTolerance);
        }

        /// <summary>
        /// Converts a point in a control point, without needing casting.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The control point.</returns>
        public static implicit operator Point4(Point3 point)
        {
            return new Point4(point);
        }

        /// <summary>
        /// Converts a point in a vector, without needing casting.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>The resulting vector3.</returns>
        public static implicit operator Vector3(Point3 point)
        {
            return new Vector3(point);
        }

        /// <summary>
        /// Converts a point in a vector, without needing casting.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>The resulting Vector.</returns>
        public static implicit operator Vector(Point3 point)
        {
            return new Vector { point.X, point.Y, point.Z };
        }

        /// <summary>
        /// Determines whether the first specified point comes before (has inferior sorting value than) the second point.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z;
        /// otherwise, false.</returns>
        public static bool operator <(Point3 a, Point3 b)
        {
            if (a.X < b.X)
                return true;
            if (a.X == b.X)
            {
                if (a.Y < b.Y)
                    return true;
                if (a.Y == b.Y && a.Z < b.Z)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified point comes before
        /// (has inferior sorting value than) the second point, or it is equal to it.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &lt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator <=(Point3 a, Point3 b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Determines whether the first specified point comes after (has superior sorting value than) the second point.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z;
        /// otherwise, false.</returns>
        public static bool operator >(Point3 a, Point3 b)
        {
            if (a.X > b.X)
                return true;
            if (a.X == b.X)
            {
                if (a.Y > b.Y)
                    return true;
                if (a.Y == b.Y && a.Z > b.Z)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified point comes after
        /// (has superior sorting value than) the second point, or it is equal to it.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &gt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator >=(Point3 a, Point3 b)
        {
            return a.CompareTo(b) >= 0;
        }

        /// <summary>
        /// Gets or sets the X (first) coordinate of this point.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y (second) coordinate of this point.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the Z (third) coordinate of this point.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Each coordinate of the point must pass the <see cref="GSharkMath.IsValidDouble"/> test.
        /// </summary>
        public bool IsValid => GSharkMath.IsValidDouble(X) && GSharkMath.IsValidDouble(Y) && GSharkMath.IsValidDouble(Z);

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
                    _ => throw new IndexOutOfRangeException()
                };
            }
            set
            {
                if (i < 0 || i > 2) throw new IndexOutOfRangeException();
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
                }
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is a <see cref="Point3"/> and has the same values as the present point.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is a Point3 and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;
            return obj is Point3 point3 && this == point3;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns>True if the two points have the same coordinates as this; otherwise false.</returns>
        public bool EpsilonEquals(Point3 other, double epsilon)
        {
            return Math.Abs(X - other.X) <= epsilon &&
                   Math.Abs(Y - other.Y) <= epsilon &&
                   Math.Abs(Z - other.Z) <= epsilon;
        }

        /// <summary>
        /// Compares this <see cref="Point3" /> with another <see cref="Point3" />.
        /// <para>Component evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Point3" /> to use in comparison.</param>
        /// <returns>
        /// <para> 0: if this is identical to other</para>
        /// <para>-1: if this.X &lt; other.X</para>
        /// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
        /// <para>-1: if this.X == other.X and this.Y == other.Y and this.Z &lt; other.Z</para>
        /// <para>+1: otherwise.</para>
        /// </returns>
        public int CompareTo(Point3 other)
        {
            if (X < other.X)
                return -1;
            if (X > other.X)
                return 1;

            if (Y < other.Y)
                return -1;
            if (Y > other.Y)
                return 1;

            if (Z < other.Z)
                return -1;
            if (Z > other.Z)
                return 1;

            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Point3 Point3)
                return CompareTo(Point3);

            throw new ArgumentException("Input must be of type Point3", nameof(obj));
        }

        /// <summary>
        /// Determines whether the specified <see cref="Point3"/> has the same values as the present point.
        /// </summary>
        /// <param name="point">The specified point.</param>
        /// <returns>True if point has the same coordinates as this; otherwise false.</returns>
        public bool Equals(Point3 point)
        {
            return this == point;
        }

        /// <summary>
        /// Computes a hash code for the present point.
        /// </summary>
        /// <returns>A non-unique integer that represents this point.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        /// Interpolate isBetween two points returning a new point at the given interpolation parameter.
        /// </summary>
        /// <param name="pA">First point.</param>
        /// <param name="pB">Second point.</param>
        /// <param name="t">Interpolation parameter. 
        /// If t=0 then this point is set to pA. 
        /// If t=1 then this point is set to pB. 
        /// Values of t in isBetween 0.0 and 1.0 result in points isBetween pA and pB.</param>
        public static Point3 Interpolate(Point3 pA, Point3 pB, double t)
        {
            if (t < 0 || t > 1) throw new ArgumentException($"{nameof(t)} must be isBetween 0 and 1.");
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

            return new Point3(x, y, z);
        }

        /// <summary>
        /// Get a point isBetween two points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <returns>Point isBetween first and second point.</returns>
        public static Point3 PointBetween(Point3 p1, Point3 p2)
        {
            return Interpolate(p1, p2, 0.5);
        }
        
        /// <summary>
        /// Calculate the centroid of an arbitrary collection of points
        /// </summary>
        /// <param name="points">A collection of points.</param>
        /// <returns>The centroid of the points</returns>
        public static Point3 Centroid(IEnumerable<Point3> points)
        {
            IEnumerable<Point3> enumerable = points as Point3[] ?? points.ToArray();
            return new Point3(
                enumerable.Average(point => point.X),
                enumerable.Average(point => point.Y),
                enumerable.Average(point => point.Z));
        }
        /// <summary>
        /// Constructs the string representation for the current point.
        /// </summary>
        /// <returns>The point representation in the form X,Y,Z.</returns>
        public override string ToString()
        {
            return $"Point3: ({GSharkMath.Truncate(X)},{GSharkMath.Truncate(Y)},{GSharkMath.Truncate(Z)})";
        }

        /// <summary>
        /// Computes the distance isBetween two points.
        /// </summary>
        /// <param name="other">Other point for distance measurement.</param>
        /// <returns>The length of the line isBetween this and the other point; or 0 if any of the points is not valid.</returns>
        public double DistanceTo(Point3 other)
        {
            double d;
            if (IsValid && other.IsValid)
            {
                double dx = other.X - X;
                double dy = other.Y - Y;
                double dz = other.Z - Z;
                d = Vector3.GetLengthHelper(dx, dy, dz);
            }
            else
            {
                d = 0.0;
            }
            return d;
        }
        
        /// <summary>
        /// Projects a point onto a plane.
        /// </summary>
        /// <param name="plane">Plane  to project onto</param>
        /// <returns name="point3">The point projected to plane</returns>
        public Point3 ProjectToPlan(Plane plane)
        {
            Vector3 v = plane.Origin - this;
            Vector3 normal = plane.ZAxis;
            double d = Vector3.DotProduct(v, normal);
            return this + d * normal;
        }

        /// <summary>
        /// Calculates the distance of a point to a line.
        /// </summary>
        /// <param name="line">The line from which to calculate the distance.</param>
        /// <returns>The distance.</returns>
        public double DistanceTo(Line line)
        {
            Point3 projectedPt = line.ClosestPoint(this);
            Vector3 ptToProjectedPt = projectedPt - this;
            return ptToProjectedPt.Length;
        }

        /// <summary>
        /// Transforms the point using a transformation matrix.
        /// </summary>
        /// <param name="t">The transformation matrix.</param>
        /// <returns>The transformed point as a new instance.</returns>
        public Point3 Transform(TransformMatrix t)
        {
            return this * t;
        }

        /// <summary>
        /// Removes duplicates in the supplied set of points.
        /// </summary>
        /// <param name="points">A list, an array or any enumerable of <see cref="Point3"/>.</param>
        /// <param name="tolerance">The minimum distance isBetween points.
        /// <para>Points that fall within this tolerance will be discarded.</para>
        /// .</param>
        /// <returns>An array of points without duplicates; or null on error.</returns>
        public static Point3[] CullDuplicates(IEnumerable<Point3> points, double tolerance)
        {
            if (null == points)
                return null;

            var pointList = new List<Point3>(points);
            int count = pointList.Count;
            if (0 == count)
                return null;

            bool[] dup_list = new bool[count];
            var nonDups = new List<Point3>();

            for (int i = 0; i < count; i++)
            {
                // Check if the entry has been flagged as a duplicate
                if (dup_list[i] == false)
                {
                    nonDups.Add(pointList[i]);
                    // Only compare with entries that haven't been checked
                    for (int j = i + 1; j < count; j++)
                    {
                        if (pointList[i].DistanceTo(pointList[j]) <= tolerance)
                            dup_list[j] = true;
                    }
                }
            }

            return nonDups.ToArray();
        }

        /// <summary>
        /// Test whether a point lies on a plane.
        /// </summary>
        /// <param name="plane">The plane to test against.</param>
        /// <param name="tolerance">Default is use 1e-6</param>
        /// <returns>Returns true if point is on plane.</returns>
        public bool IsOnPlane(Plane plane, double tolerance = GSharkMath.MaxTolerance)
        {
            return Math.Abs(Vector3.DotProduct(this - plane.Origin, plane.ZAxis)) < tolerance;
        }
        
        /// <summary>
        /// Test whether a point lies on a line.
        /// </summary>
        /// <param name="line">The line to test against.</param>
        /// <param name="tolerance">Default is use 1e-6</param>
        /// <returns>Returns true if point is on line.</returns>
        public bool IsOnLine(Line line, double tolerance = GSharkMath.MaxTolerance)
        {
            return line.ClosestPoint(this).DistanceTo(this) < tolerance;
        }

        /// <summary>
        /// Tests whether a point is inside, outside, or coincident with a polygon.
        /// <para>See https://stackoverflow.com/a/63436180</para>
        /// </summary>
        /// <param name="polygon">The polygon to test against.</param>
        /// <returns>Returns -1 if point is outside the polygon, 0 if it is coincident with a polygon edge, or 1 if it is inside the polygon.</returns>        
        public int InPolygon(Polygon polygon)
        {
            //check if point lies on polygon plane, else return
            var polygonPlane = polygon.Plane;

            if (!this.IsOnPlane(polygonPlane)) return -1;

            //translate polygon and point to XY plane for 2d calculations to account for rotated polygons and 3d points
            var xForm = Core.Transform.PlaneToPlane(polygonPlane, Plane.PlaneXY);
            var polygonOriented = polygon.Transform(xForm);
            var pointOriented = this.Transform(xForm);
            
            //tests whether a value is isBetween two other values
            Func<double, double, double, bool> isValueBetween = (p, a, b) => 
                ((p - a) >= double.Epsilon) &&
                ((p - b) <= double.Epsilon) ||
                ((p - a) <= double.Epsilon) &&
                ((p - b) >= double.Epsilon);
            
            bool inside = false;

            for (int i = polygonOriented.ControlPointLocations.Count - 1, j = 0; j < polygonOriented.ControlPointLocations.Count; i = j, j++)
            {
                Point3 A = polygonOriented.ControlPointLocations[i];
                Point3 B = polygonOriented.ControlPointLocations[j];
                
                // corner cases
                if (
                    (Math.Abs(pointOriented.X - A.X) <= double.Epsilon) && 
                    (Math.Abs(pointOriented.Y - A.Y) <= double.Epsilon) || 
                    (Math.Abs(pointOriented.X - B.X) <= double.Epsilon) &&
                    (Math.Abs(pointOriented.Y - B.Y) <= double.Epsilon)) return 0;
                
                if (
                    Math.Abs(A.Y - B.Y) <= double.Epsilon && 
                    Math.Abs(pointOriented.Y - A.Y) <= double.Epsilon && 
                    isValueBetween(pointOriented.X, A.X, B.X)) return 0;

                if (isValueBetween(pointOriented.Y, A.Y, B.Y))
                {
                    // if P inside the vertical range
                    // filter out "ray pass vertex" problem by treating the line a little lower
                    if (
                        Math.Abs(pointOriented.Y - A.Y) <= double.Epsilon &&
                        (B.Y - A.Y) >= double.Epsilon || 
                        Math.Abs(pointOriented.Y - B.Y) <= double.Epsilon &&
                        (A.Y - B.Y) <= double.Epsilon) continue;
                    
                    // calc cross product `PA X PB`, P lays on left side of AB if c > 0 
                    double c = (A.X - pointOriented.X) * (B.Y - pointOriented.Y) - (B.X - pointOriented.X) * (A.Y - pointOriented.Y);
                    
                    if (c > 0 && c < GSharkMath.MinTolerance ) return 0;

                    if ((A.Y < B.Y) == (c > 0))
                        inside = !inside;
                }
            }
            return inside ? 1 : -1;
        }
        
    }
}

