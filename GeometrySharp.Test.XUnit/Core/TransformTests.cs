using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GeometrySharp.Core;
using Xunit;

namespace GeometrySharp.Test.XUnit.Core
{
    public class TransformTests
    {
        [Fact]
        public void It_Returns_An_Instance_Of_Transform()
        {
            var transform = new Transform();

            transform.Should().NotBeNull();
            transform.Count.Should().Be(4);
            transform[0].Count.Should().Be(4);
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
    }
}
