using FluentAssertions;
using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class NurbsSurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;
        public NurbsSurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_NURBS_Surface_By_Four_Points()
        {
            // Arrange
            Point3 p1 = new Point3(0.0, 0.0, 0.0);
            Point3 p2 = new Point3(10.0, 0.0, 0.0);
            Point3 p3 = new Point3(10.0, 10.0, 2.0);
            Point3 p4 = new Point3(0.0, 10.0, 4.0);

            Point3 expectedPt = new Point3(5.0, 5.0, 1.5);

            // Act
            NurbsSurface surfaceCcw = NurbsSurface.FromCorners(p1, p2, p3, p4);
            NurbsSurface surfaceCw = NurbsSurface.FromCorners(p1, p4, p3, p2);
            Point3 evalPtCcw = new Point3(surfaceCcw.PointAt(0.5, 0.5));
            Point3 evalPtCw = new Point3(surfaceCw.PointAt(0.5, 0.5));

            // Assert
            surfaceCcw.Should().NotBeNull();
            surfaceCcw.ControlPointLocations.Count.Should().Be(2);
            surfaceCcw.ControlPointLocations[0].Count.Should().Be(2);
            surfaceCcw.ControlPointLocations[0][1].Equals(p4).Should().BeTrue();
            (evalPtCcw.EpsilonEquals(expectedPt, GSharkMath.MinTolerance) && evalPtCw.EpsilonEquals(expectedPt, GSharkMath.MinTolerance)).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { -0.020397, -0.392974, 0.919323 })]
        [InlineData(0.5, 0.5, new double[] { 0.091372, -0.395944, 0.913717 })]
        [InlineData(1.0, 1.0, new double[] { 0.507093, -0.169031, 0.845154 })]
        public void It_Returns_The_Surface_Normal_At_A_Given_U_And_V_Parameter(double u, double v, double[] pt)
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedNormal = new Vector3(pt[0], pt[1], pt[2]);

            // Act
            Vector3 normal = surface.EvaluateAt(u, v, EvaluateSurfaceDirection.Normal);

            // Assert
            normal.EpsilonEquals(expectedNormal, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Evaluated_Surface_At_A_Given_U_And_V_Parameter()
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedUDirection = new Vector3(0.985802, 0.152837, 0.069541);
            Vector3 expectedVDirection = new Vector3(0.053937, 0.911792, 0.407096);

            // Act
            Vector3 uDirection = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.U);
            Vector3 vDirection = surface.EvaluateAt(0.3, 0.5, EvaluateSurfaceDirection.V);

            // Assert
            uDirection.EpsilonEquals(expectedUDirection, GSharkMath.MinTolerance).Should().BeTrue();
            vDirection.EpsilonEquals(expectedVDirection, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 1.901998, 16.685193, 5.913446 })]
        [InlineData(0.1, 0.1, new double[] { -15.044280, 3.808873, 0.968338 })]
        [InlineData(1.0, 1.0, new double[] { 5, 35, 0 })]
        public void It_Returns_A_Normal_Lofted_Surface_By_Opened_Curves(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.FromLoft(NurbsCurveCollection.OpenCurves());
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 0.625, 17.5, 6.59375 })]
        [InlineData(0.1, 0.1, new double[] { -14.7514, 3.14, 1.63251 })]
        [InlineData(1.0, 1.0, new double[] { 5, 35, 0 })]
        public void It_Returns_A_Loose_Lofted_Surface_By_Opened_Curves(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.FromLoft(NurbsCurveCollection.OpenCurves(), LoftType.Loose);
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.5, 0.5, new double[] { 8.515625, 17.5, 1.890625 })]
        [InlineData(0.1, 0.1, new double[] { -3.9403, 3.14, 6.446595 })]
        [InlineData(1.0, 1.0, new double[] { -2.5, 35, 9 })]
        public void It_Returns_A_Loose_Lofted_Surface_By_Closed_Curves(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);

            // Act
            NurbsSurface surface = NurbsSurface.FromLoft(NurbsCurveCollection.ClosedCurves(), LoftType.Loose);
            Point3 evalPt = surface.PointAt(u, v);

            // Assert
            surface.Should().NotBeNull();
            evalPt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_The_Curves_Are_Null()
        {
            // Act
            Func<NurbsSurface> func = () => NurbsSurface.FromLoft(null);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_There_Are_Null_Curves()
        {
            // Arrange
            List<NurbsBase> crvs = NurbsCurveCollection.OpenCurves();
            crvs.Add(null);

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.FromLoft(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("The input set contains null curves.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_Curves_Count_Are_Less_Than_Two()
        {
            // Arrange
            NurbsBase[] crvs = { NurbsCurveCollection.OpenCurves()[0] };

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.FromLoft(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("An invalid number of curves to perform the loft.");
        }

        [Fact]
        public void Lofted_Surface_Throws_An_Exception_If_The_All_Curves_Are_Not_Closed_Or_Open()
        {
            // Arrange
            List<NurbsBase> crvs = NurbsCurveCollection.OpenCurves();
            crvs[1] = crvs[1].Close();

            // Act
            Func<NurbsSurface> func = () => NurbsSurface.FromLoft(crvs);

            // Assert
            func.Should().Throw<Exception>()
                         .WithMessage("Loft only works if all curves are open, or all curves are closed.");
        }

        [Theory]
        [InlineData(new double[] { 2.60009, 7.69754, 3.408162 }, new double[] { 2.5, 7, 5 })]
        [InlineData(new double[] { 2.511373, 1.994265, 0.887211 }, new double[] { 2.5, 1.5, 2 })]
        [InlineData(new double[] { 8.952827, 2.572942, 0.735217 }, new double[] { 9, 2.5, 1 })]
        [InlineData(new double[] { 5.073733, 4.577509, 1.978153 }, new double[] { 5, 5, 1 })]
        public void Returns_The_Closest_Point_On_The_Surface(double[] expectedPt, double[] testPt)
        {
            // Arrange
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Point3 pt = new Point3(testPt[0], testPt[1], testPt[2]);
            Point3 expectedClosestPt = new Point3(expectedPt[0], expectedPt[1], expectedPt[2]);

            // Act
            Point3 closestPt = surface.ClosestPoint(pt);

            // Assert
            closestPt.DistanceTo(expectedClosestPt).Should().BeLessThan(GSharkMath.MaxTolerance);
        }

        [Fact]
        public void Returns_True_If_Two_Surfaces_Are_Equals()
        {
            // Arrange
            NurbsSurface surface0 = NurbsSurfaceCollection.SurfaceFromPoints();
            NurbsSurface surface1 = NurbsSurfaceCollection.SurfaceFromPoints();

            // Assert
            surface0.Equals(surface1).Should().BeTrue();
        }

        [Fact]
        public void Returns_A_Reversed_Surface_In_The_U_Direction()
        {
            // Arrange
            List<List<Point3>> expectedPts = new List<List<Point3>>
            {
                new List<Point3>{ new Point3(10.0, 0.0, 0.0), new Point3(10.0, 10.0, 2.0)},
                new List<Point3>{ new Point3(5.0, 0.0, 0.0), new Point3(5.0,10.0,5.0)},
                new List<Point3>{ new Point3(0.0, 0.0, 0.0), new Point3(0.0, 10.0, 4.0)}
            };

            // Act
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints().Reverse(SurfaceDirection.U);

            // Assert
            surface.ControlPointLocations
                .Zip(expectedPts, (ptsA, ptsB) => ptsA.SequenceEqual(ptsB))
                .All(res => res)
                .Should().BeTrue();
        }

        [Fact]
        public void Returns_A_Reversed_Surface_In_The_V_Direction()
        {
            // Arrange
            List<List<Point3>> expectedPts = new List<List<Point3>>
            {
                new List<Point3>{ new Point3(0.0, 10.0, 4.0), new Point3(0.0, 0.0, 0.0)},
                new List<Point3>{ new Point3(5.0, 10.0, 5.0), new Point3(5.0,0.0,0.0)},
                new List<Point3>{ new Point3(10.0, 10.0, 2.0), new Point3(10.0, 0.0, 0.0)}
            };

            // Act
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints().Reverse(SurfaceDirection.V);

            // Assert
            surface.ControlPointLocations
                .Zip(expectedPts, (ptsA, ptsB) => ptsA.SequenceEqual(ptsB))
                .All(res => res)
                .Should().BeTrue();
        }

        [Fact]
        public void Returns_True_If_Surface_Is_Closed()
        {
            // Act
            NurbsSurface surface = NurbsSurface.FromLoft(NurbsCurveCollection.ClosedCurves(), LoftType.Loose);

            // Assert
            surface.IsClosed(SurfaceDirection.V).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { 0.2655, 1, 2.442 })]
        [InlineData(0.5, 0.5, new double[] { 4.0625, 5, 4.0625 })]
        [InlineData(1.0, 1.0, new double[] { 10, 10, 0 })]
        public void Returns_A_Ruled_Surface_Between_Two_Nurbs_Curve(double u, double v, double[] pt1)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt1[0], pt1[1], pt1[2]);
            List<Point3> ptsA = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(0, 0, 5),
                new Point3(5, 0, 5),
                new Point3(5, 0, 0),
                new Point3(10, 0, 0)
            };

            List<Point3> ptsB = new List<Point3>
            {
                new Point3(0, 10, 0),
                new Point3(0, 10, 5),
                new Point3(5, 10, 5),
                new Point3(5, 10, 0),
                new Point3(10, 10, 0)
            };

            NurbsCurve curveA = new NurbsCurve(ptsA, 3);
            NurbsCurve curveB = new NurbsCurve(ptsB, 2);

            // Act
            NurbsSurface ruledSurface = NurbsSurface.Ruled(curveA, curveB);
            Point3 pointAt = ruledSurface.PointAt(u, v);

            // Assert
            pointAt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { 0.0225, 1, 2.055 })]
        [InlineData(0.5, 0.5, new double[] { 4.6875, 5, 4.6875 })]
        [InlineData(1.0, 1.0, new double[] { 10, 10, 0 })]
        public void Returns_A_Ruled_Surface_Between_A_Polyline_And_A_Nurbs_Curve(double u, double v, double[] pt)
        {
            // Arrange
            Point3 expectedPt = new Point3(pt[0], pt[1], pt[2]);
            List<Point3> ptsA = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(0, 0, 5),
                new Point3(5, 0, 5),
                new Point3(5, 0, 0),
                new Point3(10, 0, 0)
            };

            List<Point3> ptsB = new List<Point3>
            {
                new Point3(0, 10, 0),
                new Point3(0, 10, 5),
                new Point3(5, 10, 5),
                new Point3(5, 10, 0),
                new Point3(10, 10, 0)
            };

            PolyLine poly = new PolyLine(ptsA);
            NurbsCurve curveB = new NurbsCurve(ptsB, 2);

            // Act
            NurbsSurface ruledSurface = NurbsSurface.Ruled(poly, curveB);
            Point3 pointAt = ruledSurface.PointAt(u, v);

            // Assert
            pointAt.EpsilonEquals(expectedPt, GSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Revolved_Surface_From_A_Line()
        {
            // Arrange
            Ray axis = new Ray(Point3.Origin, Vector3.ZAxis);
            Line profile = new Line(new Point3(1, 0, 0), new Point3(0, 0, 1));

            List<List<Point3>> expectedPts0 = new List<List<Point3>>
            {
                new List<Point3>
                    {
                        new Point3(1, 0, 0),
                        new Point3(0, 0, 1)
                    },
                new List<Point3>
                {
                    new Point3(1, 0.41421356237309526, 0),
                    new Point3(0, 0, 1)
                },
                new List<Point3>
                {
                    new Point3(0.7071067811865476,0.7071067811865476,0),
                    new Point3(0,0,1)
                }
            };
            List<List<Point3>> expectedPts1 = new List<List<Point3>>
            {
                new List<Point3>
                {
                    new Point3(1, 0, 0),
                    new Point3(0, 0, 1)
                },
                new List<Point3>
                {
                    new Point3(1, 1, 0),
                    new Point3(0, 0, 1)
                },
                new List<Point3>
                {
                    new Point3(0,1,0),
                    new Point3(0,0,1)
                },
                new List<Point3>
                {
                    new Point3(-1,1,0),
                    new Point3(0,0,1)
                },
                new List<Point3>
                {
                    new Point3(-1,0,0),
                    new Point3(0,0,1)
                },
                new List<Point3>
                {
                    new Point3(-1,-1,0),
                    new Point3(0,0,1)
                },
                new List<Point3>
                {
                    new Point3(0,-1,0),
                    new Point3(0,0,1)
                },
                new List<Point3>
                {
                    new Point3(1,-1,0),
                    new Point3(0,0,1)
                },
                new List<Point3>
                {
                    new Point3(1, 0, 0),
                    new Point3(0, 0, 1)
                }
            };

            // Act
            NurbsSurface revolvedSurface0 = NurbsSurface.Revolved(profile, axis, Math.PI * 0.25);
            NurbsSurface revolvedSurface1 = NurbsSurface.Revolved(profile, axis, 2 * Math.PI);

            // Assert
            revolvedSurface0.ControlPointLocations
                .Zip(expectedPts0, (ptsA, ptsB) => ptsA.SequenceEqual(ptsB))
                .All(res => res)
                .Should().BeTrue();

            revolvedSurface1.ControlPointLocations
                .Zip(expectedPts1, (ptsA, ptsB) => ptsA.SequenceEqual(ptsB))
                .All(res => res)
                .Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Revolved_Surface_From_An_Arc()
        {
            // Arrange
            Ray axis = new Ray(Point3.Origin, Vector3.ZAxis);
            Arc profile = new Arc(Plane.PlaneZX, 1, new Interval(0, Math.PI * 0.5));

            List<List<Point3>> expectedPts = new List<List<Point3>>
            {
                new List<Point3>
                    {
                        new Point3(0, 0, 1),
                        new Point3(1, 0, 1),
                        new Point3(1, 0, 0)
                    },
                new List<Point3>
                {
                    new Point3(0, 0, 1),
                    new Point3(1, 0.41421356237309526, 1),
                    new Point3(1, 0.41421356237309526, 0)
                },
                new List<Point3>
                {
                    new Point3(0,0,1),
                    new Point3(0.7071067811865476,0.7071067811865476,1),
                    new Point3(0.7071067811865476,0.7071067811865476,0)
                }
            };

            // Act
            NurbsSurface revolvedSurface = NurbsSurface.Revolved(profile, axis, Math.PI * 0.25);

            // Assert
            revolvedSurface.ControlPointLocations
                .Zip(expectedPts, (ptsA, ptsB) => ptsA.SequenceEqual(ptsB))
                .All(res => res)
                .Should().BeTrue();
        }

        [Fact]
        public void It_Creates_A_NurbsSurface_From_Extrusion()
        {
            // Arrange
            List<Point3> ptsA = new List<Point3>
            {
                new Point3(0, 0, 0),
                new Point3(0, 0, 5),
                new Point3(5, 0, 5),
                new Point3(5, 0, 0),
                new Point3(10, 0, 0)
            };
            List<Point3> ptsB = new List<Point3>
            {
                new Point3(0, 10, 0),
                new Point3(0, 10, 5),
                new Point3(5, 10, 5),
                new Point3(5, 10, 0),
                new Point3(10, 10, 0)
            };
            NurbsCurve curve = new NurbsCurve(ptsA, 3);
            Vector3 direction = Vector3.YAxis * 10;
            List<List<Point3>> expectedPts = new List<List<Point3>> {ptsA, ptsB};

            // Act
            NurbsSurface extrudedSurface = NurbsSurface.FromExtrusion(direction, curve);

            // Assert
            extrudedSurface.ControlPointLocations
                .Zip(expectedPts, (ptsA, ptsB) => ptsA.SequenceEqual(ptsB))
                .All(res => res)
                .Should().BeTrue();
            extrudedSurface.DegreeU.Should().Be(1);
            extrudedSurface.DegreeV.Should().Be(curve.Degree);
        }

        [Fact]
        public void It_Creates_A_NurbsSurface_From_Swep()
        {
            // Arrange
            List<Point3> ptsA = new List<Point3>
            {
                new Point3(5, 0, 0),
                new Point3(5, 5, 0),
                new Point3(0, 5, 0),
                new Point3(0, 5, 5),
                new Point3(5, 5, 5)
            };
            List<Point3> ptsB = new List<Point3>
            {
                new Point3(4, 0, 1),
                new Point3(5, 0, 0),
                new Point3(6, 0, 1)
            };
            NurbsCurve rail = new NurbsCurve(ptsA, 3);
            NurbsCurve profile = new NurbsCurve(ptsB, 2);

            // Act
            NurbsSurface sweepSurface = NurbsSurface.FromSweep(rail, profile);

            // Assert
            (sweepSurface.ControlPointLocations.Last()[1] == ptsA.Last()).Should().BeTrue();
        }
    }
}