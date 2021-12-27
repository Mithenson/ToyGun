using UnityEngine;
using Zenject;

namespace ByteSize
{
	public class ComponentInstaller : MonoInstaller
	{
		[SerializeField]
		private Component[] _components;
		
		public override void InstallBindings()
		{
			foreach (var component in _components)
				Container.BindInterfacesAndSelfTo(component.GetType()).FromInstance(component).AsSingle().NonLazy();
		}
	}
}