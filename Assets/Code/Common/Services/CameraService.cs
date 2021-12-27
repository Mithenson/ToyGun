using Cinemachine;
using UnityEngine;
using Zenject;

namespace ByteSize
{
	public class CameraService : MonoBehaviour, ICameraService
	{
		public Camera Source => _source;
		public CinemachineVirtualCamera Virtual => _virtual;
		public Vector3 TrackedPoint => _framingTransposer.TrackedPoint;
		
		[SerializeField]
		private Camera _source;

		[SerializeField]
		private CinemachineVirtualCamera _virtual;

		private CinemachineFramingTransposer _framingTransposer;
		
		[Inject]
		public void Inject([Inject(Id = LookupId.Player)] Entity player) => 
			_virtual.Follow = player[Root.Presentations];

		private void Awake() =>
			_framingTransposer = _virtual.GetCinemachineComponent<CinemachineFramingTransposer>();
	}
}