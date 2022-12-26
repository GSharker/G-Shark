using System.Collections.Generic;

namespace GShark.Geometry
{
    /// <summary>
    ///     Topology explorer for meshes. Contains all methods to explore mesh connections between members.
    /// </summary>
    public class MeshTopology
    {
        // Per Vertex adjacency index lists
        // Returns 2 dimensional array: 1 array per vertex index containing an array with the corresponding adjacent member index
        private readonly Mesh mesh;


        /// <summary>
        ///     Initializes a new instance of the <see cref="MeshTopology" /> class.
        /// </summary>
        /// <param name="mesh">Mesh to construct topology connections from.</param>
        public MeshTopology(Mesh mesh)
        {
            this.mesh = mesh;

            VertexVertex = new Dictionary<int, List<int>>();
            VertexFaces = new Dictionary<int, List<int>>();
            VertexEdges = new Dictionary<int, List<int>>();
            FaceVertex = new Dictionary<int, List<int>>();
            FaceFace = new Dictionary<int, List<int>>();
            FaceEdge = new Dictionary<int, List<int>>();
            EdgeVertex = new Dictionary<int, List<int>>();
            EdgeFace = new Dictionary<int, List<int>>();
            EdgeEdge = new Dictionary<int, List<int>>();
            
            ComputeEdgeAdjacency();
            ComputeFaceAdjacency();
            ComputeVertexAdjacency();
        }


        /// <summary>
        ///     Gets vertex-Vertex topological connections.
        /// </summary>
        public Dictionary<int, List<int>> VertexVertex { get; }

        /// <summary>
        ///     Gets vertex-Face topological connections.
        /// </summary>
        public Dictionary<int, List<int>> VertexFaces { get; }

        /// <summary>
        ///     Gets vertex-Edge topological connections.
        /// </summary>
        public Dictionary<int, List<int>> VertexEdges { get; }

        /// <summary>
        ///     Gets edge-Edge topological connections.
        /// </summary>
        public Dictionary<int, List<int>> EdgeEdge { get; }

        /// <summary>
        ///     Gets edge-Vertex topological connections.
        /// </summary>
        public Dictionary<int, List<int>> EdgeVertex { get; }

        /// <summary>
        ///     Gets edge-Face topological connections.
        /// </summary>
        public Dictionary<int, List<int>> EdgeFace { get; }

        /// <summary>
        ///     Gets face-Vertex topological connections.
        /// </summary>
        public Dictionary<int, List<int>> FaceVertex { get; }

        /// <summary>
        ///     Gets face-Edge topological connections.
        /// </summary>
        public Dictionary<int, List<int>> FaceEdge { get; }

        /// <summary>
        ///     Gets face-Face topological connections.
        /// </summary>
        public Dictionary<int, List<int>> FaceFace { get; }


        /// <summary>
        ///     Computes vertex adjacency for the whole mesh and stores it in the appropriate dictionaries.
        /// </summary>
        public void ComputeVertexAdjacency()
        {
            foreach (var vertex in mesh.Vertices)
            {
                foreach (var adjacent in vertex.AdjacentVertices())
                {
                    if (!VertexVertex.ContainsKey(vertex.Index))
                        VertexVertex.Add(vertex.Index, new List<int> {adjacent.Index});
                    else
                        VertexVertex[vertex.Index].Add(adjacent.Index);
                }

                foreach (var adjacent in vertex.AdjacentFaces())
                {
                    if (!VertexFaces.ContainsKey(vertex.Index))
                        VertexFaces.Add(vertex.Index, new List<int> {adjacent.Index});
                    else
                        VertexFaces[vertex.Index].Add(adjacent.Index);
                }

                foreach (var adjacent in vertex.AdjacentEdges())
                {
                    if (!VertexEdges.ContainsKey(vertex.Index))
                        VertexEdges.Add(vertex.Index, new List<int> {adjacent.Index});
                    else
                        VertexEdges[vertex.Index].Add(adjacent.Index);
                }
            }
        }


        /// <summary>
        ///     Computes face adjacency for the whole mesh and stores it in the appropriate dictionaries.
        /// </summary>
        public void ComputeFaceAdjacency()
        {
            foreach (var face in mesh.Faces)
            {
                foreach (var adjacent in face.AdjacentVertices())
                {
                    if (!FaceVertex.ContainsKey(face.Index))
                        FaceVertex.Add(face.Index, new List<int> {adjacent.Index});
                    else
                        FaceVertex[face.Index].Add(adjacent.Index);
                }

                foreach (var adjacent in face.AdjacentFaces())
                {
                    if (!FaceFace.ContainsKey(face.Index))
                        FaceFace.Add(face.Index, new List<int> {adjacent.Index});
                    else
                        FaceFace[face.Index].Add(adjacent.Index);
                }

                foreach (var adjacent in face.AdjacentEdges())
                {
                    if (!FaceEdge.ContainsKey(face.Index))
                        FaceEdge.Add(face.Index, new List<int> {adjacent.Index});
                    else
                        FaceEdge[face.Index].Add(adjacent.Index);
                }
            }
        }


        /// <summary>
        ///     Computes edge adjacency for the whole mesh and stores it in the appropriate dictionaries.
        /// </summary>
        public void ComputeEdgeAdjacency()
        {
            foreach (var edge in mesh.Edges)
            {
                foreach (var adjacent in edge.AdjacentVertices())
                {
                    if (!EdgeVertex.ContainsKey(edge.Index))
                        EdgeVertex.Add(edge.Index, new List<int> {adjacent.Index});
                    else
                        EdgeVertex[edge.Index].Add(adjacent.Index);
                }

                foreach (var adjacent in edge.AdjacentFaces())
                {
                    if (!EdgeFace.ContainsKey(edge.Index))
                        EdgeFace.Add(edge.Index, new List<int> {adjacent.Index});
                    else
                        EdgeFace[edge.Index].Add(adjacent.Index);
                }

                foreach (var adjacent in edge.AdjacentEdges())
                {
                    if (!EdgeEdge.ContainsKey(edge.Index))
                        EdgeEdge.Add(edge.Index, new List<int> {adjacent.Index});
                    else
                        EdgeEdge[edge.Index].Add(adjacent.Index);
                }
            }
        }


        /// <summary>
        ///     Gets the string representation of a given topology dictionary.
        /// </summary>
        /// <param name="dict">Dictionary to convert.</param>
        /// <returns></returns>
        public string TopologyDictToString(Dictionary<int, List<int>> dict)
        {
            var finalString = string.Empty;

            foreach (var pair in dict)
            {
                var tmpString = "Key: " + pair.Key + " --> ";
                foreach (var i in pair.Value)
                    tmpString += i + " ";

                tmpString += "\n";
                finalString += tmpString;
            }

            return finalString;
        }
    }
}