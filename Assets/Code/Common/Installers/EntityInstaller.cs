using Zenject;

namespace ByteSize
{
	public class EntityInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			var entity = GetComponent<Entity>();
			Container.BindInterfacesAndSelfTo(entity.GetType()).FromInstance(entity).AsSingle().NonLazy();
		}
	}
}