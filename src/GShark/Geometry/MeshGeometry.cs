using System;
using System.Linq;

namespace GShark.Geometry
{
    /// <summary>
    ///     Represents the geometry of a <see cref="Mesh"/> such as positions at vertices.
    /// </summary>
    public static class MeshGeometry
    {
        /// <summary>
        ///     Calculate the vector of a specified half-edge.
        /// </summary>
        /// <returns>The half-edge vector.</returns>
        /// <param name="halfEdge">Half edge.</param>
        public static Vector3 Vector(MeshHalfEdge halfEdge) =>
            halfEdge.Vertex - (Point3)halfEdge.Next.Vertex;


        /// <summary>
        ///     Calculates the length of the specified edge.
        /// </summary>
        /// <returns>The length.</returns>
        /// <param name="edge">Edge.</param>
        public static double Length(MeshEdge edge) => Vector(edge.HalfEdge).Length;


        /// <summary>
        ///     Calculates the midpoint of the specified edge.
        /// </summary>
        /// <returns>The point.</returns>
        /// <param name="edge">Edge.</param>
        public static Point3 MidPoint(MeshEdge edge)
        {
            var halfEdge = edge.HalfEdge;
            Point3 a = halfEdge.Vertex;
            Point3 b = halfEdge.Twin.Vertex;
            return (a + b) / 2;
        }


        /// <summary>
        ///     Calculates the mean edge length of the mesh.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <returns>The mean edge length of the mesh.</returns>
        public static double MeanEdgeLength(Mesh mesh)
        {
            return mesh.Edges.Sum(Length) / mesh.Edges.Count;
        }


        /// <summary>
        ///     Computes the area of the specified face.
        /// </summary>
        /// <returns>The face area.</returns>
        /// <param name="face">Face.</param>
        public static double Area(MeshFace face)
        {
            if (face.IsBoundaryLoop())
                return 0.0;
            
            var adjacentFaces = face.AdjacentVertices();
            var firstEdge = face.HalfEdge;
            switch (adjacentFaces.Count)
            {
                case 3: // Triangle face
                {
                    var u = Vector(firstEdge);
                    var v = -Vector(firstEdge.Prev);
                    return 0.5 * u.Cross(v).Length;
                }
                case 4: // Quad face
                {
                    var u1 = Vector(firstEdge);
                    var v1 = -Vector(firstEdge.Prev);
                    var a1 = 0.5 * u1.Cross(v1).Length;
                    var opposite = firstEdge.Next.Next;
                    var u2 = Vector(opposite);
                    var v2 = -Vector(opposite.Prev);
                    var a2 = 0.5 * u2.Cross(v2).Length;
                    return a1 + a2;
                }
                default: // NGon face
                    //TODO: Calculate the area of an arbitrary polygon.
                    throw new NotImplementedException("NGon face area calculation is not supported yet");
            }
        }


        /// <summary>
        ///     Computes the total area of the mesh.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        /// <returns>The mesh area.</returns>
        public static double Area(Mesh mesh) => mesh.Faces.Sum(Area);


        /// <summary>
        ///     Compute the normal vector of the specified face.
        /// </summary>
        /// <returns>The normal.</returns>
        /// <param name="face">Face.</param>
        public static Vector3 FaceNormal(MeshFace face)
        {
            // TODO: This must be checked as it will not apply in quad/ngon cases
            var u = Vector(face.HalfEdge);
            var v = -Vector(face.HalfEdge.Prev);
            return u.Cross(v).Unit();
        }


        /// <summary>
        ///     Compute the centroid of the specified face.
        /// </summary>
        /// <returns>The centroid.</returns>
        /// <param name="face">Face.</param>
        public static Point3 Centroid(MeshFace face)
        {
            var hE = face.HalfEdge;
            Point3 a = hE.Vertex;
            Point3 b = hE.Next.Vertex;
            Point3 c = hE.Prev.Vertex;

            if (face.IsBoundaryLoop())
                return (a + b) / 2;

            return (a + b + c) / 3;
        }


        /// <summary>
        ///     Compute the circumcenter the specified face.
        /// </summary>
        /// <returns>The circumcenter.</returns>
        /// <param name="face">Face.</param>
        public static Point3 Circumcenter(MeshFace face)
        {
            var hE = face.HalfEdge;

            Point3 a = hE.Vertex;
            Point3 b = hE.Next.Vertex;
            Point3 c = hE.Prev.Vertex;

            if (face.IsBoundaryLoop())
                return (a + b) / 2;

            var ac = c - a;
            var ab = b - a;
            var w = ab.Cross(ac);

            var u = w.Cross(ab) * ac.SquareLength;
            var v = ac.Cross(w) * ab.SquareLength;

            var x = ( Point3 ) (u + v) / (2 * w.SquareLength);

            return x + a;
        }


