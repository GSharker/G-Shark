using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class PolyCurveTests
    {
        private PolyCurve _polycurve;
        private NurbsCurve _nurbs;
        private Line _line;
        private Arc _arc;

        private readonly ITestOutputHelper _testOutput;

        public PolyCurveTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(0, 5, 5),
                new Point3(0, 0, 0),
                new Point3(5, 0, 0),
                new Point3(5, 0, 5),
                new Point3(5, 5, 5),
                new Point3(5, 5, 0)
            };

            _nurbs = new NurbsCurve(pts, degree);
            _line = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));
            _arc = Arc.ByStartEndDirection(new Point3(5, 5, -2.5), new Point3(10, 5, -7.5), new Vector3(0, 0, -1));

            _polycurve = new PolyCurve();
            _polycurve.Append(_nurbs);
            _polycurve.Append(_line);
            _polycurve.Append(_arc);
        }

        [Fact]
        public void It_Returns_A_PolyCurve_With_Three_Segments()
        {
            // Arrange
            int numberOfExpectedSegments = 3;

            // Act            

            // Assert
            _polycurve.Should().NotBeNull();
            _polycurve.SegmentCount.Should().Be(numberOfExpectedSegments);
        }

        [Fact]
        public void It_Returns_If_PolyCurve_Is_Closed()
        {
            // Arrange

            // Act            

            // Assert
            _polycurve.Should().NotBeNull();
            _polycurve.IsClosed.Should().BeFalse();

            var closedPolyCurve = _polycurve;
            closedPolyCurve.Close();
            closedPolyCurve.IsClosed.Should().BeTrue();
            closedPolyCurve.SegmentCount.Should().Be(4);
        }


        [Fact]
        public void It_Returns_A_PolyCurve_Start_And_End_Points()
        {
            // Arrange
            var startPoint = _nurbs.PointAt(0.0);
            var endPoint = _arc.EndPoint;

            // Act            

            // Assert
            _polycurve.StartPoint.Should().Be(startPoint);
            _polycurve.EndPoint.Should().Be(endPoint);
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_PolyCurve()
        {
            // Arrange
            var nested = new PolyCurve(_polycurve);

            // Act            
            var length = _polycurve.Length;
            var lengthNested = nested.Length;

            // Arrange
            length.Should().BeApproximately(30.623806269249716, GSharkMath.Epsilon);
            lengthNested.Should().BeApproximately(30.623806269249716, GSharkMath.Epsilon);

            _testOutput.WriteLine(string.Format("Length: {0}", length));
        }

        [Theory]
        [InlineData(new double[] { 5, 3.04250104617472, 4.51903625915119 }, 15)]
        [InlineData(new double[] { 5, 5, -1.73017533397891 }, 22)]
        [InlineData(new double[] { 6.00761470775174, 5, -5.51012618975348 }, 26)]
        public void It_Returns_A_Point_At_Length(double[] coords, double l)
        {
            // Arrange
            Point3 pc = new Point3(coords[0], coords[1], coords[2]);

            //Act
            var pt = _polycurve.PointAtLength(l);

            // Assert
            pt.DistanceTo(pc).Should().BeLessOrEqualTo(GSharkMath.MaxTolerance);

#if DEBUG
            _testOutput.WriteLine(string.Format("{0}", pt));
#endif
        }

        [Theory]
        [InlineData(new double[] { 0, 0.967518309198639, -0.252800952065861 }, 15)]
        [InlineData(new double[] { 0, 0, -1 }, 22)]
        [InlineData(new double[] { 0.602025237950695, 0, -0.798477058449652 }, 26)]
        public void It_Returns_The_Unitized_Tangent_At_Length(double[] coords, double l)
        {
            // Arrange
            Vector3 v = new Vector3(coords[0], coords[1], coords[2]);
            //Act
            var vp = _polycurve.TangentAtLength(l);

            // Assert
            vp.EpsilonEquals(v, 1e-5).Should().BeTrue();

#if DEBUG
            _testOutput.WriteLine(string.Format("{0} on the nurbs", vp));
#endif
        }

        [Theory]
        [InlineData(new double[] { 5, 3.04250104617472, 4.51903625915119 }, 0.265154444812697)]
        [InlineData(new double[] { 5, 5, -1.73017533397891 }, 0.564023377863855)]
        [InlineData(new double[] { 6.00761470775174, 5, -5.51012618975348 }, 0.803759565721669)]
        public void It_Returns_A_Point_At_Parameter(double[] coords, double t)
        {
            // Arrange
            Point3 pc = new Point3(coords[0], coords[1], coords[2]);

            //Act
            var pt = _polycurve.PointAt(t);

            // Assert
            pt.DistanceTo(pc).Should().BeLessOrEqualTo(GSharkMath.MinTolerance);

#if DEBUG
            _testOutput.WriteLine(string.Format("{0}", pt));
#endif
        }

        [Theory]
        [InlineData(new double[] { 0, 0.967518309198639, -0.252800952065861 }, 0.265154444812697)]
        [InlineData(new double[] { 0, 0, -1 }, 0.564023377863855)]
        [InlineData(new double[] { 0.602025237950695, 0, -0.798477058449652 }, 0.803759565721669)]
        public void It_Returns_The_Unitized_Tangent_At_Parameter(double[] coords, double t)
        {
            // Arrange
            Vector3 v = new Vector3(coords[0], coords[1], coords[2]);
            //Act
            var vp = _polycurve.TangentAt(t);

            // Assert
            vp.EpsilonEquals(v, GSharkMath.MinTolerance).Should().BeTrue();

#if DEBUG
            _testOutput.WriteLine(string.Format("{0} on the nurbs", vp));
#endif
        }

        [Theory]
        [InlineData(
            new double[] {7.15481954032674, 4.79086090227394, -6.55460521997569},
            new double[] {7.12797713014594, 5, -6.59285775895464})]
        [InlineData(
            new double[] {5.0540122130917, 4.83487464215808, -1.48980606046795},
            new double[] { 5, 5, -1.48980606046795 })]
        [InlineData(
            new double[] { 8.08052457366638, 4.71114088587457, -6.20376780791034 },
            new double[] { 7.69935578910596, 5, -6.93926077347116 })]
        public void It_Returns_The_Closest_Point(double[] coords, double[] coordsClosest)
        {
            // Arrange
            Point3 pt = new Point3(coords[0], coords[1], coords[2]);
            Point3 cp = new Point3(coordsClosest[0], coordsClosest[1], coordsClosest[2]);

            //Act
            var closest = _polycurve.ClosestPoint(pt);

            // Assert
            closest.DistanceTo(cp).Should().BeLessOrEqualTo(GSharkMath.MinTolerance);

#if DEBUG
            _testOutput.WriteLine(string.Format("{0}", pt));
#endif
        }
    }
}
