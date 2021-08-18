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
        /// Appends and matches the start of the line to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(Line line)
        {
            this.Segments.Add(line);
        }

        /// <summary>
        /// Appends and matches the start of the arc to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(Arc arc)
        {
            this.Segments.Add(arc);
        }

        /// <summary>
        /// Appends and matches the start of the nurbs to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(NurbsCurve nurbs)
        {
            this.Segments.Add(nurbs);
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
        public ICollection<ICurve> Segments { get; set; } = new List<ICurve>();

        /// <summary>
        /// Returns the start point of the polycurve
        /// </summary>
        public Point3 StartPoint => this.Segments.First().PointAt(0);

        /// <summary>
        /// Returns the end point of the polycurve
        /// </summary>
        public Point3 EndPoint => this.Segments.Last().PointAt(1);

        /// <summary>
        /// Number of segments
        /// </summary>
        public int SegmentCount => this.Segments.Count;

        /// <summary>
        /// Returns the location points for each segment
        /// </summary>
        public List<Point3> LocationPoints => this.GetLocationPoints();

        /// <summary>
        /// Returns the controls points for each segment
        /// </summary>
        public List<Point4> ControlPoints => this.GetControlPoints();

        public int Degree => throw new NotImplementedException();

        /// <summary>
        /// Returns the location points for each segment
        /// </summary>
        /// <returns></returns>
        private List<Point3> GetLocationPoints()
        {
            List<Point3> locPts = new List<Point3>();
            foreach (var segm in Segments)
            {
                locPts.AddRange(segm.LocationPoints.Select(pts => pts));
            }
            return locPts;
        }

        /// <summary>
        /// Returns the location points for each segment
        /// </summary>
        /// <returns></returns>
        private List<Point4> GetControlPoints()
        {
            List<Point4> ctrlPts = new List<Point4>();
            foreach (var segm in Segments)
            {
                ctrlPts.AddRange(segm.ControlPoints.Select(pts => pts));
            }
            return ctrlPts;
        }

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
