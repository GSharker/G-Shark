using verb.core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit
{
    /// <summary>
    /// Test used to review methods and functionality used in verb.
    /// Once the library will be completed this test will be removed.
    /// </summary>
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
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 0, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 0.5, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 2.5, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 3, 0, 0, 1 }));

            Array<double> knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 1.0, 2.0, 2.0, 2.0 });
            Array<double> weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byPoints(pts, 3);

            double res = verb.eval.Analyze.rationalBezierCurveArcLength(curve._data, 1, 16);

            _testOutput.WriteLine($"{res}");
        }

        [Fact]
        public void NurbsCurvebyPoints()
        {
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 5, 5, 0 }));
            pts.push(new Array<double>(new double[] { 10, 10, 0 }));
            pts.push(new Array<double>(new double[] { 20, 15, 0 }));
            pts.push(new Array<double>(new double[] { 35, 15, 0 }));
            pts.push(new Array<double>(new double[] { 45, 10, 0 }));
            pts.push(new Array<double>(new double[] { 50, 5, 0 }));

            Array<double> knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 });
            Array<double> weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts, weights);
            verb.geom.NurbsCurve curve2 = verb.geom.NurbsCurve.byPoints(pts, 3);

            Array<double> ptAt = curve2.point(0.5);
            Array<double> vec = verb.core.Vec.normalized(curve2.tangent(0.5));
            Array<double> k = curve2.knots();

            for (int i = 0; i < k.length; i++)
            {
                k[i] = System.Math.Round(k[i], 6);
            }

            _testOutput.WriteLine($"{k}");
            _testOutput.WriteLine($"{ptAt[0]},{ptAt[1]},{ptAt[2]}");

            LazyCurveBoundingBoxTree lazy = new verb.core.LazyCurveBoundingBoxTree(curve._data, -10);

            _testOutput.WriteLine($"{lazy._knotTol}");
            _testOutput.WriteLine($"{curve._data.knots}");
        }

        [Fact]
        public void CurvesIntersections()
        {
            Array<object> pts0 = new Array<object>();
            Array<object> pts1 = new Array<object>();

            pts0.push(new Array<double>(new double[] { -5,0,0 }));
            pts0.push(new Array<double>(new double[] { 10, 0, 0 }));
            pts0.push(new Array<double>(new double[] { 10, 10, 0 }));
            pts0.push(new Array<double>(new double[] { 0, 10, 0 }));
            pts0.push(new Array<double>(new double[] { 5, 5, 0 }));

            pts1.push(new Array<double>(new double[] { -5, 0, 0 }));
            pts1.push(new Array<double>(new double[] { 5, -1, 0 }));
            pts1.push(new Array<double>(new double[] { 10, 5, 0 }));
            pts1.push(new Array<double>(new double[] { 3, 10, 0 }));
            pts1.push(new Array<double>(new double[] { 5, 12, 0 }));

            Array<double> knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 });
            Array<double> weights = new Array<double>(new double[] { 1, 1, 1, 1, 1 });

            verb.geom.NurbsCurve curve0 = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts0, weights);
            verb.geom.NurbsCurve curve1 = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts1, weights);

            Array<object> intersections = verb.eval.Intersect.curves(curve0._data, curve1._data, verb.core.Constants.TOLERANCE);
            
            _testOutput.WriteLine(intersections.length.ToString());
            for (int i = 0; i < intersections.length; i++)
            {
                verb.core.CurveCurveIntersection t = (verb.core.CurveCurveIntersection)intersections[i];
                _testOutput.WriteLine(t.point0.ToString());
                _testOutput.WriteLine(t.u0.ToString());
            }
        }

        [Fact]
        public void rationalCurveAdaptiveSample()
        {
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 5, 5, 0 }));
            pts.push(new Array<double>(new double[] { 10, 10, 0 }));
            pts.push(new Array<double>(new double[] { 20, 15, 0 }));
            pts.push(new Array<double>(new double[] { 35, 15, 0 }));
            pts.push(new Array<double>(new double[] { 45, 10, 0 }));
            pts.push(new Array<double>(new double[] { 50, 5, 0 }));

            Array<double> knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 0.0, 0.333333, 0.666667, 1.0, 1.0, 1.0, 1.0 });
            Array<double> knots2 = new Array<double>(new double[] { 0.0, 0.2, 0.4, 0.6, 0.8, 1.0});
            Array<double> weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts, weights);
            verb.geom.NurbsCurve curve2 = verb.geom.NurbsCurve.byPoints(pts, 1);

            Array<object> p = verb.eval.Tess.rationalCurveAdaptiveSample(curve._data, 1e-6, null);

            _testOutput.WriteLine($"{p.length}");
            for (int i = 0; i < p.length; i++)
            {
                _testOutput.WriteLine($"{p[i]}");
            }
        }

        [Fact]
        public void rationalCurveRegularSample()
        {
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 1, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 1, 1, 0, 1 }));
            pts.push(new Array<double>(new double[] { 0, 2, 0, 1 }));

            Array<double> knots = new Array<double>(new double[] { 0, 0, 0, 1, 1, 1 });

            NurbsCurveData curve = new verb.core.NurbsCurveData(2, knots, pts);

            Array<object> p = verb.eval.Tess.rationalCurveRegularSample(curve, 10, true);

            _testOutput.WriteLine($"{p.length}");
            for (int i = 0; i < p.length; i++)
            {
                _testOutput.WriteLine($"{p[i]}");
            }
        }

        [Fact]
        public void decomposeMatrix()
        {
            Array<object> matrix = new Array<object> ();

            //matrix.push(new Array<object>(new object[] {10, -7, 0}));
            //matrix.push(new Array<object>(new object[] { -3, 2, 6 }));
            //matrix.push(new Array<object>(new object[] { 5, -1, 5 }));

            matrix.push(new Array<object>(new object[] { 1,2,3 }));
            matrix.push(new Array<object>(new object[] { 2,4,5 }));
            matrix.push(new Array<object>(new object[] { 1,3,4 }));

            //matrix.push(new Array<object>(new object[] { 4,3 }));
            //matrix.push(new Array<object>(new object[] { 6,3 }));


            verb.core._Mat.LUDecomp matrixLU = Mat.LU(matrix);

            //var vector = new Array<double>(new double[] {3,13,4});
            Array<double> vector = new Array<double>(new double[] { 0,4,17});

            Array<double> solved = Mat.LUsolve(matrixLU, vector);

            for (int i = 0; i < matrixLU.LU.length; i++)
            {
                _testOutput.WriteLine($"{matrixLU.LU[i]}");
            }
            _testOutput.WriteLine($"Solved - {solved}");
        }
    }
}
