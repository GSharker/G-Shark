using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Geometry
{
    public class ArcTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ArcTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static Arc ExampleArc3D
        {
            get
            {
                Vector3 pt1 = new Vector3 { 74.264416, 36.39316, -1.884313 };
                Vector3 pt2 = new Vector3 { 97.679126, 13.940616, 3.812853 };
                Vector3 pt3 = new Vector3 { 100.92443, 30.599893, -0.585116 };

                return new Arc(pt1, pt2, pt3);
            }
        }

            [Fact]
        public void Initializes_An_Arc()
        {
            double angle = GeoSharpMath.ToRadians(40);
            Arc arc = new Arc(Plane.PlaneXY, 15, angle);

            arc.Should().NotBeNull();
            arc.Length.Should().BeApproximately(10.471976, GeoSharpMath.MAXTOLERANCE);
            arc.Center.Should().BeEquivalentTo(Plane.PlaneXY.Origin);
            arc.Radius.Should().Be(15);
            arc.Angle.Should().BeApproximately(0.698132, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void Initializes_An_Arc_By_Three_Points()
        {
            Arc arc = ExampleArc3D;

            arc.Length.Should().BeApproximately(71.333203, GeoSharpMath.MAXTOLERANCE);
            arc.Radius.Should().BeApproximately(16.47719, GeoSharpMath.MAXTOLERANCE);
            GeoSharpMath.ToDegrees(arc.Angle).Should().BeApproximately(248.045414, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_The_BoundingBox_Of_The_Arc()
        {
            double angle = GeoSharpMath.ToRadians(40);
            Arc arc = new Arc(Plane.PlaneXY, 15, angle);

            BoundingBox bBox = arc.BoundingBox;

            bBox.Min.IsEqualRoundingDecimal(new Vector3 {11.490667, 0, 0}, 6).Should().BeTrue();
            bBox.Max.IsEqualRoundingDecimal(new Vector3 { 15, 9.641814, 0 }, 6).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Point_On_The_Arc_At_The_Given_Parameter()
        {
            Vector3 expectedPt = new Vector3 {69.81863, 29.435661, -0.021966};

            Arc arc = ExampleArc3D;
            Vector3 pt = arc.PointAt(0.12);

            pt.IsEqualRoundingDecimal(expectedPt, 6).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[]{ 82.248292, 15.836914, 3.443127 }, new double[] { 80.001066, 9.815219, 5.041724 })]
        [InlineData(new double[] { 85.591741, 24.79606, 1.064717 }, new double[] { 74.264416, 36.39316, -1.884313 })]
        public void It_Returns_The_Closest_Point_On_An_Arc(double[] ptToTest, double[] result)
        {
            Vector3 testPt = new Vector3(ptToTest);
            Vector3 expectedPt = new Vector3(result);

            Arc arc = ExampleArc3D;
            Vector3 pt = arc.ClosestPt(testPt);

            _testOutput.WriteLine(pt.ToString());
            pt.IsEqualRoundingDecimal(expectedPt, 5).Should().BeTrue();
        }
    }
}

