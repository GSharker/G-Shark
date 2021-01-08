using VerbNurbsSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Evaluation
{
    //ToDO initialized the class Make
    public class Make
    {
        public Make()
        {

        }

        public static NurbsCurve Polyline(List<Vector> points)
        {
            KnotArray knots = new KnotArray() { 0.0, 0.0 };
            double lsum = 0.0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                lsum += Constants.DistanceTo(points[i], points[i + 1]);
                knots.Add(lsum);
            }
            knots.Add(lsum);
            var weights = points.Select(x => 1.0).ToList();
            points.ForEach(x => weights.Add(1.0));
            return new NurbsCurve(1, knots, Eval.Homogenize1d(points, weights));
        }

        public static NurbsCurve RationalBezierCurve(List<Vector> controlPoints, List<double> weights = null)
        {
			int degree = controlPoints.Count - 1;
			KnotArray knots = new KnotArray();
			for (int i = 0; i < degree + 1; i++)
				knots.Add(0.0);
			for (int i = 0; i < degree + 1; i++)
				knots.Add(1.0);
			if (weights == null)
				weights = Sets.RepeatData(1.0, controlPoints.Count);

			return new NurbsCurve(degree, knots, Eval.Homogenize1d(controlPoints, weights));
        }

        public static NurbsCurve ClonedCurve(NurbsCurve curve)
        {
            return new NurbsCurve(curve.Degree, curve.Knots, curve.ControlPoints);
        }


        //////////////////////////// =================================== not implemented yet ================================== ///////////////////

        /// <summary>
        /// Generate a surface by translating a profile curve along a rail curve
        /// </summary>
        /// <param name="profile">NurbCurveData Profile</param>
        /// <param name="rail">NurbCurveData Rail</param>
        /// <returns></returns>
        public static NurbsSurface RationalTranslationalSurface(NurbsCurve profile, NurbsCurve rail)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Extract the boundary curves from a surface
        /// </summary>
        /// <param name="surface"></param>
        /// <returns>an List containing 4 elements, first 2 curves in the V direction, then 2 curves in the U direction</returns>
        public static List<NurbsCurve> SurfaceBoundaryCurves(NurbsSurface surface)
        {
            throw new NotImplementedException();
        }

        public static NurbsCurve SurfaceIsocurve(NurbsSurface surface, double u, bool useV = false)
        {
            throw new NotImplementedException();
        }

        public static NurbsSurface LoftedSurface(List<NurbsCurve> curves, int degreeV = 0)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1">first point in counter-clockwise form</param>
        /// <param name="p2">second point in counter-clockwise form</param>
        /// <param name="p3">third point in counter-clockwise form</param>
        /// <param name="p4">forth point in counter-clockwise form</param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static NurbsSurface FourPointSurface(Point p1, Point p2, Point p3, Point p4, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate the control points, weights, and knots of an elliptical arc
        /// </summary>
        /// <param name="center">the center</param>
        /// <param name="xaxis">the scaled x axis</param>
        /// <param name="yaxis">the scaled y axis</param>
        /// <param name="startAngle">start angle of the ellipse arc, between 0 and 2pi, where 0 points at the xaxis</param>
        /// <param name="endAngle">end angle of the arc, between 0 and 2pi, greater than the start angle</param>
        /// <returns>a NurbsCurveData object representing a NURBS curve</returns>
        public static NurbsCurve EllipseArc(Point center, Vector xaxis, Point yaxis, double startAngle, double endAngle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate the control points, weights, and knots of an arbitrary arc
        /// (Corresponds to Algorithm A7.1 from Piegl & Tiller)
        /// </summary>
        /// <param name="center">the center of the arc</param>
        /// <param name="xaxis">the xaxis of the arc</param>
        /// <param name="yaxis">orthogonal yaxis of the arc</param>
        /// <param name="radius">radius of the arc</param>
        /// <param name="startAngle">start angle of the arc, between 0 and 2pi</param>
        /// <param name="endAngle">end angle of the arc, between 0 and 2pi, greater than the start angle</param>
        /// <returns>a NurbsCurveData object representing a NURBS curve</returns>
        public static NurbsCurve Arc(Point center, Vector xaxis, Vector yaxis, double radius, double startAngle, double endAngle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate the control points, weights, and knots of an extruded surface
        /// </summary>
        /// <param name="axis">axis of the extrusion</param>
        /// <param name="length">length of the extrusion</param>
        /// <param name="profile">a NurbsCurveData object representing a NURBS surface</param>
        /// <returns>an object with the following properties: controlPoints, weights, knots, degree</returns>
        public static NurbsSurface ExtrudedSurface(Vector axis, double length, NurbsCurve profile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate the control points, weights, and knots of a cylinder
        /// </summary>
        /// <param name="axis">normalized axis of cylinder</param>
        /// <param name="xaxis">xaxis in plane of cylinder</param>
        /// <param name="basePt">position of base of cylinder</param>
        /// <param name="height">height from base to top</param>
        /// <param name="radius">radius of the cylinder</param>
        /// <returns>an object with the following properties: controlPoints, weights, knotsU, knotsV, degreeU, degreeV</returns>
        public static NurbsSurface CylindricalSurface(Vector axis, Vector xaxis, Point basePt, double height, double radius)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="center"></param>
        /// <param name="axis"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static NurbsSurface RevolvedSurface(NurbsCurve profile, Point center, Vector axis, double theta)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate the control points, weights, and knots of a sphere
        /// </summary>
        /// <param name="center">the center of the sphere</param>
        /// <param name="axis">normalized axis of sphere</param>
        /// <param name="xaxis">vector perpendicular to axis of sphere, starting the rotation of the sphere</param>
        /// <param name="radius">radius of the sphere</param>
        /// <returns>an object with the following properties: controlPoints, weights, knotsU, knotsV, degreeU, degreeV</returns>
        public static NurbsSurface SphericalSurface(Point center, Vector axis, Vector xaxis, double radius)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate the control points, weights, and knots of a cone
        /// </summary>
        /// <param name="axis">normalized axis of cone</param>
        /// <param name="xaxis"></param>
        /// <param name="basePt">position of base of cone</param>
        /// <param name="height">height from base to tip</param>
        /// <param name="radius">radius at the base of the cone</param>
        /// <returns></returns>
        public static NurbsSurface ConicalSurface(Vector axis, Vector xaxis, Point basePt, double height, double radius)
        {
            throw new NotImplementedException();
        }

        public static NurbsCurve RationalInterpCurve(List<List<double>> points, int degree, bool homogeneousPoints = false, Point start_tangent = null, Point end_tangent = null)
        {
            throw new NotImplementedException();
        }
    }
}
