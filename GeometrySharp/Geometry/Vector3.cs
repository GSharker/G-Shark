using System;
using System.Collections.Generic;
using System.Linq;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;

namespace GeometrySharp.Geometry
{
    /// <summary>
    /// Vector3 is represented simply by an list of double point numbers.
    /// So, you would write simply [1,0,0] to create a Vector3 in the X direction.
    /// </summary>
    public class Vector3 : List<double>, IEquatable<Vector3>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Vector3()
        {
        }

        /// <summary>
        /// Vector3 initialized by a list of values.
        /// So, you would write simply [1,0,0] to create a Vector3 in the X direction.
        /// </summary>
        /// <param name="values"></param>
        public Vector3(IEnumerable<double> values)
        {
            AddRange(values);
        }

        /// <summary>
        /// Gets the value of a point at location GeoSharpMath.UNSETVALUE,GeoSharpMath.UNSETVALUE,GeoSharpMath.UNSETVALUE.
        /// </summary>
        public static Vector3 Unset => new Vector3
            {GeoSharpMath.UNSETVALUE, GeoSharpMath.UNSETVALUE, GeoSharpMath.UNSETVALUE};

        /// <summary>
        /// The angle in radians between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The angle in radians</returns>
        public static double AngleBetween(Vector3 a, Vector3 b)
        {
            return Math.Acos(Dot(a, b) / (a.Length() * b.Length()));
        }

        /// <summary>
        /// Reverses this vector in place (reverses the direction).
        /// </summary>
        /// <param name="a">The vector to reverse.</param>
        /// <returns>The reversed vector.</returns>
        public static Vector3 Reverse(Vector3 a)
        {
            return new Vector3(a.Select(x => -x));
        }

        /// <summary>
        /// Gets a value indicating whether this vector is valid.
        /// A valid vector must be formed of finite numbers.
        /// </summary>
        /// <param name="a">The vector to be valued.</param>
        /// <returns>True if the vector is valid.</returns>
        public bool IsValid()
        {
            return this.Any(GeoSharpMath.IsValidDouble);
        }

        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        /// <param name="a">The vector to check.</param>
        /// <returns>True if all the component are less than Epsilon.</returns>
        public bool IsZero()
        {
            return this.All(value => Math.Abs(value) < GeoSharpMath.EPSILON);
        }

        /// <summary>
        /// Gets the vector amplified by a scalar value along a direction.
        /// </summary>
        /// <param name="origin">The start point or vector.</param>
        /// <param name="dir">The direction.</param>
        /// <param name="amplitude">The scalar value to amplify the vector.</param>
        /// <returns>The vector amplified.</returns>
        public static Vector3 OnRay(Vector3 origin, Vector3 dir, double amplitude)
        {
            var vectorAmplified = dir.Normalized() * amplitude;
            return origin + vectorAmplified;
        }

        /// <summary>
        /// Vector3 normalized.
        /// </summary>
        /// <param name="a">The vector has to be normalized.</param>
        /// <returns>The vector normalized.</returns>
        public Vector3 Normalized()
        {
            return this / Length();
        }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public double Length()
        {
            if (!IsValid() || IsZero()) return 0.0;

            return Math.Sqrt(SquaredLength());
        }

        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The sum of each value squared.</returns>
        public double SquaredLength()
        {
            return this.Aggregate(0.0, (x, a) => a * a + x);
        }

        /// <summary>
        /// Cross product.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Compute the cross product.</returns>
        public static Vector3 Cross(Vector3 u, Vector3 v)
        {
            return new Vector3
            {
                u[1] * v[2] - u[2] * v[1],
                u[2] * v[0] - u[0] * v[2],
                u[0] * v[1] - u[1] * v[0]
            };
        }

        /// <summary>
        /// Compute the dot product between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector with which compute the dot product.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector3 a, Vector3 b) => a.Select((t, i) => t * b[i]).Sum();

        // ToDo has to tested.
        /// <summary>
        /// Unitize vector.
        /// </summary>
        /// <returns>The vector unitized.</returns>
        public Vector3 Unitize()
        {
            return this * (1 / this.Length());
        }

