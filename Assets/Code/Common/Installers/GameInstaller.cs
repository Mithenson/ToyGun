using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ByteSize
{
	public class GameInstaller : MonoInstaller
	{
		[FoldoutGroup("Inputs"), SerializeField]
		private InputActionAsset _inputCollection;
		
		[FoldoutGroup("Inputs"), SerializeField]
		private InputRepository _inputRepository;
		
		[FoldoutGroup("Rendering"), SerializeField]
		private CameraService _cameraService;

		private AssetBundle _configurationsAssetBundle;
		 
		public override void InstallBindings()
		{
			_configurationsAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "configurations"));
			var configurations = _configurationsAssetBundle.LoadAllAssets<Configuration>();

			foreach (var configuration in configurations)
				Container.BindInterfacesAndSelfTo(configuration.GetType()).FromInstance(configuration).AsSingle();
			
			var hierarchyService = new HierarchyService();
			Container.Bind<HierarchyService>().FromInstance(hierarchyService).AsSingle();
			
			Container.Bind<InputService>().FromNew().AsSingle().WithArguments(_inputCollection, _inputRepository);

			Container.BindInterfacesAndSelfTo<CameraService>().FromInstance(_cameraService).AsSingle();

			var entityMarks = FindObjectsOfType<EntityMark>();
			foreach (var entityMark in entityMarks)
			{
				if (!entityMark.TryGetComponent(out Entity entity))
					continue;

				Container.Bind<Entity>().WithId(entityMark.Lookup).FromInstance(entity);
			}
		}
		
		private void SetupApplication() => 
			Application.targetFrameRate = 60;

		void OnDestroy() => 
			_configurationsAssetBundle.Unload(true);
	}
}
