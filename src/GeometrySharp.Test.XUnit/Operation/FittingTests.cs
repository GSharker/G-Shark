using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class FittingTests
    {
        private readonly ITestOutputHelper _testOutput;

        public FittingTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        List<Vector3> pts => new List<Vector3>
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
        public void t()
        {
            List<Vector3> pts = new List<Vector3>
            {
                new Vector3 {0, 0, 0},
                new Vector3 {3, 4, 0},
                new Vector3 {-1, 4, 0},
                new Vector3 {-4, 0, 0},
                new Vector3 {-4, -3, 0},
            };
            Matrix m = new Matrix
            {
                new List<double> {2.0, 1.0, 0.0, 0.0},
                new List<double> {1.0, 4.0, 1.0, 0.0},
                new List<double> {0.0, 1.0, 4.0, 1.0},
                new List<double> {0.0, 0.0, 2.0, 7.0,}
            };

            List<Vector3> p = new List<Vector3>
            {
                new Vector3 {6, 8, 0},
                new Vector3 {10, 24, 0},
                new Vector3 {-12, 16, 0},
                new Vector3 {-36, -3, 0}
            };

            Matrix matrixLu = Matrix.Decompose(m, out int[] permutation);
            Matrix ptsSolved = new Matrix();

            for (int i = 0; i < p[0].Count; i++)
            {
                Vector3 b = new Vector3();
                b = p.Select(pt => pt[i]).ToVector();

                Vector3 solution = Matrix.Solve(matrixLu, permutation, b);
                ptsSolved.Add(solution);
            }

            Matrix result = ptsSolved.Transpose();
            _testOutput.WriteLine(ptsSolved.ToString());
        }
    }
}
