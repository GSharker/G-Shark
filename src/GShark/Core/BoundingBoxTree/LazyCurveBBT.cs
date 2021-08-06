using GShark.Geometry;
using GShark.Operation;
using System;
using System.Collections.Generic;
using GShark.ExtendedMethods;
using GShark.Geometry.Interfaces;

namespace GShark.Core.BoundingBoxTree
{
    internal class LazyCurveBBT : IBoundingBoxTree<ICurve>
    {
        private readonly ICurve _curve;
        private readonly BoundingBox _boundingBox;
        private readonly double _knotTolerance;

        internal LazyCurveBBT(ICurve curve, double knotTolerance = double.NaN)
        {
            _curve = curve;
            _boundingBox = new BoundingBox(curve.LocationPoints);

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

        public Tuple<IBoundingBoxTree<ICurve>, IBoundingBoxTree<ICurve>> Split()
        {
            Random r = new Random();
            double t = (_curve.Knots[_curve.Knots.Count - 1] + _curve.Knots[0]) / 2.0 +
                       (_curve.Knots[_curve.Knots.Count - 1] - _curve.Knots[0]) * 0.1 * r.NextDouble();
            List<ICurve> curves = _curve.SplitAt(t);

            return new Tuple<IBoundingBoxTree<ICurve>, IBoundingBoxTree<ICurve>>
                ( new LazyCurveBBT(curves[0], _knotTolerance), new LazyCurveBBT(curves[1], _knotTolerance));
        }

        public ICurve Yield()
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
