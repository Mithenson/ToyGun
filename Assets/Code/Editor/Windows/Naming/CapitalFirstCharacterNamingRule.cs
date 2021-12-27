using UnityEditor;
using UnityEngine;

namespace ByteSize.Editor
{
	public sealed class CapitalFirstCharacterNamingRule : StandardNamingRule
	{
		public override string Name => "Capital first character";

		private char _startIndicator;

		protected override void DrawInScope() =>
			_startIndicator = EditorGUILayout.TextField(GUIContent.none, _startIndicator.ToString())[0];

		public override string Process(string name)
		{
			var firstChar = name[0];
			firstChar = char.ToUpper(firstChar);

			name = name.Remove(0, 1).Insert(0, firstChar.ToString());

			if (_startIndicator == 0)
				return name;
			
			for (var i = 0; i < name.Length - 1; i++)
			{
				if (name[i] != _startIndicator)
					continue;

				var target = name[i + 1];
				if (!char.IsLetter(target))
					continue;
					
				name = name.Remove(i + 1, 1).Insert(i + 1, char.ToUpper(target).ToString());
				i++;
			}

			return name;
		}
	}
}