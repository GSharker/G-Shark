using System;
using System.Collections.Generic;
using VerbNurbsSharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
    public class TrigTest
    {
        private readonly ITestOutputHelper _testOutput;

        public TrigTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void ThreePointsAreFlat_ReturnTrue()
        {
            Point p1 = new Point() { 0.0, 0.0, 0.0 };
            Point p2 = new Point() { 10.0, 0.0, 0.0 };
            Point p3 = new Point() { 5.0, 5.0, 0.0 };
            Point p4 = new Point() { -5.0, -15.0, 0.0 };
            List<Point> points = new List<Point>(){p1,p2,p3,p4};

            Assert.True(Trig.ArePointsCoplanar(points));
        }
    }
}
