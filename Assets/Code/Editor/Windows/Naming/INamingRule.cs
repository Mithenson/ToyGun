namespace ByteSize.Editor
{
	public interface INamingRule
	{
		bool IsActive { get; }

		void Draw();
		string Process(string name);
	}
}