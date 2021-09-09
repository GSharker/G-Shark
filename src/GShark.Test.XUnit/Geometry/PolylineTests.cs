using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GShark.Test.XUnit.Geometry
{
    public class PolylineTests
    {
        public readonly Point3[] ExamplePts;
        private readonly Polyline _polyline;
        public PolylineTests()
        {
            #region example
            ExamplePts = new[]
            {
                new Point3(5, 0, 0),
                new Point3(15, 15, 0),
                new Point3(20, 5, 0),
                new Point3(30, 10, 0),
                new Point3(45, 12.5, 0)
            };

            _polyline = new Polyline(ExamplePts);
            #endregion
        }

        [Fact]
        public void It_Returns_A_Polyline()
        {
            // Arrange
            int numberOfExpectedSegments = 4;

            // Act
            Polyline polyline = _polyline;

            // Arrange
            polyline.SegmentsCount.Should().Be(numberOfExpectedSegments);
            polyline.Count.Should().Be(ExamplePts.Length);
            polyline[0].Should().BeEquivalentTo(ExamplePts[0]);
        }

        [Fact]
        public void Polyline_Throws_An_Exception_If_Vertex_Count_Is_Less_Than_Two()
        {
            // Arrange
            Point3[] pts = new Point3[] { new Point3(5, 0, 0) };

            // Act
            Func<Polyline> func = () => new Polyline(pts);

            // Assert
            func.Should().Throw<Exception>().WithMessage("Insufficient points for a polyline.");
        }

        [Fact]
        public void It_Returns_A_Polyline_Removing_Short_Segments()
        {
            // Arrange
            Point3[] pts = {new (5, 5, 0),
                new (5, 10, 0),
                new (5, 10, 0),
                new (15, 12, 0),
                new (20, -20, 0),
                new (20, -20, 0)
            };

            Point3[] ptsExpected = {new(5, 5, 0),
                new(5, 10, 0),
                new(15, 12, 0),
                new(20, -20, 0)
            };

            // Act
            Polyline polyline = new Polyline(pts);

            // Assert
            polyline.Should().BeEquivalentTo(ptsExpected);
        }

        [Fact]
        public void It_Returns_A_Closed_Polyline()
        {
            // Arrange
            Polyline closedPolyline = _polyline.Close();

            // Assert
            closedPolyline[0].DistanceTo(closedPolyline[closedPolyline.Count - 1]).Should().BeLessThan(GSharkMath.Epsilon);
        }

        [Fact]
        public void It_Returns_The_Length_Of_A_Polyline()
        {
            // Arrange
            double expectedLength = 55.595342;

            // Act
            double length = _polyline.Length;

            // Assert
            length.Should().BeApproximately(expectedLength, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Collection_Of_Lines()
        {
            // Arrange
            int expectedNumberOfSegments = 4;
            double expectedSegmentLength = 11.18034;

            // Act
            var segments = _polyline.Segments;

            // Assert
            segments.Count.Should().Be(expectedNumberOfSegments);
            segments[1].Length.Should().Be(segments[2].Length)
                .And.BeApproximately(expectedSegmentLength, GSharkMath.MaxTolerance);
        }

        [Theory]
        [InlineData(0.0, new double[] { 5, 0, 0 })]
        [InlineData(0.25, new double[] { 7.5, 3.75, 0 })]
        [InlineData(2.5, new double[] { 25, 7.5, 0 })]
        [InlineData(4.0, new double[] { 45, 12.5, 0 })]
        public void It_Returns_A_Point_At_The_Given_Parameter(double t, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            Point3 ptResult = _polyline.PointAt(t);

            // Assert
            ptResult.EpsilonEquals(expectedPt, GSharkMath.MaxTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.346154, new double[] { 5, 7.5, 0 })]
        [InlineData(2.0, new double[] { 15, -3, 5 })]
        [InlineData(2.48, new double[] { 22.5, 12, -3.0 })]
        public void It_Returns_A_Parameter_Along_The_Polyline_At_The_Given_Point(double expectedParam, double[] pt)
        {
            // Arrange
            Point3 closestPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            double param = _polyline.ClosestParameter(closestPt);

            // Assert
            param.Should().BeApproximately(expectedParam, GSharkMath.MaxTolerance);
        }

        [Theory]
        [InlineData(0.693375, 12.5)]
        [InlineData(0.931896, 16.8)]
        [InlineData(2.330214, 32.9)]
        [InlineData(3.415046, 46.7)]
        public void It_Returns_The_Length_At_The_Given_Parameter(double t, double expectedLength)
        {
            // Act
            double length = _polyline.LengthAt(t);

            // Assert
            length.Should().BeApproximately(expectedLength, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Point_At_The_Given_Length()
        {
            // Arrange
            Point3 expectedPt = new Point3(19.369239, 6.261522, 0);

            // Act
            Point3 pt = _polyline.PointAtNormalizedLength(0.5);
            
            // Assert
            (pt == expectedPt).Should().BeTrue();
        }

        [Theory]
        [InlineData(12.5, 0.693375)]
        [InlineData(16.8, 0.931896)]
        [InlineData(32.9, 2.330214)]
        [InlineData(46.7, 3.415046)]
        public void It_Returns_The_Parameter_At_The_Given_Length(double length, double parameterExpected)
        {
            // Act
            double parameter = _polyline.ParameterAtLength(length);

            // Assert
            parameter.Should().BeApproximately(parameterExpected, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_A_Transformed_Polyline()
        {
            // Arrange
            Transform translation = Transform.Translation(new Vector3(10, 15, 0));
            Transform rotation = Transform.Rotation(GSharkMath.ToRadians(30), new Point3(0, 0, 0));
            Transform combinedTransformations = translation.Combine(rotation);
            double[] distanceExpected = new[] { 19.831825, 20.496248, 24.803072, 28.67703, 35.897724 };

            // Act
            Polyline transformedPoly = _polyline.Transform(combinedTransformations);

            // Assert
            double[] lengths = _polyline.Select((pt, i) => pt.DistanceTo(transformedPoly[i])).ToArray();
            lengths.Select((val, i) => val.Should().BeApproximately(distanceExpected[i], GSharkMath.MaxTolerance));
        }

        [Fact]
        public void Returns_The_Offset_Of_A_Open_Polyline()
        {
            // Arrange
            Polyline pl = new Polyline(new PolylineTests().ExamplePts);
            double offset = 5;

            // Act
            Polyline offsetResult = pl.Offset(offset, Plane.PlaneXY);

            // Assert
            (offsetResult[0].DistanceTo(pl[0]) - offset).Should().BeLessThan(GSharkMath.MaxTolerance);
            (offsetResult[offsetResult.Count - 1].DistanceTo(pl[pl.Count - 1]) - offset).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            // Arrange
            Point3 testPt = new Point3(17.0, 8.0, 0.0);
            Point3 expectedPt = new Point3(18.2, 8.6, 0.0);

            // Act
            Point3 closestPt = _polyline.ClosestPoint(testPt);

            // Assert
            closestPt.EpsilonEquals(expectedPt, GSharkMath.Epsilon).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_The_Polyline()
        {
            // Arrange
            Point3 minExpected = new Point3(5.0, 0.0, 0.0);
            Point3 maxExpected = new Point3(45.0, 15.0, 0.0);

            // Act
            BoundingBox bBox = _polyline.GetBoundingBox();

            // Assert
            bBox.Min.EpsilonEquals(minExpected, GSharkMath.Epsilon).Should().BeTrue();
            bBox.Max.EpsilonEquals(maxExpected, GSharkMath.Epsilon).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Reversed_Polyline()
        {
            // Arrange
            List<Point3> reversedPts = new List<Point3>(ExamplePts);
            reversedPts.Reverse();

            // Act
            Polyline reversedPolyline = _polyline.Reverse();

            // Assert
            reversedPolyline.Should().NotBeSameAs(_polyline);
            reversedPolyline.Should().BeEquivalentTo(reversedPts);
        }

        [Fact]
        public void It_Returns_A_Polyline_Transformed_In_NurbsBase()
        {
            // Arrange
            Point3[] pts = new[]
            {
                new Point3(-1.673787, -0.235355, 14.436008),
                new Point3(13.145523, 6.066452, 0),
                new Point3(2.328185, 22.89864, 0),
                new Point3(18.154088, 30.745098, 7.561387),
                new Point3(18.154088, 12.309505, 7.561387)
            };
            Polyline poly = new Polyline(pts);

            // Act
            var polyNurbs = poly.ToNurbs();

            // Assert
            polyNurbs.Degree.Should().Be(1);
            polyNurbs.ControlPointLocations.Select((pt, i) => (pt == pts[i]).Should().BeTrue());
        }

        [Theory]
        [InlineData(0,0)]
        [InlineData(0.5, 0)]
        [InlineData(1, 0)]
        [InlineData(7, 1)]
        [InlineData(15, 1)]
        [InlineData(17.3, 2)]
        [InlineData(20, 2)]
        [InlineData(20.05, 3)]
        [InlineData(30, 3)]
        public void It_Returns_The_Segment_At_A_Given_Length(double length, int expectedIndex)
        {
            //Arrange
            var polyLine = new Polyline(new List<Point3>
            {
                new (0, 0, 0),
                new (5, 0, 0),
                new (5, 10, 0),
                new (0, 10, 0),
                new (0, 0, 0)
            });
            
            //Act
            var segment = polyLine.SegmentAtLength(length);

            //Assert
            polyLine.Segments.IndexOf(segment).Should().Be(expectedIndex);
        }
    }
}
