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


        

        //Function that checks if point is in or out a triangle (boolean result, In - True , Out - False)
        private bool IsPointInTriangle(Point3 projection, List<Point3> trianglePoints)
        {
            //edges vectors of the triangle
            Vector3 v0 = new Vector3(trianglePoints[1] - trianglePoints[0]);
            Vector3 v1 = new Vector3(trianglePoints[2] - trianglePoints[1]);
            Vector3 v2 = new Vector3(trianglePoints[0] - trianglePoints[2]);

            List<Vector3> edges_vectors = new List<Vector3>() { v0, v1, v2 };

            //normal to the triangle
            Vector3 n = Vector3.CrossProduct(v0, v1);

            //Creates a list to store the dot products of the vectors
            //Negative dot product means that the point is ouside of the triangle edges
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

        //Function that finds the closest point to a triangle (given the 3 vertices as points)
        private Point3 ClosestPointToTriangle(List<Point3> trianglePoints, Point3 point)
        {
            //Creates Plane from 3 points
            Plane FacePlane = new Plane(trianglePoints[0], trianglePoints[1], trianglePoints[2]);

            //Find projection of point of the plane
            Vector3 pointV = new Vector3(point);
            double cpdistance;
            Point3 PlaneClosestPoint = FacePlane.ClosestPoint(point, out cpdistance);

            //We create a initial base point "ClosestPoint" that will be overwritten
            Point3 ClosestPoint = new Point3(0, 0, 0);

            //Checks if the projection of the point on the FacePlane is inside the triangle
            bool InOrOut = IsPointInTriangle(PlaneClosestPoint, trianglePoints);
            if (InOrOut == true)
            {
                //Checks if the projection of the point on the FacePlane is inside the triangle
                ClosestPoint = PlaneClosestPoint;
            }

            else //If not, we need to perform a few more calculations.
                 //We create a line(a Rhino line, a mathematical segment)
                 //for each of the edges and then find the closest point on each of the edges,
                 //and then take the closest one out of these 3
            {
                //Create a list to store the closest point to each of the edges
                List<Point3> EdgesClosestPoints = new List<Point3>();
                Line segment_AB = new Line(trianglePoints[0], trianglePoints[1]);
                Line segment_BC = new Line(trianglePoints[1], trianglePoints[2]);
                Line segment_CA = new Line(trianglePoints[2], trianglePoints[0]);
                List<Line> Segments_list = new List<Line>() { segment_AB, segment_BC, segment_CA };

                //Create a segment for each vertex
                foreach (Line segment in Segments_list)
                {
                    Point3 SegmentClosestPoint = segment.ClosestPoint(point);
                    EdgesClosestPoints.Add(SegmentClosestPoint);
                }
                Point3 edgeClosestPoint = point.CloudClosestPoint(EdgesClosestPoints);
                ClosestPoint = edgeClosestPoint;
            }

            return ClosestPoint;
        }

        public Point3 ClosestPoint(Point3 point)
        {
            //Gets vertices and converts them from type "MeshVertex" to "Point3"
            List<MeshVertex> meshVertices = Vertices;
            List<Point3> verticesPoints = meshVertices.ConvertAll(v => (Point3)v);

            //Finds closer vertex to the target point
            Point3 CloserPointVertex = point.CloudClosestPoint(verticesPoints);
            //Finds the equivalent point as a type "MeshVertex" using the index of the Point3 in its equivalent list
            MeshVertex CloserVertex = meshVertices[verticesPoints.IndexOf(CloserPointVertex)];

            //Finds adjiacent faces to the closer vertex
            System.Collections.Generic.IEnumerable<MeshFace> AdjacentFaces = CloserVertex.AdjacentFaces();

            List<Point3> CloserPointToFace_List = new List<Point3>();

            foreach (MeshFace face in AdjacentFaces)
            {
                //Retrives the Vertices associated with each face
                List<MeshVertex> GSVertices = face.AdjacentVertices();
                //Converts from MeshVertex to Point3
                List<Point3> FacePoints = GSVertices.ConvertAll(v => (Point3)v);

                if (GSVertices.Count == 3)
                {
                    var trianglePoints = new List<Point3>() { FacePoints[0], FacePoints[1], FacePoints[2] };
                    Point3 ClosestPoint0 = ClosestPointToTriangle(trianglePoints, point);
                    CloserPointToFace_List.Add(ClosestPoint0);
                }
                else
                {
                    var trianglePoints1 = new List<Point3>() { FacePoints[0], FacePoints[1], FacePoints[2] };
                    var trianglePoints2 = new List<Point3>() { FacePoints[1], FacePoints[2], FacePoints[3] };
                    var trianglePoints3 = new List<Point3>() { FacePoints[0], FacePoints[1], FacePoints[3] };
                    var trianglePoints4 = new List<Point3>() { FacePoints[0], FacePoints[2], FacePoints[3] };

                    Point3 ClosestPoint1 = ClosestPointToTriangle(trianglePoints1, point);
                    Point3 ClosestPoint2 = ClosestPointToTriangle(trianglePoints2, point);
                    Point3 ClosestPoint3 = ClosestPointToTriangle(trianglePoints3, point);
                    Point3 ClosestPoint4 = ClosestPointToTriangle(trianglePoints4, point);

                    List<Point3> TrianglesCP = new List<Point3>() { ClosestPoint1, ClosestPoint2, ClosestPoint3, ClosestPoint4 };
                    Point3 ClosestPoint = point.CloudClosestPoint(TrianglesCP);

                    CloserPointToFace_List.Add(ClosestPoint);
                }
            }
            Point3 MeshClosestPoint = point.CloudClosestPoint(CloserPointToFace_List);
            return MeshClosestPoint;
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