        /// <summary>
        /// Create a list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <returns>Get a vector of r dimension.</returns>
        public static Vector3 Zero1d(int rows)
        {
            var v = new Vector3();
            for (var i = 0; i < rows; i++)
                v.Add(0.0);

            return v;
        }

        /// <summary>
        /// Create a 2 dimensional list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <param name="cols">Columns dimension.</param>
        /// <returns>Get a vector of r*c dimension.</returns>
        public static List<Vector3> Zero2d(int rows, int cols)
        {
            var lv = new List<Vector3>();
            for (var i = 0; i < rows; i++) 
                lv.Add(Zero1d(cols));

            return lv;
        }

        /// <summary>
        /// Create a 3 dimensional list of zero values.
        /// </summary>
        /// <param name="rows">Rows dimension.</param>
        /// <param name="cols">Columns dimension.</param>
        /// <param name="depth">Depth dimension.</param>
        /// <returns>Get a vector of r*c*d dimension.</returns>
        public static List<List<Vector3>> Zero3d(int rows, int cols, int depth)
        {
            var llv = new List<List<Vector3>>();
            for (var i = 0; i < rows; i++) 
                llv.Add(Zero2d(cols, depth));

            return llv;
        }

        /// <summary>
        /// The distance from this point to b.
        /// </summary>
        /// <param name="v">The target vector.</param>
        /// <returns>The distance between this vector and the provided vector.</returns>
        public double DistanceTo(Vector3 v)
        {
            if (Count != v.Count) throw new Exception("The two list doesn't match in length.");

            return Math.Sqrt(this.Select((val, i) => Math.Pow(val - v[i], 2)).Sum());
        }

        /// <summary>
        /// Get the distance of a point to a ray.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <param name="ray">The ray from which to calculate the distance.</param>
        /// <returns>The distance.</returns>
        public double DistanceTo(Ray ray)
        {
            var projectedPt = ClosestPointOn(ray);
            var ptToProjectedPt = projectedPt - this;
            return ptToProjectedPt.Length();
        }

        /// <summary>
        /// Get the distance of a point to a line.
        /// </summary>
        /// <param name="pt">The point to project.</param>
        /// <param name="line">The line from which to calculate the distance.</param>
        /// <returns>The distance.</returns>
        public double DistanceTo(Line line)
        {
            var ray = new Ray(line.Start, line.Direction);

            return DistanceTo(ray);
        }

        /// <summary>
        /// Get the closest point on a ray from a point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="ray">The ray on which to find the point.</param>
        /// <returns>Get the closest point on a ray from a point.</returns>
        public Vector3 ClosestPointOn(Ray ray)
        {
            var rayDirNormalized = ray.Direction.Normalized();
            var rayOriginToPt = this - ray.Origin;
            var dotResult = Dot(rayOriginToPt, rayDirNormalized);
            var projectedPt = ray.Origin + rayDirNormalized * dotResult;

            return projectedPt;
        }

        /// <summary>
        /// Get the closest point on the line from this point.
        /// </summary>
        /// <param name="line">The line on which to find the closest point.</param>
        /// <returns>The closest point on the line from this point.</returns>
        public Vector3 ClosestPointOn(Line line)
        {
            var dir = line.Direction;
            var v = this - line.Start;
            var d = Dot(v, dir);

            d = Math.Min(line.Length, d);
            d = Math.Max(d, 0);

            return line.Start + dir * d;
        }

        /// <summary>
        /// Determinate if the provided point lies on the plane.
        /// </summary>
        /// <param name="pt">The point to check if it lies on the plane.</param>
        /// <param name="plane">The plane on which to find if the point lies on.</param>
        /// <returns>Whether the point is on the plane.</returns>
        public bool IsPointOnPlane(Plane plane, double tol)
        {
            return Math.Abs(Dot(this - plane.Origin, plane.Normal)) < tol;
        }

        /// <summary>
        /// Multiply a vector and a scalar.
        /// </summary>
        /// <param name="v">The vector to multiply.</param>
        /// <param name="a">The scalar value to multiply.</param>
        /// <returns>A vector whose magnitude is multiplied by a.</returns>
        public static Vector3 operator *(Vector3 v, double a)
        {
            return v.Select(val => val * a).ToVector();
        }

