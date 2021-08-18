using GShark.Core;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GShark.Geometry
{
    /// <summary>
    /// Represents a curve that is the result of joining several (possibly different) types of curves.
    /// </summary>
    public class PolyCurve : ICurve
    {
        /// <summary>
        /// Initializes a newempty polycurve.
        /// </summary>
        public PolyCurve() { }

        /// <summary>
        /// Initializes a new polycurve from a curve.
        /// </summary>
        public PolyCurve(ICurve curve)
        {
            this.Segments.Add(curve);
        }

        /// <summary>
        /// Appends and matches the start of the line to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public PolyCurve Append(Line line)
        {
            this.Segments.Add(line);
            return this;
        }

        /// <summary>
        /// Appends and matches the start of the arc to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public PolyCurve Append(Arc arc)
        {
            this.Segments.Add(arc);
            return this;
        }

        /// <summary>
        /// Appends and matches the start of the nurbs to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public PolyCurve Append(NurbsCurve nurbs)
        {
            this.Segments.Add(nurbs);
            return this;
        }

        /// <summary>
        /// Explodes this PolyCurve into a list of Curve segments.
        /// </summary>
        /// <returns></returns>
        public ICollection<ICurve> Explode()
        {
            return this.Segments.Select(crv => crv).ToList();
        }

        /// <summary>
        /// The segments of the PolyCurve
        /// </summary>
        public ICollection<ICurve> Segments => Explode();

        /// <summary>
        /// Number of segments
        /// </summary>
        public int SegmentCount => this.Segments.Count;

        public int Degree => throw new NotImplementedException();

        public List<Point3> LocationPoints => throw new NotImplementedException();

        public List<Point4> ControlPoints => throw new NotImplementedException();

        public KnotVector Knots => throw new NotImplementedException();

        public Interval Domain => throw new NotImplementedException();

        public BoundingBox BoundingBox => throw new NotImplementedException();

        public Point3 ClosestPoint(Point3 pt)
        {
            throw new NotImplementedException();
        }

        public Point3 PointAt(double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs the string representation for the current PolyCurve.
        /// </summary>
        /// <returns>The point representation in the form X,Y,Z,W.</returns>
        public override string ToString()
        {
            return $"PolyCurve ({this.SegmentCount})";
        }
    }
}
