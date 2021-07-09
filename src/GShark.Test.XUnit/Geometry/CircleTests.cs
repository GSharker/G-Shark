using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class CircleTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Circle _circle2D;
        private readonly Circle _circle3D;

        public CircleTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            #region example
            // Initializes a circle from a plane and a radius.
            Point3 center = new Point3(85.591741, 24.79606, 1.064717);
            Vector3 xDir = new Vector3(-0.687455, 0.703828, -0.178976);
            Vector3 yDir = new Vector3(-0.726183, -0.663492, 0.180104);
            Plane plane = new Plane(center, xDir, yDir);
            _circle2D = new Circle(plane, 23);

            // Initializes a circle from 3 points.
            Point3 pt1 = new Point3(74.264416, 36.39316, -1.884313);
            Point3 pt2 = new Point3(97.679126, 13.940616, 3.812853);
            Point3 pt3 = new Point3(100.92443, 30.599893, -0.585116);
            _circle3D = new Circle(pt1, pt2, pt3);
            #endregion
        }

        [Fact]
        public void Initializes_A_Circle_By_A_Radius()
        {
            // Assert
            int radius = 23;
            Point3 expectedCenter = new Point3(0.0, 0.0, 0.0);

            // Act
            Circle circle = new Circle(radius);

            // Assert
            circle.Should().NotBeNull();
            circle.Radius.Should().Be(radius);
            circle.Center.Should().BeEquivalentTo(expectedCenter);
        }

        [Fact]
        public void It_Returns_A_Circle3D_With_Its_Nurbs_Representation()
        {
            // Arrange
            Circle circle = _circle3D;
            var ptsExpected = new List<Point3>
            {
                new Point3(74.264416, 36.39316, -1.884313),
                new Point3(62.298962, 25.460683, 1.083287),
                new Point3(73.626287, 13.863582, 4.032316),
                new Point3(84.953611, 2.266482, 6.981346),
                new Point3(96.919065, 13.198959, 4.013746),
                new Point3(108.884519, 24.131437, 1.046146),
                new Point3(97.557194, 35.728537, -1.902883),
                new Point3(86.22987, 47.325637, -4.851913),
                new Point3(74.264416, 36.39316, -1.88431)
            };

            // Act
            List<Point3> ctrPts = circle.LocationPoints;

            // Assert
            for (int ptIndex = 0; ptIndex < ptsExpected.Count; ptIndex++)
            {
                ctrPts[ptIndex].EpsilonEquals(ptsExpected[ptIndex], GeoSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Returns_The_Circumference_Of_A_Plane()
        {
            // Arrange
            Circle circle = _circle2D;
            int expectedCircumference = 46;

            // Act
            double circumference = circle.Circumference;

            // Assert
            (circumference / Math.PI).Should().Be(expectedCircumference);
        }

        [Theory]
        [InlineData(1.2, new double[] { 64.295230, 16.438716, 3.433960 })]
        [InlineData(2.5, new double[] { 88.263188, 2.694245, 6.841687 })]
        public void It_Returns_The_Point_On_The_Circle_At_The_Give_Parameter_T(double t, double[] pts)
        {
            // Arrange
            Point3 expectedPt = new Point3(pts[0],pts[1],pts[2]);
            Circle circle = _circle2D;

            // Act
            Point3 pt = circle.PointAt(t);

            // Assert
            pt.EpsilonEquals(expectedPt, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(1.2, new double[] { 0.377597, -0.896416, 0.232075 })]
        [InlineData(2.5, new double[] { 0.993199, 0.110330, -0.037177 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter_T(double t, double[] pts)
        {
            // Arrange
            Vector3 expectedTangent = new Vector3(pts[0], pts[1], pts[2]);
            Circle circle = _circle2D;

            // Act
            Point3 tangent = circle.TangentAt(t);

            // Assert
            tangent.EpsilonEquals(expectedTangent, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_The_Circle()
        {
            // Arrange
            Circle circle = _circle2D;
            Point3 minCheck = new Point3(62.592479, 2.549050, -4.77519);
            Point3 maxCheck = new Point3(108.591002, 47.043069, 6.904624);

            // Act
            BoundingBox bBox = circle.BoundingBox;

            // Assert
            bBox.Min.EpsilonEquals(minCheck, GeoSharkMath.MaxTolerance).Should().BeTrue();
            bBox.Max.EpsilonEquals(maxCheck, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 82.248292, 15.836914, 3.443127 }, new double[] { 77.787894, 3.884764, 6.616100 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 69.780281, 40.984098, -3.051729 })]
        public void It_Returns_The_Closest_Point_On_A_Circle(double[] ptToTest, double[] result)
        {
            // Arrange
            Point3 testPt = new Point3(ptToTest[0], ptToTest[1], ptToTest[2]);
            Point3 expectedPt = new Point3(result[0], result[1], result[2]);

            // Act
            Circle circle = _circle2D;
            Point3 pt = circle.ClosestPoint(testPt);

            // Assert
            pt.EpsilonEquals(expectedPt, GeoSharkMath.MaxTolerance).Should().BeTrue();
        }
    }
}

