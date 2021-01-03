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

        public static IEnumerable<object[]> ValidateVectors =>
            new List<object[]>
            {
                new object[] { new Vector() { 20d, -10d, 0d }, true},
                new object[] { Vector.Unset, false},
            };

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

        [Trait("Category", "Vector")]
        [Theory]
        [MemberData(nameof(ValidateVectors))]
        public void CheckingIfVectors_AreValidOrNot(Vector v, bool expected)
        {
            v.IsValid().Should().Be(expected);
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_TheCrossProduct_BetweenTwoVectors()
        {
            Vector v1 = new Vector() { -10d, 5d, 10d };
            Vector v2 = new Vector() { 10d, 15d, 5d };
            Vector crossProductExpected = new Vector() { -125d, 150d, -200d };

            Vector crossProduct = Vector.Cross(v1, v2);

            crossProduct.Should().BeEquivalentTo(crossProductExpected);
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_TheSquaredLengthOfAVector()
        {
            Vector v1 = new Vector() { 10d, 15d, 5d };

            double squaredLength = Vector.SquaredLength(v1);

            squaredLength.Should().Be(350);
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_TheLengthOfAVector()
        {
            Vector v1 = new Vector() { -18d, -21d, -17d };

            double length = Vector.Length(v1);

            length.Should().Be(32.46536616149585);
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_VectorLengthZero_ForAnInvalidVector()
        {
            double length = Vector.Length(Vector.Unset);
            length.Should().Be(0.0);
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_Normalized_Vector()
        {
            Vector v1 = new Vector() { -18d, -21d, -17d };
            Vector normalizedExpected = new Vector() { -0.5544369932703277, -0.6468431588153823, -0.5236349380886428 };

            Vector normalizedVector = Vector.Normalized(v1);

            normalizedVector.Should().Equal(normalizedExpected);
        }

        [Trait("Category", "Vector")]
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
