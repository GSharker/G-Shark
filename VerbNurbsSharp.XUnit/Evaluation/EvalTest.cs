using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Evaluation;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Evaluation
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
                new List<Vector>()
                {
                    new Vector() {0.0, 0.0, 0.0, 0.5},
                    new Vector() {1.25, -1.25, 0.0, 0.5},
                    new Vector() {2.5, 0.0, 0.0, 0.5}
                }
            },

            new object[]
            {
                new List<double>() {0.5},
                new List<Vector>()
                {
                    new Vector() {0.0, 0.0, 0.0, 0.5},
                    new Vector() {2.5, -2.5, 0.0, 1.0},
                    new Vector() {5.0, 0.0, 0.0, 1.0}
                }
            },

            new object[]
            {
                null,
                new List<Vector>()
                {
                    new Vector() {0.0, 0.0, 0.0, 1.0},
                    new Vector() {2.5, -2.5, 0.0, 1.0},
                    new Vector() {5.0, 0.0, 0.0, 1.0}
                }
            }
        };

        [Fact]
        public void EvalHomogenized1d_ThrowsAnException_IfTheSetOfWeights_IsBiggerThanControlPts()
        {
            var controlPts = new List<Vector>();
            var weights = new List<double>(){1.0,1.5,1.0};

            Func<object> resultFunction = () => Eval.Homogenize1d(controlPts, weights);

            resultFunction.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(Homogenized1dData))]
        public void NewSetOfControlPoints_Homogenized1d(List<double> weights, List<Vector> controlPtsExpected)
        {
            var controlPts = new List<Vector>()
            {
                new Vector() {0.0, 0.0, 0},
                new Vector() {2.5, -2.5, 0},
                new Vector() {5.0, 0.0, 0}
            };

            var newControlPts = Eval.Homogenize1d(controlPts, weights);

            newControlPts.Should().BeEquivalentTo(controlPtsExpected);
        }

        [Fact]
        public void Weight1d_ThrowsAnException_IfThePassedSet_HasNotTheSameDimension()
        {
            var homegeneousPts = new List<Vector>()
            {
                new Vector() {0.0, 0.0, 0.0},
                new Vector() {1.25, -1.25, 0.0, 0.5},
                new Vector() {2.5, 0.0, 0.0, 0.5}
            };

            Func<object> resultFunc = () => Eval.Weight1d(homegeneousPts);

            resultFunc.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Weight1d()
        {
            var homegeneousPts = new List<Vector>()
            {
                new Vector() {0.0, 0.0, 0.0, 0.5},
                new Vector() {1.25, -1.25, 0.0, 0.5},
                new Vector() {2.5, 0.0, 0.0, 0.5}
            };

            var weight1d = Eval.Weight1d(homegeneousPts);

            weight1d.Should().BeEquivalentTo(new List<double>() {0.5, 0.5, 0.5});
        }

        [Fact]
        public void Dehomogenizer()
        {
            var homegeneousPts = new Vector() {1.25, -1.25, 0.0, 0.5};
            var dehomogenizeExpected = new Vector() {2.5, -2.5, 0};

            var dehomogenizePts = Eval.Dehomogenize(homegeneousPts);

            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void Dehomogenizer1d()
        {
            var homegeneousPts = new List<Vector>()
            {
                new Vector() {0.0, 0.0, 0.0, 0.5},
                new Vector() {1.25, -1.25, 0.0, 0.5},
                new Vector() {2.5, 0.0, 0.0, 0.5}
            };

            var dehomogenizeExpected = new List<Vector>()
            {
                new Vector() {0.0, 0.0, 0},
                new Vector() {2.5, -2.5, 0},
                new Vector() {5.0, 0.0, 0}
            };

            var dehomogenizePts = Eval.Dehomogenize1d(homegeneousPts);

            dehomogenizePts.Should().BeEquivalentTo(dehomogenizeExpected);
        }

        [Fact]
        public void KnotSpanGivenParameter()
        {
            var knotVector = new KnotArray() { 0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5 };

            var result = Eval.KnotSpanGivenN(5, 2, 2.5, knotVector);

            result.Should().Be(4);
        }
    }
}
