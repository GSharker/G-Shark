using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Core
{
    public class LinearAlgebraTests
    {
        private readonly ITestOutputHelper _testOutput;

        public LinearAlgebraTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> Homogenized1dData => new List<object[]>
        {
            new object[]
            {
                new List<double> {0.5, 0.5, 0.5},
                new List<Point4d>
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (1.25, -1.25, 0.0, 0.5),
                    new (2.5, 0.0, 0.0, 0.5)
                }
            },

            new object[]
            {
                new List<double> {0.5},
                new List<Point4d>
                {
                    new (0.0, 0.0, 0.0, 0.5),
                    new (2.5, -2.5, 0.0, 1.0),
                    new (5.0, 0.0, 0.0, 1.0)
                }
            },
        };

        public static IEnumerable<object[]> Homogenized2dData => new List<object[]>
        {
            new object[]
            {
                new List<List<double>> {
                    new() { 0.5, 0.5, 0.5 },
                    new() { 0.5, 0.5, 0.5 },
                },
                new List<List<Point4d>>()
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
                new List<List<Point4d>>
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
                new List<List<Point4d>>
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
        public void PointsHomogeniser_Throws_An_Exception_If_The_Weight_Collection_Is_Bigger_Than_ControlPts()
        {
            // Arrange
            List<Point3d> controlPts = new List<Point3d>();
            List<double> weights = new List<double> { 1.0, 1.5, 1.0 };

            // Act
            Func<object> resultFunction = () => LinearAlgebra.PointsHomogeniser(controlPts, weights);

            // Assert
            resultFunction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(Homogenized1dData))]
        //ToDo Should parameter be called expectedHomogenizedPoints? Confusing interchangeability between control point and homogenized point?
        public void It_Returns_A_New_Set_Of_Homogenized_Points(List<double> weights, List<Point4d> expectedHomogenizedPoints)
        {
            // Arrange
            List<Point3d> controlPts = new List<Point3d>
            {
                new (0.0, 0.0, 0),
                new (2.5, -2.5, 0),
                new (5.0, 0.0, 0)
            };

            // Act
            List<Point4d> newControlPts = LinearAlgebra.PointsHomogeniser(controlPts, weights);

            // Assert
            newControlPts.SequenceEqual(expectedHomogenizedPoints).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_New_Set_Of_Homogenized_Points_By_A_Given_Weight_Value()
        {
            // Arrange
            List<Point3d> controlPts = new List<Point3d>
            {
                new (0.0, 0.0, 0),
                new (2.5, -2.5, 0),
                new (5.0, 0.0, 0)
            };
            List<Point4d> expectedHomogenizedPoints = new List<Point4d>
            {
                new (0.0, 0.0, 0.0, 1.0),
                new (2.5, -2.5, 0.0, 1.0),
                new (5.0, 0.0, 0.0, 1.0)
            };

            // Act
            List<Point4d> homogenizedPoints = LinearAlgebra.PointsHomogeniser(controlPts, 1.0);

            // Assert
            homogenizedPoints.SequenceEqual(expectedHomogenizedPoints).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Homogenized2dData))]
        public void It_Returns_A_Set_Of_Homogenized_Control_Points(List<List<double>> weights, List<List<Point4d>> expectedHomogenizedPoints)
        {
            // Arrange
            List<List<Point3d>> controlPts = new List<List<Point3d>>()
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
            List<List<Point4d>> homogeneousPoints = LinearAlgebra.PointsHomogeniser2d(controlPts, weights);

            // Assert
            for (int i = 0; i < homogeneousPoints.Count; i++)
            {
                var homogenousPts = homogeneousPoints[i];
                var expectedPoints = expectedHomogenizedPoints[i];

                for (int j = 0; j < homogenousPts.Count; j++)
                {
                    var homogenousPt = homogenousPts[j];
                    var expectedPt = expectedPoints[j];
                    homogenousPt.Equals(expectedPt).Should().BeTrue();
                }
            }
        }

        [Fact]
        public void It_Returns_A_Set_Of_Weights()
        {
            // Arrange
            List<Point4d> homogeneousPts = new List<Point4d>
            {
                new (0.0, 0.0, 0.0, 0.5),
                new (1.25, -1.25, 0.0, 0.5),
                new (2.5, 0.0, 0.0, 0.5)
            };
            List<double> expectedWeights = new List<double> {0.5, 0.5, 0.5};

            // Act
            List<double> weight1d = LinearAlgebra.GetWeights(homogeneousPts);

            // Assert
            weight1d.SequenceEqual(expectedWeights).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Two_Dimensional_Set_Of_Weights()
        {
            // Arrange
            List<List<Point4d>> homogeneousPts = new List<List<Point4d>>
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
            List<List<double>> weight2d = LinearAlgebra.GetWeights2d(homogeneousPts);

            // Assert
            for (int i = 0; i < homogeneousPts.Count; i++)
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
            Point4d homogeneousPts = new Point4d(1.25, -1.25, 0.0, 0.5);
            Point3d expectedDehomogenizedPoint = new Point3d(2.5, -2.5, 0 );

            // Act
            Point3d dehomogenizedPt = LinearAlgebra.PointDehomogenizer(homogeneousPts);

            // Assert
            dehomogenizedPt.Equals(expectedDehomogenizedPoint).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Set_Of_Dehomogenized_Points()
        {
            // Arrange
            List<Point4d> homogeneousPts = new List<Point4d>
            {
                new (0.0, 0.0, 0.0, 0.5),
                new (1.25, -1.25, 0.0, 0.5),
                new (2.5, 0.0, 0.0, 0.5)
            };

            List<Point3d> dehomogenizeExpected = new List<Point3d>
            {
                new (0.0, 0.0, 0),
                new (2.5, -2.5, 0),
                new (5.0, 0.0, 0)
            };

            // Act
            List<Point3d> dehomogenizedPts = LinearAlgebra.PointDehomogenizer1d(homogeneousPts);

            // Assert
            dehomogenizedPts.SequenceEqual(dehomogenizeExpected).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Two_Dimensional_Set_Of_Dehomogenized_Points()
        {
            // Arrange
            List<List<Point4d>> homogeneousPts = new List<List<Point4d>>
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

            List<List<Point3d>> expectedDehomogenizedPoints = new List<List<Point3d>>
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
            var dehomogenizedPts = LinearAlgebra.PointDehomogenizer2d(homogeneousPts);

            // Arrange
            dehomogenizedPts.Should().BeEquivalentTo(expectedDehomogenizedPoints);
        }

        [Fact]
        public void It_Returns_A_Rationalized_Set_Of_Points()
        {
            // Arrange
            List<Point4d> homogenousPoints = new List<Point4d>
            {
                new (0.0, 0.0, 0.0, 0.5),
                new (2.5, -2.5, 0.0, 1.0),
                new (5.0, 0.0, 0.0, 1.0)
            };

            List<Point3d> expectedPts = new List<Point3d>
            {
                new (0.0, 0.0, 0.0),
                new (2.5, -2.5, 0.0),
                new (5.0, 0.0, 0.0)
            };

            // Act
            List<Point3d> rationalPoints = LinearAlgebra.RationalPoints(homogenousPoints);

            // Assert
            rationalPoints.SequenceEqual(expectedPts).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Rationalized_Two_Dimensional_Set_Of_Points()
        {
            // Arrange
            List<List<Point4d>> homogenousPoints = new List<List<Point4d>>
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

            List<List<Point3d>> expectedPoints = new List<List<Point3d>>
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
            List<List<Point3d>> rationalPoints = LinearAlgebra.Rational2d(homogenousPoints);

            // Assert
            for (int i = 0; i < homogenousPoints.Count; i++)
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

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(1, 1, 1)]
        [InlineData(0, 1, 0)]
        [InlineData(3, 1, 3)]
        [InlineData(3, 3, 1)]
        [InlineData(5, 3, 10)]
        [InlineData(5, 4, 5)]
        [InlineData(8, 4, 70)]
        public void It_Returns_A_Binomial_Coefficient(int n, int k, double resultValue)
        {
            // Act
            double valToCheck = LinearAlgebra.GetBinomial(n, k);

            // Assert
            (System.Math.Abs(valToCheck - resultValue) < GeoSharpMath.Epsilon).Should().BeTrue();
        }
    }
}
