using System;
using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Evaluation
{
    [Trait("Category", "Eval")]
    public class EvalTest
    {
        private readonly ITestOutputHelper _testOutput;

        public EvalTest(ITestOutputHelper testOutput)
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
        public void EvalHomogenized1d_ThrowsAnException_IfTheSetOfWeights_IsBiggerThanControlPts()
        {
            var controlPts = new List<Vector3>();
            var weights = new List<double>(){1.0,1.5,1.0};

            Func<object> resultFunction = () => Eval.Homogenize1d(controlPts, weights);

            resultFunction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(Homogenized1dData))]
        public void NewSetOfControlPoints_Homogenized1d(List<double> weights, List<Vector3> controlPtsExpected)
        {
            var controlPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0},
                new Vector3() {2.5, -2.5, 0},
                new Vector3() {5.0, 0.0, 0}
            };

            var newControlPts = Eval.Homogenize1d(controlPts, weights);

            newControlPts.Should().BeEquivalentTo(controlPtsExpected);
        }

        [Fact]
        public void Weight1d_ThrowsAnException_IfThePassedSet_HasNotTheSameDimension()
        {
            var homegeneousPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0.0},
                new Vector3() {1.25, -1.25, 0.0, 0.5},
                new Vector3() {2.5, 0.0, 0.0, 0.5}
            };

            Func<object> resultFunc = () => Eval.Weight1d(homegeneousPts);

            resultFunc.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Weight1d()
        {
            var homegeneousPts = new List<Vector3>()
            {
                new Vector3() {0.0, 0.0, 0.0, 0.5},
                new Vector3() {1.25, -1.25, 0.0, 0.5},
                new Vector3() {2.5, 0.0, 0.0, 0.5}
            };

            var weight1d = Eval.Weight1d(homegeneousPts);

            weight1d.Should().BeEquivalentTo(new List<double>() {0.5, 0.5, 0.5});
        }

        [Fact]
        public void Dehomogenizer()
        {
            var homegeneousPts = new Vector3() {1.25, -1.25, 0.0, 0.5};
            var dehomogenizeExpected = new Vector3() {2.5, -2.5, 0};

            var dehomogenizePts = Eval.Dehomogenize(homegeneousPts);

            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void Dehomogenizer1d()
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

            var dehomogenizePts = Eval.Dehomogenize1d(homegeneousPts);

            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Theory]
        [InlineData(4, 2.5)]
        [InlineData(3, 1)]
        [InlineData(3, 1.5)]
        [InlineData(7, 4.9)]
        [InlineData(7, 10)]
        [InlineData(7, 5)]
        [InlineData(2, 0)]
        [InlineData(2, -1)]
        public void KnotSpanGivenParameter(int expectedValue, double parameter)
        {
            var knotVector = new Knot() { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };
            var degree = 2;

            var result = Eval.KnotSpan(knotVector.Count - degree - 2, 2, parameter, knotVector);

            result.Should().Be(expectedValue);
        }
    }
}
