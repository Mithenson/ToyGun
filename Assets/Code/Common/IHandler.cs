namespace ByteSize
{
	public interface IHandler<TOperation>
		where TOperation : IOperation
	{
		int Priority { get; }
		
		bool Handle(ref TOperation operation);
	}
}