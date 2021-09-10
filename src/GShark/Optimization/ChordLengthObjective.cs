using System;
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
            throw new NotImplementedException();
        }

        public Vector Gradient(Vector v)
        {
            throw new NotImplementedException();
        }
    }
}
