using System;
using GShark.Geometry;
using GShark.Intersection.BoundingBoxTree;

namespace GShark.Interfaces
{
    /// <summary>
    /// Interface defining the requirements used by <see cref="BoundingBoxOperations"/>.
    /// </summary>
    internal interface IBoundingBoxTree<T>
    {
        /// <summary>
        /// Gets the bounding box of object.
        /// </summary>
        BoundingBox BoundingBox();

        /// <summary>
        /// Splits the object in two parts.
        /// </summary>
        /// <returns>A tuple containing the two split parts.</returns>
        Tuple<IBoundingBoxTree<T>, IBoundingBoxTree<T>> Split();

        /// <summary>
        /// Get the object.
        /// </summary>
        T Yield();

        /// <summary>
        /// Checks if the object is divisible.
        /// </summary>
        /// <param name="tolerance">A tolerance value used for the comparison.</param>
        /// <returns>True whether the object is not more divisible.</returns>
        bool IsIndivisible(double tolerance);

        /// <summary>
        /// Gets if the bounding box tree is empty.
        /// </summary>
        bool IsEmpty();
    }
}
