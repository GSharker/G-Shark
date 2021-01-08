using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Evaluation;
using VerbNurbsSharp.ExtendedMethods;
using VerbNurbsSharp.Geometry;
using VerbNurbsSharp.XUnit.Core;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Geometry
{
    [Trait("Category", "NurbsCurve")]
    public class NurbsCurveTest
    {
        private readonly ITestOutputHelper _testOutput;

        public NurbsCurveTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static NurbsCurve NurbsCurveExample()
        {
            int degree = 2;
            List<Vector> pts = new List<Vector>()
            {
                new Vector(){-10,15,5},
                new Vector(){10,5,5},
                new Vector(){20,0,0}
            };
            KnotArray knots = new KnotArray() { 1, 1, 1, 1, 1, 1 };

            return new NurbsCurve(degree, knots, pts);
        }

        public static NurbsCurve NurbsCurveHomogenizedPtsExample()
        {
            int degree = 2;
            List<Vector> pts = new List<Vector>()
            {
                new Vector(){-10,15,5},
                new Vector(){10,5,5},
                new Vector(){20,0,0}
            };
            KnotArray knots = new KnotArray() { 1, 1, 1, 1, 1, 1 };
            var weights = new List<double>() { 0.5, 0.5, 0.5 };

            return new NurbsCurve(degree, knots, pts, weights);
        }

        [Fact]
        public void Get_A_NurbsCurve()
        {
            int degree = 2;
            List<Vector> pts = new List<Vector>()
            {
                new Vector(){-10,15,5},
                new Vector(){10,5,5},
                new Vector(){20,0,0}
            };
            KnotArray knots = new KnotArray() {1, 1, 1};

            var nurbsCurve = new NurbsCurve(degree, knots, pts);

            nurbsCurve.Should().NotBeNull();
        }

        [Fact]
        public void Get_A_NurbsCurve_EvaluatedWithAListOfWeights()
        {
            int degree = 2;
            List<Vector> pts = new List<Vector>()
            {
                new Vector(){-10,15,5},
                new Vector(){10,5,5},
                new Vector(){20,0,0}
            };
            KnotArray knots = new KnotArray() { 1, 1, 1 };
            var weights = new List<double>() {0.5, 0.5, 0.5};

            var nurbsCurve = new NurbsCurve(degree, knots, pts, weights);

            nurbsCurve.Should().NotBeNull();
            nurbsCurve.ControlPoints[2].Should().BeEquivalentTo(new Vector() {10, 0, 0, 0.5});
        }

        [Fact]
        public void Get_ACopied_NurbsCurve()
        {
            int degree = 2;
            List<Vector> pts = new List<Vector>()
            {
                new Vector(){-10,15,5},
                new Vector(){10,5,5},
                new Vector(){20,0,0}
            };
            KnotArray knots = new KnotArray() { 1, 1, 1, 1, 1, 1};

            var nurbsCurve = new NurbsCurve(degree, knots, pts);
            var copiedNurbs = nurbsCurve.Clone();

            copiedNurbs.Should().NotBeNull();
            copiedNurbs.Should().BeEquivalentTo(nurbsCurve);
        }

        [Fact]
        public void GetTheDomainOfTheCurve()
        {
            var curveDomain = NurbsCurveExample().Domain();

            curveDomain.Min.Should().Be(NurbsCurveExample().Knots.First());
            curveDomain.Max.Should().Be(NurbsCurveExample().Knots.Last());
        }

        [Fact]
        public void AreTheControlPoints_Homogenized()
        {
            NurbsCurveExample().AreControlPointsHomogenized().Should().BeFalse();
            NurbsCurveHomogenizedPtsExample().AreControlPointsHomogenized().Should().BeTrue();
        }

        [Fact]
        public void TransformNurbsCurve_ByAGivenMatrix()
        {
            var curve = NurbsCurveTest.NurbsCurveExample();
            var matrix = MatrixTest.TransformationMatrixExample;

            var transformedCurve = curve.Transform(matrix);
            var demoPts = Eval.Dehomogenize1d(transformedCurve.ControlPoints);

            var distanceBetweenPts =
                Math.Round(Vector.Length(Constants.Subtraction(demoPts[0], curve.ControlPoints[0])
                    .ToVector()),6);

            distanceBetweenPts.Should().Be(22.383029);
        }

        [Fact]
        public void Split_ReturnTwoNurbsCurve()
        {
            //int degree = 2;
            //List<Vector> pts = new List<Vector>()
            //{
            //    new Vector(){2.0,2.0,0.0},
            //    new Vector(){10.0,12.0,5.0},
            //    new Vector(){15.0,2.0,0.0}
            //};
            //KnotArray knots = new KnotArray() { 0, 0, 1, 1};
            //var weights = new List<double>() { 1, 1, 1 };

            var degree = 3;
            var knots = new KnotArray() { 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 };
            var controlPts = new List<Vector>();
            for (int i = 0; i <= knots.Count - 3 - 2; i++)
            {
                controlPts.Add(new Vector() { i, 0.0, 0.0 });
            }
            var weights = Sets.RepeatData(1.0, controlPts.Count);

            var curve = new NurbsCurve(degree, knots, controlPts, weights);

            var splitCurves = curve.Split(0.5);

            splitCurves.Should().HaveCount(2);
        }
    }
}
