using UnityEditor;
using UnityEngine;

namespace ByteSize.Editor
{
	public sealed class AlphaNumericalSeparationNamingRule : StandardNamingRule
	{
		public override string Name => "Alpha numerical separation";
        
		private char _separator;
        
		protected override void DrawInScope() =>
			_separator = EditorGUILayout.TextField(GUIContent.none, _separator.ToString())[0];

		public override string Process(string name)
		{
			for (var i = 0; i < name.Length - 1; i++)
			{
				if (!char.IsLetter(name[i]) || !char.IsDigit(name[i + 1]))
					continue;
				
				name = name.Insert(i + 1, _separator.ToString());
				i++;
			}

			return name;
		}
	}
}