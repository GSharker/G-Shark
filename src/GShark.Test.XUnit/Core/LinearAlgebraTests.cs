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
            (System.Math.Abs(valToCheck - resultValue) < GeoSharkMath.Epsilon).Should().BeTrue();
        }
    }
}
