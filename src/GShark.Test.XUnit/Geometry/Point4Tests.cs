using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class Point4Tests
    {
        private readonly ITestOutputHelper _testOutput;

        public Point4Tests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> ControlPoints1D => new List<object[]>
        {
            new object[]
            {
                new List<double> {0.5, 0.5, 0.5},
                new List<Point4>
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (1.25, -1.25, 0.0, 0.5),
                    new (2.5, 0.0, 0.0, 0.5)
                }
            },

            new object[]
            {
                new List<double> {0.5},
                new List<Point4>
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (2.5, -2.5, 0.0, 1.0),
                    new (5.0, 0.0, 0.0, 1.0)
                }
            },
        };
        public static IEnumerable<object[]> ControlPoints2D => new List<object[]>
        {
            new object[]
            {
                new List<List<double>> {
                    new() { 0.5, 0.5, 0.5 },
                    new() { 0.5, 0.5, 0.5 },
                },
                new List<List<Point4>>()
                {
                    new()
                    {
                        new (0.0, 0.0, 0.0, 0.5),
                        new (1.25, -1.25, 0.0, 0.5),
                        new (2.5, 0.0, 0.0, 0.5)
                    },
                    new()
                    {
                        new (0.0, 0.0, 0.0, 0.5),
                        new (1.25, -1.25, 0.0, 0.5),
                        new (2.5, 0.0, 0.0, 0.5)
                    }
                }
            },
            new object[]
            {
                new List<List<double>> { new() { 0.5 }, new() { 0.5 }},
                new List<List<Point4>>
                {
                    new()
                    {
                        new (0.0, 0.0, 0.0, 0.5),
                        new (2.5, -2.5, 0.0, 1.0),
                        new (5.0, 0.0, 0.0, 1.0)
                    },
                    new()
                    {
                        new (0.0, 0.0, 0.0, 0.5),
                        new (2.5, -2.5, 0.0, 1.0),
                        new (5.0, 0.0, 0.0, 1.0)
                    }
                }
            },

            new object[]
            {
                null,
                new List<List<Point4>>
                {
                    new()
                    {
                        new (0.0, 0.0, 0.0, 1.0),
                        new (2.5, -2.5, 0.0, 1.0),
                        new (5.0, 0.0, 0.0, 1.0)
                    },
                    new()
                    {
                        new (0.0, 0.0, 0.0, 1.0),
                        new (2.5, -2.5, 0.0, 1.0),
                        new (5.0, 0.0, 0.0, 1.0)
                    }
                }
            }
        };

        [Fact]
        public void It_Returns_A_Transformed_Point()
        {
            //Arrange
            var testPoint = new Point4(5, 10, 0, 0.5);
            var expectedPoint = new Point4(15, 0, 0, 0.5);
            var translation = Transform.Translation(new Vector3(10, 0, 0));

            //Act
            var xFormedPoint = testPoint.Transform(translation);

            //Assert
            xFormedPoint.EpsilonEquals(expectedPoint, GeoSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Returns_True_If_Two_Points_Are_Equal()
        {
            // Arrange
            var p1 = new Point4(5.982099, 5.950299, 0, 1);
            var p2 = new Point4(5.982099, 5.950299, 0, 1);

            // Assert
            (p1 == p2).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(ControlPoints1D))]
        public void It_Returns_A_New_Set_Of_Control_Points(List<double> weights, List<Point4> expectedControlPoints)
        {
            // Arrange
            List<Point3> controlPts = new List<Point3>
            {
                new (0.0, 0.0, 0),
                new (2.5, -2.5, 0),
                new (5.0, 0.0, 0)
            };

            // Act
            List<Point4> newControlPts = Point4.PointsHomogeniser(controlPts, weights);

            // Assert
            newControlPts.SequenceEqual(expectedControlPoints).Should().BeTrue();
        }

        [Fact]
        public void PointsHomogeniser_Throws_An_Exception_If_The_Weight_Collection_Is_Bigger_Than_ControlPts()
        {
            // Arrange
            List<Point3> controlPts = new List<Point3>();
            List<double> weights = new List<double> { 1.0, 1.5, 1.0 };

            // Act
            Func<object> resultFunction = () => Point4.PointsHomogeniser(controlPts, weights);

            // Assert
            resultFunction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void It_Returns_A_New_Set_Of_Control_Points_By_A_Given_Weight_Value()
        {
            // Arrange
            List<Point3> pts = new List<Point3>
            {
                new (0.0, 0.0, 0),
                new (2.5, -2.5, 0),
                new (5.0, 0.0, 0)
            };
            List<Point4> expectedControlPoints = new List<Point4>
            {
                new (0.0, 0.0, 0.0, 1.0),
                new (2.5, -2.5, 0.0, 1.0),
                new (5.0, 0.0, 0.0, 1.0)
            };

            // Act
            List<Point4> controlPts = Point4.PointsHomogeniser(pts, 1.0);

            // Assert
            controlPts.SequenceEqual(expectedControlPoints).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(ControlPoints2D))]
        public void It_Returns_A_Set_Of_Control_Points(List<List<double>> weights, List<List<Point4>> expectedSetOfCtrlPts)
        {
            // Arrange
            List<List<Point3>> setOfPts = new List<List<Point3>>()
            {
                new()
                {
                    new (0.0, 0.0, 0),
                    new (2.5, -2.5, 0),
                    new (5.0, 0.0, 0)
                },
                new()
                {
                    new (0.0, 0.0, 0),
                    new (2.5, -2.5, 0),
                    new (5.0, 0.0, 0)
                }
            };

            // Act
            List<List<Point4>> setOfCtrlPts = Point4.PointsHomogeniser2d(setOfPts, weights);

            // Assert
            for (int i = 0; i < setOfCtrlPts.Count; i++)
            {
                List<Point4> ctrlPts = setOfCtrlPts[i];
                List<Point4> expectedCtrlPts = expectedSetOfCtrlPts[i];

                for (int j = 0; j < ctrlPts.Count; j++)
                {
                    Point4 ctrlPt = ctrlPts[j];
                    Point4 expectedPt = expectedCtrlPts[j];
                    ctrlPt.Equals(expectedPt).Should().BeTrue();
                }
            }
        }

        [Fact]
        public void It_Returns_A_Set_Of_Weights()
        {
            // Arrange
            List<Point4> ctrlPts = new List<Point4>
            {
                new (0.0, 0.0, 0.0, 0.5),
                new (1.25, -1.25, 0.0, 0.5),
                new (2.5, 0.0, 0.0, 0.5)
            };
            List<double> expectedWeights = new List<double> { 0.5, 0.5, 0.5 };

            // Act
            List<double> weight1d = Point4.GetWeights(ctrlPts);

            // Assert
            weight1d.SequenceEqual(expectedWeights).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Two_Dimensional_Set_Of_Weights()
        {
            // Arrange
            List<List<Point4>> ctrlPts = new List<List<Point4>>
            {
                new()
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (1.25, -1.25, 0.0, 0.5),
                    new (2.5, 0.0, 0.0, 0.5)
                },
                new()
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (1.25, -1.25, 0.0, 0.5),
                    new (2.5, 0.0, 0.0, 0.5)
                }
            };

            List<List<double>> expectedWeights = new List<List<double>>
            {
                new() {0.5, 0.5, 0.5},
                new() {0.5, 0.5, 0.5}
            };

            // Act
            List<List<double>> weight2d = Point4.GetWeights2d(ctrlPts);

            // Assert
            for (int i = 0; i < ctrlPts.Count; i++)
            {
                var expectedWts = expectedWeights[i];
                var weights = weight2d[i];

                for (int j = 0; j < expectedWts.Count; j++)
                {
                    var expectedWt = expectedWts[j];
                    var weight = weights[j];
                    weight.Equals(expectedWt).Should().BeTrue();
                }
            }
        }

        [Fact]
        public void It_Returns_A_Dehomogenized_Point()
        {
            // Arrange
            Point4 ctrlPt = new Point4(1.25, -1.25, 0.0, 0.5);
            Point3 expectedPt = new Point3(2.5, -2.5, 0);

            // Act
            Point3 pt = Point4.PointDehomogenizer(ctrlPt);

            // Assert
            pt.Equals(expectedPt).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Set_Of_Dehomogenized_Points()
        {
            // Arrange
            List<Point4> ctrlPts = new List<Point4>
            {
                new (0.0, 0.0, 0.0, 0.5),
                new (1.25, -1.25, 0.0, 0.5),
                new (2.5, 0.0, 0.0, 0.5)
            };

            List<Point3> expectedPts = new List<Point3>
            {
                new (0.0, 0.0, 0),
                new (2.5, -2.5, 0),
                new (5.0, 0.0, 0)
            };

            // Act
            List<Point3> pts = Point4.PointDehomogenizer1d(ctrlPts);

            // Assert
            pts.SequenceEqual(expectedPts).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Two_Dimensional_Set_Of_Dehomogenized_Points()
        {
            // Arrange
            List<List<Point4>> ctrlPts = new List<List<Point4>>
            {
                new()
                {
                    new(0.0, 0.0, 0.0, 0.5),
                    new(1.25, -1.25, 0.0, 0.5),
                    new(2.5, 0.0, 0.0, 0.5)
                },
                new()
                {
                    new(0.0, 0.0, 0.0, 0.5),
                    new(1.25, -1.25, 0.0, 0.5),
                    new(2.5, 0.0, 0.0, 0.5)
                }
            };

            List<List<Point3>> expectedPts = new List<List<Point3>>
            {
               new()
               {
                   new (0.0, 0.0, 0),
                   new (2.5, -2.5, 0),
                   new (5.0, 0.0, 0)
               },
               new()
               {
                   new (0.0, 0.0, 0),
                   new (2.5, -2.5, 0),
                   new (5.0, 0.0, 0)
               }
            };

            // Act
            var pts = Point4.PointDehomogenizer2d(ctrlPts);

            // Arrange
            pts.Should().BeEquivalentTo(expectedPts);
        }

        [Fact]
        public void It_Returns_A_Rationalized_Set_Of_Points()
        {
            // Arrange
            List<Point4> ctrlPts = new List<Point4>
            {
                new (0.0, 0.0, 0.0, 0.5),
                new (2.5, -2.5, 0.0, 1.0),
                new (5.0, 0.0, 0.0, 1.0)
            };

            List<Point3> expectedPts = new List<Point3>
            {
                new (0.0, 0.0, 0.0),
                new (2.5, -2.5, 0.0),
                new (5.0, 0.0, 0.0)
            };

            // Act
            List<Point3> rationalPoints = Point4.RationalPoints(ctrlPts);

            // Assert
            rationalPoints.SequenceEqual(expectedPts).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Rationalized_Two_Dimensional_Set_Of_Points()
        {
            // Arrange
            List<List<Point4>> ctrlPts = new List<List<Point4>>
            {
                new()
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (2.5, -2.5, 0.0, 1.0),
                    new (5.0, 0.0, 0.0, 1.0)
                },
                new()
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (2.5, -2.5, 0.0, 1.0),
                    new (5.0, 0.0, 0.0, 1.0)
                }
            };

            List<List<Point3>> expectedPoints = new List<List<Point3>>
            {
                new()
                {
                    new (0.0, 0.0, 0.0),
                    new (2.5, -2.5, 0.0),
                    new (5.0, 0.0, 0.0)
                },
                new()
                {
                    new (0.0, 0.0, 0.0),
                    new (2.5, -2.5, 0.0),
                    new (5.0, 0.0, 0.0)
                }
            };

            // Act
            List<List<Point3>> rationalPoints = Point4.Rational2d(ctrlPts);

            // Assert
            for (int i = 0; i < ctrlPts.Count; i++)
            {
                var expectedPts = expectedPoints[i];
                var rationalPts = rationalPoints[i];

                for (int j = 0; j < expectedPoints.Count; j++)
                {
                    var expectedPt = expectedPts[j];
                    var rationalPt = rationalPts[j];
                    rationalPt.Equals(expectedPt).Should().BeTrue();
                }
            }
        }
    }
}
