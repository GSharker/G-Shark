namespace GShark.Geometry
{
    /// <summary>
    ///     Represents a mesh half-edge.
    /// </summary>
    public class MeshHalfEdge
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MeshHalfEdge" /> class.
        /// </summary>
        public MeshHalfEdge() => Index = -1;


        /// <summary>
        ///     Gets or sets the vertex linked to this half-edge.
        /// </summary>
        public MeshVertex Vertex { get; set; }

        /// <summary>
        ///     Gets or sets the edge linked to this half-edge.
        /// </summary>
        public MeshEdge Edge { get; set; }

        /// <summary>
        ///     Gets or sets the face linked to this half-edge.
        /// </summary>
        public MeshFace Face { get; set; }

        /// <summary>
        ///     Gets or sets the corner linked to this half-edge.
        /// </summary>
        public MeshCorner Corner { get; set; }

        /// <summary>
        ///     Gets or sets the next half-edge in a face.
        /// </summary>
        public MeshHalfEdge Next { get; set; }

        /// <summary>
        ///     Gets or sets the previous half-edge in a face.
        /// </summary>
        public MeshHalfEdge Prev { get; set; }

        /// <summary>
        ///     Gets or sets the opposite half-edge.
        /// </summary>
        public MeshHalfEdge Twin { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the half-edge lies on a boundary.
        /// </summary>
        public bool OnBoundary { get; set; }

        /// <summary>
        ///     Gets or sets the half-edge index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Gets the previous vertex of the half-edge.
        /// </summary>
        public MeshVertex PreviousVertex => Twin.Vertex;

        /// <summary>
        ///     Gets the opposite face of the half-edge.
        /// </summary>
        public MeshFace AdjacentFace => Twin.Face;


        /// <summary>
        ///     Gets the string representation of the half-edge.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "Half-edge " + Index;
    }
}