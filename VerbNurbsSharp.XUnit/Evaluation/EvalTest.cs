using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Evaluation;
using Xunit;

namespace VerbNurbsSharp.XUnit.Evaluation
{
    [Trait("Category", "Eval")]
    public class EvalTest
    {
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
    }
}