        /// <summary>
        ///     Compute the orthonormal bases of the specified face.
        /// </summary>
        /// <returns>Array containing the 2 Vector3d.</returns>
        /// <param name="face">Face.</param>
        public static Vector3[] OrthonormalBases(MeshFace face)
        {
            var e1 = Vector(face.HalfEdge).Unit();
            var normal = FaceNormal(face);
            var e2 = normal.Cross(e1);

            return new[] {e1, e2};
        }


        /// <summary>
        ///     Compute the angle (in radians) at the specified corner.
        /// </summary>
        /// <returns>The angle (in radians).</returns>
        /// <param name="corner">Corner.</param>
        public static double Angle(MeshCorner corner)
        {
            var u = Vector(corner.HalfEdge).Unit();
            var v = -Vector(corner.HalfEdge.Next).Unit();

            return Math.Acos(Math.Max(-1, Math.Min(1.0, u.Dot(v))));
        }


        /// <summary>
        ///     Computes the cotangent of the angle opposite to a half-edge.
        /// </summary>
        /// <returns>The cotangent value.</returns>
        /// <param name="hE">The half-edge</param>
        public static double Cotan(MeshHalfEdge hE)
        {
            if (hE.OnBoundary)
                return 0.0;

            var u = Vector(hE.Prev);
            var v = -Vector(hE.Next);

            return u.Dot(v) / u.Cross(v).Length;
        }


        /// <summary>
        ///     Computes the signed angle (in radians) between the faces adjacent to the specified half-edge.
        /// </summary>
        /// <returns>The angle (in radians) between faces.</returns>
        /// <param name="hE">H e.</param>
        public static double DihedralAngle(MeshHalfEdge hE)
        {
            if (hE.OnBoundary || hE.Twin.OnBoundary)
                return 0.0;

            var n1 = FaceNormal(hE.Face);
            var n2 = FaceNormal(hE.Twin.Face);
            var w = Vector(hE).Unit();

            var cosTheta = n1.Dot(n2);
            var sinTheta = n1.Cross(n2).Dot(w);

            return Math.Atan2(sinTheta, cosTheta);
        }


        /// <summary>
        ///     Computes the barycentric dual area around a given mesh vertex.
        /// </summary>
        /// <returns>The dual area.</returns>
        /// <param name="vertex">Vertex.</param>
        public static double BarycentricDualArea(MeshVertex vertex)
        {
            return vertex.AdjacentFaces().Sum(Area);
        }


        /// <summary>
        ///     Computes the circumcentric dual area around a given mesh vertex.
        /// </summary>
        /// <returns>The dual area.</returns>
        /// <param name="vertex">Vertex.</param>
        public static double CircumcentricDualArea(MeshVertex vertex)
        {
            return (from hE in vertex.AdjacentHalfEdges()
                    let u2 = Vector(hE.Prev).SquareLength
                    let v2 = Vector(hE).SquareLength
                    let cotAlpha = Cotan(hE.Prev)
                    let cotBeta = Cotan(hE)
                    select (u2 * cotAlpha + v2 * cotBeta) / 8).Sum();
        }


        /// <summary>
        ///     Computes the equally weighted normal around the specified vertex.
        /// </summary>
        /// <returns>The normal vector at that vertex.</returns>
        /// <param name="vertex">Vertex.</param>
        public static Vector3 VertexNormalEquallyWeighted(MeshVertex vertex)
        {
            var n = new Vector3();
            foreach (var f in vertex.AdjacentFaces())
                n += FaceNormal(f);

            return n.Unit();
        }


        /// <summary>
        ///     Computes the area weighted normal around the specified vertex.
        /// </summary>
        /// <returns>The normal vector at that vertex.</returns>
        /// <param name="vertex">Vertex.</param>
        public static Vector3 VertexNormalAreaWeighted(MeshVertex vertex)
        {
            var n = new Vector3();
            foreach (var f in vertex.AdjacentFaces())
            {
                var normal = FaceNormal(f);
                var area = Area(f);

                n += normal * area;
            }

            return n.Unit();
        }


        /// <summary>
        ///     Computes the angle weighted normal around the specified vertex.
        /// </summary>
        /// <returns>The normal vector at that vertex.</returns>
        /// <param name="vertex">Vertex.</param>
        public static Vector3 VertexNormalAngleWeighted(MeshVertex vertex)
        {
            var n = new Vector3();
            foreach (var c in vertex.AdjacentCorners())
            {
                var normal = FaceNormal(c.HalfEdge.Face);
                var angle = Angle(c);

                n += normal * angle;
            }

            return n.Unit();
        }


