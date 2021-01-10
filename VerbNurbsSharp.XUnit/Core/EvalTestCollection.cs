using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.XUnit.Core
{
	public class EvalTestCollection
	{
		public static IEnumerable<object[]> KnotSpanData =>
			new List<object[]>
			{
				new object[] { 2, 3.5, new KnotArray() { 0, 0, 0, 1, 2, 3, 4, 5, 6, 6, 6 }, 5 },
				new object[] { 2, 2.5, new KnotArray() { 0, 0, 0, 1, 2, 3, 3, 3 }, 4 },
				new object[] { 2, 4.4, new KnotArray() { 0, 0, 0, 1, 2, 3, 4, 5, 6, 6, 6 }, 6 },
				new object[] { 2, 5.9, new KnotArray() { 0, 0, 0, 1, 2, 3, 4, 5, 6, 6, 6 }, 7 },
				new object[] { 3, 5.5, new KnotArray() { 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 6, 6, 6 }, 8 },
				new object[] { 3, 6.5, new KnotArray() { 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 6, 6, 6 }, 8 },
				new object[] { 3, 1.9, new KnotArray() { 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 6, 6, 6 }, 4 },
				new object[] { 3, 1.9, new KnotArray() { 0, 0, 0, 0, 1, 2, 3, 3, 3, 3 }, 4 }
			};
	}
}
