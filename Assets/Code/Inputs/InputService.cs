using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Object = UnityEngine.Object;

namespace ByteSize
{
	public class InputService
	{
		private const string GAMEPLAY_MAP_NAME = "Gameplay";
		
		private InputActionAsset _collectionCopy;
		private InputRepository _repository;

		private InputActionMap _gameplayMap;
		private Dictionary<InputKind, InputAction> _inputs;

		public InputService(InputActionAsset collection, InputRepository repository)
		{
			_repository = repository;
			
			_collectionCopy = Object.Instantiate(collection);
			_collectionCopy.Enable();
			
			_gameplayMap = _collectionCopy.FindActionMap(GAMEPLAY_MAP_NAME, true);
		
			_inputs = new Dictionary<InputKind, InputAction>();
			foreach (var mapping in repository)
			{
				if (_inputs.ContainsKey(mapping.Kind))
					throw new InvalidOperationException($"`{nameof(InputService)}`: the same `{nameof(InputKind)}` has been encountered twice.");

				_inputs.Add(mapping.Kind, mapping.GetInputFrom(_collectionCopy));
			}
		}

		public void EnableGameplayInputs() =>
			_gameplayMap.Enable();
		public void DisableGameplayInputs() => 
			_gameplayMap.Disable();

		public bool TryGetInput(InputKind kind, out InputAction input) => 
			_inputs.TryGetValue(kind, out input);
	}
}