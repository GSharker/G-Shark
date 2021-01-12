using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Core
{
    public class BinaryHeapTest
    {
        private readonly ITestOutputHelper _testOutput;

        public BinaryHeapTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Trait("Category", "BinaryHeap")]
        [Fact]
        public void Binary_Heap_Pop()
        {
            BinaryHeap<double> bh = new BinaryHeap<double>();
            bh.Add(-1);
            bh.Add(12);
            bh.Add(10);
            bh.Add(-10);

            bh.Pop().Should().Be(-10);
        }
    }
}
