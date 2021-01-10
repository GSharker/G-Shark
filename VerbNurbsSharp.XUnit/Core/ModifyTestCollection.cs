using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VerbNurbsSharp.Core;

namespace VerbNurbsSharp.XUnit.Core
{
	public class ModifyTestCollection
	{
		public static IEnumerable<object[]> CurveKnotRefindData =>
		new List<object[]>
		{
				new object[] {
					3,
					new KnotArray() { 0, 0, 0, 1, 2, 3, 3, 3 },
					new List<Vector>() {
						new Vector() { -11.652072, -3.739098, 0 },
						new Vector() { -4.034747, -0.191303, 0 },
						new Vector() { 1.669551, 0, 5.965165 },
						new Vector() { 5.843427, -9.895566, 0 },
						new Vector() { 8.556447, 0, -7.565151 },
						new Vector() { 18.608533, 0, 2.834758 }
					},
					new List<double>(){ },
					3,
					new KnotArray() { 0, 0, 0, 1, 2, 3, 3, 3 },
					new List<Vector>() {
						new Vector() { -11.652072, -3.739098, 0 },
						new Vector() { -4.034747, -0.191303, 0 },
						new Vector() { 1.669551, 0, 5.965165 },
						new Vector() { 5.843427, -9.895566, 0 },
						new Vector() { 8.556447, 0, -7.565151 },
						new Vector() { 18.608533, 0, 2.834758 }
					}
				},
				new object[] {
					3,
					new KnotArray(){ 0, 0, 0, 0, 1, 2, 3, 3, 3, 3 },
					new List<Vector>() {
						new Vector() { -11.652072, -3.739098, 0 },
						new Vector() { -4.034747, -0.191303, 0 },
						new Vector() { 1.669551, 0, 5.965165 },
						new Vector() { 5.843427, -9.895566, 0 },
						new Vector() { 8.556447, 0, -7.565151 },
						new Vector() { 18.608533, 0, 2.834758 }
					},
					new List<double>() { 1.9 },
					3,
					new KnotArray() { 0, 0, 0, 0, 1, 1.9, 2, 3, 3, 3, 3 },
					new List<Vector>() {
						new Vector() { -11.652072, -3.739098, 0 },
						new Vector() { -4.034747, -0.191303, 0 },
						new Vector() { 1.669551, 0, 5.965165 },
						new Vector() { 5.843427, -9.895566, 0 },
						new Vector() { 8.556447, 0, -7.565151 },
						new Vector() { 18.608533, 0, 2.834758 }
					}
				}
		};
	}
}
