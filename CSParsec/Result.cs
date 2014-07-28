using System;

namespace CSParsec
{

	public class Result<T>
	{
		public readonly T Value;
		public readonly IInput Rest;

		public Result(T value, IInput rest)
		{
			this.Value = value;
			this.Rest = rest;
		}
	}

	public delegate Result<T> Parser<T>(IInput input);

	public static class Parser
	{
		public static Parser<T> Return<T>(T value)
		{
			return input => new Result<T>(value, input);
		}
		public static Parser<U> Bind<T, U>(this Parser<T> parser, Func<T, Parser<U>> f)
		{
			return input =>
			{
				Result<T> result1 = parser(input);
				return f(result1.Value)(result1.Rest);
			};
		}
		public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> converter)
		{
			return parser.Bind<T, U>(t => Parser.Return(converter(t)));
		}
		public static Parser<V> SelectMany<T, U, V>(this Parser<T> parser, Func<T, Parser<U>> selector, Func<T, U, V> projector)
		{
			return parser.Bind<T, V>(t => selector(t).Select<U, V>(u => projector(t, u)));
		}
		public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)
		{
			return input =>
				{
					Result<T> result = parser(input);
					if (!predicate(result.Value))
					{
						throw new ParseException();
					}
					return result;
				};
		}
	}
}