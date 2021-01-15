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

        [Fact]
        public void It_Tests_A_Basic_Function()
        {
            var degree = 2;
            var span = 4;
            var knots = new Knot() {0, 0, 0, 1, 2, 3, 4, 4, 5, 5, 5};

            var result1 = Eval.BasicFunction(degree, knots, span, 2.5);
            var result2 = Eval.BasicFunction(degree, knots,2.5);

            result1.Should().BeEquivalentTo(result2);
            result1.Count.Should().Be(3);
            result1[0].Should().Be(0.125);
            result1[1].Should().Be(0.75);
            result1[2].Should().Be(0.125);
        }
    }
}
