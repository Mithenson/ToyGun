using System;
using UnityEngine;
using Zenject;

namespace ByteSize
{
	public abstract class Script : MonoBehaviour, IReferencable
	{
		public event Action<IReferencable> OnDisposed;

		public bool CanBeReferenced => enabled;

		protected bool HasBeenInitialized;

		[Inject]
		public virtual void OnCreation() =>
			HasBeenInitialized = true;
		
		protected virtual void OnDestroy() =>
			OnDisposed?.Invoke(this);
	}
}