using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Core
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
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {1.25, -1.25, 0.0, 0.5},
                    new Vector3 {2.5, 0.0, 0.0, 0.5}
                }
            },

            new object[]
            {
                new List<double> {0.5},
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {2.5, -2.5, 0.0, 1.0},
                    new Vector3 {5.0, 0.0, 0.0, 1.0}
                }
            },

            new object[]
            {
                null,
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 1.0},
                    new Vector3 {2.5, -2.5, 0.0, 1.0},
                    new Vector3 {5.0, 0.0, 0.0, 1.0}
                }
            }
        };

        public static IEnumerable<object[]> Homogenized2dData => new List<object[]>
        {
            new object[]
            {
                new List<List<double>> {
                    new List<double> { 0.5, 0.5, 0.5 },
                    new List<double> { 0.5, 0.5, 0.5 },
                },
                new List<List<Vector3>>()
                {
                    new List<Vector3>{
                        new Vector3 {0.0, 0.0, 0.0, 0.5},
                        new Vector3 {1.25, -1.25, 0.0, 0.5},
                        new Vector3 {2.5, 0.0, 0.0, 0.5}
                    },
                    new List<Vector3>{
                        new Vector3 {0.0, 0.0, 0.0, 0.5},
                        new Vector3 {1.25, -1.25, 0.0, 0.5},
                        new Vector3 {2.5, 0.0, 0.0, 0.5}
                    }
                }
            },
            new object[]
            {
                new List<List<double>> { new List<double> { 0.5 }, new List<double> { 0.5 }},
                new List<List<Vector3>>
                {
                    new List<Vector3>
                    {
                        new Vector3() {0.0, 0.0, 0.0, 0.5},
                        new Vector3() {2.5, -2.5, 0.0, 1.0},
                        new Vector3() {5.0, 0.0, 0.0, 1.0}
                    },
                    new List<Vector3>
                    {
                        new Vector3() {0.0, 0.0, 0.0, 0.5},
                        new Vector3() {2.5, -2.5, 0.0, 1.0},
                        new Vector3() {5.0, 0.0, 0.0, 1.0}
                    }
                }
            },

            new object[]
            {
                null,
                new List<List<Vector3>>
                {
                    new List<Vector3>
                    {
                        new Vector3() {0.0, 0.0, 0.0, 1.0},
                        new Vector3() {2.5, -2.5, 0.0, 1.0},
                        new Vector3() {5.0, 0.0, 0.0, 1.0}
                    },
                    new List<Vector3>
                    {
                        new Vector3() {0.0, 0.0, 0.0, 1.0},
                        new Vector3() {2.5, -2.5, 0.0, 1.0},
                        new Vector3() {5.0, 0.0, 0.0, 1.0}
                    }
                }
            }
        };

        [Fact]
        public void Eval_Homogenized1d_Throws_An_Exception_If_The_Weight_Collection_Is_Bigger_Than_ControlPts()
        {
            // Arrange
            List<Vector3> controlPts = new List<Vector3>();
            List<double> weights = new List<double> { 1.0, 1.5, 1.0 };

            // Act
            Func<object> resultFunction = () => LinearAlgebra.Homogenize1d(controlPts, weights);

            // Assert
            resultFunction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(Homogenized1dData))]
        public void It_Returns_A_New_Set_Of_ControlPoints_Homogenized1d(List<double> weights, List<Vector3> controlPtsExpected)
        {
            // Arrange
            List<Vector3> controlPts = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0},
                new Vector3 {2.5, -2.5, 0},
                new Vector3 {5.0, 0.0, 0}
            };

            // Act
            List<Vector3> newControlPts = LinearAlgebra.Homogenize1d(controlPts, weights);

            // Assert
            newControlPts.Should().BeEquivalentTo(controlPtsExpected);
        }

        [Theory]
        [MemberData(nameof(Homogenized2dData))]
        public void It_Returns_A_New_Set_Of_ControlPoints_Homogenized2d(List<List<double>> weights, List<List<Vector3>> controlPtsExpected)
        {
            // Arrange
            List<List<Vector3>> controlPts = new List<List<Vector3>>()
            {
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0},
                    new Vector3 {2.5, -2.5, 0},
                    new Vector3 {5.0, 0.0, 0}
                },
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0},
                    new Vector3 {2.5, -2.5, 0},
                    new Vector3 {5.0, 0.0, 0}
                }
            };

            // Act
            List<List<Vector3>> newControlPts = LinearAlgebra.Homogenize2d(controlPts, weights);
            
            // Assert
            newControlPts.Should().BeEquivalentTo(controlPtsExpected);
        }

        [Fact]
        public void Weight1d_Throws_An_Exception_If_The_Set_Of_Points_Has_Not_The_Same_Dimension()
        {
            // Arrange
            List<Vector3> homogeneousPts = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0.0},
                new Vector3 {1.25, -1.25, 0.0, 0.5},
                new Vector3 {2.5, 0.0, 0.0, 0.5}
            };

            // Act
            Func<object> resultFunc = () => LinearAlgebra.Weight1d(homogeneousPts);

            // Assert
            resultFunc.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void It_Returns_A_Weighted1d_Set()
        {
            // Arrange
            List<Vector3> homogeneousPts = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0.0, 0.5},
                new Vector3 {1.25, -1.25, 0.0, 0.5},
                new Vector3 {2.5, 0.0, 0.0, 0.5}
            };
            List<double> expectedWeights = new List<double> {0.5, 0.5, 0.5};

            // Act
            List<double> weight1d = LinearAlgebra.Weight1d(homogeneousPts);

            // Assert
            weight1d.Should().BeEquivalentTo(expectedWeights);
        }

        [Fact]
        public void It_Returns_A_Weighted2d_Set()
        {
            // Arrange
            List<List<Vector3>> homogeneousPts = new List<List<Vector3>>
            {
                new List<Vector3> {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {1.25, -1.25, 0.0, 0.5},
                    new Vector3 {2.5, 0.0, 0.0, 0.5}
                },
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {1.25, -1.25, 0.0, 0.5},
                    new Vector3 {2.5, 0.0, 0.0, 0.5}
                }
            };

            List<List<double>> expectedWeights = new List<List<double>>
            {
                new List<double> {0.5, 0.5, 0.5},
                new List<double> {0.5, 0.5, 0.5}
            };

            // Act
            List<List<double>> weight2d = LinearAlgebra.Weight2d(homogeneousPts);
            
            // Assert
            weight2d.Should().BeEquivalentTo(expectedWeights);
        }

        [Fact]
        public void It_Returns_A_Set_Of_Points_Dehomogenized()
        {
            // Arrange
            Vector3 homogeneousPts = new Vector3 { 1.25, -1.25, 0.0, 0.5 };
            Vector3 dehomogenizeExpected = new Vector3 { 2.5, -2.5, 0 };

            // Act
            Vector3 dehomogenizePts = LinearAlgebra.Dehomogenize(homogeneousPts);

            // Assert
            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void It_Returns_A_Set_Of_Points_Dehomogenizer1d()
        {
            // Arrange
            List<Vector3> homogeneousPts = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0.0, 0.5},
                new Vector3 {1.25, -1.25, 0.0, 0.5},
                new Vector3 {2.5, 0.0, 0.0, 0.5}
            };

            List<Vector3> dehomogenizeExpected = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0},
                new Vector3 {2.5, -2.5, 0},
                new Vector3 {5.0, 0.0, 0}
            };

            // Act
            List<Vector3> dehomogenizePts = LinearAlgebra.Dehomogenize1d(homogeneousPts);

            // Assert
            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void It_Returns_A_Set_Of_Points_Dehomogenizer2d()
        {
            // Arrange
            List<List<Vector3>> homogeneousPts = new List<List<Vector3>>
            {
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {1.25, -1.25, 0.0, 0.5},
                    new Vector3 {2.5, 0.0, 0.0, 0.5}
                },
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {1.25, -1.25, 0.0, 0.5},
                    new Vector3 {2.5, 0.0, 0.0, 0.5}
                }
            };

            List<List<Vector3>> dehomogenizeExpected = new List<List<Vector3>>
            {
               new List<Vector3>
               {
                   new Vector3 {0.0, 0.0, 0},
                   new Vector3 {2.5, -2.5, 0},
                   new Vector3 {5.0, 0.0, 0}
               },
               new List<Vector3>
               {
                   new Vector3 {0.0, 0.0, 0},
                   new Vector3 {2.5, -2.5, 0},
                   new Vector3 {5.0, 0.0, 0}
               }
            };

            // Act
            List<List<Vector3>> dehomogenizePts = LinearAlgebra.Dehomogenize2d(homogeneousPts);

            // Arrange
            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void It_Returns_A_Rationalized_Set_Of_Points()
        {
            // Arrange
            List<Vector3> homoPts = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0.0, 0.5},
                new Vector3 {2.5, -2.5, 0.0, 1.0},
                new Vector3 {5.0, 0.0, 0.0, 1.0}
            };

            List<Vector3> expectedPts = new List<Vector3>
            {
                new Vector3 {0.0, 0.0, 0.0},
                new Vector3 {2.5, -2.5, 0.0},
                new Vector3 {5.0, 0.0, 0.0}
            };

            // Act
            List<Vector3> ratioPts = LinearAlgebra.Rational1d(homoPts);

            // Assert
            ratioPts.All(pt => pt.Count == 3).Should().BeTrue();
            ratioPts.Should().BeEquivalentTo(expectedPts);
        }

        [Fact]
        public void It_Returns_A_Rationalized_Set_Of_Sets_Points()
        {
            // Arrange
            List<List<Vector3>> homoPts = new List<List<Vector3>>
            {
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {2.5, -2.5, 0.0, 1.0},
                    new Vector3 {5.0, 0.0, 0.0, 1.0}
                },
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0, 0.5},
                    new Vector3 {2.5, -2.5, 0.0, 1.0},
                    new Vector3 {5.0, 0.0, 0.0, 1.0}
                }
            };

            List<List<Vector3>> expectedPts = new List<List<Vector3>>
            {
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0},
                    new Vector3 {2.5, -2.5, 0.0},
                    new Vector3 {5.0, 0.0, 0.0}
                },
                new List<Vector3>
                {
                    new Vector3 {0.0, 0.0, 0.0},
                    new Vector3 {2.5, -2.5, 0.0},
                    new Vector3 {5.0, 0.0, 0.0}
                }
            };

            // Act
            List<List<Vector3>> ratioPts = LinearAlgebra.Rational2d(homoPts);

            // Assert
            ratioPts.All(lst => lst.Count == 3).Should().BeTrue();
            ratioPts[0].All(pt => pt.Count == 3).Should().BeTrue();
            ratioPts.Should().BeEquivalentTo(expectedPts);
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
            (System.Math.Abs(valToCheck - resultValue) < GeoSharpMath.EPSILON).Should().BeTrue();
        }
    }
}
