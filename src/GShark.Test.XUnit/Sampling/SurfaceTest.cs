using FluentAssertions;
using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GShark.Test.XUnit.Sampling
{
    public class SurfaceTest
    {
        [Fact]
        public void It_Returns_The_Surface_Split_At_The_Given_Parameter_At_V_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            List<List<Point3>> surfacePtsLeft = new List<List<Point3>>
                    {
                        new List<Point3>{ new Point3(0.0, 0.0, 0.0), new Point3(0.0, 5.0, 2.0)},
                        new List<Point3>{ new Point3(5.0, 0.0, 0.0), new Point3(5.0,6.666666,3.333333)},
                        new List<Point3>{ new Point3(10.0, 0.0, 0.0), new Point3(10.0, 5.0, 1.0)}
                    };

            List<List<Point3>> surfacePtsRight = new List<List<Point3>>
                    {
                        new List<Point3>{ new Point3(0.0, 5.0, 2.0), new Point3(0.0, 10.0, 4.0)},
                        new List<Point3>{ new Point3(5.0, 6.666666, 3.333333), new Point3(5.0,10.0,5.0)},
                        new List<Point3>{ new Point3(10.0, 5.0, 1.0), new Point3(10.0, 10.0, 2.0)}
                    };

            List<List<double>> weightsLeft = new List<List<double>>
                    {
                        new List<double>{ 1, 1},
                        new List<double>{ 1, 1.5},
                        new List<double>{ 1, 1}
                    };

            List<List<double>> weightsRight = new List<List<double>>
                    {
                        new List<double>{ 1, 1},
                        new List<double>{ 1.5, 2},
                        new List<double>{ 1, 1}
                    };

            Point3 expectedPtLeft = new Point3(5.0, 3.333333, 1.444444);
            Point3 expectedPtRight = new Point3(5.0, 8.181818, 3.545455);

            // Act
            NurbsSurface[] surfaces = surface.SplitAt(0.5, SplitDirection.V);
            Point3 evaluatePtLeft = surfaces[0].PointAt(0.5, 0.5);
            Point3 evaluatePtRight = surfaces[1].PointAt(0.5, 0.5);

            // Assert
            evaluatePtLeft.DistanceTo(expectedPtLeft).Should().BeLessThan(GSharkMath.MinTolerance);
            evaluatePtRight.DistanceTo(expectedPtRight).Should().BeLessThan(GSharkMath.MinTolerance);

            surfaces[0].Weights.Should().BeEquivalentTo(weightsLeft);
            surfaces[1].Weights.Should().BeEquivalentTo(weightsRight);

            _ = surfaces[0].ControlPointLocations.Select((pts, i) => pts.Select((pt, j) =>
                pt.EpsilonEquals(surfacePtsLeft[i][j], GSharkMath.MinTolerance).Should().BeTrue()));
            _ = surfaces[1].ControlPointLocations.Select((pts, i) => pts.Select((pt, j) =>
                pt.EpsilonEquals(surfacePtsRight[i][j], GSharkMath.MinTolerance).Should().BeTrue()));
        }

        [Fact]
        public void It_Returns_The_Surface_Split_At_The_Given_Parameter_At_U_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            List<List<Point3>> surfacePtsTop = new List<List<Point3>>
            {
                new List<Point3>{ new Point3(0.0, 0.0, 0.0), new Point3(0.0, 10.0, 4.0) },
                new List<Point3>{ new Point3(2.5, 0.0, 0.0), new Point3(3.333333, 10.0,4.666666)},
                new List<Point3>{ new Point3(5.0, 0.0, 0.0), new Point3(5.0, 10.0, 4.333333)}
            };

            List<List<Point3>> surfacePtsBottom = new List<List<Point3>>
            {
                new List<Point3>{ new Point3(5.0, 0.0, 0.0), new Point3(5.0, 10.0, 4.333333)},
                new List<Point3>{ new Point3(7.5, 0.0, 0.0), new Point3(6.666666, 10.0, 4.0) },
                new List<Point3>{ new Point3(10.0, 0.0, 0.0), new Point3(10.0, 10.0, 2.0)}
            };

            List<List<double>> weightsTop = new List<List<double>>
            {
                new List<double>{ 1, 1},
                new List<double>{ 1, 1.5},
                new List<double>{ 1, 1.5}
            };

            List<List<double>> weightsBottom = new List<List<double>>
            {
                new List<double>{ 1, 1.5},
                new List<double>{ 1, 1.5},
                new List<double>{ 1, 1}
            };

            Point3 expectedPtTop = new Point3(2.894737, 5.789474, 2.578947);
            Point3 expectedPtBottom = new Point3(7.105263, 5.789474, 2.157895);

            // Act
            NurbsSurface[] surfaces = surface.SplitAt(0.5, SplitDirection.U);
            Point3 evaluatePtTop = surfaces[0].PointAt(0.5, 0.5);
            Point3 evaluatePtBottom = surfaces[1].PointAt(0.5, 0.5);

            // Assert
            evaluatePtTop.DistanceTo(expectedPtTop).Should().BeLessThan(GSharkMath.MinTolerance);
            evaluatePtBottom.DistanceTo(expectedPtBottom).Should().BeLessThan(GSharkMath.MinTolerance);

            surfaces[0].Weights.Should().BeEquivalentTo(weightsTop);
            surfaces[1].Weights.Should().BeEquivalentTo(weightsBottom);

            _ = surfaces[0].ControlPointLocations.Select((pts, i) => pts.Select((pt, j) =>
                pt.EpsilonEquals(surfacePtsTop[i][j], GSharkMath.MinTolerance).Should().BeTrue()));
            _ = surfaces[1].ControlPointLocations.Select((pts, i) => pts.Select((pt, j) =>
                pt.EpsilonEquals(surfacePtsBottom[i][j], GSharkMath.MinTolerance).Should().BeTrue()));
        }

        [Fact]
        public void It_Returns_The_Surface_Split_At_The_Given_Parameter_At_Both_Direction()
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            List<Point3> expectedPts = new List<Point3>
            {
                new Point3(2.714286, 3.142857, 1.4),
                new Point3(7.285714, 3.142857, 1.171429),
                new Point3(3.04878, 8.04878, 3.585366),
                new Point3(6.95122, 8.04878, 3)
            };

            // Act
            NurbsSurface[] splitSrf = surface.SplitAt(0.5, SplitDirection.Both);
            var ptsAt = splitSrf.Select(s => s.PointAt(0.5, 0.5));

            // Assert
            splitSrf.Length.Should().Be(4);
            _ = ptsAt.Select((pt, i) => pt.DistanceTo(expectedPts[i]).Should().BeLessThan(GSharkMath.MinTolerance));
        }

        [Theory]
        [InlineData(-0.2)]
        [InlineData(1.3)]
        public void Split_Surface_Throws_An_Exception_If_Parameter_Is_Outside_The_Domain(double parameter)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();

            // Act
            Func<object> func = () => surface.SplitAt(parameter, SplitDirection.U);

            // Assert
            func.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
