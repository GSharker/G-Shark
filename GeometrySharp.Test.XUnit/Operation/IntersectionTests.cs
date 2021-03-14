using System;
using FluentAssertions;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class IntersectionTests
    {
        private readonly ITestOutputHelper _testOutput;
        public IntersectionTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_Two_Planes()
        {
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneYZ.SetOrigin(new Vector3{10,10,5});
            Plane pl2 = Plane.PlaneXZ.SetOrigin(new Vector3 {10, -10, -5});

            Ray intersection0 = Intersect.PlanePlane(pl0, pl1);
            Ray intersection1 = Intersect.PlanePlane(pl1, pl2);

            intersection0.Position.Should().BeEquivalentTo(new Vector3 {10, 0, 0});
            intersection0.Direction.Should().BeEquivalentTo(new Vector3 { 0, 1, 0 });

            intersection1.Position.Should().BeEquivalentTo(new Vector3 { 10, -10, 0 });
            intersection1.Direction.Should().BeEquivalentTo(new Vector3 { 0, 0, 1 });
        }

        [Fact]
        public void PlaneToPlane_Throws_An_Exception_If_Planes_Are_Parallel()
        {
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneXY.SetOrigin(new Vector3 { 10, 10, 5 });

            Func<object> func = () => Intersect.PlanePlane(pl0, pl1);

            func.Should().Throw<Exception>().WithMessage("The two planes are parallel.");
        }

        [Fact]
        public void It_Returns_The_Intersection_Point_Between_A_Segment_And_A_Plane()
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Line ln0 = new Line(new Vector3 { 0, 0, 0 }, new Vector3 { 20, 20, 10 });
            Line ln1 = new Line(new Vector3 { 0, 0, 0 }, new Vector3 { 5, 5, 10 });

            Vector3 pt0 = Intersect.LinePlane(ln0, pl);
            Vector3 pt1 = Intersect.LinePlane(ln1, pl);

            pt0.Equals(new Vector3 {10, 10, 5}).Should().BeTrue();
            pt1.Equals(new Vector3 { 10, 10, 20 }).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 10, 0 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 0, 10 })]
        [InlineData(new double[] { 10, 20, 5 }, new double[] { 10, 20, 10 })]
        public void LinePlane_Throws_An_Exception_If_Line_Is_Parallel(double[] startPt, double[] endPt)
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Line ln = new Line(new Vector3 (startPt), new Vector3(endPt));

            Func<object> func = () => Intersect.LinePlane(ln, pl);

            func.Should().Throw<Exception>().WithMessage("Segment parallel to the plane or lies in plane.");
        }
    }
}
