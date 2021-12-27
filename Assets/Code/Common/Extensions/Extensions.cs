using System;
using System.Collections.Generic;

namespace ByteSize
{
	public static class Extensions
	{
		public static void Relay<T>(this IEnumerable<T> source, Action<T> relayedMethod)
		{
			foreach (var item in source)
				relayedMethod(item);
		}
	}
}