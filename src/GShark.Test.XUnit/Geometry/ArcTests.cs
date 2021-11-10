using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
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
            double angle = GSharkMath.ToRadians(40);
            _exampleArc2D = new Arc(Plane.PlaneXY, 15, angle);

            // Initializes an arc by 3 points.
            Point3 pt1 = new Point3(74.264416, 36.39316, -1.884313);
            Point3 pt2 = new Point3(97.679126, 13.940616, 3.812853);
            Point3 pt3 = new Point3(100.92443, 30.599893, -0.585116);
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
            arc.Length.Should().BeApproximately(10.471976, GSharkMath.MaxTolerance);
            arc.Center.Should().BeEquivalentTo(Plane.PlaneXY.Origin);
            arc.Radius.Should().Be(15);
            arc.Angle.Should().BeApproximately(0.698132, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Initializes_An_Arc_By_Three_Points()
        {
            // Arrange
            Arc arc = _exampleArc3D;

            // Assert
            arc.Length.Should().BeApproximately(71.333203, GSharkMath.MaxTolerance);
            arc.Radius.Should().BeApproximately(16.47719, GSharkMath.MaxTolerance);
            GSharkMath.ToDegrees(arc.Angle).Should().BeApproximately(248.045414, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Initializes_An_Arc_By_Two_Points_And_A_Direction()
        {
            // Arrange
            Point3 pt1 = new Point3(5, 5, 5);
            Point3 pt2 = new Point3(10, 15, 10);
            Point3 dir = new Point3(3, 3, 0);

            // Act
            Arc arc = Arc.ByStartEndDirection(pt1, pt2, dir);

            // Assert
            arc.StartPoint.EpsilonEquals(pt1, 1e-6).Should().BeTrue();
            arc.EndPoint.EpsilonEquals(pt2, 1e-6).Should().BeTrue();
            arc.Radius.Should().BeApproximately(12.247449, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_BoundingBox_Of_The_Arc()
        {
            // Arrange
            double angle = GSharkMath.ToRadians(40);
            Arc arc2D = new Arc(Plane.PlaneXY, 15, angle);
            Arc arc3D = _exampleArc3D;

            // Act
            BoundingBox bBox2D = arc2D.BoundingBox();
            BoundingBox bBox3D = arc3D.BoundingBox();

            // Assert
            bBox2D.Min.EpsilonEquals(new Vector3(11.490667, 0, 0), 6).Should().BeTrue();
            bBox2D.Max.EpsilonEquals(new Vector3(15, 9.641814, 0), 6).Should().BeTrue();

            bBox3D.Min.EpsilonEquals(new Vector3(69.115079, 8.858347, -1.884313), 6).Should().BeTrue();
            bBox3D.Max.EpsilonEquals(new Vector3(102.068402, 36.39316, 5.246477), 6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_Arc()
        {
            // Arrange
            Arc arc = _exampleArc3D;
            Point3 expectedStartPt = new Point3(16.47719, 0, 0);
            Point3 expectedEndPt = new Point3(-6.160353, -15.282272, 0);
            var transform = Transform.PlaneToPlane(arc.Plane, Plane.PlaneXY);

            // Act
            Arc arcTransformed = arc.Transform(transform);

            // Assert
            arcTransformed.StartPoint.EpsilonEquals(expectedStartPt, GSharkMath.MaxTolerance).Should().BeTrue();
            arcTransformed.EndPoint.EpsilonEquals(expectedEndPt, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Is_A_Curve_Representation_Of_The_Arc_From_0_To_90_Deg()
        {
            // Arrange
            double[] weightChecks = new[] { 1.0, 0.9004471023526769, 1.0, 0.9004471023526769, 1.0 };
            Point3[] ptChecks = new[] {
                new Point3(0, 20, 0),
                new Point3(0, 20, 9.661101312331581),
                new Point3(0, 12.432199365413288, 15.666538192549668),
                new Point3(0, 4.864398730826554, 21.671975072767786),
                new Point3(0, -4.544041893861742, 19.476952617563903)
            };

            // Act
            NurbsBase arc = new Arc(Plane.PlaneYZ, 20, new Interval(0.0, 1.8));

            // Assert
            arc.ControlPointLocations.Count.Should().Be(5);
            arc.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                arc.ControlPointLocations[i].Equals(ptChecks[i]).Should().BeTrue();
                arc.ControlPoints[i].W.Should().Be(weightChecks[i]);

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
            Point3[] ptChecks = {
                new Point3(74.264416, 36.39316, -1.8843129999999997),
                new Point3(63.73736394529969, 26.774907230101093, 0.7265431054950776),
                new Point3(72.2808868605866, 15.429871621311115, 3.6324963299804987),
                new Point3(80.8244097758736, 4.084836012521206, 6.538449554465901),
                new Point3(93.52800280921122, 10.812836698886068, 4.6679117389561),
                new Point3(106.23159584254901, 17.54083738525103, 2.797373923446271),
                new Point3(100.92443, 30.599893, -0.5851159999999997)
            };

            // Act
            NurbsBase arc = _exampleArc3D;

            // Assert
            arc.ControlPointLocations.Count.Should().Be(7);
            arc.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                arc.ControlPointLocations[i].EpsilonEquals(ptChecks[i], GSharkMath.MaxTolerance).Should().BeTrue();
                arc.ControlPoints[i].W.Should().Be(weightChecks[i]);

                if (i < 3)
                {
                    arc.Knots[i].Should().Be(0);
                    arc.Knots[i + 7].Should().Be(1);
                }
                else if (i < 5)
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
            Point3 startPt = new Point3(5, 5, 5);
            Point3 endPt = new Point3(10, 15, 10);
            Vector3 dir = new Vector3(3, 3, 0);
            double radiusExpected = 12.247449;
            double angleExpected = GSharkMath.ToRadians(60);
            Point3 centerExpected = new Point3(0, 10, 15);

            // Act
            Arc arc = Arc.ByStartEndDirection(startPt, endPt, dir);

            // Assert
            arc.Angle.Should().BeApproximately(angleExpected, 1e-6);
            arc.Radius.Should().BeApproximately(radiusExpected, 1e-6);
            arc.Plane.Origin.EpsilonEquals(centerExpected, 1e-6).Should().BeTrue();
        }
    }
}
