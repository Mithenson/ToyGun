using UnityEditor;
using UnityEngine;

namespace ByteSize.Editor
{
	public sealed class ReplacementNamingRule : StandardNamingRule
	{
		public override string Name => "Replacement";
        
		private string _old;
		private string _new;

		protected override void DrawInScope()
		{
			_old = EditorGUILayout.TextField(GUIContent.none, _old, GUILayout.MinWidth(0.0f));
			_new = EditorGUILayout.TextField(GUIContent.none, _new, GUILayout.MinWidth(0.0f));
		}

		public override string Process(string name) => 
			name.Replace(_old, _new);
	}
}