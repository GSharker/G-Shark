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
	[Trait("Category", "Modify")]
	public class ModifyTest
	{
		private readonly ITestOutputHelper _testOutput;

		public ModifyTest(ITestOutputHelper testOutput)
		{
			_testOutput = testOutput;
		}
	}
}
