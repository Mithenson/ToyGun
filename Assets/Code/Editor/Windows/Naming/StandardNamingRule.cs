using UnityEditor;
using UnityEngine;

namespace ByteSize.Editor
{
	public abstract class StandardNamingRule : INamingRule
	{
		public bool IsActive => _isActive;
		public abstract string Name { get; }

		protected bool _isActive;
        
		public void Draw()
		{
			using var prefixHorizontalScope = new EditorGUILayout.HorizontalScope();

			EditorGUILayout.LabelField(Name, GUILayout.Width(EditorGUIUtility.labelWidth));
			_isActive = GUILayout.Toggle(_isActive, GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight));
                
			DrawInScope();
		}
		protected abstract void DrawInScope();

		public abstract string Process(string name);
	}
}