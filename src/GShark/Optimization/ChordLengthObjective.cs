using System;
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
            return Math.Pow(_chordLength - _curve.PointAt(v[0]).DistanceTo(_curve.PointAt(_startParam)), 2);
        }

        public Vector Gradient(Vector v)
        {
            var currentChordLength = _curve.PointAt(v[0]).DistanceTo(_curve.PointAt(_startParam));
            var slope = -2 * (_chordLength - currentChordLength);

            return new Vector() {slope, slope};
        }
    }
}
