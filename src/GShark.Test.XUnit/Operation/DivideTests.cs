using FluentAssertions;
using GShark.Core;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Operation
{
    public class DivideTests
    {
        private readonly ITestOutputHelper _testOutput;

        public DivideTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(0.25)]
        [InlineData(0.5)]
        [InlineData(0.75)]
        [InlineData(1.0)]
        public void It_Returns_Two_Curves_Splitting_One_Curve(double parameter)
        {
            // Arrange
            int degree = 3;
            List<Point3> pts = new List<Point3>
            {
                new Point3(2,2,0),
                new Point3(4,12,0),
                new Point3(7,12,0),
                new Point3(15,2,0)
            };
            NurbsCurve curve = new NurbsCurve(pts, degree);

            // Act
            List<NurbsCurve> curves = curve.SplitAt(parameter);

            // Assert
            curves.Should().HaveCount(2);

            for (int i = 0; i < degree + 1; i++)
            {
                int d = curves[0].Knots.Count - (degree + 1);
                curves[0].Knots[d + i].Should().BeApproximately(parameter, GSharkMath.MaxTolerance);
            }

            for (int i = 0; i < degree + 1; i++)
            {
                int d = 0;
                curves[1].Knots[d + i].Should().BeApproximately(parameter, GSharkMath.MaxTolerance);
            }
        }

        [Fact]
        public void It_Returns_A_SubCurve_Defined_By_Domain()
        {
            // Arrange
            Interval domain = new Interval(0.65, 0.85);
            int degree = 3;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(2,2,0),
                new Point3(4,12,0),
                new Point3(7,12,0),
                new Point3(15,2,5)
            };
            var expectedKnotVector = new KnotVector(new List<double> {0.65, 0.65, 0.65, 0.65, 0.85, 0.85, 0.85, 0.85});
            NurbsCurve curve = new NurbsCurve(controlPts, degree);
            NurbsCurve expectedSubCurve = new NurbsCurve(
                new List<Point3>
                {
                    new (8.266,8.825,1.373125),
                    new (9.264,8.225,1.795625),
                    new (10.406,7.225,2.348125),
                    new (11.724,5.825,3.070625)
                },
                degree:3);

            // Act
            NurbsCurve subCurve = curve.SubCurve(domain);

            // Assert
            subCurve.ControlPointLocations.SequenceEqual(expectedSubCurve.ControlPointLocations).Should().BeTrue();
            subCurve.ControlPoints.SequenceEqual(expectedSubCurve.ControlPoints).Should().BeTrue();
            subCurve.Knots.SequenceEqual(expectedKnotVector).Should().BeTrue();
        }

        [Fact]
        public void It_Splits_A_Curve_Into_Segments_At_Given_Parameters()
        {
            // Arrange
            var parameters = new[]{0.25, 0.5, 0.75};
            int degree = 3;
            List<Point3> controlPts = new List<Point3>
            {
                new Point3(2,2,0),
                new Point3(4,12,0),
                new Point3(7,12,0),
                new Point3(15,2,5)
            };
            KnotVector knots = new KnotVector(degree, controlPts.Count);
            NurbsCurve curve = new NurbsCurve(controlPts, degree);

            // Act
            var segments = curve.SplitAt(parameters.ToArray());

            // Assert
            segments.Count.Should().Be(4);
        }

        [Fact]
        public void Divide_By_Number_Of_Segments_Returns_Points_And_Parameters_Along_Curve()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            double[] tValuesExpected = {
                    0,
                    0.12294074023135007,
                    0.26515583503755935,
                    0.4202931617987752,
                    0.5797068382012247,
                    0.7348441649624406,
                    0.87705925976865,
                    1
                };

            List<Point3> pointsExpected = tValuesExpected.Select(t => curve.PointAt(t)).ToList();
            int segments = 7;

            // Act
            (List<Point3> Points, List<double> Parameters) divideResult = curve.Divide(segments);

            // Assert
            divideResult.Parameters.Count.Should().Be(tValuesExpected.Length).And.Be(segments + 1);
            for (int i = 0; i < tValuesExpected.Length; i++)
            {
                divideResult.Parameters[i].Should().BeApproximately(tValuesExpected[i], GSharkMath.MaxTolerance);
                divideResult.Points[i].EpsilonEquals(pointsExpected[i], GSharkMath.MaxTolerance).Should().BeTrue();
            }
        }

        [Fact]
        public void Divide_By_Length_Returns_Points_And_Parameters_Along_Curve()
        {
            // Arrange
            NurbsCurve curve = NurbsCurveCollection.NurbsCurvePlanarExample();
            double[] tValuesExpected = {
                0,
                0.12294074023135007,
                0.26515583503755935,
                0.4202931617987752,
                0.5797068382012247,
                0.7348441649624406,
                0.87705925976865,
                1
            };
            List<Point3> pointsExpected = tValuesExpected.Select(t => curve.PointAt(t)).ToList();
            int steps = 7;
            double length = curve.Length / steps;

            // Act
            (List<Point3> Points, List<double> Parameters) divideResult = curve.Divide(length);

            // Assert
            divideResult.Parameters.Count.Should().Be(pointsExpected.Count).And.Be(steps + 1);
            for (int i = 0; i < pointsExpected.Count; i++)
            {
                divideResult.Parameters[i].Should().BeApproximately(tValuesExpected[i], GSharkMath.MaxTolerance);
                divideResult.Points[i].EpsilonEquals(pointsExpected[i], GSharkMath.MaxTolerance).Should().BeTrue();
            }
        }


        [Fact]
        public void It_Calculates_Rotation_Minimized_Frames_Along_Curve()
        {
            //Arrange
            List<Point3> curvePoints = new List<Point3>()
            {
                new Point3(2.97886194909822,-1.92270769398443,2.47457404024129),
                new Point3(3.32385541965108,1.80584085689923,6.0601278626774),
                new Point3(0.0633036527226603,-0.407036899902438,-1.31591991952815),
                new Point3(5.95969448693081,4.64784644734791,1.40007910718518),
                new Point3(-0.354248010530259,7.14708134218695,4.20008044306221)
            };
            int curveDegree = 3;

            NurbsCurve curve = new NurbsCurve(curvePoints, curveDegree);
            List<Plane> expectedPerpFrames = new List<Plane>()
            {
                new Plane
                (
                    new Point3(2.97886194909822,-1.92270769398443,2.47457404024129),
                    new Vector3(0,-0.693149997654275,0.720793368970524),
                    new Vector3(0.997783397299274,-0.0479654928730318,-0.0461259532949279)
                ),

                new Plane
                (
                    new Point3(3.02489432434604,-0.770003248884809,3.47387763966775),
                    new Vector3(0.00276695744436017,-0.595491044201997,0.803357180973517),
                    new Vector3(0.999768784387978,0.0187784142185988,0.0104761120087221)
                ),

                new Plane
                (
                    new Point3(2.76029490981731,0.579547802552417,3.95798740300145),
                    new Vector3(-0.254143225840459,0.296761353381737,0.920512856997905),
                    new Vector3(0.870004800114335,0.485913203321248,0.0835464338921744)
                ),

                new Plane
                (
                    new Point3(2.26150940752748,1.08203309965507,2.66730467530961),
                    new Vector3(-0.621887630449795,0.755614910419348,0.205674213856528),
                    new Vector3(0.762450619071222,0.64415242860156,-0.0611285710991555)
                ),

                new Plane
                (
                    new Point3(2.35006257723804,1.40722983093391,1.21150170219858),
                    new Vector3(-0.632783991131049,0.766599007595894,0.109134697146467),
                    new Vector3(0.643301474963722,0.442014097591834,0.625129386478972)
                ),

                new Plane
                (
                    new Point3(3.25050248842797,2.48795939512857,0.810277036129874),
                    new Vector3(-0.852876708246807,0.492418314588269,0.173567059052516),
                    new Vector3(0.0832978556936132,-0.199844963834041,0.976280419586)
                ),

                new Plane
                (
                    new Point3(3.60609059610478,3.83750463220352,1.34371920699578),
                    new Vector3(-0.996340802086922,-0.068690803002822,-0.0508584278130699),
                    new Vector3(-0.00960803991509081,-0.501260851912808,0.865242881455049)
                ),

                new Plane
                (
                    new Point3(3.08741580539977,5.00612710169302,2.15068883565368),
                    new Vector3(-0.844433205709907,-0.441089898316079,-0.303928055134766),
                    new Vector3(0.0302652787133271,-0.605770335123178,0.795063716936673)
                ),

                new Plane
                (
                    new Point3(2.10568615895179,5.88882222994236,2.9114489256168),
                    new Vector3(-0.687425157390972,-0.586557221682423,-0.428249084853889),
                    new Vector3(0.0590711058571009,-0.632871482530973,0.772000188505072)
                ),

                new Plane
                (
                    new Point3(0.925460884650867,6.57980489941838,3.58944298733261),
                    new Vector3(-0.579677821471797,-0.650509210518833,-0.490725371591767),
                    new Vector3(0.0736919779897127,-0.641614523639824,0.763479073344127)
                ),

                new Plane
                (
                    new Point3(-0.354248010530259,7.14708134218695,4.20008044306221),
                    new Vector3(-0.50441907117695,-0.684124711293709,-0.526815698352169),
                    new Vector3(0.0815380701767222,-0.645136167147176,0.759704461583915)
                )

            };

            //Act
            List<double> uValues = curve.Divide(10).Parameters;

            foreach (double uValue in uValues)
            {
                _testOutput.WriteLine(uValue.ToString());
            }

            List<Plane> perpFrames = curve.PerpendicularFrames(uValues);

            foreach (Plane perpFrame in perpFrames)
            {
                _testOutput.WriteLine(perpFrame.ToString());
            }

            //Assert
            for (int i = 0; i < perpFrames.Count; i++)
            {
                Plane perpFrame = perpFrames[i];
                Plane expectedPerpFrame = expectedPerpFrames[i];
                perpFrame.Origin.EpsilonEquals(expectedPerpFrame.Origin, GSharkMath.MaxTolerance);
                perpFrame.XAxis.EpsilonEquals(expectedPerpFrame.XAxis, GSharkMath.MaxTolerance);
                perpFrame.YAxis.EpsilonEquals(expectedPerpFrame.YAxis, GSharkMath.MaxTolerance);
                perpFrame.ZAxis.EpsilonEquals(expectedPerpFrame.ZAxis, GSharkMath.MaxTolerance);
            }
        }
    }
}
