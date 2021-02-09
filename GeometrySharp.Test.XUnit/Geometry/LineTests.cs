using System;
using System.Linq;
using FluentAssertions;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class LineTests
    {
        private readonly ITestOutputHelper _testOutput;
        public LineTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Line ExampleLine => new Line(new Vector3 {5, 0, 0}, new Vector3 {15, 15, 0});

        public static TheoryData<Line> DataLine => new TheoryData<Line> {ExampleLine};

        [Fact]
        public void It_Returns_A_Line()
        {
            var p1 = new Vector3 {-0.913, 1.0, 4.68};
            var p2 = new Vector3 {6.363, 10.0, 7.971};
            var l = new Line(p1, p2);
            
            l.Should().NotBeNull();
            l.Start.All(p1.Contains).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_Line()
        {
            var p1 = new Vector3 { -0.913, 1.0, 4.68 };
            var p2 = new Vector3 { 6.363, 10.0, 7.971 };
            var l = new Line(p1, p2);
            double expectedLength = 12.03207;

            l.Length.Should().BeApproximately(expectedLength, 5);
        }

        [Fact]
        public void It_Returns_The_Line_Direction()
        {
            var p1 = new Vector3 { 0, 0, 0 };
            var p2 = new Vector3 { 5, 0, 0 };
            var l = new Line(p1, p2);
            var expectedDirection = new Vector3 { 1, 0, 0 };

            l.Direction.Should().BeEquivalentTo(expectedDirection);
        }

        [Fact]
        public void It_Returns_The_ClosestPoint()
        {
            var line = new Line(new Vector3{ 0, 0, 0 }, new Vector3{ 30, 45, 0 });
            var pt = new Vector3{ 10, 20, 0 };
            var expectedPt = new Vector3{ 12.30769230769231, 18.461538461538463, 0 };

            var closestPt = line.ClosestPoint(pt);

            closestPt.Should().BeEquivalentTo(expectedPt);
        }

        [Theory]
        [MemberData(nameof(DataLine))]
        public void PointAt_Throw_An_Exception_If_Parameter_Outside_The_Curve_Domain(Line line)
        {
            Func<Vector3> func = () => line.PointAt(2);

            func.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Parameter is outside the domain 0.0 to 1.0 *");
        }

        // Values compared with Rhino.
        [Theory]
        [InlineData(0.0, new[] {5.0, 0, 0})]
        [InlineData(0.15, new[] {6.5, 2.25, 0})]
        [InlineData(0.36, new[] {8.6, 5.4, 0})]
        [InlineData(0.85, new[] {13.5, 12.75, 0})]
        [InlineData(1.0, new[] {15.0, 15.0, 0})]
        public void It_Returns_The_Evaluated_Point_At_The_Given_Parameter(double t, double[] ptExpected)
        {
            var ptEvaluated = ExampleLine.PointAt(t);

            ptEvaluated.Equals(ptExpected.ToVector()).Should().BeTrue();
        }
    }
}
