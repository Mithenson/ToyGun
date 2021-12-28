using UnityEngine;

namespace ByteSize
{
	public static class Constants
	{
		public static class Physics
		{
			public const int HitBufferCapacity = 8;
			
			public static readonly LayerMask InteractableMask = LayerMask.GetMask(new string[] { "Interactable" });
		}
	}
}