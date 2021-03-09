using FluentAssertions;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class CircleTests
    {
        private readonly ITestOutputHelper _testOutput;
        public CircleTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Circle BaseCircle
        {
            get
            {
                Vector3 center = new Vector3 { 85.591741, 24.79606, 1.064717 };
                Vector3 xDir = new Vector3 { -0.687455, 0.703828, -0.178976 };
                Vector3 yDir = new Vector3 { -0.726183, -0.663492, 0.180104 };
                Vector3 normal = new Vector3 { 0.008012, 0.253783, 0.967228 };
                Plane plane = new Plane(center, xDir, yDir, normal);

                return new Circle(plane, 23);
            }
        }

        [Fact]
        public void Initializes_A_Circle_By_A_Radius()
        {
            Circle circle = new Circle(23);

            circle.Should().NotBeNull();
            circle.Radius.Should().Be(23);
            circle.Center.Should().BeEquivalentTo(new Vector3 {0.0, 0.0, 0.0});
        }

        [Fact]
        public void It_Returns_The_Circumference_Of_A_Plane()
        {
            Circle circle = BaseCircle;

            double circumference = circle.Circumference;

            (circumference / Math.PI).Should().Be(46);
        }

        [Theory]
        [InlineData(0.0, new double[] {69.780279, 40.984093, -3.051743})]
        [InlineData(0.1, new double[] {62.982688, 28.922671, 0.169262})]
        public void It_Returns_The_Point_On_The_Circle_At_The_Give_Parameter_T(double t, double[] expectedPt)
        {
            Vector3 checkPt = new Vector3(expectedPt);
            Circle circle = BaseCircle;

            Vector3 pt = circle.PointAt(t);

            pt.IsEqualRoundingDecimal(checkPt, 4).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.0, new double[] { -0.726183, -0.663492, 0.180104 })]
        [InlineData(0.1, new double[] { -0.183418, -0.950475, 0.250907 })]
        public void It_Returns_The_Tangent_At_The_Give_Parameter_T(double t, double[] expectedPt)
        {
            Vector3 checkTangent = new Vector3(expectedPt);
            Circle circle = BaseCircle;

            Vector3 tangent = circle.TangentAt(t);

            tangent.IsEqualRoundingDecimal(checkTangent, 4).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_The_Circle()
        {
            Circle circle = BaseCircle;
            Vector3 minCheck = new Vector3 { 62.592479, 2.549053, -4.7752 };
            Vector3 maxCheck = new Vector3 { 108.591003, 47.043067, 6.904634 };

            BoundingBox bBox = circle.BoundingBox;

            bBox.Min.IsEqualRoundingDecimal(minCheck, 6).Should().BeTrue();
            bBox.Max.IsEqualRoundingDecimal(maxCheck, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 82.248292, 15.836914, 3.443127 }, new double[] { 77.787891, 3.884768, 6.616098 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 69.780279, 40.984093, -3.051743 })]
        public void It_Returns_The_Closest_Point_On_A_Circle(double[] ptToTest, double[] result)
        {
            Vector3 testPt = new Vector3(ptToTest);
            Vector3 expectedPt = new Vector3(result);

            Circle circle = BaseCircle;
            Vector3 pt = circle.ClosestPt(testPt);

            _testOutput.WriteLine(pt.ToString());
            pt.IsEqualRoundingDecimal(expectedPt, 4).Should().BeTrue();
        }
    }
}

