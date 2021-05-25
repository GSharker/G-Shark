using System.Collections.Generic;
using FluentAssertions;
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
            Vector3 center = new Vector3 { 85.591741, 24.79606, 1.064717 };
            Vector3 xDir = new Vector3 { -0.687455, 0.703828, -0.178976 };
            Vector3 yDir = new Vector3 { -0.726183, -0.663492, 0.180104 };
            Vector3 normal = new Vector3 { 0.008012, 0.253783, 0.967228 };
            Plane plane = new Plane(center, xDir, yDir, normal);
            _circle2D = new Circle(plane, 23);

            // Initializes a circle from 3 points.
            Vector3 pt1 = new Vector3 { 74.264416, 36.39316, -1.884313 };
            Vector3 pt2 = new Vector3 { 97.679126, 13.940616, 3.812853 };
            Vector3 pt3 = new Vector3 { 100.92443, 30.599893, -0.585116 };
            _circle3D = new Circle(pt1, pt2, pt3);
            #endregion
        }

        [Fact]
        public void Initializes_A_Circle_By_A_Radius()
        {
            // Assert
            int radius = 23;
            Vector3 expectedCenter = new Vector3 {0.0, 0.0, 0.0};

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
            Vector3[] ptsExpected = new []
            {
                new Vector3 {74.264416, 36.39316, -1.884313},
                new Vector3 {62.298962, 25.460683, 1.083287},
                new Vector3 {73.626287, 13.863582, 4.032316},
                new Vector3 {84.953611, 2.266482, 6.981346},
                new Vector3 {96.919065, 13.198959, 4.013746},
                new Vector3 {108.884519, 24.131437, 1.046146},
                new Vector3 {97.557194, 35.728537, -1.902883},
                new Vector3 {86.22987, 47.325637, -4.851913},
                new Vector3 {74.264416, 36.39316, -1.884313}
            };

            // Act
            List<Vector3> ctrPts = circle.ControlPoints;

            // Assert
            ctrPts.Count.Should().Be(9);
            for (int i = 0; i < ptsExpected.Length; i++)
            {
                ctrPts[i].IsEqualRoundingDecimal(ptsExpected[i], 6).Should().BeTrue();
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
            Vector3 expectedPt = new Vector3(pts);
            Circle circle = _circle2D;

            // Act
            Vector3 pt = circle.PointAt(t);

            // Assert
            pt.IsEqualRoundingDecimal(expectedPt, 4).Should().BeTrue();
        }

        [Theory]
        [InlineData(1.2, new double[] { 0.377597, -0.896416, 0.232075 })]
        [InlineData(2.5, new double[] { 0.993199, 0.110331, -0.037176 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter_T(double t, double[] pts)
        {
            // Arrange
            Vector3 expectedTangent = new Vector3(pts);
            Circle circle = _circle2D;

            // Act
            Vector3 tangent = circle.TangentAt(t);

            // Assert
            tangent.IsEqualRoundingDecimal(expectedTangent, 4).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_The_Circle()
        {
            // Arrange
            Circle circle = _circle2D;
            Vector3 minCheck = new Vector3 { 62.592479, 2.549053, -4.7752 };
            Vector3 maxCheck = new Vector3 { 108.591003, 47.043067, 6.904634 };

            // Act
            BoundingBox bBox = circle.BoundingBox;

            // Assert
            bBox.Min.IsEqualRoundingDecimal(minCheck, 6).Should().BeTrue();
            bBox.Max.IsEqualRoundingDecimal(maxCheck, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 82.248292, 15.836914, 3.443127 }, new double[] { 77.787891, 3.884768, 6.616098 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 69.780279, 40.984093, -3.051743 })]
        public void It_Returns_The_Closest_Point_On_A_Circle(double[] ptToTest, double[] result)
        {
            // Arrange
            Vector3 testPt = new Vector3(ptToTest);
            Vector3 expectedPt = new Vector3(result);

            // Act
            Circle circle = _circle2D;
            Vector3 pt = circle.ClosestPt(testPt);

            // Assert
            pt.IsEqualRoundingDecimal(expectedPt, 4).Should().BeTrue();
        }
    }
}

