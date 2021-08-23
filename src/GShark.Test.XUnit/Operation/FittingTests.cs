using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class FittingTests
    {
        private readonly ITestOutputHelper _testOutput;

        public FittingTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static List<Point3> pts => new()
        {
            new(0, 0, 0),
            new(3, 4, 0),
            new(-1, 4, 0),
            new(-4, 0, 0),
            new(-4, -3, 0),
        };

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Interpolates_A_Collection_Of_Points(int degree)
        {
            // Act
            NurbsCurve crv = Fitting.InterpolatedCurve(pts, degree);

            // Assert
            crv.Degree.Should().Be(degree);
            crv.ControlPointLocations[0].DistanceTo(pts[0]).Should().BeLessThan(GSharkMath.MaxTolerance);
            crv.ControlPointLocations[^1].DistanceTo(pts[^1]).Should().BeLessThan(GSharkMath.MaxTolerance);

            foreach (var pt in pts)
            {
                var closestPt = crv.ClosestPoint(pt);
                closestPt.DistanceTo(pt).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void Interpolates_With_End_And_Start_Tangent()
        {
            // Arrange
            Vector3 v1 = new Vector3(1.278803, 1.06885, 0);
            Vector3 v2 = new Vector3(-4.204863, -2.021209, 0);

            // Act
            NurbsCurve crv = Fitting.InterpolatedCurve(pts, 2, v1, v2);

            // Assert
            crv.ControlPointLocations[0].DistanceTo(pts[0]).Should().BeLessThan(GSharkMath.MaxTolerance);
            crv.ControlPointLocations[^1].DistanceTo(pts[^1]).Should().BeLessThan(GSharkMath.MaxTolerance);

            foreach (var crvControlPoint in crv.ControlPointLocations)
            {
                _testOutput.WriteLine($"{{{crvControlPoint}}}");
            }

            foreach (var pt in pts)
            {
                var closedPt = crv.ClosestPoint(pt);
                closedPt.DistanceTo(pt).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void Returns_A_Sets_Of_Interpolated_Beziers_From_A_Collection_Of_Points()
        {
            // Act
            List<NurbsCurve> crvs = Fitting.BezierInterpolation(pts);

            // Assert
            crvs.Count.Should().Be(4);
            for (int i = 0; i < crvs.Count - 1; i++)
            {
                bool areCollinear = Trigonometry.ArePointsCollinear(crvs[i].ControlPointLocations[2], crvs[i].ControlPointLocations[3],
                     crvs[i + 1].ControlPointLocations[1]);
                areCollinear.Should().BeTrue();
            }
        }

        [Fact]
        public void Returns_An_Approximated_Curve()
        {
            // Arrange
            List<Point3> expectedCtrlPts = new List<Point3>
            {
                new (0, 0, 0),
                new (9.610024470158852, 8.200277881464892, 0.0),
                new (-8.160625855418692, 3.3820642030608417, 0.0),
                new (-4, -3, 0)
            };

            // Act
            NurbsCurve approximateCurve = Fitting.ApproximateCurve(pts, 3);

            // Assert
            approximateCurve.ControlPointLocations.Count.Should().Be(4);
            for (int i = 0; i < approximateCurve.ControlPointLocations.Count; i++)
            {
                approximateCurve.ControlPointLocations[i].DistanceTo(expectedCtrlPts[i]).Should().BeLessThan(GSharkMath.MaxTolerance);
            }
        }
    }
}
