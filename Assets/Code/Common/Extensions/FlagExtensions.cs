using System;
using ToyGun;
using UnityEngine;

namespace ByteSize
{
	public static class FlagExtensions
	{
		public static TEnum AddFlags<TEnum>(this TEnum value, TEnum addition) where TEnum : Enum
		{
			var asInt = Convert.ToInt32(value);
			asInt |= Convert.ToInt32(addition);

			return (TEnum)Enum.ToObject(typeof(TEnum), asInt);
		}
		public static TEnum RemoveFlags<TEnum>(this TEnum value, TEnum removal) where TEnum : Enum
		{
			var valueAsInt = Convert.ToInt32(value);
			var removalAsInt =  Convert.ToInt32(removal);
			
			valueAsInt |= removalAsInt;
			valueAsInt ^= removalAsInt;

			return (TEnum)Enum.ToObject(typeof(TEnum), valueAsInt);
		}
	}
}