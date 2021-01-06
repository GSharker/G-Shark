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
        public void Get_A_NurbsCurve_FromANurbs()
        {
            int degree = 2;
            List<Vector> pts = new List<Vector>()
            {
                new Vector(){-10,15,5},
                new Vector(){10,5,5},
                new Vector(){20,0,0}
            };
            KnotArray knots = new KnotArray() { 1, 1, 1 };

            var nurbsCurve = new NurbsCurve(degree, knots, pts);
            var copiedNurbs = new NurbsCurve(nurbsCurve);

            copiedNurbs.Should().NotBeNull();
            copiedNurbs.Should().BeEquivalentTo(nurbsCurve);
        }
    }
}
