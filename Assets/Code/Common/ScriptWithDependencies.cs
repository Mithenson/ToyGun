namespace ByteSize
{
	public abstract class ScriptWithDependencies : Script
	{
		private Dependency[] _dependencies;
		
		public override void OnCreation()
		{
			base.OnCreation();
			_dependencies = CollectDependencies();
		}

		protected abstract Dependency[] CollectDependencies();

		protected override void OnDestroy()
		{
			base.OnDestroy();

			foreach (var dependency in _dependencies)
				dependency.Dispose();
		}
	}
}