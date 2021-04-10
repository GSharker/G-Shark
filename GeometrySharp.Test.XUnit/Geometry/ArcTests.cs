using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
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

        [Fact]
        public void It_Returns_A_Point_On_The_Arc_At_The_Given_Parameter()
        {
            Vector3 expectedPt = new Vector3 {69.81863, 29.435661, -0.021966};

            Arc arc = ExampleArc3D;
            Vector3 pt = arc.PointAt(0.12);

            pt.IsEqualRoundingDecimal(expectedPt, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[]{ 82.248292, 15.836914, 3.443127 }, new double[] { 80.001066, 9.815219, 5.041724 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 74.264416, 36.39316, -1.884313 })]
        public void It_Returns_The_Closest_Point_On_An_Arc(double[] ptToTest, double[] result)
        {
            Vector3 testPt = new Vector3(ptToTest);
            Vector3 expectedPt = new Vector3(result);

            Arc arc = ExampleArc3D;
            Vector3 pt = arc.ClosestPt(testPt);

            _testOutput.WriteLine(pt.ToString());
            pt.IsEqualRoundingDecimal(expectedPt, 5).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Transformed_Arc()
        {
            Arc arc = ExampleArc3D;
            Vector3 expectedStartPt = new Vector3 { 16.47719, 0, 0 };
            Vector3 expectedEndPt = new Vector3 { -6.160353, -15.282272, 0 };
            Transform transform = Transform.PlaneToPlane(arc.Plane, Plane.PlaneXY);

            Arc arcTransformed = arc.Transform(transform);

            arcTransformed.PointAt(0.0).IsEqualRoundingDecimal(expectedStartPt, 6).Should().BeTrue();
            arcTransformed.PointAt(1.0).IsEqualRoundingDecimal(expectedEndPt, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.0, new double[] { -0.726183, -0.663492, 0.180104 })]
        [InlineData(0.1, new double[] { -0.370785, -0.897553, 0.238573 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter_T(double t, double[] expectedPt)
        {
            Vector3 checkTangent = new Vector3(expectedPt);
            Arc arc = ExampleArc3D;

            Vector3 tangent = arc.TangentAt(t);

            tangent.IsEqualRoundingDecimal(checkTangent, 4).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Representation_Of_The_Arc_From_0_To_90_Deg()
        {
            Arc arc = new Arc(Plane.PlaneYZ, 20, new Interval(0.0, 1.8));
            double[] weightChecks = new[] {1.0, 0.9004471023526769, 1.0, 0.9004471023526769, 1.0};
            Vector3[] ptChecks = new[] {
                new Vector3 { 0, 20, 0 },
                new Vector3 { 0, 20, 9.661101312331581 },
                new Vector3 { 0, 12.432199365413288, 15.666538192549668 },
                new Vector3 { 0, 4.864398730826554, 21.671975072767786 },
                new Vector3 { 0, -4.544041893861742, 19.476952617563903 }};

            NurbsCurve curve = arc.ToNurbsCurve();

            curve.ControlPoints.Count.Should().Be(5);
            curve.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                curve.ControlPoints[i].Equals(ptChecks[i]).Should().BeTrue();
                curve.Weights[i].Should().Be(weightChecks[i]);

                if (i < 3)
                {
                    curve.Knots[i].Should().Be(0);
                    curve.Knots[i + 5].Should().Be(1);
                }
                else
                {
                    curve.Knots[i].Should().Be(0.5);
                }
            }
        }

        [Fact]
        public void It_Returns_A_NurbsCurve_Representation_Of_ExampleArc3D()
        {
            Arc arc = ExampleArc3D;
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

            NurbsCurve curve = arc.ToNurbsCurve();

            curve.ControlPoints.Count.Should().Be(7);
            curve.Degree.Should().Be(2);

            for (int i = 0; i < ptChecks.Length; i++)
            {
                curve.ControlPoints[i].Equals(ptChecks[i]).Should().BeTrue();
                curve.Weights[i].Should().Be(weightChecks[i]);

                if (i < 3)
                {
                    curve.Knots[i].Should().Be(0);
                    curve.Knots[i + 7].Should().Be(1);
                }
                else if(i<5)
                {
                    curve.Knots[i].Should().Be(0.3333333333333333);
                }
                else
                {
                    curve.Knots[i].Should().Be(0.6666666666666666);
                }
            }
        }
    }
}
