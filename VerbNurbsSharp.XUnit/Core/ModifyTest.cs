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

		//[Fact]
		//public void Check_CurveKnotRefine(
		//	int inputDegree, 
		//	List<double> inputKnots, 
		//	List<Vector> inputControlPts, 
		//	List<double>knotsToInsert,
		//	int outputDegree,
		//	List<double> outputKnots,
		//	List<Vector> outputControlPts)
		//{
		//	//arrange
		//	NurbsCurve inputCurve = new NurbsCurve(inputDegree, (KnotArray)inputKnots, inputControlPts);
			
		//	//act
		//	NurbsCurve expectedCurve = Evaluation.Modify.CurveKnotRefine(inputCurve, knotsToInsert);
		//	//assert
		//	Assert.Equal(expectedCurve.Degree, inputCurve.Degree);
		//}
	}
}
