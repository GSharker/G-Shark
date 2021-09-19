using System;
using System.Linq;
using System.Xml.Schema;
using GShark.Core;
using GShark.Geometry;
using GShark.Operation;

namespace GShark.Optimization
{
    /// <summary>
    /// The basic functions used into the minimization process, to define the intersection results between a curve and a plane.
    /// </summary>
    internal class ChordLengthObjective : IObjectiveFunction
    {
        private readonly NurbsBase _curve;
        private readonly double _startParam;
        private readonly double _chordLength;

        internal ChordLengthObjective(NurbsBase crv, double startParam, double chordLength)
        {
            _curve = crv;
            _startParam = startParam;
            _chordLength = chordLength;
        }

        public double Value(Vector v)
        {
            var pointAtT = Evaluation.RationalCurveDerivatives(_curve, v[0], 0).First();
            var pointAtStart = Evaluation.RationalCurveDerivatives(_curve, _startParam, 0).First();
            var vec = pointAtT - pointAtStart;
            return  Math.Pow(_chordLength - vec.Length, 2);
        }

        public Vector Gradient(Vector v)
        {
            var pointAtT = Evaluation.RationalCurveDerivatives(_curve, v[0], 0).First();
            var pointAtStart = Evaluation.RationalCurveDerivatives(_curve, _startParam, 0).First();
            var vec = pointAtT - pointAtStart;
            var slope = -2 * (_chordLength - vec.Length);

            return new Vector() {slope, slope};
        }
    }
}
