using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;

namespace GeometrySharp.Core.BoundingBoxTree
{
    public class LazyCurveBBT : IBoundingBoxTree<NurbsCurve>
    {
        private readonly NurbsCurve _curve;
        private readonly BoundingBox _boundingBox;
        private readonly double _knotTolerance;

        public LazyCurveBBT(NurbsCurve curve, double knotTolerance = -1)
        {
            _curve = curve;

            if (knotTolerance < 0.0)
            {
                _knotTolerance = (_curve.Knots[^1] - _curve.Knots[0]) / 64;
            }

            _knotTolerance = knotTolerance;
        }
        public BoundingBox BoundingBox()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool Indivisible(double tolerance)
        {
            throw new NotImplementedException();
        }

        public bool Empty()
        {
            throw new NotImplementedException();
        }
    }
}
