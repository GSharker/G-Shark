using verb.core;
using verb.eval;
using verb.geom;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit
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
        public void NurbsBasebyPoints()
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

            pts0.push(new Array<double>(new double[] { -5, 0, 0 }));
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
            Array<double> knots2 = new Array<double>(new double[] { 0.0, 0.2, 0.4, 0.6, 0.8, 1.0 });
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
            Array<object> matrix = new Array<object>();

            //matrix.push(new Array<object>(new object[] {10, -7, 0}));
            //matrix.push(new Array<object>(new object[] { -3, 2, 6 }));
            //matrix.push(new Array<object>(new object[] { 5, -1, 5 }));

            matrix.push(new Array<object>(new object[] { 1, 2, 3 }));
            matrix.push(new Array<object>(new object[] { 2, 4, 5 }));
            matrix.push(new Array<object>(new object[] { 1, 3, 4 }));

            //matrix.push(new Array<object>(new object[] { 4,3 }));
            //matrix.push(new Array<object>(new object[] { 6,3 }));


            verb.core._Mat.LUDecomp matrixLU = Mat.LU(matrix);

            //var vector = new Array<double>(new double[] {3,13,4});
            Array<double> vector = new Array<double>(new double[] { 0, 4, 17 });

            Array<double> solved = Mat.LUsolve(matrixLU, vector);

            for (int i = 0; i < matrixLU.LU.length; i++)
            {
                _testOutput.WriteLine($"{matrixLU.LU[i]}");
            }
            _testOutput.WriteLine($"Solved - {solved}");
        }

        [Fact]
        public void DecomposeNurbsIntoBeziers()
        {
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 0, 5, 5 }));
            pts.push(new Array<double>(new double[] { 0, 0, 0 }));
            pts.push(new Array<double>(new double[] { 4, 0, 0 }));
            pts.push(new Array<double>(new double[] { 5, 5, 5 }));
            pts.push(new Array<double>(new double[] { 0, 5, 0 }));

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byPoints(pts, 3);

            var res = verb.eval.Modify.decomposeCurveIntoBeziers(curve._data);

            for (int i = 0; i < res.length; i++)
            {
                _testOutput.WriteLine($"{res[i]}");
            }
        }

        [Fact]
        public void AdaptiveSubdivision()
        {
            Array<object> pts = new Array<object>();

            //pts.push(new Array<double>(new double[] { 100, 25, 0 }));
            //pts.push(new Array<double>(new double[] { 10, 90, 0 }));
            //pts.push(new Array<double>(new double[] { 110, 100, 0 }));
            //pts.push(new Array<double>(new double[] { 150, 195, 0 }));

            pts.push(new Array<double>(new double[] { 4.5, 2.5, 2.5 }));
            pts.push(new Array<double>(new double[] { 5, 5, 5 }));
            pts.push(new Array<double>(new double[] { 0, 5, 0 }));

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byPoints(pts, 2);

            var res = verb.eval.Tess.rationalCurveAdaptiveSample(curve._data, 1, true);

            _testOutput.WriteLine(res.length.ToString());
            for (int i = 0; i < res.length; i++)
            {
                var r = res[i];
                _testOutput.WriteLine($"{{{r}}}");
            }
        }

        [Fact]
        public void FourPointSurface()
        {
            Array<double> pt1 = new Array<double>(new double[] { 0.0, 0.0, 0.0 });
            Array<double> pt2 = new Array<double>(new double[] { 10.0, 0.0, 0.0 });
            Array<double> pt3 = new Array<double>(new double[] { 10.0, 10.0, 2.0 });
            Array<double> pt4 = new Array<double>(new double[] { 0.0, 10.0, 4.0 });

            var s1 = verb.eval.Make.fourPointSurface(pt1, pt2, pt3, pt4, 1);
            var p = verb.eval.Eval.rationalSurfacePoint(s1, 0.2, 0.5);

            _testOutput.WriteLine($"{{{p}}}");

            for (int i = 0; i < s1.controlPoints.length; i++)
            {
                var r = s1.controlPoints[i];
                _testOutput.WriteLine($"{{{r}}}");
            }
        }

        [Fact]
        public void SurfaceFromPoints()
        {
            Array<double> pt1 = new Array<double>(new double[] { 0.0, 0.0, 0.0 });
            Array<double> pt2 = new Array<double>(new double[] { 10.0, 0.0, 0.0 });
            Array<double> pt3 = new Array<double>(new double[] { 10.0, 10.0, 2.0 });
            Array<double> pt4 = new Array<double>(new double[] { 0.0, 10.0, 4.0 });
            Array<double> pt5 = new Array<double>(new double[] { 5.0, 0.0, 0.0 });
            Array<double> pt6 = new Array<double>(new double[] { 5.0, 10.0, 5.0 });

            Array<object> pts = new Array<object>();
            Array<object> c1 = new Array<object>();
            c1.push(pt1);
            c1.push(pt4);

            Array<object> c2 = new Array<object>();
            c2.push(pt5);
            c2.push(pt6);

            Array<object> c3 = new Array<object>();
            c3.push(pt2);
            c3.push(pt3);

            pts.push(c1);
            pts.push(c2);
            pts.push(c3);

            Array<object> weight = new Array<object>();
            Array<object> w1 = new Array<object>();
            w1.push(1);
            w1.push(1);

            Array<object> w2 = new Array<object>();
            w2.push(1);
            w2.push(2);

            Array<object> w3 = new Array<object>();
            w3.push(1);
            w3.push(1);

            weight.push(w1);
            weight.push(w2);
            weight.push(w3);

            Array<double> knotsU = new Array<double>(new double[] { 0, 0, 0, 1, 1, 1 });
            Array<double> knotsV = new Array<double>(new double[] { 0, 0, 1, 1 });

            var surface = verb.geom.NurbsSurface.byKnotsControlPointsWeights(2, 1, knotsU, knotsV, pts, weight);
            var surfaces = Divide.surfaceSplit(surface._data, 0.5, false);

            var derivatives = verb.eval.Eval.rationalSurfaceDerivatives(surface._data, 0.3, 0.5, 1);

            var rational = verb.eval.Eval.dehomogenize2d(derivatives);

            _testOutput.WriteLine($"{{{rational}}}");

            //for (int i = 0; i < s1.controlPoints.length; i++)
            //{
            //    var r = s1.controlPoints[i];
            //    _testOutput.WriteLine($"{{{r}}}");
            //}
        }

        [Fact]
        public void CurveKnotRefine()
        {
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 0.0, 10.0, 4.0 }));
            pts.push(new Array<double>(new double[] { 5.0, 10.0, 5.0 }));
            pts.push(new Array<double>(new double[] { 10.0, 10.0, 2.0 }));

            Array<double> knots = new Array<double>(new double[] { 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 });
            Array<double> weights = new Array<double>(new double[] { 1.0, 2.0, 1.0 });
            Array<double> knotsToInsert = new Array<double>(new double[] { 0.5, 0.5, 0.5 });

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byKnotsControlPointsWeights(2, knots, pts, weights);
            var after = verb.eval.Modify.curveKnotRefine(curve._data, knotsToInsert);

            _testOutput.WriteLine($"{after.controlPoints}");
        }

        [Fact]
        public void CurveKnotRefine2()
        {
            Array<object> pts = new Array<object>();

            for (int i = 0; i < 8; i++)
            {
                pts.push(new Array<double>(new double[] { i, 0, 0 }));
            }

            Array<double> knots = new Array<double>(new double[] { 0, 0, 0, 0, 1, 2, 3, 4, 5, 5, 5, 5 });
            Array<double> weights = new Array<double>(new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });
            Array<double> newKnots = new Array<double>(new double[] { 0.5 });

            verb.geom.NurbsCurve curve = verb.geom.NurbsCurve.byKnotsControlPointsWeights(3, knots, pts, weights);
            var curveData = new verb.core.NurbsCurveData(3, knots, pts);
            var after = verb.eval.Modify.curveKnotRefine(curve._data, newKnots);

            _testOutput.WriteLine($"{after.controlPoints}");
        }

        [Fact]
        public void SurfaceClosestPt()
        {
            Array<double> pt1 = new Array<double>(new double[] { 0.0, 0.0, 0.0 });
            Array<double> pt2 = new Array<double>(new double[] { 10.0, 0.0, 0.0 });
            Array<double> pt3 = new Array<double>(new double[] { 10.0, 10.0, 2.0 });
            Array<double> pt4 = new Array<double>(new double[] { 0.0, 10.0, 4.0 });
            Array<double> pt5 = new Array<double>(new double[] { 5.0, 0.0, 0.0 });
            Array<double> pt6 = new Array<double>(new double[] { 5.0, 10.0, 5.0 });

            Array<object> pts = new Array<object>();
            Array<object> c1 = new Array<object>();
            c1.push(pt1);
            c1.push(pt4);

            Array<object> c2 = new Array<object>();
            c2.push(pt5);
            c2.push(pt6);

            Array<object> c3 = new Array<object>();
            c3.push(pt2);
            c3.push(pt3);

            pts.push(c1);
            pts.push(c2);
            pts.push(c3);

            Array<object> weight = new Array<object>();
            Array<object> w1 = new Array<object>();
            w1.push(1);
            w1.push(1);

            Array<object> w2 = new Array<object>();
            w2.push(1);
            w2.push(2);

            Array<object> w3 = new Array<object>();
            w3.push(1);
            w3.push(1);

            weight.push(w1);
            weight.push(w2);
            weight.push(w3);

            Array<double> knotsU = new Array<double>(new double[] { 0, 0, 0, 1, 1, 1 });
            Array<double> knotsV = new Array<double>(new double[] { 0, 0, 1, 1 });

            var surface = verb.geom.NurbsSurface.byKnotsControlPointsWeights(2, 1, knotsU, knotsV, pts, weight);
            Array<object> derivatives = verb.eval.Eval.rationalSurfaceDerivatives(surface._data, 0.25, 0.25, 2);

            var pt = new Array<double>(new double[] { 2.5, 1.5, 2 });

            var rational =
                verb.eval.Analyze.rationalSurfaceClosestParam(surface._data, pt);

            var ptd = (Array<object>)derivatives.__a[0];
            var diff = verb.core.Vec.sub((Array<double>)ptd.__a[0], pt);

            var Su = (Array<object>)derivatives.__a[1];
            var Sv = (Array<object>)derivatives.__a[0];

            var Suu = (Array<object>)derivatives.__a[2];
            var Svv = (Array<object>)derivatives.__a[0];

            var Suv = (Array<object>)derivatives.__a[1];
            var Svu = (Array<object>)derivatives.__a[1];

            var f = verb.core.Vec.dot((Array<double>)Su.__a[0], diff);
            var g = verb.core.Vec.dot((Array<double>)Sv.__a[1], diff);

            var k = new Array<double>(new double[] { -f, -g });
            var uv = new Array<double>(new double[] { 0.25, 0.25 });

            var J00 = Vec.dot((Array<double>)Su.__a[0], (Array<double>)Su.__a[0]) + Vec.dot((Array<double>)Suu.__a[0], diff);
            var J01 = Vec.dot((Array<double>)Su.__a[0], (Array<double>)Sv.__a[1]) + Vec.dot((Array<double>)Suv.__a[1], diff);
            var J10 = Vec.dot((Array<double>)Su.__a[0], (Array<double>)Sv.__a[1]) + Vec.dot((Array<double>)Svu.__a[1], diff);
            var J11 = Vec.dot((Array<double>)Sv.__a[1], (Array<double>)Sv.__a[1]) + Vec.dot((Array<double>)Svv.__a[2], diff);

            Array<object> matrix = new Array<object>();
            matrix.push(new Array<object>(new object[] { J00, J01 }));
            matrix.push(new Array<object>(new object[] { J10, J11 }));

            verb.core._Mat.LUDecomp matrixLU = Mat.LU(matrix);

            var d = verb.core.Mat.solve(matrix, k);
            var res = Vec.add(d, uv);

            //_testOutput.WriteLine($"{{{rational}}}");
        }

        [Fact]
        public void ElevateDegree()
        {
            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 0, 0, 0 }));
            pts.push(new Array<double>(new double[] { 10, 10, 0 }));

            var polyline = verb.eval.Make.polyline(pts);

            var elevatePoly = verb.eval.Modify.curveElevateDegree(polyline, 3);

            _testOutput.WriteLine($"{elevatePoly.controlPoints.length}");
            for (int i = 0; i < elevatePoly.controlPoints.length; i++)
            {
                _testOutput.WriteLine($"{elevatePoly.controlPoints[i]}");
            }
        }

        [Fact]
        public void RevolvedSurface()
        {
            var axis = new Array<double>(new double[] { 0, 0, 1 });
            var pt = new Array<double>(new double[] { 0, 0, 1 });
            var xaxis = new Array<double>(new double[] { 1, 0, 0 });
            var center = new Array<double>(new double[] { 0, 0, 0 });

            var arc = new Arc(center, axis, xaxis, 1, 0.0, Math.PI);

            Array<object> pts = new Array<object>();

            pts.push(new Array<double>(new double[] { 1, 0, 0, 1 }));
            pts.push(new Array<double>(new double[] { 0, 0, 1, 1 }));
            Array<double> knots = new Array<double>(new double[] { 0.0, 0.0, 1.0, 1.0 });
            var profile = new verb.core.NurbsCurveData(1, knots, pts);

            var comps = verb.eval.Make.revolvedSurface(arc._data, center, axis, 0.25 * Math.PI);

            for (int i = 0; i < comps.controlPoints.length; i++)
            {
                var h = verb.eval.Eval.dehomogenize1d((Array<object>) comps.controlPoints.__a[i]);
                _testOutput.WriteLine($"{h}");
            }
        }
    }
}
