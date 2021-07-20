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
            new (0, 0, 0),
            new (3, 4, 0),
            new (-1, 4, 0),
            new (-4, 0, 0),
            new (-4, -3, 0),
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
            crv.LocationPoints[0].DistanceTo(pts[0]).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            crv.LocationPoints[crv.LocationPoints.Count - 1].DistanceTo(pts[pts.Count - 1]).Should().BeLessThan(GeoSharkMath.MaxTolerance);

            foreach (var pt in pts)
            {
                var closestPt = crv.ClosestPoint(pt);
                closestPt.DistanceTo(pt).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void Interpolates_With_End_And_Start_Tangent()
        {
            Point3 v1 = new Point3(1.278803, 1.06885, 0);
            Point3 v2 = new Point3(-4.204863, -2.021209, 0);

            var newPts = new List<Point3>(pts);
            newPts.Insert(1, v1);
            newPts.Insert(newPts.Count-1, v2);
            // Act
            NurbsCurve crv = Fitting.InterpolatedCurve(pts, 2, v1, v2);

            // Assert
            crv.LocationPoints[0].DistanceTo(pts[0]).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            crv.LocationPoints[crv.LocationPoints.Count - 1].DistanceTo(pts[pts.Count - 1]).Should().BeLessThan(GeoSharkMath.MaxTolerance);

            foreach (var crvControlPoint in crv.LocationPoints)
            {
                _testOutput.WriteLine($"{{{crvControlPoint}}}");
            }

            foreach (var pt in pts)
            {
                var closedPt = crv.ClosestPoint(pt);
                closedPt.DistanceTo(pt).Should().BeLessThan(GeoSharkMath.MaxTolerance);
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
               bool areCollinear = Trigonometry.ArePointsCollinear(crvs[i].LocationPoints[2], crvs[i].LocationPoints[3],
                    crvs[i + 1].LocationPoints[1]);
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
            approximateCurve.LocationPoints.Count.Should().Be(4);
            for (int i = 0; i < approximateCurve.LocationPoints.Count; i++)
            {
                approximateCurve.LocationPoints[i].DistanceTo(expectedCtrlPts[i]).Should().BeLessThan(GeoSharkMath.MaxTolerance);
            }
        }
    }
}
