using System;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using GShark.Enumerations;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Core
{
    public class TransformMatrixTests
    {
        private readonly ITestOutputHelper _testOutput;
        public TransformMatrixTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Combines_Transformation_Matrices()
        {
            //Arrange
            var t1 = TransformMatrix.Translation(new Vector3(5, 5, 2));
            var t2 = TransformMatrix.Rotation(Vector3.ZAxis, GSharkMath.ToRadians(30));
            var t3 = TransformMatrix.Scale(2, 2, 2);
            var expectedMatrix1 = new TransformMatrix() //Compound(t1, t2, t3) checked gh.
            {
                M00 = 1.7320508075688774,
                M01 = 0.9999999999999999,
                M10 = -0.9999999999999999,
                M11 = 1.7320508075688774,
                M22 = 2,
                M30 = 3.660254037844388,
                M31 = 13.660254037844386,
                M32 = 4
            };

            var expectedMatrix2 = new TransformMatrix() //Compound(t2,t1,t3) checked in gh.
            {
                M00 = 1.7320508075688774,
                M01 = 0.9999999999999999,
                M10 = -0.9999999999999999,
                M11 = 1.7320508075688774,
                M22 = 2,
                M30 = 10,
                M31 = 10,
                M32 = 4
            };

            //Act
            //order matters, matrices non commutative.
            var result1 = t1.Combine(t2).Combine(t3);
            var result2 = t2.Combine(t1).Combine(t3);

            //Assert
#if DEBUG
            _testOutput.WriteLine(result1.ToString());
            _testOutput.WriteLine(result2.ToString());
#endif
            result1.Equals(expectedMatrix1).Should().BeTrue();
            result2.Equals(expectedMatrix2).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Identity_TransformMatrix()
        {
            // Act
            var transformMatrix = new TransformMatrix();

            // Assert
            var matrix = transformMatrix.Matrix;
            matrix.Should().NotBeNull();
            matrix.Count.Should().Be(4);
            matrix[0].Count.Should().Be(4);
        }

        [Fact]
        public void It_Creates_A_TransformationMatrix_By_Copying_Another_TransformationMatrix()
        {
            // Arrange
            var translationMatrix = TransformMatrix.Translation(new Vector3(15, 15, 0));


            // Act
            var copyMatrix = new TransformMatrix(translationMatrix);

            // Assert
            copyMatrix.Equals(translationMatrix);
        }

        [Fact]
        public void It_Returns_A_Identity_Transform_Matrix()
        {
            // Act
            var transform = new TransformMatrix();

            // Assert
            var matrix = transform.Matrix;

            matrix.Count.Should().Be(4);
            matrix[0].Count.Should().Be(4);
            matrix[0][0].Should().Be(1);
            matrix[1][1].Should().Be(1);
            matrix[2][2].Should().Be(1);
            matrix[3][3].Should().Be(1);
        }

        [Fact]
        public void It_Returns_A_Translation_Matrix()
        {
            // Arrange
            var translation = new Vector3(10, 10, 0);

            // Act
            var transform = Transform.Translation(translation);

            // Assert
            transform.M30.Should().Be(10);
            transform.M31.Should().Be(10);
            transform.M32.Should().Be(0);
            transform.M33.Should().Be(1);
        }

        [Fact]
        public void It_Returns_A_Rotation_Matrix()
        {
            // Arrange
            var center = new Point3(5, 5, 0);
            double angleInRadians = GSharkMath.ToRadians(30);

            // Act
            var transform = Transform.Rotation(angleInRadians, center, RotationAxis.Z);

            // Assert
#if DEBUG
            _testOutput.WriteLine(transform.ToString());
#endif
            transform.M00.Equals(0.8660254037844387).Should().BeTrue();
            transform.M10.Equals(-0.49999999999999994).Should().BeTrue();
            transform.M20.Equals(0).Should().BeTrue();
            transform.M30.Equals(3.169872981077806).Should().BeTrue(); 
            transform.M01.Equals(0.49999999999999994).Should().BeTrue();
            transform.M11.Equals(0.8660254037844387).Should().BeTrue();
            transform.M21.Equals(0).Should().BeTrue();
            transform.M31.Equals(-1.8301270189221928).Should().BeTrue();
            transform.M02.Equals(0).Should().BeTrue();
            transform.M12.Equals(0).Should().BeTrue();
            transform.M22.Equals(1).Should().BeTrue();
            transform.M32.Equals(0).Should().BeTrue();
            transform.M03.Equals(0).Should().BeTrue();
            transform.M13.Equals(0).Should().BeTrue();
            transform.M23.Equals(0).Should().BeTrue();
            transform.M33.Equals(1).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Scale_Matrix()
        {
            // Act
            var scale1 = Transform.Scale(0.5);
            var scale2 = Transform.Scale(new Point3(10, 10, 0), 0.5);

            // Assert
#if DEBUG
            _testOutput.WriteLine(scale2.ToString());
#endif
            scale1.M00.Should().Be(0.5); scale2.M00.Should().Be(0.5);
            scale1.M11.Should().Be(0.5); scale2.M11.Should().Be(0.5);
            scale1.M22.Should().Be(0.5); scale2.M22.Should().Be(0.5);
            scale1.M33.Should().Be(1.0); scale2.M33.Should().Be(1.0);

            scale2.M30.Should().Be(5.0);
            scale2.M31.Should().Be(5.0);
        }

        [Fact]
        public void It_Returns_A_Reflection_Matrix()
        {
            // Arrange
            var pt = new Point3(10, 10, 0);
            Plane plane = new Plane(pt, Vector3.XAxis);

            // Act
            var transform = TransformMatrix.Reflection(plane);

            // Assert
            transform.M00.Should().Be(-1.0);
            transform.M11.Should().Be(1.0);
            transform.M22.Should().Be(1.0);
            transform.M33.Should().Be(1.0);
            transform.M30.Should().Be(20);
        }

        [Fact]
        public void It_Returns_A_Projection_Matrix()
        {
            // Arrange
            var origin = new Point3(5, 0, 0);
            var dir = new Point3(-10, -15, 0);
            Plane plane = new Plane(origin, dir);

            // Act
            var transform = TransformMatrix.Projection(plane);

            // Assert
#if DEBUG
            _testOutput.WriteLine(transform.ToString());
#endif
            transform.M00.Should().BeApproximately(0.692308, GSharkMath.MaxTolerance);
            transform.M10.Should().BeApproximately(-0.461538, GSharkMath.MaxTolerance);
            transform.M30.Should().BeApproximately(1.538462, GSharkMath.MaxTolerance);
            transform.M01.Should().BeApproximately(-0.461538, GSharkMath.MaxTolerance);
            transform.M11.Should().BeApproximately(0.307692, GSharkMath.MaxTolerance);
            transform.M31.Should().BeApproximately(2.307692, GSharkMath.MaxTolerance);
            transform.M33.Should().BeApproximately(1.0, GSharkMath.MaxTolerance);
        }

        [Fact]
        public void It_Multiplies_A_Point3()
        {
            //Arrange
            var p = new Point3(5, -2, 12.5);
            var expectedPt = new Vector3(8,0,13);
            var m = TransformMatrix.Translation(new Vector3(3, 2, 0.5));

            //Act
            var result = p * m;

            //Assert
#if DEBUG
            _testOutput.WriteLine(result.ToString());
#endif
            result.Equals(expectedPt).Should().BeTrue();
        }

       
    }
}
