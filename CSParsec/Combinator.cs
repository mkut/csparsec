using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSParsec
{
	public static class Combinator
	{
		public static Parser<T> Choice<T>(this IEnumerable<Parser<T>> parsers)
		{
			return input =>
				{
					foreach (Parser<T> parser in parsers)
					{
						try
						{
							return parser(input);
						}
						catch (ParseException)
						{ }
					}
					throw new ParseException();
				};
		}

		public static Parser<IEnumerable<T>> Count<T>(this Parser<T> parser, int n)
		{
			return Enumerable.Repeat(parser, n).Sequence();
		}

		public static Parser<T> Between<Open, Close, T>(Parser<Open> open, Parser<Close> close, Parser<T> parser)
		{
			return from o in open
					 from x in parser
					 from c in close
					 select x;
		}

		public static Parser<T> Option<T>(this Parser<T> parser, T defaultValue)
		{
			return input =>
				{
					try
					{
						return parser(input);
					}
					catch (ParseException)
					{
						return new Result<T>(defaultValue, input);
					}
				};
		}

		public static Parser<Maybe<T>> OptionMaybe<T>(this Parser<T> parser)
		{
			return input =>
				{
					try
					{
						Result<T> result = parser(input);
						return new Result<Maybe<T>>(Maybe.Just<T>(result.Value), result.Rest);
					}
					catch (ParseException)
					{
						return new Result<Maybe<T>>(Maybe.Nothing<T>(), input);
					}
				};
		}

		public static Parser<Unit> Optional<T>(this Parser<T> parser)
		{
			return from x in OptionMaybe(parser)
					 select Unit.U;
		}

		public static Parser<Unit> SkipMany<T>(this Parser<T> parser)
		{
			return from skip in parser.Many()
					 select Unit.U;
		}

		public static Parser<Unit> SkipMany1<T>(this Parser<T> parser)
		{
			return from skip in parser.Many1()
					 select Unit.U;
		}

		public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
		{
			return input =>
			{
				IInput i = input;
				List<T> ret = new List<T>();
				while (true)
				{
					try
					{
						Result<T> result = parser(i);
						ret.Add(result.Value);
						i = result.Rest;
					}
					catch (ParseException)
					{
						return new Result<IEnumerable<T>>(ret, i);
					}
				}
			};
		}

		public static Parser<IEnumerable<T>> Many1<T>(this Parser<T> parser)
		{
			return Sequence(
				new Parser<IEnumerable<T>>[] { parser.Once(), Many<T>(parser) })
					.Select(xs => xs.SelectMany(x => x));
		}

		public static Parser<IEnumerable<T>> SepBy<T, Sep>(this Parser<T> parser, Parser<Sep> separator)
		{
			Parser<T> sepp =
				from _sep in separator
				from _p in parser
				select _p;
			return Subsequence(new Parser<IEnumerable<T>>[] { parser.Once(), sepp.Many() })
				.Select(xs => xs.SelectMany(x => x));
		}

		public static Parser<IEnumerable<T>> SepBy1<T, Sep>(this Parser<T> parser, Parser<Sep> separator)
		{
			Parser<T> sepp =
				from _sep in separator
				from _p in parser
				select _p;
			return Sequence(new Parser<IEnumerable<T>>[] { parser.Once(), sepp.Many() })
				.Select(xs => xs.SelectMany(x => x));
		}

		public static Parser<IEnumerable<T>> EndBy<T, Sep>(this Parser<T> parser, Parser<Sep> separator)
		{
			Parser<T> sepp =
				from _sep in separator
				from _p in parser
				select _p;
			return sepp.Many();
		}

		public static Parser<IEnumerable<T>> EndBy1<T, Sep>(this Parser<T> parser, Parser<Sep> separator)
		{
			Parser<T> sepp =
				from _sep in separator
				from _p in parser
				select _p;
			return sepp.Many1();
		}

		public static Parser<IEnumerable<T>> SepEndBy<T, Sep>(this Parser<T> parser, Parser<Sep> separator)
		{
			Parser<T> sepp =
				from _sep in separator
				from _p in parser
				select _p;
			return Subsequence(new Parser<IEnumerable<T>>[] { parser.Once(), sepp.Many(), separator.Optional().Select(any => Enumerable.Empty<T>()) })
				.Select(xs => xs.SelectMany(x => x));
		}

		public static Parser<IEnumerable<T>> SepEndBy1<T, Sep>(this Parser<T> parser, Parser<Sep> separator)
		{
			Parser<T> sepp =
				from _sep in separator
				from _p in parser
				select _p;
			return Sequence(new Parser<IEnumerable<T>>[] { parser.Once(), sepp.Many(), separator.OptionMaybe().Select(any => Enumerable.Empty<T>()) })
				.Select(xs => xs.SelectMany(x => x));
		}

		// TODO
		public static Parser<T> ChainL<T>(this Parser<T> parser, Parser<Func<T, T, T>> op, T defaultValue)
		{
			return input =>
				{
					try
					{
						Result<T> result = parser(input);
						IInput i = result.Rest;
						T acc = result.Value;
						while (true)
						{
							try
							{
								Result<Func<T, T, T>> resultOp = op(i);
								result = parser(resultOp.Rest);
								acc = resultOp.Value(acc, result.Value);
								i = result.Rest;
							}
							catch (ParseException)
							{
								return new Result<T>(acc, i);
							}
						}
					}
					catch (ParseException)
					{
						return new Result<T>(defaultValue, input);
					}
				};
		}

		// TODO
		public static Parser<T> ChainL1<T>(this Parser<T> parser, Parser<Func<T, T, T>> op)
		{
			return input =>
			{
				Result<T> result = parser(input);
				IInput i = result.Rest;
				T acc = result.Value;
				while (true)
				{
					try
					{
						Result<Func<T, T, T>> resultOp = op(i);
						result = parser(resultOp.Rest);
						acc = resultOp.Value(acc, result.Value);
						i = result.Rest;
					}
					catch (ParseException)
					{
						return new Result<T>(acc, i);
					}
				}
			};
		}

		// TODO
		// public static Parser<T> ChainR<T>(this Parser<T> parser, Parser<Func<T, T, T>> op, T defaultValue)

		// TODO
		// public static Parser<T> ChainR1<T>(this Parser<T> parser, Parser<Func<T, T, T>> op)

		// TODO
		public static Parser<Unit> Eof()
		{
			return input =>
				{
					if (!input.AtEnd)
					{
						throw new ParseException();
					}
					return new Result<Unit>(Unit.U, input);
				};
		}

		// TODO
		// notFollowedBy
		// manyTill
		// lookAhead
		// anyToken



		public static Parser<T> Or<T>(this Parser<T> parser1, Parser<T> parser2)
		{
			return input =>
				{
					try
					{
						return parser1(input);
					}
					catch (ParseException)
					{
						return parser2(input);
					}
				};
		}

		public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
		{
			return parser.Select(x => Enumerable.Empty<T>().Concat(new T[] { x }));
		}

		public static Parser<IEnumerable<T>> Sequence<T>(this IEnumerable<Parser<T>> parsers)
		{
			return parsers.Aggregate(
				Parser.Return(Enumerable.Empty<T>()),
				(xs, p) => xs.SelectMany(any => p, (ys, y) => ys.Concat(new T[] { y })));
		}

		public static Parser<IEnumerable<T>> Subsequence<T>(this IEnumerable<Parser<T>> parsers)
		{
			return input =>
				{
					IInput i = input;
					List<T> ret = new List<T>();
					foreach (Parser<T> parser in parsers)
					{
						try
						{
							Result<T> result = parser(i);
							ret.Add(result.Value);
							i = result.Rest;
						}
						catch (ParseException)
						{
							break;
						}
					}
					return new Result<IEnumerable<T>>(ret, i);
				};
		}
	}
}