        /// <summary>
        /// Divide a vector by a scalar.
        /// </summary>
        /// <param name="a">The scalar divisor.</param>
        /// <param name="v">The vector to divide.</param>
        /// <returns>A vector whose magnitude is multiplied by a.</returns>
        public static Vector3 operator /(Vector3 v, double a)
        {
            return v.Select(val => val / a).ToVector();
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the difference between a and b.</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return a.Select((val, i) => val - b[i]).ToVector();
        }

        /// <summary>
        /// Add two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector which is the sum of a and b.</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return a.Select((val, i) => val + b[i]).ToVector();
        }

        /// <summary>
        /// Are the two vectors the same within Epsilon?
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return Equals(a, b);
        }

        /// <summary>
        /// Are the two vectors the same within Epsilon?
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Is the vector equal to the provided vector?
        /// </summary>
        /// <param name="obj">The vector to test.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Vector3)) return false;

            var vec = (Vector3)obj;
            return IsAlmostEqualTo(vec);
        }

        /// <summary>
        /// Is the vector equal to the provided vector?
        /// </summary>
        /// <param name="obj">The vector to test.</param>
        /// <returns>Returns true if all components of the two vectors are within Epsilon, otherwise false.</returns>
        public bool Equals(Vector3 other)
        {
            return IsAlmostEqualTo(other);
        }

        /// <summary>
        /// Determine whether this vector's components are equal to those of v, within tolerance.
        /// </summary>
        /// <param name="v">The vector to compare.</param>
        /// <returns>
        /// True if the difference of this vector and the supplied vector's components are all within Tolerance, otherwise
        /// false.
        /// </returns>
        public bool IsAlmostEqualTo(Vector3 v)
        {
            return this.Select((val, i) => Math.Abs(val - v[i]))
                .All(val => val < GeoSharpMath.EPSILON);
        }

        /// <summary>
        /// Compute the perpendicular to another vector. Result is not unitized.
        /// </summary>
        /// <param name="other">Vector to use as guide.</param>
        /// <returns>Return the perpendicular vector.</returns>
        /// https://stackoverflow.com/questions/11132681/what-is-a-formula-to-get-a-vector-perpendicular-to-another-vector
        public Vector3 PerpendicularTo(Vector3 other)
        {
            var tempVector = new double[3]; 
            int i, j, k;
            double a, b;
            k = 2;
            if (Math.Abs(other[1]) > Math.Abs(other[0]))
            {
                if (Math.Abs(other[2]) > Math.Abs(other[1]))
                {
                    // |v.z| > |v.y| > |v.x|
                    i = 2;
                    j = 1;
                    k = 0;
                    a = other[2];
                    b = -other[1];
                }
                else if (Math.Abs(other[2]) > Math.Abs(other[0]))
                {
                    // |v.y| >= |v.z| >= |v.x|
                    i = 1;
                    j = 2;
                    k = 0;
                    a = other[1];
                    b = -other[2];
                }
                else
                {
                    // |v.y| > |v.x| > |v.z|
                    i = 1;
                    j = 0;
                    k = 2;
                    a = other[1];
                    b = -other[0];
                }
            }
            else if (Math.Abs(other[2]) > Math.Abs(other[0]))
            {
                // |v.z| > |v.x| >= |v.y|
                i = 2;
                j = 0;
                k = 1;
                a = other[2];
                b = -other[0];
            }
            else if (Math.Abs(other[2]) > Math.Abs(other[1]))
            {
                // |v.x| >= |v.z| > |v.y|
                i = 0;
                j = 2;
                k = 1;
                a = other[0];
                b = -other[2];
            }
            else
            {
                // |v.x| >= |v.y| >= |v.z|
                i = 0;
                j = 1;
                k = 2;
                a = other[0];
                b = -other[1];
            }

            tempVector[i] = b;
            tempVector[j] = a;
            tempVector[k] = 0.0;

            return tempVector.ToVector();
        }

        /// <summary>
        /// Constructs the string representation of the vector.
        /// </summary>
        /// <returns>The vector in string format</returns>
        public override string ToString()
        {
            return $"{Math.Round(this[0], 6)},{Math.Round(this[1], 6)},{Math.Round(this[2], 6)}";
        }
    }
}