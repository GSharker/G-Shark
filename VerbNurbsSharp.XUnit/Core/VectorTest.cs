using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
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

        // ToDo collect a list of vectors in a class.

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_TheRadianAngle_BetweenTwoVectors()
        {
            Vector v1 = new Vector(){20d,0d,0d};
            Vector v2 = new Vector(){-10d,15d,0d};

            double angle = Vector.AngleBetween(v1, v2);

            angle.Should().Be(2.158799);
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_AReversedVector()
        {
            Vector v1 = new Vector() { 20d, 0d, 0d };
            Vector vectorExpected = new Vector() { -20d, 0d, 0d };

            Vector reversedVector = Vector.Reverse(v1);

            reversedVector.Should().BeEquivalentTo(vectorExpected);
        }

        // ToDo take more than one vector to test.
        [Trait("Category", "Vector")]
        [Fact]
        public void ReturnTrue_IfTheVectorIsValid()
        {
            Vector v1 = new Vector() { 20d, -10d, 0d };
            v1.IsValid().Should().BeTrue();
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
