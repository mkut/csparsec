using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSParsec;

namespace CSParsec.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			string str = Char.String("foo").Or(Char.String("bar")).Text().Parse("bar");
			System.Console.WriteLine(str);
		}
	}
}
