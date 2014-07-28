using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSParsec
{
	public interface IInput
	{
		IInput Advance();

		string Source { get; }
		char Current { get; }
		bool AtEnd { get; }
		int Position { get; }
		int Line { get; }
		int Column { get; }
	}
}
