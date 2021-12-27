using UnityEditor;
using UnityEngine;

namespace ByteSize.Editor
{
	public sealed class PrefixNamingRule : StandardNamingRule
	{
		public override string Name => "Prefix";
        
		private string _prefix;
        
		protected override void DrawInScope() =>
			_prefix = EditorGUILayout.TextField(GUIContent.none, _prefix);

		public override string Process(string name) => 
			$"{_prefix}{name}";
	}
}