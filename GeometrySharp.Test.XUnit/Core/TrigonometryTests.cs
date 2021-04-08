using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using System.Collections.Generic;
using Xunit;

namespace GeometrySharp.Test.XUnit.Core
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
    }
}
