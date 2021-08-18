using GShark.Core;
using GShark.Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Initializes a new polycurve from an existing curve.
        /// </summary>
        public PolyCurve(ICurve curve) {
            this.Segments.Add(curve);
        }

        /// <summary>
        /// Appends and matches the start of the line to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(Line line)
        {
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(line.Start) <= GSharkMath.Epsilon)
            {
                this.Segments.Add(line);
            }
            else
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
        }

        /// <summary>
        /// Appends and matches the start of the arc to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(Arc arc)
        {
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(arc.StartPoint) <= GSharkMath.Epsilon)
            {
                this.Segments.Add(arc);
            }
            else
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
        }

        /// <summary>
        /// Appends and matches the start of the nurbs to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(NurbsCurve nurbs)
        {
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(nurbs.LocationPoints.First()) <= GSharkMath.Epsilon)
            {
                this.Segments.Add(nurbs);
            }
            else
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
        }

        /// <summary>
        /// Appends and matches the start of the nurbs to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(PolyCurve polycurve)
        {
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(polycurve.LocationPoints.First()) <= GSharkMath.Epsilon)
            {
                this.Segments.Add(polycurve);
            }
            else
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
        }


        /// <summary>
        /// Explodes this PolyCurve into a list of Curve segments.
        /// </summary>
        /// <returns></returns>
        public ICollection<ICurve> Explode()
        {
            return this.Segments.Select(crv => crv).ToList();
        }

        public double Length => GetLength();

        /// <summary>
        /// The segments of the PolyCurve
        /// </summary>
        public ICollection<ICurve> Segments { get; set; } = new List<ICurve>();

        /// <summary>
        /// Returns the start point of the polycurve
        /// </summary>
        public Point3 StartPoint => this.Segments.First().LocationPoints.First();

        /// <summary>
        /// Returns the end point of the polycurve
        /// </summary>
        public Point3 EndPoint => this.Segments.Last().LocationPoints.Last();

        /// <summary>
        /// First and last point of the PolyCurve are coincident
        /// </summary>
        public bool IsClosed => this.StartPoint.DistanceTo(this.EndPoint) <= GSharkMath.Epsilon;

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
        /// Closes the polycurve with a line segment
        /// </summary>
        public void Close()
        {
            if (!this.IsClosed && this.SegmentCount > 0)
            {
                if (this.SegmentCount > 0)
                {

                    this.Segments.Add(new Line(this.EndPoint, this.StartPoint));
                }
                else
                {
                    throw new InvalidOperationException("The polycurve is empty");
                }
            }
            else
            {
                throw new InvalidOperationException("The polycurve is already closed");
            }
        }

        /// <summary>
        /// It calculates the total length of a polycurve based on the different segments.
        /// If there is a polycurve a segment il will recoursively calculate the length of the nested segments.
        /// </summary>
        /// <returns></returns>
        private double GetLength()
        {
            double length = 0;
            foreach (ICurve segment in this.Segments)
            {
                var t = segment.GetType().Name;
                switch (t)
                {
                    case "NurbsCurve":
                        length += ((NurbsCurve)segment).Length();
                        break;
                    case "Line":
                        length += ((Line)segment).Length;
                        break;
                    case "Arc":
                        length += ((Arc)segment).Length;
                        break;
                    case "PolyCurve":
                        length += ((PolyCurve)segment).Length;
                        break;
                }
            }
            return length;
        }

        public List<Interval> GetCurveDomainsFromLength()
        {
            var intervals = new List<Interval>();
            
            return intervals;
        }

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
