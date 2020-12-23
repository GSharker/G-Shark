using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// BoundingBox is an n-dimensional bounding box implementation. It is used by many of verb's intersection algorithms.
    /// The first point added to the BoundingBox using BoundingBox.add will be used to define the dimensionality of the bounding box.
    /// 
    /// </summary>
    public class BoundingBox
    {
        private static readonly BoundingBox _unset = new BoundingBox(Point.Unset);
        private bool _initialized = false;
        private int _dim = 3;

        /// <summary>
        /// Points to add, if desired.  Otherwise, will not be _initialized until add is called.
        /// </summary>
        /// <param name="pts"></param>
        public BoundingBox(IList<Point> pts)
        {
            if (pts != null)
                this.AddRange(pts);
        }
        /// <summary>
        /// Create a bounding box _initialized with a single element.
        /// </summary>
        /// <param name="pt"></param>
        public BoundingBox(Point pt)
        {
            this.Add(pt);
        }
        /// <summary>
        /// The minimum point of the BoundingBox - the coordinates of this point are always <= max.
        /// </summary>
        public Point Min { get; set; }
        /// <summary>
        /// The maximum point of the BoundingBox. The coordinates of this point are always >= min.
        /// </summary>
        public Point Max { get; set; }
        /// <summary>
        /// Gets a bounding box that has Unset coordinates for Min and Max.
        /// </summary>
        public static BoundingBox Unset
        {
            get
            {
                return BoundingBox.Unset;
            }
            
        }
        /// <summary>
        /// Adds a point to the bounding box, expanding the bounding box if the point is outside of it.
        /// If the bounding box is not _initialized, this method has that side effect.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public BoundingBox Add(Point pt)
        {
            if (this._initialized == false)
            {
                this._dim = pt.Count;
                this.Min = new Point(){pt[0],pt[1],pt[2]};
                this.Max = new Point(){pt[0],pt[1],pt[2]};
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
        public BoundingBox AddRange(IList<Point> pts)
        {
            foreach (var t in pts)
                this.Add(t);
            return this;
        }
        /// <summary>
        /// Tests a point for BoundingBox inclusion.
        /// </summary>
        /// <param name="pt">Point to test</param>
        /// <param name="strict">If true, the point needs to be fully on the inside of the BoundingBox. I.e. coincident points will be considered 'outside'.</param>
        /// <returns></returns>
        public bool Contains(Point pt, bool strict)
        {
            if (pt == null) return false;
            if (!this._initialized) return false;

            if (strict)
            {
                if (pt[0] <= this.Min[0] || pt[0] >= this.Max[0] || (pt[1] <= this.Min[1] || pt[1] >= this.Max[1]) || (pt[2] <= this.Min[2] || 2 >= this.Max[2]))
                    return false;
            }
            else
            if (pt[0] < this.Min[0] || pt[0] > this.Max[0] || (pt[1] < this.Min[1] || pt[1] > this.Max[1]) || (pt[2] < this.Min[2] || 2 > this.Max[2]))
                return false;
            return true;
        }

        public static BoundingBox Intersect(BoundingBox bBox1, BoundingBox bBox2)
        {
            return BoundingBox.Unset;
        }
    }
}
