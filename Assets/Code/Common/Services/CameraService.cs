using Cinemachine;
using UnityEngine;
using Zenject;

namespace ByteSize
{
	public class CameraService : MonoBehaviour, ICameraService
	{
		public Camera Source => _source;
		public CinemachineVirtualCamera Virtual => _virtual;
		
		[SerializeField]
		private Camera _source;

		[SerializeField]
		private CinemachineVirtualCamera _virtual;
	}
}