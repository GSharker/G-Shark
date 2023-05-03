using System.Collections.Generic;
using GShark.Geometry;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    
    
    public class MeshCornerTests
    {
        [Theory]
        [MemberData(nameof(MeshTestData.Meshes), MemberType=typeof(MeshTestData))]
        public void HasPropertiesAssigned(Mesh mesh)
        {
            mesh.Corners.ForEach(
                corner =>
                {
                    Assert.NotNull(corner.Vertex);
                    Assert.NotNull(corner.Face);
                    Assert.NotNull(corner.Next);
                    Assert.NotNull(corner.Prev);
                    Assert.NotEqual(corner.Index, -1);
                });
        }
    }
}