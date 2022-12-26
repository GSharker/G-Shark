using System.Collections.Generic;
using System.Linq;
using GShark.Geometry;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    public class MeshVertexTests
    {
        [Fact]
        public void CanCompute_Adjacencies() =>
            MeshTestData.FlatSquare.Vertices.ForEach(
                vertex =>
                {
                    Assert.Single(vertex.AdjacentFaces());
                    Assert.Single(vertex.AdjacentCorners());
                    Assert.Equal(2, vertex.AdjacentVertices().Count());
                    Assert.Equal(2, vertex.AdjacentEdges().Count());
                    Assert.Equal(2, vertex.AdjacentHalfEdges().Count());
                    Assert.Equal(2, vertex.Valence());
                    Assert.True(vertex.OnBoundary());
                });


        [Fact]
        public void CanConvert_ToString() =>
            MeshTestData.FlatSquare.Vertices.ForEach(vertex => { Assert.NotNull(vertex.ToString()); });


        [Fact]
        public void CanCreate()
        {
            Assert.NotNull(new MeshVertex());
            Assert.NotNull(new MeshVertex(Point3.Origin));
        }
    }
}