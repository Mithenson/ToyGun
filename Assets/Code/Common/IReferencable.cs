using System;

namespace ByteSize
{
	public interface IReferencable
	{
		event Action<IReferencable> OnDisposed;
		
		bool CanBeReferenced { get; }
	}
}