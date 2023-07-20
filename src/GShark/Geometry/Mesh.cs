using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    ///     Represents a Half-Edge Mesh data structure.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Mesh" /> class.
        /// </summary>
        private Mesh()
        {
            Vertices = new List<MeshVertex>();
            Edges = new List<MeshEdge>();
            Faces = new List<MeshFace>();
            Corners = new List<MeshCorner>();
            HalfEdges = new List<MeshHalfEdge>();
            Boundaries = new List<MeshFace>();
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Mesh" /> class from verticees and faces.
        /// </summary>
        /// <param name="vertices">list of mesh vertices.</param>
        /// <param name="faceIndexes">Nested list with face vertices index.</param>
        public Mesh(List<Point3> vertices, List<List<int>> faceIndexes)
            : this()
        {
            // There are 3 steps for this process
            // - Iterate through vertices, create vertex objects
            CreateVertices(vertices);

            // - Iterate through faces, creating face, edge, and halfedge objects (and connecting where possible)
            var result = CreateFaces(faceIndexes);
            if (!result)
                throw new Exception("Couldn't create faces for this mesh");
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Mesh" /> class from verticees and faces.
        /// </summary>
        /// <param name="vertices">list of mesh vertices.</param>
        /// <param name="faceIndexes">Nested list with face vertices index.</param>
        public Mesh(List<MeshVertex> vertices, List<List<int>> faceIndexes)
            : this()
        {
            Vertices = vertices;

            // - Iterate through faces, creating face, edge, and halfedge objects (and connecting where possible)
            CreateFaces(faceIndexes);
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Mesh" /> class from an existing one.
        /// </summary>
        /// <param name="halfEdgeMesh">Existing Half-Edge Mesh.</param>
        public Mesh(Mesh halfEdgeMesh)
        {
            Vertices = new List<MeshVertex>(halfEdgeMesh.Vertices);
            Edges = new List<MeshEdge>(halfEdgeMesh.Edges);
            Faces = new List<MeshFace>(halfEdgeMesh.Faces);
            Corners = new List<MeshCorner>(halfEdgeMesh.Corners);
            HalfEdges = new List<MeshHalfEdge>(halfEdgeMesh.HalfEdges);
            Boundaries = new List<MeshFace>(halfEdgeMesh.Boundaries);
        }


        /// <summary>
        ///     Gets or sets the vertices of the mesh.
        /// </summary>
        public List<MeshVertex> Vertices { get; set; }

        /// <summary>
        ///     Gets or sets the edges of the mesh.
        /// </summary>
        public List<MeshEdge> Edges { get; set; }

        /// <summary>
        ///     Gets or sets the faces of the mesh.
        /// </summary>
        public List<MeshFace> Faces { get; set; }

        /// <summary>
        ///     Gets or sets the corners of the mesh.
        /// </summary>
        public List<MeshCorner> Corners { get; set; }

        /// <summary>
        ///     Gets or sets the half-edges of the mesh.
        /// </summary>
        public List<MeshHalfEdge> HalfEdges { get; set; }

        /// <summary>
        ///     Gets or sets the boundaries of the mesh.
        /// </summary>
        public List<MeshFace> Boundaries { get; set; }

        /// <summary>
        ///     Gets the euler characteristic of the mesh.
        /// </summary>
        public int EulerCharacteristic => Vertices.Count - Edges.Count + Faces.Count;

        /// <summary>
        ///     Gets the area of the mesh.
        /// </summary>
        /// <returns>The total surface area of this mesh</returns>
        public double GetArea() => Faces.Sum(f => f.Area);

        /// <summary>
        ///     Check if the mesh has isolated vertices.
        /// </summary>
        /// <returns>True if there are isolated vertices, false if not.</returns>
        public bool HasIsolatedVertices()
        {
            foreach (var v in Vertices)
            {
                if (v.IsIsolated())
                    return true;
            }

            return false;
        }


        /// <summary>
        ///     Check if the mesh contains isolated faces.
        /// </summary>
        /// <returns>True if there are isolated faces, false if not.</returns>
        public bool HasIsolatedFaces()
        {
            foreach (var f in Faces)
            {
                var boundaryEdges = 0;
                var adjacent = f.AdjacentHalfEdges();
                foreach (var e in adjacent)
                {
                    if (e.OnBoundary)
                        boundaryEdges++;
                }

                if (boundaryEdges == adjacent.Count)
                    return true;
            }

            return false;
        }


        /// <summary>
        ///     Check if the mesh contains non-manifold edges.
        /// </summary>
        /// <returns>True if there are non-manifold edges, false if not.</returns>
        public bool HasNonManifoldEdges()
        {
            foreach (var edge in Edges)
            {
                if (edge.AdjacentFaces().Count > 2)
                    return true;
            }

            return false;
        }


        /// <summary>
        ///     Assign an index number to each mesh member.
        /// </summary>
        public void IndexElements()
        {
            var index = -1;
            foreach (var v in Vertices)
            {
                index++;
                v.Index = index;
            }

            index = -1;
            foreach (var f in Faces)
            {
                index++;
                f.Index = index;
            }

            index = -1;
            foreach (var hE in HalfEdges)
            {
                index++;
                hE.Index = index;
            }

            index = -1;
            foreach (var e in Edges)
            {
                index++;
                e.Index = index;
            }

            index = -1;
            foreach (var c in Corners)
            {
                index++;
                c.Index = index;
            }

            index = -1;
            foreach (var b in Boundaries)
            {
                index++;
                b.Index = index;
            }
        }


        /// <summary>
        ///     Assign an index to each vertex of the mesh.
        /// </summary>
        /// <returns>Dictionary containing Vertex-Index assignments.</returns>
        public Dictionary<MeshVertex, int> IndexVertices()
        {
            var i = -1;
            var index = new Dictionary<MeshVertex, int>();
            foreach (var v in Vertices)
                index[v] = i++;
            return index;
        }


        /// <summary>
        ///     Assign an index to each face of the mesh.
        /// </summary>
        /// <returns>Dictionary containing Face-Index assignments.</returns>
        public Dictionary<MeshFace, int> IndexFaces()
        {
            var i = -1;
            var index = new Dictionary<MeshFace, int>();
            foreach (var v in Faces)
                index[v] = i++;
            return index;
        }


        /// <summary>
        ///     Assign an index to each edge of the mesh.
        /// </summary>
        /// <returns>Dictionary containing Edge-Index assignments.</returns>
        public Dictionary<MeshEdge, int> IndexEdges()
        {
            var i = -1;
            var index = new Dictionary<MeshEdge, int>();
            foreach (var v in Edges)
                index[v] = i++;
            return index;
        }


        /// <summary>
        ///     Assign an index to each Half-Edge of the mesh.
        /// </summary>
        /// <returns>Dictionary containing HalfEdge-Index assignments.</returns>
        public Dictionary<MeshHalfEdge, int> IndexHalfEdges()
        {
            var i = -1;
            var index = new Dictionary<MeshHalfEdge, int>();
            foreach (var f in HalfEdges)
                index[f] = i++;
            return index;
        }


        /// <summary>
        ///     Assign an index to each corner of the mesh.
        /// </summary>
        /// <returns>Dictionary containing Corner-Index assignments.</returns>
        public Dictionary<MeshCorner, int> IndexCorners()
        {
            var i = -1;
            var index = new Dictionary<MeshCorner, int>();
            foreach (var f in Corners)
                index[f] = i++;
            return index;
        }


        /// <summary>
        ///     Check if a mesh is triangular.
        /// </summary>
        /// <returns>Returns true if all faces are triangular.</returns>
        public bool IsTriangularMesh() => IsMesh() == IsMeshResult.Triangular;


        /// <summary>
        ///     Check if a mesh is quad.
        /// </summary>
        /// <returns>Returns true if all faces are quads.</returns>
        public bool IsQuadMesh() => IsMesh() == IsMeshResult.Quad;


        /// <summary>
        ///     Check if a mesh is n-gonal.
        /// </summary>
        /// <returns>Returns true if the mesh contains ANY ngons.</returns>
        public bool IsNgonMesh() => IsMesh() == IsMeshResult.Ngon;


        /// <summary>
        ///     Returns an enum corresponding to the mesh face topology  (triangular, quad or ngon).
        /// </summary>
        private IsMeshResult IsMesh()
        {
            var count = CountFaceEdges();
            if (count.Triangles == Faces.Count)
                return IsMeshResult.Triangular;
            if (count.Quads == Faces.Count)
                return IsMeshResult.Quad;
            if (count.Ngons != 0)
                return IsMeshResult.Ngon;
            return IsMeshResult.Error;
        }


        /// <summary>
        ///     Get human readable description of this mesh.
        /// </summary>
        /// <returns>Mesh description as text.</returns>
        public string GetMeshInfo()
        {
            const string head = "--- Mesh Info ---\n";

            var vef = "V: " + Vertices.Count + "; F: " + Faces.Count + "; E:"
                    + Edges.Count + "\n";
            var hec = "Half-edges: " + HalfEdges.Count + "; Corners: " + Corners.Count
                    + "\n";
            var bounds = "Boundaries: " + Boundaries.Count + "\n";
            var euler = "Euler characteristic: " + EulerCharacteristic + "\n";
            var isoVert = "Isolated vertices: " + HasIsolatedVertices() + "\n";
            var isoFace = "Isolated faces: " + HasIsolatedFaces() + "\n";
            var manifold = "Has Non-Manifold Edges: " + HasNonManifoldEdges() + "\n";

            var faceData = CountFaceEdges();
            var triangles = "Tri faces: " + faceData.Triangles + "\n";
            var quads = "Quad faces: " + faceData.Quads + "\n";
            var ngons = "Ngon faces: " + faceData.Ngons + "\n";

            const string tail = "-----       -----\n\n";

            return head + vef + hec + bounds + euler + isoVert + isoFace + manifold + triangles
                 + quads + ngons + tail;
        }


        /// <summary>
        ///     Gets string representation of the mesh.
        /// </summary>
        /// <returns>Mesh string.</returns>
        public override string ToString()
        {
            var vefh = "V: " + Vertices.Count + "; F: " + Faces.Count + "; E:"
                     + Edges.Count
                     + "; hE: " + HalfEdges.Count;
            return "HE_Mesh{" + vefh + "}";
        }


        private void CreateVertices(List<Point3> points)
        {
            var verts = new List<MeshVertex>(points.Count);

            foreach (var pt in points)
            {
                var vertex = new MeshVertex(pt.X, pt.Y, pt.Z);
                verts.Add(vertex);
            }

            Vertices = verts;
        }


        // Takes a List containing another List per face with the vertex indexes belonging to that face
        private bool CreateFaces(IEnumerable<List<int>> faceIndexes)
        {
            var edgeCount = new Dictionary<string, int>();
            var existingHalfEdges = new Dictionary<string, MeshHalfEdge>();
            var hasTwinHalfEdge = new Dictionary<MeshHalfEdge, bool>();

            // Create the faces, edges and half-edges, non-boundary loops and link references when possible;
            foreach (var indexes in faceIndexes)
            {
                var f = new MeshFace();
                Faces.Add(f);

                var tempHEdges = new List<MeshHalfEdge>(indexes.Count);

                // Create empty half-edges
                for (var i = 0; i < indexes.Count; i++)
                {
                    var h = new MeshHalfEdge();
                    tempHEdges.Add(h);
                }

                // Fill out each half edge
                for (var i = 0; i < indexes.Count; i++)
                {
                    // Edge goes from v0 to v1
                    var v0 = indexes[i];
                    var v1 = indexes[(i + 1) % indexes.Count];

                    var h = tempHEdges[i];

                    // Set previous and next
                    h.Next = tempHEdges[(i + 1) % indexes.Count];
                    h.Prev = tempHEdges[(i + indexes.Count - 1) % indexes.Count];

                    h.OnBoundary = false;
                    hasTwinHalfEdge.Add(h, false);

                    // Set half-edge & vertex mutually
                    h.Vertex = Vertices[v0];
                    Vertices[v0].HalfEdge = h;

                    // Set half-edge face & vice versa
                    h.Face = f;
                    f.HalfEdge = h;

                    // Reverse v0 and v1 if v0 > v1
                    if (v0 > v1)
                        (v0, v1) = (v1, v0);
                    
                    var key = v0 + " " + v1;
                    if (existingHalfEdges.ContainsKey(key))
                    {
                        // If this half-edge key already exists, it is the twin of this current half-edge
                        var twin = existingHalfEdges[key];
                        h.Twin = twin;
                        twin.Twin = h;
                        h.Edge = twin.Edge;
                        hasTwinHalfEdge[h] = true;
                        hasTwinHalfEdge[twin] = true;
                        edgeCount[key]++;
                    }
                    else
                    {
                        // Create an edge and set its half-edge
                        var e = new MeshEdge();
                        Edges.Add(e);
                        h.Edge = e;
                        e.HalfEdge = h;

                        // Record the newly created half-edge
                        existingHalfEdges.Add(key, h);
                        edgeCount.Add(key, 1);
                    }
                }

                HalfEdges.AddRange(tempHEdges);
            }

            // Create boundary edges
            for (var i = 0; i < HalfEdges.Count; i++)
            {
                var h = HalfEdges[i];
                if (!hasTwinHalfEdge[h])
                {
                    var f = new MeshFace();
                    Boundaries.Add(f);

                    var boundaryCycle = new List<MeshHalfEdge>();
                    var halfEdge = h;
                    do
                    {
                        var boundaryHalfEdge = new MeshHalfEdge();
                        HalfEdges.Add(boundaryHalfEdge);
                        boundaryCycle.Add(boundaryHalfEdge);

                        var nextHalfEdge = halfEdge.Next;
                        while (hasTwinHalfEdge[nextHalfEdge])
                            nextHalfEdge = nextHalfEdge.Twin.Next;

                        boundaryHalfEdge.Vertex = nextHalfEdge.Vertex;
                        boundaryHalfEdge.Edge = halfEdge.Edge;
                        boundaryHalfEdge.OnBoundary = true;

                        boundaryHalfEdge.Face = f;
                        f.HalfEdge = boundaryHalfEdge;

                        boundaryHalfEdge.Twin = halfEdge;
                        halfEdge.Twin = boundaryHalfEdge;

                        halfEdge = nextHalfEdge;
                    } while (halfEdge != h);

                    var n = boundaryCycle.Count;
                    for (var j = 0; j < n; j++)
                    {
                        boundaryCycle[j].Next = boundaryCycle[(j + n - 1) % n];
                        boundaryCycle[j].Prev = boundaryCycle[(j + 1) % n];
                        hasTwinHalfEdge[boundaryCycle[j]] = true;
                        hasTwinHalfEdge[boundaryCycle[j].Twin] = true;
                    }
                }

                if (h.OnBoundary)
                    continue;

                var corner = new MeshCorner {HalfEdge = h};
                h.Corner = corner;
                Corners.Add(corner);
            }

            // Check mesh for common errors
            if (HasIsolatedFaces() || HasIsolatedVertices() || HasNonManifoldEdges())
                return false;

            // Index elements
            IndexElements();

            return true;
        }


        private FaceData CountFaceEdges()
        {
            FaceData data = default;

            foreach (var face in Faces)
            {
                switch (face.AdjacentCorners().Count)
                {
                    case 3:
                        data.Triangles++;
                        break;
                    case 4:
                        data.Quads++;
                        break;
                    default:
                        data.Ngons++;
                        break;
                }
            }

            return data;
        }

        /// <summary>
        /// Checks if point is in or out a triangle.
        /// </summary>
        /// <param name="projection">Point to check.</param>
        /// <param name="trianglePoints">Triangle vertices.</param>
        /// <returns>Boolean result (In - True , Out - False).</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when trianglePoints is not 3 points.</exception>
        private bool IsPointInTriangle(Point3 projection, Point3[] trianglePoints)
        {
            if (trianglePoints.Length != 3)
                throw new ArgumentOutOfRangeException($"{nameof(trianglePoints)} must be 3.");

            //edges vectors of the triangle
            Vector3 v0 = new Vector3(trianglePoints[1] - trianglePoints[0]);
            Vector3 v1 = new Vector3(trianglePoints[2] - trianglePoints[1]);
            Vector3 v2 = new Vector3(trianglePoints[0] - trianglePoints[2]);

            var edges_vectors = new Vector3[] { v0, v1, v2 };

            // normal to the triangle
            Vector3 n = Vector3.CrossProduct(v0, v1);

            // Creates a list to store the dot products of the vectors
            // Negative dot product means that the point is ouside of the triangle edges
            List<double> dotProducts = new List<double>();

            for (int i = 0; i < 3; i++)
            {
                Vector3 w = new Vector3(projection - trianglePoints[i]);
                Vector3 t = Vector3.CrossProduct(edges_vectors[i], w);
                double dotProduct = n * t;
                dotProducts.Add(dotProduct);
            }

            return ((dotProducts[0] > 0) && (dotProducts[1] > 0) && (dotProducts[2] > 0));
            }


        /// <summary>
        /// Finds the closest point to a triangle.
        /// </summary>
        /// <param name="trianglePoints">Triangle vertices.</param>
        /// <param name="point">Test point.</param>
        /// <returns>Closest point to the triangle.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when trianglePoints is not 3 points.</exception>
        private Point3 ClosestPointToTriangle(Point3[] trianglePoints, Point3 point)
        {
            if (trianglePoints.Length != 3)
                throw new ArgumentOutOfRangeException($"{nameof(trianglePoints)} must be 3.");
            // Creates Plane from 3 points
            // TODO: checks if the triangle degenerate into a line or a point before this
            Plane facePlane = new Plane(trianglePoints[0], trianglePoints[1], trianglePoints[2]);
            // Find projection of point of the plane
            Point3 closestPoint = facePlane.ClosestPoint(point, out _);
            // if the projection of the point is inside the triangle, we return the closest point
            if (IsPointInTriangle(closestPoint, trianglePoints))
                return closestPoint;
             
            // If not, we create a line for each triangle edges
            // and then find the closest point on each edge
            Line segmentAB = new Line(trianglePoints[0], trianglePoints[1]);
            Line segmentBC = new Line(trianglePoints[1], trianglePoints[2]);
            Line segmentCA = new Line(trianglePoints[2], trianglePoints[0]);
            List<Line> segments = new List<Line>() { segmentAB, segmentBC, segmentCA };
            List<Point3> edgesClosestPoints = new List<Point3>();
            foreach (Line segment in segments)
            {
                Point3 segmentClosestPoint = segment.ClosestPoint(point);
                edgesClosestPoints.Add(segmentClosestPoint);
            }
            return Point3.CloudClosestPoint(edgesClosestPoints, point);
        }

        /// <summary>
        /// Finds the closest point to a mesh.
        /// </summary>
        /// <param name="point">Test point.</param>
        /// <returns>Mesh Closest Point.</returns>
        /// <remark>Method might not work with concave Ngon faces.</remark>
        public Point3 ClosestPoint(Point3 point)
        {
            List<MeshVertex> meshVertices = Vertices;
            // All faces check 
            IEnumerable<MeshFace> allFaces = this.Faces;
            var closerPointToFaceList = new List<Point3>();

            foreach (MeshFace face in allFaces)
            {
                // Retrives the vertices associated with each face
                List<MeshVertex> faceVertices = face.AdjacentVertices();
                
                if (faceVertices.Count == 3)
                {
                    var trianglePoints = new Point3[] { faceVertices[0], faceVertices[1], faceVertices[2] };
                    Point3 closestPoint0 = ClosestPointToTriangle(trianglePoints, point);
                    closerPointToFaceList.Add(closestPoint0);
                }
                else 
                {
                    // Ngon triangulation using the vertices' centroid and consecutive vertices to create the triangles 
                    Point3 ngonCentre = Point3.AveragePoint(faceVertices);
                    var TrianglesCP = new List<Point3>();

                    for (int i = 0; i < faceVertices .Count; i++)
                    {
                        Point3[] trianglePoints = new Point3[] { ngonCentre, faceVertices[i], faceVertices[(i + 1)% faceVertices.Count] };
                        Point3 ClosestPointToNgon = ClosestPointToTriangle(trianglePoints, point);
                        TrianglesCP.Add(ClosestPointToNgon);
                    }
                    Point3 ClosestPoint = Point3.CloudClosestPoint(TrianglesCP, point);
                    closerPointToFaceList.Add(ClosestPoint);
                }
            }
            Point3 meshClosestPoint = Point3.CloudClosestPoint(closerPointToFaceList, point);
            return new Point3(meshClosestPoint);
        }

        /// <summary>
        ///     Type of mesh (Triangular, Quad, Ngon or Error).
        /// </summary>
        private enum IsMeshResult
        {
            Triangular, Quad, Ngon, Error
        }

        private struct FaceData
        {
            public int Triangles;
            public int Quads;
            public int Ngons;
        }
    }
}