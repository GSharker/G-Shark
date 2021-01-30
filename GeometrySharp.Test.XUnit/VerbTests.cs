using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verb.core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit
{
    public class VerbTests
    {
        private readonly ITestOutputHelper _testOutput;

        public VerbTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void rationalBezierCurveArcLength()
        {
            var pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 0, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 0.5, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 2.5, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 3, 0, 0, 1 }));

            var knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0, 1.0 });
            var weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });

            var curve = verb.geom.NurbsCurve.byPoints(pts, 3);

            var res = verb.eval.Analyze.rationalBezierCurveArcLength(curve._data, 1, 16);

            _testOutput.WriteLine($"{res}");
        }

        [Fact]
        public void NurbsCurvebyPoints()
        {
            var pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 5, 5, 0 }));
            pts.push(new Array<double>(new double[] { 10, 10, 0 }));
            pts.push(new Array<double>(new double[] { 20, 15, 0 }));
            pts.push(new Array<double>(new double[] { 35, 15, 0 }));
            pts.push(new Array<double>(new double[] { 45, 10, 0 }));
            pts.push(new Array<double>(new double[] { 50, 5, 0 }));

            var knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 });
            var weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });

            var curve = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts, weights);
            var curve2 = verb.geom.NurbsCurve.byPoints(pts, 3);

            var ptAt = curve2.point(0.5);
            var vec = verb.core.Vec.normalized(curve2.tangent(0.5));
            var k = curve2.knots();

            for (int i = 0; i < k.length; i++)
            {
                k[i] = System.Math.Round(k[i], 6);
            }

            _testOutput.WriteLine($"{k}");
            _testOutput.WriteLine($"{ptAt[0]},{ptAt[1]},{ptAt[2]}");
        }

        [Fact]
        public void rationalCurveAdaptiveSample()
        {
            var pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 5, 5, 0 }));
            pts.push(new Array<double>(new double[] { 10, 10, 0 }));
            pts.push(new Array<double>(new double[] { 20, 15, 0 }));
            pts.push(new Array<double>(new double[] { 35, 15, 0 }));
            pts.push(new Array<double>(new double[] { 45, 10, 0 }));
            pts.push(new Array<double>(new double[] { 50, 5, 0 }));

            var knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 });
            var knots2 = new Array<double>(new double[] { 0.0, 0.2, 0.4, 0.6, 0.8, 1.0});
            var weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });

            var curve = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts, weights);
            var curve2 = verb.geom.NurbsCurve.byPoints(pts, 1);

            var p = verb.eval.Tess.rationalCurveAdaptiveSample(curve2._data, 0.1, null);

            _testOutput.WriteLine($"{p.length}");
            for (int i = 0; i < p.length; i++)
            {
                _testOutput.WriteLine($"{p[i]}");
            }
        }

        [Fact]
        public void rationalCurveRegularSample()
        {
            var pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 1, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 1, 1, 0, 1 }));
            pts.push(new Array<double>(new double[] { 0, 2, 0, 1 }));

            var knots = new Array<double>(new double[] { 0, 0, 0, 1, 1, 1 });

            var curve = new verb.core.NurbsCurveData(2, knots, pts);

            var p = verb.eval.Tess.rationalCurveRegularSample(curve, 10, true);

            _testOutput.WriteLine($"{p.length}");
            for (int i = 0; i < p.length; i++)
            {
                _testOutput.WriteLine($"{p[i]}");
            }
        }
    }
}
