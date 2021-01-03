using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// `Constants` contains a collection of default constants used throughout the library
    /// </summary>
    public class Constants
    {
        //ToDo make tests for this class.
        /// <summary>
        /// The current version of GeometryLib
        /// </summary>
        public static string VERSION => "1.0.0";
        /// <summary>
        /// The default euclidean distance that identifies whether two points are coincident
        /// </summary>
        public static double TOLERANCE => 1e-6;
        /// <summary>
        /// The minimum value to determine whether two floating point numbers are the same
        /// </summary>
        public static double EPSILON => 1e-10;
        /// <summary>
        /// The value of an unset object.
        /// </summary>
        public static double UNSETVALUE => -1.23432101234321E+308;
        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="degrees">Value degrees.</param>
        /// <returns>Get the radians value.</returns>
        public static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="radians">Value radians.</param>
        /// <returns>Get the degree value.</returns>
        public static double ToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
        /// <summary>
        /// Check if it is a value double.
        /// </summary>
        /// <param name="x">Double value.</param>
        /// <returns>Return if it is valid.</returns>
        public static bool IsValidDouble(double x)
        {
            return x != -1.23432101234321E+308 && !double.IsInfinity(x) && !double.IsNaN(x);
        }

        //ToDo maybe these operators can be split for Point and Vector or in a specific class called MathVerb, for example.
        /// <summary>
        /// Add two list of numbers, so you can sum points or vectors.
        /// </summary>
        /// <param name="a">The first list.</param>
        /// <param name="b">The second list.</param>
        /// <returns>A vector which is the sum of a and b.</returns>
        public static IList<double> Addition(IList<double> a, IList<double> b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] + b[i]);
            return vec;
        }
        /// <summary>
        /// Multiply a scalar and a list numbers, so you can multiple vectors.
        /// </summary>
        /// <param name="a">The list to divide.</param>
        /// <param name="b">The scalar value to multiply.</param>
        /// <returns>A list whose magnitude is multiplied by b.</returns>
        public static IList<double > Multiplication(IList<double> a, double b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] * b);
            return vec;
        }
        /// <summary>
        /// Divide a list of numbers by a scalar, so you can divide vectors.
        /// </summary>
        /// <param name="a">The list to divide.</param>
        /// <param name="b">The scalar divisor.</param>
        /// <returns>A list whose magnitude is multiplied by b.</returns>
        public static IList<double> Division(IList<double> a, double b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] / b);
            return vec;
        }
        /// <summary>
        /// Subtract two lists of numbers, so you can subtract points and vectors.
        /// </summary>
        /// <param name="a">The first list.</param>
        /// <param name="b">The second list.</param>
        /// <returns>A list which is the difference between a and b.</returns>
        public static IList<double> Subtraction(IList<double> a, IList<double> b)
        {
            Vector vec = new Vector();
            for (int i = 0; i < a.Count; i++)
                vec.Add(a[i] - b[i]);
            return vec;
        }
        /// <summary>
        /// The distance from two vectors or two points.
        /// </summary>
        /// <param name="a">The first list.</param>
        /// <param name="b">The second list.</param>
        /// <returns>The distance between the provided list.</returns>
        public static double DistanceTo(IList<double> a, IList<double> b)
        {
            if(a.Count != b.Count) throw new Exception("The two list doesn't match in length.");
            return Math.Sqrt(a.Zip(b, (first, second) => Math.Pow(first - second, 2)).Sum());
        }

        // ToDo value if this method is necessary.
        public static double DistSquared(Vector a, Vector b) => throw new NotImplementedException();
    }
}
