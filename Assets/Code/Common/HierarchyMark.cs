using UnityEngine;

namespace ByteSize
{
	public class HierarchyMark : MonoBehaviour
	{
		#region Nested types

		public enum Option
		{
			Entities = 0,
			Environment = 1
		}

		#endregion
		
		public Option Value => _value;
		
		[SerializeField]
		private Option _value;
	}
}