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

        public static IEnumerable<object[]> VectorLengths =>
            new List<object[]>
            {
                new object[] { new Vector() { -18d, -21d, -17d }, 32.46536616149585},
                new object[] { Vector.Unset, 0.0},
                new object[] { new Vector() { -0d, 0d, 0d }, 0.0}
            };

        public static IEnumerable<object[]> AmplifiedVectors =>
            new List<object[]>
            {
                new object[] { new Vector() { 3.0930734141595426, 11.54653670707977, 6.726731646460115 }, 15},
                new object[] { new Vector() { -27.457431218879393, -3.7287156094396963, 14.364357804719848 }, -20}
            };

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_TheRadianAngle_BetweenTwoVectors()
        {
            Vector v1 = new Vector(){20d,0d,0d};
            Vector v2 = new Vector(){-10d,15d,0d};

            double angle = Vector.AngleBetween(v1, v2);

            angle.Should().Be(2.1587989303424644);
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
        public void Return_TheDotProduct_BetweenTwoVectors()
        {
            Vector v1 = new Vector() { -10d, 5d, 10d };
            Vector v2 = new Vector() { 10d, 15d, 5d };

            double dotProduct = Vector.Dot(v1, v2);

            dotProduct.Should().Be(25);
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
        [Theory]
        [MemberData(nameof(VectorLengths))]
        public void Return_TheLengthOfAVector(Vector v, double expectedLength)
        {
            double length = Vector.Length(v);

            length.Should().Be(expectedLength);
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
        public void Return_AZero1dVector()
        {
            Vector vec1D = Vector.Zero1d(4);

            vec1D.Should().HaveCount(4);
            vec1D.Select(val => val.Should().Be(0.0));
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_AZero2dVector()
        {
            var vec2D = Vector.Zero2d(3,3);

            vec2D.Should().HaveCount(3);
            vec2D.Select(val => val.Should().HaveCount(3));
            vec2D.Select(val => val.Should().Contain(0.0));
        }

        [Trait("Category", "Vector")]
        [Fact]
        public void Return_AZero3dVector()
        {
            var vec3D = Vector.Zero3d(3, 3, 4);

            vec3D.Should().HaveCount(3);
            vec3D.Select(val => val.Should().HaveCount(4));
            vec3D.Select(val => val.Select(x => x.Should().Contain(0.0)));
        }

        [Trait("Category", "Vector")]
        [Theory]
        [MemberData(nameof(AmplifiedVectors))]
        public void Return_VectorAmplified_LongADirection(Vector expected, double amplitude)
        {
            var pt = new Point(){ -10, 5, 10 };
            var dir = new Vector(){ 20,10,-5};

            var amplifiedVector = Vector.OnRay(pt, dir, amplitude);

            amplifiedVector.Should().BeEquivalentTo(expected);
        }

    }
}
