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
	public class ModifyTest
	{
		private readonly ITestOutputHelper _testOutput;

		public ModifyTest(ITestOutputHelper testOutput)
		{
			_testOutput = testOutput;
		}

		[Theory]
		[MemberData(nameof(ModifyTestCollection.CurveKnotRefindData), MemberType = typeof(ModifyTestCollection))]
		public void Check_CurveKnotRefine(
			int inputDegree,
			KnotArray inputKnots,
			List<Vector> inputControlPts,
			List<double> knotsToInsert,
			int outputDegree,
			KnotArray outputKnots,
			List<Vector> outputControlPts)
		{
			//arrange
			NurbsCurve inputCurve = new NurbsCurve(inputDegree, inputKnots, inputControlPts);

			//act
			NurbsCurve expectedCurve = Evaluation.Modify.CurveKnotRefine(inputCurve, knotsToInsert);

			//assert
			Assert.Equal(outputDegree, expectedCurve.Degree);
			Assert.Equal(outputKnots, expectedCurve.Knots);
			Assert.Equal(outputControlPts, expectedCurve.ControlPoints);
		}
	}


}
