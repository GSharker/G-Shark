using System.Collections.Generic;
using GShark.Geometry;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    public class MeshTests
    {
        [Theory]
        [MemberData(nameof(MeshTestData.Meshes_Type), MemberType=typeof(MeshTestData))]
        public void CanCheck_QuadMesh(Mesh mesh, bool isTri, bool isQuad, bool isNgon)
        {
            Assert.Equal(isTri, mesh.IsTriangularMesh());
            Assert.Equal(isQuad, mesh.IsQuadMesh());
            Assert.Equal(isNgon, mesh.IsNgonMesh());
        }


        [Theory]
        [MemberData(nameof(MeshTestData.Meshes_Boundary), MemberType=typeof(MeshTestData))]
        public void CanCompute_Boundary(Mesh mesh, int expected)
        {
            Assert.Equal(expected,mesh.Boundaries.Count);
        }

        
        [Theory]
        [MemberData(nameof(MeshTestData.Meshes_Area), MemberType=typeof(MeshTestData))]
        public void CanCompute_Area(Mesh mesh, double area)
        {
            Assert.Equal(area, mesh.GetArea());
        }
        
        [Theory]
        [MemberData(nameof(MeshTestData.Meshes_Euler), MemberType=typeof(MeshTestData))]
        public void CanCompute_EulerCharacteristic(Mesh mesh, int expected)
        {
            Assert.Equal(expected, mesh.EulerCharacteristic);
        }


        [Theory]
        [MemberData(nameof(MeshTestData.Meshes), MemberType=typeof(MeshTestData))]
        public void CanConvert_ToString(Mesh mesh)
        {
            var s = mesh.ToString();
            var info = mesh.GetMeshInfo();
            Assert.False(string.IsNullOrEmpty(s));
            Assert.False(string.IsNullOrEmpty(info));
        }


        [Theory]
        [MemberData(nameof(MeshTestData.Meshes), MemberType=typeof(MeshTestData))]
        public void CanCreate_ValidMesh(Mesh mesh)
        {
            Assert.NotNull(mesh);
            Assert.NotEmpty(mesh.Vertices);
            Assert.NotEmpty(mesh.Faces);
        }

        [Theory]
        [MemberData(nameof(MeshTestData.Meshes), MemberType=typeof(MeshTestData))]
        public void CanDetect_IsolatedVertices(Mesh mesh)
        {
            Assert.False(mesh.HasIsolatedVertices());

            mesh.Vertices.Add(new MeshVertex(10000, 0, 0));
            Assert.True(mesh.HasIsolatedVertices());
        }
    }
}