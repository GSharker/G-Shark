using System;
using System.Collections.Generic;
using GShark.Geometry;
using GShark.Interfaces;

namespace GShark.Intersection.BoundingBoxTree
{
    /// <summary>
    /// A collection of tools operating on bounding box trees.
    /// </summary>
    internal static class BoundingBoxOperations
    {
        /// <summary>
        /// The core algorithm for bounding box tree intersection.<br/>
        /// Supporting both lazy and pre-computed bounding box trees via the <see cref="IBoundingBoxTree{T}"/> interface.
        /// </summary>
        /// <param name="bbt1">The first Bounding box tree object.</param>
        /// <param name="bbt2">The second Bounding box tree object.</param>
        /// <param name="tolerance">Tolerance as per default set as 1e-9.</param>
        /// <returns>A collection of tuples extracted from the Yield method of the BoundingBoxTree.</returns>
        internal static List<Tuple<T1, T2>> BoundingBoxTreeIntersection<T1, T2>(IBoundingBoxTree<T1> bbt1, IBoundingBoxTree<T2> bbt2,
            double tolerance = 1e-9)
        {
            List<IBoundingBoxTree<T1>> aTrees = new List<IBoundingBoxTree<T1>>();
            List<IBoundingBoxTree<T2>> bTrees = new List<IBoundingBoxTree<T2>>();

            aTrees.Add(bbt1);
            bTrees.Add(bbt2);

            return GetRoot(aTrees, bTrees, tolerance);
        }

        /// <summary>
        /// The core algorithm for bounding box tree intersection.<br/>
        /// Supporting both lazy and pre-computed bounding box trees via the <see cref="IBoundingBoxTree{T}"/> interface.
        /// </summary>
        /// <param name="bbt1">The first Bounding box tree object.</param>
        /// <param name="tolerance">Tolerance as per default set as 1e-9.</param>
        /// <returns>A collection of tuples extracted from the Yield method of the BoundingBoxTree.</returns>
        internal static List<Tuple<T1, T1>> BoundingBoxTreeIntersection<T1>(IBoundingBoxTree<T1> bbt1,
            double tolerance = 1e-9)
        {
            List<IBoundingBoxTree<T1>> aTrees = new List<IBoundingBoxTree<T1>>();
            List<IBoundingBoxTree<T1>> bTrees = new List<IBoundingBoxTree<T1>>();
            Tuple<IBoundingBoxTree<T1>, IBoundingBoxTree<T1>> firstSplit = bbt1.Split();

            aTrees.Add(firstSplit.Item1);
            bTrees.Add(firstSplit.Item2);

            return GetRoot(aTrees, bTrees, tolerance);
        }

        /// <summary>
        /// The core algorithm to find the bounding box trees intersecting with a plane.<br/>
        /// Supporting both lazy and pre-computed bounding box trees via the <see cref="IBoundingBoxTree{T}"/> interface.
        /// </summary>
        /// <param name="bbt">The bounding box object.</param>
        /// <param name="pl">The plane to intersect with.</param>
        /// <param name="tolerance">Tolerance as per default set as 1e-9.</param>
        /// <returns>A collection of extracted object from the Yield method of the BoundingBoxTree.</returns>
        internal static List<T> BoundingBoxPlaneIntersection<T>(IBoundingBoxTree<T> bbt, Plane pl, double tolerance = 1e-9)
        {
            List<IBoundingBoxTree<T>> aTrees = new List<IBoundingBoxTree<T>> { bbt };
            List<T> result = new List<T>();

            while (aTrees.Count > 0)
            {
                IBoundingBoxTree<T> a = aTrees[aTrees.Count - 1];
                aTrees.RemoveAt(aTrees.Count - 1);

                if (a.IsEmpty())
                {
                    continue;
                }

                Tuple<IBoundingBoxTree<T>, IBoundingBoxTree<T>> aSplit = a.Split();

                var pt1 = a.BoundingBox().Max;
                var pt2 = a.BoundingBox().Min;
                _ = pl.ClosestPoint(pt1, out double h1);
                _ = pl.ClosestPoint(pt2, out double h2);

                if (Math.Abs(h1) < tolerance || Math.Abs(h2) < tolerance || h1 * h2 > 0.0)
                {
                    if (a.IsIndivisible(tolerance))
                    {
                        continue;
                    }
                    aTrees.Add(aSplit.Item1);
                    aTrees.Add(aSplit.Item2);
                }
                else
                {
                    if (a.IsIndivisible(tolerance))
                    {
                        result.Add(a.Yield());
                        continue;
                    }
                    aTrees.Add(aSplit.Item1);
                    aTrees.Add(aSplit.Item2);
                }
            }

            return result;
        }

        /// <summary>
        /// The core algorithm for bounding box tree intersection.<br/>
        /// Supporting both lazy and pre-computed bounding box trees via the <see cref="IBoundingBoxTree{T}"/> interface.
        /// </summary>
        /// <param name="aTrees">The first Bounding box tree object.</param>
        /// <param name="bTrees">The second Bounding box tree object.</param>
        /// <param name="tolerance">Tolerance as per default set as 1e-9.</param>
        /// <returns>A collection of tuples extracted from the Yield method of the BoundingBoxTree.</returns>
        private static List<Tuple<T1, T2>> GetRoot<T1, T2>(List<IBoundingBoxTree<T1>> aTrees, List<IBoundingBoxTree<T2>> bTrees, double tolerance)
        {
            List<Tuple<T1, T2>> result = new List<Tuple<T1, T2>>();

            while (aTrees.Count > 0)
            {
                IBoundingBoxTree<T1> a = aTrees[aTrees.Count - 1];
                aTrees.RemoveAt(aTrees.Count - 1);
                IBoundingBoxTree<T2> b = bTrees[bTrees.Count - 1];
                bTrees.RemoveAt(bTrees.Count - 1);

                if (a.IsEmpty() || b.IsEmpty())
                {
                    continue;
                }

                if (BoundingBox.AreOverlapping(a.BoundingBox(), b.BoundingBox(), tolerance) == false)
                {
                    continue;
                }

                bool aIndivisible = a.IsIndivisible(tolerance);
                bool bIndivisible = b.IsIndivisible(tolerance);
                Tuple<IBoundingBoxTree<T1>, IBoundingBoxTree<T1>> aSplit = a.Split();
                Tuple<IBoundingBoxTree<T2>, IBoundingBoxTree<T2>> bSplit = b.Split();

                if (aIndivisible && bIndivisible)
                {
                    result.Add(new Tuple<T1, T2>(a.Yield(), b.Yield()));
                    continue;
                }
                if (aIndivisible)
                {
                    aTrees.Add(a);
                    bTrees.Add(bSplit.Item2);
                    aTrees.Add(a);
                    bTrees.Add(bSplit.Item1);
                    continue;
                }
                if (bIndivisible)
                {
                    aTrees.Add(aSplit.Item2);
                    bTrees.Add(b);
                    aTrees.Add(aSplit.Item1);
                    bTrees.Add(b);
                    continue;
                }

                aTrees.Add(aSplit.Item2);
                bTrees.Add(bSplit.Item2);

                aTrees.Add(aSplit.Item2);
                bTrees.Add(bSplit.Item1);

                aTrees.Add(aSplit.Item1);
                bTrees.Add(bSplit.Item2);

                aTrees.Add(aSplit.Item1);
                bTrees.Add(bSplit.Item1);
            }

            return result;
        }

    }
}
