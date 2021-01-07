using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;
using Xunit;

namespace VerbNurbsSharp.XUnit.Geometry
{
    [Trait("Category", "NurbsCurve")]
    public class NurbsCurveTest
    {
        private static NurbsCurve NurbsCurveExample()
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

        private static NurbsCurve NurbsCurveHomogenizedPtsExample()
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

        // ToDo be implemented.
        [Fact]
        public void TransformNurbsCurve_ByAGivenMatrix()
        {

        }
    }
}
