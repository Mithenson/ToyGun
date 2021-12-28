using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ByteSize
{
	public class OneTypeOneInstanceDependency<TReference> : Dependency<TReference> where TReference : class, IReferencable
	{
		public bool HasReference => _hasReference;
		
		private bool _hasReference;
		private TReference _reference;

		public bool TryGetReference(out TReference reference)
		{
			if (!_hasReference)
			{
				reference = null;
				return false;
			}

			reference = _reference;
			return true;
		}

		public void Relay(Action<TReference> relayedMethod)
		{
			if (!_hasReference)
				return;

			relayedMethod(_reference);
		}
		
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
				
				_hasReference = true;
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