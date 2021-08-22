using System;
using System.Collections.Generic;
using System.Text;

namespace GShark.Geometry.Enum
{
    /// <summary>
    /// Defines the curve type
    /// </summary>
    public enum CurveType
    {
        /// <summary>
        /// Line between two points
        /// </summary>
        LINE,
        /// <summary>
        /// Arc as a portion of a circle
        /// </summary>
        ARC,
        /// <summary>
        /// Full circle
        /// </summary>
        CIRCLE,
        /// <summary>
        /// A list of linear segments
        /// </summary>
        POLYLINE,
        /// <summary>
        /// List of different types of curves joined
        /// </summary>
        POLYCURVE,
        /// <summary>
        /// A Non-Uniform Rational B-Spline
        /// </summary>
        NURBSCURVE
    }
}
