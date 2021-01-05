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
    public class MatrixTest
    {
        private readonly ITestOutputHelper _testOutput;
        public MatrixTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        
        [Trait("Category", "Matrix")]
        [Fact]
        public void Create_Identity_Matrix()
        {
           int i = 3;
           var identity = new Matrix() {
               new List<double>{1, 0, 0 },
               new List<double>{0, 1, 0 },
               new List<double>{0, 0, 1 }
           };

           Matrix.Identity(i).Should().BeEquivalentTo<Matrix>(identity);
        }

    }
}
