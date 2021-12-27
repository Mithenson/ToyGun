using System.Collections.Generic;
using System.Linq;

namespace ByteSize
{
	public class OneTypeOneInstanceDependency<TReference> : Dependency<TReference> where TReference : class, IReferencable
	{
		private bool _hasReference;
		private TReference _reference;
		
		protected override void OnReferencesUpdated()
		{
			if (!Entity.TryGetReferences<TReference>(out var candidates))
			{
				OnNoReferenceFound();
				return;
			}

			if (!TrySelectReference(candidates, out var selection))
				OnNoReferenceFound();
			else
			{
				if (!_hasReference || _reference != selection)
					InvokeReferenceAcquired(selection);
				
				_reference = selection;
			}
			
			void OnNoReferenceFound()
			{
				if (_hasReference)
					InvokeReferenceLost(_reference);
				
				_hasReference = false;
				_reference = null;
			}
		}

		protected virtual bool TrySelectReference(List<TReference> candidates, out TReference selection)
		{
			selection = candidates.First();
			return true;
		}
	}
}