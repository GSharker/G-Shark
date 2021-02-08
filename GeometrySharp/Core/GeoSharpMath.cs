using System;

namespace GeometrySharp.Core
{
    /// <summary>
    /// GeoSharpMath contains a collection of default constants used throughout the library.
    /// </summary>
    public class GeoSharpMath
    {
        /// <summary>
        /// The current version of GeometryLib.
        /// </summary>
        public static string VERSION => "1.0.0";

        /// <summary>
        /// The default euclidean distance that identifies whether two points are coincident.
        /// </summary>
        public static double MINTOLERANCE => 1e-3;

        /// <summary>
        /// The default euclidean distance that identifies whether two points are coincident.
        /// </summary>
        public static double MAXTOLERANCE => 1e-6;

        /// <summary>
        /// The minimum value to determine whether two floating point numbers are the same.
        /// </summary>
        public static double EPSILON => 1e-10;

        /// <summary>
        /// The value of an unset object.
        /// </summary>
        public static double UNSETVALUE => -1.23432101234321E+308;

        /// <summary>
        /// Represents the default angle tolerance, used when no other values are provided.
        /// This is one degree, expressed in radians.
        /// </summary>
        public static double ANGLETOLERANCE => 0.0174532925199433;

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
    }
}
