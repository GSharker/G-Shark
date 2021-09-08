using System;
using System.Collections.Generic;
using GShark.ExtendedMethods;
using GShark.Geometry;
using GShark.Interfaces;

namespace GShark.Intersection.BoundingBoxTree
{
    internal class LazyCurveBBT : IBoundingBoxTree<NurbsBase>
    {
        private readonly NurbsBase _curve;
        private readonly BoundingBox _boundingBox;
        private readonly double _knotTolerance;

        internal LazyCurveBBT(NurbsBase curve, double knotTolerance = double.NaN)
        {
            _curve = curve;
            _boundingBox = new BoundingBox(curve.ControlPointLocations);

            if (double.IsNaN(knotTolerance))
            {
                knotTolerance = _curve.Knots.GetDomain(_curve.Degree).Length / 64;
            }

            _knotTolerance = knotTolerance;
        }

        public BoundingBox BoundingBox()
        {
            return _boundingBox;
        }

        public Tuple<IBoundingBoxTree<NurbsBase>, IBoundingBoxTree<NurbsBase>> Split()
        {
            Random r = new Random();
            double t = (_curve.Knots[_curve.Knots.Count - 1] + _curve.Knots[0]) / 2.0 +
                       (_curve.Knots[_curve.Knots.Count - 1] - _curve.Knots[0]) * 0.1 * r.NextDouble();
            List<NurbsBase> curves = _curve.SplitAt(t);

            return new Tuple<IBoundingBoxTree<NurbsBase>, IBoundingBoxTree<NurbsBase>>
                (new LazyCurveBBT(curves[0], _knotTolerance), new LazyCurveBBT(curves[1], _knotTolerance));
        }

        public NurbsBase Yield()
        {
            return _curve;
        }

        public bool IsIndivisible(double tolerance)
        {
            return _curve.Knots.GetDomain(_curve.Degree).Length < _knotTolerance;
        }

        public bool IsEmpty()
        {
            return false;
        }
    }
}
