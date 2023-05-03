using System.Collections.Generic;
using GShark.Geometry;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    public class MeshFaceTests
    {
        [Fact]
        public void CanCompute_FaceArea() =>
            MeshTestData.FlatSquare.Faces.ForEach(face => { Assert.Equal(1, face.Area); });


        [Fact]
        public void CanCompute_FaceNormal() =>
            MeshTestData.FlatSquare.Faces.ForEach(face => { Assert.Equal(Vector3.ZAxis, face.Normal); });


        [Fact]
        public void CanCompute_FaceTopology() =>
            MeshTestData.FlatTriangle.Faces.ForEach(
                face =>
                {
                    Assert.Empty(face.AdjacentFaces());
                    Assert.Equal(3, face.AdjacentEdges().Count);
                    Assert.Equal(3, face.AdjacentCorners().Count);
                    Assert.Equal(3, face.AdjacentHalfEdges().Count);
                    Assert.Equal(3, face.AdjacentVertices().Count);
                });


        [Fact]
        public void CanConvert_ToString() =>
            MeshTestData.FlatSquare.Faces.ForEach(face => { Assert.NotNull(face.ToString()); });


        [Fact]
        public void CanGet_Index() =>
            MeshTestData.FlatSquare.Faces.ForEach(face => { Assert.True(face.Index >= 0); });
    }
}