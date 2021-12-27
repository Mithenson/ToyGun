namespace ByteSize.Editor
{
	public sealed class CapitalFirstCharacterNamingRule : StandardNamingRule
	{
		public override string Name => "Capital first character";
		
		protected override void DrawInScope() { }

		public override string Process(string name)
		{
			var firstChar = name[0];
			firstChar = char.ToUpper(firstChar);

			return name.Remove(0, 1).Insert(0, firstChar.ToString());
		}
	}
}