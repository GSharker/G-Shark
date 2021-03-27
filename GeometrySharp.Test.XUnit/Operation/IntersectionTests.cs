using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using GeometrySharp.Operation;
using verb.core;
using Xunit;
using Xunit.Abstractions;
using Plane = GeometrySharp.Geometry.Plane;
using Ray = GeometrySharp.Geometry.Ray;

namespace GeometrySharp.Test.XUnit.Operation
{
    public class IntersectionTests
    {
        private readonly ITestOutputHelper _testOutput;
        public IntersectionTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_The_Intersection_Between_Two_Planes()
        {
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneYZ.SetOrigin(new Vector3{10,10,5});
            Plane pl2 = Plane.PlaneXZ.SetOrigin(new Vector3 {10, -10, -5});

            bool intersection0 = Intersect.PlanePlane(pl0, pl1, out Line lineIntersect0);
            bool intersection1 = Intersect.PlanePlane(pl1, pl2, out Line lineIntersect1);

            intersection0.Should().BeTrue();
            lineIntersect0.Start.Should().BeEquivalentTo(new Vector3 {10, 0, 0});
            lineIntersect0.Direction.Should().BeEquivalentTo(new Vector3 { 0, 1, 0 });

            intersection1.Should().BeTrue();
            lineIntersect1.Start.Should().BeEquivalentTo(new Vector3 { 10, -10, 0 });
            lineIntersect1.Direction.Should().BeEquivalentTo(new Vector3 { 0, 0, 1 });
        }

