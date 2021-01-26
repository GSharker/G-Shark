using FluentAssertions;
using GeometrySharp.Core;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.Test.XUnit.Core
{
    [Trait("Category", "BinaryHeap")]
    public class BinaryHeapTests
    {
        private readonly ITestOutputHelper _testOutput;

        public BinaryHeapTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

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
