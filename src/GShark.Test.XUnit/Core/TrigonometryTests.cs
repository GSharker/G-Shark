using FluentAssertions;
using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using System.Collections.Generic;
using Xunit;

namespace GShark.Test.XUnit.Core
{
    public class TrigonometryTests
    {
        [Fact]
        public void It_Returns_True_If_Points_Are_Planar()
        {
            // Arrange
            Point3d pt1 = new Point3d( 0.0, 0.0, 0.0);
            Point3d pt2 = new Point3d( 10.0, 0.0, 0.0);
            Point3d pt3 = new Point3d( 5.0, 5.0, 0.0);
            Point3d pt4 = new Point3d( -5.0, -15.0, 0.0);
            List<Point3d> points = new List<Point3d>{pt1,pt2,pt3,pt4};

            // Arrange
            Trigonometry.ArePointsCoplanar(points).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_True_If_Three_Points_Are_Collinear()
        {
            // Arrange
            Point3d pt1 = new Point3d( 25.923, 27.057, 0.0);
            Point3d pt2 = new Point3d( 35.964, 31.367, 0.0);
            Point3d pt3 = new Point3d( 51.299, 37.950, 0.0);

            // Assert
            Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_False_If_Three_Points_Are_Not_Collinear()
        {
            // Arrange
            Point3d pt1 = new Point3d( 25.923, 27.057, 0.0);
            Point3d pt2 = new Point3d( 35.964, 20.451, 0.0);
            Point3d pt3 = new Point3d( 51.299, 37.950, 0.0);

            // Assert
            Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, GeoSharkMath.MinTolerance).Should().BeFalse();
        }

        [Theory]
        [InlineData(new double[] { 5, 7, 0 }, new double[] { 6, 6, 0 }, 0.2)]
        [InlineData(new double[] { 7, 6, 0 }, new double[] { 6.5, 6.5, 0 }, 0.3)]
        [InlineData(new double[] { 5, 9, 0 }, new double[] { 7, 7, 0 }, 0.4)]
        public void It_Returns_The_Closest_Point_On_A_Segment(double[] ptToCheck, double[] ptExpected, double tValExpected)
        {
            // Arrange
            // We are not passing a segment like a line or ray but the part that compose the segment,
            // t values [0 and 1] and start and end point.
            var testPt = new Point3d(ptToCheck[0], ptToCheck[1], ptToCheck[2]);
            var expectedPt = new Point3d(ptExpected[0], ptExpected[1], ptExpected[2]);
            Point3d pt0 = new Point3d( 5, 5, 0);
            Point3d pt1 = new Point3d( 10, 10, 0);

            // Act
            (double tValue, Point3d pt) closestPt = Trigonometry.ClosestPointToSegment(testPt, pt0, pt1, 0, 1);

            // Assert
            closestPt.tValue.Should().BeApproximately(tValExpected, GeoSharkMath.MaxTolerance);
            closestPt.pt.EpsilonEquals(expectedPt, GeoSharkMath.Epsilon).Should().BeTrue();
        }
    }
}
