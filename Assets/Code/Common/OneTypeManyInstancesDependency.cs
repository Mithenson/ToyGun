using System.Collections.Generic;

namespace ByteSize
{
	public class OneTypeManyInstancesDependency<TReference> : Dependency<TReference> where TReference : class, IReferencable
	{
		private List<TReference> _references;

		public OneTypeManyInstancesDependency() => 
			_references = new List<TReference>();
		
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