using System.Net;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// `Constants` contains a collection of default constants used throughout the library
    /// </summary>
    public class Constants
    {
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
        public static double UNSETVALUE => 1.23432101234321E+308;
    }
}
