﻿using GShark.Core;
using GShark.Geometry.Interfaces;
using GShark.Operation;
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
        public PolyCurve(ICurve curve)
        {
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
        public IList<ICurve> Explode()
        {
            return this.Segments.Select(crv => crv).ToList();
        }

        public double Length => GetLength();

        /// <summary>
        /// The segments of the PolyCurve
        /// </summary>
        public IList<ICurve> Segments { get; set; } = new List<ICurve>();

        /// <summary>
        /// Ordered list of segment lengths
        /// </summary>
        public IList<double> SegmentsLengths => GetSegmentLength();

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
        /// Returns a point at a given length along the polycurve
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Point3 PointAtLength(double l)
        {
            double progressiveLength = 0;
            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            else if(l > this.Length)
            {
                throw new InvalidOperationException("Length value is bigger than the polycurve total length");
            }
            else
            {
                for (int i = 0; i < this.SegmentCount; i++)
                {
                    var segment = this.Segments[i];
                    progressiveLength += this.SegmentsLengths[i];
                    if (l <= progressiveLength) // This is the right segment
                    {
                        double segmentsLength = i == 0 ? l : l - this.SegmentsLengths[i-1];
                        var t = segment.GetType().Name;
                        switch (t)
                        {
                            case "NurbsCurve":
                                var par = Analyze.CurveParameterAtLength((NurbsCurve)segment, segmentsLength, GSharkMath.Epsilon);
                                return ((NurbsCurve)segment).PointAt(par);
                            case "Line":
                                var line = ((Line)segment);
                                var dir = (line.End - line.Start).Amplify(segmentsLength);
                                return ((Line)segment).Start.Transform(Transform.Translation(dir));
                            case "Arc":
                                l = ((Arc)segment).Length;
                                break;
                            case "PolyCurve":
                                l = ((PolyCurve)segment).Length;
                                break;
                        }
                    }
                }
            }
            return new Point3();
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
                double l = 0;
                switch (t)
                {
                    case "NurbsCurve":
                        l = ((NurbsCurve)segment).Length();
                        break;
                    case "Line":
                        l = ((Line)segment).Length;
                        break;
                    case "Arc":
                        l = ((Arc)segment).Length;
                        break;
                    case "PolyCurve":
                        l = ((PolyCurve)segment).Length;
                        break;
                }
                length += l;
                this.SegmentsLengths.Add(l);
            }
            return length;
        }

        /// <summary>
        /// Returns a list of progressive curve lengths
        /// If there is a polycurve a segment il will recoursively calculate the length of the nested segments.
        /// </summary>
        /// <returns></returns>
        private List<double> GetSegmentLength()
        {
            var segmentLengths = new List<double>();
            foreach (ICurve segment in this.Segments)
            {
                var t = segment.GetType().Name;
                double l = 0;
                switch (t)
                {
                    case "NurbsCurve":
                        l = ((NurbsCurve)segment).Length();
                        break;
                    case "Line":
                        l = ((Line)segment).Length;
                        break;
                    case "Arc":
                        l = ((Arc)segment).Length;
                        break;
                    case "PolyCurve":
                        l = ((PolyCurve)segment).Length;
                        break;
                }
                segmentLengths.Add(l);
            }
            return segmentLengths;
        }

        public static List<Interval> CurveDomainsFromLengths(PolyCurve curve)
        {
            var intervals = new List<Interval>();
            double min = 0, max = 0;
            foreach (double length in curve.SegmentsLengths)
            {
                double proportion = length / curve.Length;
                max = min + proportion;
                intervals.Add(new Interval(min, max));
                min = max;
            }
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
