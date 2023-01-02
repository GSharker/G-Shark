using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class TransformTests
    {
        private readonly ITestOutputHelper _testOutput;
        private readonly Arc _arc;
        private readonly Line _line;
        private readonly NurbsCurve _curve;
        private readonly Circle _circle;
        private readonly Point3 _pt1;
        private readonly Point3 _pt2;
        private readonly Point3 _pt3;

        public TransformTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            #region example
            _pt1 = new Point3(74.264416, 36.39316, -1.884313);
            _pt2 = new Point3(97.679126, 13.940616, 3.812853);
            _pt3 = new Point3(100.92443, 30.599893, -0.585116);

            // Initializes an arc by plane, radius and angle.
            _arc = new Arc(_pt1, _pt2, _pt3);

            // Initializes a circle by 3 points.
            _circle = new Circle(_pt1, _pt2, _pt3);

            // Initializes a line 
            _line = new Line(_pt1, _pt2);

            // Initializes a curve
            _curve = new NurbsCurve(new List<Point3>() { _pt1, _pt2, _pt3 }, 2);
            #endregion
        }

        [Fact]
        public void It_Returns_A_List_Of_Transformed_NurbsBase()
        {
            // Arrange
            List<NurbsBase> geometries = new List<NurbsBase>() { _arc, _line, _curve, _circle};
            var transform = Transform.Translation(new Vector3(1, 0, 0));

            var expectedArc = new Arc(_pt1.Transform(transform), _pt2.Transform(transform), _pt3.Transform(transform));
            var expectedCircle = new Circle(_pt1.Transform(transform), _pt2.Transform(transform), _pt3.Transform(transform));
            var expectedLine = new Line(_pt1.Transform(transform), _pt2.Transform(transform));
            var expectedCurve = new NurbsCurve(new List<Point3>() { _pt1.Transform(transform), _pt2.Transform(transform), _pt3.Transform(transform) }, 2);
            List<NurbsBase> expectedGeometries = new List<NurbsBase>() { expectedArc, expectedLine, expectedCurve, expectedCircle };

            // Act
            var result = geometries.Select(geometry=> geometry.Transform(transform)).ToList();

            // Assert
            for (int i = 0; i < 4; i++)
            {
                result[i].StartPoint.EpsilonEquals(expectedGeometries[i].StartPoint, GSharkMath.MaxTolerance).Should().BeTrue();
                result[i].EndPoint.EpsilonEquals(expectedGeometries[i].EndPoint, GSharkMath.MaxTolerance).Should().BeTrue();
            }
        }
    }
}
