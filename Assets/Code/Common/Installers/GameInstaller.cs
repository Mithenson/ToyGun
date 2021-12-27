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

		[FoldoutGroup("Player"), SerializeField]
		private Entity _playerPrefab;
		
		[FoldoutGroup("Player"), SerializeField]
		private Transform _playerSpawnPoint;
		
		public override void InstallBindings()
		{
			var hierarchyService = new HierarchyService();
			Container.Bind<HierarchyService>().FromInstance(hierarchyService).AsSingle();
			
			Container.Bind<InputService>().FromNew().AsSingle().WithArguments(_inputCollection, _inputRepository);

			Container.Bind<Entity>()
			   .WithId(LookupId.Player)
			   .FromComponentInNewPrefab(_playerPrefab)
			   .UnderTransform(hierarchyService[HierarchyMark.Option.Entities])
			   .AsSingle()
			   .OnInstantiated((ctxt, value) =>
				{
					var transform = ((Entity)value).transform;
					transform.position = _playerSpawnPoint.position;
					transform.rotation = _playerSpawnPoint.rotation;
				})
			   .NonLazy();
			
			Container.BindInterfacesAndSelfTo<ICameraService>().FromInstance(_cameraService).AsSingle();
		}
		
		private void SetupApplication() => 
			Application.targetFrameRate = 60;
	}
}
