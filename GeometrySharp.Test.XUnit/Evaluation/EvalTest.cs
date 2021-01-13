using System;
using System.Collections.Generic;
using FluentAssertions;
using GeometrySharp.Core;
using GeometrySharp.Evaluation;
using GeometrySharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace GeometrySharp.XUnit.Evaluation
{
    [Trait("Category", "Eval")]
    public class EvalTest
    {
        private readonly ITestOutputHelper _testOutput;

        public EvalTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        } 

    }
}
