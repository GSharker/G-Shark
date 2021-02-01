using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;

// ToDo this class has to be commented.
// ToDo this class ha to be tested.
// ToDo this class has to be review in the naming.
namespace GeometrySharp.Core.KDTree
{
    public class KDTree<T>
    {
        public List<KDPoints<T>> Points { get; set; }
        //private var distanceFunction : Vector3 -> Vector3 -> Float;

        public int Dim { get; set; } = 3;
        public KDNode<T> Root { get; set; }

        public KDTree(List<KDPoints<T>> points)
        {
            Points = points;
            Dim = points[0].Vector3.Count;
            //this.distanceFunction = distanceFunction;
            Root = BuildTree(points, 0, null);
        }

        private KDNode<T> BuildTree(List<KDPoints<T>> points, int depth, KDNode<T> parent) => throw new NotImplementedException();
    }

    /// <summary>
    /// A node in a KdTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KDNode<T>
    {
        /// <summary>
        /// The point itself
        /// </summary>
        public KDPoints<T> KDPoint { get; set; }

        /// <summary>
        /// The left child
        /// </summary>
        public KDNode<T> Left { get; set; }

        /// <summary>
        /// The right child
        /// </summary>
        public KDNode<T> Right { get; set; }

        /// <summary>
        /// The parent of the node
        /// </summary>
        public KDNode<T> Parent { get; set; }

        /// <summary>
        /// The dimensionality of the point
        /// </summary>
        public int Dimension { get; set; }

        public KDNode(KDPoints<T> kDPoint, int dimension, KDNode<T> parent)
        {
            KDPoint = kDPoint;
            Dimension = dimension;
            Parent = parent;
            Left = null;
            Right = null;
        }
    }

    /// <summary>
    /// A point in a KdTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KDPoints<T>
    {
        /// <summary>
        /// The point
        /// </summary>
        public Vector3 Vector3 { get; set; }

        /// <summary>
        /// An arbitrary object to attach
        /// </summary>
        public T Obj { get; set; }

        public KDPoints(Vector3 point, T obj)
        {
            Vector3 = point;
            Obj = obj;
        }
    }
}
