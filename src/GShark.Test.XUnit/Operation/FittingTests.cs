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

        public static List<Vector3> pts => new List<Vector3>
        {
            new Vector3 {0, 0, 0},
            new Vector3 {3, 4, 0},
            new Vector3 {-1, 4, 0},
            new Vector3 {-4, 0, 0},
            new Vector3 {-4, -3, 0},
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
            crv.ControlPoints[0].DistanceTo(pts[0]).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            crv.ControlPoints[^1].DistanceTo(pts[^1]).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);

            foreach (Vector3 pt in pts)
            {
                Vector3 closedPt = crv.ClosestPt(pt);
                closedPt.DistanceTo(pt).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            }
        }

        [Fact]
        public void Interpolates_With_End_And_Start_Tangent()
        {
            Vector3 v1 = new Vector3 { 1.278803, 1.06885, 0 };
            Vector3 v2 = new Vector3 { -4.204863, -2.021209, 0 };

            var newPts = new List<Vector3>(pts);
            newPts.Insert(1, v1);
            newPts.Insert(newPts.Count-1, v2);
            // Act
            NurbsCurve crv = Fitting.InterpolatedCurve(pts, 2, v1, v2);

            // Assert
            crv.ControlPoints[0].DistanceTo(pts[0]).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
            crv.ControlPoints[^1].DistanceTo(pts[^1]).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);

            foreach (var crvControlPoint in crv.ControlPoints)
            {
                _testOutput.WriteLine($"{{{crvControlPoint}}}");
            }

            foreach (Vector3 pt in pts)
            {
                Vector3 closedPt = crv.ClosestPt(pt);
                closedPt.DistanceTo(pt).Should().BeLessThan(GeoSharpMath.MAXTOLERANCE);
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
               bool areCollinear = Trigonometry.AreThreePointsCollinear(crvs[i].ControlPoints[2], crvs[i].ControlPoints[3],
                    crvs[i + 1].ControlPoints[1]);
               areCollinear.Should().BeTrue();
            }
        }

        [Fact]
        public void Returns_An_Approximated_Curve()
        {
            // Act
            NurbsCurve crvs = Fitting.ApproximateCurve(pts, 3);

            // Assert
        }
    }
}
