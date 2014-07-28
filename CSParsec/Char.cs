using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSParsec
{
	public static class Char
	{
		public static Parser<Unit> Spaces()
		{
			return Space().SkipMany();
		}

		public static Parser<char> Space()
		{
			return Satisfy(char.IsWhiteSpace);
		}

		public static Parser<char> Newline()
		{
			return Character('\n');
		}

		public static Parser<char> Tab()
		{
			return Character('\t');
		}

		public static Parser<char> Upper()
		{
			return Satisfy(char.IsUpper);
		}

		public static Parser<char> Lower()
		{
			return Satisfy(char.IsLower);
		}

		public static Parser<char> AlphaNum()
		{
			return Letter().Or(Digit());
		}

		public static Parser<char> Letter()
		{
			return Upper().Or(Lower());
		}

		public static Parser<char> Digit()
		{
			return Satisfy(char.IsDigit);
		}

		public static Parser<char> HexDigit()
		{
			return Digit()
				.Or(BetweenChar('a', 'f'))
				.Or(BetweenChar('A', 'F'));
		}

		public static Parser<char> OctDigit()
		{
			return BetweenChar('0', '7');
		}

		public static Parser<char> Character(char c)
		{
			return Satisfy(cc => c == cc);
		}

		public static Parser<IEnumerable<char>> String(string str)
		{
			return str.Select(c => Character(c)).Sequence();
		}

		public static Parser<char> AnyChar()
		{
			return Satisfy(any => true);
		}

		public static Parser<char> OneOf(IEnumerable<char> option)
		{
			return Satisfy(cc => option.Any(c => c == cc));
		}

		public static Parser<char> NoneOf(IEnumerable<char> option)
		{
			return Satisfy(cc => option.All(c => c != cc));
		}

		public static Parser<char> Satisfy(Predicate<char> pred)
		{
			return input =>
				{
					if (input.AtEnd || !pred(input.Current))
					{
						throw new ParseException();
					}
					else
					{
						return new Result<char>(input.Current, input.Advance());
					}
				};
		}

		// --- --- --- //

		public static Parser<char> CharacterIgnoreCase(char c)
		{
			return Satisfy(cc => System.Char.ToLower(c) == System.Char.ToLower(cc));
		}

		public static Parser<IEnumerable<char>> StringIgnoreCase(string str)
		{
			return str.Select(c => CharacterIgnoreCase(c)).Sequence();
		}

		public static Parser<IEnumerable<char>> GenericNewline()
		{
			return String("\n").Or(String("\r\n")).Or(String("\r"));
		}

		public static Parser<char> BetweenChar(char a, char b)
		{
			return Satisfy(c => a <= c && c <= b);
		}

		public static Parser<string> Text(this Parser<IEnumerable<char>> parser)
		{
			return parser.Select(str => new string(str.ToArray()));
		}

		public static Parser<int> Number()
		{
			return Digit().Many1().Text().Select(x => int.Parse(x));
		}
	}
}
