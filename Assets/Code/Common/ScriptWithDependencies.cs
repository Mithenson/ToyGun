using Zenject;

namespace ByteSize
{
	public abstract class ScriptWithDependencies : Script
	{
		private Dependency[] _dependencies;

		[Inject]
		public void Inject(DiContainer container)
		{
			_dependencies = CollectDependencies();
			foreach (var dependency in _dependencies)
				container.Inject(dependency);
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