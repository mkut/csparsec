using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSParsec
{
	public static class Prim
	{
		public static T Parse<T>(this Parser<T> parser, string input)
		{
			Result<T> result = parser(new StringInput(input));

			return result.Value;
		}
	}
}
