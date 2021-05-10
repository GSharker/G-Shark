using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.ExtendedMethods;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
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
            var crv = Fitting.InterpolatedCurve(pts, degree);

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

            var result = ptsSolved.Transpose();
            _testOutput.WriteLine(ptsSolved.ToString());
        }
    }
}
