using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GShark.Core;
using GShark.Enumerations;
using GShark.Geometry;
using GShark.Test.XUnit.Data;
using Xunit;

namespace GShark.Test.XUnit.Modify
{
    public class SurfaceTests
    {
        [Theory]
        [InlineData(0.3, 1)]
        [InlineData(0.3, 2)]
        [InlineData(0.3, 3)]
        [InlineData(0.3, 4)]
        [InlineData(0.45, 1)]
        [InlineData(0.45, 2)]
        [InlineData(0.45, 3)]
        [InlineData(0.45, 4)]
        [InlineData(0.7, 1)]
        [InlineData(0.7, 2)]
        public void It_Refines_The_Surface_In_The_U_Direction(double val, int insertion)
        {
            // Arrange
            List<double> newKnots = new List<double>();
            for (int i = 0; i < insertion; i++)
                newKnots.Add(val);
            NurbsSurface surface = NurbsSurfaceCollection.Loft();

            // Act
            NurbsSurface surfaceAfterRefine = KnotVector.Refine(surface, newKnots, SurfaceDirection.U);
            Point3 p0 = surface.PointAt(0.5, 0.25);
            Point3 p1 = surfaceAfterRefine.PointAt(0.5, 0.25);

            // Assert
            (surface.KnotsU.Count + insertion).Should().Be(surfaceAfterRefine.KnotsU.Count);
            (surface.ControlPointLocations.Count + insertion).Should().Be(surfaceAfterRefine.ControlPointLocations.Count);

            (p0 == p1).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.3, 1)]
        [InlineData(0.3, 2)]
        [InlineData(0.3, 3)]
        [InlineData(0.3, 4)]
        [InlineData(0.45, 1)]
        [InlineData(0.45, 2)]
        [InlineData(0.45, 3)]
        [InlineData(0.45, 4)]
        [InlineData(0.7, 1)]
        [InlineData(0.7, 2)]
        public void It_Refines_The_Surface_In_The_V_Direction(double val, int insertion)
        {
            // Arrange
            List<double> newKnots = new List<double>();
            for (int i = 0; i < insertion; i++)
                newKnots.Add(val);
            NurbsSurface surface = NurbsSurfaceCollection.Loft();

            // Act
            NurbsSurface surfaceAfterRefine = KnotVector.Refine(surface, newKnots, SurfaceDirection.V);
            Point3 p0 = surface.PointAt(0.5, 0.25);
            Point3 p1 = surfaceAfterRefine.PointAt(0.5, 0.25);

            // Assert
            (surface.KnotsV.Count + insertion).Should().Be(surfaceAfterRefine.KnotsV.Count);
            (surface.ControlPointLocations[0].Count + insertion).Should().Be(surfaceAfterRefine.ControlPointLocations[0].Count);

            (p0 == p1).Should().BeTrue();
        }
    }
}
