using System.Runtime.Serialization;
using UnityEngine;

namespace ByteSize
{
	public class EntityLink : MonoBehaviour
	{
		public Entity Value { get; private set; }

		private void OnAwake()
		{
			Value = GetComponentInParent<Entity>();

			if (Value == null)
				throw new InvalidDataContractException("EXCEPTION");
		}
	}
}