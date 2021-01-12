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
    [Trait("Category", "Vector3")]
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
                new object[] { new Vector3() { 20d, -10d, 0d }, true},
                new object[] { Vector3.Unset, false},
            };

        public static IEnumerable<object[]> VectorLengths =>
            new List<object[]>
            {
                new object[] { new Vector3() { -18d, -21d, -17d }, 32.46536616149585},
                new object[] { Vector3.Unset, 0.0},
                new object[] { new Vector3() { -0d, 0d, 0d }, 0.0}
            };

        public static IEnumerable<object[]> AmplifiedVectors =>
            new List<object[]>
            {
                new object[] { new Vector3() { 3.0930734141595426, 11.54653670707977, 6.726731646460115 }, 15},
                new object[] { new Vector3() { -27.457431218879393, -3.7287156094396963, 14.364357804719848 }, -20}
            };

        [Fact]
        public void Return_TheRadianAngle_BetweenTwoVectors()
        {
            Vector3 v1 = new Vector3(){20d,0d,0d};
            Vector3 v2 = new Vector3(){-10d,15d,0d};

            double angle = Vector3.AngleBetween(v1, v2);

            angle.Should().Be(2.1587989303424644);
        }

        [Fact]
        public void Return_AReversedVector()
        {
            Vector3 v1 = new Vector3() { 20d, 0d, 0d };
            Vector3 vectorExpected = new Vector3() { -20d, 0d, 0d };

            Vector3 reversedVector = Vector3.Reverse(v1);

            reversedVector.Should().BeEquivalentTo(vectorExpected);
        }

        [Theory]
        [MemberData(nameof(ValidateVectors))]
        public void CheckingIfVectors_AreValidOrNot(Vector3 v, bool expected)
        {
            v.IsValid().Should().Be(expected);
        }

        [Fact]
        public void Return_TheCrossProduct_BetweenTwoVectors()
        {
            Vector3 v1 = new Vector3() { -10d, 5d, 10d };
            Vector3 v2 = new Vector3() { 10d, 15d, 5d };
            Vector3 crossProductExpected = new Vector3() { -125d, 150d, -200d };

            Vector3 crossProduct = Vector3.Cross(v1, v2);

            crossProduct.Should().BeEquivalentTo(crossProductExpected);
        }

        [Fact]
        public void Return_TheDotProduct_BetweenTwoVectors()
        {
            Vector3 v1 = new Vector3() { -10d, 5d, 10d };
            Vector3 v2 = new Vector3() { 10d, 15d, 5d };

            double dotProduct = Vector3.Dot(v1, v2);

            dotProduct.Should().Be(25);
        }

        [Fact]
        public void Return_TheSquaredLengthOfAVector()
        {
            Vector3 v1 = new Vector3() { 10d, 15d, 5d };

            double squaredLength = Vector3.SquaredLength(v1);

            squaredLength.Should().Be(350);
        }

        [Theory]
        [MemberData(nameof(VectorLengths))]
        public void Return_TheLengthOfAVector(Vector3 v, double expectedLength)
        {
            double length = Vector3.Length(v);

            length.Should().Be(expectedLength);
        }

        [Fact]
        public void Return_Normalized_Vector()
        {
            Vector3 v1 = new Vector3() { -18d, -21d, -17d };
            Vector3 normalizedExpected = new Vector3() { -0.5544369932703277, -0.6468431588153823, -0.5236349380886428 };

            Vector3 normalizedVector = Vector3.Normalized(v1);

            normalizedVector.Should().Equal(normalizedExpected);
        }

        [Fact]
        public void Return_AZero1dVector()
        {
            Vector3 vec1D = Vector3.Zero1d(4);

            vec1D.Should().HaveCount(4);
            vec1D.Select(val => val.Should().Be(0.0));
        }

        [Fact]
        public void Return_AZero2dVector()
        {
            var vec2D = Vector3.Zero2d(3,3);

            vec2D.Should().HaveCount(3);
            vec2D.Select(val => val.Should().HaveCount(3));
            vec2D.Select(val => val.Should().Contain(0.0));
        }

        [Fact]
        public void Return_AZero3dVector()
        {
            var vec3D = Vector3.Zero3d(3, 3, 4);

            vec3D.Should().HaveCount(3);
            vec3D.Select(val => val.Should().HaveCount(4));
            vec3D.Select(val => val.Select(x => x.Should().Contain(0.0)));
        }

        [Theory]
        [MemberData(nameof(AmplifiedVectors))]
        public void Return_VectorAmplified_LongADirection(Vector3 expected, double amplitude)
        {
            var pt = new Vector3(){ -10, 5, 10 };
            var dir = new Vector3(){ 20,10,-5};

            var amplifiedVector = Vector3.OnRay(pt, dir, amplitude);

            amplifiedVector.Should().BeEquivalentTo(expected);
        }

    }
}
