using GShark.Geometry;
using System;

namespace GShark.Core
{
    /// <summary>
    /// A collection of default constants and methods used throughout the library.
    /// </summary>
    public class GeoSharpMath
    {
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
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Value degrees.</param>
        /// <returns>The radians value.</returns>
        public static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">Value radians.</param>
        /// <returns>The degree value.</returns>
        public static double ToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// Checks if it is a valid double.
        /// </summary>
        /// <param name="x">Double value.</param>
        /// <returns>True if it is valid.</returns>
        public static bool IsValidDouble(double x)
        {
            return Math.Abs((double) x - (-1.23432101234321E+308)) > double.Epsilon && !double.IsInfinity(x) && !double.IsNaN(x);
        }

        /// <summary>
        /// Remaps a value into a new numerical range.
        /// </summary>
        /// <param name="value">Value to remap.</param>
        /// <param name="source">Numerical interval of the value.</param>
        /// <param name="target">Numerical interval into the value will be remapped.</param>
        /// <returns>Remapped value.</returns>
        public static double RemapValue(double value, Interval source, Interval target)
        {
            return target.Min + (value - source.Min) * (target.Max - target.Min) / (source.Max - source.Min);
        }

        /// <summary>
        /// Reduces the noise from the input.
        /// </summary>
        /// <param name="sinAngle">Sin angle value.</param>
        /// <param name="cosAngle">Cos angle value.</param>
        internal static void KillNoise(ref double sinAngle, ref double cosAngle)
        {
            if (Math.Abs(sinAngle) >= 1.0 - GeoSharpMath.MAXTOLERANCE &&
                Math.Abs(cosAngle) <= GeoSharpMath.MAXTOLERANCE)
            {
                cosAngle = 0.0;
                sinAngle = (sinAngle < 0.0) ? -1.0 : 1.0;
            }

            if (Math.Abs(cosAngle) >= 1.0 - GeoSharpMath.MAXTOLERANCE &&
                Math.Abs(sinAngle) <= GeoSharpMath.MAXTOLERANCE)
            {
                cosAngle = (cosAngle < 0.0) ? -1.0 : 1.0;
                sinAngle = 0.0;
            }

            if (Math.Abs(cosAngle * cosAngle + sinAngle * sinAngle - 1.0) > GeoSharpMath.MAXTOLERANCE)
            {
                Vector3 vec = new Vector3 { cosAngle, sinAngle };
                if (vec.Length() > 0.0)
                {
                    Vector3 vecUnitized = vec.Unitize();
                    cosAngle = vecUnitized[0];
                    sinAngle = vecUnitized[1];
                }
                else
                {
                    throw new Exception("SinAngle and CosAngle are both zero");
                }
            }

            if (Math.Abs(sinAngle) > 1.0 - GeoSharpMath.EPSILON &&
                Math.Abs(cosAngle) < GeoSharpMath.EPSILON)
            {
                cosAngle = 0.0;
                sinAngle = (sinAngle < 0.0) ? -1.0 : 1.0;
            }

            if (Math.Abs(cosAngle) > 1.0 - GeoSharpMath.EPSILON &&
                Math.Abs(sinAngle) < GeoSharpMath.EPSILON)
            {
                cosAngle = (cosAngle < 0.0) ? -1.0 : 1.0;
                sinAngle = 0.0;
            }
        }

        /// <summary>
        /// Delegated function, used to sort a list of numerical value.
        /// </summary>
        internal static int NumberSort(double a, double b)
        {
            return Math.Sign(a - b);
        }
    }
}
