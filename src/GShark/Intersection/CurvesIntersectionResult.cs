﻿using GShark.Geometry;

namespace GShark.Intersection
{
    /// <summary>
    /// This is a POC class used to collect the result from curves intersection.
    /// </summary>
    public class CurvesIntersectionResult
    {
        /// <summary>
        /// Collects the result from an intersection.
        /// </summary>
        /// <param name="pt0">First curve's intersection point.</param>
        /// <param name="pt1">Second curve's intersection point.</param>
        /// <param name="t0">First curve's t parameter on curve.</param>
        /// <param name="t1">Second curve's t parameter on curve.</param>
        public CurvesIntersectionResult(Point3 pt0, Point3 pt1, double t0, double t1)
        {
            Pt0 = pt0;
            Pt1 = pt1;
            T0 = t0;
            T1 = t1;
        }

        /// <summary>
        /// Gets the first curve's intersection point.
        /// </summary>
        public Point3 Pt0 { get; }

        /// <summary>
        /// Gets the second curve's intersection point.
        /// </summary>
        public Point3 Pt1 { get; }

        /// <summary>
        /// Gets the first curve's t parameter on curve.
        /// </summary>
        public double T0 { get; }

        /// <summary>
        /// Gets the second curve's t parameter on curve.
        /// </summary>
        public double T1 { get; }

        /// <summary>
        /// Compose the curve intersection result as a text.
        /// </summary>
        /// <returns>The text format of the intersection.</returns>
        public override string ToString()
        {
            return $"pt0: {Pt0} - t0: {T0} - pt1: {Pt1} - t1: {T1}";
        }
    }
}
