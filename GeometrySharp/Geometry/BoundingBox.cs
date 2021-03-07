using System;
using System.Collections.Generic;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo: BoundingBox align to a plane.
    // ToDo: Minimum BoundingBox.
    /// <summary>
    /// Represents the value of two points in a bounding box
    /// defined by the two extreme corner points.
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// Constructs a new bounding box from two corner points.
        /// </summary>
        /// <param name="min">Point containing all the minimum coordinates.</param>
        /// <param name="max">Point containing all the maximum coordinates.</param>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Constructs a BoundingBox from a list of points.
        /// </summary>
        /// <param name="pts">Collection of points will be contained in the BoundingBox.</param>
        public BoundingBox(IList<Vector3> pts)
        {
            Vector3 min = new Vector3 { double.MaxValue, double.MaxValue, double.MaxValue };
            Vector3 max = new Vector3 { double.MinValue, double.MinValue, double.MinValue };
            bool flag = true;

            foreach (Vector3 pt in pts)
            {
                if (pt[0] < min[0]) min[0] = pt[0];
                if (pt[1] < min[1]) min[1] = pt[1];
                if (pt[2] < min[2]) min[2] = pt[2];

                if (pt[0] > max[0]) max[0] = pt[0];
                if (pt[1] > max[1]) max[1] = pt[1];
                if (pt[2] > max[2]) max[2] = pt[2];
            }

            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// The minimum point of the BoundingBox - the coordinates of this point are always <= max.
        /// </summary>
        public Vector3 Min { get; }

        /// <summary>
        /// The maximum point of the BoundingBox. The coordinates of this point are always >= min.
        /// </summary>
        public Vector3 Max { get; }

        /// <summary>
        /// Gets a BoundingBox that has Unset coordinates for Min and Max.
        /// </summary>
        public static BoundingBox Unset { get; } = new BoundingBox(Vector3.Unset, Vector3.Unset);

        /// <summary>
        /// Gets if the BoundingBox is valid.
        /// </summary>
        public bool IsValid => this.Min.IsValid() && this.Max.IsValid() &&
                               (this.Min[0] <= this.Max[0] && this.Min[1] <= this.Max[1]) &&
                               this.Min[2] <= this.Max[2];

        /// <summary>
        /// Gets length of given axis.
        /// </summary>
        /// <param name="i">Index of axis to inspect (between 0 and 2)</param>
        /// <returns>Return the value length of the axis.</returns>
        public double GetAxisLength(int i)
        {
            if (i < 0 || i > 2) return 0.0;
            return Math.Abs(Min[i] - Max[i]);
        }

        /// <summary>
        /// Gets longest axis of bounding box.
        /// Value 0 = X, 1 = Y, 2 = Z.
        /// </summary>
        /// <returns>Return the value of the longest axis of BoundingBox.</returns>
        public int GetLongestAxis()
        {
            var max = double.MinValue;
            var axisIndex = 0;

            for (var i = 0; i < 3; i++)
            {
                var axisLength = GetAxisLength(i);
                if (axisLength > max)
                {
                    max = axisLength;
                    axisIndex = i;
                }
            }

            return axisIndex;
        }

        // https://stackoverflow.com/questions/20925818/algorithm-to-check-if-two-boxes-overlap
        /// <summary>
        /// Determines if two BoundingBoxes overlapping.
        /// </summary>
        /// <param name="bBox1">First BoundingBox</param>
        /// <param name="bBox2">Second BoundingBox</param>
        /// <param name="tol">Tolerance</param>
        /// <returns>Return true if the BoundingBoxes are overlapping.</returns>
        public static bool AreOverlapping(BoundingBox bBox1, BoundingBox bBox2, double tol)
        {
            if (!bBox1.IsValid || !bBox2.IsValid) return false;
            tol = tol < -0.5 ? GeoSharpMath.MAXTOLERANCE : tol;
            var count = 0;
            for (var i = 0; i < 3; i++)
            {
                var x1 = Math.Min(bBox1.Min[i], bBox1.Max[i]) - tol;
                var x2 = Math.Max(bBox1.Min[i], bBox1.Max[i]) + tol;
                var y1 = Math.Min(bBox2.Min[i], bBox2.Max[i]) - tol;
                var y2 = Math.Max(bBox2.Min[i], bBox2.Max[i]) + tol;

                if (x1 >= y1 && x1 <= y2 || x2 >= y1 && x2 <= y2 || y1 >= x1 && y1 <= x2 ||
                    y2 >= x1 && y2 <= x2)
                    count++;
            }

            return count == 3;
        }

        /// <summary>
        /// Tests a point for BoundingBox inclusion.
        /// </summary>
        /// <param name="pt">Vector3 to test</param>
        /// <param name="strict">
        /// If true, the point needs to be fully on the inside of the BoundingBox. I.e. coincident points will
        /// be considered 'outside'.
        /// </param>
        /// <returns>Return true if the point is contained in the BoundingBox.</returns>
        public bool Contains(Vector3 pt, bool strict)
        {
            if (pt == null) return false;
            if (!IsValid) return false;

            if (strict)
            {
                if (pt[0] <= Min[0] || pt[0] >= Max[0] || pt[1] <= Min[1] || pt[1] >= Max[1] || pt[2] <= Min[2] ||
                    pt[2] >= Max[2])
                    return false;
            }
            else if (pt[0] < Min[0] || pt[0] > Max[0] || pt[1] < Min[1] || pt[1] > Max[1] || pt[2] < Min[2] ||
                     pt[2] > Max[2])
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Computes the intersection of two bounding boxes.
        /// If one of the two boundary is not valid, or the two BoundingBoxes do not intersect return an unset bounding box.
        /// </summary>
        /// <param name="other">BoundingBox to intersect with</param>
        /// <returns>Return the BoundingBox intersection between the two BoundingBox.</returns>
        public BoundingBox Intersect(BoundingBox other)
        {
            return Intersect(this, other);
        }

        /// <summary>
        /// Computes the intersection of two bounding boxes.
        /// If one of the two boundary is not valid, or the two BoundingBoxes do not intersect return an unset bounding box.
        /// </summary>
        /// <param name="bBox1">First BoundingBox</param>
        /// <param name="bBox2">Second BoundingBox</param>
        /// <returns>Return the BoundingBox intersection between the two BoundingBox.</returns>
        public static BoundingBox Intersect(BoundingBox bBox1, BoundingBox bBox2)
        {
            if (!bBox1.IsValid || !bBox2.IsValid) return Unset;
            if (!AreOverlapping(bBox1, bBox2, 0.0)) return Unset;

            var minPt = new Vector3();
            var maxPt = new Vector3();
            minPt.Add(bBox1.Min[0] >= bBox2.Min[0] ? bBox1.Min[0] : bBox2.Min[0]);
            minPt.Add(bBox1.Min[1] >= bBox2.Min[1] ? bBox1.Min[1] : bBox2.Min[1]);
            minPt.Add(bBox1.Min[2] >= bBox2.Min[2] ? bBox1.Min[2] : bBox2.Min[2]);

            maxPt.Add(bBox1.Max[0] <= bBox2.Max[0] ? bBox1.Max[0] : bBox2.Max[0]);
            maxPt.Add(bBox1.Max[1] <= bBox2.Max[1] ? bBox1.Max[1] : bBox2.Max[1]);
            maxPt.Add(bBox1.Max[2] <= bBox2.Max[2] ? bBox1.Max[2] : bBox2.Max[2]);

            return new BoundingBox(minPt, maxPt);
        }

        /// <summary>
        /// Compute the boolean union of this with another BoundingBox
        /// </summary>
        /// <param name="other">BoundingBox to union with</param>
        /// <returns>Return the BoundingBox union between the two BoundingBox</returns>
        public BoundingBox Union(BoundingBox other)
        {
            return Union(this, other);
        }

        /// <summary>
        /// Compute the boolean union between two BoundingBoxes.
        /// </summary>
        /// <param name="bBox1">First BoundingBox</param>
        /// <param name="bBox2">Second BoundingBox</param>
        /// <returns>Return the BoundingBox union between the two BoundingBox</returns>
        public static BoundingBox Union(BoundingBox bBox1, BoundingBox bBox2)
        {
            if (!bBox1.IsValid) return bBox2;
            if (!bBox2.IsValid) return bBox1;

            var minPt = new Vector3();
            var maxPt = new Vector3();
            minPt.Add(bBox1.Min[0] < bBox2.Min[0] ? bBox1.Min[0] : bBox2.Min[0]);
            minPt.Add(bBox1.Min[1] < bBox2.Min[1] ? bBox1.Min[1] : bBox2.Min[1]);
            minPt.Add(bBox1.Min[2] < bBox2.Min[2] ? bBox1.Min[2] : bBox2.Min[2]);

            maxPt.Add(bBox1.Max[0] > bBox2.Max[0] ? bBox1.Max[0] : bBox2.Max[0]);
            maxPt.Add(bBox1.Max[1] > bBox2.Max[1] ? bBox1.Max[1] : bBox2.Max[1]);
            maxPt.Add(bBox1.Max[2] > bBox2.Max[2] ? bBox1.Max[2] : bBox2.Max[2]);

            return new BoundingBox(minPt, maxPt);
        }

        /// <summary>
        /// Ensures that the box is defined in an increasing fashion.
        /// If the Min or Max points are unset, this function will return a BoundingBox unset.
        /// </summary>
        /// <returns>A BoundingBox made valid.</returns>
        public BoundingBox MakeItValid()
        {
            if(!this.Min.IsValid() || !this.Max.IsValid())
                return BoundingBox.Unset;

            var x1 = Math.Min(Min[0], Max[0]);
            var y1 = Math.Min(Min[1], Max[1]);
            var z1 = Math.Min(Min[2], Max[2]);

            var x2 = Math.Max(Min[0], Max[0]);
            var y2 = Math.Max(Min[1], Max[1]);
            var z2 = Math.Max(Min[2], Max[2]);

            var min = new Vector3 {x1, y1, z1};
            var max = new Vector3 { x2, y2, z2 };

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Constructs the string representation of this aligned bounding box.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Min} - {Max}";
        }
    }
}