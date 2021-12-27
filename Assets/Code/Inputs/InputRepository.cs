using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ByteSize
{
	[CreateAssetMenu(fileName = "InputRepository", menuName = "Feud/Singletons/Input repository")]
	public class InputRepository : ScriptableObject, IEnumerable<InputRepository.Mapping>
	{
		#region Nested types

		[Serializable]
		public class Mapping
		{
			public InputKind Kind => _kind;

			[SerializeField]
			private InputKind _kind;

			[SerializeField]
			private InputActionReference _input;
			
			public InputAction GetInputFrom(InputActionAsset collection) =>
				collection.FindAction(_input.action.id);
		}

		private class Enumerator : IEnumerator<InputRepository.Mapping>
		{
			public Mapping Current => _source._mappings[_currentIndex];
			
			private InputRepository _source;
			private int _currentIndex = -1;
			
			object IEnumerator.Current => Current;
			
			public Enumerator(InputRepository source) => 
				_source = source;

			public bool MoveNext()
			{
				_currentIndex++;
				return _currentIndex < _source._mappings.Length;
			}
			public void Reset() =>
				_currentIndex = -1;
			
			public void Dispose() { }
		}

		#endregion

		[SerializeField]
		private Mapping[] _mappings;

		public IEnumerator<Mapping> GetEnumerator() => new Enumerator(this);
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}