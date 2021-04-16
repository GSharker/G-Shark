using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class PolylineTests
    {
        private readonly ITestOutputHelper _testOutput;
        public PolylineTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Vector3[] ExamplePts => new[]
        {
            new Vector3 {5, 0, 0}, new Vector3 {15, 15, 0},
            new Vector3 {20, 5, 0}, new Vector3 {30, 10, 0}, new Vector3 {45, 12.5, 0}
        };

        [Fact]
        public void It_Returns_A_Polyline()
        {
            // Arrange
            int numberOfExpectedSegments = 4;

            // Act
            Polyline polyline = new Polyline(ExamplePts);

            // Arrange
            polyline.SegmentsCount.Should().Be(numberOfExpectedSegments);
            polyline.Count.Should().Be(ExamplePts.Length);
            polyline[0].Should().BeEquivalentTo(ExamplePts[0]);
        }

        [Fact]
        public void Polyline_Throws_An_Exception_If_Vertex_Count_Is_Less_Than_Two()
        {
            // Arrange
            Vector3[] pts = new[] { new Vector3 { 5, 0, 0 } };

            // Act
            Func<Polyline> func = () => new Polyline(pts);

            // Assert
            func.Should().Throw<Exception>().WithMessage("Insufficient points for a polyline.");
        }

        [Fact]
        public void It_Returns_A_Polyline_Removing_Short_Segments()
        {
            // Arrange
            Vector3[] pts = new[]
            {
                new Vector3 {5, 5, 0}, new Vector3 {5, 10, 0},
                new Vector3 {5, 10, 0}, new Vector3 {15, 12, 0},
                new Vector3 {20, -20, 0}, new Vector3 {20, -20, 0}
            };

            Vector3[] ptsExpected = new[]
            {
                new Vector3 {5, 5, 0}, new Vector3 {5, 10, 0},
                new Vector3 {15, 12, 0}, new Vector3 {20, -20, 0}
            };

            // Act
            Polyline polyline = new Polyline(pts);

            // Assert
            polyline.Should().BeEquivalentTo(ptsExpected);
        }

        [Fact]
        public void It_Returns_The_Length_Of_A_Polyline()
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);
            double expectedLength = 55.595342;

            // Act
            double length = polyline.Length();

            // Assert
            length.Should().BeApproximately(expectedLength, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_A_Collection_Of_Lines()
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);
            int expectedNumberOfSegments = 4;
            double expectedSegmentLength = 11.18034;

            // Act
            Line[] segments = polyline.Segments();

            // Assert
            segments.Length.Should().Be(expectedNumberOfSegments);
            segments[1].Length.Should().Be(segments[2].Length)
                .And.BeApproximately(expectedSegmentLength, GeoSharpMath.MAXTOLERANCE);
        }

        [Theory]
        [InlineData(0.0, new double[] { 5, 0, 0 })]
        [InlineData(0.25, new double[] { 7.5, 3.75, 0 })]
        [InlineData(2.5, new double[] { 25, 7.5, 0 })]
        [InlineData(4.0, new double[] { 45, 12.5, 0 })]
        public void It_Returns_A_Point_At_The_Given_Parameter(double t, double[] pt)
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);
            Vector3 expectedPt = new Vector3(pt);

            // Act
            Vector3 ptResult = polyline.PointAt(t);

            // Assert
            ptResult.IsEqualRoundingDecimal(expectedPt, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.346154, new double[] { 5, 7.5, 0 })]
        [InlineData(2.0, new double[] { 15, -3, 5 })]
        [InlineData(2.48, new double[] { 22.5, 12, -3.0 })]
        public void It_Returns_A_Parameter_Along_The_Polyline_At_The_Given_Point(double expextedParam, double[] pt)
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);
            Vector3 closestPt = new Vector3(pt);

            // Act
            double param = polyline.ClosestParameter(closestPt);

            // Assert
            param.Should().BeApproximately(expextedParam, GeoSharpMath.MAXTOLERANCE);
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(4.05)]
        public void PointAt_Throws_An_Exception_If_Parameter_Is_Smaller_Than_Zero_And_Bigger_Than_One(double t)
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);

            // Act
            Func<Vector3> func = () => polyline.PointAt(t);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(0.0, new double[] { 0.5547, 0.83205, 0 })]
        [InlineData(0.25, new double[] { 0.5547, 0.83205, 0 })]
        [InlineData(2.5, new double[] { 0.894427, 0.447214, 0 })]
        [InlineData(4.0, new double[] { 0.986394, 0.164399, 0 })]
        public void It_Returns_A_Tangent_Vector_At_The_Given_Parameter(double t, double[] tangent)
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);
            Vector3 expectedTangent = new Vector3(tangent);

            // Act
            Vector3 tanResult = polyline.TangentAt(t);

            // Assert
            tanResult.IsEqualRoundingDecimal(expectedTangent, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 18.027756)]
        [InlineData(2, 11.18034)]
        public void It_Returns_A_Segment_At_The_Given_Index(int index, double segmentLength)
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);

            // Act
            Line segment = polyline.SegmentAt(index);

            // Assert
            segment.Length.Should().BeApproximately(segmentLength, GeoSharpMath.MAXTOLERANCE);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void SegmentAt_Throws_An_Exception_If_Index_Is_Smaller_Than_Zero_And_Bigger_Than_Polyline_Domain(int index)
        {
            // Arrange
            Polyline polyline = new Polyline(ExamplePts);

            // Act
            Func<Line> func = () => polyline.SegmentAt(index);

            // Assert
            func.Should().Throw<Exception>();
        }

        [Fact]
        public void It_Returns_A_Transformed_Polyline()
        {
            Transform translation = Transform.Translation(new Vector3 { 10, 15, 0 });
            Transform rotation = Transform.Rotation(GeoSharpMath.ToRadians(30), new Vector3 { 0, 0, 0 });
            Transform combinedTransformations = translation.Combine(rotation);
            double[] distanceToCheck = new[] { 19.831825, 20.496248, 24.803072, 28.67703, 35.897724 };
            Polyline polyline = new Polyline(ExamplePts);

            Polyline transformedPoly = polyline.Transform(combinedTransformations);

            double[] lengths = polyline.Select((pt, i) => pt.DistanceTo(transformedPoly[i])).ToArray();
            lengths.Select((val, i) => val.Should().BeApproximately(distanceToCheck[i], GeoSharpMath.MAXTOLERANCE));
        }

        [Fact]
        public void It_Returns_The_Closest_Point()
        {
            Polyline polyline = new Polyline(ExamplePts);
            Vector3 testPt = new Vector3 { 17.0, 8.0, 0.0 };
            Vector3 expectedPt = new Vector3 { 18.2, 8.6, 0.0 };

            Vector3 closestPt = polyline.ClosestPt(testPt);

            closestPt.IsEqualRoundingDecimal(expectedPt, 2).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Bounding_Box_Of_The_Polyline()
        {
            Polyline polyline = new Polyline(ExamplePts);
            Vector3 minExpected = new Vector3 { 5.0, 0.0, 0.0 };
            Vector3 maxExpected = new Vector3 { 45.0, 15.0, 0.0 };

            BoundingBox bBox = polyline.BoundingBox;

            bBox.Min.Should().BeEquivalentTo(minExpected);
            bBox.Max.Should().BeEquivalentTo(maxExpected);
        }

        [Fact]
        public void It_Returns_A_Reversed_Polyline()
        {
            Polyline polyline = new Polyline(ExamplePts);
            List<Vector3> reversedPts = new List<Vector3>(ExamplePts);
            reversedPts.Reverse();

            Polyline reversedPolyline = polyline.Reverse();

            reversedPolyline.Should().NotBeSameAs(polyline);
            reversedPolyline.Should().BeEquivalentTo(reversedPts);
        }

        [Fact]
        public void It_Returns_A_Polyline_Transformed_In_NurbsCurve()
        {
            Vector3[] pts = new[]
            {
                new Vector3 {-1.673787, -0.235355, 14.436008}, new Vector3 {13.145523, 6.066452, 0},
                new Vector3 {2.328185, 22.89864, 0}, new Vector3 {18.154088, 30.745098, 7.561387},
                new Vector3 {18.154088, 12.309505, 7.561387}
            };

            ICurve poly = new Polyline(pts);

            Knot knots = poly.Knots;

            poly.Degree.Should().Be(1);
            for (int i = 1; i < poly.Knots.Count - 1; i++)
            {
                Vector3 pt = poly.PointAt(knots[i]);
                pts[i - 1].Equals(poly.PointAt(knots[i])).Should().BeTrue();
            }
        }
    }
}
