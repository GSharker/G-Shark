using GShark.Geometry;
using System;

namespace GShark.Core.BoundingBoxTree
{
    internal interface IBoundingBoxTree<T>
    {
        /// <summary>
        /// Gets the bounding box of object.
        /// </summary>
        public BoundingBox BoundingBox();

        /// <summary>
        /// Splits the object in two parts.
        /// </summary>
        /// <returns>A tuple containing the two split parts.</returns>
        public Tuple<IBoundingBoxTree<T>, IBoundingBoxTree<T>> Split();

        /// <summary>
        /// Get the object.
        /// </summary>
        public T Yield();

        /// <summary>
        /// Checks if the object is divisible.
        /// </summary>
        /// <param name="tolerance">A tolerance value used for the comparison.</param>
        /// <returns>True whether the object is not more divisible.</returns>
        public bool IsIndivisible(double tolerance);


        /// <summary>
        /// Gets if the bounding box tree is empty.
        /// </summary>
        public bool IsEmpty();
    }
}
