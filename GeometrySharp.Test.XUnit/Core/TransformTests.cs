using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void It_Returns_A_Scaled_Transformation_Matrix()
        {
            var scale1 = Transform.Scale(new Vector3 {0, 0, 0}, 0.5);
            var scale2 = Transform.Scale(new Vector3 { 10, 10, 0 }, 0.5);

            scale1[0][0].Should().Be(0.5); scale2[0][0].Should().Be(0.5);
            scale1[1][1].Should().Be(0.5); scale2[1][1].Should().Be(0.5);
            scale1[2][2].Should().Be(0.5); scale2[2][2].Should().Be(0.5);
            scale1[3][3].Should().Be(1.0); scale2[3][3].Should().Be(1.0);

            scale2[0][3].Should().Be(5.0);
            scale2[1][3].Should().Be(5.0);
        }

        [Fact]
        public void It_Returns_A_Mirrored_Transformation_Matrix()
        {
            var pt = new Vector3{10,10,0};
            var plane = new Plane(pt, Vector3.XAxis);

            var transform = Transform.Reflection(plane);

            transform[0][0].Should().Be(-1.0);
            transform[1][1].Should().Be(1.0); 
            transform[2][2].Should().Be(1.0); 
            transform[3][3].Should().Be(1.0);
            transform[0][3].Should().Be(20);
        }

        [Fact]
        public void It_Returns_A_Transformation_Projection_By_A_Plane()
        {
            Vector3 origin = new Vector3 { 5, 0, 0 };
            Vector3 dir = new Vector3 { -10, -15, 0 };
            Plane plane = new Plane(origin, dir);

            Transform transform = Transform.PlanarProjection(plane);

            transform[0][0].Should().BeApproximately(0.692308, GeoSharpMath.MAXTOLERANCE);
            transform[0][1].Should().BeApproximately(-0.461538, GeoSharpMath.MAXTOLERANCE);
            transform[0][3].Should().BeApproximately(1.538462, GeoSharpMath.MAXTOLERANCE);
            transform[1][0].Should().BeApproximately(-0.461538, GeoSharpMath.MAXTOLERANCE);
            transform[1][1].Should().BeApproximately(0.307692, GeoSharpMath.MAXTOLERANCE);
            transform[1][3].Should().BeApproximately(2.307692, GeoSharpMath.MAXTOLERANCE);
            transform[3][3].Should().BeApproximately(1.0, GeoSharpMath.MAXTOLERANCE);
        }

        [Fact]
        public void It_Returns_A_Plane_To_Plane_Transformation_Matrix()
        {
            Vector3 origin = new Vector3 { 5, 0, 0 };
            Vector3 dir = new Vector3 { -10, -15, 0 };
            Plane plane = new Plane(origin, dir);

            Transform transform = Transform.PlaneToPlane(Plane.PlaneXY, plane);

            transform[0][0].Should().BeApproximately(-0.832050, GeoSharpMath.MAXTOLERANCE);
            transform[0][2].Should().BeApproximately(-0.554700, GeoSharpMath.MAXTOLERANCE);
            transform[0][3].Should().BeApproximately(5.0, GeoSharpMath.MAXTOLERANCE);
            transform[1][0].Should().BeApproximately(0.554700, GeoSharpMath.MAXTOLERANCE);
            transform[1][2].Should().BeApproximately(-0.832050, GeoSharpMath.MAXTOLERANCE);
            transform[2][1].Should().BeApproximately(-1.0, GeoSharpMath.MAXTOLERANCE);
            transform[3][3].Should().BeApproximately(1.0, GeoSharpMath.MAXTOLERANCE);
        }
    }
}
