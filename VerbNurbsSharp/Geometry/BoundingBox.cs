using System;
using System.Collections.Generic;
using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.Geometry
{
    /// <summary>
    /// BoundingBox is an n-dimensional bounding box implementation. It is used by many of verb's intersection algorithms.
    /// The first point added to the BoundingBox using BoundingBox.add will be used to define the dimensionality of the bounding box.
    /// 
    /// </summary>
    public class BoundingBox
    {
        private static readonly BoundingBox _unset = new BoundingBox(Vector3.Unset);
        private bool _initialized = false;
        private int _dim = 3;

        /// <summary>
        /// Points to add, if desired.  Otherwise, will not be _initialized until add is called.
        /// </summary>
        /// <param name="pts"></param>
        public BoundingBox(IList<Vector3> pts)
        {
            if (pts != null)
                this.AddRange(pts);
        }
        /// <summary>
        /// Create a bounding box _initialized with a single element.
        /// </summary>
        /// <param name="pt"></param>
        public BoundingBox(Vector3 pt)
        {
            this.Add(pt);
        }
        /// <summary>
        /// The minimum point of the BoundingBox - the coordinates of this point are always <= max.
        /// </summary>
        public Vector3 Min { get; set; }
        /// <summary>
        /// The maximum point of the BoundingBox. The coordinates of this point are always >= min.
        /// </summary>
        public Vector3 Max { get; set; }
        /// <summary>
        /// Gets a bounding box that has Unset coordinates for Min and Max.
        /// </summary>
        public static BoundingBox Unset
        {
            get
            {
                _unset._initialized = false;
                return _unset;
            }
            
        }
        /// <summary>
        /// If the Bounding box is initialized is a bounding box valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this._initialized;
            }

        }
        /// <summary>
        /// Adds a point to the bounding box, expanding the bounding box if the point is outside of it.
        /// If the bounding box is not _initialized, this method has that side effect.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public BoundingBox Add(Vector3 pt)
        {
            if (!this.IsValid)
            {
                this._dim = pt.Count;
                this.Min = new Vector3(){pt[0],pt[1],pt[2]};
                this.Max = new Vector3(){pt[0],pt[1],pt[2]};
                this._initialized = true;
                return this;
            }

            for (int i = 0; i < this._dim; i++)
            {
                if (pt[i] > this.Max[i])
                    this.Max[i] = pt[i];
                if (pt[i] < this.Min[i])
                    this.Min[i] = pt[i];
            }
            return this;
        }
        /// <summary>
        /// Add an array of points to the bounding box.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        public BoundingBox AddRange(IList<Vector3> pts)
        {
            foreach (var t in pts)
                this.Add(t);
            return this;
        }
        /// <summary>
        /// Clear the bounding box, leaving it in an uninitialized state.  Call add, addRange in order to initialize.
        /// </summary>
        /// <returns></returns>
        public BoundingBox Clear()
        {
            this._initialized = false;
            return this;
        }
        /// <summary>
        /// Get length of given axis.
        /// </summary>
        /// <param name="i">Index of axis to inspect (between 0 and 2)</param>
        /// <returns></returns>
        public double GetAxisLength(int i)
        {
            if (i < 0 || i > this._dim - 1) return 0.0;
            return Math.Abs(this.Min[i] - this.Max[i]);
        }
        /// <summary>
        /// Get longest axis of bounding box.
        /// Value 0 = X, 1 = Y, 2 = Z.
        /// </summary>
        /// <returns></returns>
        public int GetLongestAxis()
        {
            double max = double.MinValue;
            int axisIndex = 0;

            for (int i = 0; i < _dim; i++)
            {
                double axisLength = this.GetAxisLength(i);
                if (axisLength > max)
                {
                    max = axisLength;
                    axisIndex = i;
                }
            }

            return axisIndex;
        }
        /// <summary>
        /// Constructs the string representation of this aligned bounding box.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Min} - {this.Max}";
        }
        /// <summary>
        /// Determines if two BoundingBoxes intersect.
        /// </summary>
        /// <param name="bBox1">First BoundingBox</param>
        /// <param name="bBox2">Second BoundingBox</param>
        /// <param name="tol">Tolerance</param>
        /// <returns></returns>
        // ToDo this method can be simplified
        // first way: https://stackoverflow.com/questions/20925818/algorithm-to-check-if-two-boxes-overlap
        // Second way: replacing IsValid, as a method that check the validity of a BoundingBox, as in Rhino.
        public static bool AreOverlapping(BoundingBox bBox1, BoundingBox bBox2, double tol)
        {
            if (!bBox1.IsValid || !bBox2.IsValid) return false;
            tol = tol < -0.5 ? Constants.TOLERANCE : tol;
            int count = 0; 
            for (int i = 0; i < bBox1._dim; i++)
            {
                double x1 = Math.Min(bBox1.Min[i], bBox1.Max[i]) - tol;
                double x2 = Math.Max(bBox1.Min[i], bBox1.Max[i]) + tol;
                double y1 = Math.Min(bBox2.Min[i], bBox2.Max[i]) - tol;
                double y2 = Math.Max(bBox2.Min[i], bBox2.Max[i]) + tol;

                if ((x1 >= y1 && x1 <= y2) || (x2 >= y1 && x2 <= y2) || (y1 >= x1 && y1 <= x2) ||
                    (y2 >= x1 && y2 <= x2) == true)
                    count++;
            }
            return count == 3;
        }

        /// <summary>
        /// Tests a point for BoundingBox inclusion.
        /// </summary>
        /// <param name="pt">Vector3 to test</param>
        /// <param name="strict">If true, the point needs to be fully on the inside of the BoundingBox. I.e. coincident points will be considered 'outside'.</param>
        /// <returns></returns>
        public bool Contains(Vector3 pt, bool strict)
        {
            if (pt == null) return false;
            if (!this.IsValid) return false;

            if (strict)
            {
                if (pt[0] <= this.Min[0] || pt[0] >= this.Max[0] || pt[1] <= this.Min[1] || pt[1] >= this.Max[1] || pt[2] <= this.Min[2] || pt[2] >= this.Max[2])
                    return false;
            }
            else if (pt[0] < this.Min[0] || pt[0] > this.Max[0] || pt[1] < this.Min[1] || pt[1] > this.Max[1] || pt[2] < this.Min[2] || pt[2] > this.Max[2])
                return false;
            return true;
        }
        /// <summary>
        /// Computes the intersection of two bounding boxes.
        /// If one of the two boundary is not valid, or the two BoundingBoxes do not intersect return an unset bounding box.
        /// </summary>
        /// <param name="other">BoundingBox to intersect with</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static BoundingBox Intersect(BoundingBox bBox1, BoundingBox bBox2)
        {
            BoundingBox bBox = BoundingBox.Unset;
            if (!bBox1.IsValid || !bBox2.IsValid) return bBox;
            if (!AreOverlapping(bBox1, bBox2, 0.0)) return bBox;

            Vector3 minPt = new Vector3();
            Vector3 maxPt = new Vector3();
            minPt.Add(bBox1.Min[0] >= bBox2.Min[0] ? bBox1.Min[0] : bBox2.Min[0]);
            minPt.Add(bBox1.Min[1] >= bBox2.Min[1] ? bBox1.Min[1] : bBox2.Min[1]);
            minPt.Add(bBox1.Min[2] >= bBox2.Min[2] ? bBox1.Min[2] : bBox2.Min[2]);

            maxPt.Add(bBox1.Max[0] <= bBox2.Max[0] ? bBox1.Max[0] : bBox2.Max[0]);
            maxPt.Add(bBox1.Max[1] <= bBox2.Max[1] ? bBox1.Max[1] : bBox2.Max[1]);
            maxPt.Add(bBox1.Max[2] <= bBox2.Max[2] ? bBox1.Max[2] : bBox2.Max[2]);

            bBox._initialized = true;
            bBox.Min = minPt;
            bBox.Max = maxPt;

            return bBox;
        }
        /// <summary>
        /// Compute the boolean union of this with another BoundingBox
        /// </summary>
        /// <param name="other">BoundingBox to union with</param>
        /// <returns></returns>
        public BoundingBox Union(BoundingBox other)
        {
            return Union(this, other);
        }
        /// <summary>
        /// Compute the boolean union between two BoundingBoxes.
        /// </summary>
        /// <param name="bBox1">First BoundingBox</param>
        /// <param name="bBox2">Second BoundingBox</param>
        /// <returns></returns>
        public static BoundingBox Union(BoundingBox bBox1, BoundingBox bBox2)
        {
            if (!bBox1.IsValid) return bBox2;
            if (!bBox2.IsValid) return bBox1;

            BoundingBox bBox = BoundingBox.Unset;
            Vector3 minPt = new Vector3();
            Vector3 maxPt = new Vector3();
            minPt.Add(bBox1.Min[0] < bBox2.Min[0] ? bBox1.Min[0] : bBox2.Min[0]);
            minPt.Add(bBox1.Min[1] < bBox2.Min[1] ? bBox1.Min[1] : bBox2.Min[1]);
            minPt.Add(bBox1.Min[2] < bBox2.Min[2] ? bBox1.Min[2] : bBox2.Min[2]);

            maxPt.Add(bBox1.Max[0] > bBox2.Max[0] ? bBox1.Max[0] : bBox2.Max[0]);
            maxPt.Add(bBox1.Max[1] > bBox2.Max[1] ? bBox1.Max[1] : bBox2.Max[1]);
            maxPt.Add(bBox1.Max[2] > bBox2.Max[2] ? bBox1.Max[2] : bBox2.Max[2]);

            bBox._initialized = true;
            bBox.Min = minPt;
            bBox.Max = maxPt;

            return bBox;
        }
    }
}
