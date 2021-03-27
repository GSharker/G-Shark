using System;
using System.Collections.Generic;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;

namespace GeometrySharp.Core.BoundingBoxTree
{
    // ToDo: Make the test for the methods.
    public class LazyCurveBBT : IBoundingBoxTree<NurbsCurve>
    {
        private readonly NurbsCurve _curve;
        private readonly BoundingBox _boundingBox;
        private readonly double _knotTolerance;

        public LazyCurveBBT(NurbsCurve curve, double knotTolerance = double.NaN)
        {
            _curve = curve;
            _boundingBox = new BoundingBox(curve.ControlPoints);

            if (double.IsNaN(knotTolerance))
            {
                knotTolerance = _curve.Knots.Domain / 64;
            }

            _knotTolerance = knotTolerance;
        }
        public BoundingBox BoundingBox()
        {
            return _boundingBox;
        }

        public Tuple<IBoundingBoxTree<NurbsCurve>, IBoundingBoxTree<NurbsCurve>> Split()
        {
            Random r = new Random();
            double t = (_curve.Knots[^1] + _curve.Knots[0]) / 2.0 +
                       (_curve.Knots[^1] - _curve.Knots[0]) * 0.1 * r.NextDouble();
            List<NurbsCurve> curves = Divide.CurveSplit(_curve, t);

            return new Tuple<IBoundingBoxTree<NurbsCurve>, IBoundingBoxTree<NurbsCurve>>
                ( new LazyCurveBBT(curves[0], _knotTolerance), new LazyCurveBBT(curves[1], _knotTolerance));
        }

        public NurbsCurve Yield()
        {
            return _curve;
        }

        public bool IsIndivisible(double tolerance)
        {
            return _curve.Knots.Domain < _knotTolerance;
        }

        public bool IsEmpty()
        {
            return false;
        }
    }
}
