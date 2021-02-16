using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Core
{
    public class TransformTests
    {
        private readonly ITestOutputHelper _testOutput;
        public TransformTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_An_Instance_Of_Transform()
        {
            var transform = new Transform();

            transform.Should().NotBeNull();
            transform.Count.Should().Be(4);
            transform[0].Count.Should().Be(4);
        }

        [Fact]
        public void It_Creates_A_Transform_By_Copying_Another_Transform()
        {
            var transform = new Transform {[0] = {[0] = 2}, [1] = {[0] = 2}};

            var copyTransform = Transform.Copy(transform);

            copyTransform.Should().BeEquivalentTo(transform);

            transform[0][2] = 3;

            copyTransform.Should().NotBeEquivalentTo(transform);
        }

        [Fact]
        public void It_Returns_A_Identity_Transform_Matrix()
        {
            var transform = Transform.Identity();

            transform.Count.Should().Be(4);
            transform[0].Count.Should().Be(4);
            transform[0][0].Should().Be(1);
            transform[1][1].Should().Be(1);
            transform[2][2].Should().Be(1);
            transform[3][3].Should().Be(1);
        }

        [Fact]
        public void It_Returns_A_Translated_Transformed_Matrix()
        {
            var translation = new Vector3{10,10,0};
            var transform = Transform.Translation(translation);

            transform[0][3].Should().Be(10);
            transform[1][3].Should().Be(10);
            transform[3][3].Should().Be(1);
        }

        // ToDo this test has to be finished extracting the axis.
        [Fact]
        public void It_Returns_A_Rotated_Transformed_Matrix()
        {
            var center = new Vector3{5,5,0};
            var radiance = GeoSharpMath.ToRadians(30);

            var transform = Transform.Rotation(radiance, center);

            // Getting the angles.
            var angles = LinearAlgebra.GetYawPitchRoll(transform);
            // Getting the direction.
            var axis = LinearAlgebra.GetRotationAxis(transform);

            GeoSharpMath.ToDegrees(angles["Yaw"]).Should().BeApproximately(30, GeoSharpMath.EPSILON);
            axis.Should().BeEquivalentTo(Vector3.ZAxis);
        }
    }
}
