using System.Collections.Generic;

namespace GShark.Geometry
{
    /// <summary>
    ///     Half-edge mesh face class.
    /// </summary>
    public class MeshFace
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MeshFace" /> class.
        /// </summary>
        public MeshFace()
        {
            HalfEdge = null;
            Index = -1;
        }


        /// <summary>
        ///     Gets or sets one of the  half-edges surrounding the face.
        /// </summary>
        public MeshHalfEdge HalfEdge { get; set; }

        /// <summary>
        ///     Gets or sets the face index on the mesh face list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Gets the area of the face.
        /// </summary>
        /// <returns>Returns the value of the face area.</returns>
        public double Area => MeshGeometry.Area(this);

        /// <summary>
        ///     Gets the normal vector of the face.
        /// </summary>
        /// <returns>Returns the perpendicular vector to the face.</returns>
        public Vector3 Normal => MeshGeometry.FaceNormal(this);


        /// <summary>
        ///     Get all adjacent edges to this face.
        /// </summary>
        /// <returns>Returns a list of all adjacent edges in order.</returns>
        public List<MeshEdge> AdjacentEdges()
        {
            var edge = HalfEdge;
            var edges = new List<MeshEdge>();
            do
            {
                edges.Add(edge.Edge);
                edge = edge.Next;
            } while (edge != HalfEdge);

            return edges;
        }


        /// <summary>
        ///     Get all adjacent half-edges to this face.
        /// </summary>
        /// <returns>Returns a list of all adjacent half-edges in order.</returns>
        public List<MeshHalfEdge> AdjacentHalfEdges()
        {
            var edge = HalfEdge;
            var halfEdges = new List<MeshHalfEdge>();
            do
            {
                halfEdges.Add(edge);
                edge = edge.Next;
            } while (edge != HalfEdge);

            return halfEdges;
        }


        /// <summary>
        ///     Get all adjacent vertices to this face.
        /// </summary>
        /// <returns>Returns a list of all adjacent vertices in order.</returns>
        public List<MeshVertex> AdjacentVertices()
        {
            var vertices = new List<MeshVertex>();
            var edge = HalfEdge;
            do
            {
                vertices.Add(edge.Vertex);
                edge = edge.Next;
            } while (edge != HalfEdge);

            return vertices;
        }


        /// <summary>
        ///     Get all adjacent faces to this face.
        /// </summary>
        /// <returns>Returns a list of all adjacent faces in order.</returns>
        public List<MeshFace> AdjacentFaces()
        {
            var faces = new List<MeshFace>();
            var edge = HalfEdge;
            do
            {
                if (!edge.Twin.Face.IsBoundaryLoop() && !faces.Contains(edge.Twin.Face))
                    faces.Add(edge.Twin.Face);
                edge = edge.Next;
            } while (edge != HalfEdge);

            return faces;
        }


        /// <summary>
        ///     Get all adjacent corners to this face.
        /// </summary>
        /// <returns>Returns a list of all adjacent corners in order.</returns>
        public List<MeshCorner> AdjacentCorners()
        {
            var corners = new List<MeshCorner>();
            var edge = HalfEdge;
            do
            {
                corners.Add(edge.Corner);
                edge = edge.Next;
            } while (edge != HalfEdge);

            return corners;
        }


        /// <summary>
        ///     Checks if the current face is a boundary face.
        /// </summary>
        /// <returns>Returns true if the face is a boundary face, false if not.</returns>
        public bool IsBoundaryLoop() => HalfEdge.OnBoundary;


        /// <summary>
        ///     Convert the mesh face to string.
        /// </summary>
        /// <returns>Returns the string representation of the mesh face.</returns>
        public override string ToString()
        {
            var faceVertices = AdjacentVertices();
            var text = "F";
            foreach (var v in faceVertices)
            {
                text += " ";
                text += v.Index;
            }

            return text;
        }
    }
}