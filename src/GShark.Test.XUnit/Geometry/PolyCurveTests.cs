using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using GShark.Test.XUnit.Geometry;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace GShark.Test.XUnit.Geometry
{
    public class PolyCurveTests
    {
        private readonly PolyCurve _polycurve;

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

            #region example
            // Initializes a polycurve from a curve a line and an arc.
            NurbsCurve curve = new NurbsCurve(pts, degree);
            Line line = new Line(new Point3(5, 5, 0), new Point3(5, 5, -2.5));
            Arc arc = Arc.ByStartEndDirection(new Point3(5, 5, -2.5), new Point3(10, 5, -5), new Vector3(0, 0, -1));

            _polycurve = new PolyCurve();
            _polycurve.Append(curve);
            _polycurve.Append(line);
            _polycurve.Append(arc);
            #endregion
        }

        [Fact]
        public void It_Returns_The_Length_Of_The_PolyCurve()
        {
            // Arrange
            double expectedLength = 29.689504;

            // Act            
            var length = _polycurve.Length;

            // Arrange
            length.Should().BeApproximately(expectedLength, GSharkMath.MinTolerance);
        }

        [Theory]
        [InlineData(new double[] { 5, 3.042501, 4.519036 }, 15)]
        [InlineData(new double[] { 5, 5, -1.730175 }, 22)]
        [InlineData(new double[] { 6.118663, 5, -4.895879 }, 25.5)]
        public void It_Returns_A_Point_At_Length(double[] coords, double length)
        {
            // Arrange
            Point3 expectedPoint = new Point3(coords[0], coords[1], coords[2]);

            //Act
            Point3 pt = _polycurve.PointAtLength(length);

            // Assert
            (pt == expectedPoint).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.265154444812697, 15)]
        [InlineData(0.564023377863855, 22)]
        [InlineData(0.797352256245054, 25.5)]
        public void It_Returns_The_Length_At_Parameter(double t, double expectedLength)
        {
            //Act
            double length = _polycurve.LengthAt(t);

            // Assert
            length.Should().BeApproximately(expectedLength, GSharkMath.MinTolerance);
        }

        [Theory]
        [InlineData("../../../Resources/C3D_BORR_20-440-10_SW-1_2021_09_08.json")]
        public void PyRevit_Tests(object value)
        {
            string jsonStr = File.ReadAllText((string)value);
            CivilData cd = JsonConvert.DeserializeObject<CivilData>(jsonStr);

            List<AlignmentICurve> ents = cd.Entities;
            //sort entities
            PolyCurve polyCurve = new PolyCurve();
            foreach (AlignmentICurve ent in ents.OrderBy(x=>x.StartStation).ToList())
            {
                switch(ent.CurveType)
                {
                    case "Line":
                        Line ln = new Line(ent.StartPoint, ent.EndPoint);
                        polyCurve.Append(ln);
                        break;
                    case "Arc":
                        Arc arc = new Arc(ent.StartPoint, ent.MidPoint, ent.EndPoint);
                        polyCurve.Append(arc);
                        break;
                    case "NurbCurve":
                        break;
                }
            }

            double ch = 33;
            double para = polyCurve.ParameterAtLength(ch);

            _testOutput.WriteLine(polyCurve.PerpendicularFrameAt(para).ToString());
        }
    }
}
