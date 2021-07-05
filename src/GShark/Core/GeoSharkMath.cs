using GShark.Geometry;
using System;

namespace GShark.Core
{
    /// <summary>
    /// A collection of default constants and methods used throughout the library.
    /// </summary>
    public class GeoSharkMath
    {
        /// <summary>
        /// The default euclidean distance that identifies whether two points are coincident.
        /// </summary>
        public const double MinTolerance = 1e-3;

        /// <summary>
        /// The default euclidean distance that identifies whether two points are coincident.
        /// </summary>
        public const double MaxTolerance = 1e-6;

        /// <summary>
        /// The minimum value to determine whether two floating point numbers are the same.
        /// </summary>
        public const double Epsilon = 1e-10;

        /// <summary>
        /// The value of an unset object.
        /// </summary>
        public const double UnsetValue = -1.23432101234321E+308;

        /// <summary>
        /// Represents the default angle tolerance, used when no other values are provided.<br/>
        /// This is one degree, expressed in radians.
        /// </summary>
        public const double AngleTolerance = 0.0174532925199433;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Value in degrees.</param>
        /// <returns>The value in radians.</returns>
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
        /// Returns true if the double value is valid.
        /// </summary>
        /// <param name="x">Double value.</param>
        /// <returns>True if it is valid.</returns>
        public static bool IsValidDouble(double x)
        {
            return Math.Abs(x - UnsetValue) > double.Epsilon && !double.IsInfinity(x) && !double.IsNaN(x);
        }

        /// <summary>
        /// Remaps a value into a new numerical target range given a source range.
        /// </summary>
        /// <param name="value">Value to remap.</param>
        /// <param name="source">Source range.</param>
        /// <param name="target">Target range.</param>
        /// <returns>Remapped value.</returns>
        public static double RemapValue(double value, Interval source, Interval target)
        {
            return target.T0 + (value - source.T0) * (target.T1 - target.T0) / (source.T1 - source.T0);
        }

        /// <summary>
        /// Reduces the noise from the input.
        /// </summary>
        /// <param name="sinAngle">Sin angle value in radians.</param>
        /// <param name="cosAngle">Cos angle value in radians.</param>
        internal static void KillNoise(ref double sinAngle, ref double cosAngle)
        {
            if (Math.Abs(sinAngle) >= 1.0 - GeoSharkMath.MaxTolerance &&
                Math.Abs(cosAngle) <= GeoSharkMath.MaxTolerance)
            {
                cosAngle = 0.0;
                sinAngle = (sinAngle < 0.0) ? -1.0 : 1.0;
            }

            if (Math.Abs(cosAngle) >= 1.0 - GeoSharkMath.MaxTolerance &&
                Math.Abs(sinAngle) <= GeoSharkMath.MaxTolerance)
            {
                cosAngle = (cosAngle < 0.0) ? -1.0 : 1.0;
                sinAngle = 0.0;
            }

            if (Math.Abs(cosAngle * cosAngle + sinAngle * sinAngle - 1.0) > GeoSharkMath.MaxTolerance)
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

            if (Math.Abs(sinAngle) > 1.0 - GeoSharkMath.Epsilon &&
                Math.Abs(cosAngle) < GeoSharkMath.Epsilon)
            {
                cosAngle = 0.0;
                sinAngle = (sinAngle < 0.0) ? -1.0 : 1.0;
            }

            if (Math.Abs(cosAngle) > 1.0 - GeoSharkMath.Epsilon &&
                Math.Abs(sinAngle) < GeoSharkMath.Epsilon)
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
