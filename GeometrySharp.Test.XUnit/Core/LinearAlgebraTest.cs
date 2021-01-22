using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Core
{
    [Trait("Category", "LinearAlgebra")]
    public class LinearAlgebraTest
    {
        private readonly ITestOutputHelper _testOutput;

        public LinearAlgebraTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> Homogenized1dData => new List<object[]>
        {
            new object[]
            {
                new List<double>() {0.5, 0.5, 0.5},
                new List<Vector3>()
                {
                    new Vector3() {0.0, 0.0, 0.0, 0.5},
                    new Vector3() {1.25, -1.25, 0.0, 0.5},
                    new Vector3() {2.5, 0.0, 0.0, 0.5}
                }
            },

            new object[]
            {
                new List<double>() {0.5},
                new List<Vector3>()
                {
                    new Vector3() {0.0, 0.0, 0.0, 0.5},
                    new Vector3() {2.5, -2.5, 0.0, 1.0},
                    new Vector3() {5.0, 0.0, 0.0, 1.0}
                }
            },

            new object[]
            {
                null,
                new List<Vector3>()
                {
                    new Vector3() {0.0, 0.0, 0.0, 1.0},
                    new Vector3() {2.5, -2.5, 0.0, 1.0},
                    new Vector3() {5.0, 0.0, 0.0, 1.0}
                }
            }
        };

        [Fact]
        public void Eval_Homogenized1d_Throws_An_Exception_If_The_Weight_Collection_Is_Bigger_Than_ControlPts()
        {
            var controlPts = new List<Vector3>();
            var weights = new List<double>(){1.0,1.5,1.0};

            Func<object> resultFunction = () => LinearAlgebra.Homogenize1d(controlPts, weights);

            resultFunction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(Homogenized1dData))]
        public void It_Returns_A_New_Set_Of_ControlPoints_Homogenized1d(List<double> weights, List<Vector3> controlPtsExpected)
        {
            var controlPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0},
                new Vector3() {2.5, -2.5, 0},
                new Vector3() {5.0, 0.0, 0}
            };

            var newControlPts = LinearAlgebra.Homogenize1d(controlPts, weights);

            newControlPts.Should().BeEquivalentTo(controlPtsExpected);
        }

        [Fact]
        public void Weight1d_Throws_An_Exception_If_The_Set_Of_Points_Has_Not_The_Same_Dimension()
        {
            var homegeneousPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0.0},
                new Vector3() {1.25, -1.25, 0.0, 0.5},
                new Vector3() {2.5, 0.0, 0.0, 0.5}
            };

            Func<object> resultFunc = () => LinearAlgebra.Weight1d(homegeneousPts);

            resultFunc.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void It_Returns_A_Weighted1d_Set()
        {
            var homegeneousPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0.0, 0.5},
                new Vector3() {1.25, -1.25, 0.0, 0.5},
                new Vector3() {2.5, 0.0, 0.0, 0.5}
            };

            var weight1d = LinearAlgebra.Weight1d(homegeneousPts);

            weight1d.Should().BeEquivalentTo(new List<double>() {0.5, 0.5, 0.5});
        }

        [Fact]
        public void It_Returns_A_Set_Of_Points_Dehomogenized()
        {
            var homegeneousPts = new Vector3() {1.25, -1.25, 0.0, 0.5};
            var dehomogenizeExpected = new Vector3() {2.5, -2.5, 0};

            var dehomogenizePts = LinearAlgebra.Dehomogenize(homegeneousPts);

            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void It_Returns_A_Set_Of_Points_Dehomogenizer1d()
        {
            var homegeneousPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0.0, 0.5},
                new Vector3() {1.25, -1.25, 0.0, 0.5},
                new Vector3() {2.5, 0.0, 0.0, 0.5}
            };

            var dehomogenizeExpected = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0},
                new Vector3() {2.5, -2.5, 0},
                new Vector3() {5.0, 0.0, 0}
            };

            var dehomogenizePts = LinearAlgebra.Dehomogenize1d(homegeneousPts);

            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void It_Returns_A_Rationalized_Set_Of_Points()
        {
            List<Vector3> homoPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0.0, 0.5},
                new Vector3() {2.5, -2.5, 0.0, 1.0},
                new Vector3() {5.0, 0.0, 0.0, 1.0}
            };

            var ratioPts = LinearAlgebra.Rational1d(homoPts);

            ratioPts.All(pt => pt.Count == 3).Should().BeTrue();
            ratioPts[0].Should().BeEquivalentTo(new Vector3() {0.0, 0.0, 0.0});
            ratioPts[1].Should().BeEquivalentTo(new Vector3() {2.5, -2.5, 0.0});
            ratioPts[2].Should().BeEquivalentTo(new Vector3() {5.0, 0.0, 0.0});
        }
    }
}