        [Fact]
        public void PlanePlane_Is_False_If_Planes_Are_Parallel()
        {
            Plane pl0 = Plane.PlaneXY;
            Plane pl1 = Plane.PlaneXY.SetOrigin(new Vector3 { 10, 10, 5 });

            bool intersection = Intersect.PlanePlane(pl0, pl1, out _);

            intersection.Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Intersection_Point_Between_A_Segment_And_A_Plane()
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Line ln0 = new Line(new Vector3 { 0, 0, 0 }, new Vector3 { 20, 20, 10 });
            Line ln1 = new Line(new Vector3 { 0, 0, 0 }, new Vector3 { 5, 5, 10 });

            bool intersection0 = Intersect.LinePlane(ln0, pl, out Vector3 pt0, out _);
            bool intersection1 = Intersect.LinePlane(ln1, pl, out Vector3 pt1, out _);

            intersection0.Should().BeTrue();
            intersection1.Should().BeTrue();
            pt0.Equals(new Vector3 {10, 10, 5}).Should().BeTrue();
            pt1.Equals(new Vector3 { 10, 10, 20 }).Should().BeTrue();
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 10, 0 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 0, 10 })]
        [InlineData(new double[] { 10, 20, 5 }, new double[] { 10, 20, 10 })]
        public void LinePlane_Is_False_If_Line_Is_Parallel(double[] startPt, double[] endPt)
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Line ln = new Line(new Vector3 (startPt), new Vector3(endPt));

            bool intersection = Intersect.LinePlane(ln, pl, out _, out _);

            intersection.Should().BeFalse();
        }

        [Theory]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 3, 3, 2 }, new double[] { 4.565217, 5, 4.782609 }, new double[] { 5.217391, 5.217391, 3.478261 })]
        [InlineData(new double[] { 0, 0, 0 }, new double[] { 0, 20, 0 }, new double[] { -1, 5, 2 }, new double[] { 0, 5, 0 })]
        [InlineData(new double[] { -5, 5, 0 }, new double[] { 5, 5, 0 }, new double[] { -5, 5, 0 }, new double[] { -5, 5, 0 })]
        public void It_Returns_The_Intersection_Points_Between_Segments(double[] startPt, double[] endPt, double[] output0, double[] output1)
        {
            Line ln0 = new Line(new Vector3 { -5, 5, 0 }, new Vector3 { 5, 5, 5 });
            Line ln1 = new Line(new Vector3(startPt), new Vector3(endPt));
            Vector3 ptCheck0 = new Vector3(output0);
            Vector3 ptCheck1 = new Vector3(output1);

            bool intersection = Intersect.LineLine(ln0, ln1, out Vector3 pt0, out Vector3 pt1, out _, out _);

            pt0.IsEqualRoundingDecimal(ptCheck0, 6).Should().BeTrue();
            pt1.IsEqualRoundingDecimal(ptCheck1, 6).Should().BeTrue();
        }

        [Fact]
        public void LineLine_Is_False_If_Segments_Are_Parallel()
        {
            Line ln0 = new Line(new Vector3 { 5, 0, 0 }, new Vector3 { 5, 5, 0 });
            Line ln1 = new Line(new Vector3 { 0, 0, 0 }, new Vector3 { 0, 5, 0 });

            bool intersection = Intersect.LineLine(ln0, ln1, out _, out _, out _, out _);

            intersection.Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Intersection_Points_Between_A_Polyline_And_A_Plane()
        {
            Vector3[] pts = new[] { 
                new Vector3 { -1.673787, -0.235355, 14.436008 }, 
                new Vector3 { 13.145523, 6.066452, 0 }, 
                new Vector3 { 2.328185, 22.89864, 0 },
                new Vector3 { 18.154088, 30.745098, 7.561387 }, 
                new Vector3 { 18.154088, 12.309505, 7.561387 }};

            Vector3[] intersectionChecks = new[] {
                new Vector3 { 10, 4.728841, 3.064164 },
                new Vector3 { 10, 10.961005, 0 },
                new Vector3 { 10, 26.702314, 3.665482 }};

            Polyline poly = new Polyline(pts);
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });

            List<Vector3> intersections = Intersect.PolylinePlane(poly, pl);

            intersections.Count.Should().Be(intersectionChecks.Length);
            for (int i = 0; i < intersectionChecks.Length; i++)
            {
                intersections[i].IsEqualRoundingDecimal(intersectionChecks[i], 6).Should().BeTrue();
            }
        }

        [Fact]
        public void It_Returns_The_Intersection_Points_Between_A_Circle_And_A_Line()
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Circle cl = new Circle(pl, 20);
            Line ln0 = new Line(new Vector3 { 10, 29.769674, 17.028815 }, new Vector3 { 10, 37.594559, 24.680781 });
            Line ln1 = new Line(new Vector3 { 10, 40, 25 }, new Vector3 { 10, 40, 17 });

            Vector3[] intersectionChecks = new[] {
                new Vector3 { 10, 33.00596, 20.193584 },
                new Vector3 { 10, 4.51962, -7.663248 }};

            Vector3 intersectionCheck = new Vector3 {10, 40, 5};

            bool intersection0 = Intersect.LineCircle(cl, ln0, out Vector3[] pts0);
            bool intersection1 = Intersect.LineCircle(cl, ln1, out Vector3[] pts1);

            for (int i = 0; i < intersectionChecks.Length; i++)
            {
                pts0[i].IsEqualRoundingDecimal(intersectionChecks[i], 6).Should().BeTrue();
            }

            pts1[0].IsEqualRoundingDecimal(intersectionCheck, 6).Should().BeTrue();
        }

        [Fact]
        public void CircleLine_Intersection_Returns_False_If_No_Intersections_Are_Computed()
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Circle cl = new Circle(pl, 20);
            Line ln = new Line(new Vector3 { -15, 45, 17 }, new Vector3 { 15, 45, 25 });

            bool intersection = Intersect.LineCircle(cl, ln, out Vector3[] pts);

            intersection.Should().BeFalse();
        }

        [Fact]
        public void PlaneCircle_Is_False_If_Planes_Are_Parallel_Or_Intersection_Plane_Misses_The_Circle()
        {
            Plane pl0 = Plane.PlaneYZ;
            Plane pl1 = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Plane pl2 = Plane.PlaneXZ.SetOrigin(new Vector3 { 10, -10, -5 });
            Circle cl = new Circle(pl1, 20);

            bool intersection0 = Intersect.PlaneCircle(pl0, cl, out _);
            bool intersection1 = Intersect.PlaneCircle(pl2, cl, out _);

            intersection0.Should().BeFalse();
            intersection1.Should().BeFalse();
        }

        [Fact]
        public void It_Returns_The_Intersection_Points_Between_A_Plane_And_A_Circle()
        {
            Plane pl = Plane.PlaneYZ.SetOrigin(new Vector3 { 10, 20, 5 });
            Circle cl = new Circle(pl, 20);
            Plane plSec = new Plane(new Vector3 {10, 10, 10}, new Vector3 {10, 20, 25});

            Vector3[] intersectionChecks = new[] {
                new Vector3 { 10, 34.04646, -9.237168 },
                new Vector3 { 10, 3.026711, 15.578632 }};

            bool intersection = Intersect.PlaneCircle(plSec, cl, out Vector3[] pts);

            for (int i = 0; i < intersectionChecks.Length; i++)
            {
                pts[i].IsEqualRoundingDecimal(intersectionChecks[i], 6).Should().BeTrue();
            }
        }

        [Fact]
        public void TestIntersection()
        {
            // Assert
            int crvDegree0 = 1;
            Knot crvKnots0 = new Knot{0,0,1,1};
            List<Vector3> crvCtrPts0 = new List<Vector3>{new Vector3{0,0,0}, new Vector3 { 2, 0, 0 } };

            int crvDegree1 = 1;
            Knot crvKnots1 = new Knot { 0, 0, 1, 1 };
            List<Vector3> crvCtrPts1 = new List<Vector3> { new Vector3 { 0.5, 0.5, 0 }, new Vector3 { 0.5, -1.5, 0 } };

            NurbsCurve crv0 = new NurbsCurve(crvDegree0, crvKnots0, crvCtrPts0);
            NurbsCurve crv1 = new NurbsCurve(crvDegree1, crvKnots1, crvCtrPts1);

            var intersection = Intersect.CurveCurve(crv0, crv1, GeoSharpMath.MAXTOLERANCE);

            _testOutput.WriteLine(intersection.ToString());
        }
    }
}
