using GShark.Core;
using GShark.Geometry.Enum;
using GShark.Geometry.Interfaces;
using GShark.ExtendedMethods;
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
            if (this.SegmentCount != 0 && this.EndPoint.DistanceTo(line.StartPoint) > GSharkMath.Epsilon)
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
            this.Segments.Add(line);
        }

        /// <summary>
        /// Appends and matches the start of the arc to the end of polycurve.
        /// </summary>
        /// <param name="curve">The arc.</param>
        /// <returns></returns>
        public void Append(Arc arc)
        {
            if (this.SegmentCount != 0 && this.EndPoint.DistanceTo(arc.StartPoint) > GSharkMath.Epsilon)
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
            this.Segments.Add(arc);
        }

        /// <summary>
        /// Appends and matches the start of the nurbs to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(NurbsCurve nurbs)
        {
            if (nurbs.IsClosed())
            {
                throw new InvalidOperationException("The curve is closed.");
            }
            if (this.SegmentCount != 0 && this.EndPoint.DistanceTo(nurbs.PointAt(1.0)) > GSharkMath.Epsilon)
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
            this.Segments.Add(nurbs);
        }

        /// <summary>
        /// Appends and matches the start of the nurbs to the end of polycurve.
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public void Append(PolyCurve polycurve)
        {
            if (this.SegmentCount != 0 && this.EndPoint.DistanceTo(polycurve.StartPoint) > GSharkMath.Epsilon)
            {
                this.Segments.Add(polycurve);
            }
            else
            {
                throw new InvalidOperationException("The two curves can not be connected.");
            }
        }

        /// <summary>
        /// Return the polycurve bounding box.
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
        /// <returns>The ordered list of curves composing the polycurve.</returns>
        public IList<ICurve> Explode()
        {
            return this.Segments.Select(crv => crv).ToList();
        }

        /// <summary>
        /// Return the total lenght of the polycurve.
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
        /// The segments of the polycurve.
        /// </summary>
        public IList<ICurve> Segments { get; set; } = new List<ICurve>();

        /// <summary>
        /// Ordered list of segment lengths.
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
        /// Returns the start point of the polycurve.
        /// </summary>
        public Point3 StartPoint => this.Segments.First().PointAt(0.0);

        /// <summary>
        /// Returns the end point of the polycurve.
        /// </summary>
        public Point3 EndPoint => this.Segments.Last().PointAt(1.0);

        /// <summary>
        /// First and last point of the PolyCurve are coincident.
        /// </summary>
        public bool IsClosed => this.StartPoint.DistanceTo(this.EndPoint) <= GSharkMath.Epsilon;

        /// <summary>
        /// Number of segments.
        /// </summary>
        public int SegmentCount => this.Segments.Count;

        public int Degree => throw new NotImplementedException();

        public List<Point3> ControlPointLocations => throw new NotImplementedException();

        public List<Point4> ControlPoints => throw new NotImplementedException();

        public KnotVector Knots => throw new NotImplementedException();

        public Interval Domain => throw new NotImplementedException();

        /// <summary>
        /// Closes the polycurve with a line segment.
        /// </summary>
        public PolyCurve Close()
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
            return this;
        }

        /// <summary>
        /// Returns a point at a given length along the polycurve.
        /// </summary>
        /// <param name="l">The length.</param>
        /// <returns>The point at the given length.</returns>
        public Point3 PointAtLength(double l)
        {
            if (l == 0)
            {
                return this.StartPoint;
            }

            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            if (l <= this.Length)
            {
                double progressiveEndLength = 0;
                double progressiveStartLength = 0;
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
            return this.EndPoint;
        }

        /// <summary>
        /// Returns a point at a given parameter along the polycurve [0,1].
        /// </summary>
        /// <param name="t">Parameter value between 0 and 1.</param>
        /// <returns>The point at the given parameter.</returns>
        public Point3 PointAt(double t)
        {
            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            else if (t > 1.0)
            {
                t = 1.0;
            }
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
                    var p = ((Line)segment);
                    return ((Line)segment).PointAt(param);
                case "Arc":
                    return ((Arc)segment).PointAt(param);
                case "PolyCurve":
                    return ((PolyCurve)segment).PointAt(param);
            }
            return new Point3();
        }

        /// <summary>
        /// Return a vector tangent at a given length.
        /// </summary>
        /// <param name="l">The length.</param>
        /// <returns>The vector at the given length.</returns>
        public Vector3 TangentAtLength(double l)
        {
            Vector3 tangent = new Vector3();
            double progressiveEndLength = 0;
            double progressiveStartLength = 0;

            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            if (l > this.Length)
            {
                l = this.Length;
            }
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
                            tangent = ((NurbsCurve)segment).TangentAt(par);
                            break;
                        case "Line":
                            tangent = ((Line)segment).TangentAtLength(segmentLength);
                            break;
                        case "Arc":
                            tangent = ((Arc)segment).TangentAtLength(segmentLength);
                            break;
                        case "PolyCurve":
                            tangent = ((PolyCurve)segment).TangentAtLength(segmentLength);
                            break;
                    }
                    return tangent.Unitize();
                }
            }
            return new Vector3();
        }

        /// <summary>
        /// Returns a vector tangent at a given parameter along the polycurve [0,1].
        /// </summary>
        /// <param name="t">Parameter value between 0 and 1.</param>
        /// <returns>The vector at the given parameter.</returns>
        public Vector3 TangentAt(double t)
        {
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
                var p = new Plane();
                p.Origin = segment.PointAt(param);
  
                var type = segment.GetType().Name;
                switch (type)
                {
                    case "NurbsCurve":
                        return ((NurbsCurve)segment).TangentAt(param).Unitize();
                    case "Line":
                        return ((Line)segment).TangentAt(param).Unitize();
                    case "Arc":
                        return ((Arc)segment).TangentAt(param).Unitize();
                    case "PolyCurve":
                        return ((PolyCurve)segment).TangentAt(param).Unitize();
                }
            }
            return new Vector3();
        }

        /// <summary>
        /// It returns the closest point at a given point on the polycurve.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>The point on the curve closest to the fiven point.</returns>
        public Point3 ClosestPoint(Point3 pt)
        {
            var closest = new Point3();
            var point = new Point3();
            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            else
            {
                var d = 1e10;
                foreach (var segment in this.Segments)
                {
                    var type = segment.GetType().Name;
                    switch (type)
                    {
                        case "NurbsCurve":
                            point = ((NurbsCurve)segment).ClosestPoint(pt);
                            if (point.DistanceTo(pt) < d)
                            {
                                closest = point;
                            }
                            break;
                        case "Line":
                            point = ((Line)segment).ClosestPoint(pt);
                            if (point.DistanceTo(pt) < d)
                            {
                                closest = point;
                            }
                            break;
                        case "Arc":
                            point = ((Arc)segment).ClosestPoint(pt);
                            if (point.DistanceTo(pt) < d)
                            {
                                closest = point;
                            }
                            break;
                        case "PolyCurve":
                            point = ((PolyCurve)segment).ClosestPoint(pt);
                            if (point.DistanceTo(pt) < d)
                            {
                                closest = point;
                            }
                            break;
                    }
                    d = closest.DistanceTo(pt);
                }
            }
            return closest;
        }

        /// <summary>
        /// Returns a plane at a given parameter perpendicular to the polycurve [0,1].
        /// </summary>
        /// <param name="t">Parameter value between 0 and 1.</param>
        /// <returns>The plane at the given parameter.</returns>
        public List<Vector3> FrameAt(double t)
        {
            if (this.SegmentCount == 0)
            {
                throw new InvalidOperationException("The polycurve is empty");
            }
            else if (t > 1.0)
            {
                t = 1.0;
            }
            var param = t * SegmentCount;
            var i = int.Parse(Math.Floor(param).ToString());
            param -= i;
            var segment = this.Segments[i];
            var type = segment.GetType().Name;
            var der = new List<Vector3>();
            switch (type)
            {
                case "NurbsCurve":
                    der = Evaluation.RationalCurveDerivatives(segment, param, 2);
                    break;
                case "Line":
                    der = Evaluation.RationalCurveDerivatives(((ICurve)((Line)segment).ToNurbs()), param, 2);
                    break;
                case "Arc":
                    var tangent = ((Arc)segment).TangentAt(param).Unitize();
                    var ctr = ((Arc)segment).Center;
                    var normal = new Vector3(ctr);
                    return new List<Vector3> { ((Arc)segment).PointAt(param), tangent, normal };
                //case "PolyCurve":
                //    return ((PolyCurve)segment).FrameAt(t);
            }
            return new List<Vector3> { der[0], der[1].Unitize(), der[2].Unitize() };
        }

        /// <summary>
        /// Constructs the string representation for the current PolyCurve.
        /// </summary>
        /// <returns>The polycurve representation in the form of number of segments</returns>
        public override string ToString()
        {
            return $"PolyCurve ({this.SegmentCount})";
        }
    }
}
