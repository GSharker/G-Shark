using GShark.Core;
using GShark.Geometry.Enum;
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
        /// Defines the curve type
        /// </summary>
        public CurveType CurveType => CurveType.POLYCURVE;

        /// <summary>
        /// Appends and matches the start of the line to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(Line line)
        {
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(line.StartPoint) <= GSharkMath.Epsilon)
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
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(nurbs.ControlPointLocations.First()) <= GSharkMath.Epsilon)
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
            if (this.SegmentCount == 0 || this.EndPoint.DistanceTo(polycurve.StartPoint) <= GSharkMath.Epsilon)
            {
                this.Segments.Add(polycurve);
            }
            else
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
        }

        /// <summary>
        /// Return the polycurve bounding box
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                throw new NotImplementedException();
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

        /// <summary>
        /// Return the total lenght of the polycurve
        /// </summary>
        public double Length
        {
            get
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
        }

        /// <summary>
        /// The segments of the PolyCurve
        /// </summary>
        public IList<ICurve> Segments { get; set; } = new List<ICurve>();

        /// <summary>
        /// Ordered list of segment lengths
        /// </summary>
        public IList<double> SegmentsLengths
        {
            get
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
        }

        /// <summary>
        /// Returns the start point of the polycurve
        /// </summary>
        public Point3 StartPoint => this.Segments.First().PointAt(0.0);

        /// <summary>
        /// Returns the end point of the polycurve
        /// </summary>
        public Point3 EndPoint => this.Segments.Last().PointAt(1.0);

        /// <summary>
        /// First and last point of the PolyCurve are coincident
        /// </summary>
        public bool IsClosed => this.StartPoint.DistanceTo(this.EndPoint) <= GSharkMath.Epsilon;

        /// <summary>
        /// Number of segments
        /// </summary>
        public int SegmentCount => this.Segments.Count;

        public int Degree => throw new NotImplementedException();

        public List<Point3> ControlPointLocations => throw new NotImplementedException();

        public List<Point4> ControlPoints => throw new NotImplementedException();

        public KnotVector Knots => throw new NotImplementedException();

        public Interval Domain => throw new NotImplementedException();


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
            double progressiveEndLength = 0;
            double progressiveStartLength = 0;

            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            else if (l > this.Length)
            {
                throw new InvalidOperationException("Length value is bigger than the polycurve total length");
            }
            else
            {
                for (int i = 0; i < this.SegmentCount; i++)
                {
                    var segment = this.Segments[i];
                    progressiveStartLength = progressiveEndLength;
                    progressiveEndLength += this.SegmentsLengths[i];
                    if (l <= progressiveEndLength) // This is the right segment
                    {
                        double segmentLength = i == 0 ? l : l - progressiveStartLength;
                        var type = segment.GetType().Name;
                        switch (type)
                        {
                            case "NurbsCurve":
                                var par = Analyze.CurveParameterAtLength((NurbsCurve)segment, segmentLength, GSharkMath.Epsilon);
                                var par1 = GSharkMath.RemapValue(
                                    segmentLength,
                                    new Interval(0, this.SegmentsLengths[i]),
                                    new Interval(0, 1)
                                    );
                                return ((NurbsCurve)segment).PointAt(par);
                            case "Line":
                                return ((Line)segment).PointAtLength(segmentLength);
                            case "Arc":
                                return ((Arc)segment).PointAtLength(segmentLength);
                            case "PolyCurve":
                                return ((PolyCurve)segment).PointAtLength(segmentLength);
                        }
                    }
                }
            }
            return new Point3();
        }

        /// <summary>
        /// Returns a point at a given parameter along the polycurve [0,1]
        /// </summary>
        /// <param name="t">Parameter value between 0 and 1</param>
        /// <returns></returns>
        public Point3 PointAt(double t)
        {
            var domains = CurveDomainsFromLengths(this);

            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            else if (t > 1.0)
            {
                t = 1.0;
            }
            else
            {
                var param = t * SegmentCount;
                var i = int.Parse(Math.Floor(param).ToString());
                param -= i;
                var segment = this.Segments[i];

                var type = segment.GetType().Name;
                switch (type)
                {
                    case "NurbsCurve":
                        return ((NurbsCurve)segment).PointAt(param);
                    case "Line":
                        return ((Line)segment).PointAt(param);
                    case "Arc":
                        return ((Arc)segment).PointAt(param);
                    case "PolyCurve":
                        return ((PolyCurve)segment).PointAt(param);
                }
            }
            return new Point3();
        }

        public static List<Interval> CC(PolyCurve curve)
        {
            var intervals = new List<Interval>();
            double min = 0, max = 0;
            foreach (double length in curve.SegmentsLengths)
            {
                double proportion = length / curve.SegmentCount;
                max = min + proportion;
                intervals.Add(new Interval(min, max));
                min = max;
            }

            return intervals;
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


        public Point3 ClosestPoint(Point3 pt)
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
