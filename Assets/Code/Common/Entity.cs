using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ByteSize
{
	public class Entity : Script
	{
		private const string CONTROLLERS_ROOT_NAME = "Controllers";
		private const string BEHAVIOURS_ROOT_NAME = "Behaviours";
		private const string PRESENTATIONS_ROOT_NAME = "Presentations";
		
		// For any given type, gives the currently known list of types that inherit from or implement this type
		private static readonly Dictionary<Type, List<Type>> _assignableTypes = new Dictionary<Type, List<Type>>();
		private static readonly HashSet<Type> _skippedTypes = new HashSet<Type>()
		{
			typeof(object),

			typeof(Script),
			typeof(MonoBehaviour),
			typeof(Behaviour),
			typeof(Component),

			typeof(ScriptableObject),

			typeof(Object),
		};

		public event Action OnReferencesUpdated;

		public Transform this[Root root] => root switch
		{
			Root.Controllers => _controllersRoot,
			Root.Behaviours => _behavioursRoot,
			Root.Presentations => _presentationsRoot,
			_ => throw new ArgumentOutOfRangeException("EXCEPTION"),
		};
		
		#if UNITY_EDITOR

		[ShowInInspector, ReadOnly]
		private Transform PresentationsRoot => _presentationsRoot;
		
		#endif
		
		private Transform _controllersRoot;
		private Transform _behavioursRoot;
		private Transform _presentationsRoot;
		
		// Keyed by concrete type
		private Dictionary<Type, List<IReferencable>> _references;
		
		protected virtual void Awake()
		{
			_controllersRoot = transform.Find(CONTROLLERS_ROOT_NAME);
			if (_controllersRoot == null)
				throw new InvalidDataContractException("EXCEPTION");
			
			_behavioursRoot = transform.Find(BEHAVIOURS_ROOT_NAME);
			if (_behavioursRoot == null)
				throw new InvalidDataContractException("EXCEPTION");
			
			_presentationsRoot = transform.Find(PRESENTATIONS_ROOT_NAME);
			if (_presentationsRoot == null)
				throw new InvalidDataContractException("EXCEPTION");
			
			_references = new Dictionary<Type, List<IReferencable>>();
			AddReferences(GetComponentsInChildren<IReferencable>(true));
		}

		#region Referencing

		public void AddReferences<TReference>(params TReference[] references) where TReference : class, IReferencable =>
			AddReferences(references.AsEnumerable());
		public void AddReferences<TReference>(IEnumerable<TReference> references) where TReference : class, IReferencable 
		{
			foreach (var reference in references)
				AddReferenceWithoutNotification(reference);
			
			OnReferencesUpdated?.Invoke();
		}
		public void AddReference<TReference>(TReference reference) where TReference : class, IReferencable =>
			AddReferenceWithNotification(reference);
		private void AddReferenceWithNotification<TReference>(TReference reference) where TReference : class, IReferencable 
		{
			AddReferenceWithoutNotification(reference);
			OnReferencesUpdated?.Invoke();
		}
		private void AddReferenceWithoutNotification<TReference>(TReference reference) where TReference : class, IReferencable 
		{
			var type = reference.GetType();
			
			if (!_assignableTypes.ContainsKey(type))
				CollectAssignableTypesFor(type);
			
			if (_references.ContainsKey(type))
				_references[type].Add(reference);
			else
			{
				var list = new List<IReferencable>() { reference };
				_references.Add(type, list);
			}

			reference.OnDisposed += OnReferenceDisposed;
		}

		private void OnReferenceDisposed(IReferencable reference)
		{
			var type = reference.GetType();
			
			if (!_references.TryGetValue(type, out var references))
				throw new KeyNotFoundException("EXCEPTION");

			reference.OnDisposed -= OnReferenceDisposed;
			references.Remove(reference);
			
			OnReferencesUpdated?.Invoke();
		}
		
		public bool TryGetReferences<TReference>(out List<TReference> results) where TReference : class, IReferencable 
		{
			results = new List<TReference>();
			return TryGetReferencesNonAlloc(results);
		}
		public bool TryGetReferencesNonAlloc<TReference>(List<TReference> results) where TReference : class, IReferencable 
		{
			results.Clear();

			if (!_assignableTypes.TryGetValue(typeof(TReference), out var list))
				return false;

			list.Add(typeof(TReference));
			
			foreach (var type in list)
			{
				if (!_references.TryGetValue(type, out var references))
					continue;

				foreach (var reference in references)
				{
					if (!reference.CanBeReferenced)
						continue;
					
					results.Add((TReference)reference);
				}
			}

			return results.Count > 0;
		}

		private void CollectAssignableTypesFor(Type type)
		{
			var interfaceTypes = type.GetInterfaces();
			
			foreach (var interfaceType in interfaceTypes)
			{
				if (interfaceType.IsGenericTypeDefinition || interfaceType == typeof(IReferencable))
					continue;

				if (_assignableTypes.TryGetValue(interfaceType, out var list))
					list.Add(type);
				else
				{
					list = new List<Type>() { type };
					_assignableTypes.Add(interfaceType, list);
				}
			}
			
			var current = type.BaseType;
			
			while (!current.IsGenericTypeDefinition && !_skippedTypes.Contains(current))
			{
				if (_assignableTypes.TryGetValue(current, out var list))
					list.Add(type);
				else
				{
					list = new List<Type>() { type };
					_assignableTypes.Add(current, list);
				}
				
				current = current.BaseType;
			}
		}

		#endregion

		#region Interaction

		public void Relay<TReference>(Action<TReference> relayedMethod) where TReference : class, IReferencable 
		{
			TryGetReferences<TReference>(out var results);
			results.Relay(relayedMethod);
		}
		public void RelayNonAlloc<TReference>(List<TReference> results, Action<TReference> relayedMethod) where TReference : class, IReferencable 
		{
			TryGetReferencesNonAlloc(results);
			results.Relay(relayedMethod);
		}

		public void PropagateOperationNonAlloc<TListener, TOperation>(TOperation operation)
			where TOperation : IOperation
			where TListener : class, IReferencable, IHandler<TOperation>
		{
			TryGetReferences<TListener>(out var listeners);
			PropagateOperation(listeners, operation);
		}
		public void PropagateOperationNonAlloc<TListener, TOperation>(List<TListener> listeners, TOperation operation)
			where TOperation : IOperation
			where TListener : class, IReferencable, IHandler<TOperation>
		{
			TryGetReferencesNonAlloc(listeners);
			PropagateOperation(listeners, operation);
		}
		private void PropagateOperation<TListener, TOperation>(List<TListener> listeners, TOperation operation)
			where TOperation : IOperation
			where TListener : IHandler<TOperation>
		{
			listeners.Sort((a, b) => a.Priority.CompareTo(b.Priority));
			
			foreach (var listener in listeners)
			{
				if (!listener.Handle(ref operation))
					break;
			}
		}

		#endregion

		public void Add(Root root, params GameObject[] prefabs)
		{
			var instances = new GameObject[prefabs.Length];
			var references = new List<IReferencable>();

			for (var i = 0; i < instances.Length; i++)
			{
				instances[i] = Spawn(root, prefabs[i]);
				references.AddRange(instances[i].GetComponentsInChildren<IReferencable>(true));
			}
			
			AddReferences(references);
		}
		public void Add(Root root, GameObject prefab)
		{
			var instance = Spawn(root, prefab);
			AddReferences(instance.GetComponentsInChildren<IReferencable>(true));
		}
		protected virtual GameObject Spawn(Root root, GameObject prefab) =>
			Instantiate(prefab, this[root]);
	}
}