using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
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
            NurbsBase circleNurbs = _circle3D.ToNurbs();

            // Assert
            circleNurbs.Knots.GetDomain(circleNurbs.Degree).Length.Should().Be(1.0);
            for (int ptIndex = 0; ptIndex < ptsExpected.Count; ptIndex++)
            {
                circleNurbs.ControlPointLocations[ptIndex].EpsilonEquals(ptsExpected[ptIndex], GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Returns_The_Circumference_Of_A_Circle()
        {
            // Arrange
            int expectedCircumference = 46;

            // Act
            double circumference = _circle2D.Length;

            // Assert
            (circumference / Math.PI).Should().Be(expectedCircumference);
        }

        [Theory]
        [InlineData(0.15, new double[] { 67.461878, 38.521847, -2.38649 })]
        [InlineData(0.5, new double[] { 63.708413, 31.686215, -0.561853 })]
        [InlineData(0.72, new double[] { 62.691414, 26.903904, 0.701361 })]
        public void It_Returns_The_Point_On_The_Circle_At_The_Give_Parameter(double t, double[] pts)
        {
            // Arrange
            Point3 expectedPt = new Point3(pts[0], pts[1], pts[2]);

            // Act
            Point3 pt = _circle2D.PointAt(t);

            // Assert
            pt.EpsilonEquals(expectedPt, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.15, new double[] { -0.615297, -0.76122, 0.204827 })]
        [InlineData(0.5, new double[] { -0.307702, -0.919702, 0.243862 })]
        [InlineData(0.72, new double[] { -0.092651, -0.96291, 0.253417 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter(double t, double[] pts)
        {
            // Arrange
            Vector3 expectedTangent = new Vector3(pts[0], pts[1], pts[2]);

            // Act
            Vector3 tangent = _circle2D.TangentAt(t);

            // Assert
            tangent.EpsilonEquals(expectedTangent, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(10, new double[] { 64.216161, 33.050142, -0.923928 })]
        [InlineData(17.5, new double[] { 62.623469, 25.99726, 0.939811 })]
        [InlineData(22.5, new double[] { 62.906638, 21.177684, 2.202032 })]
        public void It_Returns_The_Point_On_The_Circle_At_The_Give_Length(double length, double[] pts)
        {
            // Arrange
            Point3 expectedPt = new Point3(pts[0], pts[1], pts[2]);

            // Act
            double normalizeLength = GSharkMath.RemapValue(length, new Interval(0.0, _circle2D.Length), new Interval(0.0, 1.0));
            Point3 pt = _circle2D.PointAtLength(length);
            Point3 ptNormalizedLength = _circle2D.PointAtNormalizedLength(normalizeLength);

            // Assert
            pt.EpsilonEquals(expectedPt, GSharkMath.MaxTolerance).Should().BeTrue();
            pt.EpsilonEquals(ptNormalizedLength, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(10, new double[] { -0.369055, -0.898223, 0.238734 })]
        [InlineData(17.5, new double[] { -0.051893, -0.96585, 0.253851 })]
        [InlineData(22.5, new double[] { 0.164714, -0.954382, 0.249048 })]
        public void It_Returns_The_Tangent_At_The_Give_Length(double length, double[] pts)
        {
            // Arrange
            Vector3 expectedTangent = new Vector3(pts[0], pts[1], pts[2]);

            // Act
            Vector3 tangent = _circle2D.TangentAtLength(length);

            // Assert
            tangent.EpsilonEquals(expectedTangent, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(1.2, 27.599999)]
        [InlineData(4.0, 92)]
        [InlineData(5.1, 117.3)]
        public void It_Returns_The_Length_At_The_Give_Parameter(double parameter, double expectedLength)
        {
            // Act
            double length = _circle2D.LengthAt(parameter);

            // Assert
            length.Should().BeApproximately(expectedLength, GSharkMath.MinTolerance);
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_The_Circle()
        {
            // Arrange
            Point3 minCheck = new Point3(62.592479, 2.549050, -4.77519);
            Point3 maxCheck = new Point3(108.591002, 47.043069, 6.904624);

            // Act
            BoundingBox bBox = _circle2D.GetBoundingBox();

            // Assert
            bBox.Min.EpsilonEquals(minCheck, GSharkMath.MaxTolerance).Should().BeTrue();
            bBox.Max.EpsilonEquals(maxCheck, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 82.248292, 15.836914, 3.443127 }, new double[] { 80.001065, 9.815219, 5.041723 })]
        [InlineData(new double[] { 89.12029, 34.989032, -1.63896 }, new double[] { 90.82015, 39.899444, -2.941443 })]
        public void It_Returns_The_Closest_Point_On_A_Circle(double[] ptToTest, double[] result)
        {
            // Arrange
            Point3 testPt = new Point3(ptToTest[0], ptToTest[1], ptToTest[2]);
            Point3 expectedPt = new Point3(result[0], result[1], result[2]);

            // Act
            Circle circle = _circle3D;
            Point3 pt = circle.ClosestPoint(testPt);

            // Assert
            pt.EpsilonEquals(expectedPt, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Circle()
        {
            // Arrange
            double offset = -5;

            // Act
            Circle offsetResult = _circle2D.Offset(offset);

            // Assert
            offsetResult.Plane.Origin.Should().BeEquivalentTo(_circle2D.Plane.Origin);
            (offsetResult.Radius - offset).Should().Be(_circle2D.Radius);
        }

        [Theory]
        [InlineData(new double[] { 82.248292, 15.836914, 3.443127 }, 2.037401)]
        [InlineData(new double[] { 89.12029, 34.989032, -1.63896 }, 5.202272)]
        public void It_Returns_The_Closest_Parameter_On_A_Circle(double[] ptToTest, double expectedParameter)
        {
            // Arrange
            Point3 testPt = new Point3(ptToTest[0], ptToTest[1], ptToTest[2]);

            // Act
            double parameter = _circle3D.ClosestParameter(testPt);

            // Assert
            parameter.Should().BeApproximately(expectedParameter, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Derivatives_At_Given_Parameter()
        {
            // Arrange
            Vector3 expectedDerv0 = new Vector3(-12.988226, 9.833148, -2.472443);
            Vector3 expectedDerv1 = new Vector3(-10.13836, -12.542767, 3.374974);
            Vector3 expectedDerv2 = new Vector3(12.988226, -9.833148, 2.472443);
            Vector3 expectedDerv3 = new Vector3(10.13836, 12.542767, -3.374974);

            // Act
            Vector3 derv0 = _circle3D.DerivativeAt(0.15);
            Vector3 derv1 = _circle3D.DerivativeAt(0.15, 1);
            Vector3 derv2 = _circle3D.DerivativeAt(0.15, 2);
            Vector3 derv3 = _circle3D.DerivativeAt(0.15, 3);

            // Assert
            // The zero derivative is the vector identify the point on the circle.
            expectedDerv0.IsParallelTo(derv0).Should().NotBe(0);
            expectedDerv1.IsParallelTo(derv1).Should().NotBe(0);
            expectedDerv2.IsParallelTo(derv2).Should().NotBe(0);
            expectedDerv3.IsParallelTo(derv3).Should().NotBe(0);
        }
    }
}

