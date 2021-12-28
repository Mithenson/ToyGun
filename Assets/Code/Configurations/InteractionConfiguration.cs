using UnityEngine;

namespace ByteSize
{
	[CreateAssetMenu(menuName = "ToyGun/Configurations/Interaction", fileName = "InteractionConfiguration")]
	public class InteractionConfiguration : Configuration
	{
		[field:SerializeField]
		public float InteractionRange { get; private set; }
	}
}