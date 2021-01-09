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
    [Trait("Category", "Knot")]
    public class KnotArrayTest
    {
        private readonly ITestOutputHelper _testOutput;

        public KnotArrayTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void GenerateAnEqualSpaceKnotArray()
        {
            var result = new KnotArray(3, 8);
            result.AsValidRelations(3, 8).Should().BeTrue();
            _testOutput.WriteLine(result.ToString());
        }
    }
}
