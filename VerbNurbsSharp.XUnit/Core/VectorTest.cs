using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerbNurbsSharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
    public class VectorTest
    {
        private readonly ITestOutputHelper _testOutput;

        public VectorTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void Return_TheSubtraction_BetweenTwoVectors()
        {
            Vector vec1 = new Vector() { 8.0, 5.0, 0.0 };
            Vector vec2 = new Vector() { 1.0, 10.0, -6.0 };
            Vector vecExpected = new Vector() { -7.0, 5.0, -6.0 };

            Vector result = Vector.Subtraction(vec2, vec1);
            Assert.Equal(vecExpected, result);
        }
    }
}
