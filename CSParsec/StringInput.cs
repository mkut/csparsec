using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSParsec
{
	public class StringInput : IInput
	{
		private readonly string source;
		private readonly int pos;
		private readonly int line;
		private readonly int column;

		public StringInput(string source)
			: this(source, 0, 1, 1, false)
		{
		}
		public StringInput(string source, int pos, int line, int column, bool prevCr)
		{
			this.source = source;
			this.pos = pos;
			bool newLine = prevCr && (AtEnd || Current != '\n');
			this.line = newLine ? line + 1 : line;
			this.column = newLine ? 0 : column;
		}

		public IInput Advance()
		{
			if (AtEnd)
			{
				throw new InvalidOperationException();
			}
			return new StringInput(source, pos + 1, Current == '\n' ? line + 1 : line, Current == '\n' ? 0 : column + 1, Current == '\r');
		}

		public string Source
		{
			get
			{
				return source;
			}
		}
		public char Current
		{
			get
			{
				return source[pos];
			}
		}
		public bool AtEnd
		{
			get
			{
				return pos >= source.Length;
			}
		}
		public int Position
		{
			get
			{
				return pos;
			}
		}
		public int Line
		{
			get
			{
				return line;
			}
		}
		public int Column
		{
			get
			{
				return column;
			}
		}
	}
}
