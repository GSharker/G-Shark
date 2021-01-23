using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using Xunit;

namespace GeometrySharp.Test.XUnit.Evaluation
{
    [Trait("Category", "Check")]
    public class CheckTests
    {
        [Theory]
        [InlineData(new double[]{ 0, 0, 0, 1, 1, 1 }, 2, true)]
        [InlineData(new double[] { 0, 0, 0, 0.5, 1, 1, 1 }, 2, true)]
        [InlineData(new double[] { 0, 0, 1, 1, 1 }, 2, false)]
        [InlineData(new double[] { 0, 0, 0.5, 1, 1, 1 }, 2, false)]
        [InlineData(new double[] { 0, 0, 0, 1, 1, 2 }, 2, false)]
        [InlineData(new double[] { 0, 0, 0, 0.5, 1, 1, 2 }, 2, false)]
        [InlineData(new double[] { 0, 0, 0, 0.5, 0.25, 1, 1, 1 }, 2, false)]
        [InlineData(new double[] { 2, 2, 2, 3, 4, 4, 4 }, 2, true)]
        // [InlineData(new double[] { 0, 0, 0, 0.5, 0.25, 1, 1, 1 }, 2, true)]  Check for periodic knots
        public void It_Returns_True_If_The_Knots_Are_Valid(double[] knots, int degree, bool result)
        {
            var knot = new Knot(knots);
            Check.AreValidKnots(knot, degree).Should().Be(result);
        }
    }
}
