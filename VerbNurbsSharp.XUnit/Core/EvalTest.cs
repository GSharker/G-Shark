using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Evaluation;
using Xunit;
using Xunit.Abstractions;


namespace VerbNurbsSharp.XUnit.Core
{
	public class EvalTest
	{
		private readonly ITestOutputHelper _testOutput;

		public EvalTest(ITestOutputHelper testOutput)
		{
			_testOutput = testOutput;
		}

		[Theory]
		[MemberData(nameof(EvalTestCollection.KnotSpanData), MemberType = typeof(EvalTestCollection))]
		public void Check_KnotSpan(
			int degree,
			double u,
			KnotArray knots,
			int expectedKnotIndex)
		{
			//act
			int knotIndex = Eval.KnotSpan(degree, u, knots);
			//assert
			Assert.Equal(expectedKnotIndex, knotIndex);
		}
	}


}
