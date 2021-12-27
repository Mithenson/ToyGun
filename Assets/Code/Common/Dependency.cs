using System;
using Zenject;

namespace ByteSize
{
	public abstract class Dependency : IDisposable
	{
		protected Entity Entity;

		[Inject]
		public void Inject(Entity entity)
		{
			Entity = entity;
			Entity.OnReferencesUpdated += OnReferencesUpdated;
		}

		protected abstract void OnReferencesUpdated();

		public void Dispose()
		{
			if (Entity != null)
				Entity.OnReferencesUpdated -= OnReferencesUpdated;
		}
	}
	public abstract class Dependency<TReference> : Dependency where TReference : class, IReferencable
	{
		public event Action<TReference> OnReferenceAcquired;
		public event Action<TReference> OnReferenceLost; 
		
		protected void InvokeReferenceAcquired(TReference reference) => 
			OnReferenceAcquired?.Invoke(reference);
		protected void InvokeReferenceLost(TReference reference) => 
			OnReferenceLost?.Invoke(reference);
	}
}