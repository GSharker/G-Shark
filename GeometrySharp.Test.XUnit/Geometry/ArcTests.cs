using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class ArcTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ArcTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Arc ExampleArc3D
        {
            get
            {
                Vector3 pt1 = new Vector3 { 74.264416, 36.39316, -1.884313 };
                Vector3 pt2 = new Vector3 { 97.679126, 13.940616, 3.812853 };
                Vector3 pt3 = new Vector3 { 100.92443, 30.599893, -0.585116 };

                return new Arc(pt1, pt2, pt3);
            }
        }

        [Fact]
        public void Initializes_An_Arc()
        {
            // Arrange
            double angle = GeoSharpMath.ToRadians(40);
            Arc arc = new Arc(Plane.PlaneXY, 15, angle);

            // Assert
            arc.Should().NotBeNull();
            arc.Length.Should().BeApproximately(10.471976, GeoSharpMath.MAXTOLERANCE);
            arc.Center.Should().BeEquivalentTo(Plane.PlaneXY.Origin);
            arc.Radius.Should().Be(15);
            arc.Angle.Should().BeApproximately(0.698132, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void Initializes_An_Arc_By_Three_Points()
        {
            // Act
            Arc arc = ExampleArc3D;

            // Arrange
            arc.Length.Should().BeApproximately(71.333203, GeoSharpMath.MAXTOLERANCE);
            arc.Radius.Should().BeApproximately(16.47719, GeoSharpMath.MAXTOLERANCE);
            GeoSharpMath.ToDegrees(arc.Angle).Should().BeApproximately(248.045414, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void Initializes_An_Arc_By_Two_Points_And_A_Direction()
        {
            // Arrange
            Vector3 pt1 = new Vector3 { 5, 5, 5 };
            Vector3 pt2 = new Vector3 { 10, 15, 10 };
            Vector3 dir = new Vector3 { 3, 3, 0 };
            // Act
            Arc arc = Arc.ByStartEndDirection(pt1, pt2, dir);

            // Arrange
            arc.StartPoint.IsEqualRoundingDecimal(pt1, 6).Should().BeTrue();
            arc.EndPoint.IsEqualRoundingDecimal(pt2, 6).Should().BeTrue();
            arc.Radius.Should().BeApproximately(12.247449, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_The_BoundingBox_Of_The_Arc()
        {
            // Arrange
            double angle = GeoSharpMath.ToRadians(40);
            Arc arc2D = new Arc(Plane.PlaneXY, 15, angle);
            Arc arc3D = ExampleArc3D;

            // Act
            BoundingBox bBox2D = arc2D.BoundingBox;
            BoundingBox bBox3D = arc3D.BoundingBox;

            // Assert
            bBox2D.Min.IsEqualRoundingDecimal(new Vector3 {11.490667, 0, 0}, 6).Should().BeTrue();
            bBox2D.Max.IsEqualRoundingDecimal(new Vector3 { 15, 9.641814, 0 }, 6).Should().BeTrue();

            bBox3D.Min.IsEqualRoundingDecimal(new Vector3 { 69.115079, 8.858347, -1.884313 }, 6).Should().BeTrue();
            bBox3D.Max.IsEqualRoundingDecimal(new Vector3 { 102.068402, 36.39316, 5.246477 }, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(1.2, new double[] { 70.334926, 18.808863, 2.762032 })]
        [InlineData(2.5, new double[] { 87.505564, 8.962333, 5.203339 })]
        public void It_Returns_A_Point_On_The_Arc_At_The_Given_Parameter(double t, double[] pts)
        {
            // Arrange
            Vector3 expectedPt = new Vector3(pts);
            Arc arc = ExampleArc3D;

            // Act
            Vector3 pt = arc.PointAt(t);

            // Assert
            pt.IsEqualRoundingDecimal(expectedPt, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[]{ 82.248292, 15.836914, 3.443127 }, new double[] { 80.001066, 9.815219, 5.041724 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 74.264416, 36.39316, -1.884313 })]
        public void It_Returns_The_Closest_Point_On_An_Arc(double[] ptToTest, double[] result)
        {
            // Arrange
            Vector3 testPt = new Vector3(ptToTest);
            Vector3 expectedPt = new Vector3(result);
            Arc arc = ExampleArc3D;

            // Act
            Vector3 pt = arc.ClosestPt(testPt);

            // Assert
            pt.IsEqualRoundingDecimal(expectedPt, 5).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_Arc()
        {
            // Arrange
            Arc arc = ExampleArc3D;
            Vector3 expectedStartPt = new Vector3 { 16.47719, 0, 0 };
            Vector3 expectedEndPt = new Vector3 { -6.160353, -15.282272, 0 };
            Transform transform = Transform.PlaneToPlane(arc.Plane, Plane.PlaneXY);

            // Act
            Arc arcTransformed = arc.Transform(transform);

            // Assert
            arcTransformed.StartPoint.IsEqualRoundingDecimal(expectedStartPt, 6).Should().BeTrue();
            arcTransformed.EndPoint.IsEqualRoundingDecimal(expectedEndPt, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.0, new double[] { -0.726183, -0.663492, 0.180104 })]
        [InlineData(1.2, new double[] { 0.377597, -0.896416, 0.232075 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter_T(double t, double[] pts)
        {
            // Assert
            Vector3 expectedTangent = new Vector3(pts);
            Arc arc = ExampleArc3D;

            // Act
            Vector3 tangent = arc.TangentAt(t);

            // Arrange
            tangent.IsEqualRoundingDecimal(expectedTangent, 6).Should().BeTrue();
        }

        [Fact]
        public void It_Is_A_Curve_Representation_Of_The_Arc_From_0_To_90_Deg()
        {
            // Arrange
            double[] weightChecks = new[] {1.0, 0.9004471023526769, 1.0, 0.9004471023526769, 1.0};
            Vector3[] ptChecks = new[] {
                new Vector3 { 0, 20, 0 },
                new Vector3 { 0, 20, 9.661101312331581 },
                new Vector3 { 0, 12.432199365413288, 15.666538192549668 },
                new Vector3 { 0, 4.864398730826554, 21.671975072767786 },
                new Vector3 { 0, -4.544041893861742, 19.476952617563903 }
            };

            // Act
            Curve arc = new Arc(Plane.PlaneYZ, 20, new Interval(0.0, 1.8));

            // Assert
            arc.ControlPoints.Count.Should().Be(5);
            arc.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                arc.ControlPoints[i].Equals(ptChecks[i]).Should().BeTrue();
                arc.HomogenizedPoints[i][^1].Should().Be(weightChecks[i]);

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
            double[] weightChecks = new[] { 1.0, 0.7507927793532885, 1.0, 0.7507927793532885, 1.0, 0.7507927793532885, 1.0 };
            Vector3[] ptChecks = new[] {
                new Vector3 { 74.264416, 36.39316, -1.8843129999999997 },
                new Vector3 { 63.73736394529969, 26.774907230101093, 0.7265431054950776 },
                new Vector3 { 72.2808868605866, 15.429871621311115, 3.6324963299804987 },
                new Vector3 { 80.8244097758736, 4.084836012521206, 6.538449554465901 },
                new Vector3 { 93.52800280921122, 10.812836698886068, 4.6679117389561 },
                new Vector3 { 106.23159584254901, 17.54083738525103, 2.797373923446271 },
                new Vector3 { 100.92443, 30.599893, -0.5851159999999997 }
            };

            // Act
            Curve arc = ExampleArc3D;

            // Assert
            arc.ControlPoints.Count.Should().Be(7);
            arc.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                arc.ControlPoints[i].Equals(ptChecks[i]).Should().BeTrue();
                arc.HomogenizedPoints[i][^1].Should().Be(weightChecks[i]);

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
            Vector3 startPt = new Vector3 {5, 5, 5};
            Vector3 endPt = new Vector3 { 10, 15, 10 };
            Vector3 dir = new Vector3 { 3, 3, 0 };
            double radiusExpected = 12.247449;
            double angleExpected = GeoSharpMath.ToRadians(60);
            Vector3 centerExpected = new Vector3 { 0, 10, 15 };

            // Act
            Arc arc = Arc.ByStartEndDirection(startPt, endPt, dir);

            // Assert
            arc.Angle.Should().BeApproximately(angleExpected, 1e-6);
            arc.Radius.Should().BeApproximately(radiusExpected, 1e-6);
            arc.Plane.Origin.IsEqualRoundingDecimal(centerExpected, 6).Should().BeTrue();
        }
    }
}
