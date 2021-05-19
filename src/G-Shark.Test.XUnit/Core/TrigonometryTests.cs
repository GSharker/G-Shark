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
            Vector3 pt1 = new Vector3 { 0.0, 0.0, 0.0 };
            Vector3 pt2 = new Vector3 { 10.0, 0.0, 0.0 };
            Vector3 pt3 = new Vector3 { 5.0, 5.0, 0.0 };
            Vector3 pt4 = new Vector3 { -5.0, -15.0, 0.0 };
            List<Vector3> points = new List<Vector3>{pt1,pt2,pt3,pt4};

            // Arrange
            Trigonometry.ArePointsCoplanar(points).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_True_If_Three_Points_Are_Collinear()
        {
            // Arrange
            Vector3 pt1 = new Vector3 { 25.923, 27.057, 0.0 };
            Vector3 pt2 = new Vector3 { 35.964, 31.367, 0.0 };
            Vector3 pt3 = new Vector3 { 51.299, 37.950, 0.0 };

            // Assert
            Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, GeoSharpMath.MINTOLERANCE).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_False_If_Three_Points_Are_Not_Collinear()
        {
            // Arrange
            Vector3 pt1 = new Vector3 { 25.923, 27.057, 0.0 };
            Vector3 pt2 = new Vector3 { 35.964, 20.451, 0.0 };
            Vector3 pt3 = new Vector3 { 51.299, 37.950, 0.0 };

            // Assert
            Trigonometry.AreThreePointsCollinear(pt1, pt2, pt3, GeoSharpMath.MINTOLERANCE).Should().BeFalse();
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
            Vector3 pt0 = new Vector3 { 5, 5, 0 };
            Vector3 pt1 = new Vector3 { 10, 10, 0 };

            // Act
            (double tValue, Vector3 pt) closestPt = Trigonometry.ClosestPointToSegment(ptToCheck.ToVector(), pt0, pt1, 0, 1);

            // Assert
            closestPt.tValue.Should().BeApproximately(tValExpected, GeoSharpMath.MAXTOLERANCE);
            closestPt.pt.Should().BeEquivalentTo(ptExpected.ToVector());
        }
    }
}
