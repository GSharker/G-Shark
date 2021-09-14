using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using GShark.Intersection;

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
			foreach (AlignmentICurve ent in ents.OrderBy(x => x.StartStation).ToList())
			{
				switch (ent.CurveType)
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

        [Theory]
        [InlineData("../../../Resources/C3D_BORR_20-440-10_SW-1_2021_09_08.json")]
        public void PolyCurveIntersect_Test(object value)
        {
            string jsonStr = File.ReadAllText((string)value);
            CivilData cd = JsonConvert.DeserializeObject<CivilData>(jsonStr);

            List<AlignmentICurve> ents = cd.Entities;
            //sort entities
            PolyCurve cv = new PolyCurve();
            foreach (AlignmentICurve ent in ents.OrderBy(x => x.StartStation).ToList())
            {
                switch (ent.CurveType)
                {
                    case "Line":
                        Line ln = new Line(ent.StartPoint, ent.EndPoint);
                        cv.Append(ln);
                        break;
                    case "Arc":
                        Arc arc = new Arc(ent.StartPoint, ent.MidPoint, ent.EndPoint);
                        cv.Append(arc);
                        break;
                    case "NurbCurve":
                        break;
                }
            }

            double startCh = 22;

            double startPara = cv.ParameterAtLength(startCh);
            Point3 startPoint = cv.PointAt(startPara);
            _testOutput.WriteLine($"Start Point: {startPoint}");

            Plane ppln = cv.PerpendicularFrameAt(startPara);
            Vector3 yaxis = ppln.YAxis;
            _testOutput.WriteLine($"YAxis: {yaxis}");

            Plane pln = new Plane(startPoint, yaxis);
            Circle c = new Circle(pln, 2);
            _testOutput.WriteLine($"Circle Center point: {c.Center}");

            List<CurvesIntersectionResult> results = Intersect.CurveCurve(cv, c, 1e-3);
            _testOutput.WriteLine(results.ToString());
            foreach (CurvesIntersectionResult r in results)
            {
                _testOutput.WriteLine(r.PointA.ToString());
                _testOutput.WriteLine(r.PointB.ToString());
            }

            _testOutput.WriteLine(cv.PerpendicularFrameAt(startPara).ToString());
        }
    }
}
