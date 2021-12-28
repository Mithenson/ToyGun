using MoreMountains.Feedbacks;
using UnityEngine;
using Zenject;

namespace ByteSize
{
	[AddComponentMenu(""), FeedbackPath("Byte size/Orthographic size")]
	public sealed class FeedbackOrthographicSize : FeedbackSingleModification<float>
	{
		#if UNITY_EDITOR

		public override Color FeedbackColor => MMFeedbacksInspectorColors.CameraColor;

		#endif
		
		private CameraService _cameraService;

		[Inject]
		public void Inject(CameraService cameraService) => 
			_cameraService = cameraService;

		protected override float GetStart() => 
			_cameraService.Virtual.m_Lens.OrthographicSize;

		protected override float Interpolate(float start, float destination, float interpolation) => 
			Mathf.Lerp(start, destination, interpolation);
		protected override void ProcessInterpolation(float interpolation) => 
			_cameraService.Virtual.m_Lens.OrthographicSize = interpolation;
	}
}