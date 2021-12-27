using Cinemachine;
using UnityEngine;

namespace ByteSize
{
	public interface ICameraService
	{
		Camera Source { get; }
		CinemachineVirtualCamera Virtual { get; }
		Vector3 TrackedPoint { get; }
	}
}