        /// <summary>
        ///     Computes the gauss curvature weighted normal around the specified vertex.
        /// </summary>
        /// <returns>The normal vector at that vertex.</returns>
        /// <param name="vertex">Vertex.</param>
        public static Vector3 VertexNormalGaussCurvature(MeshVertex vertex)
        {
            var n = new Vector3();
            foreach (var hE in vertex.AdjacentHalfEdges())
            {
                var weight = 0.5 * DihedralAngle(hE) / Length(hE.Edge);
                n -= Vector(hE) * weight;
            }

            return n.Unit();
        }


        /// <summary>
        ///     Computes the mean curvature weighted normal around the specified vertex.
        /// </summary>
        /// <returns>The normal vector at that vertex.</returns>
        /// <param name="vertex">Vertex.</param>
        public static Vector3 VertexNormalMeanCurvature(MeshVertex vertex)
        {
            var n = new Vector3();
            foreach (var hE in vertex.AdjacentHalfEdges())
            {
                var weight = 0.5 * Cotan(hE) + Cotan(hE.Twin);
                n -= Vector(hE) * weight;
            }

            return n.Unit();
        }


        /// <summary>
        ///     Computes the sphere inscribed normal around the specified vertex.
        /// </summary>
        /// <returns>The normal vector at that vertex.</returns>
        /// <param name="vertex">Vertex.</param>
        public static Vector3 VertexNormalSphereInscribed(MeshVertex vertex)
        {
            var n = new Vector3();
            foreach (var c in vertex.AdjacentCorners())
            {
                var u = Vector(c.HalfEdge.Prev);
                var v = -Vector(c.HalfEdge.Next);

                n += u.Cross(v) / (u.SquareLength * v.SquareLength);
            }

            return n.Unit();
        }


        /// <summary>
        ///     Computes the angle defect at the given vertex.
        /// </summary>
        /// <param name="vertex">Vertex to compute angle defect.</param>
        /// <returns>Number representing the deviation of the current vertex from $2\PI$.</returns>
        public static double AngleDefect(MeshVertex vertex)
        {
            var angleSum = 0.0;
            foreach (var c in vertex.AdjacentCorners())
                angleSum += Angle(c);

            // if (vertex.OnBoundary()) angleSum = Math.PI - angleSum;
            return vertex.OnBoundary() ? Math.PI - angleSum : 2 * Math.PI - angleSum;
        }


        /// <summary>
        ///     Compute the Gaussian curvature at the given vertex.
        /// </summary>
        /// <param name="vertex">Vertex to compute Gaussian curvature.</param>
        /// <returns>Number representing the gaussian curvature at that vertex.</returns>
        public static double ScalarGaussCurvature(MeshVertex vertex) =>
            AngleDefect(vertex) / CircumcentricDualArea(vertex);


        /// <summary>
        ///     Compute the Mean curvature at the given vertex.
        /// </summary>
        /// <param name="vertex">Vertex to compute Mean curvature.</param>
        /// <returns>Number representing the Mean curvature at that vertex.</returns>
        public static double ScalarMeanCurvature(MeshVertex vertex)
        {
            var sum = 0.0;
            foreach (var hE in vertex.AdjacentHalfEdges())
                sum += 0.5 * Length(hE.Edge) * DihedralAngle(hE);
            return sum;
        }


        /// <summary>
        ///     Compute the total angle defect of the mesh.
        /// </summary>
        /// <param name="mesh">Mesh to compute angle defect.</param>
        /// <returns>Returns the total angle defect as a scalar value.</returns>
        public static double TotalAngleDefect(Mesh mesh)
        {
            var totalDefect = 0.0;
            foreach (var v in mesh.Vertices)
                totalDefect += AngleDefect(v);
            return totalDefect;
        }


        /// <summary>
        ///     Compute the principal curvature scalar values at a given vertex.
        /// </summary>
        /// <param name="vertex">Vertex to compute the curvature.</param>
        /// <returns>Returns an array of 2 values {k1, k2}.</returns>
        public static double[] PrincipalCurvatures(MeshVertex vertex)
        {
            var a = CircumcentricDualArea(vertex);
            var h = ScalarMeanCurvature(vertex) / a;
            var k = AngleDefect(vertex) / a;

            var discriminant = h * h - k;
            if (discriminant > 0)
                discriminant = Math.Sqrt(discriminant);
            else
                discriminant = 0;

            var k1 = h - discriminant;
            var k2 = h + discriminant;

            return new[] {k1, k2};
        }
    }
}