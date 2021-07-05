using System.Drawing;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class ArcTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Arc _exampleArc3D;
        private readonly Arc _exampleArc2D;

        public ArcTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            #region example
            // Initializes an arc by plane, radius and angle.
            double angle = GeoSharpMath.ToRadians(40);
            _exampleArc2D = new Arc(Plane.PlaneXY, 15, angle);

            // Initializes an arc by 3 points.
            Point3d pt1 = new Point3d(74.264416, 36.39316, -1.884313);
            Point3d pt2 = new Point3d(97.679126, 13.940616, 3.812853);
            Point3d pt3 = new Point3d(100.92443, 30.599893, -0.585116);
            _exampleArc3D = new Arc(pt1, pt2, pt3);
            #endregion
        }

        [Fact]
        public void Initializes_An_Arc()
        {
            // Arrange
            Arc arc = _exampleArc2D;

            // Assert
            arc.Should().NotBeNull();
            arc.Length.Should().BeApproximately(10.471976, GeoSharpMath.MaxTolerance);
            arc.Center.Should().BeEquivalentTo(Plane.PlaneXY.Origin);
            arc.Radius.Should().Be(15);
            arc.Angle.Should().BeApproximately(0.698132, GeoSharpMath.MaxTolerance);
        }

        [Fact]
        public void Initializes_An_Arc_By_Three_Points()
        {
            // Arrange
            Arc arc = _exampleArc3D;

            // Assert
            arc.Length.Should().BeApproximately(71.333203, GeoSharpMath.MaxTolerance);
            arc.Radius.Should().BeApproximately(16.47719, GeoSharpMath.MaxTolerance);
            GeoSharpMath.ToDegrees(arc.Angle).Should().BeApproximately(248.045414, GeoSharpMath.MaxTolerance);
        }

        [Fact]
        public void Initializes_An_Arc_By_Two_Points_And_A_Direction()
        {
            // Arrange
            Point3d pt1 = new Point3d(5, 5, 5);
            Point3d pt2 = new Point3d(10, 15, 10);
            Point3d dir = new Point3d(3, 3, 0);

            // Act
            Arc arc = Arc.ByStartEndDirection(pt1, pt2, dir);

            // Assert
            arc.StartPoint.EpsilonEquals(pt1, 1e-6).Should().BeTrue();
            arc.EndPoint.EpsilonEquals(pt2, 1e-6).Should().BeTrue();
            arc.Radius.Should().BeApproximately(12.247449, GeoSharpMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_BoundingBox_Of_The_Arc()
        {
            // Arrange
            double angle = GeoSharpMath.ToRadians(40);
            Arc arc2D = new Arc(Plane.PlaneXY, 15, angle);
            Arc arc3D = _exampleArc3D;

            // Act
            BoundingBox bBox2D = arc2D.BoundingBox;
            BoundingBox bBox3D = arc3D.BoundingBox;

            // Assert
            bBox2D.Min.EpsilonEquals(new Vector3d(11.490667, 0, 0), 6).Should().BeTrue();
            bBox2D.Max.EpsilonEquals(new Vector3d(15, 9.641814, 0), 6).Should().BeTrue();
                       
            bBox3D.Min.EpsilonEquals(new Vector3d(69.115079, 8.858347, -1.884313), 6).Should().BeTrue();
            bBox3D.Max.EpsilonEquals(new Vector3d(102.068402, 36.39316, 5.246477), 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(1.2, new double[] { 70.334926, 18.808863, 2.762032 })]
        [InlineData(2.5, new double[] { 87.505564, 8.962333, 5.203339 })]
        public void It_Returns_A_Point_On_The_Arc_At_The_Given_Parameter(double t, double[] pts)
        {
            // Arrange
            var expectedPt = new Point3d(pts[0], pts[1], pts[2]);
            Arc arc = _exampleArc3D;

            // Act
            var pt = arc.PointAt(t);

            // Assert
            pt.EpsilonEquals(expectedPt, 1e-6).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[]{ 82.248292, 15.836914, 3.443127 }, new double[] { 80.001066, 9.815219, 5.041724 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 74.264416, 36.39316, -1.884313 })]
        public void It_Returns_The_Closest_Point_On_An_Arc(double[] ptToTest, double[] result)
        {
            // Arrange
            Point3d testPt = new Point3d(ptToTest[0], ptToTest[1], ptToTest[2]);
            Point3d expectedPt = new Point3d(result[0], result[1], result[2]);
            Arc arc = _exampleArc3D;

            // Act
            Point3d pt = arc.ClosestPoint(testPt);

            // Assert
            pt.EpsilonEquals(expectedPt, 1e-6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_Arc()
        {
            // Arrange
            Arc arc = _exampleArc3D;
            Point3d expectedStartPt = new Point3d(16.47719, 0, 0);
            Point3d expectedEndPt = new Point3d(-6.160353, -15.282272, 0);
            Transform transform = Transform.PlaneToPlane(arc.Plane, Plane.PlaneXY);

            // Act
            Arc arcTransformed = arc.Transform(transform);

            // Assert
            arcTransformed.StartPoint.EpsilonEquals(expectedStartPt, 1e-6).Should().BeTrue();
            arcTransformed.EndPoint.EpsilonEquals(expectedEndPt, 1e-6).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.0, new double[] { -0.726183, -0.663492, 0.180104 })]
        [InlineData(1.2, new double[] { 0.377597, -0.896416, 0.232075 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter_T(double t, double[] pts)
        {
            // Arrange
            Vector3d expectedTangent = new Vector3d(pts[0], pts[1], pts[2]);
            Arc arc = _exampleArc3D;

            // Act
            Vector3d tangent = arc.TangentAt(t);

            // Assert
            tangent.EpsilonEquals(expectedTangent, 1e-6).Should().BeTrue();
        }

        [Fact]
        public void It_Is_A_Curve_Representation_Of_The_Arc_From_0_To_90_Deg()
        {
            // Arrange
            double[] weightChecks = new[] {1.0, 0.9004471023526769, 1.0, 0.9004471023526769, 1.0};
            Point3d[] ptChecks = new[] {
                new Point3d(0, 20, 0),
                new Point3d(0, 20, 9.661101312331581),
                new Point3d(0, 12.432199365413288, 15.666538192549668),
                new Point3d(0, 4.864398730826554, 21.671975072767786),
                new Point3d(0, -4.544041893861742, 19.476952617563903)
            };

            // Act
            ICurve arc = new Arc(Plane.PlaneYZ, 20, new Interval(0.0, 1.8));

            // Assert
            arc.ControlPoints.Count.Should().Be(5);
            arc.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                arc.ControlPoints[i].Equals(ptChecks[i]).Should().BeTrue();
                arc.HomogenizedPoints[i].W.Should().Be(weightChecks[i]);

                if (i < 3)
                {
                    arc.Knots[i].Should().Be(0);
                    arc.Knots[i + 5].Should().Be(1);
                }
                else
                {
                    arc.Knots[i].Should().Be(0.5);
                }
            }
        }

        [Fact]
        public void It_Is_A_Curve_Representation_Of_ExampleArc3D()
        {
            // Arrange
            double[] weightChecks = { 1.0, 0.7507927793532885, 1.0, 0.7507927793532885, 1.0, 0.7507927793532885, 1.0 };
            Point3d[] ptChecks = {
                new Point3d(74.264416, 36.39316, -1.8843129999999997),
                new Point3d(63.73736394529969, 26.774907230101093, 0.7265431054950776),
                new Point3d(72.2808868605866, 15.429871621311115, 3.6324963299804987),
                new Point3d(80.8244097758736, 4.084836012521206, 6.538449554465901),
                new Point3d(93.52800280921122, 10.812836698886068, 4.6679117389561),
                new Point3d(106.23159584254901, 17.54083738525103, 2.797373923446271),
                new Point3d(100.92443, 30.599893, -0.5851159999999997)
            };

            // Act
            ICurve arc = _exampleArc3D;

            // Assert
            arc.ControlPoints.Count.Should().Be(7);
            arc.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                arc.ControlPoints[i].EpsilonEquals(ptChecks[i], GeoSharpMath.MaxTolerance).Should().BeTrue();
                arc.HomogenizedPoints[i].W.Should().Be(weightChecks[i]);

                if (i < 3)
                {
                    arc.Knots[i].Should().Be(0);
                    arc.Knots[i + 7].Should().Be(1);
                }
                else if(i<5)
                {
                    arc.Knots[i].Should().Be(0.3333333333333333);
                }
                else
                {
                    arc.Knots[i].Should().Be(0.6666666666666666);
                }
            }
        }

        [Fact]
        public void It_Returns_A_Arc_Based_On_A_Start_And_An_End_Point_And_A_Direction()
        {
            // Arrange
            Point3d startPt = new Point3d(5, 5, 5);
            Point3d endPt = new Point3d(10, 15, 10);
            Vector3d dir = new Vector3d(3, 3, 0);
            double radiusExpected = 12.247449;
            double angleExpected = GeoSharpMath.ToRadians(60);
            Point3d centerExpected = new Point3d(0, 10, 15);

            // Act
            Arc arc = Arc.ByStartEndDirection(startPt, endPt, dir);

            // Assert
            arc.Angle.Should().BeApproximately(angleExpected, 1e-6);
            arc.Radius.Should().BeApproximately(radiusExpected, 1e-6);
            arc.Plane.Origin.EpsilonEquals(centerExpected, 1e-6).Should().BeTrue();
        }
    }
}
