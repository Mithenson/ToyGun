using UnityEngine;

namespace ByteSize
{
	public class EntityMark : MonoBehaviour
	{
		public LookupId Lookup => _lookup;
		
		[SerializeField]
		private LookupId _lookup;
	}
}