using System;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using System.Linq;
using GShark.Enumerations;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Core
{
    public class TransformTests
    {
        private readonly ITestOutputHelper _testOutput;
        public TransformTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }



        [Fact]
        public void It_Returns_A_Non_Uniform_Scaling_Matrix_From_A_Center_Point()
        {
            //Arrange
            var centerPoint = new Point3(5, 5, 2);
            var expectedMatrix = new TransformMatrix()
            {
                M00 = 0.5,
                M10 = 0,
                M20 = 0,
                M30 = 2.5,
                M01 = 0,
                M11 = 0.2,
                M21 = 0,
                M31 = 4,
                M02 = 0,
                M12 = 0,
                M22 = 0.7,
                M32 = 0.6000000000000001,
                M03 = 0,
                M13 = 0,
                M23 = 0,
                M33 = 1
            };

            //Act
            var scalingMatrix = Transform.Scale(centerPoint, .5, .2, .7);

            //Assert
#if DEBUG
            _testOutput.WriteLine(scalingMatrix.ToString());
#endif
            scalingMatrix.Equals(expectedMatrix).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Plane_To_Plane_Transformation_Matrix()
        {
            // Arrange
            Plane pA = new Plane();
            Plane pB = new Plane();

            pA.Origin = new Point3(7.4026739, 4.5163439, 0);
            pA.XAxis = new Vector3(0.7124, 0.701773, 0);
            pA.YAxis = new Vector3(-0.226587, 0.230018, 0.946441);

            pB.Origin = new Point3(17.897769, 22.522106, 0);
            pB.XAxis = new Vector3(-0.416727, -0.909032, 0);
            pB.YAxis = new Vector3(-0.909032, 0.416727, 0);

            var expectedXForm = new TransformMatrix();

            expectedXForm.M00 = -0.09090148101600004;
            expectedXForm.M10 = -0.501541479547;
            expectedXForm.M20 = -0.8603451551119999;
            expectedXForm.M30 = 20.835816822737556;

            expectedXForm.M01 = -0.742019317549;
            expectedXForm.M11 = -0.54207940265;
            expectedXForm.M21 = 0.394407518607;
            expectedXForm.M31 = 30.463250038789766;

            expectedXForm.M02 = -0.6641868664529847;
            expectedXForm.M12 = 0.674244696876492;
            expectedXForm.M22 = -0.3228775234749122;
            expectedXForm.M32 = 1.871637857168802;

            // Act
            var transform = Transform.PlaneToPlane(pA, pB);

            // Assert
            _testOutput.WriteLine(transform.ToString());
            _testOutput.WriteLine(expectedXForm.ToString());
            transform.Equals(expectedXForm).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Translation_Matrix_From_A_Vector()
        {
            //Arrange
            var ptA = new Point3(0.3, 4, 1.2);
            var ptB = new Point3(12, 7.5, 2);
            var vec = new Vector3(ptB - ptA);
            var expectedMatrix = new TransformMatrix()
            {
                M00 = 1,
                M10 = 0,
                M20 = 0,
                M30 = 11.7,
                M01 = 0,
                M11 = 1,
                M21 = 0,
                M31 = 3.5,
                M02 = 0,
                M12 = 0,
                M22 = 1,
                M32 = 0.8,
                M03 = 0,
                M13 = 0,
                M23 = 0,
                M33 = 1
            };

            //Act
            var translation = Transform.Translation(vec);

            //Assert
            translation.Equals(expectedMatrix).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Translation_Matrix_From_Two_Points()
        {
            //Arrange
            var ptA = new Point3(0.3, 4, 1.2);
            var ptB = new Point3(12, 7.5, 2);
            var expectedMatrix = new TransformMatrix()
            {
                M00 = 1,
                M10 = 0,
                M20 = 0,
                M30 = 11.7,
                M01 = 0,
                M11 = 1,
                M21 = 0,
                M31 = 3.5,
                M02 = 0,
                M12 = 0,
                M22 = 1,
                M32 = 0.8,
                M03 = 0,
                M13 = 0,
                M23 = 0,
                M33 = 1
            };

            //Act
            var translation = Transform.Translation(ptA, ptB);

            //Assert
            translation.Equals(expectedMatrix).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Translation_Matrix_X_Y_Z_Values()
        {
            //Arrange
            var x = 11.7;
            var y = 3.5;
            var z = 0.8;
            var expectedMatrix = new TransformMatrix()
            {
                M00 = 1,
                M10 = 0,
                M20 = 0,
                M30 = 11.7,
                M01 = 0,
                M11 = 1,
                M21 = 0,
                M31 = 3.5,
                M02 = 0,
                M12 = 0,
                M22 = 1,
                M32 = 0.8,
                M03 = 0,
                M13 = 0,
                M23 = 0,
                M33 = 1
            };

            //Act
            var translation = Transform.Translation(x,y,z);

            //Assert
            translation.Equals(expectedMatrix).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Rotation_Matrix_From_Angle_And_Center_Point()
        {
            //Arrange
            var degrees = GSharkMath.ToRadians(30);
            var centerPoint = new Point3(5, 5, 2);

            var expectedMatrix = new TransformMatrix()
            {
                M00 = 0.8660254037844387,
                M10 = -0.49999999999999994,
                M30 = 3.169872981077806,
                M01 = 0.49999999999999994,
                M11 = 0.8660254037844387,
                M31 = -1.8301270189221928,
            };

            //Act
            var rotationMatrix = Transform.Rotation(degrees, centerPoint);

            //Assert
#if DEBUG
            _testOutput.WriteLine(rotationMatrix.ToString());
#endif
            rotationMatrix.Equals(expectedMatrix).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_Rotation_Matrix_From_Angle_Axis_And_Center_Point()
        {
            //Arrange
            var axis = new Vector3(5, 15.6, 2);
            var degrees = GSharkMath.ToRadians(30);
            var centerPoint = new Point3(5, 5, 2);

            var expectedMatrix = new TransformMatrix()
            {
                M00 = 0.8783229691589027,
                M10 = -0.0222254166980732,
                M20 = 0.477550827347714,
                M30 = -0.23558941699957536,
                M01 = 0.09896222463472895,
                M11 = 0.9857348241656216,
                M21 = -0.1361371900786715,
                M31 = -0.15121086384441007,
                M02 = -0.46771277504814274,
                M12 = 0.16683191325333382,
                M22 = 0.8679930142443529,
                M32 = 1.7684182804853388,
                M03 = 0,
                M13 = 0,
                M23 = 0,
                M33 = 1
            };

            //Act
            var rotationMatrix = Transform.Rotation(degrees, centerPoint, axis);

            //Assert
#if DEBUG
            _testOutput.WriteLine(rotationMatrix.ToString());
#endif
            rotationMatrix.Equals(expectedMatrix).Should().BeTrue();
        }

    }
}
