using System.Collections.Generic;
using GShark.Geometry;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    
    public class MeshGeometryTests
    {
        [Fact]
        private void CanCompute_CornerAngles() =>
            MeshTestData.FlatSquare.Corners.ForEach(
                corner =>
                {
                    var angle = MeshGeometry.Angle(corner);
                    Assert.True(Math.abs(angle - 0.5 * Math.PI) < MeshTestData.Tolerance);
                });


        [Fact]
        private void CanCompute_EdgeLengths() =>
            MeshTestData.FlatSquare.Edges.ForEach(
                edge =>
                {
                    var length = MeshGeometry.Length(edge);
                    Assert.True(Math.abs(length - 1) < MeshTestData.Tolerance);
                });


        [Fact]
        private void CanCompute_FaceArea()
        {
            var area = MeshGeometry.Area(MeshTestData.FlatTriangle.Faces[0]);
            Assert.True(Math.abs(area - 0.5) < MeshTestData.Tolerance);
        }


        [Fact]
        private void CanCompute_FaceNormal()
        {
            var normal = MeshGeometry.FaceNormal(MeshTestData.FlatTriangle.Faces[0]);
            Assert.Equal(Vector3.ZAxis, normal);
        }


        [Fact]
        private void CanCompute_MeshArea()
        {
            var area = MeshGeometry.Area(MeshTestData.FlatTriangle);
            Assert.True(Math.abs(area - 0.5) < MeshTestData.Tolerance);
        }


        [Fact]
        private void CanCompute_VertexNormal() =>
            MeshTestData.FlatTriangle.Vertices.ForEach(
                vertex =>
                {
                    var normal = MeshGeometry.VertexNormalAngleWeighted(vertex);
                    Assert.Equal(Vector3.ZAxis, normal);
                    normal = MeshGeometry.VertexNormalEquallyWeighted(vertex);
                    Assert.Equal(Vector3.ZAxis, normal);
                    normal = MeshGeometry.VertexNormalAreaWeighted(vertex);
                    Assert.Equal(Vector3.ZAxis, normal);
                });
    }
}