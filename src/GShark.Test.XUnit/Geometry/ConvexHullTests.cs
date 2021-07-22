using GShark.Geometry;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class ConvexHullTests
    {
        private readonly ITestOutputHelper _testOutput;
        
        public ConvexHullTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        
        [Fact]
        public void It_Generates_A_Convex_Hull_For_A_3d_Set_Of_Points()
        {
            //Arrange
            //Random points on sphere plus sphere centroid
            List<Point3> pointSet = new List<Point3>()
            {
                new Point3(), //centroid at 0,0,0
                new Point3(7.634068, 6.375793, -1.034536),
                new Point3(-5.3673, -8.173048, -2.096037),
                new Point3(-2.650043, 2.574374, 9.292463),
                new Point3(-5.287558, 4.189759, -7.381575),
                new Point3(5.167184, -3.885535, -7.629078),
                new Point3(7.202291, -2.427026, 6.498965),
                new Point3(-0.285312, -8.002962, 5.989256),
                new Point3(-9.473178, 2.291361, 2.237982),
                new Point3(-1.618406, 9.624193, -2.18075),
                new Point3(4.526953, 6.246705, 6.362812),
                new Point3(-6.435259, -3.556436, 6.777847),
                new Point3(1.881371, 2.128979, -9.587903),
                new Point3(0.945476, -9.046681, -4.154954),
                new Point3(9.232401, -3.590601, -1.367609),
                new Point3(-8.716214, -2.91291, -3.942407),
                new Point3(4.856799, -8.735927, 0.308344),
                new Point3(-2.027307, -2.06058, -9.573089),
                new Point3(0.836308, -3.646638, 9.27376),
                new Point3(-6.235135, 7.505124, 2.190026),
                new Point3(2.353462, 2.067135, 9.496745)
            };

            List<Point3> vertices = new List<Point3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            //Act
            ConvexHull hull = new ConvexHull();
            hull.GenerateHull(pointSet,true, ref vertices, ref triangles, ref normals);

            //Assert
            vertices.ForEach(x => _testOutput.WriteLine(x.ToString()));
            triangles.ForEach(x => _testOutput.WriteLine(x.ToString()));
            var ptHash = pointSet.ToHashSet();
            var vertHash = vertices.ToHashSet();
            vertHash.IsProperSubsetOf(ptHash).Should().BeTrue();
            vertHash.Contains(new Point3()).Should().BeFalse();
        }
    }
}
