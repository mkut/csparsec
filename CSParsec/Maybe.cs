using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSParsec
{
	public class Maybe<T>
	{
		private T value;
		public readonly bool IsJust;

		public T Value
		{
			get
			{
				if (!IsJust)
				{
					throw new InvalidOperationException();
				}
				return value;
			}
		}

		internal Maybe()
		{
			IsJust = false;
		}
		internal Maybe(T value)
		{
			this.value = value;
			IsJust = true;
		}
	}

	public static class Maybe
	{
		public static Maybe<T> Just<T>(T value)
		{
			return new Maybe<T>(value);
		}
		public static Maybe<T> Nothing<T>()
		{
			return new Maybe<T>();
		}
	}
}
