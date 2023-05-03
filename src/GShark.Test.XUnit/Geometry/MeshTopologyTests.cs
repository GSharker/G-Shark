using System.Collections.Generic;
using GShark.Geometry;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    public class MeshTopologyTests
    {
        [Fact]
        public void CanCreate_MeshTopology()
        {
            // TODO: Improve this tests with better assertions.
            var topo = new MeshTopology(MeshTestData.FlatSquare);
            
            Assert.Empty(topo.FaceFace);
        }
        
        [Fact]
        public void CanConvert_ToString()
        {
            // TODO: Improve this tests with better assertions.
            var topo = new MeshTopology(MeshTestData.FlatSquare);
            
            Assert.NotNull(topo.TopologyDictToString(topo.FaceFace));
            Assert.NotNull(topo.TopologyDictToString(topo.VertexVertex));
            Assert.NotNull(topo.TopologyDictToString(topo.EdgeEdge));
        }
    }
}