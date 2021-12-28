using System;
using System.Collections.Generic;

namespace ByteSize
{
	public class OneTypeManyInstancesDependency<TReference> : Dependency<TReference> where TReference : class, IReferencable
	{
		public bool HasAnyReference => _references.Count > 0;
		
		private List<TReference> _references;

		public OneTypeManyInstancesDependency() => 
			_references = new List<TReference>();

		public bool TryGetReferences(out IReadOnlyList<TReference> references)
		{
			if (!HasAnyReference)
			{
				references = null;
				return false;
			}

			references = _references;
			return true;
		}

		public void Relay(Action<TReference> relayedMethod)
		{
			if (!HasAnyReference)
				return;
			
			_references.Relay(relayedMethod);
		}
		
		protected override void OnReferencesUpdated()
		{
			if (!Entity.TryGetReferences<TReference>(out var refreshedReferences))
			{
				foreach (var reference in _references)
					InvokeReferenceLost(reference);
				
				_references.Clear();
				return;
			}

			foreach (var reference in _references)
			{
				if (!refreshedReferences.Contains(reference))
					InvokeReferenceLost(reference);
			}

			foreach (var reference in refreshedReferences)
			{
				if (!_references.Contains(reference))
					InvokeReferenceAcquired(reference);
			}
		}
	}
}