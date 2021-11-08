using System;
using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using System.Collections.Generic;
using System.Linq;
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
        public void It_Returns_The_Rotation_Matrix_To_Rotate_An_Object_About_An_Axis_And_Center_Point()
        {
            //Arrange
            var axis = Vector3.ZAxis;
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
            var rotationMatrix = Transform.Rotation(axis, degrees, centerPoint);

            //Assert
#if DEBUG
            _testOutput.WriteLine(rotationMatrix.ToString());
#endif
            rotationMatrix.Equals(expectedMatrix).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Scaling_Matrix_To_Scale_An_Object_Non_Uniformly_From_A_Center_Point()
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
        public void It_Returns_A_PlaneToPlane_TransformMatrix()
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
    }
}
